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
namespace AccountEx.Repositories
{
    public class DomainMappingRepository
    {
        public AccountExContext Db = null;
        public DomainMappingRepository()
        {
            Db = Connection.GetContext();

        }
        public List<Company> GetAll()
        {
            return Db.Companies.ToList();
        }
        public Company GetById(int Id)
        {
            return Db.Companies.FirstOrDefault(p => p.Id == Id);
        }
        public void Save(CompanyExtra entity)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var company = new Company()
                {
                    Name = entity.Name,
                    ParentCompanyId = entity.ParentCompanyId,
                    CanCreateChild = true,
                    CanBeParent = true,
                    CompanyId = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = SiteContext.Current.User.Id
                };
                Db.Companies.Add(company);
                Db.SaveChanges();
                var username = entity.UserName.ToLower();
                var user = new User()
                {
                    Username = entity.UserName,
                    Hash = Sha1Sign.Hash(username + entity.Password),
                    CompanyId = company.Id,
                    IsLive = true,
                    IsSystemUser = true,
                    FirstName = company.Name,
                    CreatedAt = DateTime.Now,
                    CreatedBy = SiteContext.Current.User.Id
                };
                var copyData = entity.Type == "Baseline" ? 0 : 1;
                var sourceCompany = GetByName(entity.Type == "Baseline" ? ConfigurationReader.GetConfigKeyValue<string>("BaselineCompany", "Abdul Aleem Traders") : ConfigurationReader.GetConfigKeyValue<string>("DemoCompany", "Abdul Aleem Traders"));
                Db.Users.Add(user);
                Db.SaveChanges();
                var sqlquery = string.Format("EXEC [dbo].[CopyCompnayData] @SourceCompanyId = {0}, @TargetCompanyId = {1}, @CreatedBy = {2},@DatabaseName={3},@CopyData={4}", sourceCompany.Id, company.Id, user.Id, ConfigurationReader.GetConfigKeyValue<string>("SourceDatabase", "AccountexDev"), copyData + "");
                ((IObjectContextAdapter)Db).ObjectContext.CommandTimeout = 600;
                Db.Database.ExecuteSqlCommand(sqlquery);
                Db.UserRoles.Add(new UserRole()
                {
                    UserId = user.Id,
                    RoleId = Db.Roles.FirstOrDefault(p => p.CompanyId == company.Id).Id,
                    CompanyId = company.Id,
                    CreatedAt = DateTime.Now,
                    CreatedBy = SiteContext.Current.User.Id
                });

                scope.Complete();



            }


        }

        public IQueryable<DomainMapping> AsQueryable()
        {

            return Db.DomainMappings.AsQueryable();
        }
        public DomainMapping GetByName(string name, int id)
        {
            return Db.DomainMappings.FirstOrDefault(p => p.CompanyName.ToLower() == name.ToLower() && p.Id != id);
        }
        public DomainMapping GetByName(string name)
        {
            return Db.DomainMappings.FirstOrDefault(p => p.CompanyName.ToLower() == name.ToLower());
        }
        public DomainMapping GetByDomainName(string domain)
        {
            return Db.DomainMappings.FirstOrDefault(p => p.Domain.ToLower() == domain.ToLower());
        }
       
    }
}