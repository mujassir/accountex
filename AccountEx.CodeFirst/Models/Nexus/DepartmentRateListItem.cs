//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountEx.CodeFirst.Models.Nexus
{
    using System;
    using System.Collections.Generic;

    public partial class DepartmentRateListItem : BaseEntity
    {
        public int DepartmentId { get; set; }
        public int DepartmentRateListId { get; set; }
        public int TestId { get; set; }
        public decimal ApprovedRate { get; set; }
    }
}
