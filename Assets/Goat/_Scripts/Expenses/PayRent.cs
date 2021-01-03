using Goat.Events;
using System.Collections;
using System.Collections.Generic;

namespace Goat.Expenses
{
    public class PayRent : PayExpense
    {
        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay()
        {
            throw new System.NotImplementedException();
        }

        public override void Pay()
        {
        }
    }
}