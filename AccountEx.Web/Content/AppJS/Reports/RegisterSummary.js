
var RegisterSummary = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            var type = Common.GetQueryStringValue("type").toLowerCase();
            if (type == "sale") {

                $(".main-title").html("Sale Summary");
            }
            else if (type == "salereturn") {

                $(".main-title").html("Sale Return Summary");
            }
            else if (type == "purchase") {

                $(".main-title").html("Purchase Summary");
            }
            else if (type == "purchasereturn") {

                $(".main-title").html("Purchase Return Summary");
            }
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
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetRegisterSummary";
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
                blockMessage: "Loading  summary ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var account = "";
                        var sales = data;
                        for (var j in sales) {
                            var sale = sales[j];
                            var saleItems = sale.SaleItems;

                            var qtyTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Amount")).format();
                            var discountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.DiscountAmount")).format();
                            var gstTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.GSTAmount")).format();
                            var netTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.NetAmount")).format();
                            html += "<tr data-row='summary' class='hide'>";
                            html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + sale.GrossTotal + "</td>";
                            html += "<td>" + sale.Discount + "</td>";
                            html += "<td>" + sale.NetTotal + "</td></tr>";

                        }



                        if (res.Data.Records.length == 0) {
                            html += "  <tr data-row='detail' class='hide'><td colspan='4' style='text-align: center'>No record(s) found</td></tr>";
                            html += "  <tr data-row='summary' class='hide'><td colspan='4' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        else {
                            var qtyTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Amount")).format();
                            var grossTotal = Common.GetInt(Enumerable.From(data).Sum("$.GrossTotal")).format();
                            var discountTotal = Common.GetInt(Enumerable.From(data).Sum("$.Discount")).format();
                            var gstTotel = Common.GetInt(Enumerable.From(data).Sum("$.GstAmountTotal")).format();
                            var netTotal = Common.GetInt(Enumerable.From(data).Sum("$.NetTotal")).format();
                            html += "<tr data-row='detail' style='background-color:skyblue' class='bold grand-total'><td ></td>"; //colspan=3
                            html += "<td>Grand Total</td>";
                            html += "<td></td><td>" + grossTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td>" + gstTotal + "</td>";
                            }
                            html += "<td>" + netTotal + "</td></tr>";
                            //Summary row
                            html += "<tr data-row='summary' style='background-color:skyblue' class='bold grand-total'>";//<td colspan=3></td>
                            html += "<td colspan=1>Grand Total</td>";
                            html += "<td>" + grossTotal + "</td>";
                            //if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                            html += "<td>" + discountTotal + "</td>";
                            //}
                            //else {
                            //    html += "<td>" + gstTotal + "</td>";
                            //}
                            html += "<td>" + netTotal + "</td></tr>";
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