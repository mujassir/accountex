
var SupplierBalances = function () {
    var API_CONTROLLER = "SupplierBalances";
    var REPORT_HTML = "";
    var Data;
    return {
        init: function () {
            var $this = this;
            REPORT_HTML = $(".report-div").html();
            $("#Account").select2();
            $("#btnShowReport").click(function (e) {
                $this.LoadBalances()
            });

            $("#CityName,#GroupName").change(function (e) {
                $this.FilterReport()
            });
        },
        LoadBalances: function (accountId, date) {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();

            $("#lblReportDate").html("Date: " + date1 + " To " + date2);
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?date1=" + date1 + "&date2=" + date2,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading   cutomers balances...please wait",
                success: function (res) {
                    if (res.Success) {
                        $(".report-div").html("");
                        Data = res.Data;
                        var recordsGroupByCity = Enumerable.From(Data).GroupBy("$.CityName", null,
              function (key, g) {
                  var result =
                      {
                          City: key,
                          Transactions: g.Where(function (x) { return x.Debit != x.Credit }).ToArray(),
                      };
                  return result;
              }).ToArray();

                        for (var i in recordsGroupByCity) {

                            var recordGroupByCity = recordsGroupByCity[i];
                            if (recordGroupByCity.Transactions.length == 0)
                                continue;
                            var html1 = REPORT_HTML;
                            var $html = $(html1);
                            $(".report-div").append($html);


                            $(".report-div label.title").last().text(recordGroupByCity.City);
                            $html.attr("data-city", recordGroupByCity.City)
                            var records = Enumerable.From(recordGroupByCity.Transactions).GroupBy("$.GroupName", null,
             function (key, g) {
                 var result =
                     {
                         GroupName: key,
                         Transactions: g.ToArray(),
                     };
                 return result;
             }).ToArray();
                            var html = "";
                            var grandDebitTotal = 0
                            var grandCreditTotal = 0;
                            for (var i in records) {

                                var record = records[i];
                                html += "<tr data-group='" + record.GroupName + "' style='font-weight: bold;'><td class='group' colspan='4'>Group Name: " + record.GroupName + "</td></tr>";
                                var balances = record.Transactions;
                                for (var j in balances) {
                                    var data = balances[j];
                                    html += "<tr data-group='" + record.GroupName + "'>";

                                    const maxAmountThreshold = data?.MaxAmountThreshold ?? Infinity;
                                    const creditAmount = data?.Credit ?? 0;
                                    html += `<td${maxAmountThreshold < creditAmount ? " class='red-flag'" : ""}>${data.Code}</td>`;

                                    html += " <td><a href='general-ledger?accountId=" + data.AccountId + "&fromDate=" + date1 + "&toDate=" + date2 + "'>" + data.Name + "</td>";
                                    html += "<td class='align-right'>" + (data.Debit > 0 ? data.Debit.format() : "") + "</td>";
                                    html += "<td class='align-right'>" + (data.Credit > 0 ? data.Credit.format() : "") + "</td>";
                                    html += "</tr>";
                                }
                                var debitTotal = Enumerable.From(balances).Sum("$.Debit");
                                var creditTotal = Enumerable.From(balances).Sum("$.Credit");
                                grandDebitTotal += debitTotal;
                                grandCreditTotal += creditTotal;
                                html += "<tr data-group='" + record.GroupName + "'  class='bold subtotal align-right'><td class='' colspan='2'>" + record.GroupName + " Total:</td><td>" + debitTotal.format() + "</td><td>" + creditTotal.format() + "</td></tr>";
                            }

                            html += "<tr data-city='" + recordGroupByCity.City + "'  class='bold grand-total align-right'><td class='' colspan='2' style='font-size: 20px;'>" + recordGroupByCity.City + " Total:</td><td>" + grandDebitTotal.format() + "</td><td>" + grandCreditTotal.format() + "</td></tr>";
                            $html.find("table tbody").append(html);
                        }
                        $this.FilterReport();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        FilterReport: function () {

            var city = $("#CityName").val();
            var group = $("#GroupName").val();
            if (group == "0") {
                $("[data-group]").removeClass("hide");
            }
            else {
                $("[data-group]").addClass("hide");
                $("[data-group='" + group + "']").removeClass("hide");
            }
            $("#div-table").removeClass("hide");

        }
    };
}();