
var Employees = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var UPLOAD_FOLDER = "Employeepic";
    var API_CONTROLLER = "Employee";
    var PageSetting = new Object();
    var LIST_LOADED = false;

    return {
        init: function () {
            var $this = this;
            $this.ListView();
            //$this.GetClients();
            //$this.GetEmployees();
            $("#DeploymentStatus").select2();
            Common.BindFileInput("EmployeePic", "PictureUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
            $("#Code").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $("#ParentId").change(function () {
                Common.GetNextAccountCode(API_CONTROLLER);
            });
            Common.GetNextAccountCode(API_CONTROLLER);

            $this.LoadPageSetting();
            $this.LoadAccountDetail();

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
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, res.Data[$(this).attr("Id")]);
                            });
                            $("#TempCode").val(res.Data.Code);
                            Common.RefreshFileInput("EmployeePic", "PictureUrl", res.Data.PictureUrl, UPLOAD_FOLDER)
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
        },

        Close: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
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
                var record = Common.SetValue($(".form"));
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
                    blockMessage: "Saving employee...please wait",
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
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?requestkey=edit",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading employee...please wait",
                success: function (res) {
                    if (res.Success) {

                        Common.MapEditData(res.Data.Employee, "#form-info");
                        if (res.Data.Bank !=null)
                        $("#BankName").val(res.Data.Bank.Name);
                        $(".date-picker,.ac-date").each(function () {

                            Common.SetDate(this, res.Data.Employee[$(this).attr("Id")]);
                        });
                        $("#TempCode").val(res.Data.Employee.Code);
                        Common.RefreshFileInput("EmployeePic", "PictureUrl", res.Data.Employee.PictureUrl, UPLOAD_FOLDER)
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
                    blockElement: "#form-info",
                    blockMessage: "Deleting employee...please wait",
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

        LoadPageSetting: function () {
            $this = this;
        
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },

        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.Banks;
        },
        LoadAccounts: function () {
            var $this = this;
            //var id = 0;
            //id = PageSetting.Banks;
           
            var tokens = PageSetting.Banks
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        AccountId: token.AccountId,
                        value: token.Name,
                        label: token.Code + "-" + token.Name
                        //id: token.AccountId,
                        //value: token.AccountCode,
                        //label: token.AccountCode + "-" + token.DisplayName
                    }
                );
            }

            $("#BankName").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                  //  var account = Common.GetByCode(ui.item.value);
                    var account = ui.item;
                    if (typeof account != "undefined" && account != null) {

                        $("#BankName").val(account.value);
                        $("#BankId").val(account.AccountId);  //AccountId
                       // var address = account.Address;
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
