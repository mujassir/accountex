using System;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class vw_Vouchers
    {
        public int Id { get; set; }
        public int VoucherNumber { get; set; }
        public int BranchId { get; set; }
        public VoucherType TransactionType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Comments { get; set; }
        public string Username { get; set; }
        public int CompanyId { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}
