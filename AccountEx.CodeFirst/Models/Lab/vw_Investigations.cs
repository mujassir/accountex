using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AccountEx.CodeFirst.Models.Lab
{
    public partial class vw_Investigations
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public int MainCategoryId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        
        public int CompanyId { get; set; }
    }
}
