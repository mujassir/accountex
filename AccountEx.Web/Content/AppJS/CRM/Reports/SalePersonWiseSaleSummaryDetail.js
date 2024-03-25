var SalePersonWiseSaleSummaryDetail = function () {
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
                var qs = "?key=SalePersonWiseSaleSummaryDetail";
                qs += "&fromDate=" + fromdate;
                qs += "&toDate=" + toDate;
                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { regionIds: regionIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var amount = 0;
                                var sn = 1;
                                var summaryrecords = res.Data.Table;
                                var detailrecords = res.Data.Table1;
                                detailrecords = Enumerable.From(detailrecords).OrderBy("$.SalesPerson").ThenBy("$.Date").ToArray();
                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var salePersons = Enumerable.From(summaryrecords).Select("$.SalesPerson").Distinct().OrderBy("$").ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Region</th>";
                                headerHtml += "<th rowspan='2'>Sales Person</th>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th colspan='3' class='align-center'>" + division + "</th>";

                                }
                                headerHtml += "<th colspan='3' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                    headerHtml += "<th class='align-right'>Import</th>";
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }
                                headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                headerHtml += "<th class='align-right'>Import</th>";
                                headerHtml += "<th class='align-right'>Total</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";

                                for (var m in salePersons) {
                                    html += "<tr>";
                                    var sp = salePersons[m];
                                    var sale = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp }).FirstOrDefault();
                                    html += "<td>" + (sale != null ? sale.Region : '') + "</td>";
                                    html += "<td>" + sp + "</td>";


                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var exStockTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp && x.Division == division }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp && x.Division == division }).Sum("$.ImportValue");
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp && x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var exStockTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp }).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp }).Sum("$.ImportValue");
                                    var total = Enumerable.From(summaryrecords).Where(function (x) { return x.SalesPerson == sp }).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td colspan='2' class='align-right'>Grand Total</td>";
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var exStockTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.ImportValue");
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var exStockTotal = Enumerable.From(summaryrecords).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(summaryrecords).Sum("$.ImportValue");
                                    var total = Enumerable.From(summaryrecords).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);
                                html = "";
                                for (var i in detailrecords) {
                                    var record = detailrecords[i];
                                    html += "<tr>";
                                    html += "<td>" + record.SalesPerson + "</td>";
                                    html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                                    html += "<td>" + record.OGPNo + "</td>";
                                    html += "<td>" + record.InvoiceNumber + "</td>";
                                    html += "<td>" + record.Organization + "</td>";
                                    html += "<td>" + record.Product + "</td>";
                                    html += "<td>" + record.Division + "</td>";
                                    html += "<td>" + record.Quantity + "</td>";
                                    html += "<td>" + record.Currency + "</td>";
                                    html += "<td>" + record.Price + "</td>";
                                    html += "<td class='align-right'>" + record.Value.format() + "</td>";
                                    html += "<td>" + (record.DeliveryType == 1 ? "Ex-Stock" : "Import") + "</td>";
                                    html += "<td>" + record.Region + "</td>";
                                    html += "</tr>";

                                    sn++;
                                }
                                if (detailrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                $("#tbl-sale-detail tbody").html(html);


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