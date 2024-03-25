/*
Copyright 2015 Google Inc

Licensed under the Apache License, Version 2.0(the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.Integrations.Google.Calendar;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using AccountEx.Common.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AccountEx.Web.Controllers.mvc
{
    public class GoogleCalendarIntegrationController : Controller
    {
        public ActionResult Calendar()
        {
            try
            {

                //var method = Request["method"];
                //var account = Request["account"];
                //var username = Request["username"];
                //var password = Request["password"];

                var username = ConfigurationReader.GetConfigKeyValue("UserName");
                var password = ConfigurationReader.GetConfigKeyValue("Password");
                var account = ConfigurationReader.GetConfigKeyValue("Account");

                var postProfileUrl = Request["postProfileUrl"];
                //Session["postProfileUrl"] = postProfileUrl;
                //var postProfileUrl = ConfigurationReader.GetConfigKeyValue("PostProfileUrl");
                Response.Cookies.Add(new HttpCookie("postProfileUrl", postProfileUrl) { Expires = DateTime.Now.AddHours(1) });

                // var err = ProfileManager.Validate(method, account, username, password, postProfileUrl);
                var err = "";
                if (string.IsNullOrWhiteSpace(err))
                {
                    var authenticationType = AuthenticationType.googlecalendar;
                    ProceedAuthentication(authenticationType);
                }
                else
                {
                    ErrorManager.Log(err);
                    ViewBag.Error = err;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ErrorManager.Log(ex);
                ErrorManager.Log(ex);
            }
            return View();
        }

        /// <summary>
        /// After authentication redirects user to specified social media login page.
        /// </summary>
        /// <param name="type"><c>AuthenticationType</c> enum i-e (Facebook, Google, Twitter, Xing, LinkedIn, Viadeo)</param>
        /// <exception cref="NullReferenceException">Parameter <c>NullReferenceException</c></exception>
        private void ProceedAuthentication(AuthenticationType type)
        {
            switch (type)
            {
                case AuthenticationType.googlecalendar:
                    OAuth2Helper obj = new OAuth2Helper(type);
                    Response.Redirect(obj.GetLoginUrl(true), true);
                    break;

                default:
                    obj = new OAuth2Helper(type);
                    Response.Redirect(obj.GetLoginUrl(), true);
                    break;
            }

        }

        /// <summary>
        /// Retreive user profile from specified social media and returns XML Document.
        /// </summary>
        /// <param name="code">Authorization code received from specified social media</param>
        /// <param name="state">Code for the validation of accesstoken.</param>
        /// <param name="type"><c>AuthenticationType</c> enum i-e (Facebook, Google, Twitter, Xing, LinkedIn, Viadeo)</param>
        /// <returns>Returns received profile information as required format of XML Document </returns>
        ///   /// <exception cref="ArgumentNullException"></exception>
        public ActionResult CalendarCallback(string code, string state)
        {
            IntegrationErrorResponse error = null;
            OAuth2Helper helper = new OAuth2Helper(AuthenticationType.googlecalendar);
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {

                    var toekn=helper.GetAccessToken(code);
                    if (!string.IsNullOrWhiteSpace(helper.AccessToken))
                    {
                        var crmevent = new vw_CRMCalendarEvents();
                       // helper.CreateGoogleCalendarEvent(crmevent);
                    }
                }
                else
                {


                    var ex = new Exception("Unable to Fetch autorization code.", new UnauthorizedAccessException());
                    ErrorManager.Log(ex);
                    var returnUrl = !string.IsNullOrWhiteSpace(Request.Cookies["postProfileUrl"] + "") ? Request.Cookies["postProfileUrl"].Value : "";
                    //returnUrl = ConfigurationReader.GetConfigKeyValue("PostProfileUrl");
                    Response.Redirect(returnUrl, false);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
            }
            if (string.IsNullOrWhiteSpace(helper.AccessToken))
            {
                var ex = new Exception("Unable to Fetch Access token", new UnauthorizedAccessException());
            }
            if (error != null)
            {
                ViewBag.Error = error;
            }
            return RedirectToAction("calendarevents", "crm");
            

        }

        

    }
}