using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;

namespace AccountEx.Web.Controllers.api
{
    public class VoucherPrintController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return null;
        }
        public ApiResponse Get(int id)
        {
            try
            {
                var transactions = new TransactionRepository().GetByVoucherNumber(VoucherType.CashPayments, id);
                var tr = transactions.FirstOrDefault(p => p.EntryType == (byte)EntryType.MasterDetail);
                return new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        VoucherNumber = tr.VoucherNumber,
                        Date = tr.Date.ToString(AppSetting.DateFormat),
                        AccountTitle = tr.AccountTitle,
                        Amount = Numerics.DecimalToString(tr.Debit),
                        Comments = tr.Comments,
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Error = ex.Message,
                };
            }
        }
    }
}
