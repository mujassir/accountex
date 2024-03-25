
var Register = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            PageSetting = Common.LoadPageSetting();
            $this.BindSalesMan();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            var type = Common.GetQueryStringValue("type").toLowerCase();
            if (type == "sale") {

                $(".main-title").html("Sale Register");
            }
            else if (type == "salereturn") {

                $(".main-title").html("Sale Return Register");
            }
            else if (type == "purchase") {

                $(".main-title").html("Purchase Register");
            }
            else if (type == "purchasereturn") {

                $(".main-title").html("Purchase Return Register");
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
        BindSalesMan: function (key) {
            var $this = this;
            var salesman = Common.GetLeafAccounts(PageSetting.Salesman);
            Common.BindSelect(salesman, $("#SalesmanId"), true)

        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var salesmanId = $("#SalesmanId").val();
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetPartyTrans";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[type];
            qs += "&accountId=0";
            qs += "&SalesmanId=" + salesmanId;
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
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var account = "";
                        var sales = data;
                        for (var j in sales) {
                            var sale = sales[j];
                            var saleItems = sale.SaleItems;
                            var url = Common.GetTransactionUrl(sale.TransactionType, sale.VoucherNumber);
                            var vType = VoucherTypes[sale.TransactionType];
                            html += "<tr data-row='detail' class='hide'>";
                            html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + sale.VoucherNumber + "</td>";
                            html += "<td>" + sale.InvoiceNumber + "</td>";
                            html += "<td>" + sale.AccountName + "</td>";
                            html += "<td colspan='6'></td>";
                            html += "</tr>";



                            for (var k in saleItems) {
                                var item = saleItems[k];
                                html += "<tr data-row='detail' class='hide'>";
                                html += "<td colspan='4'></td>";
                                html += "<td>" + item.ItemName + "</td>";
                                html += "<td>" + item.Quantity + "</td>";


                                if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                    html += "<td style='text-align:left'>" + item.Rate + "</td>";
                                    html += "<td colspan='2' style='text-align:left'>" + Common.GetFloat(item.Amount).format() + "</td>";
                                    //html += "<td>" + Common.GetFloat(item.DiscountAmount).format() + "</td>";
                                }
                                else {
                                    html += "<td>" + item.Rate + "</td>";
                                    html += "<td>" + Common.GetFloat(item.Amount).format() + "</td>";
                                    html += "<td>" + Common.GetFloat(item.GSTAmount).format() + "</td>";
                                }
                                html += "<td>" + Common.GetFloat(item.NetAmount).format() + "</td></tr>";
                            }
                            var qtyTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.Quantity"));
                            var amountTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.Amount")).format();
                            var discountTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.DiscountAmount")).format();
                            var gstTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.GSTAmount")).format();
                            var netTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.NetAmount")).format();
                            html += "<tr data-row='detail' class='bold subtotal hide'><td colspan=4></td>";
                            html += "<td>Voucher Total</td><td>" + qtyTotal + "</td>";

                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td></td><td style='text-align:left'>" + sale.GrossTotal + "</td>";
                                //html += "<td>" + discountTotal + "</td>";
                                html += "<td>" + sale.Discount + "</td>";
                                html += "<td>" + sale.NetTotal + "</td></tr>";
                            }
                            else {
                                html += "<td></td><td>" + amountTotal + "</td>";
                                html += "<td>" + gstTotal + "</td>";
                                html += "<td>" + sale.NetTotal + "</td></tr>";
                            }
                            //Summary row
                            html += "<tr data-row='summary' class='hide'>";
                            html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + sale.VoucherNumber + "</td>";
                            html += "<td>" + sale.InvoiceNumber + "</td>";
                            html += "<td>" + sale.AccountName + "</td>";
                            html += "<td>" + qtyTotal + "</td>";
                            html += "<td>" + sale.GrossTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {

                                html += "<td>" + sale.Discount + "</td>";

                            }
                            else {

                                html += "<td>" + gstTotal + "</td>";
                            }
                            html += "<td>" + sale.NetTotal + "</td></tr>";

                        }



                        if (res.Data.Records.length == 0) {
                            html += "  <tr data-row='detail' class='hide'><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";
                            html += "  <tr data-row='summary' class='hide'><td colspan='8' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        else {
                            var qtyTotal = Common.GetFloat(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetFloat(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Amount")).format();
                            var discountTotal = Common.GetFloat(Enumerable.From(data).Sum("$.Discount")).format();
                            var gstTotal = Common.GetFloat(Enumerable.From(data).Sum("$.GstAmountTotal")).format();
                            var netTotal = Common.GetFloat(Enumerable.From(data).Sum("$.NetTotal")).format();
                            html += "<tr data-row='detail' style='background-color:skyblue' class='bold grand-total'><td colspan=3></td>";
                            html += "<td colspan=2>Grand Total</td><td>" + qtyTotal + "</td>";
                            html += "<td></td><td>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td>" + gstTotal + "</td>";
                            }
                            html += "<td>" + netTotal + "</td></tr>";
                            //Summary row
                            html += "<tr data-row='summary' style='background-color:skyblue' class='bold grand-total'><td colspan=3></td>";
                            html += "<td colspan=1>Grand Total</td><td>" + qtyTotal + "</td>";
                            html += "<td>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td>" + gstTotal + "</td>";
                            }
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