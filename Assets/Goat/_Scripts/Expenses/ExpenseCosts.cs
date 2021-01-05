using UnityEngine;

namespace Goat.Expenses
{
    [CreateAssetMenu(fileName = "ExpenseCosts", menuName = "ScriptableObjects/GlobalVariables/ExpenseCosts")]
    public class ExpenseCosts : ScriptableObject
    {
        [SerializeField] private int rentPrice;
        [SerializeField] private int taxPercentage;
        [SerializeField] private int powerCost;
        public int RentPrice => rentPrice;
        public int TaxPercentage => taxPercentage;
        public int PowerCost => powerCost;
    }
}