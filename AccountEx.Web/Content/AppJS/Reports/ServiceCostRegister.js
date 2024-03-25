
var ServiceCostRegister = function () {
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
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
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
            var qs = "?key=GetPartyTrans";
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
                blockMessage: "Loading  party wise" + Common.GetQueryStringValue("type").toLowerCase() + " ...please wait",
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
                            html += "<tr>";
                            html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + sale.VoucherNumber + "</td>";
                            //html += "<td>" + sale.InvoiceNumber + "</td>";
                            html += "<td>" + sale.AccountName + "</td>";
                            html += "<td colspan='8'></td>";
                            html += "</tr>";

                            for (var k in saleItems) {
                                var item = saleItems[k];
                                html += "<tr>";
                                html += "<td colspan='3'></td>";
                                html += "<td>" + item.ItemName + "</td>";
                                html += "<td class='align-right'>" + item.Quantity + "</td>";
                                html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                                html += "<td class='align-right'>" + Common.GetInt(item.Amount).format() + "</td>";
                                html += "<td class='align-right'>" + Common.GetInt(item.GSTAmount).format() + "</td>";
                                html += "<td class='align-right'>" + Common.GetInt(item.NetAmount).format() + "</td><td colspan='2'></td></tr>";
                            }
                            var qtyTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Amount")).format();
                            var discountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.DiscountAmount")).format();
                            var gstTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.GSTAmount")).format();
                            var netTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.NetAmount"));

                            var costTotal = Common.GetInt(Enumerable.From(sale.ServiceExpenses).Sum("$.Amount"));
                            var finaltotal = Common.GetInt(netTotal - costTotal);
                            
                            html += "<tr class='bold subtotal'><td colspan='3'></td>";
                            html += "<td class='align-right'>Voucher Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            html += "<td class='align-right'>" + gstTotal + "</td>";
                            html += "<td class='align-right'>" + netTotal.format() + "</td>";
                            html += "<td class='align-right'>" + costTotal.format() + "</td>";
                            html += "<td class='align-right'>" + finaltotal.format() + "</td></tr>";
                        }

                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";
                        else {
                            var qtyTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Amount")).format();
                            var discountTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.DiscountAmount")).format();
                            var gstTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.GSTAmount")).format();
                            var netTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.NetAmount"));

                            var costTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.ServiceExpenses").Sum("$.Amount"));
                            var finaltotal = Common.GetInt(netTotal - costTotal);

                            html += "<tr style='background-color:skyblue' class='bold grand-total'><td colspan=2></td>";
                            html += "<td colspan='2'>Grand Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            html += "<td class='align-right'>" + gstTotal + "</td>";
                            html += "<td class='align-right'>" + netTotal.format() + "</td>";
                            html += "<td class='align-right'>" + costTotal.format() + "</td>";
                            html += "<td class='align-right'>" + finaltotal.format() + "</td></tr>";
                        }
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