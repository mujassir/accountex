
var Repossession = function () {
    var API_CONTROLLER = "VehicleReport";
    return {
        init: function () {
            var $this = this;
            $this.LoadData();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#BranchId").change(function () {
                $this.LoadData();
            });
        },
        LoadData: function () {
            var branchId = $("#BranchId").val();
            var recoveryStatus = $("#RecoveryStatus").val();
            var qs = "?key=GetVehicleRepossessions";
            qs += "&branchId=" + branchId;
            qs += "&recoveryStatus=" + recoveryStatus;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Repossessions...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var records = res.Data;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";
                            html += "<td>" + Common.FormatDate(record.LetterDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + (record.Customer + "," + record.LocalId + "," + record.ContactNumber) + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td>" + record.Auctionner + "</td>";
                            html += "<td>" + record.LetterIssued + "</td>";
                            html += "<td>" + record.Repossessed + "</td>";
                            html += "<td>" + Common.FormatDate(record.RepossessedDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.PendingRepossessed + "</td>";
                            html += "<td>" + record.NotificationDays + "</td>";
                            html += "<td>" + record.Newspaper + "</td>";
                            html += "<td>" + Common.FormatDate(record.AdvertisementDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.InventoryReturn + "</td>";
                            html += "<td>" + record.Settlement + "</td>";
                            html += "<td>" + record.AgreementRemarks + "</td>";
                            html += "<td>" + record.Resold + "</td>";
                            html += "</tr>";
                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='16' style='text-align: center'>No record(s) found</td></tr>";

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