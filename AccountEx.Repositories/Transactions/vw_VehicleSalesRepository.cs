using AccountEx.CodeFirst.Models.Views;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Transactions
{
    public class vw_VehicleSalesRepository : GenericRepository<vw_VehicleSales>
    {
        public vw_VehicleSalesRepository() : base() { }
        public vw_VehicleSalesRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public List<IdName> GetVehiclesWithCustomer()
        {


            return AsQueryable<vw_VehicleSales>().Select(p => new IdName()
            {
                Id = p.Id,
                Name = "Customer:" + p.Customer + " Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer
            }).ToList();
        }
    }
}
