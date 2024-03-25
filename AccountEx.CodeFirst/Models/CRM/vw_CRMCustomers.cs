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
    
    public partial class vw_CRMCustomers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string Region { get; set; }


        
        public Nullable<int> UOMId { get; set; }
        public string Capicty { get; set; }
        public Nullable<int> IndustryTypeId { get; set; }
        public string Industry { get; set; }
        public string EmailDomain { get; set; }
        public string Fax { get; set; }
        public string NTN { get; set; }
        public string GSTRN { get; set; }
        public string CellNo { get; set; }
        public string OfficeNo { get; set; }
        public string HomeNo { get; set; }
        public string Email { get; set; }
        public string SecondayEmail { get; set; }
        public string Web { get; set; }
        public string Company { get; set; }
        public string ShippingAddress { get; set; }
        public Nullable<int> ShippingProvinceId { get; set; }
        public Nullable<int> ShippingRegionId { get; set; }
        public string BillingAddress { get; set; }
        public Nullable<int> BillingProvinceId { get; set; }
        public Nullable<int> BillingRegionId { get; set; }
        public string ImportAddress { get; set; }
        public Nullable<int> ImportProvinceId { get; set; }
        public Nullable<int> ImportRegionId { get; set; }
        public bool IsAllowPortal { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrader { get; set; }
        public bool IsDeleted { get; set; }

        public int CompanyId { get; set; }
    }
}
