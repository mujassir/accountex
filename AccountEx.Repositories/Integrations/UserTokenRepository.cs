using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common;
using AccountEx.Common.Lab;
using AccountEx.DbMapping.Lab;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Integrations
{
    public class UserTokenRepository : GenericRepository<UserToken>
    {
        public UserTokenRepository() : base() { }
        public UserTokenRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public void SaveRefreshToken(UserToken token)
        {
            var dBToken = Collection.AsNoTracking().FirstOrDefault(p => p.UserId == token.UserId && p.Type == TokenType.Refresh);
            if (dBToken != null)
            {
                token.Id = dBToken.Id;
            }
            base.Save(token);
        }
        public UserToken GetRefreshTokenByUserId(int userId, int companyId)
        {
            return Db.UserTokens.FirstOrDefault(p => p.CompanyId == companyId && p.UserId == userId && p.Type == TokenType.Refresh);

        }

    }
}

