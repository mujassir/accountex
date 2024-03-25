using System;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System.Linq;
using System.Web;
using AccountEx.DbMapping;

namespace AccountEx.BussinessLogic
{
    public static class AccountManager
    {
        private const string MainAccountName = "Chart Of Account";
        private const string ControlAccountName = "Control System";
        private const string SubAccountName = "Sub System";
        private const string EmployeeAccountId = "EmployeeAccountId";
        private const string MasterAccountId = "MasterAccountId";
        private const string ItemAccountId = "ItemAccountId";
        private const string IncomeTaxId = "ItemAccountId";
        public const string CustomerAccountTitle = "A/C Payables";



        //public static List<AccountCode> GetSuggestions()
        //{

        //}

        public static string GetNextAccountCode()
        {
            var lastCode = new AccountRepository().GetLastAccountCode();
            if (string.IsNullOrWhiteSpace(lastCode))
                return "1001";
            if (IsNumber(lastCode))
                return (Convert.ToInt32(lastCode) + 1) + "";
            return IncreaseCode(lastCode);
        }
        public static string GetNextAccountCode(int parentId)
        {
            var lastCode = new AccountRepository().GetLastAccountCode(parentId);
            if (string.IsNullOrWhiteSpace(lastCode))
                return "1001";
            if (IsNumber(lastCode))
                return (Convert.ToInt32(lastCode) + 1) + "";
            return IncreaseCode(lastCode);
        }

        private static string IncreaseCode(string input)
        {
            var resultString = System.Text.RegularExpressions.Regex.Match(input, @"\d+").Value;
            if (string.IsNullOrWhiteSpace(resultString)) return input + "2";
            var newNumber = (Convert.ToInt32(resultString) + 1) + "";
            var newCode = input.Replace(resultString, newNumber);
            if (newCode == input) newCode = newCode + "2";
            return newCode;
        }

