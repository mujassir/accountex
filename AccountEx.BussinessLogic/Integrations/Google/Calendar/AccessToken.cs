using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.Integrations.Google.Calendar
{

    /// <summary>
    /// When someone connects with an app using social media Login, the app will be able to obtain an access token which provides temporary, secure access to social media APIs.
    /// An access token is an opaque string that identifies a user, app, or Page and can be used by the app to make API calls.
    /// The token includes information about when the token will expire and which app generated the token. Because of privacy checks, the majority of API calls on api need to include an access token.
    /// </summary>
    /// <param name="access_token"><code>String</code>Gets the Access Token requested. </param>
    /// <param name="expires_in"><code>DateTimeOffset</code>Gets the point in time in which the Access Token returned in the AccessToken property ceases to be valid. This value is calculated based on current UTC time measured locally and the value expiresIn received from the service.</param>
    /// <param name="token_type"><code>String-</code>Gets the type of the Access Token returned.</param>
    /// <param name="refresh_token"><code>String</code>Refresh token is a special kind of token that contains the infomation required to obtain  a new aceess token </param>

    public class AccessToken
    {
        
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }


    }

    public class AccessTokenOauth1
    {

        public string oauth_token { get; set; }
        public string oauth_token_secret { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }


    }
}
