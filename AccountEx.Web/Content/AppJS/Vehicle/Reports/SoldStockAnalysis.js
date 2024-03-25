



var SoldStockAnalysis = function () {
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
            $this.LoadData();
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetSoldStockAnalysis";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading sold Stocks...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var records = res.Data;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";

                            html += "<td>" + record.FileNo + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td>" + record.VehicleName + "</td>";
                            html += "<td>" + record.SupplierName + "</td>";
                            html += "<td>" + Common.FormatDate(record.BLReceivedDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Consignee + "</td>";
                            html += "<td>" + record.ShipName + "</td>";
                            html += "<td>" + record.BLNumber + "</td>";
                            html += "<td>" + record.EngineNo + "</td>";
                            html += "<td>" + record.Color + "</td>";
                            html += "<td>" + record.Year + "</td>";
                            html += "<td>" + record.CC + "</td>";
                            html += "<td>" + record.ClearingAgent + "</td>";
                            html += "<td class='align-right'>" + record.PurchasePrice.format() + "</td>";
                            html += "<td class='align-right'>" + record.DutyTaxes.format() + "</td>";
                            html += "<td class='align-right'>" + record.BLCharges.format() + "</td>";
                            html += "<td class='align-right'>" + record.MiscCharges.format() + "</td>";
                            html += "<td class='align-right'>" + record.TotalCost.format() + "</td>";
                            html += "<td class='align-right'>" + record.SalePrice.format() + "</td>";
                            html += "<td class='align-right'>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                            html += "<td class='align-right'>" + record.Profit.format() + "</td>";
                            html += "<td>" + record.CashHP + "</td>";
                            html += "<td>" + record.Status + "</td>";
                            html += "<td>" + (record.Customer + "," + record.CustomerId + "," + record.ContactNumber) + "</td>";
                            html += "<td>" + record.AgreementRemarks + "</td>";
                            html += "</tr>";
                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='26' style='text-align: center'>No record(s) found</td></tr>";

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