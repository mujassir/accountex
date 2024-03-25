var PartyTransDetail = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            var id = Common.GetQueryStringValue("accountId");
            if (id == undefined || id == "") {
                //$("#AccountId").select2("val", id);
                $(".row-account").removeClass("hide");
            }
            $("#AccountCode").keyup(function () {

                var ac = Common.GetByCode($(this).val());
                if (typeof ac == "undefined" || ac == null) {
                    $("#AccountName").val("");
                    $("#AccountId").val("");
                    $("input[name='all-product']").prop("checked", true);
                    $.uniform.update();
                }
                else {
                    $("#AccountName").val(ac.DisplayName);
                    $("#AccountId").val(ac.Id);
                    $("input[name='all-product']").prop("checked", false);
                    $.uniform.update();
                }
            });
            $("input[name='all-product']").change(function () {
                if ($(this).is(":checked")) {
                    $("#AccountName").val("");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                }

            });
            $this.LoadPageSetting();

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

            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();

            var qs = "?key=GetPartyTransDetail";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[Common.GetQueryStringValue("type").toLowerCase()];
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  party wise " + Common.GetQueryStringValue("type").toLowerCase() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var account = "";
                        var parties = Enumerable.From(data).GroupBy("$.AccountId", null,
                 function (key, g) {
                     var result = {
                         AccountId: key,
                         Party: g.FirstOrDefault(),
                         TotalAmount: g.Sum("$.GrossTotal"),
                         TotalDiscount: g.Sum("$.NetTotal"),
                         TotalNetAmount: g.Sum("$.NetTotal"),
                         TotalQuantity: g.Sum("$.Quantity"),
                         Sales: g.ToArray()
                     }
                     return result;
                 }).ToArray();

                        for (var i in parties) {

                            var party = parties[i];
                            html += "<tr class='group" + party.Party.AccountCode.replace(/\s+/g, "-").toLowerCase() + " group-tr'><td colspan='10' class='group'>" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td></tr>";
                            var sales = party.Sales;
                            for (var j in sales) {
                                var sale = sales[j];
                                var saleItems = sales[j];
                                var url = Common.GetTransactionUrl(sale.TransactionType, sale.VoucherNumber);
                                var vType = VoucherTypes[sale.TransactionType];

                                html += "<tr><td>" + trans.Date1 + "</td>";
                                html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + trans.VoucherNumber + "</td>";
                                html += "<td>" + trans.Comments + "</td></tr>";

                                for (var k in saleItems) {
                                    var item = saleItems[k];
                                    html += "<tr><td colspan='3'>" + item.ItemName + "</td>";
                                    html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + trans.VoucherNumber + "</td>";
                                    html += "<td>" + trans.Name + "<a/></td>";
                                    html += "<td>" + trans.Comments + "</td>";
                                    html += "<td>" + trans.Quantity + "</td>";
                                    html += "<td>" + trans.Rate + "</td>";
                                    html + "<td>" + trans.Amount + "</td>";
                                    html += "<td>" + trans.DiscountAmount + "</td>";
                                    html += "<td>" + trans.NetAmount + "</td></tr>";
                                }


                            }
                            html += "<tr class='bold subtotal'><td></td><td>Total</td><td></td><td></td><td>" + party.TotalQuantity + "</td><td></td><td>" + party.TotalAmount + "</td><td>" + party.TotalDiscount + "</td><td>" + party.TotalNetAmount + "</td><td></td></tr>";

                        }



                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td>Total</td><td></td><td></td><td></td><td></td><td>" + res.Data.TotalAmount + "</td><td>" + res.Data.TotalDiscount + "</td><td>" + res.Data.TotalNetAmount + "</td><td>" + res.Data.TotalBalance + "</td></tr>";
                        $(".report-table tbody").html(html);
                        //$(".report-table tfoot").html(html);
                        //$(".opening-balance").html(res.Data.OpeningBalance);
                        //$(".total-debit").html(res.Data.TotalDebit);
                        //$(".total-credit").html(res.Data.TotalCredit);
                        //$(".total-balance").html(res.Data.TotalBalance);
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