using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using Entities.CodeFirst;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common.VehicleSystem;

namespace AccountEx.BussinessLogic
{

    public static class CurrencyRateManager
    {
        public static void Save(CurrencyRate input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                var repo = new CurrencyRateRepository();
                var dbVehicle = new Vehicle();
                repo.Save(input);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static string ValidatePurchaseReturn(Vehicle v)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();

                if (v.IsSale)
                {
                    err += "vehicle is sold, It can't be return.,";
                }
                if (v.Status == (byte)VehicleStatus.PurchaseReturn)
                {
                    err += "Vehicle is already returned.,";
                }
                if (v.Type == (byte)VehicleType.TradIn && saleRepo.IsTradeUnitUsed(v.Id))
                {
                    err += "tarde unit is used in sale and can't be return.,";
                }
                if (SettingManager.PurchaseAccountHeadId == 0)
                {
                    err += "purchase account is missing.,";
                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }

            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }



        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new CurrencyRateRepository();
                repo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }
        }



    }
}
