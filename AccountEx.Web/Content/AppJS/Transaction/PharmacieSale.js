
var PharmacieSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "PharmacieSale";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    CURRENT_VOCUHER = null;
    return {
        init: function () {
            var $this = this;
            var dataTypes = ["Sale", "Purchase", "SaleReturn", "PurchaseReturn"];
            for (var i = 0; i < dataTypes.length; i++) {
                $("a[data-type='" + dataTypes[i] + "']").click(function () {
                    var type = $(this).attr("data-type");
                    $this.ChangeType(type);
                    return false;
                });
            }
            $(".date-picker").on("show", function (e) {
                //console.debug('show', e.date, $(this).data('stickyDate'));

                if (e.date) {
                    $(this).data("stickyDate", e.date);
                }
                else {
                    $(this).data("stickyDate", null);
                }
                $(this).datepicker("hide");
            });



            Common.InitDateMask();


            $("input[name='ac-type']").change(function () {
                try {
                    if ($(this).val() == "cash") {
                        var acc = Common.GetById(PageSetting.CashAccount);
                        $("#AccountId").val(acc.Id);
                        $("#AccountCode").val(acc.AccountCode);
                        $("#AccountName").val(acc.DisplayName);
                        $("#AccountCode").attr("disabled", "disabled");
                        $(".customername-row").removeClass('hide');
                        $this.MapComments();
                    }
                    else {
                        $("#AccountCode").removeAttr("disabled");
                        $("#AccountId").val("");
                        $("#AccountCode").val("");
                        $("#AccountName").val("");
                        $(".customername-row").addClass('hide');
                    }


                } catch (e) {

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
                        if ($("input:radio[value='cash']").is(":checked")) {
                            $("#Comments").focus();

                        }
                        else
                            $("#AccountCode").focus();

                    else {
                        $("#AccountCode").focus();
                    }
                }


            });
            $(document).on("change", ".Unit", function (event) {
                //$this.CalculateUnitQuantity($(this).parent().parent());
                var tr = $(this).parent().parent();
                $this.GetQuantityPriceTotal(tr);
                $this.GetStockPreviousBalance(tr, $(tr).find(":nth-child(1) input.ItemId").val());
            });

            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
                if (e.which == 13) {
                    $("#Date").focus();

                }
            });

            $(document).on("keyup", ".Code", function (event) {
                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            if (typeof product == "undefined" && product == null) {
                                product = Common.GetByCode($(this).val());
                                product = Common.GetAccountDetailByAccountId(product.Id);
                            }
                            var tr = $(this).closest("tr");
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                var discount = $this.GetDiscountDetail(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.MainUnitQuantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(type == "sale" || type == "salereturn" ? product.SalePrice : product.PurchasePrice));
                                $("#lbllocation").html(product.Location);
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $("#item-container tbody tr:nth-last-child(1) input.MainUnitQuantity").focus().select();
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
                            $("#item-container tbody tr:nth-last-child(1) input.MainUnitQuantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
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
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = +$(this).val() + " is not valid party code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }




                }
            });
            $(document).on("blur", ".Code", function () {

                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                var options = "<option>Unit</option>";
                if (typeof account != "undefined" && account != null) {
                    var product = Common.GetAccountDetailByAccountId(account.Id);
                    var discount = $this.GetDiscountDetail(account.Id);

                    if (product.IsStrip != null && product.IsStrip)
                        options += "<option>Strip</option>";
                    if (product.IsPack != null && product.IsPack)
                        options += "<option>Pack</option>";
                    if (product.IsCotton != null && product.IsCotton)
                        options += "<option>Cotton</option>";
                    $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                    $(tr).find(":nth-child(2) input.Name").val(account.Name);
                    $(tr).find(":nth-child(6) input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                    $("#lbllocation").html(product.Location);
                    $(tr).find(":nth-child(8) input.DiscountPercent").val(Common.GetFloat(discount));
                    $(tr).find(":nth-child(4) select.Unit").html(options);
                    $(".container-message").hide();
                    $this.GetStockPreviousBalance(tr, product.AccountId);



                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = +$(this).val() + " is not valid code.";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });
            $(document).on("blur", ".MainUnitQuantity", function (event) {
                var tr = $(this).parent().parent();
                $this.DoItemCalculations(tr, event);
            });
            $(document).on("keyup", ".Quantity,.MainUnitQuantity,.Rate,.Amount,.DiscountPercent,.DiscountAmount", function (event) {
                var tr = $(this).parent().parent();
                $this.DoItemCalculations(tr, event);
            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $this.GetPreviousBalance(party.AccountId);
                    $(".container-message").hide();
                    var address = party.Address;

                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.</li>";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            AppData.CustomerDiscount = PageSetting.Discounts;
            $("#full-screen").click();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                if (Setting.PageLandingView == "DetailView") {
                    this.Add();
                } else {
                    this.ListView();
                }
            }
            $("input[name='ac-type']").trigger("change");
        },
        DoItemCalculations: function (tr, event) {
            var $this = this;
            var qty = Common.GetInt($(tr).find("input.Quantity").val());
            if (qty == 0) qty = Common.GetInt($(tr).find("input.MainUnitQuantity").val());
            if (qty == 0) {
                $(tr).find("input.MainUnitQuantity,input.Quantity").val(1);
                qty = 1;
            }
            var code = $(tr).find("input.Code").val();
            $this.GetQuantityPriceTotal(tr);
            if (event.which == 13 && qty > 0)
                $this.AddItem();
            else if (event.which == 13 && qty <= 0) {
                var err = "Item " + code + " must have quantity greater than zero(0).";
                Common.ShowError(err);
                $(tr).find("input.Quantity").focus();
            }
            $this.CalculateStockPreviousBalance(tr, $(tr).find("input.ItemId").val());
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val();
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
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        GetDiscountDetail: function (productid) {


            var customerid = 0;
            var discount = $.grep(AppData.CustomerDiscount, function (e) { return e.CustomerId == customerid && e.COAProductId == productid; })[0];
            if (discount != null)
                return discount.Discount;
            else
                return 0;
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            this.DetailView();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        ListView: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (LIST_LOADED) {
                if (LIST_CHANGED) {
                    this.RebindData();
                    LIST_CHANGED = false;
                }
            } else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
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
            Common.InitNumerics();
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
            $("#lbldiscount").html("0 %");


            //}
            Common.InitDateMask();
            //$this.AutoCompleteInit();
            //if (!PageSetting.BarCodeEnabled) {
            $this.AutoCompleteInit();
            //}

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
                focusElement = "date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });


            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                //$this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                //$this.CustomClear();
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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {
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
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        if (res.Success) {

                            $this.LoadReportData(res);
                            $this.Print();
                            $this.GetNextVoucherNumber();

                            callback();
                        } else {
                            Common.ShowError(res.Error.split(","));
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        GetQuantityPriceTotal: function (tr) {
            var $this = this;
            $this.CalculateUnitQuantity(tr);
            var Quantity = 0;
            var Rate = 0;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(amount);

            var discount = Common.GetFloat($(tr).find(":nth-child(8) input.DiscountPercent").val());
            var discountAmount = Common.GetFloat(amount) * discount / 100;
            var netAmount = Common.GetFloat(amount) - discountAmount;
            $(tr).find("input.DiscountAmount ").val(discountAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(Common.GetFloat(netAmount.toFixed(3)));
            $("#DiscountPercent").val("0");
            $this.GetWholeTotal();
        },
        GetWholeTotal: function (isedit) {
            var $this = this;
            var MainUnitQuantity = 0;
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                MainUnitQuantity += Common.GetFloat($(this).find(":nth-child(3) input.MainUnitQuantity ").val());
                Price += Common.GetFloat($(this).find(":nth-child(7) input.Amount").val());
                Quantity += Common.GetFloat($(this).find(":nth-child(5) input.Quantity").val());
                discount += Common.GetFloat($(this).find(":nth-child(9) input.DiscountAmount").val());
                netamount += Common.GetFloat($(this).find(":nth-child(10) input.NetAmount").val());
            });
            Quantity = Quantity.toFixed(2);
            Price = Price.toFixed(2);
            $("#item-container tfoot tr").find(":nth-child(2) input.MainUnitQuantity").val(MainUnitQuantity.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(3) input.Quantity").val(Quantity);
            $("#item-container tfoot tr").find(":nth-child(4) input.Amount").val(Price);
            $("#item-container tfoot tr").find(":nth-child(5) input.DiscountAmount").val(discount.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(6) input.NetAmount").val(netamount.toFixed(2));
            $("#qtytotal1").val(Quantity);
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            //var incAmount = Price - discount;
            var incAmount = Price;
            if (typeof isedit != undefined && isedit) {

            }
            else
                $("#Discount").val(discount);

            $("#GrossTotal").val(incAmount);
            $("#NetTotal").val(incAmount + "");
            $this.CalculatePreviousBalance();
            $this.GetNetTotal();
        },
        GetNetTotal: function () {

            var total = Common.GetFloat($("#GrossTotal").val());
            var discount = Common.GetFloat($("#Discount").val());
            var nettotal = Common.GetInt(Math.ceil(total - discount));

            $("#NetTotal").val(nettotal);
        },
        CustomClear: function () {

            $("#item-container tbody").html("");
            $("#btndelete,#btnprint").prop("disabled", true);
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            Common.Clear();
            $("input:radio[value='cash']").prop("checked", true).trigger("change");
            $.uniform.update();

        },
        SetCashAccount: function () {

        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            var items = d.SaleItems;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            //$(".total-amount").html(d.NetTotal)
            $("#lblDate").html(moment(d.Date).format("DD-MM-YYYY"));
            $("#lblTime").html(moment(d.CreatedDate).format("hh:mm"));
            $("#lblVoucherNumber1").html(d.VoucherNumber);
            if (d.CashSale) {
                $("#lblCustomerName").closest("div").removeClass('hide');
                $("#lblCustomerName").html(d.CustomerName);
            }
            else {
                $("#lblCustomerName").closest("div").addClass('hide');

            }


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
            var index = 1;
            $("#report-saleitem tbody").html("");
            var isdcounted = false;
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                // html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";

                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td class='col-discount12 hide'>" + item.DiscountAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
                if (item.DiscountAmount > 0)
                    isdcounted = true;
            }
            $("#report-saleitem tbody").append(html);
            if (d.NetTotal > 0)
                $(".col-discount").removeClass("hide");
            else
                $(".col-discount").addClass("hide");
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
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        CURRENT_VOCUHER = d;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#DCNo").prop("disabled", "disabled");
                            //alert($("#Date").val());
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (!d.CashSale && d.Id > 0) {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                                $("input:radio[value='credit']").trigger("change");

                            }
                            else {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0) {
                                    $("#btndelete,#btnprint").prop("disabled", false);

                                }
                                $("input:radio[value='cash']").trigger("change");
                            }

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.SaleItems;
                                Common.MapItemData(items);
                                //for (var i in items) {
                                //    var item = items[i];
                                //    html = "";
                                //    var options = "<option>Unit</option>";
                                //    var product = $this.GetByCode(item.ItemCode);
                                //    if (typeof product != "undefined" && product != null) {

                                //        if (product.IsStrip != null && product.IsStrip)
                                //            options += "<option>Strip</option>";
                                //        if (product.IsPack != null && product.IsPack)
                                //            options += "<option>Pack</option>";
                                //        if (product.IsCotton != null && product.IsCotton)
                                //            options += "<option>Cotton</option>";
                                //    }

                                //    $("#item-container tbody").append(html);
                                //    //$("#item-container tbody").find("tr:last-child select.Unit").val(item.Unit);
                                //}

                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                Common.InitDateMask();
                                $this.GetWholeTotal(true);

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
        GetStockPreviousBalance: function (row, itemid) {
            var $this = this;
            var type = $this.GetType();
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
            var currentbalance = 0;
            var id = 0;
            var invoicetotal = Common.GetFloat($("#NetTotal").val());
            var previousbalance = Common.GetFloat($("#lblpreviousbalance").html());
            var qty = $(row).find(":nth-child(5) input.Quantity").val();
            var currentstock = Common.GetInt($("#lblcurrentstock").html());
            var balance = 0;
            if (type == "sale") {
                if (CURRENT_VOCUHER == null) {
                    balance = currentstock - qty;
                    $("#lblremainingstockbalance").html(balance);
                }
                else if (CURRENT_VOCUHER.Id > 0 && CURRENT_VOCUHER.saleitem != null && CURRENT_VOCUHER.saleitem.length > 0) {
                    var currentitem = $.grep(CURRENT_VOCUHER.saleitem, function (e) { return e.ItemId == itemid })[0];
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
                else if (CURRENT_VOCUHER.Id > 0 && CURRENT_VOCUHER.saleitem != null && CURRENT_VOCUHER.saleitem.length > 0) {
                    var currentitem = $.grep(CURRENT_VOCUHER.saleitem, function (e) { return e.ItemId == itemid })[0];
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
        GetCustomerProducts: function (id) {
            var $this = this;
            AppData.CustomerDiscount = PageSetting.Discounts;
        },
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;

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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno;
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
        ShowAttachments: function (el) {
            // MEDIA_ELEMENT = el;
            $("#dialogAttachments").addClass("in");
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
            voucher = voucher.toLowerCase();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $("#account-type-conatiner").addClass("hide");
            if (voucher == "sale") {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").removeClass("hide");
                $("#account-type-conatiner").removeClass("hide");
            }
            else
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").addClass("hide");
            if (voucher == "sale") {
                $(".sale-content").removeClass("hide");
                $(".other-content").addClass("hide");
                var acc = Common.GetById(PageSetting.CashAccount);
                $("#AccountId").val(acc.Id);
                $("#AccountCode").val(acc.AccountCode);
                $("#AccountName").val(acc.DisplayName);
                $("#AccountCode").attr("disabled", "disabled");
            }
            else {
                $(".other-content").removeClass("hide");
                $(".sale-content").addClass("hide");
            }

            this.LoadAccounts();
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
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                position: { collision: "flip" },
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    var options = "<option>Unit</option>";
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = $this.GetDiscountDetail(account.Id);

                        if (product.IsStrip != null && product.IsStrip)
                            options += "<option>Strip</option>";
                        if (product.IsPack != null && product.IsPack)
                            options += "<option>Pack</option>";
                        if (product.IsCotton != null && product.IsCotton)
                            options += "<option>Cotton</option>";
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(6) input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $("#lbllocation").html(product.Location);
                        $(tr).find(":nth-child(8) input.DiscountPercent").val(Common.GetFloat(discount));
                        $(tr).find(":nth-child(4) select.Unit").html(options);
                        $(".container-message").hide();
                        //$(tr).find("input.MainUnitQuantity").focus();



                    }

                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                },
                //open: function () {
                //    $("ul.ui-menu").width($(this).innerWidth());

                //}
            });

        },
        CalculateUnitQuantity: function (tr) {

            var $this = this;
            var account = Common.GetByCode($(tr).find(":nth-child(1) input.Code").val());
            var unit = $(tr).find(":nth-child(4) select.Unit").val();
            if (unit == null)
                unit = "unit";
            var qty = Common.GetFloat($(tr).find(":nth-child(3) input.MainUnitQuantity").val());
            unit = unit.toLowerCase();
            if (typeof account != "undefined" && account != null) {
                var medicine = Common.GetAccountDetailByAccountId(account.Id);
                var unitqty = qty;

                if (unit == "unit") {
                    unitqty = qty;

                }
                else if (unit == "strip") {

                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        unitqty = qty * Common.GetFloat(medicine.Quantity);
                    }



                }
                else if (unit == "pack") {

                    if (medicine.IsCotton != null && medicine.IsCotton) {
                        qty = qty * Common.GetFloat(medicine.PPCQuantity);
                    }
                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        qty = qty * Common.GetFloat(medicine.SPPQuantity);
                        unitqty = qty * Common.GetFloat(medicine.Quantity);
                    }
                    unitqty = qty * Common.GetFloat(medicine.Quantity);
                }
                else if (unit == "cotton") {

                    if (medicine.IsPack != null && medicine.IsPack) {
                        qty = qty * Common.GetFloat(medicine.SPPQuantity);
                    }

                    if (medicine.IsPack != null && medicine.IsPack) {
                        qty = qty * Common.GetFloat(medicine.SPPQuantity);
                    }
                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        qty = qty * Common.GetFloat(medicine.SPPQuantity);
                        unitqty = qty * Common.GetFloat(medicine.Quantity);
                    }
                }

            }
            $(tr).find(":nth-child(5) input.Quantity").val(unitqty);
        },
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};