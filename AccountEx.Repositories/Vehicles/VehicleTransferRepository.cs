using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using Entities.CodeFirst;

namespace AccountEx.Repositories
{
    public class VehicleTransferRepository : GenericRepository<VehicleRequest>
    {

        public VehicleTransferRepository() : base() { }
        public VehicleTransferRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public bool IsPendingRequestExist(int vehicleId)
        {
            return Collection.Any(p => p.VehicleId == vehicleId && p.Status == (byte)VehicleRequestStatus.Pending);
        }
        public IdName GetToBranchTransferDetail(int vehicleId, int branchId)
        {
            return (from vr in Collection
                    join vb in AsQueryable<VehicleBranch>() on vr.ToBranchId equals vb.Id

                    where vr.VehicleId == vehicleId && vr.FromBranchId == branchId
                    select new IdName()
                    {
                        Id = vb.Id,
                        Name = vb.Name,

                    }).FirstOrDefault();
        }

        public void UpdateStatus(VehicleRequest input)
        {
            var repo = new VehicleRepository();
            using (var scope = TransactionScopeBuilder.Create())
            {
                var record = Collection.FirstOrDefault(p => p.Id == input.Id);
                if (record != null)
                {
                    record.Reason = input.Reason;
                    record.Status = input.Status;
                    var vehicle = repo.GetById(record.VehicleId);
                    vehicle.BranchId = record.ToBranchId;
                    SaveChanges();
                    repo.SaveChanges();
                    scope.Complete();
                }

            }
        }
    }
}
