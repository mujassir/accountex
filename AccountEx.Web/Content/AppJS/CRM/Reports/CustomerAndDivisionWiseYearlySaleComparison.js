var CustomerAndDivisionWiseYearlySaleComparison = function () {
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

                //var fromdate = $("#FromDate").val();
                //var toDate = $("#ToDate").val();
                //$("#lblReportDate").html("Date: " + fromdate + " to " + toDate);


                var yearIds = $("#YearIds").val();
                if (!Common.isNullOrWhiteSpace(yearIds))
                    yearIds = yearIds.join(",");
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


                var deliveryTypeIds = $("#DeliveryTypeIds").val();
                if (!Common.isNullOrWhiteSpace(deliveryTypeIds))
                    deliveryTypeIds = deliveryTypeIds.join(",");
                var customerType = $("#CustomerType").val();

                var qs = "?key=CustomerAndDivisionWiseYearlySaleComparison";
                qs += "&customerType=" + customerType
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { customerIds: customerIds, salePersonIds: salePersonIds, regionIds: regionIds, divisionIds: divisionIds, yearIds: yearIds, deliveryTypeIds: deliveryTypeIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var amount = 0;
                                var sn = 1;
                                var summaryrecords = res.Data.Table;
                                var detailrecords = res.Data.Table1;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var years = Enumerable.From(summaryrecords).Select("$.Year").Distinct().ToArray();
                                var customers = Enumerable.From(summaryrecords).Select("$.Customer").Distinct().ToArray();


                                //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupRecords = Enumerable.From(summaryrecords)
.GroupBy(
"{ Customer: $.Customer , Region: $.Region, SalePerson: $.SalePerson,Industry: $.Industry }",
null,
"{ Customer: $.Customer, Region: $.Region, SalePerson: $.SalePerson,Industry: $.Industry,Records:$$.ToArray() }",
"$.Customer + ' ' + $.Region+ ' ' + $.SalePerson + ' ' + $.Industry") // this must be included
.ToArray();


                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Customer</th>";
                                headerHtml += "<th rowspan='2'>Sale Person</th>";
                                headerHtml += "<th rowspan='2'>Region</th>";
                                headerHtml += "<th rowspan='2'>Industry</th>";
                                for (var i in years) {
                                    var year = years[i];
                                    headerHtml += "<th colspan='" + (divisions.length + 1) + "' class='align-center'>" + year + "</th>";

                                }
                                headerHtml += "<th colspan='" + (divisions.length + 1) + "' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in years) {
                                    var year = years[i];
                                    for (var j in divisions) {
                                        var division = divisions[j];
                                        headerHtml += "<th class='align-right'>" + division + "</th>";

                                    }
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }
                                //grand total header
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>" + division + "</th>";

                                }
                                headerHtml += "<th class='align-right'>Whole Total</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";

                                for (var g in groupRecords) {
                                    var record = groupRecords[g];
                                    var customerRecords = record.Records;
                                    html += "<tr>";
                                    html += "<td>" + record.Customer + "</td>";
                                    html += "<td>" + record.SalePerson + "</td>";
                                    html += "<td>" + record.Region + "</td>";
                                    html += "<td>" + record.Industry + "</td>";

                                    for (var y in years) {
                                        var year = years[y];

                                        for (var i in divisions) {
                                            var division = divisions[i];
                                            var total = Enumerable.From(customerRecords).Where(function (x) { return x.Year == year && x.Division == division }).Sum("$.TotalValue");
                                            html += "<td class='align-right'>" + total.format() + "</td>";

                                        }
                                        var total = Enumerable.From(customerRecords).Where(function (x) { return x.Year == year }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";
                                    }
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var total = Enumerable.From(customerRecords).Where(function (x) { return x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";
                                    }
                                    var total = Enumerable.From(customerRecords).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td colspan='4' class='align-right'>Grand Total</td>";

                                    for (var y in years) {
                                        var year = years[y];
                                        for (var i in divisions) {
                                            var division = divisions[i];
                                            var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division && x.Year == year }).Sum("$.TotalValue");
                                            html += "<td class='align-right'>" + total.format() + "</td>";

                                        }
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Year == year }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var total = Enumerable.From(summaryrecords).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);






                                //var html = "";
                                //var select = "";
                                //var records = res.Data.Table;
                                //var years = Enumerable.From(records).Select("$.Year").Distinct().ToArray();
                                //var customers = Enumerable.From(records).Select("$.Customer").Distinct().ToArray();
                                //var headerHtml = "<tr>";
                                //headerHtml += "<th rowspan='2'>Month</th>";
                                //for (var i in years) {
                                //    var year = years[i];
                                //    headerHtml += "<th colspan='4' class='align-center'>" + year + "</th>";

                                //}
                                //headerHtml += "</tr>";
                                //headerHtml += "<tr>";
                                //for (var i in years) {
                                //    var year = years[i];
                                //    headerHtml += "<th class='align-right'>PC</th>";
                                //    headerHtml += "<th class='align-right'>EC</th>";
                                //    headerHtml += "<th class='align-right'>Dyestuff</th>";
                                //    headerHtml += "<th class='align-right'>Total</th>";
                                //}

                                //$("#tbl-summary thead").html(headerHtml);
                                //headerHtml += "</tr>";


                                //for (var m in customers) {
                                //    html += "<tr>";
                                //    var customer = customers[m];
                                //    html += "<td>" + customer + "</td>";

                                //    for (var i in years) {
                                //        var year = years[i];
                                //        var pcTotal = Enumerable.From(records).Where(function (x) { return x.Customer == customer && x.Year == year }).Sum("$.PCTotalValue");
                                //        var ecTotal = Enumerable.From(records).Where(function (x) { return x.Customer == customer && x.Year == year }).Sum("$.ECTotalValue");
                                //        var dyeStuffTotal = Enumerable.From(records).Where(function (x) { return x.Customer == customer && x.Year == year }).Sum("$.DyesTotalValue");
                                //        var total = Enumerable.From(records).Where(function (x) { return x.Customer == customer && x.Division == year }).Sum("$.TotalValue");
                                //        html += "<td class='align-right'>" + pcTotal.format() + "</td>";
                                //        html += "<td class='align-right'>" + ecTotal.format() + "</td>";
                                //        html += "<td class='align-right'>" + dyeStuffTotal.format() + "</td>";
                                //        html += "<td class='align-right'>" + total.format() + "</td>";

                                //    }
                                //    html += "</tr>";

                                //}
                                ////if (detailrecords.length == 0)
                                ////    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                                //$("#tbl-summary tbody").html(html);


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