using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class InvoiceClearingController : BaseApiController
    {
        public ApiResponse Post([FromBody]List<InvoiceClearingExtra> input)
        {
            ApiResponse response;
            try
            {
                var records = new List<InvoiceClearing>();
                var repo = new SaleRepository();
                var invoiceClearingRepo = new InvoiceClearingRepository(repo);
                foreach (var item in input)
                {
                    if (item.NetTotal == item.TotalPaid + item.Amount)
                    {
                        var invoiceRecord = repo.GetById(item.Id, true); 
                        invoiceRecord.IsCleared = true;
                        repo.Update(invoiceRecord);
                    }

                    var record = new InvoiceClearing
                    {
                        AccountId = item.AccountId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        InvoiceId = item.Id,
                        Date = item.Date,
                        InvoiceNo = item.VoucherNumber,
                        Amount = item.Amount,
                    };
                    records.Add(record);
                }

                invoiceClearingRepo.Save(true, records);
                response = new ApiResponse()
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
    }
}
