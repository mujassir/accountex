
var IncomeStatment = function () {
    var API_CONTROLLER = "VehicleReport";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#BranchId").change(function () {
                $this.LoadData();
            });

        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetVehicleIncomeStatment";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading  Income statement...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var expenses = res.Data.Expenses;
                        var totalExpenses = Enumerable.From(expenses).Sum(function (x) { return x.Balance });
                        var netTotal = data.SalePrice - data.IndriectExpenses - totalExpenses;
                        var html = "   <tr><td><a href='SoldStockAnalysisByDates?fromDate=" + date1 + "&toDate=" + date2 + "&branchId=" + branchId + "' target='_blank'>Total Unit</a></td><td>" + data.NoOfUnits + "</td></tr>";
                        html += "   <tr><td><a href='SoldStockAnalysisByDates?fromDate=" + date1 + "&toDate=" + date2 + "&branchId=" + branchId + "' target='_blank'>Sold Price</a></td><td>" + data.SalePrice.format() + "</td></tr>";
                        html += "   <tr><td><a href='SoldStockAnalysisByDates?fromDate=" + date1 + "&toDate=" + date2 + "&branchId=" + branchId + "' target='_blank'>Total Cost</a></td><td>" + data.Cost.format() + "</td></tr>";
                        html += "   <tr><td>Indirect Expense</td><td>" + totalExpenses.format() + "</td></tr>";
                        html += "   <tr class='bold grand-total'><td>Net Income</td><td>" + netTotal.format() + "</td></tr>";


                        $(".report-table tbody").html(html);

                        var html = "";
                        var data = Enumerable.From(expenses).Where(function (x) { return x.Balance != 0 }).GroupBy("$.ParentId", null,
                   function (key, g) {
                       var result = {
                           ParentId: key,
                           Account: Common.GetById(key),
                           Expenses: g.ToArray(),
                           Total: g.Sum("$.Balance")
                       }
                       return result;
                   }).ToArray();
                        for (var i in data) {
                            var groupExpenses = data[i];
                            var expenses = groupExpenses.Expenses;
                            var account = groupExpenses.Account;
                            html += "<tr data-row='detail' class='bold sub-head'><td colspan='2'>" + account.DisplayName + "</td></tr>";
                            //html += "<tr data-row='summary'><td>" + account.DisplayName + "</td><td>" + Common.GetInt(groupExpenses.Total).format(); + "</td></tr>";
                            for (var j in expenses) {
                                var expense = expenses[j];
                                html += "<tr data-row='detail'><td><a href='../reports/GeneralLedger?fromDate=" + date1 + "&toDate=" + date2 + "&accountId=" + expense.Id + "' target='_blank'>" + expense.Name + "</a></td><td>" + Common.GetInt(expense.Balance).format(); + "</td></tr>";
                            }
                            html += "<tr data-row='detail' class='bold subtotal'><td>" + account.DisplayName + " Total</td><td>" + Common.GetInt(groupExpenses.Total).format(); + "</td></tr>";

                        }
                        $("#report-table-expenses tbody").html(html);


                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        }
    };
}();