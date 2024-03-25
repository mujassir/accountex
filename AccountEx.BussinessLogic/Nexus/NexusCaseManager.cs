using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using AccountEx.Common.Nexus;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{

    public static class NexusCaseManager
    {



        public static void Save(NexusPostedCases postedCase)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new NexusCaseRepository();
                var tranrRepo = new TransactionRepository(repo);
                if (postedCase.Id == 0)
                {

                    postedCase.VoucherNumber = repo.GetNextVoucherNumber();
                    repo.Add(postedCase);

                }
                else
                {
                    repo.Update(postedCase);

                }
                repo.SaveChanges();
                scope.Complete();

            }

        }
        public static void Save(List<NexusUnpostedCaseSave> unPostedCases, NexusCaseType type = NexusCaseType.Departmental)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new NexusCaseRepository();
                var voucherNo = repo.GetNextVoucherNumber();
                var tranrRepo = new TransactionRepository(repo);
                var groupCases = unPostedCases.GroupBy(p => p.CaseId).Select(p => new
                {
                    CaseId = p.Key,
                    Cases = p.ToList()
                }).ToList();
                var postedCases = new List<NexusPostedCases>();
                foreach (var gc in groupCases)
                {
                    var postedCase = new NexusPostedCases();
                    postedCase.CaseId = gc.CaseId;
                    postedCase.FiscalId = SiteContext.Current.Fiscal.Id;
                    postedCase.VoucherNumber = voucherNo;
                    postedCase.NetAmount = gc.Cases.Sum(p => p.Price);
                    var r = gc.Cases.FirstOrDefault();
                    if (r != null)
                    {
                        postedCase.EmployeeId = r.EmployeeId;
                        postedCase.EmployeeName = r.EmployeeName;
                        postedCase.Relationship = r.Relationship;
                        postedCase.DepartmentId = r.DepartmentId;
                        postedCase.Date = r.RegistrationDate.HasValue ? r.RegistrationDate.Value : DateTime.Now;

                    }
                    foreach (var item in gc.Cases)
                    {
                        postedCase.NexusPostedCasesItems.Add(new NexusPostedCasesItems()
                        {
                            TestId = item.TestId,
                            TestName = item.TestName,
                            Price = item.Price,
                            CaseDetailId = item.CaseDetailId
                        });
                    }
                    postedCases.Add(postedCase);

                }
                repo.UpdateIsPosted(unPostedCases.Select(p => p.CaseDetailId).Distinct().ToList());
                AddTransaction(unPostedCases, voucherNo, repo, type);
                repo.Save(postedCases);
                repo.SaveChanges();
                scope.Complete();

            }

        }
        public static void Update(List<NexusUnpostedCaseSave> unPostedCases, NexusCaseType type = NexusCaseType.Departmental)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new NexusCaseRepository();
                var Itemrepo = new PostedCaseItemRepository(repo);

                var voucherNo = repo.GetNextVoucherNumber();
                var tranrRepo = new TransactionRepository(repo);
                var groupCases = unPostedCases.GroupBy(p => p.CaseId).Select(p => new
                {
                    CaseId = p.Key,
                    Cases = p.ToList()
                }).ToList();
                var caseIds = groupCases.Select(p => p.CaseId).Distinct().ToList();
                var dbPostedCases = repo.GetByCaseIds(caseIds);
                var postedCases = new List<NexusPostedCases>();
                var postedCaseItems = new List<NexusPostedCasesItems>();
                var caseDetaildIs = new List<int>();
                foreach (var gc in groupCases)
                {
                    var postedCase = dbPostedCases.FirstOrDefault(p => p.CaseId == gc.CaseId);
                    if (postedCase != null)
                    {
                        postedCase.NetAmount = gc.Cases.Sum(p => p.Price);
                        var r = gc.Cases.FirstOrDefault();
                        if (r != null)
                        {
                            postedCase.EmployeeId = r.EmployeeId;
                            postedCase.EmployeeName = r.EmployeeName;
                            postedCase.Relationship = r.Relationship;
                            postedCase.DepartmentId = r.DepartmentId;
                            postedCase.Date = r.RegistrationDate.HasValue ? r.RegistrationDate.Value : DateTime.Now;

                        }
                        caseDetaildIs.AddRange(postedCase.NexusPostedCasesItems.Select(p => p.Id).ToList());
                        postedCase.NexusPostedCasesItems = null;
                        postedCase.NexusPostedCasesItems = new List<NexusPostedCasesItems>();
                        foreach (var item in gc.Cases)
                        {
                            postedCaseItems.Add(new NexusPostedCasesItems()
                            {
                                PostedCaseId = postedCase.Id,
                                TestId = item.TestId,
                                TestName = item.TestName,
                                Price = item.Price,
                                CaseDetailId = item.CaseDetailId
                            });
                        }
                        postedCases.Add(postedCase);
                    }
                    

                }
                Itemrepo.Delete(caseDetaildIs);
                Itemrepo.Add(postedCaseItems);
                repo.SaveChanges();
                AddTransaction(unPostedCases, voucherNo, repo, type);
                repo.Save(postedCases);
                repo.SaveChanges();
                scope.Complete();

            }

        }


        public static void AddTransaction(List<NexusUnpostedCaseSave> unPostedCases, int voucherNo, BaseRepository baseRepo, NexusCaseType type = NexusCaseType.Departmental)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            var voucherRepo = new VoucherTransRepository(baseRepo);

            var trans = new List<Transaction>();
            foreach (var v in unPostedCases.GroupBy(p => p.CaseDetailId))
            {
                transRepo.HardDeleteByReferenceIdTransactionType(Numerics.GetInt(v.Key), VoucherType.PostedCases);
            }
            foreach (var v in unPostedCases)
            {
              
                var comments = "Test price for Patient:" + v.PatientName + " against case no.:" + v.CaseNumber;
                if (type == NexusCaseType.Departmental)
                    comments = "Test price for Patient:" + v.PatientName + " Employee:" + v.EmployeeId + "-" + v.EmployeeName + " against case no.:" + v.CaseNumber;
                trans.AddRange(new List<Transaction>()
                {
                new Transaction
                {
                    ReferenceId = Numerics.GetInt(v.CaseDetailId),
                    AccountId = Numerics.GetInt(v.DepartmentId),
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    TransactionType = VoucherType.PostedCases,
                    EntryType = (byte)EntryType.Item,
                    Comments = comments,
                    Debit = v.Price,
                    Date=v.RegistrationDate.Value

                },
                 new Transaction
                 {
                     ReferenceId = Numerics.GetInt(v.CaseDetailId),
                     AccountId = SettingManager.SaleAccountHeadId,
                     InvoiceNumber = voucherNo,
                     VoucherNumber = voucherNo,
                     TransactionType = VoucherType.PostedCases,
                     EntryType = (byte)EntryType.MasterDetail,
                     Comments = comments,
                     Credit = v.Price,
                     Date= v.RegistrationDate.Value
                 }
                });

            }
            foreach (var item in trans)
            {
                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }


        public static void Delete(string caseNo)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new NexusCaseRepository();
                var tranrRepo = new TransactionRepository(repo);
                var nexusCaseId = repo.GetCaseIdFromNexusCaseViewByCaseNo(caseNo);
                if (nexusCaseId > 0)
                {
                    var voucherNo = repo.GetVoucherNoByCaseId(nexusCaseId);
                    if (voucherNo > 0)
                    {
                        repo.DeleteByVoucherNumber(voucherNo);
                        tranrRepo.HardDelete(voucherNo, VoucherType.PostedCases);
                        repo.UpdateIsPostedByNexusCaseId(nexusCaseId, false);
                    }
                    else
                    {
                        throw new OwnException("No case detail found for deletion.");
                    }
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



        public static string ValidateSave(List<NexusUnpostedCaseSave> unPostedCases, NexusCaseType type = NexusCaseType.Departmental)
        {
            var err = ",";
            try
            {
                var pmcRepo = new PMCRepository();
                if (type == NexusCaseType.Departmental)
                {
                    foreach (var item in unPostedCases)
                    {
                        if (item.Price > 0 && item.TestId == 0)
                        {
                            err += "Test must be selected for case no:" + item.CaseNumber + " where price is greater than zero(" + item.Price + "),";
                        }
                        if (item.TestId > 0 && item.DepartmentId == 0)
                        {
                            err += "department must be selected for case no:" + item.CaseNumber + " where Test is selected(" + item.TestName + "),";
                        }
                        if (!FiscalYearManager.IsValidFiscalDate(item.RegistrationDate.Value))
                        {
                            //  err += "case date should be within current fiscal year for case no:" + item.CaseNumber + ".,";
                        }


                    }
                    var groupCases = unPostedCases.GroupBy(p => p.CaseId).Select(p => new
                    {
                        CaseId = p.Key,
                        Cases = p.ToList()
                    }).ToList();
                    var postedCases = new List<NexusPostedCases>();

                    foreach (var item in groupCases)
                    {

                        if (!item.Cases.Any(p => p.TestId > 0))
                        {
                            err += "Atleast one test must be selected for case no:" + item.Cases.FirstOrDefault().CaseNumber + ".,";
                        }


                    }
                }
                if (type == NexusCaseType.Cash && SettingManager.CashAccountId == 0)
                {
                    err += "Cash account is missing.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }



            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }

        public static string ValidateSave(NexusPostedCases input)
        {
            var err = ",";
            try
            {
                var pmcRepo = new PMCRepository();
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

                if (input.CaseId == 0)
                {
                    err += "Please select the case to process.,";
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
                    err += "case date should be within current fiscal year.,";
                }

                var isExist = pmcRepo.IsVoucherExits(input.VoucherNumber, input.Id);
                if (isExist)
                {
                    err += "Voucher no already exist.,";
                }


                //foreach (var item in input.PMCItems.Where(p => p.ProductId == 0))
                //{
                //    err +=  item.ItemName + " is not valid.,";
                //}

                //var Itemcountlist = input.PMCItems.GroupBy(p => p.ProductId).Select(p => new
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
                    //var dbSale = pmcRepo.GetById(input.Id, true);
                    //if (dbSale.VoucherNumber != input.VoucherNumber)
                    //{
                    //    err += "can't change voucher no.please use previous voucher no.(" + dbSale.VoucherNumber + "),";
                    //}

                    //if (dbSale.DCNo != input.DCNo)
                    //{
                    //    err += "can't change dc no.please use previous dc no.(" + dbSale.DCNo + "),";
                    //}

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
