var YearWiseMonthlySaleComparison = function () {
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

                var fromdate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

                var yearIds = $("#YearIds").val();
                if (!Common.isNullOrWhiteSpace(yearIds))
                    yearIds = yearIds.join(",");

                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");
                var qs = "?key=YearWiseMonthlySaleComparison";
                qs += "&fromDate=" + fromdate;
                qs += "&toDate=" + toDate;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { yearIds: yearIds, regionIds: regionIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                
                                var records = res.Data.Table;
                                var years = Enumerable.From(records).Select("$.Year").OrderBy("$").Distinct().ToArray();
                                var months = Enumerable.From(records).Select("$.Month").OrderBy("$").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Month</th>";
                                for (var i in years) {
                                    var year = years[i];
                                    headerHtml += "<th colspan='3' class='align-center'>" + year + "</th>";

                                }
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in years) {
                                    var year = years[i];
                                    headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                    headerHtml += "<th class='align-right'>Import</th>";
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }

                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";


                                for (var m in months) {
                                    html += "<tr>";
                                    var month = months[m];
                                    html += "<td>" + moment.months(month - 1) + "</td>";

                                    for (var y in years) {
                                        var year = years[y];
                                        var exStockTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Year == year }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Year == year }).Sum("$.ImportValue");
                                        var total = Enumerable.From(records).Where(function (x) { return x.Month == month && x.Year == year }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    html += "</tr>";

                                }
                                if (records.length == 0)
                                    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='1'>Grand Total</td>";
                                    for (var y in years) {
                                        var year = years[y];
                                        var exStockTotal = Enumerable.From(records).Where(function (x) { return x.Year == year }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(records).Where(function (x) { return  x.Year == year }).Sum("$.ImportValue");
                                        var total = Enumerable.From(records).Where(function (x) { return  x.Year == year }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
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