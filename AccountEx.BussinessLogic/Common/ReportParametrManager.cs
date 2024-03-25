using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.Common
{
   public static class ReportParametrManager
    {

        public static List<SqlParameter> GetFiscalANDCompanyIdParametrs()
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@COMPANYID", SiteContext.Current.User.CompanyId));
            param.Add(new SqlParameter("@FISCALID", SiteContext.Current.Fiscal.Id));
            return param;
        }
        public static SqlParameter GetCompanyIdParameter()
        {

            return new SqlParameter("@COMPANYID", SiteContext.Current.User.CompanyId);

        }
        public static SqlParameter GetFromDate(DateTime date)
        {

            return new SqlParameter("@FROMDATE", date);

        }
        public static SqlParameter GetFromDate1(DateTime date)
        {

            return new SqlParameter("@FROMDATE1", date);

        }
        public static SqlParameter GetToDate(DateTime date)
        {

            return new SqlParameter("@TODATE", date);


        }
        public static SqlParameter GetToDate1(DateTime date)
        {

            return new SqlParameter("@TODATE1", date);


        }
        public static SqlParameter GetCustomerIds(string Ids)
        {

            return new SqlParameter("@CRMCustomerIds", Ids);


        }
        public static SqlParameter GetVendorIds(string Ids)
        {

            return new SqlParameter("@VendorIds", Ids);


        }
        public static SqlParameter GetProjectId(int Ids)
        {

            return new SqlParameter("@ProjectId", Ids);


        }
        public static SqlParameter GetDeliveryTypeParameter(byte deliveryType)
        {

            return new SqlParameter("@DeliveryType", deliveryType);


        }
        public static SqlParameter GetProductIds(string Ids)
        {

            return new SqlParameter("@ItemIds", Ids);


        }
        public static SqlParameter GetActualProductIds(string Ids)
        {

            return new SqlParameter("@ActualProductIds", Ids);


        }
        public static SqlParameter GetYears(string years)
        {

            return new SqlParameter("@Years", years);


        }
        public static SqlParameter GetYear(int year)
        {

            return new SqlParameter("@Year", year);


        }
        public static SqlParameter GetMonth(int month)
        {

            return new SqlParameter("@Month", month);


        }
        public static SqlParameter GetCategoryId(int CategoryId)
        {

            return new SqlParameter("@CategoryId", CategoryId);


        }
        public static SqlParameter GetSecCategoryId(int SecCategoryId)
        {

            return new SqlParameter("@SecCategoryId", SecCategoryId);


        }
        public static SqlParameter GetDivisionIds(string divisionIds)
        {

            return new SqlParameter("@DivisionIds", divisionIds);


        }
        public static SqlParameter GetGroupId(int groupId)
        {

            return new SqlParameter("@GroupId", groupId);


        }
        public static SqlParameter GetExcludeOwnProduct(int excludeOwnProduct)
        {

            return new SqlParameter("@ExcludeOwnProduct", excludeOwnProduct);


        }
        public static SqlParameter GetReportType(int reportType)
        {

            return new SqlParameter("@ReportType", reportType);


        }
        public static SqlParameter GetRecordCount(int recordCount)
        {

            return new SqlParameter("@RecordCount", recordCount);


        }

        public static SqlParameter GetIndustryTypeId(int industryTypeId)
        {

            return new SqlParameter("@IndustryTypeId ", industryTypeId);


        }

        public static SqlParameter GetSubGroupId(int subGroupId)
        {

            return new SqlParameter("@SubGroupId", subGroupId);


        }

        public static SqlParameter GetSalePersonIds(string salePersonIds)
        {

            return new SqlParameter("@SalePersonUserIds", salePersonIds);


        }
        public static SqlParameter GetSalePersonId(int salePersonId)
        {

            return new SqlParameter("@SalePersonId", salePersonId);


        }
        public static SqlParameter GetIncludeStockTransfer(int IncludeStockTransfer)
        {

            return new SqlParameter("@IncludeStockTransfer", IncludeStockTransfer);


        }
        public static SqlParameter GetRegionIds(string regionIds)
        {

            return new SqlParameter("@RegionIds", regionIds);


        }
        public static SqlParameter GetCurrencyIds(string currencyIds)
        {

            return new SqlParameter("@CurrencyIds", currencyIds);


        }
        public static SqlParameter GetDeliveryTypeIds(string deliveryTypeIds)
        {

            return new SqlParameter("@DeliveryTypeIds", deliveryTypeIds);


        }
        public static SqlParameter GetSaleTypeIds(string saleTypeIds)
        {

            return new SqlParameter("@SaleTypeIds", saleTypeIds);


        }
        public static List<SqlParameter> GetFiscalIdParameter()
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@FISCALID", SiteContext.Current.Fiscal.Id));
            return param;
        }
        public static SqlParameter GetUserIdParameter()
        {
            return GetUserIdParameter("@UserId");

        }
        public static SqlParameter GetUserIdParameter(string name)
        {

            return new SqlParameter(name, SiteContext.Current.User.Id);
        }

    }
}
