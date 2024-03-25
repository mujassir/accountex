
var VatRegister = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    var PageSetting = new Object();

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
            Common.BindSelect(salesman, $("#SalesmanIds"), true)

        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var salesmanId = $("#SalesmanIds").val();
            var includeStockTransfer = $("#IncludeStockTransfer").val();
            includeStockTransfer = Common.GetBool(includeStockTransfer) ? 1 : 0;

            var salesmanIds = $("#SalesmanIds").val();
            if (!Common.isNullOrWhiteSpace(salesmanIds))
                salesmanIds = salesmanIds.join(",");
            else
                salesmanIds = "";
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetVatRegister";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&salesmanIds=" + salesmanIds;
            qs += "&includeStockTransfer=" + includeStockTransfer;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        $("#div-table > table > tbody").html('');
                        var transactions = res.Data.Table;
                        var sales = Enumerable.From(transactions).Where(function (x) { return x.TransactionType == VoucherType.gstsale }).ToArray();
                        var purchases = Enumerable.From(transactions).Where(function (x) { return x.TransactionType == VoucherType.gstpurchase }).ToArray();
                        var stockTransfers = Enumerable.From(transactions).Where(function (x) { return x.TransactionType == VoucherType.vatst }).ToArray();



                        var saleExAmount = Common.GetFloat(Enumerable.From(sales).Sum("$.GrossTotal"));
                        var saleVatAmount = Common.GetFloat(Enumerable.From(sales).Sum("$.GstAmountTotal"));
                        var saleIncAmount = Common.GetFloat(Enumerable.From(sales).Sum("$.NetTotal"));
                        var SaleQtytotal = Common.GetFloat(Enumerable.From(sales).Sum("$.Quantity"));


                        var purchaseExAmount = Common.GetFloat(Enumerable.From(purchases).Sum("$.GrossTotal"));
                        var purchaseVatAmount = Common.GetFloat(Enumerable.From(purchases).Sum("$.GstAmountTotal"));
                        var purchaseIncAmount = Common.GetFloat(Enumerable.From(purchases).Sum("$.NetTotal"));
                        var purchaseQtytotal = Common.GetFloat(Enumerable.From(purchases).Sum("$.Quantity"));


                        var stockerTransferExAmount = Common.GetFloat(Enumerable.From(stockTransfers).Sum("$.GrossTotal"));
                        var stockerTransferVatAmount = Common.GetFloat(Enumerable.From(stockTransfers).Sum("$.GstAmountTotal"));
                        var stockerTransferIncAmount = Common.GetFloat(Enumerable.From(stockTransfers).Sum("$.NetTotal"));
                        var stockTransferQtytotal = Common.GetFloat(Enumerable.From(stockTransfers).Sum("$.Quantity"));

                        for (var j in sales) {
                            var sale = sales[j];
                            html += "<tr>";
                            html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + sale.InvNumber + "</td>";
                            html += "<td>" + sale.BookNo + "</td>";
                            html += "<td>" + sale.AccountName + "</td>";
                            html += "<td>" + sale.SalesmanName + "</td>";
                            html += "<td class='align-right'>" + sale.Quantity.format(3) + "</td>";
                            html += "<td class='align-right'>" + sale.Rate.format(3) + "</td>";
                            html += "<td>" + sale.GrossTotal.format(3) + "</td>";
                            html += "<td>" + sale.GstAmountTotal.format(3) + "</td>";
                            html += "<td>" + sale.NetTotal.format(3) + "</td>";
                            html += "</tr>";
                        }
                        //grand total
                        html += "<tr class='bold'>";
                        html += "<td colspan='5'>Total</td>";
                        html += "<td>" + SaleQtytotal.format(3) + "</td>";
                        html += "<td></td>";
                        html += "<td>" + saleExAmount.format(3) + "</td>";
                        html += "<td>" + saleVatAmount.format(3) + "</td>";
                        html += "<td>" + saleIncAmount.format(3) + "</td>";
                        html += "</tr>";
                        $(".report-table.sales tbody").html(html);

                        //Purchase Register
                        html = "";
                        for (var j in purchases) {
                            var purchase = purchases[j];
                            html += "<tr>";
                            html += "<td>" + moment(purchase.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + purchase.InvNumber + "</td>";
                            html += "<td>" + purchase.BookNo + "</td>";
                            html += "<td>" + purchase.AccountName + "</td>";
                            html += "<td>" + purchase.SalesmanName + "</td>";
                            html += "<td class='align-right'>" + purchase.Quantity.format(3) + "</td>";
                            html += "<td class='align-right'>" + purchase.Rate.format(3) + "</td>";
                            html += "<td>" + purchase.GrossTotal.format(3) + "</td>";
                            html += "<td>" + purchase.GstAmountTotal.format(3) + "</td>";
                            html += "<td>" + purchase.NetTotal.format(3) + "</td>";
                            html += "</tr>";
                        }
                        //grand total
                        html += "<tr class='bold'>";
                        html += "<td colspan='5'>Total</td>";
                        html += "<td>" + purchaseQtytotal.format(3) + "</td>";
                        html += "<td></td>";
                        html += "<td>" + purchaseExAmount.format(3) + "</td>";
                        html += "<td>" + purchaseVatAmount.format(3) + "</td>";
                        html += "<td>" + purchaseIncAmount.format(3) + "</td>";
                        html += "</tr>";
                        $(".report-table.purchases tbody").html(html);


                        //Stock Transfer
                        if (includeStockTransfer > 0) {
                            html = "";
                            for (var j in stockTransfers) {
                                var stockTransfer = stockTransfers[j];
                                html += "<tr>";
                                html += "<td>" + moment(stockTransfer.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td>" + stockTransfer.InvNumber + "</td>";
                                html += "<td>" + stockTransfer.BookNo + "</td>";
                                html += "<td>" + stockTransfer.AccountName + "</td>";
                                html += "<td>" + stockTransfer.SalesmanName + "</td>";
                                html += "<td class='align-right'>" + stockTransfer.Quantity.format(3) + "</td>";
                                html += "<td class='align-right'>" + stockTransfer.Rate.format(3) + "</td>";
                                html += "<td>" + stockTransfer.GrossTotal.format(3) + "</td>";
                                html += "<td>" + stockTransfer.GstAmountTotal.format(3) + "</td>";
                                html += "<td>" + stockTransfer.NetTotal.format(3) + "</td>";
                                html += "</tr>";
                            }
                            //grand total
                            html += "<tr class='bold'>";
                            html += "<td colspan='5'>Total</td>";
                            html += "<td>" + stockTransferQtytotal.format(3) + "</td>";
                            html += "<td></td>";
                            html += "<td>" + stockerTransferExAmount.format(3) + "</td>";
                            html += "<td>" + stockerTransferVatAmount.format(3) + "</td>";
                            html += "<td>" + stockerTransferIncAmount.format(3) + "</td>";
                            html += "</tr>";
                            $(".report-table.stock-transfer tbody").html(html);
                            $("#stock-transfer-container").removeClass("hide");
                        }
                        else {
                            $("#stock-transfer-container").addClass("hide");
                        }



                        //VAT Summary



                        html = "";
                        html += "<tr>";
                        html += "<td>Total Sales</td>";
                        html += "<td>" + saleExAmount.format(3) + "</td>";
                        html += "<td>" + saleVatAmount.format(3) + "</td>";
                        html += "<td>" + saleIncAmount.format(3) + "</td>";
                        html += "</tr>";

                        html += "<tr>";
                        html += "<td>Stock Transfer</td>";
                        html += "<td>" + stockerTransferExAmount.format(3) + "</td>";
                        html += "<td>" + stockerTransferVatAmount.format(3) + "</td>";
                        html += "<td>" + stockerTransferIncAmount.format(3) + "</td>";
                        html += "</tr>";


                        html += "<tr>";
                        html += "<td>Total Purchase</td>";
                        html += "<td>" + purchaseExAmount.format(3) + "</td>";
                        html += "<td>" + purchaseVatAmount.format(3) + "</td>";
                        html += "<td>" + purchaseIncAmount.format(3) + "</td>";
                        html += "</tr>";

                        html += "<tr class='bold'>";
                        html += "<td>Difference</td>";
                        html += "<td>" + (saleExAmount - (purchaseExAmount + stockerTransferExAmount)).format(3) + "</td>";
                        html += "<td>" + (saleVatAmount - (purchaseVatAmount + stockerTransferVatAmount)).format(3) + "</td>";
                        html += "<td>" + (saleIncAmount - (purchaseIncAmount + stockerTransferIncAmount)).format(3) + "</td>";
                        html += "</tr>";

                        //vat payable final
                        html += "<tr class='bold' rowspan='2'>";
                        html += "<td colspan='4'><h3>VAT Payable: " + (saleVatAmount - (purchaseVatAmount + stockerTransferVatAmount)).format(3) + "</h3></td>";
                        html += "</tr>";


                        $(".report-table.summary tbody").html(html);


                        //stock Summary
                        html = "";
                        var stockSummary = res.Data.Table1;
                        for (var j in stockSummary) {
                            var sm = stockSummary[j];
                            html += "<tr>";
                            html += "<td>" + sm.Name + "</td>";
                            html += "<td class='align-right'>" + sm.OBQty + "</td>";
                            html += "<td class='align-right'>" + sm.PurchaseQty + "</td>";
                            html += "<td class='align-right'>" + sm.SaleQty + "</td>";
                            html += "<td class='align-right'>" + sm.Balance + "</td>";

                            html += "</tr>";
                        }
                        $(".report-table.stock-summary tbody").html(html);


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