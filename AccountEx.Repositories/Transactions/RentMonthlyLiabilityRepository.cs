using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AutoMapper;
using System.Globalization;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class RentMonthlyLiabilityRepository : GenericRepository<RentDetail>
    {
        public RentMonthlyLiabilityRepository() : base() { }
        public RentMonthlyLiabilityRepository(BaseRepository repo)
        {
            Db = repo.GetContext();
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public int GetVoucherNoByMonthYear(int month, int year)
        {
            if (FiscalCollection.Any(p => p.Month == month && p.Year == year))
                return FiscalCollection.FirstOrDefault(p => p.Month == month && p.Year == year).VoucherNumber;
            else
                return 0;

        }
        public RentDetail GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public RentMonthYear GetLastRentMonthYear()
        {
            return FiscalCollection.OrderByDescending(p => p.Id).Select(p => new RentMonthYear()
            {
                Month = p.Month,
                Year = p.Year
            }).FirstOrDefault();

        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool CheckIfRentLiabilityGenerated(int rentAgreementId)
        {
            return FiscalCollection.Any(p => p.RentDetailItems.Any(i => i.RentAgreementId == rentAgreementId));

        }
        public RentDetail GetByVoucherNumber(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsVoucherNumberExist(int id, int voucherno, VoucherType type)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public RentDetail GetByVoucherNumber(int voucherno)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public void UpdateRentLiability(RentDetail entity)
        {
            foreach (var item in entity.RentDetailItems)
            {
                item.RentDetailId = entity.Id;
                Db.RentDetailItems.Add(item);
            }
            entity.RentDetailItems = null;
            base.Update(entity);
        }
        public List<ChallanExtra> GetLiabilityByMonthYear(int month, int year)
        {
            // DateTime date = new DateTime(year, month, 01);
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);
            DateTime date = new DateTime(year, month, lastDayOfMonth);
            var query = string.Format("EXEC [DBO].[GetRentMonthlyLiability2] @CompanyId = {0},@FiscalId={1}, @Month = {2}, @Year={3},@date={4}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, month, year, "'" + date.ToString("yyyy/MM/dd") + "'");
            return Db.Database.SqlQuery<ChallanExtra>(query).ToList();
        }

        public RentDetail GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            RentDetail v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;
                case "RentDetail":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "same")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        public void DeleteByVoucherNo(int voucherno)
        {
            var RentDetail = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Db.RentDetails.Remove(RentDetail);
            SaveLog(RentDetail, ActionType.Deleted);
            Db.SaveChanges();
        }

        public override void Save(RentDetail input)
        {
            //var repo = new ChallanRepository();
            //if (input.Id == 0)
            //{
            //    input.Year = DateTime.Now.Year;
            //    repo.Add(input);
            //}
            //else
            //{
            //    repo.Update(input);
            //}
        }

        //public override void Update(RentDetail input)
        //{
        //    var dbMisc = FiscalCollection.FirstOrDefault(p => p.Id == input.Id);
        //    var query = "Delete from RentDetailItems where RentDetailId=" + input.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
        //    Db.Database.ExecuteSqlCommand(query);
        //    foreach (var item in input.RentDetailItems)
        //    {
        //        Db.RentDetailItems.Add(item);
        //    }
        //    SaveChanges();
        //    input.RentDetailItems = null;

        //    Db.Entry(dbMisc).CurrentValues.SetValues(input);
        //    SaveChanges();
        //}

        public List<ChallanExtra> GetRentDetailByMonthYear(int month, int year)
        {
            var query = string.Format("EXEC [DBO].[GetMonthlyRentDetails] @CompanyId = {0},@FiscalId={1}, @Month = {2}, @Year={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, month, year);
            return Db.Database.SqlQuery<ChallanExtra>(query).ToList();
        }

        public RentDetail GetByMonthYear(int month, int year)
        {

            return FiscalCollection.AsNoTracking().FirstOrDefault(p => p.Month == month && p.Year == year);

        }
        public bool IsExistByRentAgreementId(int agreementId, int month, int year, int rentItemId)
        {
            return Collection.Any(p => p.Month == month && p.Year == year && p.RentDetailItems.Any(q => q.RentAgreementId == agreementId && p.Id != rentItemId));
        }
        public RentChallanExta GetByMonthYearTenant(int month, int year, int toMonth, int toYear, int accountId, int agreementId, bool loadAllPrevious = false)
        {

            var fromDate = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(toYear, toMonth);
            var toDate = new DateTime(toYear, toMonth, days);

            var query = string.Format("EXEC [DBO].[GetRentMonthlyLiabilityByAgreementId] @AgreementId = {0}, @FromDate = {1}, @ToDate = {2}, @COMPANYID = {3}, @FiscalId = {4}"
                , agreementId, "'" + fromDate.ToString("yyyy-MM-dd") + "'", "'" + toDate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            var rd = Db.Database.SqlQuery<RentDetailExtra>(query).FirstOrDefault();

            var paidChallanQuery = AsQueryable<Challan>(true).
               Where(p => (fromDate <= EntityFunctions.TruncateTime(p.ToDate) && fromDate >= EntityFunctions.TruncateTime(p.FromDate) ||
        EntityFunctions.TruncateTime(p.FromDate) <= toDate && EntityFunctions.TruncateTime(p.FromDate) >= fromDate));
            var paidChallan = paidChallanQuery.Where(p => p.RentAgreementId == agreementId && p.TenantAccountId.Value == accountId && p.TransactionType  == VoucherType.RC && p.IsReceived)
                .SelectMany(p => p.ChallanItems).GroupBy(p => p.TenantAccountId).Select(p => new PaidRentChallanExta()
                {
                    MonthlyRent = p.Sum(q => q.MonthlyRent),
                    ElectricityCharges = p.Sum(q => q.ElectricityCharges),
                    UCPercent = p.Sum(q => q.UCPercent),
                    RentArrears = p.Sum(q => q.RentArrears),
                    ElectricityArrears = p.Sum(q => q.ElectricityArrears),
                    UCPercentArears = p.Sum(q => q.UCPercentArears),
                    SurCharge = p.Sum(q => q.SurCharge),
                    TotalAmount = Math.Round(p.Sum(q => q.MonthlyRent + q.UCPercent + q.RentArrears + q.UCPercentArears + q.SurCharge), 0)
                }).FirstOrDefault();

            var allChallanQuery = AsQueryable<Challan>(true).
                Where(p => (EntityFunctions.TruncateTime(fromDate.Date) <= EntityFunctions.TruncateTime(p.ToDate) && EntityFunctions.TruncateTime(fromDate.Date) >= EntityFunctions.TruncateTime(p.FromDate) ||
         EntityFunctions.TruncateTime(p.FromDate) <= EntityFunctions.TruncateTime(toDate.Date) && EntityFunctions.TruncateTime(p.FromDate) >= EntityFunctions.TruncateTime(fromDate.Date)));
            if (loadAllPrevious)
                allChallanQuery = AsQueryable<Challan>(true);
            var allChallan = allChallanQuery.
                Where(p => p.RentAgreementId == agreementId && p.TenantAccountId.Value == accountId && p.TransactionType  == VoucherType.RC).OrderByDescending(p=>p.ToDate)
                .Select(p => new AllChallanExtra()
                {
                    VoucherNumber = p.VoucherNumber,
                    Id = p.Id,
                    Month = p.Month,
                    ToMonth = p.ToMonth,
                    Year = p.Year,
                    ToYear = p.ToYear,
                    IsAuto = p.IsAuto,
                    DueDate = p.DueDate,
                    IsReceived = p.IsReceived,
                    RcvNo = p.RcvNo,
                    MonthlyRent = Math.Round(p.ChallanItems.Sum(q => q.MonthlyRent), 0),
                    ElectricityCharges = Math.Round(p.ChallanItems.Sum(q => q.ElectricityCharges), 0),
                    UCPercent = Math.Round(p.ChallanItems.Sum(q => q.UCPercent), 0),
                    RentArrears = Math.Round(p.ChallanItems.Sum(q => q.RentArrears), 0),
                    ElectricityArrears = Math.Round(p.ChallanItems.Sum(q => q.ElectricityArrears), 0),
                    UCPercentArears = Math.Round(p.ChallanItems.Sum(q => q.UCPercentArears), 0),
                    SurCharge = Math.Round(p.ChallanItems.Sum(q => q.SurCharge), 0),
                    TotalAmount = Math.Round(p.ChallanItems.Sum(q => q.MonthlyRent + q.UCPercent + q.ElectricityCharges + q.RentArrears + q.ElectricityArrears + q.UCPercentArears + q.SurCharge), 0)
                }).ToList();




            return new RentChallanExta() { PaidRentChallan = paidChallan, RentDetail = rd, AllChallans = allChallan };
        }






        public ElectricityChallanExtra GetElectrictyByMonthYearTenant(int month, int year, int accountId, int agreementId, bool loadAllPrevious = false)
        {


            var query = string.Format("EXEC [DBO].[GetMonthlyElectricityByAgreementId] @AgreementId = {0}, @Month = {1}, @Year = {2}, @COMPANYID = {3}, @FiscalId = {4}"
               , agreementId, month, year, SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            var electricity = Db.Database.SqlQuery<ElectricityChallan>(query).FirstOrDefault();



            var paidChallanQuery = AsQueryable<Challan>(true).
                Where(p => p.Year == year && p.Month == month);



            var paidChallan = paidChallanQuery.
                Where(p => p.RentAgreementId == agreementId && p.TenantAccountId.Value == accountId && p.TransactionType  == VoucherType.ElectictyChallan && p.IsReceived)
                .SelectMany(p => p.ChallanItems).GroupBy(p => p.TenantAccountId).Select(p => new PaidRentChallanExta()
                {
                    ElectricityCharges = p.Sum(q => q.ElectricityCharges),
                    ElectricityArrears = p.Sum(q => q.ElectricityArrears),
                    SurCharge = p.Sum(q => q.SurCharge),
                }).FirstOrDefault();



            var allChallanQuery = AsQueryable<Challan>(true).
                Where(p => p.Year == year && p.Month == month);
            if (loadAllPrevious)
                allChallanQuery = AsQueryable<Challan>(true);

            var allChallan = allChallanQuery.
                Where(p => p.RentAgreementId == agreementId && p.TenantAccountId.Value == accountId && p.TransactionType  == VoucherType.ElectictyChallan).OrderByDescending(p => p.ToDate)
                .Select(p => new AllChallanExtra()
                {
                    VoucherNumber = p.VoucherNumber,
                    Id = p.Id,
                    Month = p.Month,
                    ToMonth = p.ToMonth,
                    Year = p.Year,
                    ToYear = p.ToYear,
                    RcvNo = p.RcvNo,
                    IsAuto = p.IsAuto,
                    DueDate = p.DueDate,
                    IsReceived = p.IsReceived,
                    ElectricityCharges = Math.Round(p.ChallanItems.Sum(q => q.ElectricityCharges), 0),
                    ElectricityArrears = Math.Round(p.ChallanItems.Sum(q => q.ElectricityArrears), 0),
                    SurCharge = Math.Round(p.ChallanItems.Sum(q => q.SurCharge), 0),
                    TotalAmount = p.ChallanItems.Sum(q => q.ElectricityCharges + q.ElectricityArrears + q.SurCharge)
                }).ToList();




            return new ElectricityChallanExtra() { PaidRentChallan = paidChallan, ElectricityChallan = electricity, AllChallans = allChallan };
        }

        public PrintRentChallanExtra PrintRentChallan(int challanId)
        {
            var query = string.Format("EXEC [DBO].[PrintRentChallan] @CompanyId = {0},@ChallanId={1}",
               SiteContext.Current.User.CompanyId, challanId);
            return Db.Database.SqlQuery<PrintRentChallanExtra>(query).FirstOrDefault();
        }
        public PrintRentChallanExtra PrintElectricityChallan(int challanId)
        {
            var query = string.Format("EXEC [DBO].[PrintElectricityChallan] @CompanyId = {0},@ChallanId={1}",
               SiteContext.Current.User.CompanyId, challanId);
            return Db.Database.SqlQuery<PrintRentChallanExtra>(query).FirstOrDefault();
        }



        public Object GetElecCharges(int? ElectricityUnitItemId)
        {
            var eCharges = new GenericRepository<ElectricityUnitItem>().AsQueryable().FirstOrDefault(p => p.Id == ElectricityUnitItemId);

            var elecCharges = new Object();
            if (eCharges != null)
                elecCharges = new
                {
                    PrevReading = eCharges.PreviousReading.Value,
                    CurReading = eCharges.CurrentReading.Value,
                    Units = eCharges.Unit.Value,
                };
            return elecCharges;
        }
    }
}