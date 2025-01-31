
var Products = function () {
    var max = 0;
    var FORM_TYPE = "";
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "DoctorActivity";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    return {
        init: function (type) {
            FORM_TYPE = type;
            var $this = this;
            $("#Code").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $("#ParentId").change(function () {
                //Common.GetNextAccountCode(API_CONTROLLER);
            });
            $("#Product").change(function () {
                var productId = $("#Product").val();
                var product = PageSetting.Products.find(x => x.Id == productId);
                $("#Code").val(product.Code)
                $("#Tag").val(product.Name)
                console.log()
                console.log()
            });
            $this.LoadPageSetting();
            $this.ListView();
            $this.SetBarCode();
            //Common.GetNextAccountCode(API_CONTROLLER);
        },
        SetBarCode: function () {
            var $this = this;
            if (PageSetting.BarCodeEnabled) {
                $(".barcode").removeClass('hide');
                Common.UpdateRequired("#BarCode", true);
            }
            else {
                $(".barcode").addClass('hide');
                Common.UpdateRequired("#BarCode", false);
            }
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
            //Common.GetNextAccountCode(API_CONTROLLER);
        },
        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
            $("#Name").focus();
        },
        ListView: function () {
            var $this = this;
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + FORM_TYPE;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }

        },
        Cancell: function () {
            var $this = this;
            $this.CustomClear();
            //Common.GetNextAccountCode(API_CONTROLLER);

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
                            //var element = $("#account-type input[value='" + res.Data.ParentId + "']").prop("checked", true);
                            //$.uniform.update(element);
                            //$this.Filter();
                        }
                        else {
                            $this.CustomClear();
                            var msg = "You can create a new account with this code";
                            Common.ShowMessage(true, { message: msg });
                            //Common.GetNextAccountCode(API_CONTROLLER);
                        }


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
                    console.log(record.TempCode)
                    if (record.Code != record.TempCode) {
                        err = "You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ")";
                        Common.ShowError(err);
                        return;
                    }
                }
                record.Date = $("#Date").val();
                record.Id = $("#Id").val();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving product ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            DataTable.RefreshDatatable(DATATABLE_ID);
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
                blockMessage: "Loading product ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        if (j.DOJ) {
                            const date = new Date(j.DOJ);
                            if (!isNaN(date.getTime())) { // Check if the date is valid
                                $("#Date").val(date.toISOString().split('T')[0]); // Format as YYYY-MM-DD
                            }
                        }
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
                    blockMessage: "Deleting product ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            //Common.GetNextAccountCode(API_CONTROLLER);
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
            //var $this = this;
            //$.ajax({
            //    url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=GetNextAccountCode",
            //    type: "GET",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (res) {
            //        if (res.Success) {
            //            //$("#Code").val(res.Data);
            //        }
            //        else {
            //            Common.ShowError(res.Error);
            //        }

            //    },
            //    error: function (e) {
            //    }
            //});
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
    };
}();
