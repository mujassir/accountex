
var VehiclePostDatedCheque = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "VehiclePostDatedCheque";
    var PageSetting = new Object();
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $this.LoadPageSetting();
            $this.LoadAgreements();
            $this.LoadBank();
            $this.LoadVoucher();

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
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#ChequeNo").focus();
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
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving PDC...please wait",
                    success: function (res) {
                        if (res.Success) {

                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            $this.GetNextVoucherNumber();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $("#ChequeNo").focus();
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
                blockMessage: "Loading PDC...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        Common.SetDate($("#Date"), j.Date);
                        Common.SetDate($("#ChequeDate"), j.ChequeDate);
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
        LoadAccounts: function () {
            var accounts = AppData.COA;
            var filteraccount = new Array();
            var exids = new Array();
            var exids = new Array();
            var $this = this;
            var headAccuntId = 0;
            filteraccounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var tokens = $.grep(filteraccounts, function (e) { return e.Level == Setting.AccountLevel; });
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

            $("table tr .AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var ac = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof ac != "undefined" && ac != null) {
                        $(tr).find(":nth-child(1) input.AccountId").val(ac.Id);
                        $(tr).find(":nth-child(2) input.AccountName").val(ac.DisplayName);
                        $(tr).find(":nth-child(2) input.AccountName").focus();
                        $this.MapDescription(tr);
                        $this.LoadBL(tr);
                    }
                }
            });



        },
        LoadAgreements: function () {


            var $this = this;
            $("#item-container tbody").html("");
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=loadAgreements",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading agreements....please wait",
                success: function (res) {
                    if (res.Success) {

                        Common.BindSelect(res.Data, "#VehicleSaleId", true)

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

            var $this = this;



        },
        LoadBank: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Banks);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.banks").html(html).select2();


        },
        LoadPageSetting: function () {

            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            this.LoadAccounts();
            $(".voucher-title").html(PageSetting.FormTitle);
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
                    blockMessage: "Deleting colors...please wait",
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
        GetNextVoucherNumber: function () {

            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        LoadVoucher: function (key) {
            key = "nextvouchernumber";
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=nextvouchernumber",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading next voucher No....please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },


    };
}();
