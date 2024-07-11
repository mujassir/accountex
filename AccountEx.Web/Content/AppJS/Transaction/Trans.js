
var Trans = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "Trans";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    CURRENT_VOCUHER = null;
    return {
        init: function () {
            var $this = this;
            Common.BindShortKeys($this);
            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            $("input[name='ac-type']").change(function () {
                if ($(this).val() == "cash") {
                    var acc = Common.GetById(PageSetting.CashAccount);
                    $("#AccountId").val(acc.Id);
                    $("#AccountCode").val(acc.AccountCode);
                    $("#AccountName").val(acc.DisplayName);
                    $("#AccountCode").attr("disabled", "disabled");
                    $this.MapComments();
                }
                else {
                    $("#AccountCode").removeAttr("disabled");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                    $("#AccountName").val("");
                }

            });
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
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    if (type == "sale")
                        $("#DCNo").focus();
                    else
                        $("#AccountCode").focus();
                }
            });
            $("#Date").focus(function (e) {
                $(this).select();
            });
            $("#BiltyNo").keyup(function (e) {

                if (e.which == 13) {
                    $("#ShipViaCode").focus();

                }
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
                            if (typeof product == "undefined" && product == null) {
                                product = Common.GetByCode($(this).val());
                                product = Common.GetAccountDetailByAccountId(product.Id);
                            }
                            if (typeof product != "undefined" && product != null) {

                                var discount = Common.GetDiscountDetail(product.AccountId);
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(type == "sale" || type == "salereturn" ? product.SalePrice : product.PurchasePrice));
                                $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.GetStockPreviousBalance(tr, $(tr).find("input.ItemId").val());
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
                var tr = $(this).closest("tr");
                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var account = Common.GetByCode($(this).val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var discountPercent = Common.GetFloat($(tr).find("input.DiscountPercent").val());


                if (typeof account != "undefined" && account != null) {
                    var product = Common.GetAccountDetailByAccountId(account.Id);
                    var discount = Common.GetDiscountDetail(product.AccountId);
                    $(tr).find("input.ItemId").val(product.AccountId);
                    $(tr).find("input.Name").val(account.Name);
                    if (rate <= 0)
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                    if (discountPercent <= 0)
                        $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                    $(".container-message").hide();
                    debugger;
                    $this.GetStockPreviousBalance(tr, $(tr).find("input.ItemId").val());
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.,";
                        Common.ShowError(err);
                    }
                }

            });
            $(document).on("blur", ".DiscountAmount", function () {

            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find("input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();
                }
                $this.GetQuantityPriceTotal(tr);
            });

            $(document).on("keyup", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var itemId = Common.GetInt($(tr).find("input.ItemId").val());
                $this.CalculateStockPreviousBalance(tr, itemId);
            });




            $(document).on("keyup", ".Quantity,.Rate,.Amount,.DiscountPercent,.DiscountAmount", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).,";
                    Common.ShowError(err);
                }
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
            $this.LoadPageSetting();
            //$(document).on("click", "#DCTable > tbody tr", function () {
            //    $this.SelectDC(this);
            //});
            $(document).on("click", "#select-dc", function () {
                $this.AddSelectedChallans();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();

            AppData.AccountDetail = PageSetting.AccountDetails;
            AppData.CustomerDiscount = PageSetting.Discounts;
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                this.Add();
                //} else {
                //    this.ListView();
                //}
            }

        },
        AddSelectedChallans: function () {
            var $this = this;
            var html = "";
            var dcIds = "";
            var dcnumbers = "";
            $("#DCTable > tbody tr").each(function () {
                var tr = $(this);
                if ($(tr).find("td:nth-child(1) input").is(":checked")) {
                    var dcId = $(tr).find("td:nth-child(2) input.DcId").val();
                    dcIds += dcId + ",";
                    var dcnumber = $(tr).find("td:nth-child(2) input.DCNo").val();
                    dcnumbers += dcnumber + ",";
                    html += "<tr>" + $(tr).html() + "</tr>"
                }
            });
            dcIds = dcIds.replace(/(^,)|(,$)/g, "");
            dcnumbers = dcnumbers.replace(/(^,)|(,$)/g, "");
            $("#table-dc-detail > tbody").html(html);
            $("#item-container tbody").html("");
            $("#table-dc-detail > tbody tr").find("td:nth-child(1)").remove();
            $("#DC-Container").modal("hide");
            if (dcIds.trim() == "") {
                $("#item-container tbody,#table-dc-detail > tbody").html("");
                $this.AddItem();
                return;
            }
            $("#DCNo").val(dcnumbers);
            $this.LoadDeliveryChallan("loadcs", dcIds);
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
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
            //  if (Common.Validate($("#addrow"))) {
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
            //if (!PageSetting.BarCodeEnabled) {
            $this.AutoCompleteInit();
            //}
            $this.SetFormControl();
            Common.InitNumerics();
        },
        SetFormControl: function () {
            if ($("#table-dc-detail tbody").children().length > 0) {
                $("#item-container tbody tr td:nth-last-child(1) span.action").remove();
                $("#item-container tbody tr:nth-last-child(1)").remove();
                $("#item-container tbody tr input:not(input.Rate)").prop("disabled", true);
            }
            var Id = Common.GetInt($("#Id").val());
            if (Id > 0)
                $(".btn-load,#dc-search").addClass("hide");
            else
                $(".btn-load,#dc-search").removeClass("hide");

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
                $this.CustomClear();
                focusElement = "#Date";
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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();
            var invoiceDcs = new Array();
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
                $("#item-container tbody input.Rate").trigger("keyup");
                $("#table-dc-detail > tbody tr").each(function () {
                    var tr = $(this);
                    invoiceDcs.push(
                        {
                            SaleId: Common.GetInt($("#Id").val()),
                            DcId: Common.GetInt($(tr).find("td:nth-child(1) input.DcId").val()),
                            DcNumber: Common.GetInt($(tr).find("td:nth-child(1) input.DCNo").val())
                        });
                });

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += "" + $("#AccountCode").val() + " is not valid party code.,";
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
                record["TransactionType"] = VoucherType[voucher],
                record["SaleItems"] = Items;
                record["InvoiceDcs"] = invoiceDcs;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        if (res.Success) {

                            if (record.Id > 0 && $("#ChkGetNextVoucher").is(":checked")) {
                                $("#VoucherNumber").val(Common.GetInt(record.VoucherNumber) + 1);
                                $this.LoadVoucher("same");
                            }
                            else {
                                $this.GetNextVoucherNumber();
                            }

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
            var discount = Common.GetFloat($(tr).find("input.DiscountPercent").val());
            var discountAmount = Common.GetFloat(amount) * discount / 100;
            var netAmount = Common.GetInt(amount - discountAmount);
            $(tr).find("input.DiscountAmount ").val(discountAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var discount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                Price += Common.GetFloat($(this).find("input.Amount").val());
                discount += Common.GetFloat($(this).find("input.DiscountAmount").val());
                netamount += Common.GetInt($(this).find("input.NetAmount").val());
            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity.toFixed(2));
            $("#item-container tfoot tr").find("input.Amount").val(Price.toFixed(2));
            $("#item-container tfoot tr").find("input.DiscountAmount").val(discount.toFixed(2));
            $("#item-container tfoot tr").find("input.NetAmount").val(netamount);
            $("#qtytotal1").val(Quantity.toFixed(2));
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            if (PageSetting.ShowDiscount) {
                var incAmount = Common.GetInt(Price) - discount;
                $("#Discount").val(discount);
                $("#NetTotal").val(Common.GetInt(incAmount));
            }
            else {
                var discountPercent = $("#DiscountPercent").val();
                if (discountPercent > 0)
                    $("#DiscountPercent").trigger("keyup");
                else
                    $("#Discount").trigger("keyup");

            }

            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetFloat($("#GrossTotal").val());
            var discount = Common.GetFloat($("#Discount").val());
            var nettotal = Common.GetInt(Math.ceil(total - discount));

            $("#NetTotal").val(nettotal);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            $("#lblcurrentstock").html("00");
            $("#lblremainingstockbalance").html("00");
            Common.Clear();
        },
        LoadDeliveryChallan: function (key, dcIds) {
            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.sale || type == VoucherType.purchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.salereturn)
                type = VoucherType.goodreceive
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?type=" + type + "&key=" + key + "&dcIds=" + dcIds,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        const data = res.Data;
                        $("#AccountName").val(data.AccountName);
                        $("#AccountCode").val(data.AccountCode);
                        $("#AccountId").val(data.AccountId);
                        $("#PartyAddress").val(data.PartyAddress);
                        $("#Comments").val(data.Comments);

                        var items = data.Items;
                        $("#item-container tbody").html("");
                        if (items != null && items.length > 0) {
                            var html = "";
                            for (var i in items) {
                                var item = items[i];
                                var account = Common.GetByCode(item.ItemCode);
                                //var product = Common.GetAccountDetailByAccountId(account.Id);
                                var price = item.Rate;

                                items[i].Quantity = item.Quantity = item.Quantity;
                                var qty = item.Quantity;
                                //if (typeof product != "undefined" && product != null) {
                                //    price = Common.GetFloat(product.SalePrice);
                                //}

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

                            $this.MapComments();

                            //setTimeout(function () {
                            //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                            //}, 500);

                        }
                        $this.GetWholeTotal();

                        $this.AddItem();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadReportData: function (res) {
            debugger;
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
            var type = $this.GetType();
            if (type == "sale") {
                //$("#lblReportTitle").html("Sales Estimate");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "salereturn") {
                $("#lblReportTitle").html("Sale Return");
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
            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td class='align-right'>" + item.Quantity + "</td>";
                //html += "<td>" + item.Unit + "</td>";
                html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                html += "<td class='align-right'>" + item.Amount.format() + "</td>";
                html += "<td class='align-right'>" + item.DiscountAmount.format() + "</td>";
                html += "<td class='align-right'>" + Common.GetInt(item.NetAmount).format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

            html = "";
            $("#tblAgingItems tbody").html("");
            for (var i in res.Data.AgingItems) {
                var item = res.Data.AgingItems[i];
                html += "<tr>";
                html += "<td></td>";
                html += "<td>" + item.VoucherNumber + "</td>";
                html += "<td></td>";
                html += "<td>" + moment(item.Date).format("DD/MM/YYYY") + "</td>";
                html += "<td></td>";
                html += "<td class='align-right'>" + item.DueAmount.format() + "</td>";
                html += "<td></td>";
                html += "<td class='align-right'>" + item.Balance.format() + "</td>";
                html += "<td></td>";
                html += "<td class='align-right'>" + item.Age + "</td>";
                html += "</tr>";
            }
            html += "<tr style='font-size:1.2em;'><th colspan='6' class='align-right' style='padding-top:10px;'>Current Balance :</th><th colspan='4' class='align-center' style='padding-top:10px;'>" + res.Data.Balance.format() + "</th></tr>";
            $("#tblAgingItems tbody").append(html);
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

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;

                        var dcs = res.Data.DeliveryChallans;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
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
                            if (d.CashSale && d.Id > 0) {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }
                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                CURRENT_VOCUHER = d;
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                Common.MapItemData(d.SaleItems);
                                var dchtml = "";
                                var dcnumbers = "";
                                for (var i in dcs) {
                                    var item = dcs[i];
                                    dcnumbers += item.VoucherNumber + ",";
                                    dchtml += "<tr>";
                                    dchtml += "<td>";
                                    dchtml += "<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' /><input type='text' class='DcId hide' value='" + item.Id + "' />";
                                    dchtml += item.VoucherNumber + "</td>";
                                    dchtml += "<td>" + item.AccountCode + "</td>";
                                    dchtml += "<td>" + item.AccountName + "</td>";
                                    dchtml += "<td>" + item.OrderNumber + "</td>";
                                    dchtml += "<td>" + Common.FormatDate(item.Date, "ddd, MMM DD, YYYY") + "</td>";
                                    dchtml += "</tr>";
                                }
                                dcnumbers = dcnumbers.replace(/(^,)|(,$)/g, "");
                                $("#table-dc-detail tbody").html(dchtml);
                                $("#DCNo").val(dcnumbers);
                                //Common.MapItemData(d.SaleItems, null, "#template-usman", true);
                                //Common.MapItemData(d.SaleItems, "#data-conatiner", null);
                                //Common.MapItemData(d.SaleItems, null, null,true);
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
                url: Setting.APIBaseUrl + "Misc/?key=GetPreviousBalance&accountid=" + customerid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var id = Common.GetInt($("#Id").val());
                        if (id == 0) {
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data).format());
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html((currentbalance - invoicetotal).format());

                        }
                        $this.CalculatePreviousBalance();


                    } else {
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
            var pBalanceStr = $("#lblpreviousbalance").html();
            var previousbalance = Common.GetFloatHtml(pBalanceStr);
            if (pBalanceStr.indexOf("(") != -1)
                previousbalance = -previousbalance;

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
                    break;
                case "salereturn":
                    id = PageSetting.Customers;
                    break;
                case "purchase":
                    id = PageSetting.Suppliers;

                    break;
                case "purchasereturn":
                    id = PageSetting.Suppliers;
                    break;
            }



            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var tokens = Common.GetAllLeafAccounts(exids);
            //for Rahat Bahi only
            if (StorageKey == '39fa9ec190eee7b6f4dff1100d6343e10918d044c75eac8f9e9a2596173f80c9')
            {
                tokens = Common.GetLeafAccounts(id);
            }
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
                    var type = $this.GetType();

                    if (typeof d != "undefined" && d != null) {
                        if (type == "sale") {
                            $("#Comments").val("To Invoice No. " + $("#InvoiceNumber").val());
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("To Invoice No. " + $("#InvoiceNumber").val());
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Bill No. " + $("#InvoiceNumber").val());

                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Bill No. " + $("#InvoiceNumber").val());
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
            if (voucher == "sale") {
                $("#DCNo,#dc-search,#lbldc,#lblorderno,#OrderNo").removeClass("hide");
                $("#account-type-conatiner").removeClass("hide");
            }
            else
                $("#DCNo,#dc-search,#lbldc,#lblorderno,#OrderNo").addClass("hide");
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
        Print: function (printType) {
            var Id = $("#Id").val();
            if (printType == 1) {
                $("#tblAgingItems").removeClass("hide");
                window.print();
            }
            else if (printType == 2) {
                $("#tblAgingItems").addClass("hide");
                window.print();
            }
            else {
                window.open('../Print/SaleInvoice?Id=' + Id);
            }

        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
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
                        //var discount = Common.GetDiscountDetail(account.Id);
                        var discount = 0;
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        //$(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                        $this.GetQuantityPriceTotal(tr);
                        $this.GetStockPreviousBalance(tr, $(tr).find("input.ItemId").val());

                    }

                }
            });

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
            dcNumbers = "";
            var $this = this;
            $("#table-dc-detail > tbody tr").each(function () {
                var tr = $(this);

                var dcNumber = $(tr).find("td:nth-child(1) input.DCNo").val();
                dcNumbers += dcNumber + ",";

            });
            dcNumbers = dcNumbers.replace(/(^,)|(,$)/g, "")
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.sale || type == VoucherType.purchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.salereturn)
                type = VoucherType.goodreceive
            var url = Setting.APIBaseUrl + "DeliveryChallan?type=" + type + "&dcNumbers=" + dcNumbers;
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

        GetStockPreviousBalance: function (row, itemid) {
            debugger;
            var $this = this;
            var type = $this.GetType().toLowerCase();
            if (!PageSetting.CheckStockAvailability || type != "sale")
                return;



            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetStockPreviousBalance&accountid=" + itemid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        $("#lblcurrentstock").html(res.Data);

                        $this.CalculateStockPreviousBalance(row, itemid);


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        CalculateStockPreviousBalance: function (row, itemid) {
            var $this = this;
            var type = $this.GetType().toLowerCase();

            if (!PageSetting.CheckStockAvailability || type != "sale")
                return;

            var currentbalance = 0;
            var id = 0;
            var invoicetotal = Common.GetFloat($("#NetTotal").val());
            var previousbalance = Common.GetFloat($("#lblpreviousbalance").html());
            var qty = Common.GetFloat($(row).find("input.Quantity").val());
            var currentstock = Common.GetFloat($("#lblcurrentstock").html());
            var balance = 0;
            if (type == "sale") {
                if (CURRENT_VOCUHER == null) {
                    balance = currentstock - qty;
                    $("#lblremainingstockbalance").html(balance);
                }
                else if (CURRENT_VOCUHER.Id > 0 && CURRENT_VOCUHER.SaleItems != null && CURRENT_VOCUHER.SaleItems.length > 0) {
                    var currentitem = $.grep(CURRENT_VOCUHER.SaleItems, function (e) { return e.ItemId == itemid })[0];
                    if (currentitem != null) {
                        qty = qty - currentitem.Quantity;
                        balance = currentstock - qty;
                        $("#lblremainingstockbalance").html(balance);
                    }
                    else {
                        balance = currentstock - qty;
                        $("#lblremainingstockbalance").html(balance);
                    }
                }
                if (balance < 0) {
                    //$(row).find(':nth-child(5) input.Quantity').val(currentstock)
                    //$this.CalculateStockPreviousBalance(row, itemid);
                }

            }
            else if (type = "salereturn") {
                if (CURRENT_VOCUHER == null) {
                    balance = currentstock + qty;
                    $("#lblremainingstockbalance").html(balance);
                }
                else if (CURRENT_VOCUHER.Id > 0 && CURRENT_VOCUHER.SaleItems != null && CURRENT_VOCUHER.SaleItems.length > 0) {
                    var currentitem = $.grep(CURRENT_VOCUHER.SaleItems, function (e) { return e.ItemId == itemid })[0];
                    if (currentitem != null) {
                        qty = qty - currentitem.Quantity;
                        balance = currentstock + qty;
                        $("#lblremainingstockbalance").html(balance);
                    }
                    else {
                        balance = currentstock + qty;
                        $("#lblremainingstockbalance").html(balance);
                    }
                }
                if (balance < 0) {
                    //$(row).find(':nth-child(5) input.Quantity').val(currentstock)
                    //$this.CalculateStockPreviousBalance(row, itemid);
                }

            }

        },
    };
}();