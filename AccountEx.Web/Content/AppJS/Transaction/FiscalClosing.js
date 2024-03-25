var FiscalClosing = function () {
    var API_CONTROLLER = "Report";
    var FISCAL_CONTROLLER = "Fiscal";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnCloseFiscalYear").click(this.CloseFiscalYear);
            $("#btnOpenFiscalYear").click(this.OpenFiscalYear);
            $this.LoadData();
        },
        CloseFiscalYear: function () {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + FISCAL_CONTROLLER,
                type: "Get",
                data: { closeFiscalYear: true },
                blockUI: true,
                blockElement: ".form",
                blockMessage: "Closing fiscal year...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Fiscal year closed successfully!" });
                        $(".close-fiscal,.open-fiscal").toggleClass("hide");
                        FiscalClosing.LoadData();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                }
            });

        },
        OpenFiscalYear: function () {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + FISCAL_CONTROLLER,
                type: "Get",
                data: { id: 1, openFiscalYear: true },
                blockUI: true,
                blockElement: ".form",
                blockMessage: "opening fiscal year...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Fiscal year reopend successfully!" });
                        $(".close-fiscal,.open-fiscal").toggleClass("hide");
                        FiscalClosing.LoadData();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                }
            });

        },
        LoadAccounts: function () {
            var $this = this;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = 0;
            if (type == "sale") {
                id = PageSetting.Customers;
                $(".main-title").html("Customer Wise Sale");
            }
            else if (type == "salereturn") {
                id = PageSetting.Customers;
                $(".main-title").html("Customer Wise Sale Return");
            }
            else if (type == "purchase") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Supplier Wise Purchase");
            }
            else if (type == "purchasereturn") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Supplier Wise Purchase Return");
            }


            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        Name: token.DisplayName,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    $("#AccountName").val(ui.item.Name);
                    $("#AccountId").val(ui.item.id);
                    $("input[name='all-product']").prop("checked", false);
                    $.uniform.update();
                }
            });



        },
        LoadPageSetting: function () {

            var tokens = $.parseJSON($("#FormSetting").val());

            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            var type = Common.GetQueryStringValue("type");

            this.LoadAccounts();

        },
        LoadData: function () {

            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var date1 = $("#FromDate").val();
                var date2 = $("#ToDate").val();
                $("#lblReportDate").html("Date: " + date1 + " to " + date2);
                var qs = "?key=GetProfitLoss";
                qs += "&date1=" + date1;
                qs += "&date2=" + date2;
                qs += "&voutype=" + VoucherType[Common.GetQueryStringValue("type")];
                //qs += "&OpeningStock=" + $("#OpeningStock").val();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var html = "";
                            var select = "";
                            var amount = 0;
                            var data = res.Data.Profits[0];

                            //var data = Enumerable.From(data).Where("$.SubType==Sales").ToArray();
                            html += "<tr data-row='detail' class='bold sub-head'><td colspan='2'>Detail of Sales</td></tr>";
                            var commerciaSalesTotal = Enumerable.From(data).Where("$.SubType=='Sales'").Sum("$.Amount");

                            html += "<tr data-row='detail'><td>(+) Sales Estimate</td><td>" + Common.GetInt(commerciaSalesTotal).format(); + "</td></tr>";

                            var gstSalesTotal = Enumerable.From(data).Where("$.SubType=='GSTSales'").Sum("$.Amount");
                            html += "<tr data-row='detail'><td>(+) Sales GST</td><td>" + Common.GetInt(gstSalesTotal).format(); + "</td></tr>";

                            var servicesTotal = Enumerable.From(data).Where("$.SubType=='Services'").Sum("$.Amount");
                            html += "<tr data-row='detail'><td>(+) Sales Services</td><td>" + Common.GetInt(servicesTotal).format(); + "</td></tr>";
                            var salesTotal = commerciaSalesTotal + gstSalesTotal + servicesTotal;
                            html += "<tr class='bold sub-total' data-row='detail'><td>Total Sales</td><td>" + Common.GetInt(salesTotal).format(); + "</td></tr>";
                            html += "<tr data-row='summary'><td>Sales</td><td>" + Common.GetInt(salesTotal).format(); + "</td></tr>";

                            html += "<tr  class='bold sub-head' data-row='detail'><td colspan='2'>Cost of Goods Sold</td></tr>";

                            var purchaseTotal = Enumerable.From(data).Where("$.SubType=='Purchase'").Sum("$.Amount");
                            var openingStockTotal = Enumerable.From(data).Where("$.SubType=='OpeningStock'").Sum("$.Amount");
                            var closingStockTotal = Enumerable.From(data).Where("$.SubType=='ClosingStock'").Sum("$.Amount") * -1;

                            var goodsSoldsTotal = purchaseTotal + openingStockTotal + closingStockTotal;
                            html += "<tr data-row='detail'><td>(+) Opening Stock</td><td>" + Common.GetInt(openingStockTotal).format(); + "</td></tr>";
                            html += "<tr data-row='detail'><td>(+) Purchases</td><td>" + Common.GetInt(purchaseTotal).format(); + "</td></tr>";
                            html += "<tr data-row='detail'><td>(-) Closing Stock</td><td>" + Common.GetInt(closingStockTotal).format(); + "</td></tr>";


                            html += "<tr data-row='detail' class='bold sub-total'><td>Total Cost of Goods Sold</td><td>" + Common.GetInt(goodsSoldsTotal).format(); + "</td></tr>";
                            html += "<tr data-row='summary'><td>Cost of Goods Sold</td><td>" + Common.GetInt(goodsSoldsTotal).format(); + "</td></tr>";
                            var profitTotal = Common.GetInt(salesTotal + goodsSoldsTotal);
                            html += "<tr class='grand-total'><td><strong>Gross Profit</strong></td><td><strong>" + Common.GetInt(profitTotal).format(); + "</strong></td></tr>";
                            var profitPerrcent = Common.GetFloat((profitTotal / salesTotal) * 100).format();
                            html += "<tr><td><strong>Gross Profit %</strong></td><td><strong>" + profitPerrcent + "</strong></td></tr>";
                            $("#table-goodsold tbody").html(html);
                            html = "";
                            data = res.Data.Expenses[0];
                            for (var j in data) {
                                var ac = data[j];
                                var account = Common.GetById(ac.AccountId);
                                data[j]["ParentId"] = account.ParentId;
                            }
                            var data = Enumerable.From(data).GroupBy("$.ParentId", null,
                    function (key, g) {
                        var result = {
                            ParentId: key,
                            Account: Common.GetById(key),
                            Expenses: g.ToArray(),
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
                            for (var i in data) {
                                var groupExpenses = data[i];
                                var expenses = groupExpenses.Expenses;
                                var account = groupExpenses.Account;
                                html += "<tr data-row='detail' class='bold sub-head'><td colspan='2'>" + account.DisplayName + "</td></tr>";
                                html += "<tr data-row='summary'><td>" + account.DisplayName + "</td><td>" + Common.GetInt(groupExpenses.Total).format(); + "</td></tr>";
                                for (var j in expenses) {
                                    var expense = expenses[j];
                                    html += "<tr data-row='detail'><td>" + expense.AccountName + "</td><td>" + Common.GetInt(expense.Amount).format(); + "</td></tr>";
                                }
                                html += "<tr data-row='detail' class='bold subtotal'><td>" + account.DisplayName + " Total</td><td>" + Common.GetInt(groupExpenses.Total).format(); + "</td></tr>";

                            }

                            html += "<tr class='grand-total'><td><strong>Total Expenses</strong></td><td><strong>" + Common.GetInt(res.Data.TotalExpense).format() + "</strong></td></tr>";
                            $("#table-expense tbody").html(html);

                            $("#div-profit").html(Common.GetInt(profitTotal).format());
                            $("#div-expense").html(Common.GetInt(res.Data.TotalExpense).format());
                            var grossProfit = Common.GetInt(profitTotal + res.Data.TotalExpense);
                            $("#div-netprofit").html(grossProfit.format());
                            var profitPercent = Common.GetFloat((grossProfit / salesTotal) * 100).toFixed(2);
                            $("#div-profitpercent").html(profitPercent);
                            $("#ReportType").trigger("change");

                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        Format: function (num, requireMinusSign) {
            if (requireMinusSign)
                return commafy(num, 0);
            else {
                if (num > -1)
                    return commafy(num, 0);
                else
                    return commafy(num * -1, 0);
            }

        },
    };
}();