var PartyTrans = function () {
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
            });
            $this.LoadPageSetting();
            $this.LoadData();
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
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


            var tokens = Common.GetLeafAccounts(PageSetting.PartyHeadId);
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
            var groupId =Common.GetFloat( $("#GroupId").val());
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetPartyTrans";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[type];
            qs += "&accountId=" + $("#AccountId").val();
            qs += "&groupId=" + groupId;
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
                        var parties = Enumerable.From(data).GroupBy("$.AccountId", null,
              function (key, g) {
                  var result = {
                      AccountId: key,
                      Party: g.FirstOrDefault(),
                      TotalAmount: g.Sum("$.Amount"),
                      TotalDiscount: g.Sum("$.DiscountAmount"),
                      TotalNetAmount: g.Sum("$.NetAmount"),
                      TotalQuantity: g.Sum("$.Quantity"),
                      Sales: g.ToArray()
                  }
                  return result;
              }).ToArray();
                        for (var i in parties) {

                            var party = parties[i];
                            html += "<tr  class='group" + party.Party.AccountCode.replace(/\s+/g, "-").toLowerCase() + " group-tr hide' data-row='detail'><td colspan='10' class='group' >" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td></tr>";
                            html += "<tr  class='group" + party.Party.AccountCode.replace(/\s+/g, "-").toLowerCase() + " group-tr hide' data-row='summary'><td colspan='9' class='group' >" + party.Party.AccountCode + "-" + party.Party.AccountName + "</td></tr>";
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
                                html += "<td colspan='4'></td>";
                                html += "</tr>";

                                for (var k in saleItems) {
                                    var item = saleItems[k];
                                    html += "<tr data-row='detail' class='hide'>";
                                    html += "<td colspan='4'></td>";
                                    html += "<td>" + item.ItemName + "</td>";
                                    html += "<td class='align-right'>" + item.Quantity.format() + "</td>";
                                    html += "<td class='align-right'>" + item.Rate.format() + "</td>";

                                    if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                        html += "<td  colspan=' class='align-right'>" + Common.GetFloat(item.Amount).format() + "</td>";
                                        if (item.DiscountAmount>0)
                                            html += "<td>" + Common.GetFloat(item.DiscountAmount).format() + "</td>";
                                        else
                                            html += "<td></td>";

                                    }
                                    else {
                                        html += "<td class='align-right'>" + Common.GetFloat(item.Amount).format() + "</td>";
                                        html += "<td class='align-right'>" + Common.GetFloat(item.GSTAmount).format() + "</td>";
                                    }
                                    html += "<td class='align-right'>" + Common.GetFloat(item.NetAmount).format() + "</td></tr>";
                                }
                                var qtyTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.Quantity"));
                                var amountTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.Amount")).format();
                                var discountTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.DiscountAmount")).format();
                                var gstTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.GSTAmount")).format();
                                var netTotal = Common.GetFloat(Enumerable.From(saleItems).Sum("$.NetAmount")).format();
                                html += "<tr data-row='detail' class='bold vouchertotal'><td colspan=4></td>";
                                html += "<td>Voucher Total</td><td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                                if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                    html += "<td class='align-right'>" + sale.Discount + "</td>";
                                }
                                else {
                                    html += "<td class='align-right'>" + gstTotal + "</td>";
                                }
                                html += "<td class='align-right'>" + netTotal + "</td></tr>";


                                //Summary rows goes here
                                var avgRate = 0;
                                if (qtyTotal > 0) {
                               
                                    avgRate = Common.GetIntHtml(amountTotal) / qtyTotal;
                                    avgRate=avgRate.format();
                                }


                                html += "<tr data-row='summary' class='hide'>";
                                html += "<td>" + moment(sale.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + sale.VoucherNumber + "</td>";
                                html += "<td>" + sale.InvoiceNumber + "</td>";
                                html += "<td>" + (sale.Comments != null ? sale.Comments : "") + "</td>";
                                html += "<td class='align-right'>" + qtyTotal + "</td>";
                                html += "<td class='align-right'>" + avgRate + "</td>";
                                html += "<td class='align-right'>" + amountTotal + "</td>";
                                if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                    html += "<td class='align-right'>" + discountTotal + "</td>";
                                }
                                else {
                                    html += "<td class='align-right'>" + gstTotal + "</td>";
                                }
                                html += "<td class='align-right'>" + netTotal + "</td></tr>";
                            }
                            var qtyTotal = Common.GetFloat(Enumerable.From(party.Sales).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetFloat(Enumerable.From(party.Sales).Sum("$.GrossTotal")).format();
                            var discountTotal = Common.GetFloat(Enumerable.From(party.Sales).Sum("$.Discount")).format();
                            var gstTotal = Common.GetFloat(Enumerable.From(party.Sales).Sum("$.GstAmountTotal")).format();
                            var netTotal = Common.GetFloat(Enumerable.From(party.Sales).Sum("$.NetTotal")).format();

                            html += "<tr  class='bold subtotal hide' data-row='detail'><td colspan='4'></td>";
                            html += "<td>Sub Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal + "</td>";
                            }
                            html += "<td class='align-right'>" + netTotal + "</td></tr>";


                            html += "<tr  class='bold subtotal hide' data-row='summary'><td colspan='3'></td>";
                            html += "<td>Sub Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal + "</td>";
                            }
                            html += "<td class='align-right'>" + netTotal + "</td></tr>";


                        }



                        if (res.Data.Records.length == 0) {
                            html += "  <tr data-row='detail' class='hide'><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";
                            html += "  <tr data-row='summary' class='hide'><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        else {

                            var qtyTotal = Common.GetFloat(Enumerable.From(data).SelectMany("$.SaleItems").Sum("$.Quantity"));
                            var amountTotal = Common.GetFloat(Enumerable.From(data).Sum("$.GrossTotal")).format();
                            var discountTotal = Common.GetFloat(Enumerable.From(data).Sum("$.Discount")).format();
                            var gstTotal = Common.GetFloat(Enumerable.From(data).Sum("$.GstAmountTotal")).format();
                            var netTotal = Common.GetFloat(Enumerable.From(data).Sum("$.NetTotal")).format();

                            html += "<tr style='background-color:skyblue' class='bold grand-total hide' data-row='detail'><td colspan=3></td>";
                            html += "<td colspan=2 >Grand Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal + "</td>";
                            }
                            html += "<td class='align-right'>" + netTotal + "</td></tr>";

                            //Summary rows goes here


                            html += "<tr style='background-color:skyblue' class='bold grand-total hide' data-row='summary' class='hide'><td colspan=3></td>";
                            html += "<td >Grand Total</td><td class='align-right'>" + qtyTotal + "</td>";
                            html += "<td></td><td class='align-right'>" + amountTotal + "</td>";
                            if (type.indexOf("gst") == -1 && type.indexOf("services") == -1) {
                                html += "<td class='align-right'>" + discountTotal + "</td>";
                            }
                            else {
                                html += "<td class='align-right'>" + gstTotal + "</td>";
                            }
                            html += "<td class='align-right'>" + netTotal + "</td></tr>";
                        }



                        $(".report-table tbody").html(html);
                        $("#ReportType").trigger("change");
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