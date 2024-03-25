using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.Integrations
{

    /// <summary>
    /// this enum provide the name of differnt errors 
    /// </summary>
    public enum IntegraionErrorCode
    {
        MehotdNotSupported = 300,
        InvalidCredential = 400,
        UnhandledError = 401,
        AccessToken = 402,
        Profile = 403,
        RequestToken = 404,
    }

    /// <summary>
    /// this enum provide the name of  main type that  can possess sub type of error 
    /// </summary>
    public enum IntegraionErrorType
    {
        Custom = 1,
        InvalidCredential = 2,
        AccessToken = 3,
        RequestToken = 3,
        Profile = 403
    }

    /// <summary>
    /// Provides attributes related to error logging and details.
    /// </summary>
    public class IntegrationErrorResponse
    {
        /// <summary>
        /// Gets or  Sets the error code.
        /// </summary>
        public IntegraionErrorCode Code { get; set; }
        /// <summary>
        /// Gets or Sets the error message and inner exception.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or Sets the error type.
        /// </summary>
        public IntegraionErrorType Type { get; set; }
    }

    /// <summary>
    /// Provides attributes related to LinkedIn error logging and details.
    /// </summary>
    public class GoogleCalendarError
    {
        /// <summary>
        /// Gets or  Sets the LinkedIn error code.
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// Gets or Sets the error description and inner exception from LinkedIn.
        /// </summary>
        public string error_description { get; set; }
        /// <summary>
        /// Gets or Sets the LinkedIn error type.
        /// </summary>
        public string type { get; set; }
    }
}
