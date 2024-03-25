
var PharmacyBonusPurchase = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "PharmacyBonusSale";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
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
                if (e.date) {
                    $(this).data("stickyDate", e.date);
                }
                else {
                    $(this).data("stickyDate", null);
                }
                $(this).datepicker("hide");
            });
            $(".date-picker").inputmask("d/m/y", {
                "placeholder": "dd/mm/yyyy"
            }); //multi-char placeholder

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

                }
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
                //$this.GetStockPreviousBalance(tr, $(tr).find(":nth-child(1) input.ItemId").val());
            }); 
            $(document).on("keyup change", ".GSTPercent", function (event) {
                var tr = $(this).parent().parent();
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("change", ".BonusUnit", function (event) {
                var tr = $(this).parent().parent();
                $this.CalculateUnitQuantity(tr, false);
                $this.CalculateUnitQuantity(tr, true);
            });
            $("#InvoiceNumber").keyup(function (e) {

                if (e.which == 13) {
                    $("#Date").focus();

                }
            });

            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
            });


            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).parent().parent();
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                var discount = $this.GetDiscountDetail(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
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
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.MainUnitQuantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }
                //var type = $this.GetType();
                //if (event.which == 13) {
                //    if ($(this).val() == "") {
                //        if (type != "sale")
                //            $(".btn.btn-primary.green").focus();
                //        else
                //            $("#BiltyNo").focus();

                //    }
                //    else {
                //        $("#item-container tbody tr:nth-last-child(1) td:nth-child(5) input.Quantity").val(1);
                //        $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.MainUnitQuantity").val(1).focus().select();
                //    }

                //}
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
                            var err = "<li>" + $(this).val() + " is not valid party code.</li>";
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
                    $(tr).find("input.ItemId").val(account.Id);
                    $(tr).find("input.Name").val(account.Name);
                    $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                    $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                    $(tr).find("select.Unit").html(options);
                    $(tr).find("select.BonusUnit").html(options);
                    $(".container-message").hide();




                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "<li>" + $(this).val() + " is not valid code.</li>";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });

            $(document).on("blur", ".DiscountAmount", function () {

                //$this.AddItem();

            });
            $(document).on("keyup", " .BonusMainUnitQuantity", function (event) {

                var tr = $(this).parent().parent();
                $this.CalculateUnitQuantity(tr, false);
                $this.CalculateUnitQuantity(tr, true);

                //$this.GetTotalBonusQty(tr);

            });
            $(document).on("blur", ".MainUnitQuantity", function (event) {

                var tr = $(this).parent().parent();
                $this.DoItemCalculations(tr, event);
            });


            $(document).on("keyup", ".Quantity,.MainUnitQuantity,.Rate,.Amount,.DiscountPercent,.DiscountAmount,.ExpiryDate ", function (event) {

                var tr = $(this).parent().parent();
                $this.DoItemCalculations(tr, event);
            });

            $("#AccountCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
                    $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    Common.GetPartyAddress(party.Id);
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "<li>" + $(this).val() + " is not valid party code.</li>";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });


            this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            AppData.CustomerDiscount = PageSetting.Discounts;
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }
            $("input[name='ac-type']").trigger("change");
        },
        GetTotalBonusQty: function (tr) {
            var $this = this;
            $this.CalculateUnitQuantity(tr, true);
            var unitQty = 0;
            var bonusQuantity = 0;
            var mainBonusQty = 0;
            unitQty = Common.GetInt($(tr).find(":nth-child(5) input.Quantity").val());
            bonusQuantity = Common.GetInt($(tr).find(":nth-child(7) input.BonusQuantity").val());
            mainBonusQty = unitQty + bonusQuantity;
            Common.GetInt($(tr).find(":nth-child(8) input.BonusMainUnitQuantity").val(mainBonusQty));
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
                var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                Common.ShowError(err);
                $(tr).find("input.Quantity").focus();
            }
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
            Common.Clear();
            this.DetailView();
            this.CustomClear();
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

            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
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
                    err += "<li>" + $("#AccountCode").val() + " is not valid party code.</li>";
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += "<li>" + item.ItemCode + " is not valid code.</li>";
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
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        GetQuantityPriceTotal: function (tr) {
            var $this = this;
            $this.CalculateUnitQuantity(tr, false);
            var Quantity = 0;
            var Rate = 0;
            Quantity = Common.GetFloat($(tr).find(":nth-child(5) input.Quantity").val());
            Rate = Common.GetFloat($(tr).find(":nth-child(9) input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(10) input.Amount").val(amount);

            var discount = Common.GetFloat($(tr).find(":nth-child(11) input.DiscountPercent").val());
            var discountAmount = Common.GetFloat(amount) * discount / 100;
            $(tr).find(":nth-child(12) input.DiscountAmount ").val(discountAmount.toFixed(2));

            var gstpercent = Common.GetInt($(tr).find(":nth-child(13) input.GSTPercent").val());
            var gstamount = Common.GetFloat(amount * gstpercent / 100);
            var netAmount = Common.GetFloat(amount - discountAmount) + gstamount;

            $(tr).find("input.GSTAmount ").val(gstamount.toFixed(2));
            $(tr).find("input.NetAmount").val(Common.GetFloat(netAmount.toFixed(3)));

            $this.GetWholeTotal();
        },
        GetWholeTotal: function (isedit) {
            var $this = this;
            var MainUnitQuantity = 0;
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            var gstamount = 0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                MainUnitQuantity += Common.GetFloat($(this).find(":nth-child(3) input.MainUnitQuantity ").val());
                Price += Common.GetFloat($(this).find(":nth-child(10) input.Amount").val());
                Quantity += Common.GetFloat($(this).find(":nth-child(5) input.Quantity").val());
                discount += Common.GetFloat($(this).find(":nth-child(12) input.DiscountAmount").val());
                gstamount += Common.GetFloat($(this).find("input.GSTAmount").val());
                netamount += Common.GetFloat($(this).find("input.NetAmount").val());
            });
            Quantity = Quantity.toFixed(2);
            Price = Price.toFixed(2);
            $("#item-container tfoot tr").find(":nth-child(2) input.MainUnitQuantity").val(MainUnitQuantity.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(3) input.Quantity").val(Quantity);
            $("#item-container tfoot tr").find(":nth-child(4) input.Amount").val(Price);
            $("#item-container tfoot tr").find(":nth-child(5) input.DiscountAmount").val(discount.toFixed(2));
            $("#item-container tfoot tr").find("input.GSTAmount").val(gstamount.toFixed(2));
            $("#item-container tfoot tr").find("input.NetAmount").val(netamount.toFixed(2));
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
            //$("#NetTotal").val(incAmount + "");
            $("#GSTAmount").val(gstamount.toFixed(2) + "");
            $("#NetTotal").val(netamount.toFixed(2) + "");
            $this.CalculatePreviousBalance();
            //$this.GetNetTotal();
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
        },


        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
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
            //html += "<tr style='font-size:1.2em;'><th colspan='6' class='align-right' style='padding-top:10px;'>Current Balance :</th><th colspan='4' class='align-center' style='padding-top:10px;'>" + res.Data.Balance.format() + "</th></tr>";
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
                        var d = res.Data.Order;
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
                            if (d.CashSale) {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                            }

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.SaleItems;
                                Common.MapItemData(items);
                                //Common.InitDateMask();
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
                        $(tr).find(":nth-child(8) input.DiscountPercent").val(Common.GetFloat(discount));
                        $(tr).find(":nth-child(4) select.Unit").html(options);
                        $(".container-message").hide();
                        //$(tr).find("input.MainUnitQuantity").focus();



                    }

                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                }
            });

        },
        CalculateUnitQuantity: function (tr, isBonus) {

            var $this = this;

            var isBonus = isBonus;
            var qty = 0;
            var unit = "";
            var account = Common.GetByCode($(tr).find("input.Code").val());
            $(tr).find("input.BonusQuantity").val(0);
            if (!isBonus) {
                qty = Common.GetInt($(tr).find("input.MainUnitQuantity").val());
                unit = $(tr).find("select.Unit").val();
            }
            else {
                qty = Common.GetInt($(tr).find("input.BonusMainUnitQuantity").val());
                unit = $(tr).find("select.BonusUnit").val();
            }

            unit = unit.toLowerCase();
            if (typeof account != "undefined" && account != null) {
                var medicine = Common.GetAccountDetailByAccountId(account.Id);
                var unitqty = qty;

                if (unit == "unit") {
                    unitqty = qty;

                }
                else if (unit == "strip") {

                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        unitqty = qty * Common.GetInt(medicine.Quantity);
                    }



                }
                else if (unit == "pack") {

                    if (medicine.IsCotton != null && medicine.IsCotton) {
                        qty = qty * Common.GetInt(medicine.PPCQuantity);
                    }
                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        qty = qty * Common.GetInt(medicine.SPPQuantity);
                        unitqty = qty * Common.GetInt(medicine.Quantity);
                    }
                    unitqty = qty * Common.GetInt(medicine.Quantity);
                }
                else if (unit == "cotton") {

                    if (medicine.IsPack != null && medicine.IsPack) {
                        qty = qty * Common.GetInt(medicine.SPPQuantity);
                    }

                    if (medicine.IsPack != null && medicine.IsPack) {
                        qty = qty * Common.GetInt(medicine.SPPQuantity);
                    }
                    if (medicine.IsStrip != null && medicine.IsStrip) {
                        qty = qty * Common.GetInt(medicine.SPPQuantity);
                        unitqty = qty * Common.GetInt(medicine.Quantity);
                    }
                }

            }
            if (!isBonus)
                $(tr).find("input.Quantity").val(unitqty);
            else
                $(tr).find("input.BonusQuantity").val(unitqty);

            var bonusMainUnitQuantity = $(tr).find("input.BonusQuantity").val();
            var unitqty = $(tr).find("input.Quantity").val();
            var totalQty = Common.GetInt(unitqty) + Common.GetInt(bonusMainUnitQuantity);
            $(tr).find("input.Quantity").val(totalQty);
        },
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};