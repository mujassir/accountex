using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class SiteContextExtra
    {



    }
    public class SiteContexFiscal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsDefault { get; set; }
        public bool IsClosed { get; set; }
        public virtual int CompanyId { get; set; }
    }
    public class SiteContexUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanChangeFiscal { get; set; }
        public Nullable<int> BranchId { get; set; }
        public virtual int CompanyId { get; set; }
    }
    public class SiteContexRoleAccess
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuItemId { get; set; }
        public bool CanView { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanCreate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAuthorize { get; set; }
        public virtual int CompanyId { get; set; }
    }
    public class SiteContexLogMapping
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string LogKey { get; set; }
        public byte LogType { get; set; }
        public string Description { get; set; }
        public string ModuleKey { get; set; }
        public virtual int CompanyId { get; set; }
    }

    public class SiteContextMenuItemExtra
    {
        public int Id { get; set; }
        public int ParentMenuItemId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public bool IsMegaMenu { get; set; }
        public bool HasChild { get; set; }
        public byte SequenceNumber { get; set; }
        public bool IsVisible { get; set; }
        public string DataType { get; set; }
        public virtual int CompanyId { get; set; }
        public List<SiteContextMenuItemExtra> SubMenues { get; set; }
        public SiteContextMenuItemExtra()
        {
            SubMenues = new List<SiteContextMenuItemExtra>();
        }
        public SiteContextMenuItemExtra(SiteContextMenuItemExtra m)
        {
            HasChild = m.HasChild;
            IconClass = m.IconClass;
            Id = m.Id;
            ParentMenuItemId = m.ParentMenuItemId;
            SequenceNumber = m.SequenceNumber;
            Title = m.Title;
            Url = m.Url;
            DataType = m.DataType;
            IsVisible = m.IsVisible;
            CompanyId = m.CompanyId;
            IsMegaMenu = m.IsMegaMenu;
            SubMenues = new List<SiteContextMenuItemExtra>();
        }
    }

}
