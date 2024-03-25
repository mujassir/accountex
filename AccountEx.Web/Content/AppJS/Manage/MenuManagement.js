
var MenuManagement = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "MenuManagement";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
        },
        DetailView: function () {
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },

        ListView: function () {
            var $this = this;
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
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
                var record = Common.SetValue($(".form"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: ".form",
                    blockMessage: "Saving menu...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            
                            $this.CustomClear();
                            //window.location = "menumanagement";
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
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
                blockMessage: "Loading menu...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker,.ac-date").each(function () {

                            Common.SetDate(this, res.Data[$(this).attr("Id")]);
                        });
                        Common.SetCheckValue(j);
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
                    blockMessage: "Deleting menu...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            window.location = "menumanagement";
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
    };
}();
