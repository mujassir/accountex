using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{

    public static class CRMSaleForecastManager
    {



        public static void Save(List<CRMSaleForecast> saleForecastRecrods)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMSaleForecastRepository();
                var tranrRepo = new TransactionRepository(repo);
                saleForecastRecrods.ForEach(p => p.FiscalId = SiteContext.Current.Fiscal.Id);
                var years = saleForecastRecrods.GroupBy(p => p.Year).Select(p => p.Key).Distinct().ToList();
                var type = saleForecastRecrods.FirstOrDefault().Type;
                var query = "";
                foreach (var year in years)
                {
                    var months = saleForecastRecrods.Where(p => p.Year == year).Select(p => p.Month).Distinct().ToList();
                    if (months.Count == 0)
                        months.Add(0);

                    var gbsp = saleForecastRecrods.GroupBy(p => p.SalePersonId).Select(p => new
                    {
                        SalePersonId = p.Key,
                        Records = p.ToList()
                    });

                    foreach (var sf in gbsp)
                    {
                        query += "DELETE FROM dbo.CRMSaleForecasts WHERE SalePersonId=" + sf.SalePersonId + " AND [Year]=" + year + " AND [Month] IN(" + string.Join(",", months) + ") AND Type=" + type + " AND CompanyId=" + SiteContext.Current.User.CompanyId + ";" + Environment.NewLine;

                    }


                }
                if (!string.IsNullOrWhiteSpace(query))
                    repo.ExecuteQuery(query);



                repo.Save(saleForecastRecrods);
                repo.SaveChanges();
                scope.Complete();

            }

        }


        public static void SaveRSM(List<CRMSaleForecast> saleForecastRecrods)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMSaleForecastRepository();
                var tranrRepo = new TransactionRepository(repo);
                saleForecastRecrods.ForEach(p => p.FiscalId = SiteContext.Current.Fiscal.Id);
                var years = saleForecastRecrods.GroupBy(p => p.Year).Select(p => p.Key).Distinct().ToList();
                var type =(byte)CRMSaleForecastType.RSM;
                var query = "";
                foreach (var year in years)
                {
                    var months = saleForecastRecrods.Where(p => p.Year == year).Select(p => p.Month).Distinct().ToList();
                    if (months.Count == 0)
                        months.Add(0);

                    var gbsp = saleForecastRecrods.GroupBy(p => p.SalePersonId).Select(p => new
                    {
                        SalePersonId = p.Key,
                        Records = p.ToList()
                    });

                    foreach (var sf in gbsp)
                    {
                        query += "DELETE FROM dbo.CRMSaleForecasts WHERE CreatedBy=" + SiteContext.Current.User.Id + " AND [Year]=" + year + " AND [Month] IN(" + string.Join(",", months) + ") AND Type=" + type + " AND CompanyId=" + SiteContext.Current.User.CompanyId + ";" + Environment.NewLine;

                    }


                }
                if (!string.IsNullOrWhiteSpace(query))
                    repo.ExecuteQuery(query);



                repo.Save(saleForecastRecrods);
                repo.SaveChanges();
                scope.Complete();

            }

        }

        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CRMSaleForecastRepository();
                repo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void Delete(int voucherno, List<VoucherType> transactionTypes)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var tranRepo = new TransactionRepository();
                var saleRepo = new SaleRepository(tranRepo);
                tranRepo.HardDelete(voucherno, transactionTypes);
                saleRepo.DeleteByVoucherNumber(voucherno, transactionTypes);
                saleRepo.SaveChanges();
                scope.Complete();
            }

        }



        public static string ValidateSave(List<CRMSaleForecast> events)
        {
            var err = ",";
            try
            {
                var pmcRepo = new PMCRepository();
                if (!SiteContext.Current.User.IsAdmin)
                {

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
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
    }
}
