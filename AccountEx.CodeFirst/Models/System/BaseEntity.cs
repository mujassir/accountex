using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AccountEx.CodeFirst.Models
{
    
    [Serializable]
    public partial class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
       
        
        [NeverUpdate] 
        public virtual DateTime CreatedAt { get; set; }
        
        
        
        [NeverUpdate]  
        public virtual int CreatedBy { get; set; }


        
        public virtual Nullable<DateTime> ModifiedAt { get; set; }

       
        public virtual Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [NeverUpdate]
        public virtual Nullable<int> CompanyId { get; set; }
    }

   
    
}
