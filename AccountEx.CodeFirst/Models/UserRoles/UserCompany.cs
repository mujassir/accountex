using Newtonsoft.Json;

namespace AccountEx.CodeFirst.Models
{
    public class UserCompany: BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public int AuthCompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
