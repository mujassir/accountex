using System.Collections.Generic;
using System;
using AccountEx.CodeFirst.Models;


namespace AccountEx.DbMapping
{
    [Serializable]
    public class MenuItemExtra : MenuItem
    {
        public List<MenuItemExtra> SubMenues { get; set; }
        public MenuItemExtra()
        {
            SubMenues = new List<MenuItemExtra>();
        }
        public MenuItemExtra(MenuItem m)
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
            SubMenues = new List<MenuItemExtra>();
        }
    }
}
