using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class SaleDocument : BaseEntity
    {

        public int SaleId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }

    }
}
