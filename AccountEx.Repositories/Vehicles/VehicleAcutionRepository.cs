using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.Common.RentalAgreement;
using AccountEx.Common.VehicleSystem;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Repositories
{
    public class VehicleAcutionRepository : GenericRepository<VehicleAcution>
    {
         public VehicleAcutionRepository() : base() { }
         public VehicleAcutionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public VehicleAcution GetBySaleId(int saleId)
        {
            return Collection.FirstOrDefault(p => p.SaleId == saleId);
        }
        public bool IsExistBySaleId(int vehicleSaleId)
        {
            return Collection.Any(p => p.SaleId == vehicleSaleId);
             
        }
        public PrintRepossessionLetter PrintPossessionLetter(int saleId)
        {
            var query = string.Format("EXEC [DBO].[PrintRepossessionLetter] @COMPANYID = {0},@SaleId ={1}", SiteContext.Current.User.CompanyId, saleId);
            return Db.Database.SqlQuery<PrintRepossessionLetter>(query).FirstOrDefault();
        }
        public PrintFurtherAgreement PrintFurtherAgreement(int saleId)
        {
            var query = string.Format("EXEC [DBO].[PrintFurtherAgreement] @COMPANYID = {0},@SaleId ={1}", SiteContext.Current.User.CompanyId, saleId);
            return Db.Database.SqlQuery<PrintFurtherAgreement>(query).FirstOrDefault();
        }
    }
}
