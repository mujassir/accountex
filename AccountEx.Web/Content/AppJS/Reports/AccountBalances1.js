
var AccountBalances1 = function () {
    var apiController = "Report";
    var reportHtml = "";
    return {
        init: function () {
            reportHtml = $(".report-div").html();

            $("#Account,#Type").select2();
            var $this = this;
            $("#Type").change(AccountBalances1.Filter);
            $("#btnShowReport").click(function () {
                var date = $("#txtdate").val();

                $this.LoadBalances();
            });
            var fromDate = Common.GetQueryStringValue("fromDate");
            var toDate = Common.GetQueryStringValue("toDate");
            var id = Common.GetQueryStringValue("accountId");
            var childAccountId = Common.GetQueryStringValue("childAccountId");
            if (!Common.isNullOrWhiteSpace(id)) {
                Common.SetDate("#FromDate", fromDate, true);
                Common.SetDate("#ToDate", toDate, true);
                $("#Account").select2("val", id);
                $this.LoadBalances(childAccountId);
            }
            $this.LoadBalances();
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
        LoadBalances: function (childAccountId) {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var accountId = $("#Account").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "?accountId=" + accountId + "&date1=" + date1 + "&date2=" + date2 + "&key=AccountBalances",
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
                            var j = 1;
                            for (var k in data) {
                                html += "<tr>";
                                html += "<td>" + j++ + "</td><td>" + data[k].AccountTitle + "</td>";
                                html += "<td class='align-right'>" + data[k].OpeningBalance.format() + "</td>";
                                html += "<td class='align-right'>" + data[k].Debit + "</td>";
                                html += "<td class='align-right'>" + data[k].Credit + "</td>";
                                html += "<td class='align-right'>" + data[k].Balance.format() + "</td>";
                                html += "</tr>";
                            }
                            typeOptions += "<option>" + res.Data[i].AccountTitle.trim() + "</option>";
                            $(".report-div h2.title").last().text(res.Data[i].AccountTitle);
                            $(".report-div h2.title").last().attr("data-type", res.Data[i].AccountTitle);
                            $(".report-div .report-table:last-child tbody").html(html);
                            html = "<tr><td colspan='3' class='align-right'>Total</td><td class='align-right'>" + res.Data[i].TotalDebit + "</td><td class='align-right'> " + res.Data[i].TotalCredit + "</td><td class='align-right'>" + res.Data[i].Difference + "</td></tr>";
                            //html += "<tr><td colspan='4'></td><td style='text-align: center;' colspan='2' >Difference = " + res.Data[i].Difference + "</td></tr>";
                            $(".report-div .report-table:last-child tfoot").html(html);
                        }
                        $("#Type").html(typeOptions).select2();
                        if (childAccountId != undefined && childAccountId != null) {
                            var childAccount = Common.GetById(childAccountId);
                            $("#Type").select2("val", childAccount.Name).trigger("change");
                        }
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