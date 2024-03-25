
var Promotions = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Promotion";
    var PageSetting = new Object();
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $this.AddItem();
            $(document).on("change", "#GroupId", function () {
                var groupid = $(this).val();
                $this.GetGroupItems(groupid);
            });
            $(document).on("keyup", ".Code", function (event) {
                //var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).closest("tr");
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                //$(tr).find("input.Promotion").val(1);
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
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Promotion").focus().select();
                        }
                    }

                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = Common.GetByCode($(this).val());
                    console.log(product);
                    var tr = $(this).closest("tr");
                    if (typeof product != "undefined" && product != null) {
                        $(tr).find("input.ItemId").val(product.Id);
                        $(tr).find("input.Name").val(product.Name);
                        //$(tr).find("input.Promotion").val(1);
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
            $(document).on("blur", ".Promotion", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Promotion").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    //$(tr).find(":nth-child(3) input.Promotion").val("1");
                    //$(tr).find("input.Quantity").focus();

                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Promotion", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Promotion").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have Promotion greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Promotion").focus();
                }
            });
            
            $(document).on("keyup", ".PromotionRatePurchase,.PromotionRateSale", function (event) {
                if (event.which == 13)
                {
                    $this.AddItem();
                }

            });

            $this.LoadPageSetting();
            $this.AutoCompleteInit();
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
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
            setTimeout(function () {
                $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
            }, 300);
            focusElement = "";
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
        },

        GetGroupItems: function (id)
        {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "ItemGroup/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading ItemGroup...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        for(var i in j.ItemGroupItems)
                        {
                            var itemgroupitem = j.ItemGroupItems[i];
                            var item = Common.GetById(itemgroupitem.ItemId);
                            j.ItemGroupItems[i].ItemCode = item.AccountCode;
                            j.ItemGroupItems[i].ItemName = item.Name;
                        }
                        $("#item-container tbody").html("");
                        Common.MapItemData(j.ItemGroupItems);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        },


        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Promotion = 0.0;
            var Rate = 0;
            Promotion = Common.GetFloat($(tr).find(":nth-child(3) input.Promotion").val());
            Rate = Common.GetInt($(tr).find(":nth-child(4) input.Rate").val());
            var amount = Promotion * Rate;
            $(tr).find(":nth-child(5) input.Amount").val(amount);

            var discount = Common.GetInt($(tr).find(":nth-child(6) input.DiscountPercent").val());
            var discountAmount = Common.GetInt(amount) * discount / 100;
            var netAmount = Common.GetInt(amount) - discountAmount;
            $(tr).find(":nth-child(7) input.DiscountAmount ").val(discountAmount);
            $(tr).find(":nth-child(8) input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Promotion = 0;
            var Price = 0;
            var discount = 0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Promotion += Common.GetInt($(this).find(":nth-child(3) input.Promotion").val());
                Price += Common.GetInt($(this).find(":nth-child(5) input.Amount").val());
                discount += Common.GetInt($(this).find(":nth-child(7) input.DiscountAmount").val());
                netamount += Common.GetInt($(this).find(":nth-child(8) input.NetAmount").val());
            });
            $("#item-container tfoot tr").find(":nth-child(2) input.Promotion").val(Promotion);
            $("#item-container tfoot tr").find(":nth-child(3) input.Amount").val(Price);
            $("#item-container tfoot tr").find(":nth-child(4) input.DiscountAmount").val(discount);
            $("#item-container tfoot tr").find(":nth-child(5) input.NetAmount").val(netamount);
            $("#qtytotal1").val(Promotion);
            $("#QuantityTotal").val(Promotion);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var incAmount = Price - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(incAmount + "");

        },
        GetAccountByCode: function () {
            var $this = this;

            var code = $("#Code").val();
            if (code.trim() == "")
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetTransporterDetailByCode&code=" + code,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data != null) {
                            Common.MapEditData(res.Data, "#form-info");
                            //var element = $("#account-type input[value='" + res.Data.ParentId + "']").prop("checked", true);
                            //$.uniform.update(element);
                            //$this.Filter();
                        }
                        else {
                            $this.CustomClear();
                        }


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        ListView: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            var $this = this;
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            else
            {
                DataTable.RefreshDatatable(DATATABLE_ID);
            }
            $("#item-container tbody").html("");
            $this.AddItem();
            $("#Name").focus();
        },
        CalculateTax: function () {
            var tax = Common.GetFloat($("#GrossCost").val()) * Setting.SalesTax;
            $("#SalesTax").val(tax);
            CalculateNetCost();
        },
        CalculateNetCost: function () {
            var sum = 0;
            $("#GrossCost,#SalesTax,#Miscellaneous").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#NetCost").val(sum);

        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            //$("#saleitem tbody .chooseninner").chosen();
            AllowNumerics();
            //$(".select2").select2();
            $("select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },
        CustomClear: function () {
            Common.Clear();
        },

        Save: function () {
            debugger;
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                var Items = new Array();
                if (Common.Validate($("#mainform"))) {

                    Items = Common.SaveItemData();
                    Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                    var err = "";
                    for (var i in Items) {
                        var item = Items[i];
                        if (item.Promotion <= 0) {
                            err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have Promotion greater than zero(0).";
                        }
                        var product = Common.GetByCode(item.ItemCode);
                        if (typeof product == "undefined" || product == null) {
                            err += +item.ItemCode + " is not valid code.";
                        }
                    }
                    if (Items.length <= 0) {
                        err += "Please add atleast one item.";
                    }

                    if (err.trim() != "") {
                        Common.ShowError(err);
                        return;
                    }
                    record["PromotionItems"] = Items;
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER,
                        type: "POST",
                        data: record,
                        blockUI: true,
                        blockElement: "#form-info",
                        blockMessage: "Saving Promotion...please wait",
                        success: function (res) {
                            if (res.Success) {
                                $this.ListView();
                                //DataTable.RefreshDatatable(DATATABLE_ID);
                                $this.CustomClear();
                                Common.ShowMessage(true, { message: Messages.RecordSaved });
                                $("#Name").focus();
                            }
                            else {
                                Common.ShowError(res.Error);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
            }
        },

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading Promotions...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        $("#item-container tbody").html("");
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        Common.SetDate("#FromDate", j.FromDate);
                        Common.SetDate("#ToDate", j.ToDate);
                        var Items=Enumerable.From(j.PromotionItems).Where("$.CustomerId=="+null).ToArray();
                        Common.MapItemData(Items);
                        $this.DetailView();
                        $this.GetWholeTotal();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
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
                    blockElement: "#div-table",
                    blockMessage: "Deleting Promotions...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },

        GetClients: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetEmployees: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetEmployees",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindMultiSelect(res.Data, "#Employees", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadPageSetting: function () {
            //var voucher = this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
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
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(".container-message").hide();
                    }
                }
            });
        }
    }
}();
