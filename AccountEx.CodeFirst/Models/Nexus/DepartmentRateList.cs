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
    
    public partial class DepartmentRateList:BaseEntity
    {
        public DepartmentRateList()
        {
            this.DepartmentRateListItems = new HashSet<DepartmentRateListItem>();
        }
    
        public int DepartmentAccountId { get; set; }
        public string Name { get; set; }
        public System.DateTime WithEffectFrom { get; set; }
        public virtual ICollection<DepartmentRateListItem> DepartmentRateListItems { get; set; }
    }
}
