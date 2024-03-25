namespace AccountEx.CodeFirst.Models
{
    public partial class RoleAccess : BaseEntity
    {
        public int RoleId { get; set; }
        public int MenuItemId { get; set; }
        public bool CanView { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanCreate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAuthorize { get; set; }
    }
}
