using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories.Vehicles
{
    public class VehicleFollowUpRepository : GenericRepository<VehicleFollowUp>
    {
        public List<VehicleFollowUp> GetFollowUps(int vehicleId, int customerId)
        {
           return Collection.AsQueryable().Where(p => p.VehicleId == vehicleId && p.CustomerId == customerId).ToList();
        }


    }
}
