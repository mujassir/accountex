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

    public partial class vw_NexusDepartmentMapping
    {
        public int Id { get; set; }
        public Nullable<int> DepartmentAccountId { get; set; }
        public string AmexDepartment { get; set; }
        public Nullable<int> NexusDepartmentId { get; set; }
        public string NexusDepartmentName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
