namespace AccountEx.CodeFirst.Models.COA
{
    using System;
    using System.Collections.Generic;

    public partial class Shop : BaseEntity
    {

        public Shop()
        {
            this.ShopDocuments = new HashSet<ShopDocument>();
        }

        public string ShopCode { get; set; }
        public string ShopNo { get; set; }
        public string Type { get; set; }

        public string Block { get; set; }
        public int BlockId { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> TotalArea { get; set; }

        public Nullable<decimal> GroundFloor { get; set; }
        public Nullable<decimal> InterFlooring { get; set; }
        public Nullable<decimal> Basement { get; set; }
        public Nullable<decimal> FirstFloor { get; set; }

        public string North { get; set; }
        public string East { get; set; }
        public string West { get; set; }
        public string South { get; set; }
        public string FileUrl { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<ShopDocument> ShopDocuments { get; set; }
    }
}
