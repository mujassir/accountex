using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{

    public static class PMCManager
    {



        public static void Save(PMC sale)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new PMCRepository();
                var tranrRepo = new TransactionRepository(repo);
                if (sale.Id == 0)
                {

                    sale.VoucherNumber = repo.GetNextVoucherNumber();
                    repo.Add(sale);

                }
                else
                {
                    repo.Update(sale);

                }
                repo.SaveChanges();
                scope.Complete();

            }

        }




        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new PMCRepository();
                var itemRepo = new PMCItemRepository(repo);
                itemRepo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void DeleteAll(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new PMCRepository();
                var itemRepo = new PMCItemRepository(repo);

                var dbPMC = repo.GetById(id, true);
                //add,update & remove services items
                var Ids = new List<int>();

                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    Ids.AddRange(dbPMC.PMCItems.Where(p => p.SalePersonId == SiteContext.Current.User.Id).Select(p => p.Id).ToList());
                    itemRepo.Delete(Ids);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                {
                    var dbProductIds = dbPMC.PMCItems.Select(p => p.ProductId).ToList();
                    var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                    Ids.AddRange(dbPMC.PMCItems.Where(p => DHproductIds.Contains(p.ProductId)).Select(p => p.Id).ToList());
                    itemRepo.Delete(Ids);
                }
                else
                {
                    repo.Delete(id);
                }
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void Delete(int voucherno, List<VoucherType> transactionTypes)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var tranRepo = new TransactionRepository();
                var saleRepo = new SaleRepository(tranRepo);
                tranRepo.HardDelete(voucherno, transactionTypes);
                saleRepo.DeleteByVoucherNumber(voucherno, transactionTypes);
                saleRepo.SaveChanges();
                scope.Complete();
            }

        }



        public static string ValidateSave(PMC input)
        {
            var err = ",";
            try
            {
                var pmcRepo = new PMCRepository();
                var fiscalSettingRepo = new PMCRepository();
                var fiscalSettingRepor =new FiscalSettingRepository(pmcRepo);
                var productRepo = new CRMProductRepository(pmcRepo);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new voucher.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update voucher.,";
                        }
                    }
                }

                if (input.CustomerId == 0)
                {
                    err += "Please select the customer.,";
                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (!SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                {
                    if (fiscalSettingRepor.IsPmcLocked())
                    {
                        err += "PMC Module is locked by admin.,";

                    }

                    var debugInfo = "IsAdmin:" + SiteContext.Current.User.IsAdmin +
                        " UserTypeId:" + SiteContext.Current.UserTypeId +
                        "IsPmcLocked:" + FiscalSettingManager.IsPmcLocked;
                }

                var products = productRepo.GetProductIdName(input.PMCItems.Select(p => p.ProductId).ToList());
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }

                //var isExist = pmcRepo.IsVoucherExits(input.VoucherNumber, input.Id);
                //if (isExist)
                //{
                //    err += "Voucher no already exist.,";
                //}
                var isPmcExist = pmcRepo.IsPMCExits(input.CustomerId, input.Id);
                if (isPmcExist)
                {

                    err += "Only one PMC per customer is allowed.,";
                }


                foreach (var item in input.PMCItems.Where(p => p.ProductId == 0))
                {

                    err += "Please select product for all record.,";
                }
                foreach (var item in input.PMCItems.Where(p => p.AnnualQty <= 0 || p.Price <= 0 || p.ExcRate==0))
                {
                    var productName = "";
                    if (products.Any(p => p.Id == item.ProductId))
                        productName = products.FirstOrDefault(p => p.Id == item.ProductId).Name;

                    err += productName + "must have price,exchange rate or quantity must greater than zero(0).,";
                }

                var Itemcountlist = input.PMCItems.GroupBy(p => new { p.ProductId, p.CurrencyId }).Select(p => new
                {
                    ProductId = p.Key.ProductId,
                    CurrenycyId = p.Key.CurrencyId,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    var productName = "";
                    if (products.Any(p => p.Id == item.ProductId))
                        productName = products.FirstOrDefault(p => p.Id == item.ProductId).Name;
                    err += productName + " must be added once in list for same currency.(Current Count:" + item.Count + "),";
                }

                if (input.Id > 0)
                {
                    var dbPmc = pmcRepo.GetById(input.Id, true);
                    if (dbPmc.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no.please use previous voucher no.(" + dbPmc.VoucherNumber + "),";
                    }

                    if (dbPmc.FiscalId != SiteContext.Current.Fiscal.Id)
                    {
                        err += "Only current logged in year record can be updated.,";
                    }

                }
            }
            catch (Exception ex)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
    }
}
