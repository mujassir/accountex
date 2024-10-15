
var OrderBooking = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "OrderBooking";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "SRNTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#Date";
    var currency = "";
    var requiredRequisition = false;
    var getRequisition = false;

    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            //multi-char placeholder
            $("#VoucherNumber").keypress(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#Date" : "#Date";
                    $this.LoadVoucher("same");
                }
            });
            $("#InvoiceNumber").keypress(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#Date" : "#Date";
                    $this.LoadVoucher("byOrderNo");
                }
            });

            $("#CurrencyId").change(function () {
                var text = $(this).find("option:selected").text();
                $("#item-container thead tr ").find("th:nth-child(6)").text("Amount in " + text);
            });
            $("#AuthLocationName").change(function (e) {
                $this.LoadVoucher("nextvouchernumber");
            });

            if (requiredRequisition) {
                this.MarkRequiredRequisition();
            }
            $(document).on("keyup", "input.Code", function (event) {

                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).closest("tr");
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.AddItem();
                            }
                            else {
                                if ($(this).val().trim() != "") {
                                    var err = $(this).val() + " is not valid code.";
                                    Common.ShowError(err);
                                    $(this).focus();
                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(5) input.Quantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }


                }
            });
            $(document).on("keyup change", "input.Rate", function () {
                var tr = $(this).closest("tr");
                var Quantity = $(tr).find("input.Quantity").val();
                var Rate = $(tr).find("input.Rate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.Amount").val(Amount);
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid party code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $("#SalesmanCode").keyup(function (e) {
                if (e.which == 13) {

                    var salesman = Common.GetByCode($(this).val());

                    if (typeof salesman != "undefined" && salesman != null) {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid salesman code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $("#SRN").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#InvoiceNumber").focus();
                    else
                        $this.LoadStockRequisition("challan");
                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {

                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);

                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = Common.GetDiscountDetail(product.AccountId);
                        //$(tr).find("input.ItemId").val(account.Id);
                        //$(tr).find("input.Name").val(account.Name);

                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(3) input.Article").val(product.Article);
                        $(tr).find(":nth-child(5) input.Unit").val(product.UnitType);
                        $(tr).find(":nth-child(6) input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice) - ( discount || 0));
                        //$(".container-message").hide();

                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.";
                            Common.ShowError(err);
                            $(this).focus();
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    //$(tr).find("input.Quantity").focus();
                    $(tr).find("input.Quantity").val("1");
                }

                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity, .Rate", function (event) {

                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
                var qty = Common.GetFloat($(this).val());
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }
            });
            $("#SalesmanCode").blur(function () {
                var salesman = Common.GetByCode($(this).val());
                if (typeof salesman != "undefined" && salesman != null) {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid salesman code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    //     $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    Common.LoadDiscount(party.Id, $this.AutoCompleteInit)
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            $(document).on("click", "#SRNTable > tbody tr", function () {
                $this.SelectStockRequisition(this);
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();

            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.SalesmanAutoCompleteInit();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                if (Setting.PageLandingView == "DetailView") {
                    $this.Add();
                }
                else
                    $this.GetNextVoucherNumber();
            }
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        New: function () {
            var $this = this;
            focusElement = "#Date";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            $('#AuthLocationName').trigger('change');
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            if (requiredRequisition) return
            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }, 300);
                focusElement = "";
            }
            //}
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitNumerics();
        },
        LoadStockRequisition: function (key) {
            var $this = this;
            var srn = $("#SRN").val();
            getRequisition = false;
            if (!srn) return;
            var locationId = $("#AuthLocationId").val();
            var type = VoucherType[$this.GetType()];
            const token = srn.split("-");
            var voucherNumber = srn;
            if (token.length > 1) {
                voucherNumber = token.slice(1).join("-");
            }
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "StockRequisition/" + voucherNumber + "?type=" + type + "&key=" + key + "&voucher=" + srn + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $(".portlet .container-message").addClass("hide");
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("SRN no is not valid.");
                        }

                        else {
                            $("#VoucherNumber").val(voucherno);
                            if (d.VoucherCode) {
                                $(`#AuthLocationName`).select2('val', d.VoucherCode)
                                $(`#AuthLocationId`).val(d.AuthLocationId)
                            }
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, $(this).val());
                            });
                            var items = d.StockRequisitionItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                Common.MapItemData(items);
                                $this.GetWholeTotal();
                            }
                        }
                        focusElement = "#AccountCode";
                        $this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        CloseItem: function () {
            Common.Clear();
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            $("#form-info-item").addClass("hide");
            $("#masterdetail").removeClass("hide");
            $("#div-table-item").addClass("hide");
        },
        //Print: function () {
        //    $("#lblDate").html($("#Date").val());
        //    $("#lblVoucherNumber-print").html($("#lblVoucherNumber").html());
        //    $("#lblAccountTitle").html($("#AccountIdFrom option:selected").text());
        //    $("#lblComments").html($("#Comments").val());
        //    $(".label-amount").html($("#Amount").val());
        //    $("#form-info").addClass("hide");
        //    $("#div-report").removeClass("hide");
        //},
        Print: function () {
            window.print();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                $this.CustomClear();
                $this.ListView();
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            if ($("input[value='finishedgoods']").is(":checked")) {
                record["OrderType"] = OrderType["FinishedGoods"]
            }
            else {
                record["OrderType"] = OrderType["Production"]
            }
            var Items = new Array();
            if (id > 0) {
                var prebookingno = Common.GetInt(record.PreVoucherNumber);
                var newbookingno = Common.GetInt(record.VoucherNumber);
                if (prebookingno != newbookingno) {
                    err = "You cannot change order no.Please save with previous  order no (" + prebookingno + ")";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                var qty = Common.GetInt($("#QuantityTotal").val());
                if (qty <= 0) {
                    Common.ShowError("Please add atleast one item.");
                    return;
                }
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += $("#AccountCode").val() + " is not valid party code.";
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["VoucherCode"] = $("#AuthLocationName")?.find(":selected")?.data("code") || null
                record["TransactionType"] = VoucherType[$this.GetType()];
                record["OrderItems"] = Items;
                if (record["SRN"]) {
                    const token = record["SRN"].split("-");
                    if (token.length > 1) {
                        record["SRN"] = token.splice(1).join("-")
                    }
                }
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  orderbooking ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(amount.toFixed(2));
            //var discount = Common.GetFloat($(tr).find("input.DiscountPercent").val());
            //var discountAmount = Common.GetFloat(amount) * discount / 100;
            //var netAmount = Common.GetInt(amount - discountAmount);
            //$(tr).find("input.DiscountAmount ").val(discountAmount.toFixed(2));
            //$(tr).find("input.NetAmount").val(netAmount);


            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var discount = 0.0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(5) input.Quantity").val());
                Price += Common.GetInt($(this).find(":nth-child(8) input.Amount").val());
            });
            $("#QuantityTotal").val(Quantity);
            $("#NetTotal").val(Price);
        },
        CustomClear: function () {
            $.uniform.update($("input:radio[name='order-type']").prop("checked", false));
            $.uniform.update($("input:radio[value='finishedgoods']").prop("checked", true));
            $("#item-container tbody").html("");
            Common.Clear();
            $("#btndelete,#btnprint").prop("disabled", true);
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;

            if (d == null)
                return;

            if (res.Data.Currency != null) {

                $("#report-saleitem tfoot tr ").find("td:nth-child(1)").text("Total (" + res.Data.Currency.Unit + ")");
            }
            else
                $("#report-saleitem tfoot tr ").find("td:nth-child(1)").text("Total ");
            if (d.OrderType == OrderType["FinishedGoods"]) {
                d.OrderType = "Finish Goods";
            }
            else {
                d.OrderType = "Production";
            }
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
            $("#lblInstructions").html(instruction);
            $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
            $("#lblDeliveryDate").html((d.DeliveryDate ? moment(d.DeliveryDate).format("dddd, DD-MM-YYYY") : ''));
            $("#lblShippingDate").html((d.ShippingDate ? moment(d.ShippingDate).format("DD-MM-YYYY") : ''));

            var type = $this.GetType();
            var html = "";
            var items = d.OrderItems;
            var index = 1;


            var totalamount = 0.0;

            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += `<td rowspan="${item.ItemRemarks ? 2 : 1}">${index++}</td>`;
                html += `<td rowspan="${item.ItemRemarks ? 2 : 1}">${item.ArticleNo != null ? item.ArticleNo : ""}</td>`;
                html += "<td>" + item.ItemName + "</td>";
                html += "<td class='align-right'>" + item.Quantity + "</td>";
                if (item.Unit == null)
                    html += "<td></td>";
                else
                    html += "<td>" + item.Unit + "</td>";
                html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                html += "<td class='align-right'>" + item.Amount.format() + "</td>";
                //html += "<td>" + item.DiscountAmount.format() + "</td>";
                //html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
                if (item.ItemRemarks) {
                    html += `<tr><td colspan="4">${item.ItemRemarks}</td></tr>`
                }
                totalamount += item.Amount;

            }
            $("#lblTotalAmount").html(totalamount.format());
            $("#report-saleitem tbody").append(html);
            var nettotal = $("#QuantityTotal").val();


            //$("#report-saleitem tbody").html("");
            //Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);

            //   $("#lblNetTotal").html(nettotal);
        },

        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var bookNo = Common.GetInt($("#InvoiceNumber").val());
            var locationId = Common.GetInt($("#AuthLocationId").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno + "&bookNo=" + bookNo + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  orderbooking ...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (!getRequisition)
                            $this.CustomClear();
                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        var defaultLocationId = d?.AuthLocationId || 0;
                        if (!d?.AuthLocationId) {
                            defaultLocationId = $("#AuthLocationName")?.find(":selected")?.data("custom")
                        }
                        const srn = $("#SRN").val();
                        Common.MapEditData(d, "#form-info");
                        $("#SRN").val(srn);

                        $this.MarkRequiredRequisition();
                        $(`#AuthLocationId`).val(defaultLocationId)
                        if (d == null) {
                            if (!getRequisition)
                                $this.CustomClear();
                            $("#SRN").prop("disabled", false);
                            $("#SRNSearchIcon").removeClass("hide");
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);

                            if (getRequisition)
                                $this.LoadStockRequisition("challan");
                        }
                        else {

                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (d.OrderType == OrderType["FinishedGoods"] && d.Id > 0) {
                                $("input:radio[value='finishedgoods']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='production']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0) {
                                    $("input:radio[value='finishedgoods']").prop("checked", true);
                                    $.uniform.update();
                                }
                            }
                            var items = d.OrderItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#SRN").prop("disabled", true);
                                $("#SRNSearchIcon").addClass("hide");
                                $("#btndelete,#btnprint").prop("disabled", false);
                                Common.MapItemData(items);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();
                            }
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        $this.AddItem();
                        $this.LoadReportData(res);
                        //Common.SetPageAccess(d);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
        },
        Delete: function () {
            var $this = this;
            var type = VoucherType[$this.GetType()];
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var locationId = Common.GetInt($("#AuthLocationId").val());
                var id = Common.GetInt($("#Id").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucherNo=" + voucherno + "&locationId=" + locationId;
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;

                if (id <= 0) {
                    Common.ShowError("No Voucher found for deletion.");
                    return;
                }

                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting  orderbooking ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            switch ($this.GetType().toLowerCase()) {
                case "saleorder":
                    id = PageSetting.Customers;
                    break;
                case "purchaseorder":
                    id = PageSetting.Suppliers;
                    break;
            }
            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }
            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var d = Common.GetByCode(ui.item.value);
                    if (typeof d != "undefined" && d != null) {
                        //$("#Comments").val("Sold To: " + d.AccountCode + "-" + d.Name);
                        $("#AccountName").val(d.Name);
                        $("#lblAccountName").html(d.Name);
                        $("#AccountId").val(d.Id);
                        Common.GetPartyAddress(d.Id);
                        $this.GetPartyInfo(d.Id);
                        $(".container-message").hide();
                    }
                }
            });
        },
        GetPartyInfo: function (accountId) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?accountId=" + accountId + "&key=loadpartyinfo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  orderbooking ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#Email").val(res.Data.Email);
                        $("#ContactPerson").val(res.Data.ContactPerson);
                    }
                },
                error: function (e) {
                }
            });
        },


        SalesmanAutoCompleteInit: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Salesman;
            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#SalesmanCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var d = Common.GetByCode(ui.item.value);
                    if (typeof d != "undefined" && d != null) {
                        $("#SalesmanName").val(d.Name);
                        $("#SalesmanId").val(d.Id);
                        $(".container-message").hide();
                    }
                }
            });



        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $("#order-type-conatiner").addClass("hide");
            if (voucher == "saleorder") {
                $("#order-type-conatiner").removeClass("hide");
                $("#data-srn-no").addClass("hide");
            }
            else {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").addClass("hide");
                $("#data-srn-no").removeClass("hide");
            }
            if (this.GetType() == "purchaseorder") {
                requiredRequisition = PageSetting?.RequiredPurchaseRequisition;
            } else if (this.GetType() == "saleorder") {
                requiredRequisition = PageSetting?.RequiredSaleRequisition;
            }
            this.LoadAccounts();

        },
        onCustomerLoad: function () {
            this.AutoCompleteInit()
            $this.AutoCompleteInit()
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            let products = Common.GetLeafAccounts(PageSetting.Products);
            const EnableProductIds = Common.GetData("CustomerDiscount" + Common.LocalStoragePrefix)
                ?.filter(x => x.Enable)?.map(x => x.COAProductId) || []
            if (PageSetting.ShowOnlyCustomerProducts) {
                products = products.filter(x => EnableProductIds.includes(x.Id))
            }
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = Common.GetDiscountDetail(product.AccountId);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(3) input.ArticleNo").val(product.ArticleNo);
                        $(tr).find(":nth-child(5) input.Unit").val(product.UnitType);
                        $(tr).find(":nth-child(7) input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice) - (discount || 0));

                        $(".container-message").hide();
                    }
                }
            });

        },
        GetStockRequisition: function () {
            var $this = this;
            // //$("#Id").val('');
            //$("#SearchScope").val('responsive');
            // $("#PatientTable > tbody").html('');
            //if ($('#PatientSearch-Icon').hasClass("green") == true)
            $("#Orders-Container").modal("show");
            $this.BindSRNDatatable();
        },
        BindSRNDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.fgrn)
                type = EntryType.FinishedGoods;

            //else
            //    type = VoucherType.purchaseorder
            //var url = Setting.APIBaseUrl + "StockRequisition?type=" + type
            var options =
              {
                  "bPaginate": false,
                  "bSort": false,
              }
            var url = Setting.APIBaseUrl + "StockRequisition"
            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        SelectStockRequisition: function (tr) {
            var $this = this;
            var srn = $(tr).find("input.VoucherNumber").val();
            var locationId = Common.GetInt($(tr).find("input.VoucherNumber").attr("data-location-id"));
            var locationCode = $(tr).find("input.VoucherNumber").attr("data-location-code");

            if (locationId > 0 && !PageSetting.IsMultipleLocationEnabled) {
                Common.ShowError("Multiple location wise voucher not enabled.");
                return
            }
            if (locationId == 0 && PageSetting.IsMultipleLocationEnabled) {
                Common.ShowError("Location is not selected in this voucher");
                return
            }
            if (srn.trim() != "" && srn != null) {
                $("#SRN").val(srn);
                if (PageSetting.IsMultipleLocationEnabled) {
                    const existingAutLocId = $("#AuthLocationId").val();
                    if (existingAutLocId != locationId) {
                        getRequisition = true
                        $("#AuthLocationId").val(locationId)
                        $('#AuthLocationName').select2('val', locationCode);
                        this.LoadVoucher("nextvouchernumber");
                    } else {
                        $this.LoadStockRequisition("challan");
                    }
                } else {
                    $this.LoadStockRequisition("challan");
                }
                $('#btnOrderClose').click();
            }
        },
        MarkRequiredRequisition: function () {
        }
    };
}();

