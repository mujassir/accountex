var PartyTrans1 = function () {
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

            $("#ReportType").change(function () {
                $this.SetReportView();
            });
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
        SetReportView: function () {
            var type = $("#ReportType").val();
            if (type.toLocaleLowerCase() == "summary") {
                $("tr[data-row='summary']").removeClass("hide");
                $("tr[data-row='detail']").addClass("hide");
            }
            else {
                $("tr[data-row='summary']").addClass("hide");
                $("tr[data-row='detail']").removeClass("hide");
            }
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
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetPartyTrans";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[type];
            qs += "&accountId=" + $("#AccountId").val();
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
                        var qtyTotalsummary = 0;
                        var amountTotalsummary = 0;
                        var discountTotalsummary = 0;
                        var promotionTotalsummary = 0;
                        var netTotalsummary = 0;
                        var parties = Enumerable.From(data).GroupBy("$.AccountId", null,
              function (key, g) {
                  var result = {
                      AccountId: key,
                      Party: g.FirstOrDefault(),
                      TotalAmount: g.Sum("$.Amount"),
                      TotalDiscount: g.Sum("$.DiscountAmount"),
                      PromotionTotal: g.Sum("$.PromotionTotal"),
                      TotalNetAmount: g.Sum("$.NetAmount"),
                      TotalQuantity: g.Sum("$.Quantity"),
                      Sales: g.ToArray()
                  }
                  return result;
              }).ToArray();
                        for (var i in parties) {

                            var party = parties[i];
                            html += "<tr class='hide group" + party.Party.AccountCode.replace(/\s+/g, "-").toLowerCase() + " group-tr' data-row='detail'><td colspan='11' class='group'>" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td></tr>";
                            //html += "<tr class='hide group" + party.Party.AccountCode.replace(/\s+/g, "-").toLowerCase() + " group-tr' data-row='summary'><td colspan='11' >" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td>";
                            var sales = party.Sales;
                            for (var j in sales) {
                                var sale = sales[j];
                                var saleItems = sale.SaleItems;
                                var url = Common.GetTransactionUrl(sale.TransactionType, sale.VoucherNumber);
                                var vType = VoucherTypes[sale.TransactionType];

                                html += "<tr data-row='detail' class='hide'>";
                                html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + sale.VoucherNumber + "</td>";
                                html += "<td>" + sale.InvoiceNumber + "</td>";
                                html += "<td colspan='3'>" + (sale.Comments != null ? sale.Comments : "") + "</td>";
                                html += "<td colspan='5'></td>";
                                html += "</tr>";

                                for (var k in saleItems) {
                                    var item = saleItems[k];
                                    html += "<tr data-row='detail' class='hide'>";
                                    html += "<td colspan='4'></td>";
                                    html += "<td>" + item.ItemName + "</td>";
                                    html += "<td class='align-right'>" + item.Quantity.format() + "</td>";
                                    html += "<td class='align-right'>" + item.Rate.format() + "</td>";

                                    if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                        html += "<td  class='align-right'>" + Common.GetInt(item.Amount).format() + "</td>";
                                        html += "<td></td>";
                                        html += "<td></td>";
                                      //  html += "<td  colspan='3' class='align-right'>" + Common.GetInt(item.Amount).format() + "</td>"; 
                                        //html += "<td>" + Common.GetInt(item.DiscountAmount).format() + "</td>";
                                    }
                                    else {
                                        html += "<td class='align-right'>" + Common.GetInt(item.Amount).format() + "</td>";
                                        html += "<td class='align-right'>" + Common.GetInt(item.GSTAmount).format() + "</td>";
                                    }

                                    html += "<td class='align-right'>" + Common.GetInt(item.NetAmount).format() + "</td></tr>";
                                }
                                var qtyTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Quantity"));
                                var amountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.Amount")).format();
                                var discountTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.DiscountAmount")).format();
                                var promotionTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.PromotionTotal")).format();
                                var gstTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.GSTAmount")).format();
                                var netTotal = Common.GetInt(Enumerable.From(saleItems).Sum("$.NetAmount")).format();
                                html += "<tr class='bold vouchertotal hide' data-row='detail'><td colspan=4></td>";
                                html += "<td class='align-right'>Voucher Total</td><td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td></td><td class='align-right'>" + sale.GrossTotal.format() + "</td>";
                                if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                    html += "<td class='align-right'>" + sale.Discount + "</td>";
                                    html += "<td class='align-right'>" + sale.PromotionTotal + "</td>";
                                }
                                else {
                                    html += "<td class='align-right'>" + gstTotal.format() + "</td>";
                                }

                                html += "<td class='align-right'>" + sale.NetTotal.format() + "</td></tr>";




                            }

                            //html += "<td></td><td>" + sale.GrossTotal + "</td>";
                            //if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                            //    html += "<td>" + sale.Discount + "</td>";
                            //    html += "<td>" + sale.PromotionTotal + "</td>";
                            //}
                            //else {
                            //    html += "<td>" + gstTotal + "</td>";
                            //}
                            //html += "<td>" + sale.NetTotal + "</td></tr>";

                            var qtyTotal = Common.GetInt(Enumerable.From(party.Sales).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(party.Sales).Sum("$.GrossTotal"));
                            var discountTotal = Common.GetInt(Enumerable.From(party.Sales).Sum("$.Discount"));
                            var promotionTotal = Common.GetInt(Enumerable.From(party.Sales).Sum("$.PromotionTotal"));
                            var gstTotal = Common.GetInt(Enumerable.From(party.Sales).Sum("$.GstAmountTotal"));
                            var netTotal = Common.GetInt(Enumerable.From(party.Sales).Sum("$.NetTotal"));

                            html += "<tr class='bold subtotal hide' data-row='detail'><td colspan='4'></td>";
                            html += "<td>Sub Total</td><td class='align-right'>" + qtyTotal.format() + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal.format() + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal.format() + "</td>";
                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal.format() + "</td>";
                            }
                            html += "<td class='align-right'>" + promotionTotal.format() + "</td>";

                            html += "<td class='align-right'>" + netTotal.format() + "</td></tr>";




                            html += "<tr class='hide' data-row='summary'><td>" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td>";

                            html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                            html += "<td class='align-right'>" + amountTotal.format() + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal.format() + "</td>";

                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal.format() + "</td>";
                            }
                            html += "<td class='align-right'>" + promotionTotal.format() + "</td>";

                            html += "<td class='align-right'>" + netTotal.format() + "</td></tr>";

                            qtyTotalsummary += Common.GetFloat(qtyTotal);
                            amountTotalsummary += Common.GetFloat(amountTotal);
                            discountTotalsummary += Common.GetFloat(discountTotal);
                            promotionTotalsummary += Common.GetFloat(promotionTotal);
                            netTotalsummary += Common.GetFloat(netTotal);



                        }

                        html += "<tr class='hide bold' data-row='summary'>";
                        html += "<td class='align-right'>Total</td>";
                        html += "<td class='align-right'>" + qtyTotalsummary.format() + "</td>";
                        html += "<td class='align-right'>" + amountTotalsummary.format() + "</td>";
                        html += "<td class='align-right'>" + discountTotalsummary.format() + "</td>";

                        html += "<td class='align-right'>" + promotionTotalsummary.format() + "</td>";
                        html += "<td class='align-right'>" + netTotalsummary.format() + "</td>";
                        html += "</tr>";


                        if (res.Data.Records.length == 0)
                            html += "  <tr data-row='detail' class='hide'><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                        else {

                            var qtyTotal = Common.GetInt(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetInt(Enumerable.From(data).Sum("$.GrossTotal")).format();
                            var discountTotal = Common.GetInt(Enumerable.From(data).Sum("$.Discount")).format();
                            var promotionTotal = Common.GetInt(Enumerable.From(data).Sum("$.PromotionTotal")).format();
                            var gstTotal = Common.GetInt(Enumerable.From(data).Sum("$.GstAmountTotal")).format();
                            var netTotal = Common.GetInt(Enumerable.From(data).Sum("$.NetTotal")).format();

                            html += "<tr style='background-color:skyblue' class='bold grand-total hide' data-row='detail'><td colspan=3></td>";
                            html += "<td colspan=2 >Grand Total</td><td class='align-right'>" + qtyTotal.format() + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal + "</td>";

                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal + "</td>";
                            }
                            html += "<td class='align-right'>" + promotionTotal + "</td>";
                            html += "<td class='align-right '>" + netTotal + "</td></tr>";
                        }



                        $(".report-table tbody").html(html);

                        setTimeout(PartyTrans1.SetReportView(), 500);


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