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

    public static class CRMSaleInvoiceManager
    {



        public static void Save(CRMSaleInvoice sale)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMSaleInvoiceRepository();
                var tranrRepo = new TransactionRepository(repo);
                sale.FiscalId = SiteContext.Current.Fiscal.Id;
                foreach (var item in sale.CRMSaleInvoiceItems)
                {
                    if (item.ProjectId == 0)
                        item.ProjectId = null;
                }
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
        public static void Import(CRMImportExtra data, bool isValidate)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMSaleInvoiceRepository();
                var customerRepo = new CRMCustomerRepository(repo);
                var productRepo = new CRMProductRepository(repo);
                var currencyRepo = new CurrencyRepository(repo);
                var projectRepo = new CRMProjectRepository(repo);
                var tranrRepo = new TransactionRepository(repo);
                var groupInvoices = data.Invoices.GroupBy(p => new { p.InvoiceNumber, p.Customer }).Select(p => new
                {
                    p.Key.InvoiceNumber,
                    p.Key.Customer,
                    Invoice = p.FirstOrDefault(),
                    TotalAmount = p.Sum(q => q.Total),
                    NetAmount = p.Sum(q => q.Total) + p.FirstOrDefault().Tax,
                    Records = p.ToList()
                }).ToList();
                var err = "";
                var sale = new CRMSaleInvoice();
                foreach (var item in groupInvoices)
                {
                    var invoice = item.Invoice;
                    var date = DateTime.MinValue;
                    DateTime.TryParseExact(invoice.Date, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
                    if (item.InvoiceNumber == 0)
                    {
                        err += "Invoice no is required.,";
                    }
                    if (string.IsNullOrWhiteSpace(item.Customer))
                    {
                        err += "Customer is required.,";
                    }
                    if (string.IsNullOrWhiteSpace(invoice.InvoiceType))
                    {
                        err += "Invoice type is required.,";
                    }
                    if (date == DateTime.MinValue)
                    {
                        err += "invoice date is missing or invalid.,";
                    }
                    if (item.TotalAmount <= 0)
                    {
                        err += "Invoice total amount must be greater than zero.,";
                    }
                    //if (repo.IsBookNoExits(item.InvoiceNumber, 0))
                    //{
                    //    err += "Invoice number already exist.,";
                    //}
                    if (!customerRepo.IsExist(item.Customer, 0))
                    {
                        err += "No customer exist in the system with specific name (" + item.Customer + ").,";
                    }
                    foreach (var invItem in item.Records)
                    {
                        if (!productRepo.IsExist(invItem.Product, 0))
                        {
                            err += "No product exist in the system with specific name (" + invItem.Product + ").,";
                        }
                        else if (!productRepo.IsOwnProduct(invItem.Product))
                        {
                            err += "Only own product can be imported.(" + invItem.Product + ").,";
                        }
                        if (!currencyRepo.IsExist(invItem.Currency, 0, "ShortName"))
                        {
                            err += "No currency exist in the system with specific name (" + invItem.Currency + ").,";
                        }
                        if (invItem.Quantity <= 0)
                        {
                            err += "" + invItem.Product + " must have quantity greater than zero.,";
                        }
                        if (invItem.Price <= 0)
                        {
                            err += "" + invItem.Product + " must have price greater than zero.,";
                        }
                        if (invItem.Total <= 0)
                        {
                            err += "" + invItem.Product + " must have total greater than zero.,";
                        }
                        var product = productRepo.GetOwnProductByName(invItem.Product);
                        var currencyId = currencyRepo.GetIdByShortName(invItem.Currency);
                        var productId = product != null ? product.Id : 0;
                        var customerId = customerRepo.GetIdByName(item.Customer);
                        var project = projectRepo.GetProject(customerId, productId, currencyId);
                        var salePeronId = customerRepo.GetSalesPersonId(customerId, productId);
                        if (customerId > 0 && productId > 0)
                        {
                            if (project == null && salePeronId == 0)
                            {
                                err += "" + invItem.Product + " must have valid sale person attached.,";
                            }
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

                        foreach (var item in groupInvoices)
                        {
                            var invoice = item.Invoice;
                            var date = DateTime.ParseExact(invoice.Date, new string[] { "dd/MM/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                            sale = new CRMSaleInvoice();
                            sale.InvoiceNumber = item.InvoiceNumber;
                            sale.OGPNo = invoice.OGP;
                            sale.CustomerId = customerRepo.GetIdByName(item.Customer);
                            sale.DeliveryType = CRMSaleDeliveryType.ExStock;
                            sale.SaleType = invoice.InvoiceType.ToString().ToLower().ToLower() == "gst" ? CRMInvoiceType.GST : CRMInvoiceType.NonGST;
                            sale.Tax = invoice.Tax;
                            sale.NetTotal = item.NetAmount;
                            sale.Date = date;
                            sale.CreatedAt = DateTime.Now;
                            var counter = 1;
                            foreach (var invItem in item.Records)
                            {
                                var product = productRepo.GetByName(invItem.Product);
                                var currencyId = currencyRepo.GetIdByShortName(invItem.Currency);
                                var project = projectRepo.GetProject(sale.CustomerId, product.Id, currencyId);
                                var salePeronId = customerRepo.GetSalesPersonId(sale.CustomerId, product.Id);
                                var saleItem = new CRMSaleInvoiceItem();
                                saleItem.SRNo = counter;
                                saleItem.Rate = invItem.Price;
                                saleItem.Quantity = invItem.Quantity;
                                saleItem.Amount = invItem.Total;
                                saleItem.ItemId = product.Id;
                                saleItem.DivisionId = (product.DivisionId.HasValue ? product.DivisionId.Value : 0);
                                saleItem.ItemName = product.Name;
                                saleItem.SalePersonId = (project != null ? project.SalePersonId : salePeronId);
                                if (project != null)
                                    saleItem.ProjectId = project.Id;
                                saleItem.CurrencyId = currencyId;
                                saleItem.SaleType = project != null ? CRMSaleType.Project : CRMSaleType.Reguler;
                                saleItem.CreatedAt = DateTime.Now;
                                counter++;
                                sale.CRMSaleInvoiceItems.Add(saleItem);

                            }
                        }

                        sale.FiscalId = SiteContext.Current.Fiscal.Id;
                        if (sale.Id == 0)
                        {

                            sale.VoucherNumber = repo.GetNextVoucherNumber();
                            repo.Add(sale);

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
                var repo = new CRMSaleInvoiceItemRepository();
                repo.Delete(id);
                //var itemRepo = new CRMSaleInvoiceItemRepository(repo);
                //var dbPMC = repo.GetById(id, true);
                ////add,update & remove services items
                //var Ids = new List<int>();

                //if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                //{
                //    Ids.AddRange(dbPMC.CRMSaleInvoiceItems.Where(p => p.SalePersonId == SiteContext.Current.User.Id).Select(p => p.Id).ToList());
                //    itemRepo.Delete(Ids);
                //}
                //else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                //{
                //    var dbProductIds = dbPMC.CRMSaleInvoiceItems.Select(p => p.ItemId).ToList();
                //    var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                //    Ids.AddRange(dbPMC.CRMSaleInvoiceItems.Where(p => DHproductIds.Contains(p.ItemId)).Select(p => p.Id).ToList());
                //    itemRepo.Delete(Ids);
                //}
                //else
                //{

                //}

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



        public static string ValidateSave(CRMSaleInvoice input)
        {
            var err = ",";
            try
            {
                var CRMSaleInvoiceRepo = new CRMSaleInvoiceRepository();
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
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                //if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                //{
                //    err += "Voucher date should be within current fiscal year.,";
                //}

                //var isExist = CRMSaleInvoiceRepo.IsVoucherExits(input.VoucherNumber, input.Id);
                //if (isExist)
                //{
                //    err += "Voucher no already exist.,";
                //}


                foreach (var item in input.CRMSaleInvoiceItems.Where(p => p.SalePersonId == 0))
                {
                    err += item.ItemName + "must have a sale person attached.,";
                }
                //var Itemcountlist = input.CRMSaleInvoiceItems.GroupBy(p => p.ItemId).Select(p => new
                //{
                //    ItemId = p.Key,
                //    ItemName = p.FirstOrDefault().ItemName,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();

                //foreach (var item in Itemcountlist)
                //{
                //    err += item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                //}

                if (input.Id > 0)
                {
                    var dbSale = CRMSaleInvoiceRepo.GetById(input.Id, true);
                    if (dbSale.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no.please use previous voucher no.(" + dbSale.VoucherNumber + "),";
                    }

                    //if (dbSale.DCNo != input.DCNo)
                    //{
                    //    err += "can't change dc no.please use previous dc no.(" + dbSale.DCNo + "),";
                    //}

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
