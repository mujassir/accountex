
var DetailTrialBalances = function () {
    var apiController = "DetailTrialBalance";
    return {
        init: function () {
            var $this = this;
            //var d = new Date();
            //d.setDate(1);
            //d.setMonth(6);
            //var todate = new Date(Common.Fiscal.ToDate);
            //todate.setHours(0, 0, 0, 0);
            //if (d > new Date()) d.setFullYear(d.getFullYear() - 1);
            //$("#FromDate").datepicker("setDate", new Date(Common.Fiscal.FromDate));
            //$("#ToDate").datepicker("setDate", todate);

            $("#btnShowReport").click(function () {
                var date1 = $("#FromDate").val();
                var date2 = $("#ToDate").val();
                $this.LoadTrailBalance(date1, date2);
            });
        },
        LoadTrailBalance: function (date1, date2) {
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var isfilterzero = $("#FilterZeroBalance").is(":checked")
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "?date1=" + date1 + "&date2=" + date2 + "&IsFilter=" + isfilterzero,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  trial balance...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var j = 1;
                        for (var i in data) {
                            var trans = data[i];
                            html += "<tr>"
                            html += " <td>" + trans.Code + "</td>";
                            html += "<td><a href='../reports/general-ledger?accountId=" + trans.AccountId + "&fromDate=" + date1 + "&toDate=" + date2 + "'>" + trans.AccountTitle + "</a></td>";
                            html += "<td class='align-right'>" + trans.OpeningBalance + "</td>";
                            html += "<td class='align-right'>" + trans.Debit + "</td>";
                            html += " <td class='align-right'>" + trans.Credit + "</td>";
                            html += " <td class='align-right'>" + trans.Balance + "</td>";
                            html += " </tr>";
                        }
                        $("#tbldetail tbody").html(html);
                        debugger
                        html = "<tr><td colspan='2'>Total</td><td class='align-right'>" + res.Data.TotalOpeningBalance + "</td><td class='align-right'>" + res.Data.TotalDebit + "</td><td class='align-right'>" + res.Data.TotalCredit + "</td><td class='align-right'>" + res.Data.TotalBalance + "</td></tr>";
                        html += "<tr><td colspan='4'></td><td style='text-align: right;' colspan='2' >Difference = " + res.Data.Difference + "</td></tr>";
                        $("#tbldetail tfoot").html(html);
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