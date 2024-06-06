using AccountEx.Common;
using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    [Serializable]
    public partial class User : BaseEntity
    {

        public User()
        {
            UserRoles = new HashSet<UserRole>();
            //Company = new Company();
        }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string MHash { get; set; }
        public Nullable<DateTime> LastLogin { get; set; }
        public bool IsLive { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }
        public string RoleIds { get; set; }
        public byte UserType { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanChangeFiscal { get; set; }
        public bool IsDoctor { get; set; } = false;
        public Nullable<int> BranchId { get; set; }
        public string CellNo { get; set; }
        public string OfficeNo { get; set; }
        public bool? locked { get; set; }
        public Nullable<int> DomainId { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> RSMId { get; set; }
        public Nullable<CRMUserType> UserTypeId { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        //public virtual Company Company { get; set; }

        public string Designation { get; set; }
        public string Qualification { get; set; }



        public string GetFullName()
        {
            return (FirstName + " " + LastName).Trim();
        }

    }
}
