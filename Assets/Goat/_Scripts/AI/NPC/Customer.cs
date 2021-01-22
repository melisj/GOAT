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
using ReadOnly = Sirenix.OdinInspector.ReadOnlyAttribute;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    /// <summary>
    /// Customer AI class that controlls customer behaviour
    /// </summary>
    public class Customer : NPC, IAtomListener<bool>
    {
        // Choosen for player money instead of grocery amount because money gives a more dynamic way of handeling groceries and buying behaviour.
        [SerializeField, TabGroup("Settings")] private float maxSearchingTime = 60;
        [SerializeField, ReadOnly, TabGroup("Settings")] private int storeArea;
        [SerializeField, TabGroup("References")] private CustomerReview review;
        [SerializeField, TabGroup("References")] private CustomerFeelings feelings;
        [SerializeField, TabGroup("References")] private IntVariable customerCapacity;
        [SerializeField, TabGroup("References")] private UnloadLocations entrances;
        [SerializeField, TabGroup("References")] private BoolEvent onDay;
        [SerializeField, TabGroup("References")] private AudioCue angry, questioning, checkout;
        [SerializeField, TabGroup("References")] private ResourceArray resourcesInProject;
        [SerializeField, TabGroup("References")] private FieldOfView fov;
        [SerializeField, TabGroup("Debug"), ReadOnly] private int money;
        [SerializeField, TabGroup("Debug"), ReadOnly] private int remainingMoney;
        [SerializeField, TabGroup("Debug"), ReadOnly] private bool leavingStore;
        [SerializeField, TabGroup("Debug"), ReadOnly] private float totalPriceProducts;
        [SerializeField, TabGroup("Debug"), ReadOnly] private float customerSelfConstraint = 0;

        //Bool Functions
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool calculatedGroceries;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool enteredStore;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasStorageTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool stuckForSeconds;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool storageDepleted;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool checkoutFull;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool goToCheckout;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool leaveStore;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool findShortestCheckoutQueue;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool askForHelp;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool waitingInQueue;

        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "ExitStore")] private ExitStore exitStore;
        public FieldOfView Fov => fov;
        public int Money { get => money; set => money = value; }
        public int RemainingMoney { get => remainingMoney; set => remainingMoney = value; }
        public int AmountGroceries { get; set; }
        public bool OutofTime => SearchingTime >= maxSearchingTime && Inventory.ItemsInInventory == 0;
        public float TotalPriceProducts { get => totalPriceProducts; set => totalPriceProducts = value; }
        public bool LeavingStore { get => leavingStore; set => leavingStore = value; }
        public bool EnteredStore { get => enteredStore; set => enteredStore = value; }
        public float CustomerSelfConstraint { get => customerSelfConstraint; set => customerSelfConstraint = value; }

        protected override void Setup()
        {
            base.Setup();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            EnterStoreCustomer enterStore = new EnterStoreCustomer(this, NavMeshAgent, entrances, customerCapacity);
            SetRandomDestination SetRandomDestination = new SetRandomDestination(this, NavMeshAgent, storeArea, questioning);
            SearchForCheckout searchForCheckout = new SearchForCheckout(this);
            exitStore = new ExitStoreCustomer(this, NavMeshAgent, review, customerCapacity, checkout, angry);
            DoNothing doNothing = new DoNothing(this);

            // Conditions

            // Groceries
            // When groceries are calculated inside the CalculateGroceries state
            Func<bool> CalculatedGroceries() => () =>
            {
                calculatedGroceries = calculateGroceries.calculatedGroceries;
                return calculatedGroceries;
            };
            // When customer has entered the store
            Func<bool> EnteredStore() => () =>
            {
                enteredStore = enterStore.enteredStore;
                return enteredStore;
            };

            // Movement
            // When customer has found a storage to take groceries from and the customer is not leaving the store
            Func<bool> HasStorageTarget() => () =>
            {
                hasStorageTarget = TargetStorage != null && !LeavingStore;
                return hasStorageTarget;
            };
            // When the customer has a new destination to move to
            Func<bool> HasDestination() => () =>
            {
                hasDestination = Vector3.Distance(transform.position, TargetDestination) >= NpcSize / 2 && TargetStorage == null && TargetDestination != Vector3.zero;
                return hasDestination;
            };
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () =>
            {
                stuckForSeconds = moveToDestination.AmountStuckCalled > 3 || moveToTarget.AmountStuckCalled > 3;
                return stuckForSeconds;
            };
            // When the customer has arrived at its destination
            Func<bool> ReachedDestination() => () =>
            {
                reachedDestination = NavMeshAgent.remainingDistance < NpcSize / 2 && TargetStorage == null && !searchForCheckout.inQueue && !LeavingStore;
                return reachedDestination;
            };
            // When the customer has arrived at a target
            Func<bool> ReachedTarget() => () =>
            {
                reachedTarget = NavMeshAgent.remainingDistance < NpcSize / 2 && TargetStorage != null;
                return reachedTarget;
            };

            // Shopping
            // When the storage the customer is taking items from no longer has any of the items the customer wants
            Func<bool> StorageDepleted() => () =>
            {
                storageDepleted = TakeItem.Depleted;
                return storageDepleted;
            };
            // When the customer wants to pay for groceries but the queue for the checkout is to long
            Func<bool> CheckoutFull() => () =>
            {
                checkoutFull = LeavingStore && TargetDestination == Vector3.zero;
                return checkoutFull;
            };
            // When there is room in the checkout queue
            Func<bool> GoToCheckout() => () =>
            {
                goToCheckout = (SearchingTime >= maxSearchingTime || (ItemsToGet.ItemsInInventory == 0 && enterStore.enteredStore))
                                && Inventory.ItemsInInventory > 0
                                && searchForCheckout.checks < 1
                                && NavMeshAgent.remainingDistance < 1;
                return goToCheckout;
            };
            // When the customer can't find any of his groceries
            Func<bool> LeaveStore() => () =>
            {
                leaveStore = SearchingTime >= maxSearchingTime && Inventory.ItemsInInventory == 0;
                return leaveStore;
            };
            // When the customer arrives at the checkout it needs to find the first available position in the queue
            Func<bool> FindShortestCheckoutQueue() => () =>
            {
                findShortestCheckoutQueue = NavMeshAgent.remainingDistance < 4 && (searchForCheckout.checks < 2 && searchForCheckout.checks > 0);
                return findShortestCheckoutQueue;
            };

            // Interaction
            Func<bool> AskForHelp() => () =>
            {
                askForHelp = ItemsToGet.ItemsInInventory > 0 && SearchingTime >= maxSearchingTime;
                return askForHelp;
            };
            // Checkout
            Func<bool> WaitingInQueue() => () =>
            {
                waitingInQueue = searchForCheckout.inQueue && NavMeshAgent.remainingDistance < 0.1f;
                return waitingInQueue;
            };

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(calculateGroceries, enterStore, CalculatedGroceries());
            AT(enterStore, SetRandomDestination, EnteredStore());
            AT(SetRandomDestination, moveToDestination, HasDestination());
            AT(moveToDestination, SetRandomDestination, StuckForSeconds());
            AT(moveToDestination, SetRandomDestination, ReachedDestination());
            AT(moveToTarget, TakeItem, ReachedTarget());
            AT(TakeItem, SetRandomDestination, StorageDepleted());
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
            TargetDestination = newPosition;
            stateMachine.SetState(moveToDestination);
        }

        private void OnEnable()
        {
            onDay.RegisterSafe(this);
        }

        public override void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            SearchingTime = 0;
            EnteredStore = false;
            LeavingStore = false;
            Money = UnityEngine.Random.Range(1, 3) * 100;
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