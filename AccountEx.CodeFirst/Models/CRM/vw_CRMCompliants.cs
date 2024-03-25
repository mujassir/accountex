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
    using Common;
    using System;
    using System.Collections.Generic;

    public partial class vw_CRMCompliants
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public int VoucherNo { get; set; }
        public string Customer { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Project { get; set; }
        public decimal Price { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string Product { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public string Vendor { get; set; }

        
        public Nullable<int> ActualProductId { get; set; }
        public string ActualProduct { get; set; }
        public string ActualProductDivision { get; set; }
        public string CaseType { get; set; }
        public Nullable<int> CaseTypeId { get; set; }
        public string Priority { get; set; }
        public int PriorityId { get; set; }
        public string TestType { get; set; }
        public string TestTypeId { get; set; }
        public string Description { get; set; }
        public string Lab { get; set; }
        public int LabId { get; set; }
        public string Status { get; set; }
        public CRMComplaintStatus StatusId { get; set; }
        public string CounterProductIds { get; set; }
        public string CreateByUser { get; set; }
        
             public string AssignedToUser { get; set; }
        public Nullable<int> AssignedToId { get; set; }
        public string ResolvedByUser { get; set; }
        public Nullable<int> ResolvedById { get; set; }
        public Nullable<System.DateTime> ResolvedDate { get; set; }
        public Nullable<int> ClosedById { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }

        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
    }
}