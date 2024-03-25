
var AccountBalances = function () {
    var apiController = "AccountBalances";
    var reportHtml = "";
    return {
        init: function () {
            reportHtml = $(".report-div").html();

            $("#Account,#Type").select2();
            var $this = this;
            $("#Type").change(AccountBalances.Filter);
            $("#btnShowReport").click(function () {
                var date = $("#txtdate").val();
                var accountId = $("#Account").val();
                $this.LoadBalances(accountId, date);
            });
        },
        Filter: function () {
            var type = $("#Type option:selected").text();
            if (type == "All") {
                $(".report-div").children().removeClass("hide");
            }
            else {
                $(".report-div").children().addClass("hide");
                $(".report-div h2[data-type='" + type + "']").removeClass("hide");
                $(".report-div h2[data-type='" + type + "']").next().removeClass("hide");
            }
        },
        LoadBalances: function (accountId, date) {
            $("#lblReportDate").html("Date: " + date);
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "?accountId=" + accountId + "&date=" + date,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        $("#div-table").removeClass("hide");
                        debugger;
                        $(".report-div").html("");
                        var typeOptions = "<option>All</option>";
                        for (var i = 0; i < res.Data.length; i++) {
                            $(".report-div").append(reportHtml);
                            var html = "";
                            var amount = 0;
                            var data = res.Data[i].Records;
                            var totalDebit = 0
                            var totalCredit = 0
                            var j = 1;
                            for (var k in data) {
                                var account = data[k];
                                var difference = Common.GetFloatHtml(account.Debit) - Common.GetFloatHtml(account.Credit);
                                html += "<tr>";
                                html += "<td>" + j++ + "</td>";
                                html += "<td>" + account.AccountTitle + "</td>";
                                if (difference < 0) {
                                    totalCredit += difference;
                                    html += "<td></td>";
                                    html += "<td>" + difference.format() + "</td>";
                                }
                                else {
                                    totalDebit += difference;
                                    html += "<td>" + difference.format() + "</td>";
                                    html += "<td></td>";
                                }


                                html += "</tr>";
                            }
                            typeOptions += "<option>" + res.Data[i].AccountTitle.trim() + "</option>";
                            $(".report-div h2.title").last().text(res.Data[i].AccountTitle);
                            $(".report-div h2.title").last().attr("data-type", res.Data[i].AccountTitle);
                            $(".report-div .report-table:last-child tbody").html(html);
                            html = "<tr><td colspan='2' class='align-right'>Total</td><td>" + totalDebit.format() + "</td><td>" + totalCredit.format() + "</td></tr>";
                            html += "<tr><td colspan='2'></td><td style='text-align: right;' colspan='2' >Difference = " + (totalDebit - totalCredit).format() + "</td></tr>";
                            $(".report-div .report-table:last-child tfoot").html(html);
                        }
                        $("#Type").html(typeOptions).select2();
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