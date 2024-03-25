
var FlourSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "FlourSale";
    var DATATABLE_ID = "mainTable";
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
                    $this.MapComments();
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }
                else {
                    $("#AccountCode").removeAttr("disabled");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                    $("#AccountName").val("");
                    $("#InvoiceNumber").focus();
                }
            });
            $("#Xmill").change(function () {

                if ($(this).is(":checked")) {
                    $("#VehicleCode,#VehicleNo").val("").prop("disabled", true);
                    $("#VehicleId,#TotalFreight").val("0");
                    $("#VehicleCode,#VehicleNo").val("");
                    Common.UpdateRequired($("#VehicleCode"), false);
                }
                else {
                    $("#VehicleCode").val("").prop("disabled", false);
                    Common.UpdateRequired($("#VehicleCode"), true);
                }
                $this.GetFreightTotal();

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
                    //if (type == "sale")
                    $("#AccountCode").focus();
                }

            });
            $("#InvoiceNumber").keyup(function (e) {

                if (e.which == 13) {
                    $("#Date").focus();
                }
                $this.MapComments();
            });
            $("#InvoiceNumber").keydown(function (e) {

                if (e.which == 9) {
                    e.preventDefault();
                }
            });
            $("#VehicleCode").keyup(function (e) {
                if (e.which == 13) {
                    $(".btn.btn-primary.green").focus();
                }
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
                                var partyId = Common.GetInt($("#AccountId").val());
                                var rate = $this.GetSoldRateByPartyId(partyId, account.Id);
                                if (rate <= 0)
                                    rate = voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice;
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(rate));
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
                            $("#item-container tbody tr:nth-last-child(1) input.Quantity").focus().select();
                        }
                    }
                    else {
                        $("#VehicleCode").focus();
                    }
                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var account = Common.GetByCode($(this).val());
                    if (typeof account != "undefined" && account != null) {
                        $this.GetPreviousBalance(account.Id);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                        }, 500);
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid party code.";
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
                        var partyId = Common.GetInt($("#AccountId").val());
                        var rate = $this.GetSoldRateByPartyId(partyId, account.Id);
                        if (rate <= 0)
                            rate = voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice;
                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Weight").val(product.Weight);
                        $(tr).find("input.Freight").val(product.Freight);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(rate));
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
            $(document).on("click", ".action i.fa-trash-o", function () {
                $this.DeleteRow($(this));
            });
            $(document).on("blur", ".Quantity", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (qty <= 0) {
                    $(tr).find("input.Quantity").val("1");
                }
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13) {
                    $(tr).find("input.Rate").focus().select();
                }
                $this.MapComments();
            });
            $(document).on("keyup", ".Quantity", function (event) {
                var tr = $(this).closest("tr");
                if (event.which == 13) {
                    $(tr).find("input.Rate").focus().select();
                }
            });
            $(document).on("keyup", ".Rate", function (event) {
                var tr = $(this).closest("tr");
                if (event.which == 13) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.Comment ").focus().select();
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("blur", ".Rate", function (event) {
                $this.MapComments();
            });
            $(document).on("keyup", ".Comment", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var weight = Common.GetInt($(tr).find("input.Weight").val());

                //if (qty <= 0)
                //    $(tr).find("input.Quantity").val("1");
                if (event.which == 13) {
                    $this.AddItem();
                    if (weight <= 0)
                        $(tr).find("input.Weight").val("1");
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $("#AccountCode").blur(function () {
                var account = Common.GetByCode($(this).val());
                if (typeof account != "undefined" && account != null) {
                    $this.GetPreviousBalance(account.Id);
                    $(".container-message").hide();
                    //var address = party.Address;

                    //if (typeof address != "undefined" && address != "null")
                    //    $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.";
                        Common.ShowError(err);
                    }
                }
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.VehicleAutoCompleteInit();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            }
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            var html = "Book No:" + $("#InvoiceNumber").val();
            $("#item-container tbody tr").each(function () {
                var code = $(this).find("input.Code").val()
                if (code != "undefined" && code.trim() != "") {
                    var qty = $(this).find("input.Quantity").val();
                    var rate = $(this).find("input.Rate").val()
                    html += "," + qty + "/" + rate
                }


            })
            $("#Comments").val(html);

        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#item-container tbody tr:nth-last-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
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
            //}
            //$this.AutoCompleteInit();
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();

            $this.GetWholeTotal();
            $this.MapComments();
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

                if ($("#Xmill").length > 0 && !$("#Xmill").is(":checked")) {
                    var vehcile = Common.GetByCode($("#VehicleCode").val());
                    if (typeof vehcile == "undefined" || vehcile == null) {
                        err += $("#VehicleCode").val() + " is not valid vehicle code.";
                    }
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
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.UpdateSoldRates(Items);
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
            Quantity = Common.GetInt($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(Common.GetInt(amount));
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            $this.GetFreightTotal();
            var Quantity = 0;
            var Price = 0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find("input.Quantity").val());
                Price += Common.GetFloat($(this).find("input.Amount").val());


            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity);
            $("#item-container tfoot tr").find("input.Amount").val(Common.GetInt(Price));

            $("#qtytotal1").val(Quantity);
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            $("#NetTotal").val(Price);
            $this.CalculatePreviousBalance();
        },
        GetFreightTotal: function () {
            var freightTotal = 0
            if ($("#Xmill").is(":checked")) {
                $("#TotalFreight").val(Common.GetInt(freightTotal));
                return;
            }

            $("#item-container tbody tr").each(function () {
                var freight = Common.GetFloat($(this).find("input.Freight").val());
                // var weight = Common.GetFloat($(this).find(":nth-child(1) input.Weight").val());
                var qty = Common.GetInt($(this).find("input.Quantity").val());
                freightTotal += qty * freight


            });
            $("#TotalFreight").val(Common.GetInt(freightTotal));

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
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                //html += "<td>" + item.DiscountAmount.format() + "</td>";
                //html += "<td>" + item.NetAmount.format() + "</td>";
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
        GetSoldRateByPartyId: function (accountId, itemId) {
            var data = Enumerable.From(PageSetting.SoldRates).FirstOrDefault(null, function (p) { return p.AccountId == accountId && p.ItemId == itemId });
            if (data != null)
                return data.Rate
            else
                return 0;
        },
        UpdateSoldRates: function (items) {
            var partyId = Common.GetInt($("#AccountId").val());
            for (var i in items) {
                var item = items[i];
                var data = Enumerable.From(PageSetting.SoldRates).FirstOrDefault(null, function (p) { return p.AccountId == partyId && p.ItemId == item.ItemId });
                if (data != null) {
                    data.Rate = item.Rate;
                }
                else {
                    PageSetting.SoldRates.push(
                        {
                            AccountId: partyId,
                            Rate: item.Rate,
                            ItemId: item.ItemId
                        });
                }
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
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");

                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            Common.SetCheckValue(d);
                            $("#PreVoucherNumber").val(d.VoucherNumber);
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
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.SaleItems;
                                Common.MapItemData(items);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
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
                    blockMessage: "Deleting " + $this.GetType() + " ...please wait",
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
        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var tokens = Common.GetAllLeafAccounts(exids);
            //var tokens = Common.GetLeafAccounts(id);
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
            var accountIds = Enumerable.From(AppData.AccountDetail).Select("$.AccountId").ToArray();
            products = Enumerable.From(products).Where(function (p) { return $.inArray(p.Id, accountIds) !== -1; }).ToArray();
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
                        var partyId = Common.GetInt($("#AccountId").val());
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var rate = $this.GetSoldRateByPartyId(partyId, account.Id);
                        if (rate <= 0)
                            rate = voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice;
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Weight").val(product.Weight);
                        $(tr).find("input.Freight").val(product.Freight);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(rate));
                        $(".container-message").hide();
                        $this.GetFreightTotal();
                        $this.GetQuantityPriceTotal(tr);
                    }
                }
            });

        },
        VehicleAutoCompleteInit: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.VehicleHeadId);
            var suggestion = new Array();
            for (var i in accounts) {
                var account = accounts[i];
                suggestion.push(
                    {
                        id: account.Id,
                        value: account.AccountCode,
                        name: account.DisplayName,
                        label: account.AccountCode + "-" + account.DisplayName

                    }
                );
            }

            $("#VehicleCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    $("#VehicleNo").val(ui.item.name);
                    $("#VehicleId").val(ui.item.id);
                    $(".btn.btn-primary.green").focus();
                }
            });

        },
    };
}();
