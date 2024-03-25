
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class vw_ProductReceipes
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public decimal Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }

    }
}
