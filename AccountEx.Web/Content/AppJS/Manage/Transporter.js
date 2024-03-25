
var Transporter = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Transporter";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $("#Code").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $this.GetNextAccountCode();
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
        GetAccountByCode: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
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
                            $("#TempCode").val(res.Data.Code);
                        }
                        else {
                            $this.CustomClear();
                            var msg = "You can create a new account with this code";
                            Common.ShowMessage(true, { message: msg });
                            $this.GetNextAccountCode();
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
            var $this = this;
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $this.GetNextAccountCode();
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

            var $this = this;
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                if (Common.GetInt(record.Id) > 0) {
                    if (record.Code != record.TempCode) {
                        err = "<li>You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ") </li>";
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
                    blockMessage: "Saving transporter ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.GetNextAccountCode();
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
                blockMessage: "Loading transporter ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        $("#TempCode").val(res.Data.Code);
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
                    blockMessage: "Deleting transporter ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.GetNextAccountCode();
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
