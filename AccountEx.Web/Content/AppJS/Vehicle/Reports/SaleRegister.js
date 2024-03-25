
var SaleRegister = function () {
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
            })
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetVehicleSales";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Vehicle Sale report...please wait",
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
                            html += "<td>" + record.LocalId + "</td>";
                            html += "<td>" + record.ContactNumber + "</td>";
                            html += "<td>" + record.ProductName + "</td>";
                            html += "<td>" + record.SaleType + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td class='align-right'>" + record.SalePrice.format() + "</td>";
                            html += "<td class='align-right'>" + record.Deposit.format() + "</td>";
                            html += "<td class='align-right'>" + record.Balance.format(null, true) + "</td>";
                            html += "</tr>";

                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";

                        var TotalSalePrice = Common.GetInt(Enumerable.From(records).Sum("$.SalePrice"));
                        var TotalDeposit = Common.GetInt(Enumerable.From(records).Sum("$.Deposit"));
                        var TotalBalance = Common.GetInt(Enumerable.From(records).Sum("$.Balance"));
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan = '8' class='align-right'>Total</td>";
                        html += " <td class='align-right'>" + TotalSalePrice.format() + "</td>";
                        html += "<td class='align-right'>" + TotalDeposit.format() + "</td>";
                        html += "<td class='align-right'></td>";
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