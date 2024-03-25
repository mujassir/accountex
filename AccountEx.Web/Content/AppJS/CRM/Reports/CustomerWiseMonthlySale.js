var CustomerWiseMonthlySale = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;
    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $("#CustomerType").change(function () {
                $reportCommon.LoadCustomersByType();
            });
            $reportCommon.LoadCustomersByType();

        },


     
        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var customerIds = $("#CustomerIds").val();
                if (!Common.isNullOrWhiteSpace(customerIds))
                    customerIds = customerIds.join(",");

                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");


                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");
                var year = $("#Year").val();
                var industryTypeId = $("#IndustryTypeId").val();

                var deliveryType = $("#DeliveryType").val();
                var customerType = $("#CustomerType").val();
                var qs = "?key=CustomerWiseMonthlySale";
                qs += "&deliveryType=" + deliveryType;
                qs += "&year=" + year;
                qs += "&industryTypeId=" + industryTypeId;
                qs += "&customerType=" + customerType
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { salePersonIds: salePersonIds, regionIds: regionIds, customerIds: customerIds, divisionIds: divisionIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var records = res.Data.Table;
                                var months = new Array();
                                for (var i = 1; i <= 12; i++) {
                                    months.push(i);
                                }
                                var headerHtml = "<tr>";
                                headerHtml += "<th>Organization</th>";
                                headerHtml += "<th>Industry</th>";
                                headerHtml += "<th>Region</th>";
                                headerHtml += "<th>Sale Person</th>";

                                for (var m in months) {
                                    var month = months[m];
                                    headerHtml += "<th class='align-right'>" + moment.months(month - 1) + "</th>";
                                }
                                headerHtml += "<th class='align-right'>Total</th>";
                                headerHtml += "</tr>";
                                $("#tbl-summary thead").html(headerHtml);


                                // Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupRecords = Enumerable.From(records)
    .GroupBy(
        "{ Customer: $.Customer , Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry }",
        "{Value:$.Value | 0,Month:$.Month}",
        "{ Customer: $.Customer, Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry,Records:$$.ToArray() }",
        "$.Customer + ' ' + $.Region+ ' ' + $.SalePerson+ ' ' + $.Industry") // this must be included
    .ToArray();

                                for (var i in groupRecords) {
                                    var groupRecord = groupRecords[i];
                                    var sales = groupRecord.Records;
                                    html += "<tr>";
                                    html += "<td>" + groupRecord.Customer + "</td>";
                                    html += "<td>" + groupRecord.Industry + "</td>";
                                    html += "<td>" + groupRecord.Region + "</td>";
                                    html += "<td>" + groupRecord.SalePerson + "</td>";
                                    for (var m in months) {
                                        var month = months[m];
                                        var value = Enumerable.From(sales).Where(function (x) { return x.Month == month }).Sum("$.Value");
                                        html += "<td class='align-right'>" + value.format() + "</td>";


                                    }
                                    var totalValue = Enumerable.From(sales).Sum("$.Value");
                                    html += "<td class='align-right'>" + totalValue.format() + "</td>";
                                    html += "</tr>";

                                }
                                if (records.length == 0)
                                    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='4'>Grand Total</td>";
                                    for (var m in months) {
                                        var month = months[m];
                                        var value = Enumerable.From(records).Where(function (x) { return x.Month == month }).Sum("$.Value");
                                        html += "<td class='align-right'>" + value.format() + "</td>";


                                    }
                                    var totalValue = Enumerable.From(records).Sum("$.Value");
                                    html += "<td class='align-right'>" + totalValue.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);


                            }
                            else {
                                Common.ShowError(res.Error);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
            }
        },


    };
}();