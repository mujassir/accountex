
var Companies = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Company";
    var LIST_LOADED = false;
    var CLIENT_COMPNAIES = false;
    var AUTH_COMPNAIES = false;
    return {
        init: function () {
            var $this = this;
            if (["/company/index", "/company/index/", "/company", "/company/"].includes(location.pathname.toLocaleLowerCase())) {
                this.CLIENT_COMPNAIES = true
            }
            if (["/company/usercompanies/index", "/company/usercompanies", "/company/usercompanies/index/", "/company/usercompanies/"].includes(location.pathname.toLocaleLowerCase())) {
                this.CLIENT_COMPNAIES = true
                this.AUTH_COMPNAIES = true
            }
            $this.ListView();
            $("#AssetType").select2();
            $("#Code").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $("#Abbrivation").keyup(function (e) {
                $this.UpdateAbbrivation();
            });
            $("#btnSave").click(function (e) {

                $this.Save();
            });
            $("#btnAuthCompany").click(function (e) {

                $this.AuthorizeCompany();
            });
            $("#btnCancel").click(function (e) {

                $this.ListView();
            });

        },

        Add: function () {
           var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
           var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + (this.AUTH_COMPNAIES ? "UserCompany" : API_CONTROLLER) ;
                if (this.CLIENT_COMPNAIES) {
                    url += "?cc=true"
                }
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
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
        UpdateAbbrivation: function () {
            const abbr = document.getElementById("Abbrivation").value;
            document.getElementById("UserNameSuffix").innerHTML = "@" + abbr;
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
                const abbr = document.getElementById("Abbrivation").value;
                if (abbr) {
                    record.UserName += "@" + abbr
                }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    timeout: 600000,
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving company ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });

                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                        Common.ShowError(e);
                    }
                });
            }
        },
        AuthorizeCompany: function () {
           var $this = this;
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/UserCompany",
                    type: "POST",
                    timeout: 600000,
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Authorize company ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });

                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                        Common.ShowError(e);
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
                blockMessage: "Loading company ...please wait",
                success: function (res) {
                    if (res.Success) {
                        //for (var property in res.Data) {
                        //    $("#" + property).val(res.Data[property]);
                        //}
                        Common.MapEditData(res.Data, "#form-info");
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
        Move: function (name) {
            var $this = this;
            if (!name || !name.includes('@')) {
                Common.ShowError("Invalid username");
            }
            let firstCharacter = name[0];
            let substringAfterAt = name.substring(name.indexOf('@') + 1);
            const p = `${firstCharacter.toLowerCase()}${substringAfterAt.toLowerCase()}!@#3`;
            localStorage.defaultLogin = name + "###" + p;
            location.href = '/account/LogOff';
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
                    blockMessage: "deleting company ...please wait",
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
        DeleteAuthCompany: function (id) {
            var $this = this;
            debugger
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/UserCompany/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "deleting company ...please wait",
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
