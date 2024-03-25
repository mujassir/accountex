
var DailyActivity = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
           var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetDailyActivity";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  daily activity report...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        for (var i in data) {
                            var url = Common.GetTransactionUrl(data[i].TransactionType, data[i].VoucherNumber);
                            var vType = VoucherTypes[data[i].TransactionType];
                            html += "<tr><td>" + data[i].Date + "</td><td><a href='" + url + "' title='" + vType.Description + "'>" + vType.Code + "-" + data[i].VoucherNumber
                                + "</td><td>" + data[i].Code + "</td><td>"
                                + data[i].Name + "</td><td>" + data[i].Description
                                + "</td><td>" + data[i].Debit + "</td><td>" + data[i].Credit + "</td><td>"
                                + data[i].Balance + "</td></tr>";
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='8' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td></td><td>Total</td><td></td><td></td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td><td>" + res.Data.TotalBalance + "</td></tr>";
                        $(".report-table tbody").html(html);
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