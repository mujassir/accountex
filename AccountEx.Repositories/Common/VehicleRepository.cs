
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.Common;
using AccountEx.Common.VehicleSystem;
using AccountEx.DbMapping.VehicleSystem;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>
    {
        public VehicleRepository() : base() { }
        public VehicleRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public int GetNextFileNo()
        {
            var maxnumber = 1;
            if (!Collection.Any())
                return maxnumber;
            return Collection.OrderByDescending(p => p.FileNo).FirstOrDefault().FileNo + 1;
        }
        public void SaveLogBook(VehicleLogBookSave input, BaseRepository baseRepo)
        {
            var repo = new VehicleRepository(baseRepo);




        }
        public List<IdName> GetVehicles()
        {


            return AsQueryable<vw_Vehicles>().Select(p => new IdName()
                {
                    Id = p.Id,
                    Name = "Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer + " Model:" + p.Model,
                    //Name = "Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer + " Color:" + p.Color + " CC:" + p.EnginePower + " Model:" + p.Model + " Car Type:" + p.CarType
                }).ToList();
        }
        public List<VehicleTradeIn> GetTradeInVehicles(int customerId, int branchId)
        {

            return AsQueryable<vw_Vehicles>().Where(p => p.Type == (byte)VehicleType.TradIn && p.VendorId == customerId && p.BranchId == branchId && p.Status != (byte)VehicleStatus.PurchaseReturn)
                .ToList().Select(p => new VehicleTradeIn()
            {
                Id = p.Id,
                ChassisNo = p.ChassisNo,
                EngineNo = p.EngineNo,
                RegNo = p.RegNo,
                Model = p.Model,
                Manufacturer = p.Manufacturer,
                PurchasePrice = p.PurchasePrice
            }).ToList();
        }
        public List<IdName> GetVehicles(List<int> Ids)
        {
            return AsQueryable<vw_Vehicles>().Where(p => Ids.Contains(p.Id)).ToList().Select(p => new IdName()
            {
                Id = p.Id,
                Name = "Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer + " Model:" + p.Model,
                //Name = "Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer + " Color:" + p.Color + " CC:" + p.EnginePower + " Model:" + p.Model + " Car Type:" + p.CarType
            }).ToList();
        }

        public List<BLTransactionExtra> GetVehicleDetailForBLTransaction(List<int> Ids)
        {
            return AsQueryable<vw_Vehicles>().Where(p => Ids.Contains(p.Id)).ToList().Select(p => new BLTransactionExtra()
            {
                Id = p.Id,
                Name = "Chessis No:" + p.ChassisNo + " RegNo No:" + p.RegNo + " Manufacturer:" + p.Manufacturer + " Model:" + p.Model,
                SalePrice = p.SalePrice,
                PurchasePrice = p.PurchasePrice
            }).ToList();
        }
        public int GetBranchId(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).BranchId ?? 0;
            else return 0;
        }
        private string MakeVehcileDescription(vw_Vehicles token)
        {
            var label = "Chessis No:" + token.ChassisNo + " RegNo No:" + token.RegNo + " Manufacturer:" + token.Manufacturer;
            label += " Color:" + token.Color + " CC:" + token.EnginePower + " Model:" + token.Model + " Car Type:" + token.CarType;
            return label;
        }



        public List<vw_Vehicles> GetAll(List<int> Ids)
        {
            return AsQueryable<vw_Vehicles>(false).Where(p => Ids.Contains(p.Id)).ToList();
        }


        public Vehicle GetVehicleById(int? id)
        {
            var veh = Collection.FirstOrDefault(p => p.Id == id);
            return veh;
        }


        public bool IfChasisNumberExists(string chasisNo, int id)
        {
            return Collection.Any(p => p.ChassisNo == chasisNo && p.Id != id);
        }
        public List<vw_Vehicles> GetVehicleAvailbaleForSale(int branchId)
        {
            return AsQueryable<vw_Vehicles>(false).Where(p => !p.IsSale && p.BranchId.HasValue && p.BranchId == branchId).ToList();
        }
        public bool IfSoldVehicleExist(List<int> vehicleIds)
        {
            return Collection.Any(p => vehicleIds.Contains(p.Id) && p.IsSale);
        }
        public bool IfSold(int id)
        {
            return Collection.Any(p => p.Id == id && p.IsSale);
        }
        public List<vw_Vehicles> GetVehicleAvailbaleForBL()
        {

            var VehicleIds = AsQueryable<BLItem>(false).Select(p => p.VehicleId).Distinct().ToList();
            return AsQueryable<vw_Vehicles>(false).Where(p => !VehicleIds.Contains(p.Id) && p.Type == (byte)VehicleType.New).ToList();
        }
        public List<VehicleRecovery> GetVehicleRecovery(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleRecovery] @COMPANYID={0}, @BranchId={1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleRecovery>(query).ToList();
        }

        public List<VehicleFollowUpExtra> GetVehicleFollowUp(int branchId)
        {
            var query = string.Format("EXEC [dbo].[GetFollowUps] @COMPANYID={0}, @BranchId={1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleFollowUpExtra>(query).ToList();
        }
        public List<VehicleLogBookGet> GetVehicleLogBooks(int branchId, string soldStatus, string transfeerStatus)
        {
            var query = string.Format("EXEC [dbo].[GetVehicleLogBookDetail] @COMPANYID={0}, @BranchId={1}, @SoldStatus={2}, @TransfeerStatus={3}", SiteContext.Current.User.CompanyId, branchId, (string.IsNullOrWhiteSpace(soldStatus) ? "NULL" : "'" + soldStatus + "'"), (string.IsNullOrWhiteSpace(transfeerStatus) ? "NULL" : "'" + transfeerStatus + "'"));
            return Db.Database.SqlQuery<VehicleLogBookGet>(query).ToList();
        }

    }
}
