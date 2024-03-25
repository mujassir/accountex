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

    public static class CRMComplaintManager
    {



        public static void Save(List<CRMComplaint> compliants)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMCompliantRepository();
                var userRepo = new UserRepository(repo);
                var labUserId = userRepo.GetDefaultUserByTypeId(CRMUserType.LabUser);
                var tranrRepo = new TransactionRepository(repo);
                var newComplaints = compliants.Where(p => p.Id == 0).ToList();
                var lastVoucherNo = repo.GetNextVoucherNumber();
                foreach (var c in newComplaints)
                {
                    c.AssignedToId = labUserId;
                    c.VoucherNo = lastVoucherNo;
                    lastVoucherNo++;
                }
                repo.Add(newComplaints);
                repo.Update(compliants.Where(p => p.Id > 0).ToList());
                repo.SaveChanges();
                scope.Complete();

            }

        }
        public static void Save(CRMComplaint c)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMCompliantRepository();
                var userRepo = new UserRepository(repo);
                var labUserId = userRepo.GetDefaultUserByTypeId(CRMUserType.LabUser);
                var tranrRepo = new TransactionRepository(repo);
                if (c.Id == 0 || c.VoucherNo == 0)
                    c.VoucherNo = repo.GetNextVoucherNumber();
                c.CRMComplaintFiles = null;
                c.AssignedToId = labUserId;

                repo.Save(c);
                repo.SaveChanges();
                scope.Complete();

            }

        }


        public static void ChangeStatus(int id, CRMComplaintStatus statusId)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMCompliantRepository();
                var tranrRepo = new TransactionRepository(repo);
                repo.ChangeStatus(id, statusId);
                repo.SaveChanges();
                scope.Complete();

            }

        }
        public static void SaveDocuments(List<CRMComplaintFile> files, int complaintId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new CRMCompliantFileRepository();
                var query = "DELETE FROM dbo.CRMComplaintFiles WHERE CompanyId=" + SiteContext.Current.User.CompanyId + " AND CRMComplaintId=" + complaintId;
                repo.ExecuteQuery(query);
                if (files != null && files.Count > 0)
                {
                    repo.Add(files, false, false);

                }

                repo.SaveChanges();
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMCompliantRepository();
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



        public static string ValidateSave(CRMComplaint events)
        {
            var err = ",";
            try
            {
                var userRepo = new UserRepository();
                var labUserId = userRepo.GetDefaultUserByTypeId(CRMUserType.LabUser);
                if (labUserId == 0)
                {
                    err += "No lab user found in the system.,";
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
    }
}
