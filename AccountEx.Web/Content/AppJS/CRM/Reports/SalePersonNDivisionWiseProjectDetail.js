var SalePersonNDivisionWiseProjectDetail = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.LoadCustomers();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },


        LoadCustomers: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetCustomerIdNameByUserId",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#CustomerIds"), true)
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                //var fromdate = $("#FromDate").val();
                //var toDate = $("#ToDate").val();
                //$("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
                var year = $("#Year").val();
                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");

                var qs = "?key=SalePersonNDivisionWiseProjectDetail";
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { salePersonIds: salePersonIds, regionIds: regionIds, divisionIds: divisionIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var summaryrecords = res.Data.Table;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var salePersons = Enumerable.From(summaryrecords).Select("$.SalePerson").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Sale Person</th>";

                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th colspan='2' class='align-center'>" + division + "</th>";

                                }
                                headerHtml += "<th colspan='2' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";

                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>Target P</th>";
                                    headerHtml += "<th class='align-right'>Ach. P</th>";
                                }
                                //for grand total
                                headerHtml += "<th class='align-right'>Target P</th>";
                                headerHtml += "<th class='align-right'>Ach. P</th>";

                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";


                                for (var i in salePersons) {
                                    var sp = salePersons[i];
                                    html += "<tr>";
                                    html += "<td>" + sp + "</td>";

                                    for (var d in divisions) {
                                        var division = divisions[d];
                                        var target = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division && x.SalePerson == sp }).Sum("$.TargetPotential");
                                        var achieved = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division && x.SalePerson == sp }).Sum("$.AchievedPotential");

                                        html += "<td class='align-right'>" + target.format() + "</td>";
                                        html += "<td class='align-right'>" + achieved.format() + "</td>";


                                    }
                                    var target = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp }).Sum("$.TargetPotential");
                                    var achieved = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp }).Sum("$.AchievedPotential");
                                    html += "<td class='align-right'>" + target.format() + "</td>";
                                    html += "<td class='align-right'>" + achieved.format() + "</td>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right'>Grand Total</td>";
                                    for (var i in divisions) {
                                        var division = divisions[i];

                                        var division = divisions[i];
                                        var target = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.TargetPotential");
                                        var achieved = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.AchievedPotential");
                                        html += "<td class='align-right'>" + target.format() + "</td>";
                                        html += "<td class='align-right'>" + achieved.format() + "</td>";
                                    }

                                    var target = Enumerable.From(summaryrecords).Sum("$.TargetPotential");
                                    var achieved = Enumerable.From(summaryrecords).Sum("$.AchievedPotential");
                                    html += "<td class='align-right'>" + target.format() + "</td>";
                                    html += "<td class='align-right'>" + achieved.format() + "</td>";
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