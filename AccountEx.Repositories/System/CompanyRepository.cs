using System;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System.Collections.Generic;
using EntityFramework.Extensions;
using AccountEx.CodeFirst.Context;
using EntityFramework.BulkInsert.Extensions;
using System.Data.Entity.Infrastructure;
using AccountEx.DbMapping;
using System.Linq.Expressions;
namespace AccountEx.Repositories
{
    public class CompanyRepository
    {
        public AccountExContext Db = null;
        public CompanyRepository()
        {
            Db = Connection.GetContext();

        }
        public List<Company> GetAll()
        {
            return Db.Companies.ToList();
        }
        public List<Company> GetAll(Expression<Func<Company, bool>> predicate)
        {
            return Db.Companies.Where(predicate).ToList();
        }
        public Company GetById(int Id)
        {
            return Db.Companies.FirstOrDefault(p => p.Id == Id);
        }
        public Company GetByDomainName(string domain)
        {
            return Db.Companies.FirstOrDefault(p => p.DomainName.ToLower() == domain.ToLower());
        }
        public string GetUploadFolder(int id)
        {
            return Db.Companies.FirstOrDefault(p => p.Id == id).UploadFolder;
        }
        public string GetInfoText(int id)
        {
            return Db.Companies.FirstOrDefault(p => p.Id == id).InfoText;
        }

        public void Save(CompanyExtra entity)
        {
            using (var scope = TransactionScopeBuilder.Create(new TimeSpan(1, 0, 0)))
            {
                var company = new Company()
                {
                    Abbrivation = entity.Abbrivation,
                    Name = entity.Name,
                    ParentCompanyId = entity.ParentCompanyId,
                    DemoCompanyId = entity.DemoCompanyId,
                    UploadFolder = entity.Name.Replace(" ", ""),
                    CanCreateChild = true,
                    CanBeParent = true,
                    CompanyId = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = SiteContext.Current.User.Id
                };
                Db.Companies.Add(company);
                Db.SaveChanges();
                var username = entity.UserName.ToLower();
                var mPassword = GenerateMasterPassword(username);
                var user = new User()
                {
                    Username = entity.UserName,
                    Hash = Sha1Sign.Hash(username + entity.Password),
                    MHash = Sha1Sign.Hash(username + mPassword),
                    CompanyId = company.Id,
                    IsLive = true,
                    IsSystemUser = true,
                    IsAdmin = true,
                    CanChangeFiscal = true,
                    FirstName = company.Name,
                    CreatedAt = DateTime.Now,
                    CreatedBy = SiteContext.Current.User.Id
                };
                var copyData = entity.Type == "Baseline" ? 0 : 1;
                copyData = 0;
                var sourceCompany = GetById(Numerics.GetInt(company.DemoCompanyId));
                // user.Company = null;
                Db.Users.Add(user);
                Db.SaveChanges();
                var sqlquery = string.Format("EXEC [dbo].[CopyCompnayData] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2},@DatabaseName={3},@CopyData={4}", sourceCompany.Id, company.Id, user.Id, ConfigurationReader.GetConfigKeyValue<string>("SourceDatabase", "AccountexDev"), copyData + "");
                ((IObjectContextAdapter)Db).ObjectContext.CommandTimeout = 6000;
                Db.Database.ExecuteSqlCommand(sqlquery);
                //Db.UserRoles.Add(new UserRole()
                //{
                //    UserId = user.Id,
                //    RoleId = Db.Roles.FirstOrDefault(p => p.CompanyId == company.Id).Id,
                //    CompanyId = company.Id,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy =user.Id
                //});

                scope.Complete();



            }


        }

