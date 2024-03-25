using System;
using System.Collections.Generic;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.Web.Controllers.api.Account
{
    public class AccountSuggestionController : ApiController
    {
        public IEnumerable<AccountCode> Get()
        {
            var parentId = Convert.ToInt32(Request.GetQueryString("parentId"));
            var query = Request.GetQueryString("query");
            var list = string.IsNullOrWhiteSpace(query) ? new AccountRepository().GetAccountSuggestion(parentId) 
                : new AccountRepository().GetAccountSuggestion(parentId, query.ToLower());
            return list;
        }

    }
}
