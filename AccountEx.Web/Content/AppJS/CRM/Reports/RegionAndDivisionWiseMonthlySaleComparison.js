var RegionAndDivisionWiseMonthlySaleComparison = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },



        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var year = $("#Year").val();
                //var fromdate = $("#FromDate").val();
                //var toDate = $("#ToDate").val();
                //$("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");

                var qs = "?key=RegionAndDivisionWiseMonthlySaleComparison";
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { regionIds: regionIds, divisionIds: divisionIds },
                        success: function (res) {
                            if (res.Success) {




                                var html = "";
                                var select = "";
                                var amount = 0;
                                var sn = 1;
                                var summaryrecords = res.Data.Table;
                                var detailrecords = res.Data.Table1;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var regions = Enumerable.From(summaryrecords).Select("$.Region").Distinct().ToArray();
                                var months = Enumerable.From(summaryrecords).Select("$.Month").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Month</th>";
                                for (var i in regions) {
                                    var region = regions[i];
                                    headerHtml += "<th colspan='" + (divisions.length + 1) + "' class='align-center'>" + region + "</th>";

                                }
                                headerHtml += "<th colspan='" + (divisions.length + 1) + "' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in regions) {
                                    var region = regions[i];
                                    for (var j in divisions) {
                                        var division = divisions[j];
                                        headerHtml += "<th class='align-right'>" + division + "</th>";

                                    }
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>" + division + "</th>";

                                }
                                headerHtml += "<th class='align-right'>Total</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";

                                for (var m in months) {
                                    html += "<tr>";
                                    var month = months[m];
                                    html += "<td>" + moment.months(month - 1) + "</td>";

                                    for (var r in regions) {
                                        var region = regions[r];

                                        for (var i in divisions) {
                                            var division = divisions[i];
                                            var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Month == month && x.Region == region && x.Division == division }).Sum("$.TotalValue");
                                            html += "<td class='align-right'>" + total.format() + "</td>";

                                        }
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Month == month && x.Region == region }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";
                                    }
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Month == month && x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + total.format() + "</td>";
                                    }
                                    var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Month == month }).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right'>Grand Total</td>";
                                    for (var r in regions) {
                                        var region = regions[r];
                                        for (var i in divisions) {
                                            var division = divisions[i];
                                            var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division && x.Region == region }).Sum("$.TotalValue");
                                            html += "<td class='align-right'>" + total.format() + "</td>";

                                        }
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region }).Sum("$.TotalValue");
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
                                //var regions = Enumerable.From(records).Select("$.Region").Distinct().ToArray();
                                //var months = Enumerable.From(records).Select("$.Month").Distinct().ToArray();
                                //var headerHtml = "<tr>";
                                //headerHtml += "<th rowspan='2'>Month</th>";
                                //for (var i in regions) {
                                //    var region = regions[i];
                                //    headerHtml += "<th colspan='3' class='align-center'>" + region + "</th>";

                                //}
                                //headerHtml += "</tr>";
                                //headerHtml += "<tr>";
                                //for (var i in regions) {
                                //    var region = regions[i];
                                //    headerHtml += "<th class='align-right'>PC</th>";
                                //    headerHtml += "<th class='align-right'>EC</th>";
                                //    headerHtml += "<th class='align-right'>Dyestuff</th>";
                                //    headerHtml += "<th class='align-right'>Total</th>";
                                //}

                                //$("#tbl-summary thead").html(headerHtml);
                                //headerHtml += "</tr>";


                                //for (var m in months) {
                                //    html += "<tr>";
                                //    var month = months[m];
                                //    html += "<td>" + moment.months(month - 1) + "</td>";

                                //    for (var i in regions) {
                                //        var region = regions[i];
                                //        var pcTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Region == region }).Sum("$.PCTotalValue");
                                //        var ecTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Region == region }).Sum("$.ECTotalValue");
                                //        var dyeStuffTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Region == region }).Sum("$.DyesTotalValue");
                                //        var total = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Division == region }).Sum("$.TotalValue");
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