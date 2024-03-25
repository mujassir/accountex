using System;
namespace AccountEx.Common.CRM
{
    public class CRMComplaintExtra
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> VisitNo { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> ModeId { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string Project { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Product { get; set; }
        public Nullable<int> ProductId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Nullable<int> CompanyId { get; set; }
    }
}
