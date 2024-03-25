using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AccountEx.CodeFirst.Models;
using System.Data.Entity.Core.Objects;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class ChallanRepository : GenericRepository<Challan>
    {
        public ChallanRepository() : base() { }
        public ChallanRepository(BaseRepository repo)
        {
            Db = repo.GetContext();
        }

        public override void Save(Challan input)
        {
            var repo = new ChallanRepository();
            var transRepo = new TransactionRepository(repo);

            if (input.Id == 0)
            {
                if (input.VoucherNumber == 0)
                    input.VoucherNumber = transRepo.GetNextVoucherNumber(input.TransactionType);
                if (input.InvoiceNumber == 0)
                    input.InvoiceNumber = transRepo.GetNextInvoiceNumber(input.TransactionType);
                foreach (var item in input.ChallanItems)
                {
                    item.VoucherNumber = input.VoucherNumber;
                    item.InvoiceNumber = input.InvoiceNumber;
                }
                repo.Add(input, true, false);
            }
            else
            {
                repo.Update(input);
            }
            repo.SaveChanges();
        }
        public List<Challan> GetByIds(List<int> Ids)
        {
            return Collection.Where(p => Ids.Contains(p.Id)).ToList();
        }
        public List<Challan> GetByRcvNo(int rcvNo)
        {
            return Collection.Where(p => p.RcvNo == rcvNo).ToList();
        }

        public List<Challan> GetChallanById(int id)
        {
            return Collection.Where(p => p.Id == id).ToList();
        }
        public PrintOtherChallanExtra PrintChallanById(int challanId)
        {
            var query = string.Format("EXEC [DBO].[PrintChallanById] @CompanyId = {0},@ChallanId={1}",
               SiteContext.Current.User.CompanyId, challanId);
            return Db.Database.SqlQuery<PrintOtherChallanExtra>(query).FirstOrDefault();
        }
        public RentMonthYear GetLastRentMonthYear(int agreementId, VoucherType type)
        {
            return FiscalCollection.Where(p => p.RentAgreementId == agreementId && p.TransactionType == type).OrderByDescending(p => p.Id).Select(p => new RentMonthYear()
            {
                Month = p.Month,
                Year = p.Year
            }).FirstOrDefault();

        }
        public PrintMiscChallanExtra PrintMiscChallanById(int challanId)
        {
            var query = string.Format("EXEC [DBO].[PrintMiscChallanById] @CompanyId = {0},@ChallanId={1}",
               SiteContext.Current.User.CompanyId, challanId);
            return Db.Database.SqlQuery<PrintMiscChallanExtra>(query).FirstOrDefault();
        }


        public Challan GetByMonthYear(int month, int year)
        {

            return FiscalCollection.AsNoTracking().FirstOrDefault(p => p.Month == month && p.Year == year);

        }
        public int GetVoucherNoByMonthYear(int month, int year)
        {
            if (FiscalCollection.Any(p => p.Month == month && p.Year == year))
                return FiscalCollection.FirstOrDefault(p => p.Month == month && p.Year == year).VoucherNumber;
            else
                return 0;

        }
        public bool IsOpeningChallanExist(int agreementId, VoucherType type)
        {

            return FiscalCollection.
                Any(p => p.RentAgreementId == agreementId && p.TransactionType == type && p.IsOpening);

        }
        public Challan GetChallan(DateTime fromDate, DateTime toDate, int agreementId)
        {

            return FiscalCollection.
                FirstOrDefault(p => (fromDate <= EntityFunctions.TruncateTime(p.ToDate) && fromDate >= EntityFunctions.TruncateTime(p.FromDate) ||
         EntityFunctions.TruncateTime(p.FromDate) <= toDate && EntityFunctions.TruncateTime(p.FromDate) >= fromDate) && p.RentAgreementId == agreementId && p.TransactionType  == VoucherType.RC);

        }

        public bool IfLastChallan(int challanId, VoucherType type)
        {


            var currentChallan = Collection.Where(p => p.Id == challanId && p.TransactionType == type).OrderByDescending(p => p.Id).FirstOrDefault();
            var lastChallan = Collection.Where(p => p.RentAgreementId == currentChallan.RentAgreementId && p.TransactionType == type).OrderByDescending(p => p.Id).FirstOrDefault();
            if (lastChallan == null)
                return true;
            else return lastChallan.Id == currentChallan.Id;




        }


        public Challan GetLastPeriodChallan(DateTime fromDate, int agreementId, VoucherType type)
        {

            return FiscalCollection.Where(p => fromDate >= EntityFunctions.TruncateTime(p.ToDate) && p.RentAgreementId == agreementId && p.TransactionType == type && !p.IsOpening).OrderByDescending(p => p.ToDate).Take(1).FirstOrDefault();

        }
        public Challan GetLastPeriodChallan(int agreementId, VoucherType type)
        {

            return FiscalCollection.Where(p => p.RentAgreementId == agreementId && p.TransactionType == type && !p.IsOpening).OrderByDescending(p => p.ToDate).Take(1).FirstOrDefault();

        }
        public List<AllChallanExtra> GetAllByAgreemntId(int agreemnetId, VoucherType transactionType)
        {
            var allChallan = AsQueryable<Challan>(true).
             Where(p => p.RentAgreementId.Value == agreemnetId && p.TransactionType == transactionType)
             .Select(p => new AllChallanExtra()
             {
                 VoucherNumber = p.VoucherNumber,
                 Id = p.Id,
                 DueDate = p.DueDate,
                 IsReceived = p.IsReceived,
                 RcvNo = p.RcvNo,
                 TotalAmount = p.ChallanItems.Sum(q => q.NetAmount)
             }).OrderByDescending(p => p.Id).ToList();
            return allChallan;
        }
        public List<AllChallanExtra> GetAllByAgreemntId(int agreemnetId, VoucherType transactionType, int noOfRecord)
        {
            var allChallan = AsQueryable<Challan>(true).
             Where(p => p.RentAgreementId.Value == agreemnetId && p.TransactionType == transactionType).OrderByDescending(p => p.Id).Take(noOfRecord)
             .Select(p => new AllChallanExtra()
             {
                 VoucherNumber = p.VoucherNumber,
                 Id = p.Id,
                 DueDate = p.DueDate,
                 IsReceived = p.IsReceived,
                 TotalAmount = p.ChallanItems.Sum(q => q.NetAmount)
             }).ToList();
            return allChallan;
        }
        public bool IsChallanReceived(int id)
        {
            return Collection.Any(p => p.Id == id && p.IsReceived);
        }

        public override void Update(Challan entity)
        {
            var query = "Delete From ChallanItems Where ChallanId=" + entity.Id + " And CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in entity.ChallanItems)
            {
                item.ChallanId = entity.Id;
                Db.ChallanItems.Add(item);
            }
            entity.ChallanItems = null;
            base.Update(entity);
        }
        public int GetNextInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public bool IsVoucherExits(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public Challan GetByVoucherNumber(int voucherno, VoucherType vtype, byte entrytype, string key, out bool next, out bool previous)
        {
            Challan v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "same")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype && p.EntryType == entrytype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.EntryType == entrytype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.EntryType == entrytype && p.VoucherNumber < voucherno);
            return v;
        }
        public Challan GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            Challan v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
        public List<Challan> GetByRcvNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            Transaction t = null;
            List<Challan> challans = null;
            var types = new List<VoucherType> { VoucherType.RC, VoucherType.ElectictyChallan,VoucherType.SecurityMoney, VoucherType.PossessionCharges, VoucherType.MiscCharges };
            var query = AsQueryable<Transaction>(true);
            var transVoucherNo = 0;
            switch (key)
            {
                case "first":
                    t = query.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    t = query.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    t = query.Where(p => p.TransactionType == vtype).Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    t = query.Where(p => p.TransactionType == vtype).Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    t = query.Where(p => p.TransactionType == vtype).FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (t != null)
                voucherno = t.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                t = query.Where(p => types.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (t != null)
            {
                challans = FiscalCollection.Where(p => types.Contains(p.TransactionType) && p.RcvNo == t.VoucherNumber).OrderByDescending(p => p.VoucherNumber).ToList();
            }
            next = query.Any(p => types.Contains(p.TransactionType) && p.VoucherNumber > voucherno);
            previous = query.Any(p => types.Contains(p.TransactionType) && p.VoucherNumber < voucherno);
            return challans;
        }

        public List<ChallanExtra> GetChallanByMonthYear(int month, int year)
        {
            // DateTime date = new DateTime(year, month, 01);
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);
            DateTime date = new DateTime(year, month, lastDayOfMonth);
            var query = string.Format("EXEC [DBO].[GetRentMonthlyLiability2] @CompanyId = {0},@FiscalId={1}, @Month = {2}, @Year={3},@date={4}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, month, year, "'" + date.ToString("yyyy/MM/dd") + "'");
            return Db.Database.SqlQuery<ChallanExtra>(query).ToList();
        }

        public bool CheckIfRentLiabilityGenerated(int rentAgreementId)
        {
            return FiscalCollection.AsQueryable().Any(p => p.Month > 0 && p.Year > 0 && p.ChallanItems.Any(i => i.TransactionType  == VoucherType.RentMonthlyLiability && i.RentAgreementId == rentAgreementId));

        }
        public bool CheckIfChallanExist(int rentAgreementId)
        {
            return FiscalCollection.AsQueryable().Any(p => p.RentAgreementId == rentAgreementId);

        }


    }

}