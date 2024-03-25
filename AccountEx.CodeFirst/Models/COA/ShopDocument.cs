using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class ShopDocument : BaseEntity
    {
        public Nullable<int> ShopId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }
    }
}
