using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class ServiceExpense : BaseEntity
    {

        public int SaleId { get; set; }
        public Nullable<int> SaleItemId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        //public Nullable<int> ServiceItemId { get; set; }
        public int ServiceExpensesItemId { get; set; }
        public string ServiceExpenseName { get; set; }
        public string ServiceExpenseCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }

    }
}
