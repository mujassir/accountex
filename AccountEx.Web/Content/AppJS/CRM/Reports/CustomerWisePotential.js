var CustomerWisePotential = function () {
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

                var customerIds = $("#CustomerIds").val();
                var year = $("#Year").val();
                if (!Common.isNullOrWhiteSpace(customerIds))
                    customerIds = customerIds.join(",");

                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var currencyIds = $("#CurrencyIds").val();
                if (!Common.isNullOrWhiteSpace(currencyIds))
                    currencyIds = currencyIds.join(",");

                var qs = "?key=CustomerWisePotential";
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { customerIds: customerIds, salePersonIds: salePersonIds, regionIds: regionIds, currencyIds: currencyIds },
                        success: function (res) {
                            if (res.Success) {
                                var records = res.Data.Table;
                                var customers = Enumerable.From(records).Select("$.Customer").Distinct().ToArray();
                                var html = "";
                                var amount = 0;
                                var summaryrecords = res.Data.Table;
                                var sn = 1;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var customers = Enumerable.From(summaryrecords).Select("$.Customer").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2' style='width:15%'>Organization</th>";
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


                                for (var m in customers) {
                                    html += "<tr>";
                                    var customer = customers[m];
                                    html += "<td style='width:15%'>" + customer + "</td>";

                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer && x.Division == division }).Sum("$.TotalPotential");
                                        var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer && x.Division == division }).Sum("$.RPPLPotential");
                                        var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer && x.Division == division }).Sum("$.RPPLSale");
                                        html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rplSale.format() + "</td>";

                                    }
                                    var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer }).Sum("$.TotalPotential");
                                    var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer }).Sum("$.RPPLPotential");
                                    var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.Customer == customer }).Sum("$.RPPLSale");
                                    html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                    html += "<td class='align-right'>" + rplSale.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' >Grand Total</td>";
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