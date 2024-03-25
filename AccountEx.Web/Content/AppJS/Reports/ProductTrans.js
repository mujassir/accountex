var ProductTrans = function () {
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

        },
        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = 0;
            if (type == "sale") {
                id = PageSetting.Customers;
                $(".main-title").html("Product Sale");
            }
            else if (type == "salereturn") {
                id = PageSetting.Customers;
                $(".main-title").html("Product Sale Return");
            }
            else if (type == "purchase") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Product Purchase");
            }
            else if (type == "purchasereturn") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Product Purchase Return");
            }



            var tokens = Common.GetLeafAccounts(PageSetting.ProductHeadId);
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

            this.LoadAccounts();

        },
        LoadData: function () {

            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var qs = "?key=GetProductSalePurchaseReport";
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
                blockMessage: "Loading  product" + Common.GetQueryStringValue("type").toLowerCase() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var account = "";
                        var sales = Enumerable.From(data).GroupBy("$.ItemName", null,
                   function (key, g) {
                       var result = {
                           ItemName: key,
                           Item: g.FirstOrDefault(),
                           TotalAmount: g.Sum("$.Amount"),
                           TotalDiscount: g.Sum("$.DiscountAmount"),
                           TotalGstAmount: g.Sum("$.GSTAmount"),
                           TotalNetAmount: g.Sum("$.NetAmount"),
                           TotalQuantity: g.Sum("$.Quantity"),
                           Transactions: g.ToArray()
                       }
                       return result;
                   }).ToArray();
                  //      var groupsales = Enumerable.From(data).GroupBy("$.GroupName", null,
                  //function (key, g) {
                  //    var result = {
                  //        GroupName: key,
                  //        Group: g.FirstOrDefault(),
                  //        TotalAmount: g.Sum("$.Amount"),
                  //        TotalDiscount: g.Sum("$.DiscountAmount"),
                  //        TotalGstAmount: g.Sum("$.GSTAmount"),
                  //        TotalNetAmount: g.Sum("$.NetAmount"),
                  //        TotalQuantity: g.Sum("$.Quantity"),
                  //        Transactions: g.ToArray()
                  //    }
                  //    return result;
                  //}).ToArray();


                        for (var i in sales) {

                            var sale = sales[i];
                            html += "<tr class='group" + sale.Item.ItemCode.replace(/\s+/g, "-").toLowerCase() + " group-tr hide' data-row='detail'>"
                            if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                                html += "<td colspan='10' class='group'>" + sale.Item.ItemCode + "-" + sale.Item.ItemName + "</td>"
                            }
                            else {
                                html += "<td colspan='9' class='group'>" + sale.Item.ItemCode + "-" + sale.Item.ItemName + "</td>"
                            }
                            html += "</tr>";


                            var transactions = sale.Transactions;
                            for (var j in transactions) {
                                var trans = transactions[j];
                                var url = Common.GetTransactionUrl(trans.TransactionType, trans.VoucherNumber);
                                var vType = VoucherTypes[trans.TransactionType];
                                html += "<tr class='hide' data-row='detail' ><td>" + trans.Date1 + "</td>"
                                html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + trans.VoucherNumber + "</td>"
                                html += "<td>" + trans.Name + "<a/></td>"
                                html += "<td>" + trans.Comments + "</td><td class='align-right'>" + trans.Quantity.format() + "</td><td class='align-right'>" + trans.Rate.format() + "</td><td class='align-right'>" + trans.Amount.format() + "</td>"
                                if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                                    html += "<td class='align-right'>" + trans.GSTAmount.format() + "</td>"
                                }
                                html += "<td class='align-right'>" + trans.NetAmount.format() + "</td>" + "<td class='align-right'>" + trans.Balance1 + "</td></tr>";

                            }
                            html += "<tr class='bold subtotal hide' data-row='detail'><td colspan='3'></td><td class='align-right'>Sub Total</td>"
                            html += "<td colspan='1' class='align-right'>" + sale.TotalQuantity.format() + "</td><td></td><td class='align-right'>" + sale.TotalAmount.format() + "</td>"
                            if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                                html += "<td class='align-right'>" + sale.TotalGstAmount.format() + "</td>"
                            }

                            html += "<td colspan='1' class='align-right'>" + sale.TotalNetAmount.format() + "</td><td></td></tr>";

                            //Summary rows goes here

                            var avgRate = 0;
                            if (sale.TotalQuantity > 0)
                                avgRate = sale.TotalAmount / sale.TotalQuantity;

                            html += "<tr class='hide' data-row='summary'>"

                            html += "<td>" + sale.Item.ItemCode + "-" + sale.Item.ItemName + "</td>"
                            html += "<td class='align-right'>" + sale.TotalQuantity.format() + "</td>";
                            html += "<td class='align-right'>" + avgRate.format() + "</td>";
                            html += "<td class='align-right'>" + sale.TotalAmount.format() + "</td>";
                            if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                                html += "<td class='align-right'>" + sale.TotalGstAmount.format() + "</td>"
                            }
                            html += "<td class='align-right'>" + sale.TotalNetAmount.format() + "</td>";
                            html += "</tr>";


                        }

                        if (res.Data.Records.length == 0) {
                            html += "  <tr class='hide' data-row='detail'><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                            html += "  <tr class='hide' data-row='summary'><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        html += "<tr style='background-color:skyblue' class='bold grand-total hide' data-row='detail'>"
                        html += "<td colspan='3'></td>"
                        html += "<td>Grand Total</td>"
                        html += "<td colspan='2' class='align-right'>" + res.Data.TotalQuantity + "</td>"
                        html += "<td class='align-right'>" + res.Data.TotalAmount + "</td>"
                        if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                            html += "<td class='align-right'>" + res.Data.TotalGSTAmount + "</td>"
                        }
                        html += "<td class='align-right'>" + res.Data.TotalNetAmount + "</td>"
                        html += "<td class='align-right'>" + res.Data.TotalBalance + "</td></tr>";

                        //Summary rows goes here

                        html += "<tr style='background-color:skyblue' class='bold grand-total hide' data-row='summary'>"
                        html += "<td class='align-right'>Grand Total</td>"
                        html += "<td></td><td  class='align-right'>" + res.Data.TotalQuantity + "</td>"
                        html += "<td class='align-right'>" + res.Data.TotalAmount + "</td>"
                        if (type.indexOf("gst") != -1 || type.indexOf("services") != -1) {
                            html += "<td class='align-right'>" + res.Data.TotalGSTAmount + "</td>"
                        }
                        html += "<td class='align-right'>" + res.Data.TotalNetAmount + "</td>"
                        html += "</tr>";

                        $(".report-table tbody").html(html);
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
    };
}();