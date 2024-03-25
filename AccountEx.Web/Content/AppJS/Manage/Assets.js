
var Assets = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Asset";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;

            $("#AssetType").select2();
            $("#Code").keyup(function (e) {
                if (e.which === 13)
                    $this.GetAccountByCode();
            });
            $("#AssetType").change(function () {

                if ($(this).val() === "Land" || $(this).val() === "Building") {
                    $(".vehicle").addClass("hide");
                    $(".land").removeClass("hide");
                }
                else {
                    $(".vehicle").removeClass("hide");
                    $(".land").addClass("hide");

                }

            });
            $("#ParentId").change(function () {
                Common.GetNextAccountCode(API_CONTROLLER);
            });
            $this.ListView();
            Common.GetNextAccountCode(API_CONTROLLER);
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
            $this.CustomClear();
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            Common.GetNextAccountCode(API_CONTROLLER);
            $("#Name").focus();
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

                            Common.MapEditData(res.Data, "#form-info");
                            $("#TempCode").val(res.Data.Code);
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, res.Data[$(this).attr("Id")]);
                            });
                            $("#AssetType").trigger("change");

                            //var element = $("#account-type input[value='" + res.Data.ParentId + "']").prop("checked", true);
                            //$.uniform.update(element);
                            //$this.Filter();
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
            $("#AssetType").trigger("change");
        },

        Save: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                if (Common.GetInt(record.Id) > 0) {
                    if (record.Code != record.TempCode) {
                        var err = "<li>You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ") </li>";
                        Common.ShowError(err);
                        return;
                    }
                }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving assets ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
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
                blockMessage: "Loading assets...please wait",
                success: function (res) {
                    if (res.Success) {
                        //for (var property in res.Data) {
                        //    $("#" + property).val(res.Data[property]);
                        //}
                        Common.MapEditData(res.Data, "#form-info");
                        $(".date-picker,.ac-date").each(function () {

                            Common.SetDate(this, res.Data[$(this).attr("Id")]);
                        });
                        $("#TempCode").val(res.Data.Code);
                        $("#AssetType").trigger("change");
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
                    blockMessage: "Deleting assets ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
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


        GetClients: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
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