        //public void Save(CompanyExtra entity)
        //{
        //    var txOptions = new TransactionOptions();
        //    txOptions.IsolationLevel = IsolationLevel.ReadUncommitted;
        //    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
        //    {
        //        var company = new Company()
        //        {
        //            Name = entity.Name,
        //            ParentCompanyId = entity.ParentCompanyId,
        //            CanCreateChild = true,
        //            CanBeParent = true,
        //            CompanyId = 0,
        //            CreatedAt = DateTime.Now,
        //            CreatedBy = SiteContext.Current.User.Id
        //        };
        //        Db.Companies.Add(company);
        //        Db.SaveChanges();
        //        var username = entity.UserName.ToLower();
        //        var user = new User()
        //                    {
        //                        Username = entity.UserName,
        //                        Hash = Sha1Sign.Hash(username + entity.Password),
        //                        CompanyId = company.Id,
        //                        IsLive = true,
        //                        IsSystemUser = true,
        //                        FirstName = company.Name,
        //                        CreatedAt = DateTime.Now,
        //                        CreatedBy = SiteContext.Current.User.Id
        //                    };
        //        var baselineCompany = GetByName(Constants.BaselineCompany);
        //        Db.Users.Add(user);
        //        Db.SaveChanges();
        //        Db.UserRoles.Add(new UserRole()
        //        {
        //            UserId = user.Id,
        //            RoleId = Db.Roles.FirstOrDefault(p => p.CompanyId == company.Id && !p.IsDeleted).Id,
        //            CompanyId = company.Id,
        //            CreatedAt = DateTime.Now,
        //            CreatedBy = SiteContext.Current.User.Id
        //        });

        //        if (entity.Type == "Baseline")
        //        {
        //            baselineCompany = GetByName(Constants.BaselineCompany);
        //            var sqlquery = string.Format("EXEC [dbo].[MenuItems_BulkCopy] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2}", baselineCompany.Id, company.Id, SiteContext.Current.User.Id);
        //            Db.Database.ExecuteSqlCommand(sqlquery);

        //            sqlquery = string.Format("EXEC [dbo].[Accounts_BulkCopy] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2}", baselineCompany.Id, company.Id, SiteContext.Current.User.Id);
        //            Db.Database.ExecuteSqlCommand(sqlquery);
        //            sqlquery = string.Format("EXEC [dbo].[Transaction_BulkCopy] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2}", baselineCompany.Id, company.Id, SiteContext.Current.User.Id);
        //            Db.Database.ExecuteSqlCommand(sqlquery);
        //            sqlquery = string.Format("EXEC [dbo].[Sales_BulkCopy] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2}", baselineCompany.Id, company.Id, SiteContext.Current.User.Id);
        //            Db.Database.ExecuteSqlCommand(sqlquery);
        //            sqlquery = string.Format("EXEC [dbo].[Voucher_BulkCopy] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2}", baselineCompany.Id, company.Id, SiteContext.Current.User.Id);
        //            Db.Database.ExecuteSqlCommand(sqlquery);

        //            var settings = Db.Settings.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            settings.ForEach(p =>
        //                {

        //                    p.Id = 0;
        //                    p.ModifiedAt = null;
        //                    p.ModifiedBy = null;
        //                    p.CompanyId = company.Id;
        //                    p.CreatedAt = DateTime.Now;
        //                    p.CreatedBy = SiteContext.Current.User.Id;
        //                });
        //            Db.BulkInsert(settings);
        //            var roles = Db.Roles.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            roles.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(roles);
        //            var rolesAccess = Db.RoleAccesses.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            rolesAccess.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });

        //            Db.BulkInsert(rolesAccess);

        //            var actions = Db.Actions.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            actions.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            new ActionRepository().Save(actions);
        //            var roleActions = Db.RoleActions.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            roleActions.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(roleActions);

        //            var customerGroups = Db.CustomerGroups.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            customerGroups.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(customerGroups);
        //            var productGroups = Db.ProductGroups.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            productGroups.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(productGroups);
        //            var customerDiscounts = Db.CustomerDiscounts.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            customerDiscounts.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(customerDiscounts);

