using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{
    public static class LeadsManager
    {
        public static void Save(Lead input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new LeadRepository();
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                repo.Save(input);
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static int GetNextLeadNo()
        {
            var leadRepo = new LeadRepository();
            return (leadRepo.GetNextLeadNo());
        }
    }
}
