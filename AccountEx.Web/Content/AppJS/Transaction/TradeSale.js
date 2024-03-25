
var TradeSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "TradeSale";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            jQuery.hotkeys.options.filterInputAcceptingElements = false;
            jQuery.hotkeys.options.filterContentEditable = false;
            jQuery.hotkeys.options.filterTextInputs = false;
            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            //$(document).bind('keydown', 'Ctrl+s', function assets() {
            //    $(".td-label").html("bind");
            //    console.log("test");
            //    return false;
            //});
            $(document).on('keydown', null, 'alt+s', function () {

                $this.Save();
                return false;
            });
            $(document).on('keydown', null, 'alt+n', function () {

                $this.LoadVoucher("nextvouchernumber");
                return false;
            });
            $(document).on('keydown', null, 'alt+v', function () {

                $("#VoucherNumber").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+b', function () {

                $("#InvoiceNumber").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+d', function () {

                $("#Date").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+p', function () {

                $("#AccountCode").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+h', function () {

                $("#ShipViaName").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+c', function () {

                $("#ShipViaCode").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+i', function () {

                $this.AddItem();
                return false;
            });
            $(document).on('keydown', null, 'alt+c', function () {

                $("input:radio[value='" + SaleType.cashsale + "']").prop("checked", true);
                $.uniform.update();
            });
            $(document).on('keydown', null, 'alt+r', function () {

                $("input:radio[value='" + SaleType.creditsale + "']").prop("checked", true);
                $.uniform.update();
            });
            $(document).on('keydown', null, 'alt+g', function () {

                $("input:radio[value='" + SaleType.sampling + "']").prop("checked", true);
                $.uniform.update();
            });
            //$(document).on('keydown', null, 'alt+l', function () {

            //    $("input:radio[value='" + SaleType.sampling + "']").prop("checked", true);
            //    $.uniform.update();
            //});



            //$(document).bind('keydown', 'ctrl+a', function () {
            //    $(".td-label").html("bind");
            //    console.log("test");
            //    return false;
            //});
            //$("input[name='ac-type']").change(function () {
            //    if ($(this).val() == "cash") {
            //        var acc = Common.GetById(PageSetting.CashAccount);
            //        $("#AccountId").val(acc.Id);
            //        $("#AccountCode").val(acc.AccountCode);
            //        $("#AccountName").val(acc.DisplayName);
            //        $("#AccountCode").attr("disabled", "disabled");
            //        $this.MapComments();
            //    }
            //    else {
            //        $("#AccountCode").removeAttr("disabled");
            //        $("#AccountId").val("");
            //        $("#AccountCode").val("");
            //        $("#AccountName").val("");
            //    }

            //});
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                    //setTimeout(function () {
                    //    $('#InvoiceNumber').focus();
                    //}, 1000);
                    //$('#InvoiceNumber').focus();
                }
            });
            $("#GstAmountTotal").keyup(function () {
                $("#GST").val("0");
                $this.GetNetTotal();
            });
            $("#GST").keyup(function (e) {

                var percent = Common.GetFloat($("#GST").val());
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                var gstamount = Common.GetInt((grossTotal * percent) / 100);
                $("#GstAmountTotal").val(gstamount);
                $this.GetNetTotal();
            });
            $("#Discount").keyup(function () {
                $("#DiscountPercent").val("0");
                $this.GetNetTotal();
            });
            $("#DiscountPercent").keyup(function (e) {

                var percent = Common.GetFloat($("#DiscountPercent").val());
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                var discount = Common.GetInt((grossTotal * percent) / 100);
                $("#Discount").val(discount);
                $this.GetNetTotal();
            });
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    if (type == "sale")
                        $("#DCNo").focus();
                    else
                        $("#AccountCode").focus();
                }
            });
            $("#Date,#AccountCode").blur(function () {
                var type = $this.GetType();
                if (type == "sale") {
                    $("#item-container tbody tr").each(function () {
                        var tr = $(this);
                        var code = $(tr).find('input.Code').val();
                        var account = Common.GetByCode(code);
                        if (typeof account != "undefined" && account != null) {
                            var product = Common.GetAccountDetailByAccountId(account.Id);
                            $this.SetDiscount(tr, account, product);
                            $this.GetQuantityPriceTotal(tr);
                        }
                    });
                    //$("#DCNo").focus();
                }
            });
            $("#BiltyNo").keyup(function (e) {

                if (e.which == 13) {
                    $("#BiltyDate").focus();

                }
            });
            $("#BiltyDate").keyup(function (e) {
                $("#ShipViaCode").focus();
            });
            $("#InvoiceNumber").keyup(function (e) {
                if (e.which == 13) {
                    $("#Date").focus();
                }
                $this.MapComments();
            });
            $("#ShipQty").keypress(function (e) {
                if (e.which == 13) {

                    $(".btn.btn-primary.green").focus();
                }
            });
            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
            });
            $("#DCNo").keyup(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#AccountCode").focus();
                    else
                        $this.LoadDeliveryChallan("challan");
                }
                $this.MapComments();
            });
            $("#ShipViaCode").keyup(function (e) {
                if (e.which == 13) {
                    $(".btn.btn-primary.green").focus();
                }
            });
            $(document).on("keyup", ".Code", function (event) {
                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        if (type != "sale")
                            $(".btn.btn-primary.green").focus();
                        else
                            $("#BiltyNo").focus();
                    }
                    else {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).parent().parent();
                            if (typeof product != "undefined" && product != null) {
                                var discount = Common.GetDiscountDetail(product.AccountId);
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(type == "sale" || type == "salereturn" ? product.SalePrice : product.PurchasePrice));
                                $this.SetDiscount(tr, account, product);
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
                            $("#item-container tbody tr:nth-last-child(1) input.Quantity").focus().select();
                        }
                    }

                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = +$(this).val() + " is not valid party code.,";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = Common.GetDiscountDetail(product.AccountId);
                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $this.SetDiscount(tr, account, product);
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.,";
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".DiscountAmount", function () {

            });
            $(document).on("blur", ".Quantity,.PromotionPercent", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find("input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Amount,.DiscountPerItem,.DiscountAmount,.PromotionPercent", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).,";
                    Common.ShowError(err);
                }
            });

            $(document).on("keyup", ".Quantity,.Rate", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
            });
            $("#qty").keyup(function (e) {
                if (e.keyCode == 13)
                    $("#Rate").focus();
            });
            $("#Rate").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $this.AddItem();
            });
            $("#AccountCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
                    $("#AccountId").val(party.Id);
                    $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    var address = party.Address;
                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid party code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $("#SalemanCode").blur(function () {
                var saleman = Common.GetByCode($(this).val());
                if (typeof saleman != "undefined" && saleman != null) {
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid Saleman code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $("#OrderTakerCode").blur(function () {
                var ordertaker = Common.GetByCode($(this).val());
                if (typeof ordertaker != "undefined" && ordertaker != null) {
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid OrderTaker code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $("#TerritoryManagerCode").blur(function () {
                var territorymanager = Common.GetByCode($(this).val());
                if (typeof territorymanager != "undefined" && territorymanager != null) {
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid TerritoryManager code.,";
                        Common.ShowError(err);
                    }
                }
            });

            $this.LoadPageSetting();
            $(document).on("click", "#DCTable > tbody tr", function () {
                $this.SelectDC(this);
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
            AppData.CustomerDiscount = PageSetting.Discounts;
            $this.SalemanAutoCompleteInit();
            $this.OrderTakerAutoCompleteInit();
            $this.TerritoryManagerAutoCompleteInit();
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                $this.GetNextVoucherNumber();
            }
            //$("#full-screen").click();
            //$this.LoadDiscounts();

        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val() + ", Dc No:" + $("#DCNo").val() + ", Order No:" + $("#OrderNo").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
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
            $("#qty,#Rate,#Amount").val("");
            //}
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitNumerics();
        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                //$this.CustomClear();
                focusElement = "#Date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });

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
            //record["IsLessSale"] = $("input[value='lesssale']").is(":checked");
            record["SaleType"] = $("input[name='sale-type']:checked").val();
            var Items = new Array();
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += "" + $("#AccountCode").val() + " is not valid party code.,";
                }

                if (typeof $("#SalemanCode").val() != "undefined") {
                    var saleman = Common.GetByCode($("#SalemanCode").val());
                    if (typeof saleman == "undefined" || saleman == null) {
                        err += "" + $("#SalemanCode").val() + " is not valid saleman code.,";
                    }
                }
                if (typeof $("#OrderTakerCode").val() != "undefined") {
                    var ordertaker = Common.GetByCode($("#OrderTakerCode").val());
                    if (typeof ordertaker == "undefined" || ordertaker == null) {
                        err += "" + $("#OrderTakerCode").val() + " is not valid ordertaker code.,";
                    }
                }
                if (typeof $("#TerritoryManagerCode").val() != "undefined") {
                    var territorymanager = Common.GetByCode($("#TerritoryManagerCode").val());
                    if (typeof territorymanager == "undefined" || territorymanager == null) {
                        err += "" + $("#TerritoryManagerCode").val() + " is not valid territory manager code.,";
                    }
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).,";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += "" + item.ItemCode + " is not valid code.,";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "Transaction total amount should be graeter than zero(0).,";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["TransactionType"] = VoucherType[voucher];
                if (record.TransactionType == VoucherType.purchase) {
                    record["AdvanceTaxPercent"] = PageSetting.AdvanceTaxPercent
                }
                record["SaleItems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
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
            var discountperitem = Common.GetFloat($(tr).find("input.DiscountPerItem").val());
            var permotionPercent = Common.GetFloat($(tr).find("input.PromotionPercent").val());
            //var discountAmount = Common.GetFloat(amount) * discount / 100;
            var discountAmount = Common.GetFloat(Quantity) * discountperitem;
            var promotionAmount = Common.GetFloat(Quantity) * permotionPercent;
            var netAmount = Common.GetInt(amount - discountAmount - promotionAmount);
            $(tr).find("input.DiscountAmount").val(discountAmount.toFixed(2));
            $(tr).find("input.PromotionAmount").val(promotionAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount);
            $("#DiscountPercent").val("0");
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var discount = 0.0;
            var promotionAmount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                Price += Common.GetFloat($(this).find("input.Amount").val());
                discount += Common.GetFloat($(this).find("input.DiscountAmount").val());
                promotionAmount += Common.GetFloat($(this).find("input.PromotionAmount ").val());
                netamount += Common.GetFloat($(this).find("input.NetAmount").val());
            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity.toFixed(2));
            $("#item-container tfoot tr").find("input.Amount").val(Price.toFixed(2));
            $("#item-container tfoot tr").find("input.DiscountAmount").val(discount.toFixed(2));
            $("#item-container tfoot tr").find("input.PromotionAmount").val(promotionAmount.toFixed(2));
            $("#item-container tfoot tr").find("input.NetAmount").val(netamount);
            $("#qtytotal1").val(Quantity.toFixed(2));
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var incAmount = Common.GetInt(Price) - discount;
            $("#PromotionTotal").val(promotionAmount);
            $("#Discount").val(discount);
            $("#NetTotal").val(incAmount + "");
            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.purchase || type == VoucherType.purchasereturn) {
                var percent = Common.GetFloat(PageSetting.AdvanceTaxPercent);
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                var gstamount = Common.GetFloat((grossTotal * percent) / 100);
                $("#AdvanceTaxTotal").val(gstamount.toFixed(3));
            }
            var total = Common.GetFloat($("#GrossTotal").val());
            discount = Common.GetFloat($("#Discount").val());
            gstamount = Common.GetFloat($("#AdvanceTaxTotal").val());
            var promotionTotal = Common.GetInt($("#PromotionTotal").val());
            var nettotal = Common.GetFloat(total - discount - promotionTotal);
            nettotal = Common.GetFloat(nettotal + gstamount);
            $("#NetTotal").val(nettotal);
        },
        GetNetTotal: function () {

            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.purchase || type == VoucherType.purchasereturn) {
                var percent = Common.GetFloat($("#GST").val());
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                var gstamount = Common.GetInt((grossTotal * percent) / 100);
                $("#GstAmountTotal").val(gstamount);
            }
            var total = Common.GetInt($("#GrossTotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var promotionTotal = Common.GetInt($("#PromotionTotal").val());
            gstamount = Common.GetInt($("#AdvanceTaxTotal").val());
            var nettotal = Common.GetInt(total - discount - promotionTotal);
            nettotal = Common.GetInt(nettotal + gstamount);
            $("#NetTotal").val(nettotal);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            $("#item-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadDeliveryChallan: function (key) {
            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.sale || type == VoucherType.purchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.salereturn)
                type = VoucherType.goodreceive
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan/" + orderno + "?type=" + type + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                            //else if (d.Status == 5) {
                            //    Common.ShowError("Challan has already processed.");
                            //    focusElement = "dcnumber";
                            //}
                        else {
                            Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#qtytotal1").val(d.QuantityTotal);
                            $("#AccountCode").trigger("blur");
                            $("#Id").val(0);

                            $("#OBNo").val(d.InvoiceNumber);
                            $("#OrderDate").val(d.Date);

                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                var html = "";
                                for (var i in items) {
                                    var item = items[i];
                                    var account = Common.GetByCode(item.ItemCode);
                                    var product = Common.GetAccountDetailByAccountId(account.Id);
                                    var price = 0;
                                    items[i].Quantity = item.Quantity = item.Quantity - item.QtyDelivered;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        price = Common.GetFloat(product.SalePrice);
                                    }

                                    var amount = qty * price;
                                    //var gstpercent = PageSetting.GSTPercent;
                                    //var gstamount = Common.GetInt(amount) * gstpercent / 100;
                                    //var netAmount = Common.GetInt(amount) + gstamount;
                                    item["Rate"] = price;
                                    item["Amount"] = amount;
                                    //item["GSTPercent"] = gstpercent;
                                    //item["GSTAmount"] = gstamount;
                                    //item["NetAmount"] = netAmount;

                                }
                                Common.MapItemData(items);
                                $("#item-container tbody").append(html);
                                $this.MapComments();
                                //setTimeout(function () {
                                //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                //}, 500);

                            }
                            $this.GetWholeTotal();
                        }

                        $this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixFClasses(d, "#div-report", "lbl", "html");
            $(".lblDate").html(moment(d.Date).format("DD-MM-YYYY"));
            var type = $this.GetType();
            if (type == "sale") {
                $("#lblReportTitle").html("Sales Estimate");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "purchase") {
                $(".row-sale").addClass("hide");
                $("#lblReportTitle").html("Purchase Voucher");
                $("#lblFooterNotes").html(" * All imported items are not returnable");
            }

            var html = "";
            var items = d.SaleItems;
            var index = 1;
            for (var i in items) {
                var item = items[i];
                item.SrNo = index;
                index++;
            }

            $("#report-saleOriginal tbody").html("");
            $("#report-saleCopy tbody").html("");
            Common.MapItemData(items, "#report-saleOriginal", "#template-print", true);
            Common.MapItemData(items, "#report-saleCopy", "#template-print", true);
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data.Order;
                        $("#item-container tbody").html("");
                        var isclear = $("#Cleared").is(":Checked");
                        if (d == null && isclear) {
                            $("#Id").val(0);
                            $("#AccountCode,#AccountId,#AccountName").val("");
                            focusElement = "#AccountCode";
                            $("#item-container tfoot tr td input").val("");
                            $("#qtytotal1,#GrossTotal,#Discount,#PromotionTotal,#NetTotal").val("");
                            $("#lblpreviousbalance,#lblcurrentbalance").html("");
                        }
                        else {
                            $this.CustomClear();
                        }
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            $("input:radio[value='" + SaleType.cashsale + "']").prop("checked", true);
                            $.uniform.update();
                        }
                        else {
                            //var voucher = this.GetType();
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            //if (voucher != "sale") 
                            $("#DCNo").prop("disabled", "disabled");
                            //else
                            //    $("#DCNo").removeProp("disabled");
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.SaleType == SaleType.creditsale && d.Id > 0) {
                                $("input:radio[value='" + SaleType.creditsale + "']").prop("checked", true);
                                $.uniform.update();
                            }
                            if (d.SaleType == SaleType.cashsale && d.Id > 0) {
                                $("input:radio[value='" + SaleType.cashsale + "']").prop("checked", true);
                                $.uniform.update();
                            }
                            if (d.SaleType == SaleType.sampling && d.Id > 0) {
                                $("input:radio[value='" + SaleType.sampling + "']").prop("checked", true);
                                $.uniform.update();
                            }

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                Common.MapItemData(d.SaleItems);
                                if (d.SalemanId != null) {
                                    var saleman = Common.GetById(d.SalemanId);
                                    $("#SalemanId").val(saleman.Id);
                                    $("#SalemanCode").val(saleman.AccountCode);
                                    $("#SalemanName").val(saleman.Name);
                                }
                                if (d.OrderTakerId != null) {
                                    var ordertaker = Common.GetById(d.OrderTakerId);
                                    $("#OrderTakerId").val(ordertaker.Id);
                                    $("#OrderTakerCode").val(ordertaker.AccountCode);
                                    $("#OrderTakerName").val(ordertaker.Name);
                                }
                                if (d.TerritoryManagerId != null) {
                                    var territorymanager = Common.GetById(d.TerritoryManagerId);
                                    $("#TerritoryManagerId").val(territorymanager.Id);
                                    $("#TerritoryManagerCode").val(territorymanager.AccountCode);
                                    $("#TerritoryManagerName").val(territorymanager.Name);
                                }
                                //Common.MapItemData(d.SaleItems, null, "#template-usman", true);
                                //Common.MapItemData(d.SaleItems, "#data-conatiner", null);
                                //Common.MapItemData(d.SaleItems, null, null,true);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();

                            }
                            $("#Discount").val(d.Discount);
                            $("#GSTAmountTotal").val(d.GSTAmountTotal);
                            $("#NetTotal").val(d.NetTotal);
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
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        GetPreviousBalance: function (customerid) {
            var $this = this;
            var type = $this.GetType();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/" + "?key=GetPreviousBalance&accountid=" + customerid + "&getids=GetPreviousBalanceWithIds",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var id = Common.GetInt($("#Id").val());
                        if (id == 0) {
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data.PreviousBalance).format());
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data.PreviousBalance);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html((currentbalance - invoicetotal).format());

                        }
                        //if (res.Data.SalemanId != null) {
                        //    var saleman = Common.GetById(res.Data.SalemanId);
                        //    $("#SalemanId").val(saleman.Id);
                        //    $("#SalemanCode").val(saleman.AccountCode);
                        //    $("#SalemanName").val(saleman.Name);
                        //}
                        //if (res.Data.OrderTakerId != null) {
                        //    var ordertaker = Common.GetById(res.Data.OrderTakerId);
                        //    $("#OrderTakerId").val(ordertaker.Id);
                        //    $("#OrderTakerCode").val(ordertaker.AccountCode);
                        //    $("#OrderTakerName").val(ordertaker.Name);
                        //}
                        //if (res.Data.TerritoryManagerId != null) {
                        //    var territorymanager = Common.GetById(res.Data.TerritoryManagerId);
                        //    $("#TerritoryManagerId").val(territorymanager.Id);
                        //    $("#TerritoryManagerCode").val(territorymanager.AccountCode);
                        //    $("#TerritoryManagerName").val(territorymanager.Name);
                        //}
                        $this.CalculatePreviousBalance();


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadDiscounts: function () {
            var $this = this;
            var type = $this.GetType();
            var lastDate = Common.GetData("LastDiscountDate");
            var discounts = Common.GetData("Discounts");
            var companyId = Common.GetInt(Common.GetData("DiscountCompanyId"));
            if (PageSetting.Promotions != null)
                return;
            else if (type.indexOf("sale") == -1)
                return;

            var data =
                 {
                     LastDate: lastDate,
                     key: 'loaddiscounts',
                     CompnayId: companyId
                 }
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc",
                type: "GET",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "body",
                blockMessage: "loading discounts...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data.LoadDiscounts) {
                            Common.SetData("Discounts", res.Data.Discounts);
                            Common.SetData("LastDiscountDate", res.Data.LastDate + "");
                            Common.SetData("DiscountCompanyId", res.Data.CompanyId);
                            AppData["Discounts"] = Common.GetData("Discounts");
                        }
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        CalculatePreviousBalance: function () {

            var $this = this;
            var type = $this.GetType().toLowerCase();
            var currentbalance = 0;
            var invoicetotal = Common.GetFloat($("#NetTotal").val());
            var previousbalance = Common.GetFloat($("#lblpreviousbalance").html());

            var currentbalance = 0;
            if (type == "sale" || type == "purchasereturn") {
                currentbalance = previousbalance + invoicetotal;
            }
            else {
                currentbalance = previousbalance - invoicetotal;
            }

            $("#lblcurrentbalance").html((currentbalance).format());

        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        DeleteMultiple: function (id) {
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            RebindData();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        Delete: function () {
            var $this = this;
            var type = VoucherType[$this.GetType()];

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucherNo=" + voucherno;
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;
                var id = Common.GetInt($("#Id").val());
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
                    blockMessage: "Deleting  " + $this.GetType() + " ...please wait",
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
                case "sale":
                    id = PageSetting.Customers;
                    $(".net-total-row").removeClass('hide');
                    break;
                case "salereturn":
                    id = PageSetting.Customers;
                    $(".net-total-row").removeClass('hide');
                    break;
                case "purchase":
                    id = PageSetting.Suppliers;
                    $(".net-total-row").removeClass('hide');

                    break;
                case "purchasereturn":
                    id = PageSetting.Suppliers;
                    $(".net-total-row").addClass('hide');
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

                    //$("#SalemanCode").val("");
                    //$("#SalemanId").val("");
                    //$("#SalemanName").val("");

                    //$("#OrderTakerCode").val("");
                    //$("#OrderTakerId").val("");
                    //$("#OrderTakerName").val("");

                    //$("#TerritoryManagerCode").val("");
                    //$("#TerritoryManagerId").val("");
                    //$("#TerritoryManagerName").val("");

                    var d = Common.GetByCode(ui.item.value);
                    var type = $this.GetType();

                    if (typeof d != "undefined" && d != null) {
                        if (type == "sale") {
                            $("#Comments").val("Sold To: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("Sale Return From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Purchase From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.AccountCode + "-" + d.Name);
                        }
                        $("#AccountName").val(d.Name);
                        $("#AccountId").val(d.Id);
                        Common.GetPartyAddress(d.Id);
                        $(".container-message").hide();
                    }
                    $this.MapComments();
                }
            });



        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $("#account-type-conatiner").addClass("hide");
            if (voucher == "sale" || voucher == "purchase") {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").removeClass("hide");
                $("#account-type-conatiner").removeClass("hide");
            }
            else
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").addClass("hide");
            if (voucher == "sale") {
                $(".sale-content").removeClass("hide");
                $(".other-content").addClass("hide");
            }
            else {
                $(".other-content").removeClass("hide");
                $(".sale-content").addClass("hide");
            }

            this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName,
                        name: product.DisplayName

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

                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $this.SetDiscount(tr, account, product);

                    }

                }
            });

        },
        SetDiscount: function (tr, account, product) {
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var customerId = Common.GetInt($("#AccountId").val());
            if ((voucher == "sale" || voucher == "salereturn")) {
                var promotionAmount = 0;
                var lessAmount = 0
                var date = $("#Date").val();
                date = moment(date, "DD/MM/YYYY").format("YYYY-MM-DD");
                var lessAmount = Common.GetFloat(product.SaleDiscount);
                var discount = Common.GetDiscountDetail(account.Id);
                discount = Common.GetFloat(discount);
                var customerLess = Enumerable.From(PageSetting.CustomerLesses).FirstOrDefault(null, function (x) { return x.CustomerId == customerId && x.ItemId == account.Id });
                var promotion = Enumerable.From(PageSetting.Promotions).FirstOrDefault(null, function (x) { return x.CustomerId == customerId && x.ItemId == account.Id && moment(x.FromDate).format("YYYY-MM-DD") <= date && moment(x.ToDate).format("YYYY-MM-DD") >= date })
                if (customerLess != null && customerLess.LessRate > 0) {
                    lessAmount = customerLess.LessRate;
                }
                if (promotion != null && promotion.PromotionRateSale > 0) {
                    promotionAmount = promotion.PromotionRateSale;
                }
                $(tr).find("input.DiscountPerItem").val(lessAmount);
                $(tr).find("input.PromotionPercent ").val(promotionAmount);
            }
            else {
                var promotionAmount = Common.GetFloat(product.PurchaseDiscount);
                var promotion = Enumerable.From(PageSetting.Promotions).FirstOrDefault(null, function (x) { return x.CustomerId == customerId && x.ItemId == account.Id && moment(x.FromDate).format("YYYY-MM-DD") <= date && moment(x.ToDate).format("YYYY-MM-DD") >= date && x.CustomerId == null })
                if (promotion != null && promotion.PromotionRatePurchase > 0) {
                    promotionAmount = promotion.PromotionRatePurchase;
                }
                $(tr).find("input.DiscountPerItem").val(Common.GetFloat(promotionAmount));
            }
        },
        SalemanAutoCompleteInit: function () {
            var salemans = Common.GetLeafAccounts($("#SalemanHeadId").val());
            var suggestion = new Array();
            for (var i in salemans) {
                var saleman = salemans[i];
                suggestion.push(
                    {
                        id: saleman.Id,
                        value: saleman.AccountCode,
                        label: saleman.AccountCode + "-" + saleman.DisplayName,
                        name: saleman.DisplayName
                    }
                 );

                $("#SalemanCode").autocomplete({
                    source: suggestion,
                    selectFirst: true,
                    autoFocus: true,
                    select: function (event, ui) {
                        var d = Common.GetByCode(ui.item.value);
                        if (typeof d != "undefined" && d != null) {
                            $("#SalemanId").val(d.Id);
                            $("#SalemanName").val(d.Name);
                        }

                    }

                })

            }
        },

        OrderTakerAutoCompleteInit: function () {
            var ordertakers = Common.GetLeafAccounts($("#OrderTakerHeadId").val());
            var suggestion = new Array();
            for (var i in ordertakers) {
                var ordertaker = ordertakers[i];
                suggestion.push(
                    {
                        id: ordertaker.Id,
                        value: ordertaker.AccountCode,
                        label: ordertaker.AccountCode + "-" + ordertaker.DisplayName,
                        name: ordertaker.DisplayName
                    }
                 );

                $("#OrderTakerCode").autocomplete({
                    source: suggestion,
                    selectFirst: true,
                    autoFocus: true,
                    select: function (event, ui) {
                        var d = Common.GetByCode(ui.item.value);
                        if (typeof d != "undefined" && d != null) {
                            $("#OrderTakerId").val(d.Id);
                            $("#OrderTakerName").val(d.Name);
                        }

                    }

                })

            }
        },

        TerritoryManagerAutoCompleteInit: function () {
            var territorymanagers = Common.GetLeafAccounts($("#TerritoryManagerHeadId").val());
            var suggestion = new Array();
            for (var i in territorymanagers) {
                var territorymanager = territorymanagers[i];
                suggestion.push(
                    {
                        id: territorymanager.Id,
                        value: territorymanager.AccountCode,
                        label: territorymanager.AccountCode + "-" + territorymanager.DisplayName,
                        name: territorymanager.DisplayName
                    }
                 );

                $("#TerritoryManagerCode").autocomplete({
                    source: suggestion,
                    selectFirst: true,
                    autoFocus: true,
                    select: function (event, ui) {
                        var d = Common.GetByCode(ui.item.value);
                        if (typeof d != "undefined" && d != null) {
                            $("#TerritoryManagerId").val(d.Id);
                            $("#TerritoryManagerName").val(d.Name);
                        }

                    }

                })

            }
        },

        TransporterAutoCompleteInit: function (data) {
            var $this = this;
            var tokens = data;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.Code,
                        label: token.Code + "-" + token.Name,
                        Name: token.Name

                    }
                );
            }

            $("#ShipViaCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    $("#ShipViaName").val(ui.item.Name);
                    $("#ShipViaId").val(ui.item.id);
                    $(".btn.btn-primary.green").focus();
                }


            });

        },

        GetDC: function () {
            var $this = this;
            $("#DC-Container").modal("show");
            $this.BindDCDatatable();
        },
        BindDCDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.sale || type == VoucherType.purchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.salereturn)
                type = VoucherType.goodreceive
            var url = Setting.APIBaseUrl + "DeliveryChallan?type=" + type
            var options =
             {
                 "bPaginate": false,
                 "bSort": false,
             }
            DataTable.DestroyDatatable(DC_DATATABLE_ID);
            DataTable.BindDatatable(DC_DATATABLE_ID, url, options);
        },
        SelectDC: function (tr) {
            var $this = this;
            var dcno = $(tr).find("input.DCNo ").val();
            if (dcno.trim() != "" && dcno != null) {
                $("#DCNo").val(dcno);
                $this.LoadDeliveryChallan("challan");
                $('#btnDCClose').click();
            }
        },



    };
}();