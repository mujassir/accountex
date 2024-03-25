
var NTVehicleDetail = function () {
    var API_CONTROLLER = "Report";
    var excelFile = "";
    return {
        init: function () {

            var $this = this;

            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#btnExcelExport").click(function () {
                if (!Common.isNullOrWhiteSpace(excelFile))
                    window.open(excelFile);
            });


        },
        LoadData: function () {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;


            var qs = "?key=GetNTVehicleDetail";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Vehicle Detail...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";

                        var vehicalDetails = res.Data.Vehicles;
                        excelFile = res.Data.FilePath;
                        for (var item in vehicalDetails) {
                            var vehicle = vehicalDetails[item];
                            html += "<tr>";
                            html += "<td>" + vehicle.PartyName + "</td>";

                            html += "<td>" + moment(vehicle.DCDate).format('DD/MM/YYYY'); + "</td>";
                            html += "<td>" + vehicle.Description + "</td>";
                            if (vehicle.VehicleNo == null) vehicle.VehicleNo = '';
                            html += "<td>" + vehicle.VehicleNo + "</td>";

                            html += "<td>" + vehicle.DeliveredTo + "</td>";
                            html += "<td>" + vehicle.GatePassNo + "</td>";
                            html += "<td>" + vehicle.TotalMeters + "</td>";
                            html += "<td>" + vehicle.Kgs + "</td>";
                            html += "</tr>";
                        }

                        $(".report-table tbody").html("");
                        $(".report-table tbody").html(html);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        ExportExcel: function () {
            var $this = this;

            Common.ConvertToPDF("ConvertExcel");


        },
        GetVoucherDetail: function (voucherNo, transactionType, deliveryChallans) {
            var records = Enumerable.From(deliveryChallans).Where(function (p) { return p.VoucherNumber == voucherNo && p.TransactionType == transactionType }).ToArray();
            var html = "<table class='boldR'>";
            for (var i in records) {
                var dc = records[i];
                html += "<tr>";
                html += "<td>By Bill No. " + dc.InvoiceNumber + ":" + dc.ItemCode + "_" + dc.ItemName + ":" + dc.Quantity + "X" + dc.Rate + "+" + dc.GSTAmount + "</td>";
                html += "</tr>";

            }
            html += "</table>";
            return html;
        }


    };
}();