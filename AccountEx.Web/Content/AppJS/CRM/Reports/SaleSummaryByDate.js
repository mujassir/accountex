var SaleSummaryByDate = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;
    return {
        init: function () {
            debugger;
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
            $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

            var groupId = $("#GroupId").val();
            var subGroupId = $("#SubGroupId").val();
            var invoiceType = $("#InvoiceType").val();
         
            var customerType = $("#CustomerType").val();


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


            var qs = "?key=SaleSummaryByDate";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            qs += "&groupId=" + groupId;
            qs += "&subGroupId=" + subGroupId;
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
                        saleTypeIds: saleTypeIds, customerIds:
                        customerIds, productIds: productIds,
                        regionIds: regionIds, divisionIds: divisionIds,
                        salePersonIds: salePersonIds,
                        industryTypeIds: industryTypeIds
                    },
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var select = "";
                            var amount = 0;
                            var summaryrecords = res.Data.Table;
                            var detailrecords = res.Data.Table1;
                            detailrecords = Enumerable.From(detailrecords).OrderBy("$.Date").ThenBy("$.OGPNo").ToArray();
                            var sn = 1;
                            var exStockTotal = 0;
                            var importTotal = 0;
                            var totalAmount = 0;
                            for (var i in summaryrecords) {
                                var record = summaryrecords[i];
                                html += "<tr>";
                                html += "<td>" + record.Division + "</td>";
                                html += "<td class='align-right'>" + record.ExStock.format() + "</td>";
                                html += " <td class='align-right'>" + record.Import.format() + "</td>";
                                html += "<td class='align-right'>" + record.Total.format() + "</td>";
                                html += "</tr>";
                                exStockTotal += record.ExStock;
                                importTotal += record.Import;
                                totalAmount += record.Total;
                                sn++;
                            }
                            if (summaryrecords.length == 0)
                                html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                            html += "<tr class='bold'><td class='align-right'>Grand Total</td><td class='align-right'>" + exStockTotal.format() + "</td><td class='align-right'>" + importTotal.format() + "</td><td class='align-right'>" + totalAmount.format() + "</td></tr>";
                            $("#tbl-summary tbody").html(html);
                            html = "";
                            for (var i in detailrecords) {
                                var record = detailrecords[i];
                                html += "<tr>";
                                html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + record.OGPNo + "</td>";
                                html += "<td>" + record.PINumber + "</td>";
                                html += "<td>" + record.InvoiceNumber + "</td>";
                                html += "<td>" + record.Organization + "</td>";
                                html += "<td>" + record.Product + "</td>";
                                html += "<td>" + record.Division + "</td>";
                                html += "<td class='align-right'>" + record.Quantity.format() + "</td>";
                                html += "<td>" + record.Currency + "</td>";
                                html += "<td>" + record.Price + "</td>";
                                html += "<td class='align-right'>" + record.Value.format() + "</td>";
                                html += "<td>" + record.SaleType + "</td>";
                                html += "<td>" + record.SalePerson + "</td>";
                                html += "</tr>";
                                exStockTotal += record.ExStock;
                                importTotal += record.Import;
                                totalAmount += record.Total;
                                sn++;
                            }
                            if (detailrecords.length == 0)
                                html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                            else {
                                html += "<tr class='bold'>";
                                html += "<td class='align-right' colspan='6'>Grand Total</td>";
                                var qtyTotal = Enumerable.From(detailrecords).Sum("$.Quantity");
                                var valueTotal = Enumerable.From(detailrecords).Sum("$.Value");
                                html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td colspan='2'></td>";
                                html += "<td class='align-right'>" + valueTotal.format() + "</td>";
                                html += "<td colspan='1'></td>";
                                html += "</tr>";
                            }
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