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
    public class VehicleSaleDetailRepository : GenericRepository<VehicleSaleDetail>
    {

        public VehicleSaleDetailRepository() : base() { }
        public VehicleSaleDetailRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public VehicleSaleDetail GetByInstallmentNo(int installmentNo, int vehicleSaleId)
        {
            return Collection.AsQueryable().Where(p => p.InstalmentNo == installmentNo && p.VehicleSaleId == vehicleSaleId).FirstOrDefault();
        }
        public dynamic GetPendingInstalment(int vehicleSaleId)
        {
            return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && !p.IsRecieved).Select(p => new
                {
                    p.Id,
                    p.InstalmentNo,
                    p.Amount,
                    p.RecievedAmount,
                    Pending = p.Amount - p.RecievedAmount,
                }).ToList();
        }

        public List<VehicleSaleDetail> GetPendingInstalmentByVehicleSaleId(int vehicleSaleId)
        {
            return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && !p.IsRecieved).ToList();
        }
        public List<VehicleSaleDetail> GetPaidInstalmentByVehicleSaleId(int vehicleSaleId)
        {
            return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && p.RecievedAmount > 0).ToList();
        }
        public List<VehicleSaleDetail> GetUnPaidByIds(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id) && p.RecievedAmount > 0).ToList();
        }
        public List<VehicleSaleDetail> GetPaidInstalmentByVehicleSId(int vehicleSaleId)
        {
            return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && p.RecievedAmount > 0).ToList();
        }
        public decimal GetTotalOutStanding(int vehicleSaleId)
        {
            if (Collection.Any(p => p.VehicleSaleId == vehicleSaleId && !p.IsRecieved))
                return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && !p.IsRecieved).Sum(p => p.Amount - p.RecievedAmount);
            else return 0;
        }
        public decimal GetCurrentMonthOutStanding(int vehicleSaleId)
        {

            DateTime date = DateTime.Now.Date;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            if (Collection.Any(p => p.VehicleSaleId == vehicleSaleId && EntityFunctions.TruncateTime(p.InstallmentDate) >= EntityFunctions.TruncateTime(startDate) && EntityFunctions.TruncateTime(p.InstallmentDate) <= EntityFunctions.TruncateTime(endDate)))
                return Collection.Where(p => p.VehicleSaleId == vehicleSaleId && EntityFunctions.TruncateTime(p.InstallmentDate) >= EntityFunctions.TruncateTime(startDate) && EntityFunctions.TruncateTime(p.InstallmentDate) <= EntityFunctions.TruncateTime(endDate)).Sum(p => p.Amount - p.RecievedAmount);
            else
                return 0;
        }

    }
}
