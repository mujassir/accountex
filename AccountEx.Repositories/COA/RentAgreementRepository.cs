using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class RentAgreementRepository : GenericRepository<RentAgreement>
    {
        public RentAgreementRepository() : base() { }
        public RentAgreementRepository(BaseRepository repo)
        {
            Db = repo.GetContext();
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }

        public RentAgreement GetByVoucherNumber(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public RentAgreement GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            RentAgreement v = null;
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

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            //if (v == null && !FiscalCollection.Any())
            //{
            //    v = new RentAgreement
            //    {
            //        VoucherNumber = 1001,
            //    };
            //}
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        public override void Update(RentAgreement entity)
        {
            var query = "Delete from RentAgreementSchedules where RentAgreementId=" + entity.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);

            foreach (var item in entity.RentAgreementSchedules)
            {
                item.RentAgreementId = entity.Id;
                Db.RentAgreementSchedules.Add(item);
            }
            entity.RentAgreementSchedules = null;
            base.Update(false, entity);
            //SaveChanges();
        }

        public bool CheckIfTransfeeredFromOtherAgreement(int rentAgreementId)
        {
            return FiscalCollection.Any(p => p.Id == rentAgreementId && p.TransfeerAgreementId.HasValue);

        }
        public bool CheckIfTransfeered(int rentAgreementId)
        {
            return FiscalCollection.Any(p => p.Id == rentAgreementId && p.Status==(byte)AgreementStatus.Transfeer);

        }
        public RentAgreement GetByTenantId(int tenantId)
        {
            return FiscalCollection.Where(p => p.TenantAccountId == tenantId).FirstOrDefault();
        }

        public AgreementEditingState GetAgreementEditingState(int rentAgreementId)
        {
            var query = string.Format("EXEC dbo.GetAgreementEditingState @RentAgreementId = {0}", rentAgreementId);
            var result = Db.Database.SqlQuery<AgreementEditingState>(query);
            return result.FirstOrDefault();
        }


    }
}
