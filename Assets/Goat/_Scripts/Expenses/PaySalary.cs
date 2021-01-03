using UnityEngine;
using System.Collections;
using Goat.AI.Parking;

namespace Goat.Expenses
{
    public class PaySalary : PayExpense
    {
        [SerializeField] private HiredEmployees employees;

        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay()
        {
            var looper = employees.EmployeeList.GetEnumerator();
            while (looper.MoveNext())
            {
                looper.Current.AmountPaid = looper.Current.Amount;
            }
        }

        public override void Pay()
        {
            var looper = employees.EmployeeList.GetEnumerator();
            int remainingPrice = 0;

            while (looper.MoveNext())
            {
                looper.Current.AmountPaid = 0;
                for (int i = 0; i < looper.Current.Amount; i++)
                {
                    if (money.CanPay(looper.Current.Salary))
                    {
                        looper.Current.AmountPaid++;
                        money.Amount -= looper.Current.Salary;
                    }
                    else
                    {
                        //Can't pay so make an expense and send it to an expenses holder
                        remainingPrice += looper.Current.Salary;
                    }
                }
            }

            expenseEvent.Raise(new Expense(remainingPrice, "Salary", OnFullPay));
        }
    }
}