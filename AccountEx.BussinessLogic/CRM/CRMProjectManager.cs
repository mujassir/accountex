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

    public static class CRMProjectManager
    {



        public static void Save(CRMProject project)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMProjectRepository();
                var Salerepo = new CRMSaleInvoiceRepository(repo);
                var tranrRepo = new TransactionRepository(repo);
                if (project.Id == 0)
                {

                    project.VoucherNumber = repo.GetNextVoucherNumber();
                    repo.Add(project);
                }
                else
                {
                    repo.Update(project);

                }
                repo.SaveChanges();
                Salerepo.ChangeAllToProjectSale(project.Id, project.CustomerId, project.ProductId);

                scope.Complete();

            }

        }
        public static void ChangeSaleType(int customerId, int productId)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMProjectRepository();
                var saleRepo = new CRMSaleInvoiceRepository(repo);
                if (productId > 0)
                {
                    var project = repo.GetProject(customerId, productId);
                    if (project != null)
                    {
                        saleRepo.ChangeAllToProjectSale(project.Id, project.CustomerId, project.ProductId);
                    }
                }
                else
                {
                    var products = new CRMProjectRepository(repo).GetProjectsProduct(customerId);
                    foreach (var product in products)
                    {
                        var project = repo.GetProject(customerId, product.Id);
                        if (project != null)
                        {
                            saleRepo.ChangeAllToProjectSale(project.Id, project.CustomerId, project.ProductId);
                        }
                    }

                }
                repo.SaveChanges();
                scope.Complete();

            }

        }



        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMProjectRepository();
                var Salerepo = new CRMSaleInvoiceRepository(repo);
                Salerepo.ChangeAllToRegulerSale(id);
                repo.Delete(id);
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



        public static string ValidateSave(CRMProject input)
        {
            var err = ",";
            try
            {
                var projectRepo = new CRMProjectRepository();
                var saleRepo = new CRMSaleInvoiceRepository(projectRepo);
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
                if (input.ProductId == 0)
                {
                    err += "Please select the product.,";
                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (!SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                {
                    if (FiscalSettingManager.IsProjectLocked)
                    {
                        err += "Project Module is locked by admin.,";

                    }
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                //if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                //{
                //    err += "Voucher date should be within current fiscal year.,";
                //}

                //var isExist = projectRepo.IsVoucherExits(input.VoucherNumber, input.Id);
                //if (isExist)
                //{
                //    err += "Voucher no already exist.,";
                //}
                var isPmcExist = projectRepo.IsProjectExits(input.CustomerId, input.ProductId, input.Id);
                if (isPmcExist)
                {

                    err += "Only one project per customer per product is allowed.,";
                }



                if (input.Id > 0)
                {
                    var dbProject = projectRepo.GetById(input.Id, true);
                    if (dbProject.CustomerId != input.CustomerId || dbProject.ProductId != input.ProductId || dbProject.CurrencyId != input.CurrencyId)
                    {
                        if (saleRepo.IsInvoiceExist(dbProject.CustomerId, dbProject.ProductId))
                        {
                            err += "Customer and product are linked with sale invoice and can't be changed in current project.";
                        }
                    }
                    if (dbProject.FiscalId != SiteContext.Current.Fiscal.Id)
                    {
                        err += "Only current logged in year record can be updated.,";
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
        public static string ValidateSaleTypeChange(int customerId, int productId)
        {
            var err = ",";
            try
            {
                var projectRepo = new CRMProjectRepository();
                var saleRepo = new CRMSaleInvoiceRepository(projectRepo);
                if (!SiteContext.Current.User.IsAdmin || !(SiteContext.Current.UserTypeId == CRMUserType.Admin))
                {
                    err += "you did not have sufficent right to perform this operation.,";
                }
                if (customerId == 0)
                {
                    err += "Please select the customer.,";
                }
                //if (productId == 0)
                //{
                //    err += "Please select the product.,";
                //}

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (!SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                {
                    if (FiscalSettingManager.IsProjectLocked)
                    {
                        err += "Project Module is locked by admin.,";

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
        public static string ValidatDelete(int id)
        {
            var err = ",";
            try
            {
                var projectRepo = new CRMProjectRepository();
                var saleRepo = new CRMSaleInvoiceRepository(projectRepo);

                if (!SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                {

                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete project.,";
                    }

                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (!SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                {
                    if (FiscalSettingManager.IsProjectLocked)
                    {
                        err += "Project Module is locked by admin.,";

                    }
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var dbProject = projectRepo.GetById(id, true);
                if (dbProject == null)
                {
                    err += "No Project found for deletion.,";
                }
                else
                {
                    if (saleRepo.IsInvoiceExist(dbProject.CustomerId, dbProject.ProductId) && !SiteContext.Current.User.IsAdmin && SiteContext.Current.UserTypeId != CRMUserType.Admin)
                    {
                        err += "project are linked with sale invoice and can't be deleted.";
                    }

                    if (dbProject.FiscalId != SiteContext.Current.Fiscal.Id)
                    {
                        err += "Only current logged in year record can be deleted.,";
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
    }
}
