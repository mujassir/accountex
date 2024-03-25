var RegionWisePotential = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            //  $this.LoadCustomers();
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

                //var customerIds = $("#CustomerIds").val();

                //if (!Common.isNullOrWhiteSpace(customerIds))
                //    customerIds = customerIds.join(",");

                var qs = "?key=RegionWisePotential";
                //qs += "&customerIds=" + customerIds;
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: '',
                        success: function (res) {
                            if (res.Success) {
                                var records = res.Data.Table;
                                var html = "";
                                var amount = 0;
                                var summaryrecords = res.Data.Table;
                                var sn = 1;
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var regions = Enumerable.From(summaryrecords).Select("$.Region").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Region</th>";
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


                                for (var m in regions) {
                                    html += "<tr>";
                                    var region = regions[m];
                                    html += "<td>" + region + "</td>";

                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region && x.Division == division }).Sum("$.TotalPotential");
                                        var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region && x.Division == division }).Sum("$.RPPLPotential");
                                        var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region && x.Division == division }).Sum("$.RPPLSale");
                                        html += "<td class='align-right'>" + totalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rpltotalPotentail.format() + "</td>";
                                        html += "<td class='align-right'>" + rplSale.format() + "</td>";

                                    }
                                    var totalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region }).Sum("$.TotalPotential");
                                    var rpltotalPotentail = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region }).Sum("$.RPPLPotential");
                                    var rplSale = Enumerable.From(summaryrecords).Where(function (x) { return x.Region == region }).Sum("$.RPPLSale");
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