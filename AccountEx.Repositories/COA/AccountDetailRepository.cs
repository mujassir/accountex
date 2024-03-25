using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class AccountDetailRepository : GenericRepository<AccountDetail>
    {
        public AccountDetailRepository() : base() { }
        public AccountDetailRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<AccountDetail> GetAll(AccountDetailFormType type)
        {
            return Collection.Where(p => p.AccountDetailFormId == (int)type).ToList();
        }
        public AccountDetail GetByAccountId(int accountid)
        {
            return Collection.FirstOrDefault(p => p.AccountId == accountid);
        }
        public ContactDetailExtra GetContactDetail(int accountid)
        {
            return Collection.Where(p => p.AccountId == accountid).Select(p => new ContactDetailExtra()
            {
                AccountId = p.AccountId,
                AccountCode = p.Code,
                AccountName = p.Name,
                Route = p.Route,
                PoBoxNo = p.PoBoxNo,
                ContactNumber = p.ContactNumber,
                LocalId = p.LocalId
            }).FirstOrDefault();
        }
        public List<int> GetForexSuppliers()
        {
            return Collection.Where(p => p.AccountDetailFormId == (byte)AccountDetailFormType.Suppliers && p.IsForex).Select(p => p.AccountId).ToList();
        }
        public string GetAddressByAccountId(int accountid)
        {
            if (Collection.Any(p => p.AccountId == accountid))
                return Collection.FirstOrDefault(p => p.AccountId == accountid).Address;
            else return "";
        }
        public AccountDetail GetByAccountCode(string code)
        {


            return Collection.FirstOrDefault(p => p.Code.ToLower() == code.ToLower());
        }
        public AccountDetail GetByAccountCode(string code, int formid)
        {
            return Collection.FirstOrDefault(p => p.Code.ToLower() == code.ToLower() && p.AccountDetailFormId == formid);
        }
        public List<AccountDetail> GetByAccountIds(List<int> Ids)
        {
            return Collection.Where(p => Ids.Contains(p.AccountId)).ToList();
        }
        public AccountDetail GetEmployeBySalaryItem(List<AccountDetail> accountDetail, SalaryItem salaryItem)
        {
            return accountDetail.FirstOrDefault(p => p.AccountId == salaryItem.AccountId);
        }
        public void Save(ProductStock obj)
        {
            foreach (var item in obj.Stocks)
            {
                var currentrecord = Collection.FirstOrDefault(p => p.Id == item.Id);
                if (currentrecord != null)
                {
                    currentrecord.Quantity = item.Quantity;
                    currentrecord.CompanyId = SiteContext.Current.User.CompanyId;
                }
            }
            SaveChanges();
        }
        public List<IdName> GetNames(AccountDetailFormType type)
        {
            return Collection.Where(p => p.AccountDetailFormId == (int)type).Select(p => new { p.AccountId, p.Code, p.Name })
                .ToList().Select(p => new IdName { Id = p.AccountId, Name = p.Code + "-" + p.Name }).ToList();
        }
        public override List<IdName> GetNames()
        {
            return Collection.Select(p => new { p.Id, p.Code, p.Name }).ToList().Select(p => new IdName { Id = p.Id, Name = p.Code + "-" + p.Name }).ToList();
        }
        public List<IdName> GetNamesWithoutCode(AccountDetailFormType type)
        {
            return Collection.Where(p => p.AccountDetailFormId == (int)type).Select(p => new { p.AccountId, p.Code, p.Name })
                .ToList().Select(p => new IdName { Id = p.AccountId, Name = p.Name }).ToList();
        }

        public bool CheckIfBarCodeExist(string barcode, int id)
        {
            return Collection.Any(p => p.BarCode == barcode && p.Id != id);
        }
        public List<CustomerVendorCodesEx> GetByOwnVendorCodes()
        {
            return Collection.Where(p => p.OwnVendorCode == true).Select(p => new CustomerVendorCodesEx { AccountId = p.AccountId, VendorCode = p.VendorCode }).ToList();
        }
        public AccountDetail GetByVendorCode(string vendorcode, int id)
        {
            return Collection.Where(p => p.VendorCode == vendorcode && p.Id != id).FirstOrDefault();
        }
        public bool CheckIfVendorCodeExist(string vendorcode, int id)
        {
            return Collection.Any(p => p.VendorCode == vendorcode && p.Id != id);
        }
        public string GetVendorCode(int id)
        {
            var venderCode = "";
            if (new AccountDetailRepository().AsQueryable().Any(p => p.AccountId == id))
                venderCode = new AccountDetailRepository().AsQueryable().FirstOrDefault(p => p.AccountId == id).VendorCode;
            return venderCode;
        }
        public List<VehicleConfigExtra> GetVehcileConfig(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.AccountId)).Select(p => new VehicleConfigExtra()
            {
                AccountId = p.AccountId,
                AccountName = p.Name,
                IsUniquePerVehicle = p.IsUniquePerVehicle,
                IsVehicleRequired = p.IsVehicleRequired
            }).ToList();
        }

    }
}