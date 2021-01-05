using UnityEngine;
using System.Collections;
using Goat.AI.Parking;
using System.Collections.Generic;

namespace Goat.Expenses
{
    public class PaySalary : PayExpense<List<EmployeeNotPaid>>
    {
        [SerializeField] private HiredEmployees employees;
        private readonly List<EmployeeNotPaid> employeesNotPaid = new List<EmployeeNotPaid>();

        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay(List<EmployeeNotPaid> employeesNotPaid)
        {
            for (int i = 0; i < employeesNotPaid.Count; i++)
            {
                HiredEmployee employee = employeesNotPaid[i].HiredEmployee;
                int amountPaid = employeesNotPaid[i].AmountPaid;

                employee.AmountPaid = Mathf.Min(amountPaid + employee.AmountPaid, employee.Amount);
            }
        }

        public override void Pay()
        {
            var looper = employees.EmployeeList.GetEnumerator();
            int remainingPrice = 0;
            int fullPrice = 0;
            employeesNotPaid.Clear();

            while (looper.MoveNext())
            {
                HiredEmployee employee = looper.Current;

                employee.AmountPaid = 0;
                for (int i = 0; i < employee.Amount; i++)
                {
                    fullPrice += employee.Salary;
                    if (money.CanPay(employee.Salary))
                    {
                        employee.AmountPaid++;
                        money.Amount -= employee.Salary;
                    }
                    else
                    {
                        //Can't pay so make an expense and send it to an expenses holder
                        remainingPrice += employee.Salary;
                    }
                }

                if (employee.AmountPaid < employee.Amount)
                {
                    employeesNotPaid.Add(new EmployeeNotPaid(employee, employee.Amount - employee.AmountPaid));
                }
            }
            onExpenseCreated.Raise(fullPrice);
            expenseEvent.Raise(new Expense(remainingPrice, "Salary", time.GetDate(), () => OnFullPay(employeesNotPaid)));
        }
    }
}