using AccountEx.Common;
using System.Text.RegularExpressions;
using WebMatrix.WebData;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Helpers
{
    public class ExtendedSimpleMembershipProvider : SimpleMembershipProvider
    {

        public override bool ValidateUser(string login, string password)
        {
            var user = new Repositories.UserRepository().GetByUsername(login);
            var hash = Sha1Sign.Hash(login + password);
            var rs = !string.IsNullOrWhiteSpace(hash) && hash == user.Hash;
            if (rs) SiteContext.Current.User = UtilityFunctionManager.GetUserForSiteContext(user);
            return rs;
        }

        bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

    }
}