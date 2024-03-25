using System.Text;
using System.IO;
using AccountEx.Common;
using System.Reflection;
using AccountEx.CodeFirst.Models.COA;
using System.Globalization;
using System.Data;
using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;
using System.Linq;
using System.Web;
using System.Net;

namespace AccountEx.BussinessLogic
{

    public static class UtilityFunctionManager
    {
        /// <summary>
        /// Convert a DataTable to JSON string
        /// </summary>
        /// <param name="dt">Input DataTable object</param>
        /// <returns>JSON string</returns>
        public static string DataTableToJson(DataTable dt)
        {
            var dateFormat = "yyyy-MM-dd hh:mm:ss tt";
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        string stringValue = ds.Tables[0].Rows[i][j].ToString();
                        if (ds.Tables[0].Rows[i][j].GetType() == typeof(DateTime))
                        {
                            stringValue = ((DateTime)ds.Tables[0].Rows[i][j]).ToString(dateFormat);
                        }
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + stringValue + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + stringValue + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
        public static string ReplacePlaceHolder(string filePath, object data)
        {
            string html = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(filePath));
            foreach (var prop in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                html = html.Replace("{" + prop.Name + "}", prop.GetValue(data, null) + "");
            }
            return html;


        }
        public static string ReplacePlaceHolder(object data, string html)
        {

            foreach (var prop in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                html = html.Replace("{" + prop.Name + "}", prop.GetValue(data, null) + "");
            }
            return html;


        }
        public static string GetLocalStorgaeKey()
        {
            if (SiteContext.Current.Fiscal != null)
                return Sha2Sign.Hash(SiteContext.Current.User.CompanyId);
            else return "";

        }
        public static dynamic GetFiscalForLocalStorgae()
        {
            if (SiteContext.Current.Fiscal != null)
            {
                var fiscal = SiteContext.Current.Fiscal;
                return new
                {
                    fiscal.Id,
                    fiscal.FromDate,
                    fiscal.ToDate
                };

            }
            else return "";

        }
        public static SiteContexFiscal GetFiscalForSiteContext(Fiscal f)
        {
            if (f == null)
                return null;
            return new SiteContexFiscal()
            {
                Id = f.Id,
                Name = f.Name,
                FromDate = f.FromDate,
                ToDate = f.ToDate,
                IsClosed = f.IsClosed,
                IsDefault = f.IsDefault,
                CompanyId = f.CompanyId.Value

            };



        }
        public static SiteContexUser GetUserForSiteContext(User u)
        {
            if (u == null)
                return null;

            return new SiteContexUser()
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                CanChangeFiscal = u.CanChangeFiscal,
                BranchId = u.BranchId,
                CompanyId = u.CompanyId.Value

            };



        }
        public static SiteContexRoleAccess GetRoleAccessForSiteContext(RoleAccess access)
        {
            if (access == null)
                return null;
            return new SiteContexRoleAccess()
            {
                Id = access.Id,
                RoleId = access.RoleId,
                MenuItemId = access.MenuItemId,
                CanView = access.CanView,
                CanUpdate = access.CanUpdate,
                CanCreate = access.CanCreate,
                CanAuthorize = access.CanAuthorize,
                CanDelete = access.CanDelete,
                CompanyId = access.CompanyId.HasValue ? access.CompanyId.Value : 0

            };
        }
        public static List<SiteContexLogMapping> GetLogMappingForSiteContext(List<LogMapping> mappings)
        {

            return mappings.Select(p => new SiteContexLogMapping()
            {
                Id = p.Id,
                TableName = p.TableName,
                LogKey = p.LogKey,
                LogType = p.LogType,
                Description = p.Description,
                ModuleKey = p.ModuleKey,
                CompanyId = p.CompanyId.Value
            }).ToList();
        }
        public static SiteContextMenuItemExtra GetMenuItemForSiteContext(MenuItem menuItem)
        {
            if (menuItem == null)
                return null;

            return new SiteContextMenuItemExtra()
            {
                Id = menuItem.Id,
                ParentMenuItemId = menuItem.ParentMenuItemId,
                Title = menuItem.Title,
                Url = menuItem.Url,
                IconClass = menuItem.IconClass,
                IsMegaMenu = menuItem.IsMegaMenu,
                HasChild = menuItem.HasChild,
                SequenceNumber = menuItem.SequenceNumber,
                IsVisible = menuItem.IsVisible,
                DataType = menuItem.DataType,
                CompanyId = menuItem.CompanyId.Value

            };
        }
        public static string GetChallanPeriod(Challan challan)
        {
            return GetChallanPeriod(challan.Month, challan.Year, challan.ToMonth, challan.ToYear);
        }

