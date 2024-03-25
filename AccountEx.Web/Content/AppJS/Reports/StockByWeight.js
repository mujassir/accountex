
var StockByWeight = function () {
    var apiController = "Stock";
    return {
        init: function () {
            var $this = this;
            $this.LoadStock();
            $("#btnShowReport").click($this.LoadStock);
            $("#AccountId").change($this.LoadStock);
            $("#GroupName").change($this.FilterByGroup);
            $("input[name='stock-type']").change($this.FilterByGroup);

        },
        FilterByGroup: function () {

            var group = $("#GroupName option:selected").text();
            if (group == undefined || group == "" || group.trim() == "All Groups") {
                $("#tbldetail tbody tr").removeClass("hide");
            }
            else {
                var gr = group.replace(/\s+/g, "-").toLowerCase();
                $("#tbldetail tbody tr:not(.group" + gr + ")").addClass("hide");
                $("#tbldetail tbody tr.group" + gr).removeClass("hide");
            }
            if ($("input[value='stock']").is(":checked")) {
                $(".stock-value").removeClass("hide");
                $(".book-value").addClass("hide");

            }
            else {
                $(".stock-value").addClass("hide");
                $(".book-value").removeClass("hide");
            }
        },
        LoadStock: function () {
            var $this = this;
            var fromdate = $("#FromDate").val();
            var todate = $("#ToDate").val();
            //$("#lblReportDate").html("Date: " + date);
            $("#lblReportDate").html("Date: " + fromdate + " to " + todate);
            var qs = "?key=GetStockByWeight";
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
                     Stocks: g.ToArray()
                 }
                 return result;
             }).ToArray();

                        for (var i in stockbyGroups) {
                            var group = "";
                            var stockbyGroup = stockbyGroups[i];
                            if (stockbyGroup.GroupName != undefined && stockbyGroup.GroupName != null && stockbyGroup.GroupName != "null") {
                                group = stockbyGroup.GroupName;
                                html += "<tr class='group" + group.replace(/\s+/g, "-").toLowerCase() + " group-tr'><td colspan='8' class='group'>" + group + "</td></tr>";
                                var stocks = stockbyGroup.Stocks;
                                for (var i in stocks) {
                                    var product = stocks[i];
                                    html += "<tr>";
                                    html += "<td>" + product.Code + "</td>";
                                    html += "<td>" + product.Name + "</td>";
                                    html += "<td class='align-right'>" + product.Initial.format() + "</td>";
                                    html += "<td class='align-right'>" + product.StockIn.format() + "</td>";
                                    html += "<td class='align-right'>" + product.StockOut.format() + "</td>";
                                    html += "<td class='align-right'>" + product.Balance.format() + "</td>";
                                    if (typeof Url.type == 'undefined' && Url.type == null) {
                                        html += "<td class='align-right'>" + product.AvgRate.format() + "</td>";
                                        html += "<td class='stock-value align-right num3'>" + Common.GetInt(product.StockValue).format() + "</td>";
                                        html += "<td class='book-value hide align-right num3'>" + product.BookValue.format() + "</td>";
                                    }
                                    html += "</tr>";
                                }
                                html += "<tr class='bold subtotal'>";
                                html += "<td colspan='2'>Total</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalInitial.format() + "</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalStockIn.format() + "</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalStockOut.format() + "</td>";
                                html += "<td class='align-right'>" + stockbyGroup.TotalBalance.format() + "</td>";
                                if (typeof Url.type == 'undefined' && Url.type == null) {
                                    html += "<td></td>";
                                    html += "<td class='stock-valuealign-right num3'>" + Common.GetInt(stockbyGroup.TotalStockValue).format() + "</td>";
                                    html += "<td class='book-value hide align-right num3'>" + stockbyGroup.TotalBookValue.format() + "</td>";
                                }
                                html += "</tr>";
                            }
                        }

                        var initialTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.Initial")).format();
                        var stockinTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockIn")).format();
                        var stockoutTotal = Common.GetFloat(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockOut")).format();
                        var balanceTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.Balance")).format();
                        var stockvalueTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.StockValue")).format();
                        var bookvalueTotal = Common.GetInt(Enumerable.From(stockbyGroups).SelectMany("$.Stocks").Sum("$.BookValue")).format();
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan='2'>Total</td>";
                        html += "<td class='align-right'>" + initialTotal + "</td>";
                        html += "<td class='align-right'>" + stockinTotal + "</td>";
                        html += "<td class='align-right'>" + stockoutTotal + "</td>";
                        html += "<td class='align-right'>" + balanceTotal + "</td>";
                        if (typeof Url.type == 'undefined' && Url.type == null) {
                            html += "<td></td>";
                            html += "<td class='stock-value align-right num3'>" + stockvalueTotal + "</td>";
                            html += "<td class='book-value hide align-right num3'>" + bookvalueTotal + "</td>";
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