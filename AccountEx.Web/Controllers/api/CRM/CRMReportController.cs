using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Text;
using SelectPdf;
using System.Web;
using System.IO;
using System.Drawing;
using AccountEx.CodeFirst;
using AccountEx.CodeFirst.Models;
using BussinessLogic;
using AccountEx.Repositories.Config;
using AccountEx.DbMapping;
using AccountEx.Repositories.Nexus;
using System.Collections.Specialized;

namespace AccountEx.Web.Controllers.api.Reports
{


    public class CRMReportController : BaseApiController
    {

        public virtual ApiResponse Post(string key)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {


                switch (key)
                {

                    case "SaleSummaryByDate":
                        response = SaleSummaryByDate();
                        break;
                    case "SaleSummaryByCustomerProduct":
                        response = SaleSummaryByCustomerProduct();
                        break;
                    case "SaleSummaryProductWise":
                        response = SaleSummaryProductWise();
                        break;
                    case "GetCustomerAndDivisionWiseSaleSummaryDetail":
                        response = GetCustomerAndDivisionWiseSaleSummaryDetail();
                        break;
                    case "SaleForecastByProductCustomer":
                        response = SaleForecastByProductCustomer();
                        break;
                    case "SaleForecastSummaryRegionWise":
                        response = SaleForecastSummaryRegionWise();
                        break;
                        
                    case "SalePersonWiseSaleSummaryDetail":
                        response = GetSalePersonWiseSaleSummaryDetail();
                        break;
                    case "YearWiseMonthlySaleComparison":
                        response = GetYearWiseMonthlySaleComparison();
                        break;
                    case "DivisionWiseMonthlySaleComparison":
                        response = GetDivisionWiseMonthlySaleComparison();
                        break;
                    case "SalePersonWiseMonthlySaleComparison":
                        response = GetSalePersonWiseMonthlySaleComparison();
                        break;
                    case "RegionAndDivisionWiseMonthlySaleComparison":
                        response = GetRegionAndDivisionWiseMonthlySaleComparison();
                        break;

                    case "CustomerAndDivisionWiseYearlySaleComparison":
                        response = GetCustomerAndDivisionWiseYearlySaleComparison();
                        break;
                    case "CustomerWisePotential":
                        response = GetCustomerWisePotential();
                        break;
                    case "RegionWisePotential":
                        response = GetRegionWisePotential();
                        break;

                    case "SalePersonWisePotential":
                        response = GetSalePersonWisePotential();
                        break;
                    case "ProductWisePotential":
                        response = GetProductWisePotential();
                        break;
                    case "OrganizationNDivisionWiseProjectDetail":
                        response = GetOrganizationNDivisionWiseProjectDetail();
                        break;
                    case "SalePersonNDivisionWiseProjectDetail":
                        response = GetSalePersonNDivisionWiseProjectDetail();
                        break;
                    case "RegionNDivisionWiseProjectDetail":
                        response = GetRegionNDivisionWiseProjectDetail();
                        break;
                    case "ProjectDetail":
                        response = GetProjectDetail();
                        break;

                    case "ProjectDetailProductWise":
                        response = GetProjectDetailProductWise();
                        break;
                    case "VisitDetailByDate":
                        response = GetVisitDetailByDate();
                        break;
                    case "VisitDetailBySalePerson":
                        response = GetVisitDetailBySalePerson();
                        break;
                    case "VisitDetailByCustomer":
                        response = GetVisitDetailByCustomer();
                        break;
                    case "VisitCountByCustomer":
                        response = GetVisitCountByCustomer();
                        break;
                    case "CustomerWiseMonthlySale":
                        response = GetCustomerWiseMonthlySale();
                        break;
                    case "ProductWiseMonthlySale":
                        response = GetProductWiseMonthlySale();
                        break;

                        
                    case "InvoicePerformaDateWise":
                        response = GetInvoicePerformaDateWise();
                        break;
                    case "TestingResults":
                        response = GetTestingResults();
                        break;
                    case "SaleComparisonByYear":
                        response = SaleComparisonByYear();
                        break;
                    case "SaleComparisonByYearByType":
                        response = SaleComparisonByYearByType();
                        break;
                    case "MarketShareSituation":
                        response = MarketShareSituation();
                        break;








                }

            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };


            }
            return response;
        }

        private ApiResponse SaleSummaryByDate()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var deliveryTypeIds = Request.GetFormValue("deliveryTypeIds");
            var saleTypeIds = Request.GetFormValue("saleTypeIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var customerIds = Request.GetFormValue("customerIds");
            var productIds = Request.GetFormValue("productIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));

            var industryTypeIds = Request.GetFormValue("industryTypeIds");
            var invoiceType = Numerics.GetInt(Request.GetQueryString("invoiceType"));

            var records = CRMReportManager.GetSaleSummaryByDate(fromDate, toDate, deliveryTypeIds, saleTypeIds, customerIds, productIds, regionIds, divisionIds, groupId, subGroupId,salePersonIds,customerType,invoiceType,industryTypeIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SaleSummaryByCustomerProduct()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var deliveryTypeIds = Request.GetFormValue("deliveryTypeIds");
            var saleTypeIds = Request.GetFormValue("saleTypeIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var customerIds = Request.GetFormValue("customerIds");
            var productIds = Request.GetFormValue("productIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var industryTypeIds = Request.GetFormValue("industryTypeIds");
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));
            var invoiceType = Numerics.GetInt(Request.GetQueryString("invoiceType"));
            var records = CRMReportManager.SaleSummaryByCustomerProduct(fromDate, toDate, deliveryTypeIds, saleTypeIds, customerIds, productIds, regionIds, divisionIds, groupId, subGroupId, salePersonIds,industryTypeIds,customerType, invoiceType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse MarketShareSituation()
        {

            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var reportType = Numerics.GetInt(Request.GetQueryString("reportType"));
            var recordCount = Numerics.GetInt(Request.GetQueryString("recordCount"));
            var deliveryTypeIds = Request.GetFormValue("deliveryTypeIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetMarketShareSituation(year, regionIds, divisionIds, deliveryTypeIds, reportType, groupId, subGroupId, recordCount);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SaleComparisonByYear()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));


            var fromDate1 = Convert.ToDateTime(Request.GetQueryString("fromDate1"));
            var toDate1 = Convert.ToDateTime(Request.GetQueryString("toDate1"));

            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetSaleComparisonByYear(fromDate, toDate, fromDate1, toDate1, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SaleComparisonByYearByType()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));


            var fromDate1 = Convert.ToDateTime(Request.GetQueryString("fromDate1"));
            var toDate1 = Convert.ToDateTime(Request.GetQueryString("toDate1"));

            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetSaleComparisonByYearBySaleType(fromDate, toDate, fromDate1, toDate1, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SaleSummaryProductWise()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var customerIds = Request.GetFormValue("customerIds");
            var productIds = Request.GetFormValue("productIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var industryTypeId = Numerics.GetInt(Request.GetQueryString("industryTypeId"));
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));
            var industryTypeIds = Request.GetFormValue("industryTypeIds");
            var records = CRMReportManager.GetSaleSummaryProductsWise(fromDate, toDate, customerIds, productIds, regionIds, divisionIds, groupId, subGroupId, industryTypeIds, customerType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetCustomerAndDivisionWiseSaleSummaryDetail()
        {

            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));
            var records = CRMReportManager.GetCustomerAndDivisionWiseSaleSummaryDetail(customerIds, salePersonIds, year, regionIds, divisionIds,customerType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;




        }
        private ApiResponse SaleForecastByProductCustomer()
        {

            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var categoryId = Numerics.GetInt(Request.GetQueryString("categoryId"));
            var secCategoryId = Numerics.GetInt(Request.GetQueryString("secCategoryId"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var productIds = Request.GetFormValue("productIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));

            var records = CRMReportManager.SaleForecastByProductCustomer(year,month,customerIds, salePersonIds, regionIds, divisionIds, categoryId, secCategoryId, groupId, subGroupId, productIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;




        }
        private ApiResponse SaleForecastSummaryRegionWise()
        {

            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var categoryId = Numerics.GetInt(Request.GetQueryString("categoryId"));
            var secCategoryId = Numerics.GetInt(Request.GetQueryString("secCategoryId"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var productIds = Request.GetFormValue("productIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));



            var records = CRMReportManager.SaleForecastSummaryRegionWise(year, month, customerIds, salePersonIds, regionIds, divisionIds, categoryId, secCategoryId, groupId, subGroupId, productIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;




        }


        
        private ApiResponse GetSalePersonWiseSaleSummaryDetail()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var regionIds = Request.GetFormValue("regionIds");
            var records = CRMReportManager.GetSalePersonWiseSaleSummaryDetail(fromDate, toDate, regionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetYearWiseMonthlySaleComparison()
        {
            //var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            //var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var YearIds = Request.GetFormValue("YearIds");
            var regionIds = Request.GetFormValue("regionIds");
            var records = CRMReportManager.GetYearWiseMonthlySaleComparison(YearIds, regionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetDivisionWiseMonthlySaleComparison()
        {
            var divisionIds = Request.GetFormValue("divisionIds");
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var records = CRMReportManager.GetDivisionWiseMonthlySaleComparison(divisionIds, year);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }


        private ApiResponse GetSalePersonWiseMonthlySaleComparison()
        {
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var records = CRMReportManager.GetSalePersonWiseMonthlySaleComparison(year,salePersonIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

        private ApiResponse GetRegionAndDivisionWiseMonthlySaleComparison()
        {
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var records = CRMReportManager.GetRegionAndDivisionWiseMonthlySaleComparison(year,regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetCustomerAndDivisionWiseYearlySaleComparison()
        {

            var yearIds = Request.GetFormValue("YearIds");
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var deliveryTypeIds = Request.GetFormValue("deliveryTypeIds");
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));
            var records = CRMReportManager.GetCustomerAndDivisionWiseYearlySaleComparison(customerIds, salePersonIds, yearIds, regionIds, divisionIds, deliveryTypeIds, customerType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

        private ApiResponse GetCustomerWisePotential()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var currencyIds = Request.GetFormValue("currencyIds");
            var records = CRMReportManager.GetCustomerWisePotential(year, customerIds, regionIds, salePersonIds, currencyIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetRegionWisePotential()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var records = CRMReportManager.GetRegionWisePotential(year);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetSalePersonWisePotential()
        {
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var records = CRMReportManager.GetSalePersonWisePotential(salePersonIds, year, regionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

        private ApiResponse GetProductWisePotential()
        {
            var productIds = Request.GetFormValue("productIds");
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var customerIds = Request.GetFormValue("customerIds");
            var vendorIds = Request.GetFormValue("vendorIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var industryTypeIds = Request.GetFormValue("industryTypeIds");
            var excludeOwnProduct = Numerics.GetInt(Request.GetQueryString("excludeOwnProduct"));
            var currencyIds = Request.GetFormValue("currencyIds");


            var records = CRMReportManager.GetProductWisePotential(productIds, year, salePersonIds, regionIds, customerIds, divisionIds, vendorIds, groupId, subGroupId, industryTypeIds, excludeOwnProduct, currencyIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

        private ApiResponse GetOrganizationNDivisionWiseProjectDetail()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetOrganizationNDivisionWiseProjectDetail(customerIds, salePersonIds, year, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetSalePersonNDivisionWiseProjectDetail()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetSalePersonNDivisionWiseProjectDetail(salePersonIds, year, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetRegionNDivisionWiseProjectDetail()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetRegionNDivisionWiseProjectDetail(year, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetProjectDetail()
        {

            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var records = CRMReportManager.GetProjectDetail(customerIds, salePersonIds, year, regionIds, divisionIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

        private ApiResponse GetProjectDetailProductWise()
        {
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var regionIds = Request.GetFormValue("regionIds");
            var productIds = Request.GetFormValue("productIds");
            var actualProductIds = Request.GetFormValue("actualProductIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var records = CRMReportManager.ProjectDetailProductWise(productIds, actualProductIds, year, regionIds,divisionIds,groupId,subGroupId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

















        private ApiResponse PendingSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetPendingSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SummaryOfDepartmentBillingByPatient()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetSummaryOfDepartmentBillingByPatient(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse BillingByPatient()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var departmentId = Numerics.GetInt(Request.GetQueryString("DepartmentId"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetBillingByPatient(fromDate, ToDate, departmentId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse ReferralSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            var repo = new NexusReportRepository();
            var records = repo.GetReferralSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse ReceivablesSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            if (toMonth > 0 && toYear > 0)
                ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);

            var repo = new NexusReportRepository();
            var records = repo.GetReceivablesSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse DetailBillPayment()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var departmentId = Numerics.GetInt(Request.GetQueryString("DepartmentId"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            if (toMonth > 0 && toYear > 0)
                ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);

            var repo = new NexusReportRepository();
            var records = repo.GetReceivablesSummaryByDepartment(fromDate, ToDate, departmentId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse MonthlyReceiptSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            var repo = new NexusReportRepository();
            var records = repo.GetMonthlyReceiptSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }



        private ApiResponse GetRepossessedStocks()
        {


            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            // var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleRepossessedStock(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetDeliveredStock()
        {

            var repo = new ReportRepository();
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var records = repo.GetVehicleDeliveredStock(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetGeneralLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var currencyId = Numerics.GetInt(Request.GetQueryString("currencyId"));
            var repo = new TransactionRepository();
            var openingBalance = repo.GetForexOpeningBalance(accountId, date1, 0);
            var recieptTypes = new List<VoucherType> { VoucherType.VCR, VoucherType.VBR, VoucherType.VSD, VoucherType.AdvanceReceipts, VoucherType.AuctionnerPayments, VoucherType.PenaltyPayments };
            var transactions = repo.GetForexTransactions(accountId, date1, date2, currencyId).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.VoucherNumber).ToList();
            var totalDebit = transactions.Sum(p => p.Debit);
            var totalCredit = transactions.Sum(p => p.Credit);
            var records = new List<GeneralLedgerEntry>();
            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.Debit - item.Credit;
                var entry = new GeneralLedgerEntry(item);
                entry.Balance = Numerics.DecimalToString(runningTotal);
                records.Add(entry);
            }

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = records, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit + openingBalance),
                }
            };
            return response;

        }

        private ApiResponse GetSoldStockAnalysis()
        {

            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleSoldStockAnalysis(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetCashBankSummary()
        {
            var todate = Convert.ToDateTime(Request.GetQueryString("date1"));

            var repo = new ReportRepository();
            var records = repo.GetCashBankSumamry(todate);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetProfitLoss()
        {

            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var isBeforeClosing = Convert.ToBoolean(QueryString["isBeforeClosing"]);
            // var openingstock = Numerics.GetInt(Request.GetQueryString("OpeningStock"));
            // var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            var profitLoss = ReportManager.GetProfitLoss(date1, date2, isBeforeClosing);
            var totalprofit = profitLoss.Where(p => p.AccountType == "Profit").Sum(p => p.Accounts.Sum(q => q.Amount));
            var totalexpense = profitLoss.Where(p => p.AccountType == "Expense").Sum(p => p.Accounts.Sum(q => q.Amount));
            var totalnetamount = totalprofit - totalexpense;
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {

                    Profits = profitLoss.Where(p => p.AccountType == "Profit").Select(q => q.Accounts).ToList(),
                    Expenses = profitLoss.Where(p => p.AccountType == "Expense").Select(q => q.Accounts).ToList(),
                    TotalProfit = totalprofit,
                    TotalExpense = totalexpense,
                    TotalNetAmount = totalnetamount,
                }
            };
            return response;

        }
        private ApiResponse GetVehicleCustomerBalances()
        {

            var todate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleCustomerBalance(todate, branchId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetVehicleIncomeStatment()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var record = repo.GetVehicleIncomeStatment(fromdate, todate, branchId);
            var expenses = repo.GetVehicleExpensesForIncomeStatment(fromdate, todate, branchId);





            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = record,
                    Expenses = expenses
                }
            };
            return response;

        }
        private ApiResponse GetVehicleMonthlyCredits()
        {
            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var isBadDebit = Numerics.GetBool(Request.GetQueryString("isBadDebit"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleMonthlyCredits(month, year, branchId, isBadDebit);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehiclePeriodicActivity()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new ReportRepository();
            var records = repo.GetVehiclePeriodicActivity(fromdate, todate, accountId, branchId);



            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleRepossessions()
        {
            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var recoveryStatus = Numerics.GetInt(Request.GetQueryString("recoveryStatus"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleRepossessions(branchId, recoveryStatus);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleSales()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));

            var repo = new ReportRepository();
            var records = repo.GetVehicleSales(fromdate, todate, branchId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetCustomerCollections()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));

            var repo = new ReportRepository();
            var records = repo.GetCustomerCollections(fromdate, todate, branchId, accountId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetVehicleOverDueAmounts()
        {
            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var isBadDebit = Numerics.GetBool(Request.GetQueryString("isBadDebit"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleOverDueAmounts(fromdate, todate, branchId, isBadDebit);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleFollowups()
        {

            var fromdate = DateConverter.ConvertFromDmy(Request.GetQueryString("date1"));

            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var type = String.Format(Request.GetQueryString("type"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleFollowups(fromdate, branchId, type);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetSoldStockAnalysisByDate()
        {

            var fromdate = DateConverter.ConvertFromDmy(Request.GetQueryString("date1"));
            var todate = DateConverter.ConvertFromDmy(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleSoldStockAnalysisByDates(fromdate, todate, branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse GetVisitDetailByDate()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var records = CRMReportManager.GetVisitDetailByDate(fromDate, toDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVisitDetailBySalePerson()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var projectId = Numerics.GetInt(Request.GetFormValue("projectId"));
            var records = CRMReportManager.GetVisitDetailBySalePerson(fromDate, toDate, salePersonIds, projectId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVisitDetailByCustomer()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var customerIds = Request.GetFormValue("customerIds");
            var projectId = Numerics.GetInt(Request.GetFormValue("projectId"));
            var records = CRMReportManager.GetVisitDetailByCustomer(fromDate, toDate, customerIds, projectId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVisitCountByCustomer()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var projectId = Numerics.GetInt(Request.GetFormValue("projectId"));
            var records = CRMReportManager.GetVisitCountByCustomer(fromDate, toDate, customerIds, projectId,salePersonIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetCustomerWiseMonthlySale()
        {



            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var divisionIds = Request.GetFormValue("divisionIds");
            var customerIds = Request.GetFormValue("customerIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var industryTypeId = Numerics.GetInt(Request.GetQueryString("industryTypeId"));
            var deliveryType = Numerics.GetByte(Request.GetQueryString("deliveryType"));
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));
            var records = CRMReportManager.GetCustomerWiseMonthlySale(year, customerIds, salePersonIds, regionIds, deliveryType, divisionIds,industryTypeId, customerType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetProductWiseMonthlySale()
        {




            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var deliveryTypeIds = Request.GetFormValue("deliveryTypeIds");
            var saleTypeIds = Request.GetFormValue("saleTypeIds");
            var salePersonIds = Request.GetFormValue("salePersonIds");
            var regionIds = Request.GetFormValue("regionIds");
            var divisionIds = Request.GetFormValue("divisionIds");
            var customerIds = Request.GetFormValue("customerIds");
            var productIds = Request.GetFormValue("productIds");
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var subGroupId = Numerics.GetInt(Request.GetQueryString("subGroupId"));
            var customerType = Numerics.GetInt(Request.GetQueryString("customerType"));

            var industryTypeIds = Request.GetFormValue("industryTypeIds");
            var invoiceType = Numerics.GetInt(Request.GetQueryString("invoiceType"));
            var reportType = Numerics.GetInt(Request.GetQueryString("reportType"));


            var records = CRMReportManager.GetProductWiseMonthlySale(fromDate, toDate, deliveryTypeIds, saleTypeIds, customerIds, productIds, regionIds, divisionIds, groupId, subGroupId, salePersonIds, customerType, invoiceType, industryTypeIds,reportType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetInvoicePerformaDateWise()
        {

            var fromDate = Convert.ToDateTime(Request.GetQueryString("fromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("toDate"));
            var records = CRMReportManager.GetInvoicePerformaDateWise(fromDate, toDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetTestingResults()
        {

            var productIds = Request.GetFormValue("productIds");
            var records = CRMReportManager.GetTestingResults(productIds);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
















    }
}
