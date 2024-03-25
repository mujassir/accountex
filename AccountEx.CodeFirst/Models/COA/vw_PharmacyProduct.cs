using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountEx.CodeFirst.Models
{
    [Table("vw_PharmacyProducts")]
    public partial class vw_PharmacyProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Generic { get; set; }
        public string Brand { get; set; }
        public string PackagingType { get; set; }
        public string Location { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Nullable<int> CompanyId { get; set; }
    }
}
