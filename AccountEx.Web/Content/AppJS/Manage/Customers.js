
var Customers = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Customer";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $("#VendorCode").prop("disabled", true);
            $("#Code").keyup(function (e) {
                if (e.which == 13)
                    $this.GetAccountByCode();
            });

            $("input[name='ac-type']").change(function () {
                if ($(this).val() == "child") {
                    $("#VendorCode").prop("disabled", true);
                    $("#VendorCode").val("");
                    $("#ParentVendorCode").prop("disabled", false);

                }
                else {
                    $("#VendorCode").prop("disabled", false);
                    $("#ParentVendorCode").prop("disabled", true);
                    $("#ParentVendorCode").val("");
                    $("#ParentVendorCustomerName").val("");
                    $("#ParentVendorAccountId").val("");
                }

            });

            $("#ParentId").change(function () {
                if ($("#Id").val() == "" || $("#Id").val() == "0") {
                    Common.GetNextAccountCode(API_CONTROLLER);
                }
            });
            $this.ListView();
            Common.GetNextAccountCode(API_CONTROLLER);
            $this.CustomersAutoCompleteInit();
            $this.SetOrderTakers();
            $this.SetTerritoryManagers();
            //$("#Code").blur(function () {
            //    var id = Common.GetInt($("#Id").val());
            //    if (id > 0) {
            //        var prevCode = $("#TempCode").val();
            //        var newcode = $(this).val();
            //        if (prevCode != newcode) {
            //            $this.GetAccountByCode();
            //        }
            //    }
            //});
        },
        SetOrderTakers: function () {
            var ordertakers = Common.GetLeafAccounts($("#OrderTakerHeadId").val());
            var html = "<option></option>";
            for (var i in ordertakers) {
                var ordertaker = ordertakers[i];
                html += "<option value='" + ordertaker.Id + "'>" + ordertaker.Name + "</option>";
            }
            $("#OrderTakerId").html(html);
        },
        SetTerritoryManagers: function () {
            var territorymanagers = Common.GetLeafAccounts($("#TerritoryManagerHeadId").val());
            var html = "<option></option>";
            for (var i in territorymanagers) {
                var territorymanager = territorymanagers[i];
                html += "<option value='" + territorymanager.Id + "'>" + territorymanager.Name + "</option>";
            }
            $("#TerritoryManagerId").html(html);
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },

        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            var $this = this;
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            Common.GetNextAccountCode(API_CONTROLLER);
            $("#Name").focus();
            //$("#VendorCode").prop("disabled", false);
            //$("#ParentVendorCode").prop("disabled", false);
        },
        CalculateTax: function () {
            var tax = Common.GetFloat($("#GrossCost").val()) * Setting.SalesTax;
            $("#SalesTax").val(tax);
            CalculateNetCost();
        },
        GetAccountByCode: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
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
                            var account = Common.GetById(res.Data.AccountId);
                            res.Data["ParentId"] = (account != undefined && account != null) ? account.ParentId : 0;
                            Common.MapEditData(res.Data, "#form-info");
                            $("#TempCode").val(res.Data.Code);
                        }
                        else {
                            $this.CustomClear();
                            var msg = "You can create a new account with this code";
                            Common.ShowMessage(true, { message: msg });
                            Common.GetNextAccountCode(API_CONTROLLER);
                        }


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
            var $this = this;
            var err = "";
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                if (record.VendorCode != "") {
                    record.OwnVendorCode = true;
                }
                if (Common.GetInt(record.Id) > 0) {
                    if (record.Code != record.TempCode) {
                        err = "<li>You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ") </li>";
                        Common.ShowError(err);
                        return;
                    }

                }
                if (record.ParentVendorCode.trim() != "") {
                    var vendorcodecustomers = $.parseJSON($("#VendorCodeCustomers").val());
                    var parentvendorcode = $.grep(vendorcodecustomers, function (e) { return e.VendorCode == record.ParentVendorCode });
                    if (parentvendorcode.length == 0) {
                        err += record.ParentVendorCode + " is not valid code.";
                    }
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving customer...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.UpdateParentId(record.AccountId, record.ParentId);
                            $this.CustomClear();
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            Common.GetNextAccountCode(API_CONTROLLER);
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
                blockMessage: "Loading customer...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        var account = Common.GetById(j.AccountId);
                        j["ParentId"] = (account != undefined && account != null) ? account.ParentId : 0;
                        Common.MapEditData(j, $("#form-info"));
                        if (j.ParentVendorAccountId > 0) {
                            var ParentVendorData = Common.GetById(j.ParentVendorAccountId);
                            $("#ParentVendorCustomerName").val(ParentVendorData.Name);
                            var vendorcodecustomers = $.parseJSON($("#VendorCodeCustomers").val());
                            var parentvendordata = Enumerable.From(vendorcodecustomers).Where("$.AccountId==" + j.ParentVendorAccountId).FirstOrDefault();
                            $("#ParentVendorCode").val(parentvendordata.VendorCode);
                            $("#VendorCode").prop("disabled", true);
                            $("#ParentVendorCode").prop("disabled", false);
                            $.uniform.update($("input:radio[value='child']").prop("checked", true));
                            $.uniform.update($("input:radio[value='parent']").prop("checked", false));
                        }
                        if (j.VendorCode != null) {
                            $.uniform.update($("input:radio[value='child']").prop("checked", false));
                            $.uniform.update($("input:radio[value='parent']").prop("checked", true));
                            $("#VendorCode").prop("disabled", false);
                            $("#ParentVendorCode").prop("disabled", true);
                            $("#ParentVendorCode").val("");
                            $("#ParentVendorCustomerName").val("");
                            $("#ParentVendorAccountId").val("");
                        }
                        else if (j.VendorCode == null && j.ParentVendorAccountId == null) {
                            $.uniform.update($("input:radio[value='child']").prop("checked", true));
                            $.uniform.update($("input:radio[value='parent']").prop("checked", false));
                            $("#VendorCode").prop("disabled", true);
                            $("#ParentVendorCode").prop("disabled", false);
                            $("#ParentVendorCode").val("");
                            $("#ParentVendorCustomerName").val("");
                            $("#ParentVendorAccountId").val("");
                            $("#VendorCode").val("");
                        }
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        $("#TempCode").val(res.Data.Code);
                        $("#ShowBarcodeBox").prop("checked", record.ShowBarcodeBox);
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
                    blockMessage: "Deleting customer...please wait",
                    success: function (res) {

                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.GetNextAccountCode(API_CONTROLLER);
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
        GetNextAccountCode: function () {
            var $this = this;
            $.ajax({
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
        CustomersAutoCompleteInit: function () {
            var $this = this;
            var tokens = $.parseJSON($("#VendorCodeCustomers").val());
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.AccountId,
                        value: token.VendorCode,
                        label: token.VendorCode
                    }
                );
            }

            $("#ParentVendorCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var d = Common.GetById(ui.item.id);
                    if (typeof d != "undefined" && d != null) {
                        $("#ParentVendorCustomerName").val(d.Name);
                        $("#ParentVendorAccountId").val(d.Id);
                        $(".container-message").hide();
                        $("#VendorCode").prop("disabled", true);
                    }
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
