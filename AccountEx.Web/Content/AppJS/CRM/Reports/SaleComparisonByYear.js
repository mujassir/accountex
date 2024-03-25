var SaleComparisonByYear = function () {
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
            var qs = "?key=SaleComparisonByYear";
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
                                var exStockyear1Total = 0;
                                var importyear1Total = 0;
                                var totalyear1 = 0;
                                var exStockyear2Total = 0;
                                var importyear2Total = 0;
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
                                        html += "<th class='align-right'>Ex-Stock</th>";
                                        html += "<th class='align-right'>Import</th>";
                                        html += "<th class='align-right'>Total</th>";
                                    }
                                    if (years.length > 1) {
                                        html += "<th class='align-right'>Ex-Stock</th>";
                                        html += "<th class='align-right'>Import</th>";
                                        html += "<th class='align-right'>Total</th>";
                                    }
                                    html += "</tr>";

                                    for (var d in divisions) {
                                        html += "<tr>";
                                        var division = divisions[d];
                                        html += "<td>" + division + "</td>";
                                        var year1 = years[0];
                                        var year2 = years[1];
                                        exStockyear1Total = 0;
                                        importyear1Total = 0;
                                        totalyear1 = 0;
                                        exStockyear2Total = 0;
                                        importyear2Total = 0;
                                        totalyear2 = 0;


                                        if (year1 != null && year1 != undefined) {


                                            exStockyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.ExStock"));
                                            importyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.Import"));
                                            totalyear1 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header && x.Division == division }).Sum("$.Total"));
                                            html += "<td class='align-right'>" + exStockyear1Total.format() + "</td>";
                                            html += "<td class='align-right'>" + importyear1Total.format() + "</td>";
                                            html += "<td class='align-right'>" + totalyear1.format() + "</td>";
                                        }
                                        if (year2 != null && year2 != undefined) {
                                            exStockyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.ExStock"));
                                            importyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.Import"));
                                            totalyear2 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header && x.Division == division }).Sum("$.Total"));
                                            html += "<td class='align-right'>" + exStockyear2Total.format() + "</td>";
                                            html += "<td class='align-right'>" + importyear2Total.format() + "</td>";
                                            html += "<td class='align-right'>" + totalyear2.format() + "</td>";
                                        }

                                        if (year2 != null && year2 != undefined && (year1 != null && year1 != undefined)) {

                                            var exStockChange = 0;
                                            var importChange = 0;
                                            var totalChange = 0;
                                            if (exStockyear1Total > 0)
                                                exStockChange = Common.GetFloat(((exStockyear2Total - exStockyear1Total) / exStockyear1Total) * 100);




                                            if (importyear1Total > 0)
                                                importChange = Common.GetFloat(((importyear2Total - importyear1Total) / importyear1Total) * 100);




                                            if (totalyear1 > 0)
                                                totalChange = Common.GetFloat(((totalyear2 - totalyear1) / totalyear1) * 100);


                                            html += "<td class='align-right'>" + exStockChange.format() + "</td>";
                                            html += "<td class='align-right'>" + importChange.format() + "</td>";
                                            html += "<td class='align-right'>" + totalChange.format() + "</td>";
                                        }

                                        html += "</tr>";
                                    }

                                    html += "<tr class='bold grand-total'>";
                                    html += "<td class='align-right' colspan='1'>Grand Total</td>";
                                    var year1 = years[0];
                                    var year2 = years[1];
                                    if (year1 != null && year1 != undefined) {


                                        exStockyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.ExStock"));
                                        importyear1Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.Import"));
                                        totalyear1 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year1 && x[type] == header }).Sum("$.Total"));
                                        html += "<td class='align-right'>" + exStockyear1Total.format() + "</td>";
                                        html += "<td class='align-right'>" + importyear1Total.format() + "</td>";
                                        html += "<td class='align-right'>" + totalyear1.format() + "</td>";
                                    }
                                    if (year2 != null && year2 != undefined) {
                                        exStockyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.ExStock"));
                                        importyear2Total = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.Import"));
                                        totalyear2 = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Year == year2 && x[type] == header }).Sum("$.Total"));
                                        html += "<td class='align-right'>" + exStockyear2Total.format() + "</td>";
                                        html += "<td class='align-right'>" + importyear2Total.format() + "</td>";
                                        html += "<td class='align-right'>" + totalyear2.format() + "</td>";
                                    }

                                    if (year2 != null && year2 != undefined && (year1 != null && year1 != undefined)) {

                                        var exStockChange = 0;
                                        var importChange = 0;
                                        var totalChange = 0;
                                        if (exStockyear1Total > 0)
                                            exStockChange = Common.GetFloat(((exStockyear2Total - exStockyear1Total) / exStockyear1Total) * 100);

                                        if (importyear1Total > 0)
                                            importChange = Common.GetFloat(((importyear2Total - importyear1Total) / importyear1Total) * 100);

                                        if (totalyear1 > 0)
                                            totalChange = Common.GetFloat(((totalyear2 - totalyear1) / totalyear1) * 100);



                                        html += "<td class='align-right'>" + exStockChange.format() + "</td>";
                                        html += "<td class='align-right'>" + importChange.format() + "</td>";
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