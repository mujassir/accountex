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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class vw_CRMPRoducts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HSCode { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Category { get; set; }
        public Nullable<int> SecCategoryId { get; set; }
        public string SecCategory { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public string Division { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string Vendor { get; set; }
        public string Supplier { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public Nullable<int> SubGroupId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public bool IsOwnProduct { get; set; }
        public bool IsDeleted { get; set; }
    }
}