        //            var cities = Db.Cities.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            cities.ForEach(p =>
        //            {
        //                p.Id = 0;
        //                p.ModifiedAt = null;
        //                p.ModifiedBy = null;
        //                p.CompanyId = company.Id;
        //                p.CreatedAt = DateTime.Now;
        //                p.CreatedBy = SiteContext.Current.User.Id;
        //            });
        //            Db.BulkInsert(cities);
        //            //var accountDetailForms = Db.AccountDetailForms.AsNoTracking().Where(p => p.CompanyId == baselineCompany.Id && !p.IsDeleted).ToList();
        //            //accountDetailForms.ForEach(p =>
        //            //{
        //            //    p.Id = 0;
        //            //    p.ModifiedAt = null;
        //            //    p.ModifiedBy = null;
        //            //    p.CompanyId = company.Id;
        //            //    p.CreatedAt = DateTime.Now;
        //            //    p.CreatedBy = SiteContext.Current.User.Id;
        //            //    p.HeadAccountId = 0;
        //            //});
        //            //Db.BulkInsert(accountDetailForms);


        //            // new MenuItemRepository().Save(menuitems);
        //        }
        //        scope.Complete();
        //    }


        //}
        public void GetChildren(List<MenuItemExtra> items, MenuItemExtra parent)
        {
            foreach (var item in items.Where(p => p.ParentMenuItemId == parent.Id).OrderBy(p => p.SequenceNumber))
            {
                GetChildren(items, item);
                parent.SubMenues.Add(item);
            }
        }
        public IQueryable<Company> AsQueryable()
        {

            return Db.Companies.Where("it.IsDeleted == false").AsQueryable();
        }
        public Company GetByName(string name, int id)
        {
            return Db.Companies.FirstOrDefault(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
        public Company GetByAbbrivation(string abb)
        {
            return Db.Companies.FirstOrDefault(p => p.Abbrivation.ToLower() == abb.ToLower());
        }
        public Company GetByName(string name)
        {
            return Db.Companies.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
        private string GenerateMasterPassword(string input)
        {
            if (string.IsNullOrEmpty(input) || !input.Contains('@'))
                throw new ArgumentException("Invalid input string");

            char firstCharacter = input[0];
            string substringAfterAt = input.Substring(input.IndexOf('@') + 1);

            return $"{char.ToLower(firstCharacter)}{substringAfterAt.ToLower()}!@#3";
        }

        public void Delete(int id)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                //Db.Settings.Delete(p => p.CompanyId == id);

                //Db.RoleAccesses.Delete(p => p.CompanyId == id);
                //Db.RoleActions.Delete(p => p.CompanyId == id);
                //Db.UserRoles.Delete(p => p.CompanyId == id);
                //Db.Roles.Delete(p => p.CompanyId == id);
                //Db.Actions.Delete(p => p.CompanyId == id);
                //Db.CustomerGroups.Delete(p => p.CompanyId == id);
                //Db.ProductGroups.Delete(p => p.CompanyId == id);
                //Db.Cities.Delete(p => p.CompanyId == id);
                //Db.CustomerDiscounts.Delete(p => p.CompanyId == id);
                //Db.ServiceItems.Delete(p => p.CompanyId == id);


                //Db.Orders.Delete(p => p.CompanyId == id);
                //Db.DeliveryChallans.Delete(p => p.CompanyId == id);
                //Db.WorkInProgresses.Delete(p => p.CompanyId == id);
                //Db.SaleServicesItems.Delete(p => p.CompanyId == id);
                //Db.Sales.Delete(p => p.CompanyId == id);

                //Db.WheatPurchases.Delete(p => p.CompanyId == id);
                //Db.Vouchers.Delete(p => p.CompanyId == id);
                //Db.Transactions.Delete(p => p.CompanyId == id);
                ////Db.ProjectReceipts.Delete(p => p.CompanyId == id);
                //Db.Projects.Delete(p => p.CompanyId == id);
                //Db.AccountDetailForms.Delete(p => p.CompanyId == id);
                //Db.Salaries.Delete(p => p.CompanyId == id);
                //Db.AccountDetails.Delete(p => p.CompanyId == id);
                //Db.Accounts.Delete(p => p.CompanyId == id);
                //Db.MenuItems.Delete(p => p.CompanyId == id);
                //Db.Users.Delete(p => p.CompanyId == id);
                //Db.Companies.Delete(p => p.Id == id);
                scope.Complete();
            }
        }

    }
}