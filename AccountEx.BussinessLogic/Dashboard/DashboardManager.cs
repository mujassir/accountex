using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.DbMapping;

namespace AccountEx.BussinessLogic
{
    public static class DashboardManager
    {
        public static Dashboard GetDashboard(string name, DateTime lastLoadTime)
        {
            var repo = new DashboardRepository();
            var roleRepor = new RoleRepository();
            var wRepo = new WidgetRepository();
            var widgetColumnRepo = new WidgetColumnRepository();

            var dashboard = new Dashboard();
            if (!string.IsNullOrWhiteSpace(name))
                dashboard = repo.GetByName(name);
            else
            {
                var dashboarId = GetDashBoardIdByRoleIds();
                dashboard = repo.GetDashboardById(dashboarId);
            }

            dashboard.Widgets = wRepo.GetByDashboard(SiteContext.Current.User.CompanyId, dashboard.Id, lastLoadTime);
            foreach (var item in dashboard.Widgets)
            {
                item.WidgetColumns = widgetColumnRepo.GetColumn(item.Id);
            }
            //dashboard.Widgets = GetWidgets(dashboard.Id);
            //dashboard.Widgets = new WidgetRepository().GetByDashboard(dashboard.Id);

            return dashboard;
        }
        public static List<IdName> GetAvailableDashBoard()
        {
            return new CompanyDashboardRepository().GetAvailableDashBoard();
        }
        public static int GetDashBoardIdByRoleIds()
        {
            var roleRepor = new RoleRepository();
            var roleIds = SiteContext.Current.UserRoles.ToList();
            var dashboarId = roleRepor.GetDashBoradId(roleIds);
            return dashboarId;
        }

        public static DataTable GetReportData(WidgetObject ro)
        {
            var report = new WidgetRepository().GetById(ro.Id);
            return new SqlRepository().GetDataTable(report.Location, GetParameters(ro.Id, ro.Parameters));
        }

        private static List<SqlParameter> GetParameters(int reportId, List<WidgetParameter> parameters)
        {
            var paramValues = parameters.ToDictionary(p => p.Name, q => q.Value);
            var widgetParams = new WidgetRepository().GetParameters(reportId);
            var list = new List<SqlParameter>();
            foreach (var item in widgetParams)
            {
                if (item.AdvanceType == (byte)ReportParameterType.Simple)
                {
                    if (!paramValues.ContainsKey(item.Name))  throw new OwnException(string.Format("Parameter: {0} is missing", item.Name));
                    var value = paramValues[item.Name];
                    if (item.Type == SqlDbType.Date.ToString())
                    {
                        DateTime dt;
                        DateTime.TryParseExact(paramValues[item.Name], AppSetting.DateFormat, new CultureInfo("en-US"), DateTimeStyles.None, out dt);
                        list.Add(new SqlParameter(item.Name, dt) { });
                    }
                    else
                        list.Add(new SqlParameter(item.Name, paramValues[item.Name]) { });
                }
                else if (item.AdvanceType == (byte)ReportParameterType.Session)
                {
                    var value = SiteContext.Current.User.CompanyId;
                    list.Add(new SqlParameter(item.Name, value) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.FiscalId)
                {
                    var value = SiteContext.Current.Fiscal.Id;
                    list.Add(new SqlParameter(item.Name, value) { });

                }

                else if (item.AdvanceType == (byte)ReportParameterType.DefaultValue)
                {
                    list.Add(new SqlParameter(item.Name, item.Value) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.FiscalYearStartDate)
                {
                    var date = FiscalYearManager.GetStartDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.FiscalYearEndDate)
                {
                    var date = FiscalYearManager.GetEndDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.MonthStartDate)
                {
                    var date = FiscalYearManager.GetCurrentMonthStartDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.MonthEndDate)
                {
                    var date = FiscalYearManager.GetCurrentMonthEndDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.TodayDate)
                {
                    var date = DateTime.Now.Date;
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.IdType)
                {
                    var idTypeList = item.Value.Split(',').Select(int.Parse).ToList().ToIdTypeDataTable();
                    list.Add(new SqlParameter(item.Name, idTypeList) { SqlDbType = SqlDbType.Structured });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.LoggedInUserId)
                {
                    var value = SiteContext.Current.User.Id;
                    list.Add(new SqlParameter(item.Name, value) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.CurenntFiscalWithPrevious)
                {
                    var fiscals = new List<int>() { SiteContext.Current.Fiscal.Id };
                    var prviousFiscal = new FiscalRepository().GetPreviousFiscal(SiteContext.Current.Fiscal.FromDate, SiteContext.Current.User.CompanyId);
                    if (prviousFiscal != null)
                        fiscals.Add(prviousFiscal.Id);

                    var value = string.Join(",", fiscals);
                    list.Add(new SqlParameter(item.Name, value) { });

                }

            }
            return list;
        }


        public static List<Widget> GetWidgets(int dashboardId)
        {
            var widgets = new List<Widget>();
            var widgetRepo = new WidgetRepository();
            var wCollection = widgetRepo.GetByDashboard(dashboardId);
            foreach (var report in wCollection)
            {
                if (!report.IsExternal && !report.IsExecuted)
                {
                    var sqlParams = new SqlRepository().GetParams(report.Location);
                    var sn = 0;
                    foreach (SqlParameter param in sqlParams)
                    {
                        if (param.ParameterName == "@RETURN_VALUE") continue;
                        var wParam = new WidgetParameter()
                        {
                            WidgetId = report.Id,
                            Name = param.ParameterName,
                            Type = param.SqlDbType.ToString(),
                            ControlType = "text",
                            IsVisible = GetIsVisible(param.ParameterName),
                            AdvanceType = GetAdvanceType(param.ParameterName),
                            SequenceNumber = ++sn,
                            DisplayLabel = GetDisplayLabel(param.ParameterName),
                            InplutCssClass = GetInplutCssClass(param),
                        };
                        report.WidgetParameters.Add(wParam);
                    }
                    report.IsExecuted = true;
                    widgetRepo.Save(report);
                }
                widgets.Add(report);
            }
            return widgets;
        }

        private static string GetDisplayLabel(string label)
        {
            return System.Text.RegularExpressions.Regex.Replace(label.Replace("@", ""), "(\\B[A-Z])", " $1");
        }
        private static bool GetIsVisible(string name)
        {
            name = name.Replace("@", "");
            var data = new List<string> { "VoucherType", "CompanyId", "UserId", "AccountDetailFormId" };
            return !data.Contains(name);
        }

        private static byte GetAdvanceType(string name)
        {
            name = name.Replace("@", "");
            var defaultParameters = new List<string> { "VoucherType", "AccountDetailFormId" };
            var sessionParameters = new List<string> { "CompanyId", "UserId" };
            if (defaultParameters.Contains(name))
                return (byte)ReportParameterType.DefaultValue;
            else if (sessionParameters.Contains(name))
                return (byte)ReportParameterType.Session;
            else
                return (byte)ReportParameterType.Simple;
        }
        private static string GetInplutCssClass(SqlParameter param)
        {
            var InplutCssClass = "";
            switch (param.SqlDbType)
            {
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    InplutCssClass = "date-picker";
                    break;
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    InplutCssClass = "number";
                    break;
            }
            return InplutCssClass;
        }
    }
}
