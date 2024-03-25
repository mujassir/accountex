namespace AccountEx.CodeFirst.Models
{
    public partial class RoleAction : BaseEntity
    {
        public int RoleId { get; set; }
        public int ActionId { get; set; }
        public bool Allowed { get; set; }
    }
}
