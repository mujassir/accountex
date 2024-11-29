
var InternalStockTransfer = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            PageSetting = Common.LoadPageSetting();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            $("#ReportType").change(function () {
                var type = $(this).val();
                if (type.toLocaleLowerCase() == "summary") {
                    $("tr[data-row='summary']").removeClass("hide");
                    $("tr[data-row='detail']").addClass("hide");
                }
                else {
                    $("tr[data-row='summary']").addClass("hide");
                    $("tr[data-row='detail']").removeClass("hide");
                }
                // Common.InitMultiSelectWithTable("column-hide-show-container", "mainTable");
            });
            // Common.InitMultiSelectWithTable("column-hide-show-container", "mainTable");
        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var qs = "?key=InternalStockTransfer";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&accountId=0";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  party wise" + " " + Common.GetQueryStringValue("type").toLowerCase() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        const vouchers = [...new Map(data.map(item => [item.VoucherNumber, item])).values()];
                        for (var voucher in vouchers) {
                            const item = vouchers[voucher];
                            const items = data.filter(e => e.VoucherNumber === item.VoucherNumber);
                            const totalQty = items.map(e => e.Quantity).reduce((accumulator, current) => accumulator + current);
                            html += "<tr data-row='detail' class='hide'>";
                            html += "<td>" + moment(item.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + item.VoucherNumber + "</td>";
                            html += "<td>" + item.FromLocationName + "</td>";
                            html += "<td>" + item.FromWarehouseName + "</td>";
                            html += "<td>" + item.ToLocationName + "</td>";
                            html += "<td>" + item.ToWarehouseName + "</td>";
                            html += "<td></td>";
                            html += "<td></td>";
                            html += "</tr>"

                            for (var key in items) {
                                const product = items[key];
                                html += "<tr data-row='detail' class='hide'>"
                                html += "<td colspan='6'></td>";
                                html += "<td>" + product.ItemCode + " " + product.ItemName + "</td>";
                                html += "<td>" + product.Quantity + "</td>";
                                html += "</tr>";
                            }

                            html += "<tr data-row='detail' class='hide'>"
                            html += "<th colspan='7' class='text-right'>Total:</th>";
                            html += "<th>" + totalQty + "</th>";
                            html += "</tr>";

                            //Summary row
                            html += "<tr data-row='summary' class='hide'>";
                            html += "<td>" + moment(item.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + item.VoucherNumber + "</td>";
                            html += "<td>" + item.FromLocationName + "</td>";
                            html += "<td>" + item.FromWarehouseName + "</td>";
                            html += "<td>" + item.ToLocationName + "</td>";
                            html += "<td>" + item.ToWarehouseName + "</td>";
                            html += "<td>" + totalQty + "</td>";
                            html += "</tr>"
                        }



                        if (res.Data.length == 0) {
                            html += "  <tr data-row='detail' class='hide'><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";
                            html += "  <tr data-row='summary' class='hide'><td colspan='8' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        $(".report-table tbody").html(html);
                        $("#ReportType").trigger("change");
                        $('#column-hide-show-container select').trigger('change');

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