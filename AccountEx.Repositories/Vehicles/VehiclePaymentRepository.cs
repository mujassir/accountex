using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Repositories
{
    public class VehiclePaymentRepository : GenericRepository<VehiclePayment>
    {

        public VehiclePaymentRepository() : base() { }
        public VehiclePaymentRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public dynamic GetPaidInstalments(int vehicleSaleId)
        {
            return Collection.Where(p => p.VehicleSaleId == vehicleSaleId).Select(p => new
            {
                p.Id,
                p.Amount,
                p.Discount,
                p.RecievedDate,
                p.RcvAccountId,
                p.PaymentMode,

            }).ToList();
        }
        public decimal GetPaidAmount(int vehicleSaleId)
        {
            if (Collection.Any(p => p.VehicleSaleId == vehicleSaleId))
                return Collection.Where(p => p.VehicleSaleId == vehicleSaleId).Sum(p => p.Amount);
            else return 0M;
        }
        public bool IsExistBySaleId(int vehicleSaleId)
        {
            return Collection.Any(p => p.VehicleSaleId == vehicleSaleId);
             
        }

    }
}
