using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountEx.CodeFirst.Models
{
    public partial class vw_ProductGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DivisionId { get; set; }
        public string Division { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Nullable<int> CompanyId { get; set; }
    }
}
