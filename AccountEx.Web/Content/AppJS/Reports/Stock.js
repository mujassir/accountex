
var Stock = function () {
    var apiController = "Stock";
    var DECIMAL_POINTS_FOR_TOTALS = 0;
    var DECIMAL_POINTS_FOR_STOCKTOTALS = 4;
    return {
        init: function () {
            var $this = this;
            $this.LoadStock();
            $("#btnShowReport").click($this.LoadStock);
            $("#AccountId").change($this.LoadStock);
            $("#GroupName").change($this.FilterByGroup);
            $("input[name='stock-type']").change($this.FilterByGroup);

            $('#txtSearch').keyup(function () {
                filterTable(document.getElementById("txtSearch"), document.getElementById("tbldetail"));
            });

        },
        FilterByGroup: function () {

            var group = $("#GroupName option:selected").text();
            if (group == undefined || group == "" || group.trim() == "All Groups") {
                $("#tbldetail tbody tr").removeClass("hide");
            }
            else {
                var gr = group.replace(/\s+/g, "-").replace(/\\|\//g, "-").toLowerCase();
                $("#tbldetail tbody tr:not(.grouptr" + gr + ")").addClass("hide");
                $("#tbldetail tbody tr.grouptr" + gr).removeClass("hide");
            }
            if ($("input[value='stock']").is(":checked")) {
                $(".stock-value,.avg-sale-rate").removeClass("hide");
                $(".book-value,.unit-value,.avg-purchase-rate,.unit-price").addClass("hide");
            }
            else if ($("input[value='book']").is(":checked")) {
                $(".stock-value,.unit-value,.avg-sale-rate,.unit-price").addClass("hide");
                $(".book-value,.avg-purchase-rate").removeClass("hide");
            }
            else if ($("input[value='unit']").is(":checked")) {
                $(".stock-value,.book-value,.avg-sale-rate,.avg-purchase-rate").addClass("hide");
                $(".unit-value,.unit-price").removeClass("hide");
            }
        },
        LoadStock: function () {
            var $this = this;
            var fromdate = $("#FromDate").val();
            var todate = $("#ToDate").val();
            //$("#lblReportDate").html("Date: " + date);
            $("#lblReportDate").html("Date: " + fromdate + " to " + todate);
            var qs = "?key=GetStock";
            qs += "&fromdate=" + fromdate;
            qs += "&todate=" + todate;
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  stock...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        var j = 1;
                        var group = "";
                        var stockbyGroups = Enumerable.From(data).GroupBy("$.GroupName", null,
             function (key, g) {
                 var result = {
                     GroupName: key,
                     TotalInitial: g.Sum("$.Initial"),
                     TotalStockIn: g.Sum("$.StockIn"),
                     TotalStockOut: g.Sum("$.StockOut"),
                     TotalBalance: g.Sum("$.Balance"),
                     TotalStockValue: g.Sum("$.StockValue"),
                     TotalBookValue: g.Sum("$.BookValue"),
                     TotalUnitValue: g.Sum("$.UnitValue"),
                     Stocks: g.OrderBy("$.Code").ToArray()
                 }
                 return result;
             }).ToArray();

                        for (var i in stockbyGroups) {
                            var group = "";
                            var stockbyGroup = stockbyGroups[i];
                            if (stockbyGroup.GroupName != undefined && stockbyGroup.GroupName != null && stockbyGroup.GroupName != "null")
                                group = stockbyGroup.GroupName.replace(/\\|\//g, "-");
                            html += "<tr class='grouptr" + group.replace(/\s+/g, "-").toLowerCase() + " group-tr'><td colspan='8' class='group'>" + group + "</td></tr>";
                            var stocks = stockbyGroup.Stocks;
                            for (var i in stocks) {
                                var product = stocks[i];
                                html += "<tr class='grouptr" + group.replace(/\s+/g, "-").toLowerCase() + "'>";
                                html += "<td>" + product.Code + "</td>";
                                html += "<td>" + product.Name + "</td>";
                                if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "1") {
                                    html += "<td>" + (product.ArticleNo != null ? product.ArticleNo : "") + "</td>";
                                    html += "<td>" + (product.Location != null ? product.Location : "") + "</td>";
                                }
                                else if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "2") {
                                    html += "<td>" + (product.ArticleNo != null ? product.ArticleNo : "") + "</td>";
                                    html += "<td>" + (product.Location != null ? product.Location : "") + "</td>";
                                    html += "<td>" + (product.SalePrice != null ? product.SalePrice : "") + "</td>";
                                }
                                if (typeof Url.currentStock == 'undefined' && Url.currentStock == null) {
                                    html += "<td class='align-right'>" + product.Initial.format() + "</td>";
                                    html += "<td class='align-right'>" + product.StockIn.format() + "</td>";
                                    html += "<td class='align-right'>" + product.StockOut.format() + "</td>";
                                }
                                html += "<td class='align-right'>" + product.Balance.format() + "</td>";
                                if (typeof Url.type == 'undefined' && Url.type == null) {
                                    html += "<td class='avg-sale-rate hide align-right'>" + product.AvgSaleRate.format() + "</td>";
                                    html += "<td class='avg-purchase-rate align-right'>" + product.AvgPurchaseRate.format() + "</td>";
                                    html += "<td class='unit-price hide align-right'>" + product.UnitPrice.format() + "</td>";
                                  

                                    //with sale price and avg sale rate
                                    html += "<td class='stock-value align-right num3'>" + Common.GetInt(product.StockValue).format() + "</td>";

                                    //with purchase price
                                    html += "<td class='book-value hide align-right num3'>" + product.BookValue.format() + "</td>";

                                    //with unit price from product opening balance
                                    html += "<td class='unit-value hide align-right num3'>" + product.UnitValue.format() + "</td>";

                                }
                                html += "</tr>";
                            }
                            html += "<tr class='bold subtotal grouptr" + group.replace(/\s+/g, "-").toLowerCase() + "'>";
                            if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "1") {
                                html += "<td colspan='4'>Total</td>";
                            }
                            else if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "2") {
                                html += "<td colspan='5'>Total</td>";
                            }
                            else {

                                html += "<td colspan='2'>Total</td>";
                            }
                            if (typeof Url.currentStock == 'undefined' && Url.currentStock == null) {
                                html += "<td class='align-right'>" + stockbyGroup.TotalInitial.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalStockIn.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalStockOut.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                            }
                            html += "<td class='align-right'>" + stockbyGroup.TotalBalance.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                            if (typeof Url.type == 'undefined' && Url.type == null) {
                                html += "<td></td>";
                                html += "<td class='stock-value align-right num3'>" + Common.GetInt(stockbyGroup.TotalStockValue).format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                                html += "<td class='book-value hide align-right num3'>" + stockbyGroup.TotalBookValue.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                                html += "<td class='unit-value hide align-right num3'>" + stockbyGroup.TotalUnitValue.format(DECIMAL_POINTS_FOR_TOTALS) + "</td>";
                            }
                            html += "</tr>";

                        }

                        var initialTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.Initial")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var stockinTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockIn")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var stockoutTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockOut")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var balanceTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.Balance")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var stockvalueTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockValue")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var bookvalueTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.BookValue")).format(DECIMAL_POINTS_FOR_TOTALS);
                        var unitvalueTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.UnitValue")).format(DECIMAL_POINTS_FOR_TOTALS);
                        html += "<tr class='bold grand-total'>";
                        if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "1") {
                            html += "<td colspan='4'>Total</td>";
                        }
                        else if (typeof Url.format != 'undefined' && Url.format != null && Url.format == "2") {
                            html += "<td colspan='5'>Total</td>";
                        }
                        else {
                            html += "<td colspan='2'>Total</td>";
                        }
                        if (typeof Url.currentStock == 'undefined' && Url.currentStock == null) {
                            html += "<td class='align-right'>" + initialTotal + "</td>";
                            html += "<td class='align-right'>" + stockinTotal + "</td>";
                            html += "<td class='align-right'>" + stockoutTotal + "</td>";
                        }
                        html += "<td class='align-right'>" + balanceTotal + "</td>";
                        if (typeof Url.type == 'undefined' && Url.type == null) {
                            html += "<td></td>";
                            html += "<td class='stock-value align-right num3' >" + stockvalueTotal + "</td>";
                            html += "<td class='book-value hide align-right num3' >" + bookvalueTotal + "</td>";
                            html += "<td class='unit-value hide align-right num3' >" + unitvalueTotal + "</td>";
                        }
                        html += "</tr>";

                        $("#tbldetail tbody").html(html);
                        Stock.FilterByGroup();
                        //html = "<tr><td colspan='2'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td></tr>";
                        //html += "<tr><td colspan='2'></td><td style='text-align: center;' colspan='2' >Difference = " + res.Data.Difference + "</td></tr>";
                        //$("#tbldetail tfoot").html(html);

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