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

    public static class OwnershipTransferManager
    {



        public static void Transfer(CRMOwnershipTransfer input)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new GenericRepository<CRMOwnershipTransfer>();
                repo.Save(input);
                string query = string.Format("EXEC [DBO].[CRM_OwnershipTransfer] @FromSalePersonId={0},@ToSalePersonId={1},@COMPANYID={2},@CustomerId={3}", input.FromSalePersonId, input.ToSalePersonId, SiteContext.Current.User.CompanyId, input.CustomerId);
                repo.ExecuteQuery(query);
                repo.SaveChanges();
                scope.Complete();

            }

        }


        public static string Validate(CRMOwnershipTransfer input)
        {
            var err = ",";
            try
            {
                var pmcRepo = new PMCRepository();
                var productRepo = new CRMProductRepository(pmcRepo);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    err += "you did not have sufficent right to change ownership.,";

                }

                if (input.FromSalePersonId == 0 || input.ToSalePersonId == 0)
                {
                    err += "sale person must be selected.,";
                }
                if (input.FromSalePersonId == input.ToSalePersonId)
                {
                    err += "Can't transfer ownership to same sale person.,";
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
