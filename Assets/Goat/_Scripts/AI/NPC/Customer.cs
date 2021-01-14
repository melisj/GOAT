using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.AI.States;
using Goat.Storage;
using Goat.Grid.Interactions;
using Goat.AI.Satisfaction;
using Goat.Pooling;
using Goat.ScriptableObjects;
using Goat.AI.Parking;
using Goat.AI.Feelings;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace Goat.AI
{
    /// <summary>
    /// Customer AI class that controlls customer behaviour
    /// </summary>
    public class Customer : NPC, IAtomListener<bool>
    {
        // Choosen for player money instead of grocery amount because money gives a more dynamic way of handeling groceries and buying behaviour.
        [SerializeField] private float maxSearchingTime = 60;
        [SerializeField] private CustomerReview review;
        [SerializeField] private CustomerFeelings feelings;
        [SerializeField] private IntVariable customerCapacity;
        [SerializeField] private UnloadLocations entrances;
        [SerializeField] private BoolEvent onDay;
        [SerializeField] private int storeArea;
        [SerializeField] private AudioCue angry, questioning, checkout;
        public int money = 0;
        [HideInInspector] public int remainingMoney = 0;

        [SerializeField] private ResourceArray resourcesInProject;

        public float customerSatisfaction = 100;
        [HideInInspector] public float customerSelfConstraint = 0;
        [SerializeField] private FieldOfView fov;
        public FieldOfView Fov => fov;
        public int AmountGroceries { get; set; }
        public bool OutofTime => searchingTime >= maxSearchingTime && Inventory.ItemsInInventory == 0;

        [HideInInspector] public bool enteredStore;
        [HideInInspector] public bool leavingStore;

        [HideInInspector] public float totalPriceProducts;

        private ExitStore exitStore;

        protected override void Setup()
        {
            base.Setup();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            EnterStoreCustomer enterStore = new EnterStoreCustomer(this, navMeshAgent, entrances, customerCapacity);
            SetRandomDestination SetRandomDestination = new SetRandomDestination(this, navMeshAgent, storeArea, questioning);
            SearchForCheckout searchForCheckout = new SearchForCheckout(this);
            exitStore = new ExitStoreCustomer(this, navMeshAgent, review, customerCapacity, checkout, angry);
            DoNothing doNothing = new DoNothing(this);

            // Conditions

            // Groceries
            // When groceries are calculated inside the CalculateGroceries state
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            // When customer has entered the store
            Func<bool> EnteredStore() => () => enterStore.enteredStore;

            // Movement
            // When customer has found a storage to take groceries from and the customer is not leaving the store
            Func<bool> HasStorageTarget() => () => targetStorage != null && !leavingStore;
            // When the customer has a new destination to move to
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize / 2 && targetStorage == null && targetDestination != Vector3.zero;
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () => moveToDestination.amountStuckCalled > 3 || moveToTarget.amountStuckCalled > 3;
            // When the customer has arrived at its destination
            Func<bool> ReachedDestination() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage == null && !searchForCheckout.inQueue && !leavingStore;
            // When the customer has arrived at a target
            Func<bool> ReachedTarget() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;

            // Shopping
            // When the storage the customer is taking items from no longer has any of the items the customer wants
            Func<bool> StorageDepleted() => () => takeItem.depleted;
            // When the customer wants to pay for groceries but the queue for the checkout is to long
            Func<bool> CheckoutFull() => () => leavingStore && targetDestination == Vector3.zero;
            // When there is room in the checkout queue
            Func<bool> GoToCheckout() => () => (searchingTime >= maxSearchingTime || (ItemsToGet.ItemsInInventory == 0 && enterStore.enteredStore)) && Inventory.ItemsInInventory > 0 && searchForCheckout.checks < 1 && navMeshAgent.remainingDistance < 1;
            // When the customer can't find any of his groceries
            Func<bool> LeaveStore() => () => searchingTime >= maxSearchingTime && Inventory.ItemsInInventory == 0;
            // When the customer arrives at the checkout it needs to find the first available position in the queue
            Func<bool> FindShortestCheckoutQueue() => () => navMeshAgent.remainingDistance < 4 && (searchForCheckout.checks < 2 && searchForCheckout.checks > 0);
            //Func<bool> ArrivedAtCheckout() => () => itemsToGet.Count == 0 && Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage == null;

            // Interaction
            Func<bool> AskForHelp() => () => ItemsToGet.ItemsInInventory > 0 && searchingTime >= maxSearchingTime;
            // Checkout
            Func<bool> WaitingInQueue() => () => searchForCheckout.inQueue && navMeshAgent.remainingDistance < 0.1f;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(calculateGroceries, enterStore, CalculatedGroceries());
            AT(enterStore, SetRandomDestination, EnteredStore());
            AT(SetRandomDestination, moveToDestination, HasDestination());
            AT(moveToDestination, SetRandomDestination, StuckForSeconds());
            AT(moveToDestination, SetRandomDestination, ReachedDestination());
            AT(moveToTarget, takeItem, ReachedTarget());
            AT(takeItem, SetRandomDestination, StorageDepleted());
            AT(moveToDestination, moveToTarget, HasStorageTarget());
            AT(SetRandomDestination, moveToTarget, HasStorageTarget());

            AT(moveToDestination, searchForCheckout, GoToCheckout());
            AT(searchForCheckout, moveToDestination, HasDestination());
            AT(moveToDestination, searchForCheckout, FindShortestCheckoutQueue());
            AT(moveToDestination, exitStore, LeaveStore());

            AT(moveToDestination, doNothing, WaitingInQueue());
            AT(searchForCheckout, SetRandomDestination, CheckoutFull());

            stateMachine.SetState(calculateGroceries);

            fov = GetComponentInChildren<FieldOfView>();
            feelings.Setup();
        }

        public void UpdatePositionInCheckoutQueue(Vector3 newPosition)
        {
            targetDestination = newPosition;
            stateMachine.SetState(moveToDestination);
        }

        private void OnEnable()
        {
            onDay.RegisterSafe(this);
        }

        public override void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            searchingTime = 0;
            enteredStore = false;
            leavingStore = false;
            money = UnityEngine.Random.Range(1, 3) * 100;
            base.OnGetObject(objectInstance, poolKey);
        }

        private void OnDisable()
        {
            onDay.UnregisterSafe(this);
        }

        public override void OnReturnObject()
        {
            feelings.OnReturn();

            base.OnReturnObject();
        }

        public void LeaveStore()
        {
            stateMachine.SetState(exitStore);
        }

        public void OnEventRaised(bool isDay)
        {
            if (!isDay && stateMachine.CurrentState != exitStore)
                LeaveStore();
        }
    }
}