SELECT * FROM dbo.MenuItems WHERE CompanyId=91 AND Id=13222


DECLARE @MenuId INT=13219
DELETE FROM dbo.RoleAccesses WHERE MenuItemId IN(SELECT Id FROM dbo.MenuItems WHERE CompanyId=91 AND ParentMenuItemId=@MenuId)
AND CompanyId=91
DELETE FROM dbo.MenuItems WHERE CompanyId=91 AND ParentMenuItemId=@MenuId


DELETE FROM dbo.RoleAccesses WHERE MenuItemId IN(SELECT Id FROM dbo.MenuItems WHERE CompanyId=91 AND Id=@MenuId)
AND CompanyId=91
DELETE FROM dbo.MenuItems WHERE CompanyId=91 AND Id=@MenuId





DELETE FROM dbo.RoleAccesses WHERE MenuItemId IN(SELECT Id FROM dbo.MenuItems WHERE CompanyId=91 AND ParentMenuItemId=13172)
AND CompanyId=91