using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AccountEx.CodeFirst.Models
{
    [Serializable]
    public partial class GlobalBaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
       
        [NeverUpdate] [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        
        
        [NeverUpdate]  [JsonIgnore]
        public int CreatedBy { get; set; }


        [JsonIgnore]
        public Nullable<DateTime> ModifiedAt { get; set; }
        
        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
