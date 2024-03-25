
var ProductionDetail = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            $this.LoadData();
        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetProductionDetail";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[type];
            qs += "&accountId=0";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  production detail...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var vouchers = res.Data;
                        if (vouchers.length == 0) {
                            html += "  <tr><td colspan='7' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        else {

                            for (var j in vouchers) {
                                var voucher = vouchers[j];
                                html += "<tr data-row='detail'>";

                                html += "<td>" + voucher.VoucherNumber + "</td>";
                                html += "<td>" + voucher.InvoiceNumber + "</td>";
                                html += "<td>" + moment(voucher.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td>" + Common.GetInt(voucher.NetTotal).format() + "</td>";
                                html += "<td>" + Common.GetInt(voucher.FinishedNetTotal).format() + "</td>";
                                html += "<td>" + Common.GetInt(voucher.TotalCharges).format() + "</td>";
                                html += "<td>" + Common.GetInt(voucher.Difference).format() + "</td>";
                                html += "</tr>";

                            }

                            var issueTotal = Common.GetInt(Enumerable.From(vouchers).Sum("$.NetTotal"));
                            var finishedTotal = Common.GetInt(Enumerable.From(vouchers).Sum("$.FinishedNetTotal")).format();
                            var finishedTotal = Common.GetInt(Enumerable.From(vouchers).Sum("$.TotalCharges")).format();
                            var differenceTotal = Common.GetInt(Enumerable.From(vouchers).Sum("$.Difference")).format();
                           


                            html += "<tr data-row='summary' style='background-color:skyblue' class='bold grand-total'>";
                            html += "<td colspan='3' class='align-right'>Grand Total</td>";
                            html += "<td>" + issueTotal + "</td>";
                            html += "<td>" + finishedTotal + "</td>";
                            html += "<td>" + finishedTotal + "</td>";
                            html += "<td>" + differenceTotal + "</td>";
                            html += "</tr>";
                        }
                        $("#mainTable tbody").html(html);
                       

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