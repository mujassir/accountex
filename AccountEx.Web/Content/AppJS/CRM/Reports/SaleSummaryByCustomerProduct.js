var SaleSummaryByCustomerProduct = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;
    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $(document).on("change", "#GroupId", function (event) {
                $reportCommon.GetSubGroupByGroupId();
            });
            $("#CustomerType").change(function () {
                $reportCommon.LoadCustomersByType();
            });
            $reportCommon.LoadCustomersByType();
            $reportCommon.LoadProducts();

        },
        LoadData: function () {

            var $this = this;
            var fromdate = $("#FromDate").val();
            var toDate = $("#ToDate").val();
            var industryTypeId = $("#IndustryTypeId").val();
            $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

            var groupId = $("#GroupId").val();
            var subGroupId = $("#SubGroupId").val();
            var industryTypeId = $("#IndustryTypeId").val();
            var customerType = $("#CustomerType").val();
            var invoiceType = $("#InvoiceType").val();


            var customerIds = $("#CustomerIds").val();
            var productIds = $("#ProductIds").val();
            if (!Common.isNullOrWhiteSpace(customerIds))
                customerIds = customerIds.join(",");


            var regionIds = $("#RegionIds").val();
            if (!Common.isNullOrWhiteSpace(regionIds))
                regionIds = regionIds.join(",");

            var divisionIds = $("#DivisionIds").val();
            if (!Common.isNullOrWhiteSpace(divisionIds))
                divisionIds = divisionIds.join(",");

            if (!Common.isNullOrWhiteSpace(productIds))
                productIds = productIds.join(",");

            var salePersonIds = $("#SalePersonIds").val();
            if (!Common.isNullOrWhiteSpace(salePersonIds))
                salePersonIds = salePersonIds.join(",");

            var industryTypeIds = $("#IndustryTypeIds").val();
            if (!Common.isNullOrWhiteSpace(industryTypeIds))
                industryTypeIds = industryTypeIds.join(",");



            var qs = "?key=SaleSummaryByCustomerProduct";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            qs += "&groupId=" + groupId;
            qs += "&subGroupId=" + subGroupId;
            qs += "&industryTypeId=" + industryTypeId;
            qs += "&customerType=" + customerType
            qs += "&invoiceType=" + invoiceType
            var deliveryTypeIds = $("#DeliveryTypeIds").val();
            if (!Common.isNullOrWhiteSpace(deliveryTypeIds))
                deliveryTypeIds = deliveryTypeIds.join(",");

            var saleTypeIds = $("#SaleTypeIds").val();
            if (!Common.isNullOrWhiteSpace(saleTypeIds))
                saleTypeIds = saleTypeIds.join(",");

            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: {
                        deliveryTypeIds: deliveryTypeIds,
                        saleTypeIds: saleTypeIds,
                        customerIds: customerIds,
                        productIds: productIds,
                        regionIds: regionIds,
                        divisionIds: divisionIds,
                        salePersonIds: salePersonIds,
                        industryTypeIds: industryTypeIds
                    },
                    success: function (res) {
                        if (res.Success) {

                            var html = "";
                            var reportType = $("#ReportType").val();
                            var select = "";
                            var records = res.Data.Table;

                            var groupRecords = Enumerable.From(records)
  .GroupBy(
      "{ Customer: $.Customer , Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry,Group:$.Group,Division:$.Division,Product:$.Product }",
      "{ExStockValue:$.ExStockValue | 0,ImportValue:$.ImportValue | 0,ExStockQty:$.ExStockQty | 0,ImportQty:$.ImportQty | 0, Month:$.Month}",
      "{ Customer: $.Customer, Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry,Month:$.Month,Group:$.Group,Division:$.Division,Product:$.Product,Records:$$.ToArray() }",
      "$.Customer + ' ' + $.Region+ ' ' + $.SalePerson+ ' ' + $.Industry + ' ' + $.Group + ' ' + $.Division + ' ' + $.Product") // this must be included
  .ToArray();


                            for (var i in groupRecords) {
                                var sale = groupRecords[i];
                                html += "<tr>";
                                html += "<td>" + sale.Region + "</td>";
                                html += "<td>" + sale.Customer + "</td>";
                                html += "<td>" + sale.SalePerson + "</td>";
                                html += "<td>" + sale.Group + "</td>";
                                html += "<td>" + sale.Division + "</td>";
                                html += "<td>" + sale.Product + "</td>";

                                var totalValue = Common.GetInt(Enumerable.From(sale.Records).Sum("$.ExStockValue+$.ImportValue"));
                                var totalQty = Common.GetInt(Enumerable.From(sale.Records).Sum("$.ExStockQty+$.ImportQty"));

                                html += "<td class='align-right'>" + (totalQty).format() + "</td>";
                                html += "<td class='align-right'>" + (totalValue).format() + "</td>";
                                html += "</tr>";

                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='8' style='text-align: center'>No record(s) found</td></tr>";
                            else {
                                html += "<tr class='bold'>";
                                html += "<td class='align-right' colspan='6'>Grand Total</td>";
                                var totalValue = Common.GetInt(Enumerable.From(records).Sum("$.ExStockValue+$.ImportValue"));
                                var totalQty = Common.GetInt(Enumerable.From(records).Sum("$.ExStockQty+$.ImportQty"));
                                html += "<td class='align-right'>" + totalQty.format() + "</td>";
                                html += "<td class='align-right'>" + totalValue.format() + "</td>";
                                html += "</tr>";
                            }
                            $("#tbl-summary tbody").html(html);

                            //detail of report
                            html = "";
                            var months = new Array();
                            for (var i = 1; i <= 12; i++) {
                                months.push(i);
                            }
                            var headerHtml = "<tr>";
                            headerHtml += "<th rowspan='2'>Region</th>";
                            headerHtml += "<th rowspan='2'>Customer</th>";
                            headerHtml += "<th rowspan='2'>Industry</th>";
                            headerHtml += "<th rowspan='2'>Sale Person</th>";
                            headerHtml += "<th rowspan='2'>Division</th>";
                            headerHtml += "<th rowspan='2'>Group</th>";
                            headerHtml += "<th rowspan='2'>Product</th>";

                            for (var m in months) {
                                var month = months[m];
                                headerHtml += "<th colspan='2' class='align-center' >" + moment.months(month - 1) + "</th>";
                            }
                            headerHtml += "</tr>";
                            headerHtml += "<tr>";
                            for (var m in months) {
                                var month = months[m];
                                headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                headerHtml += "<th class='align-right'>Import</th>";
                            }
                            headerHtml += "</tr>";

                            $("#tbl-sale-detail thead").html(headerHtml);
                            for (var i in groupRecords) {
                                var groupRecord = groupRecords[i];
                                var sales = groupRecord.Records;
                                html += "<tr>";
                                html += "<td>" + groupRecord.Region + "</td>";
                                html += "<td>" + groupRecord.Customer + "</td>";
                                html += "<td>" + groupRecord.Industry + "</td>";
                                html += "<td>" + groupRecord.SalePerson + "</td>";
                                html += "<td>" + groupRecord.Division + "</td>";
                                html += "<td>" + groupRecord.Group + "</td>";
                                html += "<td>" + groupRecord.Product + "</td>";
                                for (var m in months) {
                                    var month = months[m];
                                    var exStockTotal = 0;
                                    var importTotal = 0;
                                    if (reportType == 2) {
                                        //value
                                        exStockTotal = Enumerable.From(sales).Where(function (x) { return x.Month == month }).Sum("$.ExStockValue");
                                        importTotal = Enumerable.From(sales).Where(function (x) { return x.Month == month }).Sum("$.ImportValue");
                                    }
                                    else {
                                        //Qty
                                        exStockTotal = Enumerable.From(sales).Where(function (x) { return x.Month == month }).Sum("$.ExStockQty");
                                        importTotal = Enumerable.From(sales).Where(function (x) { return x.Month == month }).Sum("$.ImportQty");
                                    }
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";

                                }
                                html += "</tr>";

                            }
                            //if (records.length == 0)
                            //    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                            //else {
                            //    html += "<tr class='bold'>";
                            //    html += "<td class='align-right' colspan='4'>Grand Total</td>";
                            //    for (var m in months) {
                            //        var month = months[m];
                            //        var value = Enumerable.From(records).Where(function (x) { return x.Month == month }).Sum("$.Value");
                            //        html += "<td class='align-right'>" + value.format() + "</td>";


                            //    }
                            //    var totalValue = Enumerable.From(records).Sum("$.Value");
                            //    html += "<td class='align-right'>" + totalValue.format() + "</td>";
                            //    html += "</tr>";
                            //}
                            $("#tbl-sale-detail tbody").html(html);




                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },


    };
}();