
var CashBankSummary = function () {
    var API_CONTROLLER = "VehicleReport";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
        },
        LoadData: function () {
            var date1 = $("#Date").val();
            var date2 = $("#ToDate").val();
          
            var qs = "?key=GetCashBankSummary";
            qs += "&date1=" + date1;
           
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading cash & bank Summary...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var j = 1;
                        var records = res.Data.Records;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";

                            html += "<td>" + j + "</td>";
                            html += "<td>" + record.Name + "</td>";
                            html += "<td class='align-right'>" + record.Balance.format() + "</td>";
                            html += "</tr>";
                            j++;
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='3' style='text-align: center'>No record(s) found</td></tr>";


                        var TotalBalance = Common.GetInt(Enumerable.From(records).Sum("$.Balance"));
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan = '2' class='align-right'>Total</td>";

                        html += "<td class='align-right'>" + TotalBalance.format() + "</td>";
                        html += "</tr>";
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