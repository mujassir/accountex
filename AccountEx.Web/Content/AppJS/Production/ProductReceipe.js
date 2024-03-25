
var ProductReceipe = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "ProductReceipe";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
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

            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    {
                        $("#item-container tbody tr:nth-last-child(1) input.Percentage").focus().select();
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

                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {

                    $(tr).find("input.ItemId").val(account.Id);
                    $(tr).find("input.Name").val(account.Name);

                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.,";
                        Common.ShowError(err);
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
                    $(tr).find("input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Quantity,.Rate,.Amount,.Percentage", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                //&& rate > 0
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).,";
                    Common.ShowError(err);
                }
                //else if (event.which == 13 && rate <= 0) {
                //    var err = "Item " + code + " must have rate greater than zero(0).,";
                //    Common.ShowError(err);
                //}
            });
            $this.LoadPageSetting();
            $this.AutoCompleteInit(true);
            $this.ListView();
            $this.Add();
            $this.AddItem();


        },
        Add: function () {
            this.CustomClear();
        },
        ListView: function () {
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
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
            $this.AutoCompleteInit(false);
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
                DataTable.RefreshDatatable(DATATABLE_ID);
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });

            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            var err = "";

            if (Common.Validate($("#mainform"))) {
                $("#item-container tbody input.Rate").trigger("keyup");
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
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

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["ProductReceipeitems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving...please wait",
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



        Edit: function (id, key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            if (key == undefined || key == null)
                key = "byId"
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?&key=" + key,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var d = res.Data;
                        Common.MapEditData(d, "#form-info");
                        Common.MapItemData(d.ProductReceipeitems);
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                        }, 500);
                        $this.GetWholeTotal();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
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
            var incAmount = Common.GetInt(Price) - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(Common.GetInt(incAmount));
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetFloat($("#GrossTotal").val());
            var discount = Common.GetFloat($("#Discount").val());
            var nettotal = Common.GetInt(Math.ceil(total - discount));

            $("#NetTotal").val(nettotal);
        },
        CustomClear: function () {
            var $this = this;
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            $this.AddItem();
            Common.Clear();
        },



        Delete: function (id) {
            var $this = this;

            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordDeleted });
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
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

        },
        Print: function (allowAging) {
            if (allowAging) {
                $("#tblAgingItems").removeClass("hide");
            }
            else {
                $("#tblAgingItems").addClass("hide");
            }
            window.print();
        },
        AutoCompleteInit: function (bindProduct) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                product.Name = product.AccountCode + "-" + product.DisplayName;
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName,
                        name: product.DisplayName

                    }
                );
            }
            if (bindProduct)
                Common.BindSelect(products, $("#ProductId"), true)
            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $this.GetQuantityPriceTotal(tr);

                    }

                }
            });

        },

    };
}();