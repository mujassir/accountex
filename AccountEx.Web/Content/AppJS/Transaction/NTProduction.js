
var NTProduction = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "NTProduction";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            $(".CreatedDate").datepicker();
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
                        $("#DCNo").focus();
                    else
                        $("#AccountCode").focus();
                }
            });
            $("#InvoiceNumber").keyup(function (e) {
                if (e.which == 13) {
                    $("#Date").focus();
                }
                //$this.MapComments();
            });

            $("#InvoiceNumber").keyup(function (e) {
                // $this.MapComments();
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
                                $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
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
                            $("#item-container tbody tr:nth-last-child(1) input.Width").focus().select();
                        }
                    }

                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
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

            $(document).on("keyup", ".Rolls,.Meters", function (event) {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
            });

            $(document).on("keyup", ".Quantity", function (event) {
                var type = "";
                var tr = $(this).closest("tr");
                var rowtype = $(this).closest("table").attr("id");
                if (rowtype == "rawitem")
                    type = "issued";
                else
                    type = "received";
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (qty <= 0 && event.which == 13) {
                    qty = 1;
                    $(tr).find("input.Quantity").val("1");
                }
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem(parent, type);
            });


            $(document).on("keyup", ".Rate", function () {
                var tr = $(this).closest("tr");
                $this.GetAmountTotal(tr);
                var code = $(tr).find("input.Code").val();
                var netamount = Common.GetInt($(tr).find("input.NetAmount").val());
                if (event.which == 13 && netamount > 0)
                    $this.AddItem();
                else if (event.which == 13 && netamount <= 0) {
                    var err = "Item " + code + " must have Value greater than zero(0).,";
                    Common.ShowError(err);
                }
            });
            $(document).on("keyup", ".Quantity", function () {
                var tr = $(this).closest("tr");
                $this.GetAmountTotal(tr);
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
            $this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                $this.GetNextVoucherNumber();
            }

        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        GetAmountTotal: function (tr) {
            var $this = this;
            var code = $(tr).find("input.Code").val();
            var actualweight = Common.GetFloat($(tr).find("input.ActualWeight").val());
            var rate = Common.GetFloat($(tr).find("input.Rate").val());
            var totalmeters = Common.GetFloat($(tr).find("input.TotalMeters").val());
            var ntQty = Common.GetFloat($(tr).find("input.Quantity").val());
            var type = $(tr).find("td:nth-child(11) .Type").val();
            if (type == "Weight") {
                var netamount = actualweight * rate;
                $(tr).find("input.NetAmount,input.Amount").val(netamount);
            }
            if (type == "Meter") {
                var netamount = totalmeters * rate;
                $(tr).find("input.NetAmount,input.Amount").val(netamount);
            }
            if (type == "Quantity") {
                var netamount = ntQty * rate;
                $(tr).find("input.NetAmount,input.Amount").val(netamount);
            }
            var netamount = Common.GetInt($(tr).find("input.NetAmount").val());
            $this.GetQuantityPriceTotal(tr);
            //if (event.which == 13 && netamount > 0)
            //    $this.AddItem();
            //else if (event.which == 13 && netamount <= 0) {
            //    var err = "Item " + code + " must have Value greater than zero(0).,";
            //    Common.ShowError(err);
            //}
        },

        //MapComments: function () {
        //    var $this = this;
        //    var type = $this.GetType();
        //    if (type == "sale") {

        //        var html = "Book No:" + $("#InvoiceNumber").val();
        //    }
        //},
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
        AddItem: function (parent, type) {
            var $this = this;
            if (type == "issued") {
                var ttlkg = $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).val();
                if (typeof ttlkg != "undefined" && ttlkg <= 0) {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                    }, 300);

                    return;
                }
            }
            else {
                var code = $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).val();
                if (typeof code != "undefined" && code.trim() == "") {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                    }, 300);

                    SetFocus = "code";
                    return;
                }
            }
            var html = "";
            if (type == "issued") {
                html = $("#template-raw-item").html()
            }
            else {
                html = $("#template-item").html();
            }
            $("table tbody", parent).append(html);

            if (type == "issued") {
                setTimeout(function () {
                    $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                }, 300);

            }
            else {
                setTimeout(function () {
                    $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                }, 300);
            }

            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");

            $("#Item").focus();
            $this.AutoCompleteInit();
        },
        SetFormControl: function () {
            if ($("#table-dc-detail tbody").children().length > 0) {
                $("#item-container tbody tr td:nth-last-child(1) span.action").remove();
                $("#item-container tbody tr:nth-last-child(1)").remove();
                $("#item-container tbody tr").find("input,select").prop("disabled", true);
            }
            var Id = Common.GetInt($("#Id").val());
            if (Id > 0)
                $(".btn-load,#dc-search").addClass("hide");
            else
                $(".btn-load,#dc-search").removeClass("hide");

        },
        DeleteRow: function (elment) {
            var $this = this;
            var rowtype = $(elment).closest("table").attr("id");
            var parent = $(elment).closest("div[data-save]");
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal(parent, rowtype);
            if (rowtype == "rawitem") {
                if ($("#rawitem tbody").children().length <= 0)
                    $this.AddItem(parent, "issued");
            }
            else {
                ($("#finisheditem tbody").children().length <= 0);
                $this.AddItem(parent, "received");

            }
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
        SaveRecord: function (callback) {
            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = "production";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {

                Items = Common.SaveItemData("#rawitem");
                Items = Items.concat(Common.SaveItemData("#finisheditem"));
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["TransactionType"] = VoucherType[voucher],
                record["WPItems"] = Items;
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
            var parent = $(tr).closest("div[data-save]");
            var rowtype = $(tr).closest("table").attr("id");
            var Rolls = 0.0;
            var Meters = 0.0;
            Rolls = Common.GetFloat($(tr).find("input.Rolls").val());
            Meters = Common.GetFloat($(tr).find("input.Meters").val());
            var totalmeters = Rolls * Meters;
            $(tr).find("input.TotalMeters").val(totalmeters.toFixed(2));
            var width = Common.GetFloat($(tr).find("input.Width").val());
            var weight = Common.GetFloat($(tr).find("input.Weight").val());
            var standardweight = Common.GetFloat((((width * weight) / 39.37) / 1000) * totalmeters);
            $(tr).find("input.StandardWeight ").val(standardweight.toFixed(2));
            $this.GetWholeTotal(parent, rowtype);
        },
        GetWholeTotal: function (parent, rowtype) {
            var $this = this;
            var Quantity = 0;
            var TotalsRolls = 0;
            var TotalMeters = 0;
            var Price = 0;
            if (rowtype == "rawitem") {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetInt($(this).find("input.Quantity").val());
                });
            }
            else {
                $("table tbody tr", parent).each(function () {
                    TotalsRolls += Common.GetInt($(this).find("input.TotalsRolls").val());
                    TotalMeters += Common.GetInt($(this).find("input.TotalMeters").val());
                    Quantity += Common.GetInt($(this).find("input.Quantity").val());
                });
            }
            $("table tfoot tr", parent).find("input.TotalsRolls").val(TotalsRolls);
            $("table tfoot tr", parent).find("input.TotalMeters").val(TotalMeters);
            $("table tfoot tr", parent).find("input.Quantity").val(Quantity);

            if ($(parent).find("#rawitem").length) {
                $("#QuantityTotal").val(Quantity);
            }
            else {
                $("#FinishedQuantityTotal").val(Quantity);
            }
            var finishedQuantityTotal = $("#FinishedQuantityTotal").val();
            $("#TotalPorductionRecpeit").val(finishedQuantityTotal);

            var totalIssueKG = $("#QuantityTotal").val();
            var remaingInProcess = totalIssueKG - finishedQuantityTotal;
            $("#RemaingInProcess").val(remaingInProcess);

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

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },

        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType["production"] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#rawitem tbody").html("");
                        $("#finisheditem tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#DCNo").prop("disabled", "disabled");
                            $("#btndelete,#btnprint").prop("disabled", false);
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.WPItems != null && d.WPItems.length > 0) {
                                var html = "";
                                var items = d.WPItems;
                                var rawitems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.RawMaterial;
                                });
                                Common.MapItemData(rawitems, "#rawitem", "#template-raw-item", false);
                                $("#rawitem tbody").append(html);
                                var finisheditems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.FinishedGoods;
                                });
                                Common.MapItemData(finisheditems, "#finisheditem", "#template-item", false);
                            }
                        }
                        //$this.GetWholeTotal();
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        $this.AddItem($("#rawitem-container"), "issued");
                        $this.AddItem($("#finisheditem-container"), "received");

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucherNo=" + voucherno;
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
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
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
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Weight").val(product.Weight);
                    }

                }
            });

        },

    };
}();