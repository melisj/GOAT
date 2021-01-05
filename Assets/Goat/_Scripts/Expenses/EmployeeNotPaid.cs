using Goat.AI.Parking;

namespace Goat.Expenses
{
    public class EmployeeNotPaid
    {
        public EmployeeNotPaid(HiredEmployee hiredEmployee, int amountPaid)
        {
            HiredEmployee = hiredEmployee;
            AmountPaid = amountPaid;
        }

        public HiredEmployee HiredEmployee { get; set; }
        public int AmountPaid { get; set; }
    }
}