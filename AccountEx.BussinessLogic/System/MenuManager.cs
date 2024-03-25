using AccountEx.Common;
using AccountEx.Repositories;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.BussinessLogic
{
    public static class MenuManager
    {
        public static List<SiteContextMenuItemExtra> GetMenuItems()
        {
            if (SiteContext.Current.MenuItems != null) return SiteContext.Current.MenuItems;

            var menuItems = new MenuItemRepository().GetAll().Select(p => new SiteContextMenuItemExtra(UtilityFunctionManager.GetMenuItemForSiteContext(p))).ToList();
            if (!SiteContext.Current.User.IsAdmin)
            {

                var access = new RoleAccessRepository().GetByRoleId(SiteContext.Current.UserRoles);
                menuItems = menuItems.Where(p => access.Any(q => q.MenuItemId == p.Id && q.CanView)).ToList();
            }
            foreach (var item in menuItems.Where(p => p.ParentMenuItemId == 0).OrderBy(p => p.SequenceNumber))
            {
                GetChildren(menuItems, item);
            }
            // var list = menuItems.Where(p => p.ParentMenuItemId == 0).ToList();
            var list = menuItems.Where(p => p.ParentMenuItemId == 0).OrderBy(p => p.SequenceNumber).ToList();
            SiteContext.Current.MenuItems = list;
            return list;
        }
        public static List<SiteContextMenuItemExtra> GetMenuItems(bool appyUserRoles)
        {
            List<SiteContextMenuItemExtra> menuItems;
            if (appyUserRoles)
            {
                if (SiteContext.Current.MenuItems != null) return SiteContext.Current.MenuItems;
                menuItems = new MenuItemRepository().GetAll().Select(p => new SiteContextMenuItemExtra(UtilityFunctionManager.GetMenuItemForSiteContext(p))).ToList();
                var userRoles = new UserRoleRepository().GetRoles(SiteContext.Current.User.Id);
                var access = new RoleAccessRepository().GetByRoleId(userRoles);
                menuItems = menuItems.Where(p => access.Any(q => q.MenuItemId == p.Id && q.CanView)).ToList();
            }
            else
                menuItems = new MenuItemRepository().GetAll().Select(p => new SiteContextMenuItemExtra(UtilityFunctionManager.GetMenuItemForSiteContext(p))).ToList();
            foreach (var item in menuItems.Where(p => p.ParentMenuItemId == 0).OrderBy(p => p.SequenceNumber))
            {
                GetChildren(menuItems, item);
            }
            var list = menuItems.Where(p => p.ParentMenuItemId == 0).ToList();
            SiteContext.Current.MenuItems = list;
            return list;
        }
        public static void GetChildren(List<SiteContextMenuItemExtra> items, SiteContextMenuItemExtra parent)
        {
            foreach (var item in items.Where(p => p.ParentMenuItemId == parent.Id).OrderBy(p => p.SequenceNumber))
            {
                GetChildren(items, item);
                parent.SubMenues.Add(item);
            }
        }
        public static RoleAccess GetMenuAccess(string url)
        {
            var menuItemId = GetMenuItemId(GetMenuItems(), url.ToLower());
            return menuItemId == 0 ? null : GetMenuAccess(menuItemId);
        }
        public static int GetMenuItemId(List<SiteContextMenuItemExtra> menuItems, string url)
        {
            foreach (var item in menuItems)
            {
                var urlWithoutSlash = url.TrimStart('/');
                if (item.Url != null && (item.Url.ToLower() == ".." + url
                    || item.Url.ToLower() == "~" + url || item.Url.ToLower().Contains(url + "?") || item.Url.ToLower() == urlWithoutSlash || item.Url.ToLower() == url)) return item.Id;
                if (item.SubMenues == null || item.SubMenues.Count <= 0) continue;
                var id = GetMenuItemId(item.SubMenues, url);
                if (id > 0) return id;
            }
            return 0;
        }
        public static RoleAccess GetMenuAccess(int menuItemId)
        {
            return new RoleAccessRepository().GetByMenuItemId(SiteContext.Current.UserRoles, menuItemId);
        }
        public static void RefreshMenu()
        {
            SiteContext.Current.MenuItems = null;
            GetMenuItems();

        }
    }
}
