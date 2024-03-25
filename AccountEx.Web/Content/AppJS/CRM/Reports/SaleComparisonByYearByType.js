var SaleComparisonByYearByType = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },


        LoadData: function () {

            var $this = this;
            var fromdate = $("#FromDate").val();
            var toDate = $("#ToDate").val();
            var fromdate1 = $("#FromDate1").val();
            var toDate1 = $("#ToDate1").val();
            $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
            var qs = "?key=SaleComparisonByYearByType";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            qs += "&fromDate1=" + fromdate1;
            qs += "&toDate1=" + toDate1;


            var regionIds = $("#RegionIds").val();
            if (!Common.isNullOrWhiteSpace(regionIds))
                regionIds = regionIds.join(",");

            var divisionIds = $("#DivisionIds").val();
            if (!Common.isNullOrWhiteSpace(divisionIds))
                divisionIds = divisionIds.join(",");

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
                            var records = res.Data.Table;

                            var years = Enumerable.From(records).Select("$.Year").Distinct().Take(2).ToArray();
                            var sn = 1;
                            var exStockTotal = 0;
                            var importTotal = 0;
                            var totalAmount = 0;
                            var loopCounter = 2;
                            if (PageSetting.UserTypeId == CRMUserType.SalesExecutive)
                                loopCounter = 1;
                            html += "<tr>";
                            for (c = 1; c <= loopCounter; c++) {

                                var projectyear1Total = 0;
                                var regularyear1Total = 0;
                                var totalyear1 = 0;
                                var projectyear2Total = 0;
                                var regularyear2Total = 0;
                                var totalyear2 = 0;

                                var type = "SalePerson"
                                if (c == 2)
                                    type = "Region";

                                var groupheaders = Enumerable.From(records).Select("$." + type + "").Distinct().ToArray();

                                html += "<td style='width:50% important'>";
                                html += "<table class='table table-striped table-bordered'>";
                                for (var i in groupheaders) {
                                    var header = groupheaders[i];
                                    var divisions = Enumerable.From(records).Where("$." + type + "=='" + header + "'").Select("$.Division").Distinct().ToArray();
                                    divisions = Enumerable.From(divisions).OrderByDescending("$").ToArray();
                                    html += "<tr>";
                                    html += "<th class='align-center valign-middle'  rowspan='2'>" + header + "</td>";

                                    for (var y in years) {
                                        var year = years[y];
                                        html += "<th colspan='3' class='align-center'>" + year + "</th>";
                                    }
                                    if (years.length > 1) {
                                        html += "<th colspan='3' class='align-center'>%</th>";
                                    }
                                    html += "</tr>";
                                    html += "<tr>";
                                    for (var y in years) {
                                        var year = years[y];
                                        html += "<th class='align-right'>Regular</th>";
                                        html += "<th class='align-right'>Project</th>";
                                        html += "<th class='align-right'>Total</th>";
                                    }
                                    if (years.length > 1) {
                                        html += "<th class='align-right'>Regular</th>";
                                        html += "<th class='align-right'>Project</th>";
                                        html += "<th class='align-right'>Total</th>";
                                    }
                                    html += "</tr>";

                                    for (var d in divisions) {
                                        html += "<tr>";
                                        var division = divisions[d];
                                        html += "<td>" + division + "</td>";
                                        var year1 = years[0];
                                        var year2 = years[1];
                                        if (year1 != null && year1 != undefined) {


                                             projectyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.Project"));
                                            regularyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.Regular"));
                                            totalyear1 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.Total"));
                                            html += "<td class='align-right'>" + regularyear1Total.format() + "</td>";
                                            html += "<td class='align-right'>" + projectyear1Total.format() + "</td>";
                                            html += "<td class='align-right'>" + totalyear1.format() + "</td>";
                                        }
                                        if (year2 != null && year2 != undefined) {
                                            var projectyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.Project"));
                                            var regularyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.Regular"));
                                            var totalyear2 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.Total"));
                                            html += "<td class='align-right'>" + regularyear2Total.format() + "</td>";
                                            html += "<td class='align-right'>" + projectyear2Total.format() + "</td>";
                                            html += "<td class='align-right'>" + totalyear2.format() + "</td>";
                                        }

                                        if (year2 != null && year2 != undefined && (year1 != null && year1 != undefined)) {

                                            projectChange = 0;
                                            regularChange = 0;
                                            totalChange = 0;
                                            if (projectyear1Total > 0)
                                                projectChange = Common.GetFloat(((projectyear2Total - projectyear1Total) / projectyear1Total) * 100);




                                            if (regularyear1Total > 0)
                                                regularChange = Common.GetFloat(((regularyear2Total - regularyear1Total) / regularyear1Total) * 100);




                                            if (totalyear1 > 0)
                                                totalChange = Common.GetFloat(((totalyear2 - totalyear1) / totalyear1) * 100);


                                            html += "<td class='align-right'>" + regularChange.format() + "</td>";
                                            html += "<td class='align-right'>" + projectChange.format() + "</td>";
                                            html += "<td class='align-right'>" + totalChange.format() + "</td>";
                                        }

                                        html += "</tr>";
                                    }

                                    html += "<tr class='bold grand-total'>";
                                    html += "<td class='align-right' colspan='1'>Grand Total</td>";
                                    var year1 = years[0];
                                    var year2 = years[1];
                                    if (year1 != null && year1 != undefined) {


                                        projectyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.Project"));
                                        regularyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.Regular"));
                                        totalyear1 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.Total"));
                                        html += "<td class='align-right'>" + regularyear1Total.format() + "</td>";
                                        html += "<td class='align-right'>" + projectyear1Total.format() + "</td>";
                                        html += "<td class='align-right'>" + totalyear1.format() + "</td>";
                                    }
                                    if (year2 != null && year2 != undefined) {
                                        projectyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.Project"));
                                        regularyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.Regular"));
                                        totalyear2 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.Total"));
                                        html += "<td class='align-right'>" + regularyear2Total.format() + "</td>";
                                        html += "<td class='align-right'>" + projectyear2Total.format() + "</td>";
                                        html += "<td class='align-right'>" + totalyear2.format() + "</td>";
                                    }

                                    if (year2 != null && year2 != undefined && (year1 != null && year1 != undefined)) {

                                        var projectChange = 0;
                                        var regularChange = 0;
                                        var totalChange = 0;
                                        if (regularyear1Total > 0)
                                            regularChange = Common.GetFloat(((regularyear2Total - regularyear1Total) / regularyear1Total) * 100);

                                        if (projectyear1Total > 0)
                                            projectChange = Common.GetFloat(((projectyear2Total - projectyear1Total) / projectyear1Total) * 100);

                                        if (totalyear1 > 0)
                                            totalChange = Common.GetFloat(((totalyear2 - totalyear1) / totalyear1) * 100);



                                        html += "<td class='align-right'>" + regularChange.format() + "</td>";
                                        html += "<td class='align-right'>" + projectChange.format() + "</td>";
                                        html += "<td class='align-right'>" + totalChange.format() + "</td>";
                                    }

                                    html += "</tr>";




                                }
                                html += "</table>";
                                html += "</td>";
                                html += "<td></td>";
                            }
                            html += "</tr>";



                            if (records.length == 0)
                                html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
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
        },

        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

        },
    };
}();