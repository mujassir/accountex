
var DailyProfitLoss = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });

            $(document).on("change", "#Type", function () {
                var type = $("#Type").val();
                if (type == "Summary") {
                    $("#div-summary").removeClass("hide");
                    $("#div-table").addClass("hide");
                }
                else {
                    $("#div-summary").addClass("hide");
                    $("#div-table").removeClass("hide");
                }
            });
        },
        LoadData: function () {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var qs = "?key=GetDailyProfitLoss";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {

                        var totalSaleAmount = Enumerable.From(res.Data.Transaction).Sum("$.SalePrice");
                        var purchaseCostAmount = Enumerable.From(res.Data.Transaction).Sum("$.PurchasePrice");
                        var gp = Common.GetFloat(totalSaleAmount) - Common.GetFloat(purchaseCostAmount);
                        var dailyExpense = Math.abs(res.Data.Expenses);
                        var netProfit = gp - dailyExpense;

                        $("#lblTotalSaleAmount").html(totalSaleAmount);
                        $("#lblPurchaseCostAmount").html(purchaseCostAmount);
                        $("#lblGrossProfit").html(gp);
                        $("#lblDailyExpense").html(dailyExpense);
                        $("#lblNetProfit").html(netProfit.toFixed("2"));

                        var data = Enumerable.From(res.Data.Transaction).GroupBy("$.SaleDate", null,
                            function (key, g) {
                                var result =
                                    {
                                        SaleDate: key,
                                        ProfitLossByDate: g.ToArray(),
                                    };
                                return result;
                            }).ToArray();

                        var html = "";
                        for (var i in data) {
                            var profitLossRecord = data[i];
                            html += "<tr style='font-weight: bold;'><td colspan='5'>" + (profitLossRecord.SaleDate != null ? moment(profitLossRecord.SaleDate).format("DD/MM/YYYY") : '') + "</td></tr>";
                            var profitLossRecordByDate = profitLossRecord.ProfitLossByDate;
                            for (var j in profitLossRecordByDate) {
                                var record = profitLossRecordByDate[j];
                                html += "<tr>";
                                html += "<td>" + (record.SaleDate != null ? moment(record.SaleDate).format("DD/MM/YYYY") : '') + "</td>";
                                html += "<td>" + record.Item + "</td>";
                                html += "<td class='align-right'>" + record.SalePrice + "</td>";
                                html += "<td class='align-right'>" + record.PurchasePrice + "</td>";
                                html += "<td class='align-right'>" + record.GrossProfit + "</td>";
                                html += "</tr>";
                            }
                        }

                        html += "<tr style='font-weight: bold;'><td colspan='4' class='align-right'>Gross Profit</td><td class='align-right'><span>" + gp + "</span></td></tr>";
                        html += "<tr style='font-weight: bold;'><td colspan='4' class='align-right'>Expenses</td><td class='align-right'><span>" + dailyExpense + "</span></td></tr>";
                        html += "<tr style='font-weight: bold;'><td colspan='4' class='align-right'>Net Profit</td><td class='align-right'><span>" + netProfit.toFixed("2") + "</span></td></tr>";
                        $(".report-table tbody").html(html);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

    };
}();