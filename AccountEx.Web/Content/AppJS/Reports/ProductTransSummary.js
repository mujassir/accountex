var ProductTransSummary = function () {
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
            $("#GroupName").change($this.FilterByGroup);
            $this.LoadPageSetting();

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
            var _this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetProductSalePurchaseReport";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[Common.GetQueryStringValue("type")];
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
                        var groupWiseSales = Enumerable.From(data).GroupBy("$.GroupId", null,
                  function (key, g) {
                      var result = {
                          GroupId: key,
                          Group: g.FirstOrDefault(),
                          TotalAmount: g.Sum("$.Amount"),
                          TotalDiscount: g.Sum("$.DiscountAmount"),
                          TotalNetAmount: g.Sum("$.NetAmount"),
                          TotalQuantity: g.Sum("$.Quantity"),
                          GroupSales: g.ToArray()
                      }
                      return result;
                  }).ToArray();
                        for (var j in groupWiseSales) {
                            var groupWiseSale = groupWiseSales[j];


                            html += "<tr class='group" + groupWiseSale.Group.GroupName.replace(/\s+/g, "-").toLowerCase() + " group-tr'><td colspan='4' class='group'>" + groupWiseSale.Group.GroupName + "</td></tr>";
                            var sales = Enumerable.From(groupWiseSale.GroupSales).GroupBy("$.ItemName", null,
                       function (key, g) {
                           var result = {
                               ItemName: key,
                               Item: g.FirstOrDefault(),
                               TotalAmount: g.Sum("$.Amount"),
                               TotalDiscount: g.Sum("$.DiscountAmount"),
                               TotalNetAmount: g.Sum("$.NetAmount"),
                               TotalQuantity: g.Sum("$.Quantity"),
                               Transactions: g.ToArray()
                           }
                           return result;
                       }).ToArray();
                            for (var i in sales) {

                                var sale = sales[i];

                                var transactions = sale.Transactions;
                                //for (var j in transactions) {
                                //    var trans = transactions[j];
                                //    var url = Common.GetTransactionUrl(trans.TransactionType, trans.VoucherNumber);
                                //    var vType = VoucherTypes[trans.TransactionType];
                                //    html += "<tr><td>" + trans.Date1 + "</td><td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + trans.VoucherNumber
                                //        + "</td><td>" + trans.Name
                                //          + "<a/></td><td>" + trans.Comments + "</td><td>" + trans.Quantity + "</td><td>" + trans.Rate
                                //            + "</td><td>" + trans.Amount + "</td><td>" + trans.DiscountAmount + "</td><td>" + trans.NetAmount + "</td>"

                                //        + "<td>" + trans.Balance1 + "</td></tr>";
                                //}
                                var rate = Common.GetFloat(sale.TotalNetAmount / sale.TotalQuantity);

                                html += "<tr class='group" + groupWiseSale.Group.GroupName.replace(/\s+/g, "-").toLowerCase() + "'><td>" + sale.Item.ItemCode + "-" + sale.Item.ItemName + "</td><td class='align-right'>" + sale.TotalQuantity + "</td><td class='align-right'>" + rate.toFixed(2) + "</td><td class='align-right'>" + sale.TotalNetAmount + "</td></tr>";

                            }
                            html += "<tr class='bold subtotal'><td>Total</td><td>" + groupWiseSale.TotalQuantity + "</td><td></td><td>" + groupWiseSale.TotalNetAmount + "</td></tr>";
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='4' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td colspan='3'>Total</td><td>" + res.Data.TotalNetAmount + "</td></tr>";
                        $(".report-table tbody").html(html);
                        _this.FilterByGroup();
                        

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