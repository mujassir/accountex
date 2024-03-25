
var ActiveStocks = function () {
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
            $this.LoadData();
        },
        LoadData: function () {          
            var branchId = $("#BranchId").val();           
            var qs = "?key=GetActiveStocks";           
            qs += "&branchId=" + branchId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Active Stocks report...please wait",
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
                            html += "<td>" + record.Status + "</td>";
                            html += "</tr>";

                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='18' style='text-align: center'>No record(s) found</td></tr>";

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