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
    public class VehicleSaleDocumentRepository : GenericRepository<SaleDocument>
    {
          public VehicleSaleDocumentRepository() : base() { }
          public VehicleSaleDocumentRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public List<SaleDocument> GetBySaleId(int saleId)
        {
            return Collection.Where(p => p.SaleId == saleId).ToList();
        }
       
      
       
    }
}
