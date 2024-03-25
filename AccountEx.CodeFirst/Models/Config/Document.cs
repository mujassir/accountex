using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    [Table("Documents")]
    public partial class UploadDocument : BaseEntity
    {
       
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }

    }
}
