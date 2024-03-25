
var FollowUps = function () {
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
            })
        },
        LoadData: function () {


            var date1 = $("#Date").val();
            var type = $("#Type").val();
            var branchId = $("#BranchId").val();
            var qs = "?key=GetVehicleFollowups";
            qs += "&date1=" + date1;
            qs += "&type=" + type;
            qs += "&branchId=" + branchId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  FollowUps...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var records = res.Data;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";
                            html += "<td class='created-date hide'>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Customer + "</td>";
                            html += "<td>" + record.ContactNumber + "</td>";
                            html += "<td>" + record.ProductName + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td>" + record.Remarks + "</td>";
                            html += "<td>" + Common.FormatDate(record.NextFollowUp, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Auctionner + "</td>";
                            html += "<td>" + Common.FormatDate(record.LetetrDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Status + "</td>";
                            html += "<td>" + record.NotificationDays + "</td>";
                            html += "<td>" + record.AdvertisementDays + "</td>";
                            html += "<td>" + record.LogBookStatus + "</td>";
                            html += "</tr>";
                        }


                        if (records.length == 0)
                            html += "  <tr><td colspan='14' style='text-align: center'>No record(s) found</td></tr>";

                        $(".report-table tbody").html(html);
                        if (type == "Daily")
                            $(".created-date").removeClass("hide");
                        else
                            $(".created-date").addClass("hide");
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