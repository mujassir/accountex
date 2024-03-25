using System.Collections.Generic;
using System.Web.Mvc;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using AccountEx.Web.Code;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Repositories.COA;
using Entities.CodeFirst;
using AccountEx.Repositories.Config;
using System.Transactions;

namespace AccountEx.Web.Controllers.mvc
{
    [Compress]
    public class RentController : BaseController
    {
        //
        // GET: /Project/


        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Tenants()
        {
            return View("~/Views/RentalSystem/Tenants.cshtml");
        }
        public ActionResult RentalAccountStatement()
        {

            return View("~/Views/RentalSystem/Report/RentalAccountStatement.cshtml");
        }
      
        public ActionResult OpeningBalances()
        {
            return View("~/Views/RentalSystem/OpeningBalances.cshtml");
        }
        public ActionResult RecoveryOfPossessionCharges()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View("~/Views/RentalSystem/Report/RecoveryOfPossessionCharges.cshtml");
        }
        public ActionResult RecoveryOfPossessionCharges1()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View("~/Views/RentalSystem/Report/RecoveryOfPossessionCharges1.cshtml");
        }
        public ActionResult DetailOfOverallBillsIssueToTenants()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View("~/Views/RentalSystem/Report/DetailOfOverallBillsIssueToTenants.cshtml");
        }
        public ActionResult RecoveryOfRent()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            return View("~/Views/RentalSystem/Report/RecoveryOfRent.cshtml");
        }
        public ActionResult OverallRecoveryReport()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            return View("~/Views/RentalSystem/Report/OverallRecoveryReport.cshtml");
        }
        public ActionResult AccountStatement()
        {

            return View("~/Views/RentalSystem/Report/AccountStatement.cshtml");
        }

        public ActionResult AccountStatement1()
        {

            return View("~/Views/RentalSystem/Report/AccountStatement1.cshtml");
        }



        public ActionResult ShopSetup()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            return View("~/Views/RentalSystem/shopsetup.cshtml");
        }
        public ActionResult Blocks()
        {

            return View("~/Views/RentalSystem/Blocks.cshtml");
        }
        public ActionResult RentAgreement()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            //if (Request["setdata"] == "data")
            //{
            //    try
            //    {


            //        var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(1, 0, 0) };
            //        using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //        {

            //            var repo = new RentAgreementRepository();
            //            var agreements = repo.GetAll();
            //            foreach (var agreement in agreements)
            //            {
            //                RentAgreementManager.AddTransaction(agreement, repo);
            //                repo.SaveChanges();
            //            }

            //            scope.Complete();
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {
            //        ;

            //    }
            //}

            return View("~/Views/RentalSystem/RentAgreement.cshtml");
        }
        public ActionResult BankReceipts()
        {

            if (Request["setdata"] == "data")
            {
                try
                {


                    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(1, 0, 0) };
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
                    {

                        var repo = new ChallanRepository();
                        var challans = repo.AsQueryable().Where(p => p.IsReceived).GroupBy(p => p.RcvNo).Select(p => new
                            {
                                RcvNo = p.Key,
                                Challans = p
                            }).ToList();
                        foreach (var challan in challans)
                        {
                            ChallanManager.AddRentalReceivingTransaction(challan.Challans.ToList(), repo, challan.RcvNo);
                            repo.SaveChanges();
                        }

                        scope.Complete();
                    }
                }
                catch (System.Exception ex)
                {
                    ;

                }
            }
            return View("~/Views/RentalSystem/BankReceipts.cshtml");
        }
        public ActionResult RentMonthlyLiability()
        {

            return View("~/Views/RentalSystem/RentMonthlyLiability.cshtml");
        }
        public ActionResult MiscCharges()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Charges", Value = SettingManager.MiscChargesHeadId });
            setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/RentalSystem/MiscCharges.cshtml");
        }
        public ActionResult Transfers()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/RentalSystem/Transfers.cshtml");
        }
        public ActionResult ElectricityUnits()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "ElectriciyUnitCharges", Value = SettingManager.ElectricityPerUnitCost });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            //ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            //ViewBag.Blocks = new BlockRepository().GetNames();
            return View("~/Views/RentalSystem/ElectricityUnits.cshtml");
        }
        public ActionResult Challans()
        {
            var setting = new List<SettingExtra>();
            //setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            setting.Add(new SettingExtra() { Key = "DueDate", Value = SettingManager.RentSecurityPossessionDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/RentalSystem/Challans.cshtml");
        }
        public ActionResult MiscChargesChallans()
        {
            var setting = new List<SettingExtra>();
            //setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.Charges = new AccountRepository().GetLeafAccount(SettingManager.MiscHeadId);
            return View("~/Views/RentalSystem/MiscChargesChallans.cshtml");
        }
        public ActionResult RentBill()
        {
            //ViewBag.Tenants = new AccountDetailRepository().GetNames(AccountDetailFormType.Tenant);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/RentalSystem/RentBill.cshtml");
        }
        public ActionResult ElectricityBill()
        {
            //ViewBag.Tenants = new AccountDetailRepository().GetNames(AccountDetailFormType.Tenant);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            setting.Add(new SettingExtra() { Key = "UploadFolder", Value = SiteContext.Current.UploadFolder });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/RentalSystem/ElectricityBill.cshtml");
        }


    }
}
