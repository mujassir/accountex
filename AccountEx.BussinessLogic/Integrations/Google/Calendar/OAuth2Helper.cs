using System.Web;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using AccountEx.Common;
using AccountEx.Common.Integrations;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.CRM;
using System;
using AccountEx.Repositories.Integrations;
using AccountEx.CodeFirst.Models;
using System.IO;
using System.Collections.Generic;

namespace AccountEx.BussinessLogic.Integrations.Google.Calendar
{
    /// <summary>
    /// Implementation of OAuthAuthorization 2.0 used by Authorization server to communicate with the web application while processing requests.
    /// </summary>
    public class OAuth2Helper
    {
        /// <summary>
        /// Constructor of class taking parameter of <code>AuthenticationType</code>.
        /// Initializing properties of class by assigning them values from <code>IntegrationSetting./code> class.
        /// </summary>
        /// <param name="type"><c>AuthenticationType</c> enum i-e (Facebook, Google, Twitter, Xing, LinkedIn, Viadeo)</param>
        public OAuth2Helper(AuthenticationType type)
        {
            AuthenticationMethod = type;
            var prefix = type + "-";
            ConsumerKey = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.ConsumerKey);
            ConsumerSecret = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.ConsumerSecret);
            RedirectUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.RedirectUri);
            ApiBaseUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.ApiBaseUri);
            AutorizationUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.AutorizationUri);
            AutorizationScope = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.AutorizationScope);
            AccessTokenUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.AccessTokenUri);
            ProfileUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.ProfileUri);
            RequestTokenUri = ConfigurationReader.GetConfigKeyValue(prefix + IntegrationSetting.RequestTokenUri);
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                if (request.ApplicationPath != null)
                    SiteBaseUri = request.Url.Scheme + "://" + request.Url.Authority +
                                  request.ApplicationPath.TrimEnd('/') + "/";
            }
            else
            {
                SiteBaseUri = ConfigurationReader.GetConfigKeyValue("BaseUrl");
            }


        }

        /// <summary>Gets AuthenticationMethod property</summary>
        public AuthenticationType AuthenticationMethod { get; }

        /// <summary>
        /// Consumer key is essentially the API key associated with the application (Twitter, Facebook, etc.). 
        /// Facebook calls it <c>ClientID</c>, Twitter calls it <c>ConsumerKey</c>
        /// This key or 'client ID', as Facebook calls it is what identifies the client with specified social media.
        /// </summary>
        public string ConsumerKey { get; }

        /// <summary>Consumer secret is the client password <c>(ClientSecretKey)</c> that is used to authenticate with the authorization server</summary>
        public string ConsumerSecret { get; }

        /// <summary>
        /// Called to validate that the context.ClientId is a registered "client_id", and that the context.RedirectUri is a "redirect_uri"
        /// registered for that client. This only occurs when processing the Authorize endpoint. The application MUST implement this
        /// call, and it MUST validate both of those factors before calling context.Validated. If the request is made
        /// with a given redirectUri parameter, then this request will only become true if the incoming redirect URI matches the given redirect URI.
        /// If it mis matches then the request will not proceed further.
        /// </summary>
        public string RedirectUri { get; }

        /// <summary>Gets ApiBaseUri property</summary>
        public string ApiBaseUri { get; }

        /// <summary>Gets AutorizationUri property</summary>
        public string AutorizationUri { get; }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "authorization_code". This occurs after the Authorize
        /// endpoint as redirected the user-agent back to the client with a "code" parameter, and the client is exchanging that for an "access_token".
        /// This <c>AuthorizationCode</c> must be validated in order to instruct the Authorization
        /// Server middleware to issue an access token. Keep flow of information from the authorization code to the access token unmodified.
        /// </summary>
        public string AutorizationScope { get; }

        /// <summary>Gets AccessTokenUri property</summary>
        public string AccessTokenUri { get; }

        /// <summary>Gets ProfileUri property</summary>
        public string ProfileUri { get; }

        /// <summary>Gets RequestTokenUri property</summary>
        public string RequestTokenUri { get; }

        /// <summary>
        /// Called for each request to the Token endpoint to determine if the request is valid and should continue.
        /// The default behavior when using the OAuthAuthorization is to assume well-formed requests, with
        /// validated client credentials, should continue processing. An application may add any additional constraints.
        /// </summary>
        public string RequestToken { get; private set; }

        /// <summary>Gets SiteBaseUri property</summary>
        public string SiteBaseUri { get; }

        ///<summary>If the access token request is valid and authorized, 
        /// the authorization server issues an access token and optional refresh token. 
        /// If the request failed client authentication or is invalid, the authorization server returns an error response.</summary>
        public string AccessToken { get; private set; }




        /// <summary>If the request failed client authentication or is invalid, the authorization server returns an error response.</summary>
        public string Error { get; set; }

        /// <summary>The authorization server responds with an HTTP 400 (Bad Request)
        ///status code(unless specified otherwise) and includes these parameters with the response:
        /// <c>invalid_request</c>, <c>invalid_client</c>, <c>invalid_grant</c> <c>unauthorized_client</c>, <c>unsupported_grant_type</c></summary>
        public string ErrorCode { get; set; }

        /// <summary>The authorization server responds with error description which is Human-readable ASCII [USASCII] text providing
        ///additional information, used to assist the client developer in understanding the error that occurred.
        /// Values for the <c>"error_description"</c> parameter MUST NOT include//characters outside the set %x20-21 / %x23-5B / %x5D-7E</summary>
        public string ErrorDescription { get; set; }

        /// <summary> Constructs social media login url based on parameters i-e <code>client_id</code>, 
        /// <code>scope</code>, <code>response_type</code>, <code>redirect_uri</code>, <code>state</code> and <code>method</code> which are set 
        /// from the configuration used in web.config. </summary>
        /// <returns>Returns social media login Url with authorization code.</returns>
        public string GetLoginUrl(bool offlineAccess = false)
        {
            var queryStringFormat = "";
            var queryString = "";

            var acceessType = "offline";
            var approvalPrompt = "force";
            if (offlineAccess)
            {
                queryStringFormat = "client_id={0}&scope={1}&response_type={2}&redirect_uri={3}&state={4}&method={5}&access_type={6}&prompt={7}";
                queryString = string.Format(queryStringFormat,
               ConsumerKey,
               AutorizationScope,
               IntegrationSetting.AutorizationResponsType,
               SiteBaseUri + RedirectUri,
               UtilityFunctionManager.GenerateSatate(),
               AuthenticationMethod, "offline", "consent"
               );
            }
            else
            {
                queryStringFormat = "client_id={0}&scope={1}&response_type={2}&redirect_uri={3}&state={4}&method={5}";
                queryString = string.Format(queryStringFormat,
               ConsumerKey,
               AutorizationScope,
               IntegrationSetting.AutorizationResponsType,
               SiteBaseUri + RedirectUri,
               UtilityFunctionManager.GenerateSatate(),
               AuthenticationMethod
               );
            }


            return AutorizationUri + "?" + queryString;
        }

        /// <summary> Makes a request to the token endpoint by sending 
        /// <c>grant_type</c>, <c>code</c>, <c>redirect_uri</c>, <c>client_id</c>, <c>client_secret</c> using the "application/x-www-form-urlencoded"
        ///format per Appendix B with a character encoding of UTF-8 in the HTTP
        ///request entity-body:</summary>
        /// <param name="code">REQUIRED.  The authorization code received from the authorization server.</param>
        /// <param name="errorResponse">output parameter for any kind of erorr response</param>
        /// <returns>Returns <c>AccessToken</c> using authorization code and specified parameters with request.</returns>
        public AccessToken GetAccessToken(string code)
        {
            AccessToken tokenInfo = null;
            try
            {
                var redirectUri = SiteBaseUri + RedirectUri;
                var client = new RestClient(AccessTokenUri);
                var request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", redirectUri);
                request.AddParameter("client_id", ConsumerKey);
                request.AddParameter("client_secret", ConsumerSecret);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = client.Execute(request);
                var json = response.Content;
                tokenInfo = JsonConvert.DeserializeObject<AccessToken>(json);
                ErrorManager.ParseAccessTokenError(json, AuthenticationMethod);
                if (tokenInfo != null)
                {
                    AccessToken = tokenInfo.access_token;
                    if (!string.IsNullOrWhiteSpace(tokenInfo.refresh_token))
                        SaveRefreshToken(tokenInfo);

                }

                if (string.IsNullOrWhiteSpace(AccessToken))
                    ErrorManager.Log("Content:" + json + " Error Message:" + response.ErrorMessage + " Error Exception:" + response.ErrorException);

            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
            }
            return tokenInfo;


        }

        /// <summary> Makes a request to the token endpoint by sending 
        /// <c>grant_type</c>, <c>code</c>, <c>redirect_uri</c>, <c>client_id</c>, <c>client_secret</c> using the "application/x-www-form-urlencoded"
        ///format per Appendix B with a character encoding of UTF-8 in the HTTP
        ///request entity-body:</summary>
        /// <param name="code">REQUIRED.  The authorization code received from the authorization server.</param>
        /// <param name="errorResponse">output parameter for any kind of erorr response</param>
        /// <returns>Returns <c>AccessToken</c> using authorization code and specified parameters with request.</returns>
        public void SaveRefreshToken(AccessToken token)
        {

            var repo = new UserTokenRepository();
            var userToken = new UserToken()
            {
                Token = token.refresh_token,
                UserId = SiteContext.Current.User.Id,
                Type = TokenType.Refresh
            };
            repo.SaveRefreshToken(userToken);


        }

        /// <c>grant_type</c>, <c>code</c>, <c>redirect_uri</c>, <c>client_id</c>, <c>client_secret</c> using the "application/x-www-form-urlencoded"
        ///format per Appendix B with a character encoding of UTF-8 in the HTTP
        ///request entity-body:</summary>
        /// <param name="code">REQUIRED.  The authorization code received from the authorization server.</param>
        /// <param name="errorResponse">output parameter for any kind of erorr response</param>
        /// <returns>Returns <c>AccessToken</c> using authorization code and specified parameters with request.</returns>
        public AccessToken GetAccessTokenFromRefreshToken(string refreshToken)
        {
            var redirectUri = SiteBaseUri + RedirectUri;
            var client = new RestClient(AccessTokenUri);
            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("client_id", ConsumerKey);
            request.AddParameter("client_secret", ConsumerSecret);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var response = client.Execute(request);
            var json = response.Content;
            var tokenInfo = JsonConvert.DeserializeObject<AccessToken>(json);
            ErrorManager.ParseAccessTokenError(json, AuthenticationMethod);
            if (tokenInfo != null)
                AccessToken = tokenInfo.access_token;
            if (string.IsNullOrWhiteSpace(AccessToken))
                ErrorManager.Log("Content:" + json + " Error Message:" + response.ErrorMessage + " Error Exception:" + response.ErrorException);
            return tokenInfo;

        }


        public InsertEventResponse CreateGoogleCalendarEvent(GoogleCalendarEvent googleCalendarEvent)
        {
            var authHeaderFormat = "Bearer {0}";
            var authHeader = string.Format(authHeaderFormat, AccessToken);
            var data = JsonConvert.SerializeObject(googleCalendarEvent);
            var url = ProfileUri;
            if (!string.IsNullOrWhiteSpace(googleCalendarEvent.Id))
            {
                url += "/" + googleCalendarEvent.Id;
            }
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            if (!string.IsNullOrWhiteSpace(googleCalendarEvent.Id))
            {

                httpWebRequest.Method = "PUT";
            }
            httpWebRequest.Headers.Add("Authorization", authHeader);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var result = "";
            var googleEvent = new InsertEventResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<InsertEventResponse>(result);


        }

        ///// <summary> Returns user profile using Oauth 2.0 access token.  </summary>
        ///// <param name="errorResponse">output parameter for any kind of erorr response</param>
        ///// <returns></returns>
        //public IProfile GetProfile(out ErrorResponse errorResponse)
        //{

        //    var client = new RestClient(ProfileUri);

        //    var request = new RestRequest(Method.GET);
        //    switch (AuthenticationMethod)
        //    {

        //        case AuthenticationType.linkedin:
        //            request.AddParameter("oauth2_access_token", AccessToken);
        //            request.AddParameter("format", "json");
        //            break;
        //        case AuthenticationType.viadeo:
        //            var authHeaderFormat = "Bearer {0}";
        //            var authHeader = string.Format(authHeaderFormat, AccessToken);
        //            request.AddHeader("Authorization", authHeader);

        //            break;
        //        case AuthenticationType.hotmail:
        //            authHeaderFormat = "Bearer {0}";
        //            authHeader = string.Format(authHeaderFormat, AccessToken);
        //            request.AddHeader("Authorization", authHeader);


        //            break;

        //        default:
        //            request.AddParameter("access_token", AccessToken);
        //            break;
        //    }

        //    var response = client.Execute(request);
        //    var json = response.Content;
        //    var profile = DataParser.ParseData(json, AuthenticationMethod);
        //    errorResponse = ErrorManager.ParseProfileError(json, AuthenticationMethod);
        //    return profile;
        //}
    }
}
