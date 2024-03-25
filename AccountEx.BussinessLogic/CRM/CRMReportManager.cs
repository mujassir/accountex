using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using AccountEx.DbMapping;

namespace AccountEx.BussinessLogic
{
    public static class CRMReportManager
    {

        public static DataSet GetSaleSummaryByDate(DateTime fromdate, DateTime todate, string deliveryTypeIds, string saleTypeIds, string customerIds, string productIds, string regionIds, string divisionIds, int groupId, int subGroupId, string salePersonIds, int customerType, int invoiceType, string industryTypeIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetDeliveryTypeIds(deliveryTypeIds));
            parameters.Add(GetSaleTypeIds(saleTypeIds));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetCustomerType(customerType));
            parameters.Add(GetIndustryTypeIds(industryTypeIds));
            parameters.Add(GetInvoiceTypeId(invoiceType));
            return GetReportData("[DBO].[CRM_GetDateWiseSummaryDetail]", parameters);
        }
        public static DataSet SaleSummaryByCustomerProduct(DateTime fromdate, DateTime todate, string deliveryTypeIds, string saleTypeIds, string customerIds, string productIds, string regionIds, string divisionIds, int groupId, int subGroupId, string salePersonIds, string industryTypeIds, int customerType, int invoiceType)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetDeliveryTypeIds(deliveryTypeIds));
            parameters.Add(GetSaleTypeIds(saleTypeIds));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetIndustryTypeIds(industryTypeIds));
            parameters.Add(GetCustomerType(customerType));
            parameters.Add(GetInvoiceTypeId(invoiceType));
            return GetReportData("[DBO].[CRM_SaleSummaryByCustomerProduct]", parameters);
        }





        public static DataSet GetSaleSummaryProductsWise(DateTime fromdate, DateTime todate, string customerIds, string productIds, string regionIds, string divisionIds, int groupId, int subGroupId, string industryTypeIds, int customerType)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetIndustryTypeIds(industryTypeIds));
            parameters.Add(GetCustomerType(customerType));
            return GetReportData("[DBO].[CRM_GetProductWiseSaleSummaryDetail]", parameters);
        }
        public static DataSet GetCustomerAndDivisionWiseSaleSummaryDetail(string customerIds, string salePersonIds, int year, string regionIds, string divisionIds, int customerType)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetCustomerType(customerType));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_GetCustomerAndDivisionWiseSaleSummaryDetail]", parameters);
        }
        public static DataSet SaleForecastByProductCustomer(int year, int month, string customerIds, string salePersonIds, string regionIds, string divisionIds, int categoryId, int secCategoryId, int groupId, int subGroupId, string productIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetMonth(month));
            parameters.Add(GetCategoryId(categoryId));
            parameters.Add(GetSecCategoryId(secCategoryId));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetProductIds(productIds));
            return GetReportData("[DBO].[GetCRMSaleForecastByProductCustomer]", parameters);
        }
        public static DataSet SaleForecastSummaryRegionWise(int year, int month, string customerIds, string salePersonIds, string regionIds, string divisionIds, int categoryId, int secCategoryId,
int groupId, int subGroupId, string productIds)
        {

            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetMonth(month));
            parameters.Add(GetCategoryId(categoryId));
            parameters.Add(GetSecCategoryId(secCategoryId));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetProductIds(productIds));

            return GetReportData("[DBO].[GetCRMSaleForecastSummaryRegionWise]", parameters);
        }


        public static DataSet GetSalePersonWiseSaleSummaryDetail(DateTime fromdate, DateTime todate, string regionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetRegionIds(regionIds));
            return GetReportData("[DBO].[CRM_GetSalePersonWiseSaleSummaryDetail]", parameters);
        }
        public static DataSet GetYearWiseMonthlySaleComparison(string yearIds, string regionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetYears(yearIds));
            parameters.Add(GetRegionIds(regionIds));
            return GetReportData("[DBO].[CRM_YearWiseMonthlySaleComparison]", parameters);
        }
        public static DataSet GetDivisionWiseMonthlySaleComparison(string divisionIds, int year)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetYear(year));
            return GetReportData("[DBO].[CRM_DivisionWiseMonthlySaleComparison]", parameters);
        }
        public static DataSet GetSalePersonWiseMonthlySaleComparison(int year, string salePersonIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            return GetReportData("[DBO].[CRM_SalePersonWiseMonthlySaleComparison]", parameters);
        }
        public static DataSet GetRegionAndDivisionWiseMonthlySaleComparison(int year, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetYear(year));
            return GetReportData("[DBO].[CRM_RegionNDivisionWiseMonthlySaleComparison]", parameters);
        }

        public static DataSet GetCustomerAndDivisionWiseYearlySaleComparison(string customerIds, string salePersonIds, string yearIds, string regionIds, string divisionIds, string deliveryTypeIds, int customerType)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYears(yearIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetDeliveryTypeIds(deliveryTypeIds));
            parameters.Add(GetCustomerType(customerType));
            return GetReportData("[DBO].[CRM_CustomerNDivisionWiseYearlySaleComparison]", parameters);
        }
        public static DataSet GetCustomerWisePotential(int year, string customerIds, string regionIds, string salePersonIds, string currencyIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetYear(year));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetCurrencyIds(currencyIds));
            return GetReportData("[DBO].[CRM_CustomerWisePotential]", parameters);
        }
        public static DataSet GetRegionWisePotential(int year)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetYear(year));
            return GetReportData("[DBO].[CRM_RegionWisePotential]", parameters);
        }
        public static DataSet GetSalePersonWisePotential(string salePersonIds, int year, string regionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetYear(year));
            return GetReportData("[DBO].[CRM_SalePersonWisePotential]", parameters);
        }
        public static DataSet GetProductWisePotential(string productIds, int year, string salePersonIds, string regionIds, string customerIds, string divisionIds, string vendorIds, int groupId, int subGroupId, string industryTypeIds, int excludeOwnProduct, string currencyIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetIndustryTypeIds(industryTypeIds));
            parameters.Add(GetVendorIds(vendorIds));
            parameters.Add(GetExcludeOwnProduct(excludeOwnProduct));
            parameters.Add(GetCurrencyIds(currencyIds));
            return GetReportData("[DBO].[CRM_ProductWisePotential]", parameters);
        }
        public static DataSet GetOrganizationNDivisionWiseProjectDetail(string customerIds, string salePersonIds, int year, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_OrganizationNDivisionWiseProjectDetail]", parameters);
        }
        public static DataSet GetSalePersonNDivisionWiseProjectDetail(string salePersonIds, int year, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_SalePersonNDivisionWiseProjectDetail]", parameters);
        }

        public static DataSet GetRegionNDivisionWiseProjectDetail(int year, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetYear(year));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_RegionNDivisionWiseProjectDetail]", parameters);
        }

        public static DataSet GetProjectDetail(string customerIds, string salePersonIds, int year, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_DetailListOfProjects]", parameters);
        }
        public static DataSet ProjectDetailProductWise(string productIds, string actaulProductIds, int year, string regionIds, string divisionIds, int groupId, int subGroupId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetActualProductIds(actaulProductIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            return GetReportData("[DBO].[CRM_ProductWiseProjects]", parameters);
        }

        public static DataSet GetVisitDetailByDate(DateTime fromdate, DateTime todate)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            return GetReportData("[DBO].[CRM_DailyVisitsNDateWise]", parameters);
        }
        public static DataSet GetVisitDetailBySalePerson(DateTime fromdate, DateTime todate, string salePersonIds, int projectId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetProjectId(projectId));
            return GetReportData("[DBO].[CRM_SalePersonWiseVisitDetail]", parameters);
        }
        public static DataSet GetVisitDetailByCustomer(DateTime fromdate, DateTime todate, string customerIds, int projectId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetProjectId(projectId));
            return GetReportData("[DBO].[CRM_CustomerWiseVisitDetail]", parameters);
        }
        public static DataSet GetVisitCountByCustomer(DateTime fromdate, DateTime todate, string customerIds, int projectId, string salePersonIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetProjectId(projectId));
            parameters.Add(GetSalePersonIds(salePersonIds));
            return GetReportData("[DBO].[CRM_CustomerWiseNoOfVisits]", parameters);
        }
        public static DataSet GetCustomerWiseMonthlySale(int year, string customerIds, string salePersonIds, string regionIds, byte deliveryType, string divisionIds, int industryTypeId, int customerType)
        {

            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetCustomerIds(customerIds));
            parameters.Add(GetSalePersonIds(salePersonIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDeliveryTypeParameter(deliveryType));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetYear(year));
            parameters.Add(GetIndustryTypeId(industryTypeId));
            parameters.Add(GetCustomerType(customerType));
            return GetReportData("[DBO].[CRM_CustomerWiseMonthlySale]", parameters);
        }
        public static DataSet GetProductWiseMonthlySale(DateTime fromdate, DateTime todate, string deliveryTypeIds, string saleTypeIds, string customerIds, string productIds, string regionIds, string divisionIds, int groupId, int subGroupId, string salePersonIds, int customerType, int invoiceType, string industryTypeIds,int reportType)
        {

            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetDeliveryTypeIds(deliveryTypeIds));
            parameters.Add(GetProductIds(productIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetIndustryTypeIds(industryTypeIds));
            parameters.Add(GetInvoiceTypeId(invoiceType));
            parameters.Add(GetReportType(reportType));
            return GetReportData("[DBO].[CRM_ProductWiseMonthlySale]", parameters);
        }
        public static DataSet GetInvoicePerformaDateWise(DateTime fromdate, DateTime todate)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            return GetReportData("[DBO].[CRM_DateWiseProformaInvoice]", parameters);
        }

        public static DataSet GetTestingResults(string productIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetCompanyIdParameter());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetProductIds(productIds));
            return GetReportData("[DBO].[CRM_TestingResult]", parameters);
        }
        public static DataSet GetSaleComparisonByYear(DateTime fromdate, DateTime todate, DateTime fromdate1, DateTime todate1, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetFromDate1(fromdate1));
            parameters.Add(GetToDate1(todate1));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_GetSaleComparisonByYear]", parameters);
        }
        public static DataSet GetSaleComparisonByYearBySaleType(DateTime fromdate, DateTime todate, DateTime fromdate1, DateTime todate1, string regionIds, string divisionIds)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetFromDate(fromdate));
            parameters.Add(GetToDate(todate));
            parameters.Add(GetFromDate1(fromdate1));
            parameters.Add(GetToDate1(todate1));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetDivisionIds(divisionIds));
            return GetReportData("[DBO].[CRM_GetSaleComparisonByYearBySaleType]", parameters);
        }
        public static DataSet GetMarketShareSituation(int year, string regionIds, string divisionIds, string deliveryTypeIds, int reportType, int groupId, int subGroupId, int recordCount)
        {
            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetFiscalANDCompanyIdParametrs());
            parameters.Add(GetUserIdParameter());
            parameters.Add(GetYear(year));
            parameters.Add(GetDivisionIds(divisionIds));
            parameters.Add(GetDeliveryTypeIds(deliveryTypeIds));
            parameters.Add(GetRegionIds(regionIds));
            parameters.Add(GetGroupId(groupId));
            parameters.Add(GetSubGroupId(subGroupId));
            parameters.Add(GetReportType(reportType));
            parameters.Add(GetRecordCount(recordCount));
            return GetReportData("[DBO].[CRM_GetMarketShareSituation]", parameters);
        }










        public static DataSet GetReportData(string reportName, List<SqlParameter> parameters)
        {
            return new SqlRepository().GetDataSet(reportName, parameters);
        }

        private static List<SqlParameter> GetFiscalANDCompanyIdParametrs()
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@COMPANYID", SiteContext.Current.User.CompanyId));
            param.Add(new SqlParameter("@FISCALID", SiteContext.Current.Fiscal.Id));
            return param;
        }
        private static SqlParameter GetCompanyIdParameter()
        {

            return new SqlParameter("@COMPANYID", SiteContext.Current.User.CompanyId);

        }
        private static SqlParameter GetFromDate(DateTime date)
        {

            return new SqlParameter("@FROMDATE", date);

        }
        private static SqlParameter GetFromDate1(DateTime date)
        {

            return new SqlParameter("@FROMDATE1", date);

        }
        private static SqlParameter GetToDate(DateTime date)
        {

            return new SqlParameter("@TODATE", date);


        }
        private static SqlParameter GetToDate1(DateTime date)
        {

            return new SqlParameter("@TODATE1", date);


        }
        private static SqlParameter GetCustomerIds(string Ids)
        {

            return new SqlParameter("@CRMCustomerIds", Ids);


        }
        private static SqlParameter GetVendorIds(string Ids)
        {

            return new SqlParameter("@VendorIds", Ids);


        }
        private static SqlParameter GetProjectId(int Ids)
        {

            return new SqlParameter("@ProjectId", Ids);


        }
        private static SqlParameter GetDeliveryTypeParameter(byte deliveryType)
        {

            return new SqlParameter("@DeliveryType", deliveryType);


        }
        private static SqlParameter GetProductIds(string Ids)
        {

            return new SqlParameter("@ItemIds", Ids);


        }
        private static SqlParameter GetActualProductIds(string Ids)
        {

            return new SqlParameter("@ActualProductIds", Ids);


        }
        private static SqlParameter GetYears(string years)
        {

            return new SqlParameter("@Years", years);


        }
        private static SqlParameter GetYear(int year)
        {

            return new SqlParameter("@Year", year);


        }
        private static SqlParameter GetCustomerType(int customerType)
        {

            return new SqlParameter("@CustomerType", customerType);


        }
        private static SqlParameter GetMonth(int month)
        {

            return new SqlParameter("@Month", month);


        }
        private static SqlParameter GetCategoryId(int CategoryId)
        {

            return new SqlParameter("@CategoryId", CategoryId);


        }
        private static SqlParameter GetSecCategoryId(int SecCategoryId)
        {

            return new SqlParameter("@SecCategoryId", SecCategoryId);


        }
        private static SqlParameter GetDivisionIds(string divisionIds)
        {

            return new SqlParameter("@DivisionIds", divisionIds);


        }
        private static SqlParameter GetGroupId(int groupId)
        {

            return new SqlParameter("@GroupId", groupId);


        }
        private static SqlParameter GetExcludeOwnProduct(int excludeOwnProduct)
        {

            return new SqlParameter("@ExcludeOwnProduct", excludeOwnProduct);


        }
        private static SqlParameter GetReportType(int reportType)
        {

            return new SqlParameter("@ReportType", reportType);


        }
        private static SqlParameter GetRecordCount(int recordCount)
        {

            return new SqlParameter("@RecordCount", recordCount);


        }

        private static SqlParameter GetIndustryTypeId(int industryTypeId)
        {

            return new SqlParameter("@IndustryTypeId ", industryTypeId);


        }
        private static SqlParameter GetInvoiceTypeId(int invoiceTypeId)
        {

            return new SqlParameter("@InvoiceTypeId ", invoiceTypeId);


        }

        private static SqlParameter GetSubGroupId(int subGroupId)
        {

            return new SqlParameter("@SubGroupId", subGroupId);


        }

        private static SqlParameter GetSalePersonIds(string salePersonIds)
        {

            return new SqlParameter("@SalePersonUserIds", salePersonIds);


        }
        private static SqlParameter GetIndustryTypeIds(string industryTypeIds)
        {

            return new SqlParameter("@IndustryTypeIds", industryTypeIds);


        }
        private static SqlParameter GetRegionIds(string regionIds)
        {

            return new SqlParameter("@RegionIds", regionIds);


        }
        private static SqlParameter GetCurrencyIds(string currencyIds)
        {

            return new SqlParameter("@CurrencyIds", currencyIds);


        }
        private static SqlParameter GetDeliveryTypeIds(string deliveryTypeIds)
        {

            return new SqlParameter("@DeliveryTypeIds", deliveryTypeIds);


        }
        private static SqlParameter GetSaleTypeIds(string saleTypeIds)
        {

            return new SqlParameter("@SaleTypeIds", saleTypeIds);


        }
        private static List<SqlParameter> GetFiscalIdParameter()
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@FISCALID", SiteContext.Current.Fiscal.Id));
            return param;
        }
        private static SqlParameter GetUserIdParameter()
        {
            return GetUserIdParameter("@UserId");

        }
        private static SqlParameter GetUserIdParameter(string name)
        {

            return new SqlParameter(name, SiteContext.Current.User.Id);
        }
    }

}
