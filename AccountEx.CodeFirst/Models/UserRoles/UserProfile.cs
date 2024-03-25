using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class UserProfile : BaseEntity
    {
        public UserProfile()
        {
            webpages_UsersInRoles = new List<webpages_UsersInRoles>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
    }
}
