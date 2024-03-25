using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class VehicleInstallmentPaymentRepository : GenericRepository<VehicleInstallmentPayment>
    {
        public VehicleInstallmentPaymentRepository() : base() { }
        public VehicleInstallmentPaymentRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public decimal GetTotalPaid(int vehicleSaleDetailId)
        {
            if (Collection.Any(p => p.VehicleSaleDetailId == vehicleSaleDetailId))
                return Collection.Where(p => p.VehicleSaleDetailId == vehicleSaleDetailId).Sum(p => p.Amount);

            else return 0;
        }
        public List<VehicleInstallmentPayment> GetBySaleId(int saleId)
        {

            return Collection.Where(p => p.VehicleSaleId == saleId).ToList();

        }
        public decimal GetSumBySaleId(int saleId)
        {
            var query = Collection.Where(p => p.VehicleSaleId == saleId);
            if (query.Any())
                return query.Sum(p => p.Amount);
            return 0;

        }
        public List<VehicleInstallmentPayment> GetByPaymentId(int paymnetId)
        {

            return Collection.Where(p => p.VehiclePaymentId == paymnetId).ToList();

        }
        public List<int> GetSaleDetailIds(int paymnetId)
        {

            return Collection.Where(p => p.VehiclePaymentId == paymnetId).Select(p => p.VehicleSaleDetailId).ToList();

        }


    }
}
