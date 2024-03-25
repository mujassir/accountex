using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.Web.Controllers.api
{
    public class SuggestionController : BaseApiController
    {
        // GET api/test
        public List<AccountDetail> Get()
        {
            //var query = Request.GetQueryString("query");
            var list =
                new AccountDetailRepository().AsQueryable()
                    .Where(p => p.AccountDetailFormId == (int)AccountDetailFormType.Products).ToList();
            return list;
        }
    }
}
