using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;

namespace AccountEx.Common
{
    public partial class SiteContext
    {
        #region Constants
        private const string ConstCustomersession = "CustomerSession";
        public const string ConstCustomersessioncookie = "CustomerSessionGUIDCookie";
        #endregion

        #region Fields
        private HttpContext _context1 = HttpContext.Current;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the SiteContext class
        /// </summary>
        private SiteContext()
        {
            //if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["AdminCurrentProgramID"] == null))
            //{
            //    int programId;
            //    string cookieValue = CommonHelper.GetCookieString(AdminSelectedProgramCookie, true);

            //    if (int.TryParse(cookieValue, out programId))
            //    {
            //        HttpContext.Current.Session["AdminCurrentProgramID"] = programId;
            //    }
            //}
        }
        #endregion
        /// <summary>
        /// Gets an instance of the SiteContext, which can be used to retrieve information about current context.
        /// </summary>
        public static SiteContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                if (HttpContext.Current.Items["SiteContext"] == null)
                {
                    var context = new SiteContext();
                    HttpContext.Current.Items.Add("SiteContext", context);
                    return context;
                }
                return (SiteContext)HttpContext.Current.Items["SiteContext"];
            }
        }


        #region Methods
        //public RoleAccess RoleAccess { get; set; }

        public SiteContexRoleAccess RoleAccess
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["RoleAccess"] != null))
                {
                    return HttpContext.Current.Session["RoleAccess"] as SiteContexRoleAccess;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["RoleAccess"] = value;
                }
            }
        }
        public List<int> UserRoles
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserRoles"] != null))
                {
                    return HttpContext.Current.Session["UserRoles"] as List<int>;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["UserRoles"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public List<SiteContexLogMapping> LogMappings
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["LogMappings"] != null))
                {
                    return HttpContext.Current.Session["LogMappings"] as List<SiteContexLogMapping>;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["LogMappings"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public SiteContexUser User
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["User"] != null))
                {
                    return HttpContext.Current.Session["User"] as SiteContexUser;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["User"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public SiteContexFiscal Fiscal
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["Fiscal"] != null))
                {
                    return HttpContext.Current.Session["Fiscal"] as SiteContexFiscal;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["Fiscal"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public int UserCompany
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserCompany"] != null))
                {
                    return (int)HttpContext.Current.Session["UserCompany"];
                }
                return 0;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["UserCompany"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }

        public string InfoText
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["InfoText"] != null))
                {
                    return HttpContext.Current.Session["InfoText"] + "";
                }
                return "";
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["InfoText"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }

        public string RootPath
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["RootPath"] != null))
                {
                    return HttpContext.Current.Session["RootPath"] + "";
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["RootPath"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public CRMUserType UserTypeId
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UserTypeId"] != null))
                {
                    return (CRMUserType)Numerics.GetByte(HttpContext.Current.Session["UserTypeId"]);
                }
                return CRMUserType.Client;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["UserTypeId"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public int RegionId
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["RegionId"] != null))
                {
                    return Numerics.GetInt(HttpContext.Current.Session["RegionId"]);
                }
                return 0;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["RegionId"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public int DivisionId
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["DivisionId"] != null))
                {
                    return Numerics.GetInt(HttpContext.Current.Session["DivisionId"]);
                }
                return 0;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["DivisionId"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public int RSMId
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["RSMId"] != null))
                {
                    return Numerics.GetInt(HttpContext.Current.Session["RSMId"]);
                }
                return 0;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["RSMId"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }
        public string UploadFolder
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["UploadFolder"] != null))
                {
                    return HttpContext.Current.Session["UploadFolder"] + "";
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["UploadFolder"] = value;
                    //CommonHelper.SetCookie(AdminSelectedProgramCookie, value.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }



        /// <summary>
        /// Sets cookie
        /// </summary>
        /// <param name="application">Application</param>
        /// <param name="key">Key</param>
        /// <param name="val">Value</param>
        private static void SetCookie(HttpApplication application, string key, string val)
        {
            var cookie = new HttpCookie(key)
            {
                Value = val,
                Expires =
                    string.IsNullOrEmpty(val)
                        ? DateTime.UtcNow.AddMonths(-1)
                        : DateTime.UtcNow.AddHours((double)SiteConfig.CookieExpires)
            };
            application.Response.Cookies.Remove(key);
            application.Response.Cookies.Add(cookie);
        }
        #endregion

        #region Properties




        /// <summary>
        /// Gets or sets an object item in the context by the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>


        public object Cache(string key)
        {
            return HttpContext.Current.Application[key];
        }
        public void Cache(string key, object value)
        {
            HttpContext.Current.Application.Add(key, value);
        }


        /// <summary>
        /// Gets an user host address
        /// </summary>
        public string UserHostAddress
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UserHostAddress != null)
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                return string.Empty;
            }
        }



        /// <summary>
        /// Sets the CultureInfo 
        /// </summary>
        /// <param name="culture">Culture</param>
        public void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }


        /// <summary>
        /// Currently selected program
        /// </summary>
        public Dictionary<int, string> Statuses
        {
            get
            {
                if (HttpContext.Current.Application["Statuses"] != null)
                {
                    return HttpContext.Current.Application["Statuses"] as Dictionary<int, string>;
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Application != null))
                {
                    HttpContext.Current.Application["Statuses"] = value;
                }
            }
        }
        /// <summary>
        /// Setting is fully cache based
        /// </summary>
        public Dictionary<string, string> Settings
        {
            get
            {
                if ((HttpContext.Current.Application != null) && (HttpContext.Current.Application["Setting_" + User.CompanyId] != null))
                {
                    return (Dictionary<string, string>)HttpContext.Current.Application["Setting_" + User.CompanyId];
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Application != null))
                {
                    HttpContext.Current.Application["Setting_" + User.CompanyId] = value;
                }
            }
        }
        public Dictionary<string, string> FiscalSettings
        {
            get
            {
                if ((HttpContext.Current.Application != null) && (HttpContext.Current.Application["FiscalSetting_" + User.CompanyId] != null))
                {
                    return (Dictionary<string, string>)HttpContext.Current.Application["FiscalSetting_" + User.CompanyId];
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Application != null))
                {
                    HttpContext.Current.Application["FiscalSetting_" + User.CompanyId] = value;
                }
            }
        }


        public List<SiteContextMenuItemExtra> MenuItems
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["Menu_" + User.Id] != null))
                {
                    return (List<SiteContextMenuItemExtra>)HttpContext.Current.Session["Menu_" + User.Id];
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["Menu_" + User.Id] = value;
                }
            }
        }

        public Dictionary<string, bool> Actions
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["Action_" + User.Id] != null))
                {
                    return (Dictionary<string, bool>)HttpContext.Current.Session["Action_" + User.Id];
                }
                return null;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["Action_" + User.Id] = value;
                }
            }
        }




        #endregion
    }

}
