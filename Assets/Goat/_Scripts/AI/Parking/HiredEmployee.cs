using Goat.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.AI.Parking
{
    [CreateAssetMenu(fileName = "HiredEmployee", menuName = "ScriptableObjects/Buyable/HiredEmployee")]
    public class HiredEmployee : Buyable
    {
        [SerializeField, Title("Employee data")] private int salary;
        [SerializeField] private GameObject prefab;
        [SerializeField] private HiredEmployeeEvent hiredEmployeeEvent;
        [SerializeField] private int amountPaid;

        public override void Buy(int amount, float price = -1, bool payNow = true, bool deliverNow = true)
        {
            base.Buy(amount, price, payNow, deliverNow);
            hiredEmployeeEvent.Raise(this);
        }

        public int Salary { get => salary; set => salary = value; }
        public GameObject Prefab { get => prefab; set => prefab = value; }
        public int AmountPaid { get => amountPaid; set => amountPaid = value; }
    }
}