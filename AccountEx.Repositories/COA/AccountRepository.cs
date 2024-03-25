using System;
using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository() : base() { }
        public AccountRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Account> GetCharOfAccount()
        {
            var list = Collection.Where(p => p.Level == 1 || !p.ParentId.HasValue).ToList();
            return list;
        }
        public string GetLastAccountCode()
        {
            return Collection.Any() ? Collection.OrderByDescending(p => p.Id).FirstOrDefault().AccountCode : "";
        }
        public string GetLastAccountCode(int parentId)
        {
            return Collection.Any(p => p.ParentId == parentId) ? Collection.Where(p => p.ParentId == parentId).OrderByDescending(p => p.Id).FirstOrDefault().AccountCode : "";
        }
        public Account GetAccountByCode(string code)
        {
            return Collection.FirstOrDefault(p => p.AccountCode.ToLower() == code.ToLower());
        }
        public DateTime GetLastActivityDate()
        {
            try
            {
                var query = string.Format("EXEC [DBO].[SP_GetLastUpdatedDate] @TableName = {0}", "accounts");
                var result = Db.Database.SqlQuery<DateTime>(query);
                var accountDate = DateTime.Now;
                if (result != null)
                    accountDate = result.FirstOrDefault();
                query = string.Format("EXEC [DBO].[SP_GetLastUpdatedDate] @TableName = {0}", "accountdetails");
                var accountDetailDate = Db.Database.SqlQuery<DateTime>(query).FirstOrDefault();
                return accountDate > accountDetailDate ? accountDate : accountDetailDate;

            }
            catch (Exception)
            {

                return DateTime.Now;
            }

        }
        //        public DateTime GetLastActivityDate()
        //        {
        //            string query = @"SELECT isnull(MAX(MX),GETDATE()) FROM (
        //                            SELECT MAX(CreatedAt) AS MX FROM Accounts where CompanyId={CompanyId}
        //                                UNION
        //                                SELECT MAX(ModifiedAt) FROM Accounts where CompanyId={CompanyId}
        //	                             UNION
        //                                SELECT MAX(CreatedAt) FROM AccountDetails where CompanyId={CompanyId}
        //	                             UNION
        //                                SELECT MAX(ModifiedAt) FROM AccountDetails where CompanyId={CompanyId}
        //	
        //	                            ) MaxDate";
        //            query = query.Replace("{CompanyId}", SiteContext.Current.User.CompanyId + "");
        //            return Db.Database.SqlQuery<DateTime>(query).FirstOrDefault();
        //        }
        public bool CheckIfLoadCoa(DateTime date)
        {
            var result = false;
            if (Collection.Any(p => p.CreatedAt > date || (p.ModifiedAt.HasValue && p.ModifiedAt > date)))
                result = true;
            else if (AsQueryable<AccountDetail>().Any(p => p.CreatedAt > date || (p.ModifiedAt.HasValue && p.ModifiedAt > date)))
                result = true;
            return result;

        }


        public string GetTitleById(int id)
        {
            if (Collection.Any(p => p.Id == id))
                return Collection.FirstOrDefault(p => p.Id == id).Name;
            else
                return "";
        }

        public List<Account> GetParentAccount()
        {
            var list = Collection.Where(p => p.Level != 1 && p.Level != AppSetting.AccountLevel).ToList();
            return list;
        }
        public bool IsHeadSelected(List<int> accountIds, int headId)
        {
            return Collection.Where(p => accountIds.Contains(p.Id)).Any(p => p.ParentId == headId);
        }
        public int GetBankId(List<int> accountIds, int headId)
        {
            if (Collection.Any(p => accountIds.Contains(p.Id) && p.ParentId == headId))
                return Collection.FirstOrDefault(p => accountIds.Contains(p.Id) && p.ParentId == headId).Id;
            else return 0;
        }
        public void SaveLeaf(LeafAccountExtra input, CoaHeadIdExtra headIds)
        {


            var ac = new Account
            {
                Id = input.Id,
                AccountCode = input.AccountCode,
                DisplayName = input.DisplayName,
                HasChild = false,
                Level = input.Level,
                IsLive = input.IsLive,
                IsSystemAccount = input.IsSystemAccount,
                Name = input.Name,
                ParentId = input.ParentId
            };
            Save(ac, headIds);
            input.Id = ac.Id;
        }
        public void SaveLeaf(Account input)
        {
            base.Save(input);
        }
        public void SaveAccountDetail(AccountDetail entity)
        {


            new AccountDetailRepository().Save(entity);

            SaveChanges();
        }
        public void Save(Account entity, CoaHeadIdExtra headIds)
        {

            //true is passed becouse we need to use Id in next table
            base.Save(entity, true, true);
            if (entity.Level != AppSetting.AccountLevel)
            {
                var accounts = new List<string>
                {
                    "COA.Assets",
                    "COA.Banks",
                    "COA.Customers",
                    "COA.Products",
                    "COA.Suppliers",
                    "COA.Employees",
                    "COA.Services",
                };
                AccountDetailForm accountDetailForm = null;
                var companyId = SiteContext.Current.User.CompanyId;
                var setting = AsQueryable<Setting>().FirstOrDefault(p => p.Value == entity.Name && accounts.Contains(p.Key));
                if (setting == null) return;
                switch (setting.Key)
                {
                    case "COA.Assets":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Assets);
                        break;
                    case "COA.Banks":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Banks);
                        break;
                    case "COA.Customers":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Customers);
                        break;
                    case "COA.Products":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Products);
                        break;
                    case "COA.Suppliers":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Suppliers);
                        break;
                    case "COA.Employees":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Employees);
                        break;
                    case "COA.Services":
                        accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Id == (int)AccountDetailFormType.Services);
                        break;
                }
                if (accountDetailForm != null)
                    accountDetailForm.HeadAccountId = entity.Id;
            }
            else
            {
                //if (Db.AccountDetailForms.Any(p => p.HeadAccountId == 0)) SyncAccountDetailFormIds();
                int accountDetailFormId;
                var isAddDetail = IsAddDetail(headIds, entity.ParentId, out accountDetailFormId);
                var accountDetail = AsQueryable<AccountDetail>().FirstOrDefault(p => p.AccountId == entity.Id);
                if (accountDetail != null)
                {
                    accountDetail.Name = entity.DisplayName;
                    accountDetail.Code = entity.AccountCode;
                    if (isAddDetail) accountDetail.AccountDetailFormId = accountDetailFormId;
                }
                else
                {
                    if (isAddDetail)
                    {
                        Db.AccountDetails.Add(new AccountDetail
                        {
                            AccountId = entity.Id,
                            Name = entity.DisplayName,
                            Code = entity.AccountCode,
                            AccountDetailFormId = accountDetailFormId
                        });
                    }

                }


            }
        }

        private bool IsAddDetail(CoaHeadIdExtra headIds, int? parentId, out int accountDetailFormId)
        {
            var isAddDetail = false;
            accountDetailFormId = 0;
            //var parentId = entity.ParentId;
            var parentIds = new List<int>();
            parentIds.Add(Numerics.GetInt(parentId));
            while (parentId.HasValue && parentId.Value > 0)
            {
                var parent = Collection.FirstOrDefault(p => p.Id == parentId);
                if (parent != null && parent.ParentId.HasValue && parent.ParentId.Value > 0)
                {
                    parentId = parent.ParentId;
                    parentIds.Add(Numerics.GetInt(parentId));
                }
                else
                    break;
            }
            if (parentIds.Contains(headIds.ProductHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Products;
            }
            else if (parentIds.Contains(headIds.BankHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Banks;
            }
            else if (parentIds.Contains(headIds.CustomerHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Customers;
            }
            else if (parentIds.Contains(headIds.AssetHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Assets;
            }
            else if (parentIds.Contains(headIds.SupplierHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Suppliers;
            }
            else if (parentIds.Contains(headIds.EmployeeHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Employees;
            }
            else if (parentIds.Contains(headIds.SaleManHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Salesman;
            }
            else if (parentIds.Contains(headIds.ServicesHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Services;
            }
            else if (parentIds.Contains(headIds.EquipmentHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Equipments;
            }
            else if (parentIds.Contains(headIds.ExpenseHeadId))
            {
                isAddDetail = true;
                accountDetailFormId = (int)AccountDetailFormType.Expences;
            }
            return isAddDetail;
        }

        public List<IdName> GetLeafAccount(int pid)
        {
            var subaPccounts = Collection.Where(p => p.ParentId == pid).Select(p => p.Id).ToList();
            subaPccounts.Add(pid);
            return Collection.Where(p => subaPccounts.Contains(p.ParentId.Value) && p.Level == AppSetting.AccountLevel).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.AccountCode + " - " + p.Name,

            }).ToList();
        }
        public List<IdName> GetLeafAccount(int pid, bool mergeCodeName)
        {
            var subaPccounts = Collection.Where(p => p.ParentId == pid).Select(p => p.Id).ToList();
            subaPccounts.Add(pid);
            return Collection.Where(p => subaPccounts.Contains(p.ParentId.Value) && p.Level == AppSetting.AccountLevel).Select(p => new IdName
            {
                Id = p.Id,
                Name = mergeCodeName ? p.AccountCode + " - " + p.Name : p.Name,

            }).ToList();
        }
        public bool CheckIfLeafAccountExist(int pid)
        {
            return Collection.Any(p => p.ParentId == pid);

        }
        public bool CheckIfLeafAccountExistBackup(int pid)
        {
            var subaPccounts = Collection.Where(p => p.ParentId == pid).Select(p => p.Id).ToList();
            return Collection.Any(p => subaPccounts.Contains(p.ParentId.Value));

        }
        public List<Account> GetLeafAccount()
        {
            return Collection.Where(p => p.Level == AppSetting.AccountLevel).ToList();
        }
        public List<IdName> GetOtherThenLeafAccount(List<int> Ids)
        {
            return Collection.Where(p => p.Level != AppSetting.AccountLevel && Ids.Contains(p.Id)).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public List<Account> GetLeafAccountsWithCodeName2()
        {
            var data = Collection.Where(p => p.Level == AppSetting.AccountLevel).ToList();
            foreach (var account in data)
            {
                account.Name = account.AccountCode + "-" + account.DisplayName;
            }
            return data;
        }
        public List<IdName> GetLeafAccountsWithCodeName()
        {
            return Collection.Where(p => p.Level == AppSetting.AccountLevel).Select(p => new { p.Id, p.AccountCode, p.Name }).ToList().Select(p => new IdName { Id = p.Id, Name = p.AccountCode + "-" + p.Name }).ToList();
        }
        public List<IdName> GetLeafAccountsWithCodeName(int parentAccountId)
        {
            return Collection.Where(p => p.Level == AppSetting.AccountLevel && p.ParentId == parentAccountId).Select(p => new { p.Id, p.AccountCode, p.Name }).ToList().Select(p => new IdName { Id = p.Id, Name = p.AccountCode + "-" + p.Name }).ToList();
        }
        public List<IdName> GetGenericLeafAccount()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var pid = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            if (customersetting != null)
                pid = Numerics.GetInt(customersetting.Value);
            return Collection.Where(p => p.ParentId == pid).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public List<IdName> GetLeafAccounts(int parentId)
        {
            return GetLeafAccounts(parentId, true);


        }
        public List<IdName> GetLeafAccounts(int parentId, bool requireCode)
        {
            var l4Accounts = new List<IdName>();
            var accounts = Collection.Where(p => p.ParentId == parentId);
            foreach (var item in accounts)
            {
                if (item.Level == 4)
                    l4Accounts.Add(new IdName { Id = item.Id, Name = requireCode ? item.AccountCode + "-" + item.DisplayName : item.DisplayName });
                else
                    l4Accounts.AddRange(GetLeafAccounts(item.Id, requireCode));
            }
            return l4Accounts;


        }
        public List<Account> GetLeafAccountsDetail(List<int> parentIds)
        {
            var l4Accounts = new List<Account>();
            var accounts = Collection.Where(p => parentIds.Contains(p.ParentId.Value));
            foreach (var item in accounts)
            {
                if (item.Level == 4)
                    l4Accounts.Add(item);
                else
                    l4Accounts.AddRange(GetLeafAccountsDetail(item.Id));
            }
            return l4Accounts;


        }
        public List<Account> GetByIds(List<int> Ids)
        {

            return Collection.Where(p => Ids.Contains(p.Id)).ToList();



        }
        public List<IdName> GetNamesByIds(List<int> Ids)
        {

            return Collection.Where(p => Ids.Contains(p.Id)).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();



        }
        public List<Account> GetLeafAccountsDetail(int parentId)
        {
            var l4Accounts = new List<Account>();
            var accounts = Collection.Where(p => p.ParentId == parentId);
            foreach (var item in accounts)
            {
                if (item.Level == 4)
                    l4Accounts.Add(item);
                else
                    l4Accounts.AddRange(GetLeafAccountsDetail(item.Id));
            }
            return l4Accounts;


        }

        public List<Account> GetLeafAccounts(int parentId, int? referenceId)
        {
            var l4Accounts = new List<Account>();
            var accounts = Collection.Where(p => p.ParentId == parentId && !p.ReferenceId.HasValue);
            foreach (var item in accounts)
            {
                if (item.Level == 4)
                    l4Accounts.Add(item);
                else
                    l4Accounts.AddRange(GetLeafAccounts(item.Id, referenceId));
            }
            return l4Accounts;


        }

        public List<IdName> GetLeafAccounts()
        {
            return Collection.Where(p => p.Level == AppSetting.AccountLevel).Select(p => new { p.Id, p.DisplayName }).Select(p => new IdName { Id = p.Id, Name = p.DisplayName }).ToList();
        }
        public List<IdName> GetByLevel(int level)
        {
            return Collection.Where(p => p.Level == level).Select(p => new { p.Id, p.DisplayName }).Select(p => new IdName { Id = p.Id, Name = p.DisplayName }).ToList();
        }
        public List<IdName> GetByLevel(int level, List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id) && p.Level == level).Select(p => new { p.Id, p.DisplayName }).Select(p => new IdName { Id = p.Id, Name = p.DisplayName }).ToList();
        }

        public List<IdName> GetChildrenToNLevel(int parentId)
        {
            var list = new List<IdName>();
            var children = Collection.Where(p => p.ParentId == parentId);
            const string dashes = "---";
            foreach (var item in children)
            {
                list.Add(new IdName { Id = item.Id, Name = dashes.Substring(0, item.Level - 1) + item.DisplayName });
                list.AddRange(GetChildrenToNLevel(item.Id));
            }
            return list;
        }
        public List<IdName> GetChildrenTo3RdLevel(int parentId)
        {
            var list = new List<IdName>();
            var children = Collection.Where(p => p.ParentId == parentId && p.Level != AppSetting.AccountLevel);
            const string dashes = "---";
            foreach (var item in children)
            {
                list.Add(new IdName { Id = item.Id, Name = dashes.Substring(0, item.Level - 1) + item.DisplayName });
                list.AddRange(GetChildrenToNLevel(item.Id));
            }
            return list;
        }
        public List<IdName> GetChildrenTo3RdLevelOnly(int parentId)
        {
            var list = new List<IdName>();
            var children = Collection.Where(p => p.ParentId == parentId && p.Level != AppSetting.AccountLevel);
            const string dashes = "---";
            foreach (var item in children)
            {
                if (item.Level == 3)

                    list.Add(new IdName { Id = item.Id, Name = dashes.Substring(0, item.Level - 1) + item.DisplayName });
                else
                    list.AddRange(GetChildrenTo3RdLevelOnly(item.Id));
            }
            return list;


        }
        public List<Account> GetChildrenTo3RdLevelDetail(int parentId)
        {
            var list = new List<Account>();
            var children = Collection.Where(p => p.ParentId == parentId && p.Level != AppSetting.AccountLevel);
            const string dashes = "---";
            foreach (var item in children)
            {
                list.Add(item);
                list.AddRange(GetChildrenTo3RdLevelDetail(item.Id));
            }
            return list;


        }
        public List<Account> GetChildrenTo3RdLevelWithLeafDetail(int parentId)
        {
            var list = new List<Account>();
            var children = Collection.Where(p => p.ParentId == parentId);
            const string dashes = "---";
            foreach (var item in children)
            {
                list.Add(item);
                list.AddRange(GetChildrenTo3RdLevelDetail(item.Id));
            }
            return list;


        }

        public List<IdName> GetAccountTree()
        {
            var list = new List<IdName>();
            var l1Accounts = Collection.Where(p => p.Level == 1).ToList();
            foreach (var item in l1Accounts)
            {
                list.Add(new IdName { Id = item.Id, Name = item.DisplayName });
                list.AddRange(GetChildrenToNLevel(item.Id));
            }
            return list;
        }
        public List<IdName> GetAccountTree(int accountId)
        {
            var list = new List<IdName>();
            var account = Collection.FirstOrDefault(p => p.Id == accountId);

            list.Add(new IdName { Id = account.Id, Name = account.DisplayName });
            list.AddRange(GetChildrenTo3RdLevel(account.Id));

            return list;
        }
        public List<IdName> GetAccountTreeByLevel(int level)
        {
            var list = new List<IdName>();
            var accounts = Collection.Where(p => p.Level == level).ToList();

            foreach (var item in accounts)
            {
                var title = item.DisplayName;
                if (level > 1)
                {
                    var ac2 = Collection.FirstOrDefault(p => p.Id == item.ParentId);
                    if (ac2 != null)
                    {
                        title = ac2.DisplayName + " > " + title;
                        if (level > 2)
                        {
                            var ac1 = Collection.FirstOrDefault(p => p.Id == ac2.ParentId);
                            if (ac1 != null)
                                title = ac1.DisplayName + " > " + ac2.DisplayName + " > " + item.DisplayName;
                        }
                    }
                }
                list.Add(new IdName { Id = item.Id, Name = title });
            }
            return list;
        }
        public string GetFullName(int id)
        {
            var name = "";
            var account = Collection.FirstOrDefault(p => p.Id == id);
            if (account != null)
            {
                name = account.DisplayName;
                if (account.ParentId.HasValue)
                    name = GetFullName(account.ParentId.Value) + " > " + name;
            }
            return name;
        }

        public List<IdName> GetClients()
        {
            var config = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Project" && p.KeyName == "ClientAccountId");
            var accountId = 0;
            if (config != null)
                accountId = Numerics.GetInt(config.Value);
            if (accountId > 0)
                return GetLeafAccounts(accountId);
            return GetLeafAccounts();
        }
        public List<IdName> GetEmployees()
        {
            var config = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Project" && p.KeyName == "EmployeeAccountId");
            var accountId = 0;
            if (config != null)
                accountId = Numerics.GetInt(config.Value);
            if (accountId > 0)
                return GetLeafAccounts(accountId);
            return GetLeafAccounts();
        }
        public List<IdName> GetChildren(int parentAccountId)
        {
            return Collection.Where(p => p.ParentId == parentAccountId).Select(p => new { p.Id, p.DisplayName }).Select(p => new IdName { Id = p.Id, Name = p.DisplayName }).ToList();
        }

        public List<AccountCode> GetAccountSuggestion(int parentId, string query)
        {
            var l4Accounts = new List<AccountCode>();
            var accounts = Collection.Where(p => p.ParentId == parentId).ToList();
            foreach (var item in accounts)
            {
                switch (item.Level)
                {
                    case 4:
                        if (item.AccountCode.ToLower().Contains(query) || item.Name.ToLower().Contains(query) || item.DisplayName.ToLower().Contains(query))
                            l4Accounts.Add(new AccountCode { Id = item.Id, Name = item.DisplayName, Code = item.AccountCode });
                        break;
                    default:
                        l4Accounts.AddRange(GetAccountSuggestion(item.Id, query));
                        break;
                }
            }
            return l4Accounts;


        }
        public List<AccountCode> GetAccountSuggestion(int parentId)
        {
            var l4Accounts = new List<AccountCode>();
            var accounts = Collection.Where(p => p.ParentId == parentId);
            foreach (var item in accounts)
            {
                switch (item.Level)
                {
                    case 4:
                        l4Accounts.Add(new AccountCode { Id = item.Id, Name = item.DisplayName, Code = item.AccountCode });
                        break;
                    default:
                        l4Accounts.AddRange(GetAccountSuggestion(item.Id));
                        break;
                }
            }
            return l4Accounts.Take(10).ToList();


        }

        public Account GetByCode(string code)
        {
            return Collection.FirstOrDefault(p => p.AccountCode.ToLower() == code.ToLower());
        }
        public Account GetByName(string name)
        {
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
        public int GetIdByName(string name)
        {
            var d = Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (d == null)
                return 0;
            else
                return d.Id;

        }
        public List<int> GetIdsByName(string names)
        {
            if (string.IsNullOrWhiteSpace(names)) return new List<int>();
            names = names.ToLower();
            var titles = names.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(p => p.Trim()).ToList();
            var d = Collection.Where(p => titles.Contains(p.Name.ToLower())).Select(p => p.Id).ToList();
            return d;

        }

        public Account GetByCode(string code, int id)
        {
            return Collection.FirstOrDefault(p => p.AccountCode.ToLower() == code.ToLower() && p.Id != id);
        }
        public bool IsExistByCode(string code, int id)
        {
            return Collection.Any(p => p.AccountCode.ToLower() == code.ToLower() && p.Id != id);
        }
        public Account GetByName(string name, int id)
        {
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
        public bool IsExistByName(string name, int id)
        {
            return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
        private const string EmployeeAccountId = "EmployeeAccountId";
        private const string MasterAccountId = "MasterAccountId";
        private const string ItemAccountId = "ItemAccountId";
        private const string Customers = "COA.Customers";

        public void SyncAccountDetailFormIds()
        {
            var formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Asset" && p.KeyName == MasterAccountId);
            var accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Assets");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Bank" && p.KeyName == MasterAccountId);
            accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Banks");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Employee" && p.KeyName == EmployeeAccountId);
            accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Employees");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Sale" && p.KeyName == MasterAccountId);
            accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Customers");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Purchase" && p.KeyName == MasterAccountId);
            accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Suppliers");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            formSetting = AsQueryable<FormSetting>().FirstOrDefault(p => p.VoucherType == "Sale" && p.KeyName == ItemAccountId);
            accountDetailForm = AsQueryable<AccountDetailForm>().FirstOrDefault(p => p.Name == "Products");
            if (formSetting != null && accountDetailForm != null)
            {
                accountDetailForm.HeadAccountId = Convert.ToInt32(formSetting.Value);
            }
            SaveChanges();
        }

    }
}