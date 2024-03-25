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

    public static class CalendarEventManager
    {



        public static void Save(List<CalendarEvent> events)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CalendarEventRepository();
                var tranrRepo = new TransactionRepository(repo);
                repo.Save(events);
                repo.SaveChanges();
                scope.Complete();

            }

        }



        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new CalendarEventRepository();
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



        public static string ValidateSave(List<CalendarEvent> events)
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
                foreach (var item in events)
                {
                    //if ((item.ProjectId == 0 || !item.ProjectId.HasValue) && (item.ProductId == 0 || !item.ProductId.HasValue))
                    //{
                    //    err += "project or product must be selected for every record.,";
                    //}
                    if ((item.ModeId == 0 || !item.ModeId.HasValue))
                    {
                        err += "mode must be selected for every record.,";
                    }
                    if ((item.StatusId == 0))
                    {
                        err += "status must be selected for every record.,";
                    }
                    if(item.EndTime<=item.StartTime)
                    {
                        err += "End time must be greater than Start time.,";
                    }
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
