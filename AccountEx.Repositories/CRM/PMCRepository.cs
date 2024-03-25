using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class PMCRepository : GenericRepository<PMC>
    {
        public PMCRepository() : base() { }
        public PMCRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public int GetNextBookNumber(VoucherType vouchertype)
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }


        public PMC GetByVoucherNumber(int voucherno)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public List<PMC> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            return FiscalCollection.Where(p => p.VoucherNumber == voucherno).ToList();
        }
        public PMC GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public PMC GetByCustomerId(int customerId)
        {
            return FiscalCollection.FirstOrDefault(p => p.CustomerId == customerId );
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsPMCExits(int customerId, int id)
        {
            return FiscalCollection.Any(p => p.CustomerId == customerId && p.Id != id);
        }
        public PMC GetByBookNumber(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.Id != id);
        }



        public PMC GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            PMC v = null;
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
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any())
            {
                v = new PMC();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order

        public void DeleteByVoucherNumber(int voucherno)
        {
            var PMC = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Delete(PMC);
        }


        public override void Delete(int id)
        {
            var PMC = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(PMC);
        }
        public void Delete(PMC PMC)
        {

            base.Delete(PMC.Id);
        }

        public override void Save(PMC PMC)
        {
            var repo = new PMCRepository();
            if (PMC.Id == 0)
            {
                repo.Add(PMC);
            }
            else
            {
                repo.Update(PMC);
            }
        }

        public override void Add(PMC PMC)
        {
            base.Add(PMC, true, false);
        }
        public override void Update(PMC PMC)
        {
            var PMCItemsRepo = new PMCItemRepository(this);
            var dbPMC = GetById(PMC.Id, true);

            //add,update & remove services items
            var Ids = PMC.PMCItems.Select(p => p.Id).ToList();

            if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
            {
                Ids.AddRange(dbPMC.PMCItems.Where(p => p.SalePersonId != SiteContext.Current.User.Id).Select(p => p.Id).ToList());
            }
            else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
            {
                var dbProductIds = dbPMC.PMCItems.Select(p => p.ProductId).ToList();
                var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                Ids.AddRange(dbPMC.PMCItems.Where(p => !DHproductIds.Contains(p.ProductId)).Select(p => p.Id).ToList());
            }





            var deletedIds = dbPMC.PMCItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            PMCItemsRepo.Delete(deletedIds);
            PMCItemsRepo.Save(PMC.PMCItems.ToList());


            //SaveChanges();
            PMC.PMCItems = null;
            base.Update(PMC, true, false);
        }




    }
}