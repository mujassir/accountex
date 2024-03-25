
var VehicleInstallmentDetail = function () {
    var API_CONTROLLER = "Report";
    var excelFile = "";
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
        },
        LoadData: function () {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var qs = "?key=GetVehicleInstallmentDetail";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        $("#tbldetail tbody").html("");
                        for (var i in data) {
                            var item = data[i];
                            html += "<tr>";
                            html += "<td>" + item.VoucherNumber + "</td>";
                            html += "<td>" + item.SalePrice + "</td>";
                            html += "<td>" + item.Received + "</td>";
                            html += "<td>" + item.Balance + "</td>";
                            html += "<td>" + item.Color + "</td>";
                            html += "<td>" + item.ChassisNo + "</td>";
                            html += "<td>" + item.EngineNo + "</td>";
                            html += "<td>" + item.InstalmentNo + "</td>";
                            html += "<td>" + item.Amount + "</td>";
                            html += "<td>" + item.RecievedAmount + "</td>";
                            html += "<td>" + item.Discount + "</td>";
                            html += "</tr>";
                        }
                        $("#tbldetail tbody").html(html);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

    };
}();