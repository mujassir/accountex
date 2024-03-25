using System.Text;
using System.Net.Mail;
using System.IO;
using AccountEx.Common;
using AccountEx.BussinessLogic.Security;
using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;
using AccountEx.Repositories;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using AccountEx.Common.Integrations;

namespace AccountEx.BussinessLogic
{


    public static class ErrorManager
    {

        private static string Get(Exception ex)
        {

            if (ex.GetType().IsAssignableFrom(typeof(OwnException)))
            {
                return ex.Message;
            }
            else
            {


#if DEBUG
                return "Exception:" + ex.Message + "-----Inner Exception:" + ex.InnerException;
#else
            return ConfigurationReader.GetConfigKeyValue("error.default.message", "The application has encountered an unknown error., It doesn't appear to have affected your data.Please notify our technical staff and they will be looking into this with the utmost urgency.");
#endif
            }



        }
        /// <summary>
        /// Log exception using Error Logging Modules and Handlers (ELMAH)
        /// </summary>
        /// <param name="error">error to be logged in ELMAH.</param>
        /// <returns> A <c>string</c> of Complete stack trace and exception details.</returns>
        public static string Log(string error)
        {
            var customEx = new Exception(error, new NotSupportedException());
            return Log(customEx);

        }
        public static string Log(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            return Get(ex);
        }
        /// <summary>
        /// Parse access token error coming from response of specified social media.
        /// </summary>
        /// <param name="json">JSON received from specified social media</param>
        /// <param name="type"><c>AuthenticationType</c> enum i-e (Facebook, Google, Twitter, Xing, LinkedIn, Viadeo)</param>
        /// <returns></returns>
        public static IntegrationErrorResponse ParseAccessTokenError(string json, AuthenticationType type)
        {
            var errorStings = new List<string>() { "error", "invalid" };
            IntegrationErrorResponse errorResponse = null;
            if (json.ToLower().Contains("error") || json.ToLower().Contains("invailed"))
            {
                switch (type)
                {
                    //case AuthenticationType.facebook:
                    //    var tokeneError = JsonConvert.DeserializeObject<FacebookErrorContainer>(json);
                    //    errorResponse = new ErrorResponse()
                    //    {
                    //        Code = ErrorCode.AccessToken,
                    //        Message = tokeneError.error.message,
                    //        Type = ErrorType.AccessToken
                    //    };
                    //    break;
                    //case AuthenticationType.twitter:
                    //    var twitterError = json;
                    //    errorResponse = new ErrorResponse()
                    //    {
                    //        Code = ErrorCode.AccessToken,
                    //        Message = twitterError,
                    //        Type = ErrorType.AccessToken
                    //    };
                    //    break;
                    default:
                        var linkedInError = JsonConvert.DeserializeObject<GoogleCalendarError>(json);
                        errorResponse = new IntegrationErrorResponse()
                        {
                            Code = IntegraionErrorCode.AccessToken,
                            Message = linkedInError.error_description,
                            Type = IntegraionErrorType.AccessToken
                        };
                        break;
                }

            }
            return errorResponse;

        }
    }
}
