
var VatStockTransfer = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VatStockTransfer";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $("input[name='ac-type']").change(function () {

                if ($(this).val() == "cash") {
                    var acc = Common.GetById(PageSetting.CashAccount);
                    $("#AccountId").val(acc.Id);
                    $("#AccountCode").val(acc.AccountCode);
                    $("#AccountName").val(acc.DisplayName);
                    $("#AccountCode").attr("disabled", "disabled");
                    $("#PartyAddress").val("");
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

                }
            });
            $("#Date").keyup(function (e) {

                if (e.which == 13) {
                    $("#TimeOfSupply").focus();
                }
            });
            $("#InvoiceNumber").keyup(function (e) {
                var type = $this.GetType();
                $this.MapComments();
                if (e.which == 13) {
                    if (type == "gstsale")
                        $("#Date").focus().select();
                }
            });
            $("#DCNo").keyup(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "" || $(this).val() <= 0)
                        $("#Date").focus();
                    else
                        $this.LoadDeliveryChallan("challan");
                }
                $this.MapComments();
            });
            $(document).on("keyup", ".Code", function (event) {
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
                                $(tr).find("input.Rate").val(Common.GetFloat(product.SalePrice));
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.AddItem();
                            }
                            else {
                                if ($(this).val().trim() != "") {
                                    var err = $(this).val() + " is not valid code.";
                                    Common.ShowError(err);

                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();
                        }
                    }
                    else {
                        if ($("#item-container tbody tr").length > 1 && $("#item-container tbody tr:nth-last-child(1) td input.Code ").val() == "")
                            $(".btn.btn-primary.green").focus();
                        else {

                        }
                    }

                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {

                    var party = Common.GetByeCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        //setTimeout(function () {
                        //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                        //}, 500);
                    }

                    else {
                        //if ($(this).val().trim() != "") {
                        //    var err = $(this).val() + " is not valid party code.";
                        //    Common.ShowError(err);
                        //}
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
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        $(tr).find("input.Rate").val(Common.GetFloat(product.SalePrice));
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.";
                            Common.ShowError(err);

                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();

                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity,.Rate,.Amount,.GSTPercent ", function (event) {

                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    var address = party.Address;

                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    //if ($(this).val().trim() != "") {
                    //    var err = $(this).val() + " is not valid party code.";

                    //    Common.ShowError(err);
                    //}
                }
            });
            $("#SalesmanCode").keyup(function (e) {
                if (e.which == 13) {

                    var salesman = Common.GetByCode($(this).val());

                    if (typeof salesman != "undefined" && salesman != null) {
                        $("#Comments").focus().select();
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
            $("#SalesmanCode").blur(function () {
                var salesman = Common.GetByCode($(this).val());
                if (typeof salesman != "undefined" && salesman != null) {
                    $("#Comments").focus().select();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid salesman code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            $this.LoadPageSetting();
            $(document).on("click", "#DCTable > tbody tr", function () {
                $this.SelectDC(this);
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();

            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.SalesmanAutoCompleteInit();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                $this.Add();
                // }
            }
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "gstsale") {

                var html = "Bill No:" + $("#InvoiceNumber").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        ChangeType: function (type) {
            var $this = this;
            window.history.pushState(type, document.title + " | " + type, "index?type=" + type);
            //document.title = document.title + " | " + type;
            $this.LoadPageSetting();
            if ($("#div-table").is(":visible")) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            } else if ($("#form-info").is(":visible")) {
                $this.Add();
            }
            Common.HighlightMenu();
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            Common.Clear();
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
                    $(focusElement).focus().select();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }, 300);
                focusElement = "";
            }


            $("#lbldiscount").html("0 %");


            //}
            //$this.AutoCompleteInit();
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.AllowNumerics();
            Common.InitNumerics();
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
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
                //$this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                //$this.CustomClear();
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
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ")";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                $("#item-container tbody input.Rate").trigger("keyup");
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
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "Transaction total amount should be graeter then zero(0).";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }


                record["TransactionType"] = VoucherType[voucher],
                record["SaleItems"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
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
            $(tr).find("input.Amount").val(amount.toFixed(3));

            var gstpercent = Common.GetInt(PageSetting.GSTPercent);
            var gstamount = Common.GetFloat(amount * gstpercent / 100);
            var netAmount = Common.GetFloat(amount + gstamount);
            $(tr).find("input.GSTAmount ").val(gstamount.toFixed(3));
            $(tr).find("input.NetAmount").val(netAmount.toFixed(3));

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var GstAmount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                Price += Common.GetFloat($(this).find("input.Amount").val());
                GstAmount += Common.GetFloat($(this).find("input.GSTAmount").val());
                netamount += Common.GetFloat($(this).find("input.NetAmount").val());
            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity.toFixed(3));
            $("#item-container tfoot tr").find("input.Amount").val(Price.toFixed(3));
            $("#item-container tfoot tr").find("input.GSTAmount").val(GstAmount.toFixed(3));
            $("#item-container tfoot tr").find("input.NetAmount").val(netamount);
            $("#GstAmountTotal").val(GstAmount.toFixed(3));
            $("#GrossTotal").val(Price.toFixed(3));
            $("#NetTotal").val(netamount.toFixed(3));
            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetFloat($("#amounttotal").val());
            var discount = Common.GetFloat($("#Discount").val());
            var nettotal = Common.GetFloat(total - discount);

            $("#NetTotal").val(nettotal.toFixed(3));
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            $("#item-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint,#btndrftprint").prop("disabled", true);
            Common.Clear();
        },
        LoadDeliveryChallan: function (key) {

            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.gstsale || type == VoucherType.gstpurchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.gstpurchase || type == VoucherType.gstsalereturn)
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
                            //else if (d.Status == TransactionStatus.Delivered) {
                            //    Common.ShowError("Challan has already processed.");
                            //    focusElement = "#DCNo";
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

                                items = Enumerable.From(items).Where(function (x) { return (x.Quantity - x.QtyDelivered) > 0 }).ToArray();
                                Enumerable.From(items).ForEach(function (x) {
                                    x.Quantity = x.Quantity - x.QtyDelivered

                                });

                                var html = "";
                                for (var i in items) {
                                    var item = items[i];
                                    var account = Common.GetByCode(item.ItemCode);
                                    var product = Common.GetAccountDetailByAccountId(account.Id);
                                    //var price = 0;
                                    var price = item.Rate;
                                    var qty = item.Quantity;
                                    //if (typeof product != "undefined" && product != null) {
                                    //    price = Common.GetFloat(product.SalePrice);
                                    //}

                                    var amount = qty * price;
                                    var gstpercent = PageSetting.GSTPercent;
                                    var gstamount = Common.GetInt(amount) * gstpercent / 100;
                                    var netAmount = Common.GetInt(amount) + gstamount;
                                    item["Unit"] = item.Unit;
                                    //item["Rate"] = price;
                                    item["Rate"] = item.Rate;
                                    item["Amount"] = amount;
                                    item["GSTPercent"] = gstpercent;
                                    item["GSTAmount"] = gstamount;
                                    item["NetAmount"] = netAmount;

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
            var accountdetail = res.Data.AccountDetail;
            var dc = res.Data.deliverychallan;
            var productMappings = res.Data.ProductMappings;

            if (d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                if (accountdetail.ShowBarcodeBox)
                    $("#ShowBarcodeBox").removeClass('hide');
                else
                    $("#ShowBarcodeBox").addClass('hide');
                if (dc != null) {
                    $("#lblPOnumber").html(dc.PartyPONumber);
                    $("#lblDCNo").html(dc.VoucherNumber);
                }

                $("#lblDate").html(moment(d.Date).format("DD/MM/YYYY"));
                $("#lblOrderDate").html(moment(d.OrderDate).format("DD/MM/YYYY"));
                $("#lblAccountName").html(accountdetail.Name);
                if (!Common.isNullOrWhiteSpace(accountdetail.Address))
                    $("#lblPartyAddress").html(accountdetail.Address.replace("\n", "<br/>"));
                $("#lblSTRN").html(accountdetail.GST);
                $("#lblNTN").html(accountdetail.NTN);
                $("#lblGSTJurisdiction").html(accountdetail.GSTJurisdiction);
                var type = $this.GetType();
                var html = "";
                var items = d.SaleItems;
                var index = 1;
                $("#report-saleitem tbody").html("");
                for (var i in items) {
                    var item = items[i];
                    var prodMap = Enumerable.From(productMappings).Where(function (p) {
                        return p.COAProductId == item.ItemId
                    }).FirstOrDefault();
                    var hsCode = Common.GetAccountDetailByAccountId(item.ItemId).HSCode;
                    html += "<tr>";
                    html += "<td  style='text-align:center'>" + (index++) + "</td>";
                    html += "<td  style='text-align:center'>" + (prodMap ? prodMap.ManualCode : item.ItemCode) + "</td>";
                    html += "<td  style='text-align:left'>" + item.ItemName + (hsCode ? "<div>(H.S Code: " + hsCode + ")</div>" : "") + "</td>";
                    html += "<td  style='text-align:center'>" + item.Quantity + "</td>";
                    //html += "<td>" + item.Unit + "</td>";
                    html += "<td  style='text-align:center'>" + item.Rate.format() + "</td>";
                    html += "<td  style='text-align:center'>" + item.Amount.format() + "</td>";
                    html += "<td  style='text-align:center'>" + item.GSTPercent + "%" + "</td>";
                    html += "<td  style='text-align:center'>" + item.GSTAmount.format() + "</td>";
                    html += "<td  style='text-align:center'>" + "</td>";
                    html += "<td  style='text-align:center'>" + item.NetAmount.format() + "</td>";
                    html += "</tr>";
                }

                $("#report-saleitem tbody").append(html);
                var qtyTotal = Common.GetInt(Enumerable.From(items).Sum("$.Quantity")).format();
                var amountTotal = Common.GetInt(Enumerable.From(items).Sum("$.Amount")).format();
                var gstamtTotal = Common.GetInt(Enumerable.From(items).Sum("$.GSTAmount")).format();
                var grandTotal = Common.GetInt(Enumerable.From(items).Sum("$.NetAmount")).format();
                var gstpercent = Common.GetFloat(Enumerable.From(items).FirstOrDefault().GSTPercent);
                //$("#report-saleitem tfoot tr").find("td:nth-child(2)").html(qtyTotal);
                $("#report-saleitem tfoot tr").find("td:nth-child(3)").html(amountTotal);
                $("#report-saleitem tfoot tr").find("td:nth-child(5)").html(gstamtTotal);
                $("#report-saleitem tfoot tr").find("td:nth-child(7)").html(grandTotal);
                $("#tblAgingItems thead ").find("#totalamount").html(amountTotal);
                $("#tblAgingItems thead ").find("#salestax").html(gstamtTotal);
                $("#tblAgingItems thead ").find("#grandtotal").html(grandTotal);
                $this.LoadDraftReportData(res);

            }
        },
        LoadDraftReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            var partyPO = res.Data.PartyPO;
            var vehicleNo = res.Data.VehicleNo;
            var accountdetail = res.Data.AccountDetail;
            if (d != null) {
                Common.MapDataWithPrefixF(d, "#div-gstreport", "lblgst", "html");
                $("#lblgstDate").html(moment(d.Date).format("DD/MM/YYYY"));
                $("#lblgstOrderNo").html(partyPO);
                $("#lblgstOrderDate").html(moment(d.Date).format("DD/MM/YYYY"));
                $("#lblgstSTRN").html(accountdetail.GST);
                $("#lblgstNTN").html(accountdetail.NTN);
                $("#lblDraftTimeOfSupply").html(d.TimeOfSupply);
                $("#lblDraftDispatchedThrough").html(vehicleNo);
                var type = $this.GetType();
                var html = "";
                var items = d.SaleItems;
                var index = 1;
                $("#report-gstsaleitem tbody").html("");
                for (var i in items) {
                    var item = items[i];

                    html += "<tr>";
                    html += "<td>" + (index++) + "</td>";
                    html += "<td>" + item.ItemCode + "</td>";
                    html += "<td style='text-align:left'>" + item.ItemName + "</td>";
                    html += "<td>" + item.Quantity + "</td>";
                    //html += "<td>" + item.Unit + "</td>";
                    html += "<td>" + item.Rate.format() + "</td>";
                    html += "<td>" + item.Amount.format() + "</td>";
                    html += "<td>" + item.GSTPercent + "%" + "</td>";
                    html += "<td>" + item.GSTAmount.format() + "</td>";
                    html += "<td>" + item.NetAmount.format() + "</td>";
                    html += "</tr>";
                }

                $("#report-gstsaleitem tbody").append(html);

                var Quantity = Common.GetInt(Enumerable.From(items).Sum("$.Quantity")).format();
                var amountTotal = Common.GetInt(Enumerable.From(items).Sum("$.Amount")).format();
                var gstamtTotal = Common.GetInt(Enumerable.From(items).Sum("$.GSTAmount")).format();
                var grandTotal = Common.GetInt(Enumerable.From(items).Sum("$.NetAmount")).format();
                var gstpercent = Common.GetFloat(Enumerable.From(items).FirstOrDefault().GSTPercent);

                $("#report-gstsaleitem tfoot tr").find("td:nth-child(4)").html(Quantity);
                $("#report-gstsaleitem tfoot tr").find("td:nth-child(6)").html(amountTotal);
                $("#report-gstsaleitem tfoot tr").find("td:nth-child(8)").html(gstamtTotal);
                $("#report-gstsaleitem tfoot tr").find("td:nth-child(9)").html(grandTotal);

                $("#tblgstAgingItems thead ").find("#gsttotalamount").html(amountTotal);
                $("#tblgstAgingItems thead ").find("#gstamountTotal").html(gstamtTotal);
                $("#tblgstAgingItems thead ").find("#gstgrandtotal").html(grandTotal);

            }
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        var dc = res.Data.deliverychallan;
                        Common.MapEditData(d, "#form-info");
                        $("#DCNo").removeProp("disabled");
                        if (dc != null)
                            $("#OrderNo").val(dc.OrderNo);
                        if (res.Data.vendercode) {
                            $("#lblVenderCode").html(res.Data.vendercode);
                        }
                        else {
                            $("#lblVenderCode").html("");
                        }
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);

                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#DCNo").prop("disabled", "disabled");
                                $("#btndelete,#btnprint,#btndrftprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var items = d.SaleItems;
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

            //var $this = this;
            //var type = $this.GetType().toLowerCase();
            //var currentbalance = 0;
            //var invoicetotal = Common.GetFloat($("#NetTotal").val());
            //var previousbalance = Common.GetFloatHtml($("#lblpreviousbalance").html());

            //var currentbalance = 0;
            //if (type == "gstsale" || type == "gstpurchasereturn") {
            //    currentbalance = previousbalance + invoicetotal;
            //}
            //else {
            //    currentbalance = previousbalance - invoicetotal;
            //}

            //$("#lblcurrentbalance").html((currentbalance).format());

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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno;
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
                            focusElement = "#InvoiceNumber";
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
                case "gstsale":
                    id = PageSetting.Customers;
                    break;
                case "gstsalereturn":
                    id = PageSetting.Customers;
                    break;
                case "gstpurchase":
                    id = PageSetting.Suppliers;

                    break;
                case "gstpurchasereturn":
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
                    var type = $this.GetType();

                    if (typeof d != "undefined" && d != null) {
                        if (type == "gstsale") {
                            $("#Comments").val("Sold To: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstsalereturn") {
                            $("#Comments").val("Sale Return From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstpurchase") {
                            $("#Comments").val("Purchase From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstpurchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.AccountCode + "-" + d.Name);
                        }
                        $("#AccountName").val(d.Name);
                        $("#AccountId").val(d.Id);
                        $("#SalesmanCode").focus();
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
            if (voucher == "gstsale") {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo,#dc-search").removeClass("hide");
                $("#account-type-conatiner").removeClass("hide");
            }
            else {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo,#dc-search").addClass("hide");
            }
            this.LoadAccounts();
        },
        Print: function () {
            $("#div-report").removeClass('hidden-print');
            $("#div-gstreport").addClass('hidden-print');
            window.print();
        },
        DraftPrint: function () {
            $("#div-gstreport").removeClass('hidden-print');
            $("#div-report").addClass('hidden-print');
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
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        $(tr).find(":nth-child(5) input.Rate").val(Common.GetFloat(product.SalePrice));
                        $this.GetQuantityPriceTotal(tr);
                        $(".container-message").hide();
                    }
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
            if (type == VoucherType.gstsale || type == VoucherType.gstpurchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.gstpurchase || type == VoucherType.gstsalereturn)
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
                        // $("#Comments").focus();
                        $(".container-message").hide();
                    }
                }
            });



        },
    };
}();

