
var MedicineItem = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "MedicineItem";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $("#Code").keyup(function (e) {
                if (e.which == 13)
                    $this.GetAccountByCode();
            });
            $("#tblitem tr td input").keyup(function (e) {
                var tr = $("#tblitem tbody tr:visible").first();


                var mainUnitsalePrice = Common.GetFloat($(tr).find("input#MainUnitSaleRale").val());
                var mainUnitpurchasePrice = Common.GetFloat($(tr).find("input#MainUnitPurchaseRale").val());
                var mainQty = Common.GetFloat($(tr).find("td:nth-child(4) input").val());
                if (mainQty == 0)
                    mainQty = 1;
                var salePrice = mainUnitsalePrice / mainQty;
                $(tr).find("input.SaleRate").val(salePrice.toFixed(4))

                var purchasePrice = mainUnitpurchasePrice / mainQty;
                $(tr).find("input.PurchaseRate").val(purchasePrice.toFixed(4))

                //var salePrice = Common.GetFloat($(tr).find("input.SaleRate").val());
                //var purchasePrice = Common.GetFloat($(tr).find("input.PurchaseRate").val());


                $("#tblitem tbody tr").each(function (e, i) {
                    if ($(this).index() != $(tr).index() && $(this).is(":visible")) {
                        var qty = Common.GetInt($(this).find("input.Quantity").val());
                        if (qty > 0) {
                            salePrice = salePrice / qty;
                            purchasePrice = purchasePrice / qty;
                            $(this).find("input.SaleRate").val(parseFloat(salePrice.toFixed(4)));
                            $(this).find("input.PurchaseRate").val(parseFloat(purchasePrice.toFixed(4)));
                        }
                        else {
                            $(this).find("input.SaleRate").val("");
                            $(this).find("input.PurchaseRate").val("");
                        }
                    }
                });
            });
            $("#tblitem1 tr td input").keyup(function (e) {
                var index = $(this).closest("td").parent().index();
                if (index > 1) return;

                var trstripperpack = $("#row-stripperpack");

                var trunitperstrip = $("#row-unitperstrip");
                var trunitperpack = $("#row-unitperpack");
                var trpackpercarton = $("#row-packpercotton");


                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                Common.GetInt($(trunitperstrip).find("input.Quantity").val());


                var noOfUnits = 0;


                if ($(trunitperstrip).is(":visible"))
                    noOfUnits = Common.GetInt($(trunitperstrip).find("input.Quantity").val());
                else if ($(trunitperpack).is(":visible"))
                    noOfUnits = Common.GetInt($(trunitperpack).find("input.Quantity").val());


                if (noOfUnits > 0) {
                    var salePrice = Common.GetInt($(trstripperpack).find("input.SaleRate").val());
                    var purchasePrice = Common.GetInt($(trstripperpack).find("input.PurchaseRate").val());

                    var unitSalePrice = salePrice / noOfUnits;
                    var unitPurchasePrice = purchasePrice / noOfUnits;

                    $(trunitperstrip).find("input.SaleRate").val(unitSalePrice.toFixed(4));
                    $(trunitperstrip).find("input.PurchaseRate").val(unitPurchasePrice.toFixed(4));
                }
                else {
                    $(trunitperstrip).find("input.SaleRate").val("");
                    $(trunitperstrip).find("input.PurchaseRate").val("");
                }


            });
            $("#package-container :checkbox").change(function () {
                $this.SetPriceTable();
            });
            $("#ParentId").change(function () {
                Common.GetNextAccountCode(API_CONTROLLER);
            });
            $this.CustomClear();
            $this.ListView();
            $this.SetPriceTable();
            Common.GetNextAccountCode(API_CONTROLLER);
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
        },
        SetPriceTable: function () {
            var html = "";
            var mainUnitSelector = "input#MainUnitSaleRale,input#MainUnitPurchaseRale";
            if ($("#IsCotton").is(":checked") && $("#IsPack").is(":checked")) {
                $("#row-packpercotton").removeClass("hide");
                $(mainUnitSelector).addClass("hide");
                $("#row-packpercotton").find(mainUnitSelector).removeClass("hide");
            }
            else {
                $("#row-packpercotton").addClass("hide");
            }
            if ($("#IsPack").is(":checked")) {
                $("#row-stripperpack").removeClass("hide");
                $(mainUnitSelector).addClass("hide");
                $("#row-stripperpack").find(mainUnitSelector).removeClass("hide");
            }
            else {
                $("#row-stripperpack").addClass("hide");
            }
            if ($("#IsStrip").is(":checked")) {
                $("#row-unitperstrip").removeClass("hide");
                $("#row-unitperpack").addClass("hide");
                if ($("#IsCotton").is(":checked")) {
                    $(mainUnitSelector).addClass("hide");
                    $("#row-packpercotton").find(mainUnitSelector).removeClass("hide");
                }
                else if ($("#IsPack").is(":checked")) {
                    $(mainUnitSelector).addClass("hide");
                    $("#row-stripperpack").find(mainUnitSelector).removeClass("hide");
                }
                else {
                    $(mainUnitSelector).addClass("hide");
                    $("#row-unitperstrip").find(mainUnitSelector).removeClass("hide");
                }
            }
            else {
                $("#row-unitperstrip").addClass("hide");
            }
            if (!$("#IsPack").is(":checked") && !$("#IsStrip").is(":checked")) {
                $("#row-unit").removeClass("hide");
                $("#row-stripperpack").addClass("hide");
                $("#row-unitperpack").addClass("hide");
                $(mainUnitSelector).addClass("hide");
                $("#row-unit").find(mainUnitSelector).removeClass("hide");
            }
            else {
                $("#row-unit").addClass("hide");
            }
            if (!$("#IsStrip").is(":checked") && $("#IsPack").is(":checked")) {

                $("#row-unitperpack").removeClass("hide");
                $("#row-stripperpack").addClass("hide");
                $(mainUnitSelector).addClass("hide");
                $("#row-unitperpack").find(mainUnitSelector).removeClass("hide");
            }
            else {
                $("#row-unitperpack").addClass("hide");
            }
        },
        CalculateUnitRate: function () {
            var html = "";
            var salerate = 0;
            var purchaserate = 0;
            var qty = 0;
            if ($("#IsCotton").is(":checked") && $("#IsPack").is(":checked")) {
                var salerate = 0;
                var purchaserate = 0;
            }
            else {
                $("#row-packpercotton").addClass("hide");
            }
            if ($("#IsPack").is(":checked")) {
                $("#row-stripperpack").removeClass("hide");
            }
            else {
                $("#row-stripperpack").addClass("hide");
            }
            if ($("#IsStrip").is(":checked")) {
                $("#row-unitperstrip").removeClass("hide");
                $("#row-unitperpack").addClass("hide");
            }
            else {
                $("#row-unitperstrip").addClass("hide");
            }
            if (!$("#IsPack").is(":checked") && !$("#IsStrip").is(":checked")) {
                $("#row-unit").removeClass("hide");
                $("#row-stripperpack").addClass("hide");
                $("#row-unitperpack").addClass("hide");
            }
            else {
                $("#row-unit").addClass("hide");
            }

            if (!$("#IsStrip").is(":checked") && $("#IsPack").is(":checked")) {

                $("#row-unitperpack").removeClass("hide");
                $("#row-stripperpack").addClass("hide");
            }
            else {
                $("#row-unitperpack").addClass("hide");
            }

        },

        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        Cancel: function () {
            var $this = this;
            $this.CustomClear();
            $this.ListView();
            Common.GetNextAccountCode(API_CONTROLLER);
        },
        GetAccountByCode: function () {
            var $this = this;

            var code = $("#Code").val();
            if (code.trim() == "")
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetAccountDetailByCode&code=" + code + "&formid=" + $("#AccountDetailFormId").val(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data != null) {
                            Common.MapEditData(res.Data, "#form-info");
                            Common.SetCheckValue(res.Data);
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
        GetNextAccountCode: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=GetNextAccountCode",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#Code").val(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
            var $this = this;
            Common.Clear();
            $this.SetPriceTable();
        },

        Save: function () {
            var $this = this;
            var scope = $("#row-unit");
            var record = Common.SetValue($(".form"));
            if (!$("#row-unitperstrip").hasClass("hide")) {
                scope = $("#row-unitperstrip");
            }
            else if (!$("#row-unitperpack").hasClass("hide")) {
                scope = $("#row-unitperpack");
            }

            //record["MainUnitSaleRale"] = $("#MainUnitSaleRale", scope).val();
            //record["MainUnitPurchaseRale"] = $("#MainUnitPurchaseRale", scope).val();
            record["Quantity"] = $("#Quantity", scope).val();
            record["PurchasePrice"] = $("#PurchasePrice", scope).val();
            record["SalePrice"] = $("#SalePrice", scope).val();


            var tr = $("#tblitem tbody tr:visible").first();
            record["MainUnitSaleRale"] = $("#MainUnitSaleRale", $(tr)).val();
            record["MainUnitPurchaseRale"] = $("#MainUnitPurchaseRale", $(tr)).val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving medicine..please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        $this.ListView();
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                        Common.GetNextAccountCode(API_CONTROLLER);
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.SetPriceTable();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
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
                blockMessage: "Loading medicineitem...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        Common.SetCheckValue(res.Data);
                        $this.SetPriceTable();
                        var scope = $("#row-unit");
                        var record = Common.SetValue($(".form"));
                        if (!$("#row-unitperstrip").hasClass("hide")) {
                            scope = $("#row-unitperstrip");
                        }
                        else if (!$("#row-unitperpack").hasClass("hide")) {
                            scope = $("#row-unitperpack");
                        }
                        $("#Quantity", scope).val(res.Data.Quantity);
                        $("#PurchasePrice", scope).val(res.Data.PurchasePrice);
                        $("#SalePrice", scope).val(res.Data.SalePrice);

                        var tr = $("#tblitem tbody tr:visible").first();

                        scope = $(tr);
                        $("#MainUnitSaleRale", scope).val(res.Data.MainUnitSaleRale);
                        $("#MainUnitPurchaseRale", scope).val(res.Data.MainUnitPurchaseRale);
                        $this.DetailView();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
                    blockMessage: "Deleting medicineitem...please wait",
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
        }
    };
}();
