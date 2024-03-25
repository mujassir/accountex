using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using AccountEx.Common.CRM;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{

    public static class CRMIGRNManager
    {



        public static void Save(List<CRMImportGRN> records)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                records.ForEach(c =>
                {
                    c.FiscalId = SiteContext.Current.Fiscal.Id;
                    c.DeliveryType = c.CurrencyId == 23 ? CRMSaleDeliveryType.ExStock : CRMSaleDeliveryType.Import;
                });
                records.Where(p => p.Id == 0).ToList().ForEach(c => { c.Date = DateTime.Now; });
                var repo = new CRMImportIGRNRepository();
                var tranrRepo = new TransactionRepository(repo);
                repo.Save(records);
                repo.SaveChanges();
                scope.Complete();

            }

        }
        public static void Import(CRMIGRNImportContainerExtra input, bool isValidate)
        {

            var records = input.IGRN;
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMImportIGRNRepository();
                var customerRepo = new CRMCustomerRepository(repo);
                var productRepo = new CRMProductRepository(repo);
                var currencyRepo = new CurrencyRepository(repo);
                var projectRepo = new CRMProjectRepository(repo);
                var tranrRepo = new TransactionRepository(repo);

                var err = "";
                var igrn = new CRMImportGRN();
                foreach (var item in records)
                {
                    var invDate = DateTime.MinValue;
                    var blDate = DateTime.MinValue;
                    var piDate = DateTime.MinValue;
                    DateTime.TryParseExact(item.InvoiceDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out invDate);
                    DateTime.TryParseExact(item.BLRRDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out blDate);
                    DateTime.TryParseExact(item.PIDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out piDate);

                    //if (string.IsNullOrWhiteSpace(item.InvoiceNumber))
                    //{
                    //    err += "Invoice no is required.,";
                    //}
                    if (string.IsNullOrWhiteSpace(item.Customer))
                    {
                        err += "Customer is required.,";
                    }

                    //if (invDate == DateTime.MinValue)
                    //{
                    //    err += "invoice date is missing or invalid.,";
                    //}
                    //if (blDate == DateTime.MinValue)
                    //{
                    //    err += "BL/RR  date is missing or invalid.,";
                    //}
                    if (item.LCValue <= 0)
                    {
                        err += "LC amount must be greater than zero.,";
                    }
                    if (!customerRepo.IsExist(item.Customer, 0))
                    {
                        err += "No customer exist in the system with specific name (" + item.Customer + ").,";
                    }

                    if (!productRepo.IsExist(item.Product, 0))
                    {
                        err += "No product exist in the system with specific name (" + item.Product + ").,";
                    }
                    else if (!productRepo.IsOwnProduct(item.Product))
                    {
                        err += "Only own product can be imported.(" + item.Product + ").,";
                    }
                    if (!currencyRepo.IsExist(item.Currency, 0, "ShortName"))
                    {
                        err += "No currency exist in the system with specific name (" + item.Currency + ").,";
                    }
                    if (item.Quantity <= 0)
                    {
                        err += "" + item.Product + " must have quantity greater than zero.,";
                    }
                    if (item.LCRate <= 0)
                    {
                        err += "" + item.Product + " must have LC Rate greater than zero.,";
                    }

                    var product = productRepo.GetByName(item.Product);
                    var currencyId = currencyRepo.GetIdByShortName(item.Currency);
                    var productId = product != null ? product.Id : 0;
                    var customerId = customerRepo.GetIdByName(item.Customer);
                    var project = projectRepo.GetProject(customerId, productId);
                    var salePeronId = customerRepo.GetSalesPersonId(customerId, productId);
                    if (customerId > 0 && productId > 0)
                    {
                        if (project == null && salePeronId == 0)
                        {
                            err += "" + item.Product + " must have valid sale person attached.,";
                        }
                    }

                }
                err = err.Trim(',');
                if (isValidate)
                {
                    if (err != "")
                    {
                        throw new OwnException(err);
                    }
                }
                else
                {
                    if (err == "")
                    {

                        foreach (var item in records)
                        {
                            igrn = new CRMImportGRN();
                            var invDate = DateTime.MinValue;
                            var blDate = DateTime.MinValue;
                            var piDate = DateTime.MinValue;
                            DateTime.TryParseExact(item.InvoiceDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out invDate);
                            DateTime.TryParseExact(item.BLRRDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out blDate);
                            DateTime.TryParseExact(item.PIDate, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out piDate);
                            var customerId = customerRepo.GetIdByName(item.Customer);
                            var product = productRepo.GetByName(item.Product);
                            var currencyId = currencyRepo.GetIdByShortName(item.Currency);
                            var project = projectRepo.GetProject(customerId, product.Id);
                            var salePeronId = customerRepo.GetSalesPersonId(customerId, product.Id);
                            var productId = product != null ? product.Id : 0;
                            igrn = new CRMImportGRN();
                            igrn.PINumber = item.PINo;
                            if (piDate != DateTime.MinValue)
                                igrn.PIDate = piDate;
                            if (invDate != DateTime.MinValue)
                                igrn.InvoiceDate = invDate;
                            if (blDate != DateTime.MinValue)
                                igrn.BlDate = blDate;
                            igrn.Date = DateTime.Now;
                            igrn.CustomerId = customerId;
                            igrn.InvoiceNo = item.InvoiceNumber;
                            igrn.ProductId = productId;
                            igrn.BLNo = item.BLBRNo;
                            igrn.DeliveryType = item.Currency.ToLower() == "pkr" ? CRMSaleDeliveryType.ExStock : CRMSaleDeliveryType.Import;
                            igrn.SalePersonId = (project != null ? project.SalePersonId : salePeronId);
                            if (project != null)
                                igrn.ProjectId = project.Id;
                            igrn.CurrencyId = currencyId;
                            igrn.SaleType = project != null ? CRMSaleType.Project : CRMSaleType.Reguler;
                            igrn.Quantity = item.Quantity;
                            igrn.ExcRate = item.ExRate;
                            igrn.Price = item.LCRate;
                            igrn.NetTotal = item.LCValue;
                        }
                        igrn.FiscalId = SiteContext.Current.Fiscal.Id;
                        if (igrn.Id == 0)
                        {
                            repo.Add(igrn);
                        }
                        repo.SaveChanges();
                        scope.Complete();
                    }
                    else
                    {
                        throw new OwnException(err);
                    }
                }
            }


        }




        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMImportIGRNRepository();
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
        public static string ValidateSave(List<CRMImportGRN> records)
        {
            var err = ",";
            try
            {
                var CRMSaleInvoiceRepo = new CRMSaleInvoiceRepository();
                if (!SiteContext.Current.User.IsAdmin)
                {
                    foreach (var item in records)
                    {
                        if (item.Id == 0)
                        {
                            if (!SiteContext.Current.RoleAccess.CanCreate)
                            {
                                err += "you did not have sufficent right to add new record.,";
                            }
                        }
                        else
                        {
                            if (!SiteContext.Current.RoleAccess.CanUpdate)
                            {
                                err += "you did not have sufficent right to update record.,";
                            }
                        }
                    }
                }
                foreach (var item in records)
                {

                    if (item.CustomerId == 0)
                    {
                        err += "Please select customer for each record.,";
                    }
                    if (item.SalePersonId == 0)
                    {
                        err += "Please select sale person for each record.,";
                    }
                }



                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                //if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                //{
                //    err += "Voucher date should be within current fiscal year.,";
                //}
                var Itemcountlist = records.GroupBy(p => p.ProductId).Select(p => new
                {
                    ProductId = p.Key,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += "Product must be added once in list.(Current Count:" + item.Count + "),";
                }
            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }


    }

}
