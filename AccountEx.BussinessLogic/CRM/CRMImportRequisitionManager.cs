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

    public static class CRMImportRequisitionManager
    {



        public static void Save(CRMImportRequisition requisition, CRMImportRequisitionType type)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMImportRequisitionRepository();
                var tranrRepo = new TransactionRepository(repo);
                requisition.FiscalId = SiteContext.Current.Fiscal.Id;

                if (type == CRMImportRequisitionType.Default)
                {
                    if (requisition.Id == 0)
                    {
                        requisition.VoucherNumber = repo.GetNextVoucherNumber();
                        repo.Add(requisition);

                    }
                    else
                    {
                        repo.Update(requisition);
                    }
                }
                else if (type == CRMImportRequisitionType.DH)
                {
                    if (requisition.Status == CRMImportRequisitionStatus.Approved)
                    {
                        requisition.ApprovedBy = SiteContext.Current.User.Id;
                        requisition.ApproveDate = DateTime.Now;
                    }
                    repo.Update(requisition);
                }
                else if (type == CRMImportRequisitionType.RSM)
                {
                    requisition.Status = CRMImportRequisitionStatus.Archive;
                    requisition.RevisedBy = SiteContext.Current.User.Id;
                    requisition.ReviseDate = DateTime.Now;
                    repo.Update(requisition);
                    var newRequisition = requisition.CloneWithJson();
                    newRequisition.Id = 0;
                    newRequisition.ModifiedAt = null;
                    newRequisition.ModifiedBy = null;
                    newRequisition.Status = CRMImportRequisitionStatus.Revised;
                    newRequisition.RevisionNo = requisition.RevisionNo + 1;
                    repo.Add(newRequisition);
                }


                repo.SaveChanges();
                scope.Complete();

            }

        }



        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var saleRepo = new CRMImportRequisitionRepository();
                saleRepo.Delete(id);
                saleRepo.SaveChanges();
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



        public static string ValidateSave(CRMImportRequisition input, CRMImportRequisitionType type)
        {
            var err = ",";
            try
            {
                var CRMImportRequisitionRepo = new CRMImportRequisitionRepository();
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

                if ((type == CRMImportRequisitionType.DH || type == CRMImportRequisitionType.RSM) && input.Id == 0)
                {
                    err += "No Requisition found for updation.";
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
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }

                var isExist = CRMImportRequisitionRepo.IsVoucherExits(input.VoucherNumber, input.Id);
                if (isExist)
                {
                    err += "Voucher no already exist.,";
                }


                //foreach (var item in input.CRMImportRequisitionItems.Where(p => p.ProductId == 0))
                //{
                //    err +=  item.ItemName + " is not valid.,";
                //}

                //var Itemcountlist = input.CRMImportRequisitionItems.GroupBy(p => p.ProductId).Select(p => new
                //{
                //    ItemId = p.Key,
                //    ItemCode = p.FirstOrDefault().ItemCode,
                //    ItemName = p.FirstOrDefault().ItemName,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();

                //foreach (var item in Itemcountlist)
                //{
                //    err += item.ItemCode + "-" + item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                //}

                if (input.Id > 0)
                {
                    var dbSale = CRMImportRequisitionRepo.GetById(input.Id, true);
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
