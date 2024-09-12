using Newtonsoft.Json;

namespace AccountEx.CodeFirst.Models
{
    public class UserLocation: BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public int AuthLocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
