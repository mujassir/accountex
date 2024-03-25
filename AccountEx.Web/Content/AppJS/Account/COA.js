
var COA = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "LeafAccount";
    var LIST_LOADED = false;
    var attributeTypes;
    return {
        init: function () {
            var $this = this;

            $("#account-type label:first input[type='radio']:nth-child(1)").prop("checked", true);
            $("#account-type input[name='ac']").change(function () {
                $this.CustomClear();
                $this.Filter();
                $this.GetNextAccountCode();
            });
            $("#ParentId").change(function () {

            });
            $("#AccountCode").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $this.Filter();

        },
        LoadAttributeTypes: function () {
            attributeTypes = JSON.parse($("#txtAttributeTypeJson").val());
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        GetNextAccountCode: function () {
            var accountId = $("#account-type input[name='ac']:checked").val();
            $("#ParentId").val(accountId);
            Common.GetNextAccountCode("Bank");
        },
        Filter: function () {
            $("#div-table").removeClass("hide");
            var url = Setting.APIBaseUrl + "COAInfo";
            var accountId = $("#account-type input[name='ac']:checked").val();
            $("#ParentId").val(accountId);
            Common.GetNextAccountCode("Bank");
            url += "?account=" + accountId;
            var options = {
                "scrollY": 200,
                "bAutoWidth": false,
                "paging": false,
            };
            if (!LIST_LOADED)
                DataTable.BindDatatable(DATATABLE_ID, url, options);
            else
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url, options);
            LIST_LOADED = true;
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            var $this = this;
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
        },
        ListView: function () {
            var $this = this;
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + "COAInfo" + "?&type=" + $("#account-type input[name='ac']:checked").val();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },

        Close: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            var $this = this;
            //$("#saleitem tbody .chooseninner").chosen();
            Common.AllowNumerics();
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
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue("#form-info");
                record["ParentId"] = $("#account-type input[name=ac]:checked").val();
                record["Level"] = Setting.AccountLevel;
                record["DisplayName"] = record.Name;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving account ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.Clear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.GetNextAccountCode();
                        } else {
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
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?&key=edit",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        Common.MapEditData(res.Data, "#form-info");
                        //$("#AccountCode").val(res.Data.Code);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetAccountByCode: function () {
            var $this = this;
            var type = $this.GetType();
            var code = $("#AccountCode").val();
            if (code.trim() == "")
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetAccountByCode&code=" + code,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data != null) {
                            Common.MapEditData(res.Data, "#form-info");
                            var element = $("#account-type input[value='" + res.Data.ParentId + "']").prop("checked", true);
                            $.uniform.update(element);
                            $this.Filter();
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
        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            Common.Clear();
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
        }

    };
}();

