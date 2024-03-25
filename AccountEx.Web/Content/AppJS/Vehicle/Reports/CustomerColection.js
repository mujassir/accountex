
var CustomerColection = function () {
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
            var accountId = $("#AccountId").val();
            var qs = "?key=GetCustomerCollections";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            qs += "&accountId=" + accountId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Customer Colection...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var records = res.Data.Records;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";
                            html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Customer + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td>" + record.ProductName + "</td>";
                            html += "<td>" + record.PaymentAccount + "</td>";
                            html += "<td class='align-right'>" + record.Amount.format() + "</td>";
                            html += "</tr>";
                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        var TotalBalance = Common.GetInt(Enumerable.From(records).Sum("$.Amount"));
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan = '5' class='align-right'>Total</td>";
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