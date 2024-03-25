namespace AccountEx.Common.Integrations
{
    /// <summary>
    /// Get and Set the credantial to access the web api  and get required information 
    /// </summary>
    public static class IntegrationSetting
    {
        public static readonly string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Consumer key is essentially the API key associated with the application (Twitter, Facebook, etc.). 
        /// Facebook calls it <c>ClientID</c>, Twitter calls it <c>ConsumerKey</c>
        /// This key or 'client ID', as Facebook calls it is what identifies the client with specified social media.
        /// </summary>
        public static readonly string ConsumerKey = "consumer-key";

        /// <summary>Consumer secret is the client password <c>(ClientSecretKey)</c> that is used to authenticate with the authorization server</summary>
        public static readonly string ConsumerSecret = "consumer-secret";
        /// <summary>
        /// Called to validate that the context.ClientId is a registered "client_id", and that the context.RedirectUri is a "redirect_uri"
        /// registered for that client. This only occurs when processing the Authorize endpoint. The application MUST implement this
        /// call, and it MUST validate both of those factors before calling context.Validated. If the request is made
        /// with a given redirectUri parameter, then this request will only become true if the incoming redirect URI matches the given redirect URI.
        /// If it mis matches then the request will not proceed further.
        /// </summary>
        public static readonly string RedirectUri = "redirect_url";
        /// <summary>Gets ApiBaseUri property</summary>
        public static readonly string ApiBaseUri = "api_base_url";
        /// <summary>Gets AutorizationUri property</summary>
        public static readonly string AutorizationUri = "authorization-endpoint_url";
        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "authorization_code". This occurs after the Authorize
        /// endpoint as redirected the user-agent back to the client with a "code" parameter, and the client is exchanging that for an "access_token".
        /// This <c>AuthorizationCode</c> must be validated in order to instruct the Authorization
        /// Server middleware to issue an access token. Keep flow of information from the authorization code to the access token unmodified.
        /// </summary>
        public static readonly string AutorizationScope = "authorization_scope";

        /// <summary>Gets AccessTokenUri property</summary>
        public static readonly string AccessTokenUri = "access-token-endpoint_url";

        /// <summary>Gets ProfileUri property</summary>

        public static readonly string ProfileUri = "profile-endpoint_url";
        /// <summary>Gets RequestTokenUri property</summary>
        public static readonly string RequestTokenUri = "request_token-endpoint_url";

        /// <summary>Gets SiteBaseUri property</summary>
        public static readonly string SiteBaseUri = "site-base-url";

        /// <summary>Gets AutorizationResponsType property</summary>
        public static readonly string AutorizationResponsType = "code";
        /// <summary>
        /// Gets MethodNotSupported  message   field
        /// </summary>
        public static readonly string MethodNotSupported = "This method is currenlty not supported by the system.";
        /// <summary>
        /// Gets InvalidCredential  message   field
        /// </summary>
        public static readonly string InvalidCredential = "Credentail are invalid.";
        /// <summary>
        /// Gets UnhandledError  message   field
        /// </summary>
        public static readonly string UnhandledError = "An unhandled error has occured.";

        /// <summary>
        /// Gets UserDeniedAccess   field
        /// </summary>
        public static readonly string UserDeniedAccess = "User has deinded access to application.";

    }
}
