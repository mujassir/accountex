//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountEx.CodeFirst.Models.CRM
{
    using System;
    using System.Collections.Generic;

    public partial class vw_CRMCalendarEvents
    {
        public int Id { get; set; }
        public int VisitNo { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string Customer { get; set; }
        public string CustomerAddress { get; set; }
        public int SalePersonId { get; set; }
        public string SalePerson { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> ModeId { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> DivisionId { get; set; }


        public string Product { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string Region { get; set; }
        public string Mode { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public string GoogleCalendarEeventId { get; set; }
        public int? FiscalId { get; set; }
    }
}
