using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Account : BaseEntity
    {
        //public Account()
        //{
        //    this.AccountDetail = new AccountDetail();
        //}
        public Nullable<int> AccountTypeId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string AccountCode { get; set; }
        public byte Level { get; set; }
        public bool HasChild { get; set; }
        public bool IsLive { get; set; }
        public bool IsSystemAccount { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        //[ForeignKey("AccountId")]
        //public virtual AccountDetail AccountDetail { get; set; }
    }
}
