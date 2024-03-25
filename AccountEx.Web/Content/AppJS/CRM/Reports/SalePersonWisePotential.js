var SalePersonWisePotential = function () {
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

                //var fromdate = $("#FromDate").val();
                //var toDate = $("#ToDate").val();
                //$("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");
                var year = $("#Year").val();

                var qs = "?key=SalePersonWisePotential";
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { salePersonIds: salePersonIds, regionIds: regionIds },
                        success: function (res) {
                            if (res.Success) {
                                var records = res.Data.Table;
                                var html = "";
                                var amount = 0;
                                var summaryrecords = res.Data.Table;
                                var sn = 1;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var salePersons = Enumerable.From(summaryrecords).Select("$.SalePerson").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Sale Person</th>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th colspan='3' class='align-center'>" + division + "</th>";

                                }
                                headerHtml += "<th colspan='3' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>Total Potential</th>";
                                    headerHtml += "<th class='align-right'>RPL Potential</th>";
                                    headerHtml += "<th class='align-right'>RPL Sale</th>";
                                }
                                headerHtml += "<th class='align-right'>Total Potential</th>";
                                headerHtml += "<th class='align-right'>RPL Potential</th>";
                                headerHtml += "<th class='align-right'>RPL Sale</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";


                                for (var m in salePersons) {
                                    html += "<tr>";
                                    var sp = salePersons[m];
                                    html += "<td>" + sp + "</td>";

                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp && x.Division == division }).Sum("$.TotalPotential");
                                        var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp && x.Division == division }).Sum("$.RPPLPotential");
                                        var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp && x.Division == division }).Sum("$.RPPLSale");
                                        html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rplSale.format() + "</td>";

                                    }
                                    var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp }).Sum("$.TotalPotential");
                                    var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp }).Sum("$.RPPLPotential");
                                    var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.SalePerson == sp }).Sum("$.RPPLSale");
                                    html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rplSale.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right'>Grand Total</td>";
                                    for (var i in divisions) {
                                        var division = divisions[i];

                                        var division = divisions[i];
                                        var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.TotalPotential");
                                        var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.RPPLPotential");
                                        var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.RPPLSale");
                                        html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rplSale.format() + "</td>";
                                    }
                                    var totalPotentail = Enumerable.From(summaryrecords).Sum("$.TotalPotential");
                                    var rpltotalPotentail = Enumerable.From(summaryrecords).Sum("$.RPPLPotential");
                                    var rplSale = Enumerable.From(summaryrecords).Sum("$.RPPLSale");
                                    html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rplSale.format() + "</td>";
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