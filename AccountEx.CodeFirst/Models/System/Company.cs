using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Company : BaseEntity
    {

        public Nullable<int> ParentCompanyId { get; set; }
        public Nullable<int> DemoCompanyId { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string UploadFolder { get; set; }
        public string InfoText { get; set; }
        public bool AllowOtherLogin { get; set; }
        public bool CanCreateChild { get; set; }
        public bool CanBeParent { get; set; }
        public Nullable<int> OldId { get; set; }
    }
}