        public static string GetChallanPeriod(int fromMonth, int fromYear, int toMonth, int toYear)
        {
            var duration = "";
            if (fromMonth == toMonth && fromYear == toYear)
                duration = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fromMonth) + " " + fromYear;
            else if (fromMonth != toMonth && fromYear == toYear)
                duration = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fromMonth) + " to " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(toMonth) + " " + fromYear;
            else
                duration = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fromMonth) + " " + fromYear + " to "
            + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(toMonth) + " " + toYear;
            return duration;

        }
        public static dynamic GetRoleAccesslForLocalStorgae()
        {
            if (SiteContext.Current.RoleAccess != null)
            {
                var roleAccess = SiteContext.Current.RoleAccess;
                return new
                {
                    roleAccess.CanView,
                    roleAccess.CanCreate,
                    roleAccess.CanUpdate,
                    roleAccess.CanDelete
                };

            }
            else return "";

        }
        public static DateTime GetFromDate(int month, int year, int day = 1)
        {
            return new DateTime(year, month, day);

        }
        public static DateTime GetToDate(int month, int year, int? day = null)
        {
            if (!day.HasValue)
                day = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, day.Value);

        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            words = textInfo.ToTitleCase(words);
            return words;
        }
        public static string WrapToTootipIfRequired(string input, int length)
        {
            if (length >= input.Length)
            {
                return input;
            }
            else
            {
                var shortText = input.Substring(input.Length - length);
                return "<span class='cursor-pointer' title='" + input + "' data-toggle='tooltip'>" + shortText + "..." + "<span>";
            }
        }
        public static string CleanFileName(string fileName)
        {

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }
            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, @"\s+", " ");
            fileName = fileName.Replace(" ", "_");
            fileName = fileName.Replace("&", "_");
            return fileName;
        }

        public static bool ShowCRMRegion()
        {
            List<CRMUserType> types = new List<CRMUserType>() { CRMUserType.Admin, CRMUserType.CEO, CRMUserType.DivisionalHead };
            if (types.Contains(SiteContext.Current.UserTypeId))
                return true;
            else return false;
        }

        public static bool ShowCRMSalePersons()
        {
            List<CRMUserType> types = new List<CRMUserType>() { CRMUserType.Admin, CRMUserType.CEO, CRMUserType.DivisionalHead, CRMUserType.RSM };
            if (types.Contains(SiteContext.Current.UserTypeId))
                return true;
            else return false;
        }
        public static bool ShowCRMDivision()
        {
            List<CRMUserType> types = new List<CRMUserType>() { CRMUserType.Admin, CRMUserType.CEO, CRMUserType.RSM, CRMUserType.SalesExecutive };
            if (types.Contains(SiteContext.Current.UserTypeId))
                return true;
            else return false;
        }
        /// <summary>
        ///  use state to prevent CSRF
        /// </summary>
        /// <remarks>When your application exchanges the authorization code for an access token, 
        /// you want to be sure that the OAuth flow which resulted in the authorization code provided was actually initiated by the legitimate user. So,
        ///  before the client application kicks off the OAuth flow by redirecting the user to the provider,
        ///  the client application creates a random state value and typically store it in a server-side session.
        ///  Then, as the user completes the OAuth flow, you check to make sure state value matches the value stored in the user's server-side session-- 
        /// as that indicates the user had initiated the OAuth flow.
        /// </remarks>
        /// <returns></returns>
        public static string GenerateSatate()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            var random = new Random();
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(random.Next(123400, 9999999).ToString()));
        }
        public static DateTime NewDateWithOldTime(this DateTime datetime, DateTime newDate)
        {
            return newDate.Date + datetime.TimeOfDay;
        }
        public static string GetClientIpAddress(HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!isPrivateIP(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }
        private static bool isPrivateIP(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }
    }

}
