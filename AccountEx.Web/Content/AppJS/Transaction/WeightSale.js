
var WeightSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "Trans";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "booknumber";
    var PriceType = "";
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
            //$(document).bind('keydown', 'Ctrl+a', function () {
            //    console.log('kr');
            //    return false;
            //});


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
                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }
                else {
                    $("#AccountCode").removeAttr("disabled");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                    $("#AccountName").val("");
                    $("#InvoiceNumber").focus();
                }

            });
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
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
                    //if (type == "sale")
                    $("#AccountCode").focus();
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
                        //$("#saleitem tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").removeAttr('disabled');

                        $("#saleitem tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();

                    }

                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {

                    var party = Common.GetByCode($(this).val());

                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.AccountId);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
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
                var tr = $(this).parent().parent();
                if (typeof account != "undefined" && account != null) {
                    var discount = $this.GetDiscountDetail(account.Id);
                    var product = Common.GetAccountDetailByAccountId(account.Id);
                    $(tr).find("input.ItemId").val(product.AccountId);
                    $(tr).find("input.PriceType").val(product.PriceType.toLowerCase());
                    $(tr).find("input.Name").val(account.Name);
                    $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                    $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                    $(".container-message").hide();
                    //if (product.PriceType == "Unit Price") {
                    //    PriceType = "quantity";
                    //}
                    //else {
                    //    PriceType = "weight";
                    //}
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
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();

                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var weight = Common.GetInt($(tr).find("input.Weight").val());

                if (qty <= 0) {
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                }
                if (weight <= 0) {
                    $(tr).find(":nth-child(4) input.Weight").val("1");
                }

                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("blur", ".Weight", function (event) {

                var tr = $(this).parent().parent();

                var weight = Common.GetInt($(tr).find("input.Weight").val());


                if (weight <= 0) {
                    $(tr).find(":nth-child(4) input.Weight").val("1");
                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Weight,.Quantity,.Rate,.Amount,.DiscountPercent,.DiscountAmount", function (event) {

                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var weight = Common.GetInt($(tr).find("input.Weight").val());
                PriceType = $(tr).find("input.PriceType").val().toLowerCase();

                //if (qty <= 0)
                //    $(tr).find("input.Quantity").val("1");
                if (event.which == 13) {
                    $this.AddItem();
                    if (weight <= 0)
                        $(tr).find("input.Weight").val("1");
                }


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
                    $this.GetPreviousBalance(party.AccountId);
                    $(".container-message").hide();
                    var address = party.Address;

                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = "<li>" + $(this).val() + " is not valid party code.</li>";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            $("#Item").change(function () {
                $this.GetProductDetail();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            this.LoadPageSetting();
            $this.GetCustomerProducts();
            $this.LoadAccountDetail();
            $this.GetTransporters();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
                $this.DetailView();
            }
            else {

                if (Setting.PageLandingView == "DetailView") {
                    this.Add();
                } else {
                    this.ListView();
                }

            }
            //setTimeout(function () {
            //    $("#AccountCode").focus();
            //}, 300);

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
            SetFocus = "booknumber";
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

            var customerid = $("#AccountId").val();
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

            var code = $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();


            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }
            var html = "<tr>";
            html += "<td><input type='hidden' class='ItemId' id='ItemId'>";
            html += "<input type='hidden' id='Id' value=''><input class='PriceType' type='hidden' id='PriceType' value=''>";
            html += "<input type='text' class='Code form-control typeahead input-small'></td>";
            html += "<td><input type='text' disabled='disabled' class='Name form-control input-medium'></td>";
            html += "<td class='align-right'><input type='text' class='Quantity form-control input-small num3'></td>";
            html += "<td class='align-right'><input type='text' class='Weight form-control input-small num3'></td>";
            html += "<td class='align-right'><input type='text' class='Rate form-control input-small num3'></td>";
            html += "<td class='align-right'><input type='text' class='Amount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
            html += "<td class='align-right'><input type='text' class='DiscountPercent form-control input-small num3'></td>";
            html += "<td class='align-right'><input type='text' class='DiscountAmount form-control input-small num3' disabled='disabled' readonly='readonly' /></td>";
            html += "<td class='align-right'><input type='text' class='NetAmount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WeightSale.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
            html += "</tr>";
            $("#saleitem tbody").append(html);
            if (SetFocus == "date") {
                setTimeout(function () {
                    $("#Date").focus();
                }, 300);

            }
            else if (SetFocus == "voucher") {
                setTimeout(function () {
                    $("#VoucherNumber").focus();
                }, 300);
            }
            else if (SetFocus == "booknumber") {
                setTimeout(function () {
                    $("#InvoiceNumber").focus();
                }, 300);
            }
            else if (SetFocus == "dcnumber") {
                setTimeout(function () {
                    $("#DCNo").focus();
                }, 300);
            }
            else if (SetFocus == "accode") {
                setTimeout(function () {
                    $("#AccountCode").focus();
                }, 300);
            }

            else {
                setTimeout(function () {
                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");


            //}
            $this.AutoCompleteInit();
            Common.InitNumerics();

        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#saleitem tbody").children().length <= 0)
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
                SetFocus = "date";
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
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "<li>You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ") </li>";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {

                $("#saleitem tbody tr").each(function () {

                    var itemcode = $(this).children(":nth-child(1)").find("input.Code").val();
                    if (typeof itemcode != "undefined" && itemcode.trim() != "") {
                        var itemname = $(this).children(":nth-child(2)").find("input.Name").val();
                        var itemid = Common.GetInt($(this).children(":nth-child(1)").find("input.ItemId").val());
                        var qty = Common.GetInt($(this).children(":nth-child(3)").find("input.Quantity").val());
                        var weight = Common.GetFloat($(this).children(":nth-child(4)").find("input.Weight").val());
                        var rate = Common.GetFloat($(this).children(":nth-child(5)").find("input.Rate").val());
                        var amount = Common.GetFloat($(this).children(":nth-child(6)").find("input.Amount").val());
                        var discountpercent = Common.GetInt($(this).children(":nth-child(7)").find("input.DiscountPercent").val());
                        var discountamount = Common.GetFloat($(this).children(":nth-child(8)").find("input.DiscountAmount").val());
                        var netamount = Common.GetFloat($(this).children(":nth-child(9)").find("input.NetAmount").val());
                        var pricetype = $(this).children(":nth-child(1)").find("input.PriceType").val();
                        Items.push({
                            SaleId: Common.GetInt(record.Id),
                            InvoiceNumber: $("#InvoiceNumber").val(),
                            VoucherNumber: $("#VoucherNumber").val(),
                            Date: Common.ChangeDateFormate($("#Date").val()),
                            Quantity: qty,
                            Rate: rate,
                            Amount: amount,
                            DiscountPercent: discountpercent,
                            DiscountAmount: discountamount,
                            NetAmount: netamount,
                            Id: $(this).children(":nth-child(1)").children("#Id").val(),
                            TransactionType: VoucherType[voucher],
                            EntryType: EntryType.Item,
                            ItemCode: itemcode,
                            ItemName: itemname,
                            ItemId: itemid,
                            Weight: weight,
                            PriceType: pricetype,
                        });
                    }
                });
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += "<li>" + $("#AccountCode").val() + " is not valid party code.</li>";
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0 && item.Weight <= 0) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity/weight greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += "<li>" + item.ItemCode + " is not valid code.</li>";
                    }

                }
                if (Items.length <= 0) {
                    err += "<li>Please add atleast one item.</li>";
                }
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "<li>Transaction total amount should be graeter then zero(0).</li>";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
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
            var Quantity = 0;
            var Weight = 0;
            var Rate = 0;
            PriceType = PriceType.toLowerCase();
            if (PriceType == "unit price") {
                Quantity = Common.GetInt($(tr).find(":nth-child(3) input.Quantity").val());
                Rate = Common.GetInt($(tr).find(":nth-child(5) input.Rate").val());
                var amount = Quantity * Rate;
                $(tr).find(":nth-child(6) input.Amount").val(amount);
                var discount = Common.GetInt($(tr).find(":nth-child(7) input.DiscountPercent").val());
                var discountAmount = Common.GetInt(amount) * discount / 100;
                var netAmount = Common.GetInt(amount) - discountAmount;
                $(tr).find(":nth-child(8) input.DiscountAmount ").val(discountAmount);
                $(tr).find(":nth-child(9) input.NetAmount").val(netAmount);
            }
            else if (PriceType == "weight price") {
                Weight = Common.GetFloat($(tr).find(":nth-child(4) input.Weight").val());
                Rate = Common.GetInt($(tr).find(":nth-child(5) input.Rate").val());
                var amount = Weight * Rate;
                $(tr).find(":nth-child(6) input.Amount").val(amount);
                var discount = Common.GetInt($(tr).find(":nth-child(7) input.DiscountPercent").val());
                var discountAmount = Common.GetInt(amount) * discount / 100;
                var netAmount = Common.GetInt(amount) - discountAmount;
                $(tr).find(":nth-child(8) input.DiscountAmount ").val(discountAmount);
                $(tr).find(":nth-child(9) input.NetAmount").val(netAmount);

            }
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            var netamount = 0;
            var Weight = 0;
            $("#saleitem tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
                Weight += Common.GetFloat($(this).find(":nth-child(4) input.Weight").val());
                Price += Common.GetInt($(this).find(":nth-child(6) input.Amount").val());
                discount += Common.GetInt($(this).find(":nth-child(8) input.DiscountAmount").val());
                netamount += Common.GetInt($(this).find(":nth-child(9) input.NetAmount").val());
            });
            $("#saleitem tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity);
            $("#saleitem tfoot tr").find(":nth-child(3) input.Weight").val(Weight);
            $("#saleitem tfoot tr").find(":nth-child(4) input.Amount").val(Price);
            $("#saleitem tfoot tr").find(":nth-child(5) input.DiscountAmount").val(discount);
            $("#saleitem tfoot tr").find(":nth-child(6) input.NetAmount").val(netamount);
            $("#qtytotal1").val(Quantity);
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var incAmount = Price - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(incAmount + "");
            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            $("#saleitem tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },

        LoadDeliveryChallan: function (key) {

            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan/" + orderno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;


                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else if (d.Status == 5) {
                            Common.ShowError("Challan has already processed.");
                            SetFocus = "dcnumber";
                        }
                        else {
                            Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#qtytotal1").val(d.QuantityTotal);
                            $("#AccountCode").trigger("blur");
                            $("#Id").val(0);
                            $("#saleitem tbody").html("");
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


                                    var product = Common.GetByCode(item.ItemCode);

                                    var discountpercent = 0;
                                    var price = 0;
                                    var discount = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        discountpercent = Common.GetFloat($this.GetDiscountDetail(product.AccountId));
                                        price = Common.GetFloat(product.SalePrice);

                                    }


                                    var amount = qty * price;
                                    var discountAmount = Common.GetInt(amount) * discountpercent / 100;
                                    var netAmount = Common.GetInt(amount) - discountAmount;


                                    html += "<tr>";
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden' id='Id' vvalue='" + item.Id + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Rate form-control input-small' value='" + price + "' ></td>";
                                    html += "<td><input type='text' value='" + amount + "' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td><input type='text' value='" + discountpercent + "' class='DiscountPercent form-control input-small'></td>";
                                    html += "<td><input type='text' value='" + discountAmount + "'  class='DiscountAmount  form-control input-small' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td><input type='text' value='" + netAmount + "' class='NetAmount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WeightSale.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#saleitem tbody").append(html);
                                $this.MapComments();
                                //setTimeout(function () {
                                //    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
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
        GetTransporters: function () {

            var $this = this;
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);
            //Common.WrapAjax({
            //    url: Setting.APIBaseUrl + "Misc/?key=GetTransporters",
            //    type: "GET",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (res) {
            //        if (res.Success) {

            //            $this.TransporterAutoCompleteInit(res.Data);
            //        }

            //    },
            //    error: function (e) {
            //    }
            //});
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
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Weight + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "<td>" + (item.DiscountAmount != null ? item.DiscountAmount.format() : 0) + "</td>";
                html += "<td>" + (item.NetAmount != null ? item.NetAmount.format() : 0) + "</td>";
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
                html += "<td>" + item.DueAmount.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Balance.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Age + "</td>";
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
                blockMessage: "Loading  " + $this.GetType() + "  ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#saleitem tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (!d.CashSale && d.Id > 0) {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.SaleItems;
                                for (var i in items) {
                                    var item = items[i];
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden'  id='Id' value='" + item.Id + "'>";
                                    html += "<input type='hidden' class='PriceType' id='PriceType' value='" + item.PriceType + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' disabled='disabled' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td class='align-right'><input type='text' class='Quantity form-control input-small num3' value='" + item.Quantity + "'></td>";
                                    html += "<td class='align-right'><input type='text' class='Weight form-control input-small num3' value='" + item.Weight + "'></td>";
                                    html += "<td class='align-right'><input type='text' class='Rate form-control input-small num3'value='" + item.Rate + "' ></td>";
                                    html += "<td class='align-right'><input type='text' value='" + item.Amount + "' class='Amount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td class='align-right'><input type='text' value='" + item.DiscountPercent + "' class='DiscountPercent form-control input-small num3'></td>";
                                    html += "<td class='align-right'><input type='text' value='" + item.DiscountAmount + "'  class='DiscountAmount  form-control input-small num3' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td class='align-right'><input type='text' value='" + item.NetAmount + "' class='NetAmount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WeightSale.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#saleitem tbody").append(html);
                                setTimeout(function () {
                                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
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
                    blockMessage: "Deleting  " + $this.GetType() + "  ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            //$this.CustomClear();
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
                            $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("Sale Return From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Purchase From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.Code + "-" + d.Name);
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
                        id: product.Id,
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
                        var discount = Common.GetDiscountDetail(account.Id);
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.PriceType").val(product.PriceType);


                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));

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
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};