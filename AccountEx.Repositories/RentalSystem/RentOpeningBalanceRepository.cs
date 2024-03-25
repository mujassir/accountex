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
    public class RentOpeningBalanceRepository : GenericRepository<RentOpeningBalance>
    {
        public RentOpeningBalanceRepository() : base() { }
        public RentOpeningBalanceRepository(BaseRepository repo)
        {
            Db = repo.GetContext();
        }

        public vw_RentOpeningBalance GetByIdFromView(int id)
        {

            return AsQueryable<vw_RentOpeningBalance>(true).FirstOrDefault(p => p.Id == id);
        }
        public override void Update(RentOpeningBalance entity)
        {

            base.Update(false, entity);
            //SaveChanges();
        }

        public bool IsExistByRentAgreementId(int agreementId, int id)
        {
            return Collection.Any(p => p.RentAgreementId == agreementId && p.Id != id);
        }
        public AgreementEditingState GetAgreementEditingState(int rentAgreementId)
        {
            var query = string.Format("EXEC dbo.GetAgreementEditingState @RentAgreementId = {0}", rentAgreementId);
            var result = Db.Database.SqlQuery<AgreementEditingState>(query);
            return result.FirstOrDefault();
        }


    }
}
