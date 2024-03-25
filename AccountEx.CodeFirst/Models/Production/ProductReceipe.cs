
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ProductReceipe : BaseEntity
    {

        public ProductReceipe()
        {
            ProductReceipeitems = new HashSet<ProductReceipeitems>();

        }


        public int ProductId { get; set; }
        public byte Type { get; set; }
        public decimal Quantity { get; set; }
        public virtual ICollection<ProductReceipeitems> ProductReceipeitems { get; set; }

    }
}