        private static bool IsNumber(string input)
        {
            return input.All(char.IsDigit);
        }
        public static ApiResponse LoadCoa()
        {
            var cookie = HttpContext.Current.Request.Cookies["LastDate"];
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("LastDate", new DateTime(2010, 6, 30) + "") { Expires = DateTime.Now.AddMonths(1) });
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("Company", SiteContext.Current.User.CompanyId + "") { Expires = DateTime.Now.AddMonths(1) });
            }
            cookie = HttpContext.Current.Request.Cookies["LastDate"];
            var companyId = Numerics.GetInt(!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Cookies["Company"] + "") ? HttpContext.Current.Request.Cookies["Company"].Value : "");
            var date = DateConverter.ConvertStandardDate(cookie.Value);
            ApiResponse response;
            try
            {
                var loadCoa = new AccountRepository().CheckIfLoadCoa(date) || SiteContext.Current.User.CompanyId != companyId;
                var coa = loadCoa
                    ? new AccountRepository().GetAll().Where(p => p.Level != AppSetting.AccountLevel || (p.AccountCode != null && p.Level == AppSetting.AccountLevel))
                    : null;
                var lastActivityDate = loadCoa
                    ? new AccountRepository().GetLastActivityDate()
                    : date;
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        LoadCOA = loadCoa,
                        COA = coa,
                        LastDate = lastActivityDate
                    }
                };
                if (loadCoa)
                {

                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("LastDate", DateTime.Now + "") { Expires = DateTime.Now.AddMonths(1) });
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("Company", SiteContext.Current.User.CompanyId + "") { Expires = DateTime.Now.AddMonths(1) });
                }
            }
            catch (Exception)
            {

                response = new ApiResponse
                {
                    Success = false,

                };
            }

            return response;
        }
        public static int GetLeafAccountId(string title)
        {

            var repo = new AccountRepository();
            var ac = repo.FirstOrDefault(p => p.Name == title);
            if (ac != null) return ac.Id;
            ac = new Account
            {
                Name = title,
                DisplayName = title,
                Level = (byte)AppSetting.AccountLevel,
                IsLive = true,
                HasChild = false,
                AccountCode = GetNextAccountCode(),
                ParentId = GetSystemSubAccountId(),

            };
            repo.Add(ac);
            return ac.Id;
        }
        public static int GetSystemSubAccountId()
        {

            var repo = new AccountRepository();
            var ac = repo.FirstOrDefault(p => p.Level == 3 && p.Name == SubAccountName);
            if (ac != null) return ac.Id;
            ac = new Account
            {
                Name = SubAccountName,
                DisplayName = SubAccountName,
                Level = 3,
                AccountCode = GetNextAccountCode(),
                HasChild = true,
                IsLive = true,
                IsSystemAccount = true,
                ParentId = GetSystemControlAccountId(),

            };
            repo.Add(ac);
            return ac.Id;
        }
        public static int GetSystemControlAccountId()
        {

            var repo = new AccountRepository();
            var ac = repo.FirstOrDefault(p => p.Level == 2 && p.Name == ControlAccountName);
            if (ac != null) return ac.Id;
            ac = new Account
            {
                Name = ControlAccountName,
                DisplayName = ControlAccountName,
                Level = 2,
                AccountCode = GetNextAccountCode(),
                HasChild = true,
                IsLive = true,
                IsSystemAccount = true,
                ParentId = GetSystemMainAccountId(),

            };
            repo.Add(ac);
            return ac.Id;
        }
        public static int GetSystemMainAccountId()
        {
            var repo = new AccountRepository();
            var ac = repo.FirstOrDefault(p => p.Level == 1);
            if (ac != null) return ac.Id;
            ac = new Account
            {
                Name = MainAccountName,
                DisplayName = MainAccountName,
                Level = 1,
                AccountCode = GetNextAccountCode(),
                HasChild = true,
                IsLive = true,
                IsSystemAccount = true,
            };
            repo.Add(ac);
            return ac.Id;
        }

        public static void SyncEmployees()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Employee" && p.KeyName == EmployeeAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var employees = accounts.Select(p => new Employee
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new EmployeeRepository();
            repo.Add(employees);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static void SyncCustomers()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Sale" && p.KeyName == MasterAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var input = accounts.Select(p => new Customer
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new CustomerRepository();
            repo.Add(input);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static void SyncSuppliers()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Purchase" && p.KeyName == MasterAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var input = accounts.Select(p => new Supplier
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new SupplierRepository();
            repo.Add(input);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static void SyncBanks()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Bank" && p.KeyName == MasterAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var input = accounts.Select(p => new Bank
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new BankRepository();
            repo.Add(input);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static void SyncProducts()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Sale" && p.KeyName == ItemAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var input = accounts.Select(p => new Product
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new ProductRepository();
            repo.Add(input);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static void SyncAssets()
        {
            var setting = new FormSettingRepository().AsQueryable().FirstOrDefault(p => p.VoucherType == "Asset" && p.KeyName == MasterAccountId);
            if (setting == null) return;
            var accountId = Numerics.GetInt(setting.Value);
            if (accountId <= 0) return;
            var accounts = new AccountRepository().GetLeafAccounts(accountId, null);
            if (accounts == null || accounts.Count <= 0) return;
            var input = accounts.Select(p => new Asset
            {
                AccountId = p.Id,
                Code = p.AccountCode,
                Name = p.DisplayName,
            }).ToList();
            var repo = new AssetRepository();
            repo.Add(input);
            repo.SyncIds(accounts.Select(p => p.Id).ToList());
        }
        public static ApiResponse Save(LeafAccountExtra input)
        {
            ApiResponse response;
            var err = ServerValidateSave(input);
            if (err == "")
            {
                var headIdExtra = new CoaHeadIdExtra()
                {
                    AssetHeadId = SettingManager.AssetHeadId,
                    BankHeadId = SettingManager.BankHeadId,
                    CustomerHeadId = SettingManager.CustomerHeadId,
                    ProductHeadId = SettingManager.ProductHeadId,
                    SupplierHeadId = SettingManager.SupplierHeadId,
                    EmployeeHeadId = SettingManager.EmployeeHeadId,
                    SaleManHeadId = SettingManager.SalemanHeadId,
                    ServicesHeadId = SettingManager.ServicesHeadId,
                    EquipmentHeadId = SettingManager.EquipmentHeadId,
                    ExpenseHeadId = SettingManager.ExpensesHeadId

                };
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AccountRepository();
                    repo.SaveLeaf(input, headIdExtra);
                    repo.SaveChanges();
                    scope.Complete();
                }
                response = new ApiResponse() { Success = true, Data = input.Id };
            }
            else
            {
                response = new ApiResponse()
                {
                    Success = false,
                    Error = err
                };
            }

            return response;


        }
        //this method will be use to save data from extension forms
        public static ApiResponse Save(AccountDetail accountDetail, int accountDetialFormId, int headAccountId)
        {
            ApiResponse response;
            var repo = new AccountRepository();
            accountDetail.AccountDetailFormId = accountDetialFormId;
            var dbAccount = repo.GetById(accountDetail.AccountId);
            var account = new CodeFirst.Models.Account()
            {
                AccountCode = accountDetail.Code,
                ParentId = accountDetail.ParentId == 0 ? headAccountId : accountDetail.ParentId,
                Name = accountDetail.Name,
                DisplayName = accountDetail.Name,
                Level = 4,
                IsSystemAccount = false,
            };
            if (accountDetail.Id > 0)
            {
                dbAccount.Name = accountDetail.Name;
                dbAccount.DisplayName = accountDetail.Name;
                dbAccount.AccountCode = accountDetail.Code;
                dbAccount.ParentId = accountDetail.ParentId == 0 ? headAccountId : accountDetail.ParentId;
                account = dbAccount;
            }
            var err = ServerValidateSave(accountDetail, accountDetialFormId, headAccountId, repo);
            if (err == "")
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    //var repo = new AccountRepository();
                    var accountDetailRepo = new AccountDetailRepository(repo);
                    repo.Save(account, true, true);
                    accountDetail.AccountId = account.Id;
                    accountDetailRepo.Save(accountDetail, true, false);
                    repo.SaveChanges();
                    scope.Complete();
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = ""
                    };

                }

            }
            else
            {
                response = new ApiResponse()
                {
                    Success = false,
                    Error = err
                };
            }
            return response;


        }
        public static string ServerValidateSave(CodeFirst.Models.Account input)
        {
            var err = ",";
            try
            {
                var isExist = false;
                var repo = new AccountRepository();

                if (!string.IsNullOrWhiteSpace(input.AccountCode))
                {
                    isExist = repo.IsExistByCode(input.AccountCode, input.Id);

                    if (isExist)
                    {
                        err += "Account code alrady exist.";
                    }
                }
               
                isExist = repo.IsExistByName(input.Name, input.Id);

                if (isExist)
                {
                    err += ",Account name already exist.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        private static string ServerValidateSave(CodeFirst.Models.AccountDetail input, int accountDetialFormId, int headAccountId, AccountRepository repo)
        {
            var err = ",";
            int parentid = input.ParentId == 0 ? headAccountId : input.ParentId;
            try
            {
                var accountDetailrepo = new AccountDetailRepository(repo);
                if (string.IsNullOrWhiteSpace(input.Code))
                {
                    err += "Account code is required.";
                }
                if (string.IsNullOrWhiteSpace(input.Name))
                {
                    err += ",Account name is required.";
                }
                var record = repo.IsExistByCode(input.Code, input.AccountId);

                if (record)
                {
                    err += ",Account code already exist.";
                }
                record = repo.IsExistByName(input.Name, input.AccountId);

                if (record)
                {
                    err += ",Account name already exist.";
                }
                if (headAccountId == 0)
                {
                    err += ",Head account setting is not defined.";
                }
                if (accountDetialFormId == (int)AccountDetailFormType.Products)
                {
                    if (!string.IsNullOrWhiteSpace(input.BarCode) && accountDetailrepo.CheckIfBarCodeExist(input.BarCode, input.Id))
                    {
                        err += ",BarCode already exist.";
                    }
                }
                if (accountDetialFormId == (int)AccountDetailFormType.Customers)
                {
                    if (!string.IsNullOrWhiteSpace(input.VendorCode))
                    {
                        var vendorcoderecord = accountDetailrepo.CheckIfVendorCodeExist(input.VendorCode, input.Id);
                        if (vendorcoderecord)
                        {
                            err += ",Vendor Code already exists.";
                        }
                    }
                }
                else
                {
                    var parentAccount = repo.GetById(headAccountId);
                    if (parentAccount.Level != 3 && (parentid == 0 || headAccountId == parentid))
                    {
                        err += ",<li>Parent account is required.</li>";
                    }
                }
            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public static string ServerValidateDelete(CodeFirst.Models.Account input, AccountRepository repo)
        {
            return ServerValidateDelete(input.Id, repo);
        }
        public static string ServerValidateDelete(int id, AccountRepository repo)
        {
            var err = ",";
            try
            {
                var accountRepo = new AccountRepository(repo);
                var transRepo = new TransactionRepository(repo);
                var settingRepo = new SettingRepository(repo);

                var accountTitle = accountRepo.GetTitleById(id);
                if (accountRepo.CheckIfLeafAccountExist(id))
                {
                    err += "Account has child accounts and can't be deleted.";
                }
                var leafaccounts = repo.GetLeafAccounts(id).Select(p => p.Id).ToList();
                leafaccounts.Add(id);
                if (transRepo.CheckIfTransactionExist(leafaccounts))
                {
                    err += ",Account has transactions and can't be deleted.";
                }
                if (settingRepo.CheckIfAccountExist(accountTitle.ToLower()))
                {
                    err += ",Account is used in settings can't be deleted.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }


        public static ApiResponse DeleteFromAccountDetail(int id, int accountDetialFormId)
        {
            ApiResponse response;
            var accountdetail = new AccountDetailRepository().GetById(id);
            var err = ServerValidateAccountDetail(accountdetail.AccountId, accountDetialFormId);
            if (err == "")
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AccountRepository();
                    var accountDetailRepo = new AccountDetailRepository(repo);
                    accountDetailRepo.Delete(accountdetail.AccountId, "AccountId");
                    repo.Delete(accountdetail.AccountId);

                    repo.SaveChanges();
                    scope.Complete();
                }
                response = new ApiResponse() { Success = true };
            }
            else
            {
                response = new ApiResponse() { Success = false, Error = err };
            }


            return response;
        }

        //this method will be sued to validate all delete from extension forms
        private static string ServerValidateAccountDetail(int id, int accountDetialFormId)
        {
            var err = ",";
            try
            {

                if (new AccountRepository().CheckIfLeafAccountExist(id))
                {
                    err += "Account has child accounts and can't be deleted.";
                }
                var leafaccounts = new AccountRepository().GetLeafAccounts(id).Select(p => p.Id).ToList();
                leafaccounts.Add(id);
                if (new TransactionRepository().CheckIfTransactionExist(leafaccounts))
                {
                    var trnsfrom = new TransactionRepository().GetByAccountId(id);
                    // var trnsto = new TransactionRepository().GetLastByAccountId(id);
                    // var fromDate = trnsfrom.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    // var toDate = trnsto.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //err += ",Account has transactions and can't be deleted. &nbsp<a href='../reports/general-ledger?accountId=" + id + "&fromDate=" + fromDate + "&toDate=" + toDate + "'>View Transactions</a>";
                    err += ",Account has transactions and can't be deleted.";
                }
                switch (accountDetialFormId)
                {
                    case (int)AccountDetailFormType.Products:
                        if (new SaleItemRepository().CheckIfSalePurchaseExist(leafaccounts))
                        {

                            err += ",Product has entry in sale/purchase  and can't be deleted.";
                        }
                        if (new WheatPurchaseItemRepository().CheckIfSalePurchaseExist(leafaccounts))
                        {

                            err += ",Product has entry in purchase  and can't be deleted.";
                        }
                        if (new ProductionItemRepository().CheckIfProductionExist(leafaccounts))
                        {

                            err += ",Product has entry in 'work in procss' and can't be deleted.";
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
    }
}
