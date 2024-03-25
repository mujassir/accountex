
var AccountTypes = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "AccountType";
    var LIST_LOADED = false;
    return {
        init: function () {
           var $this = this;
            if (Setting.PageLandingView === "DetailView") {
                $this.Add();
            }
            else {
                $this.ListView();
            }
            $("#AccountId").select2();
        },

        Add: function () {
           var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
           var $this = this;
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        ListView: function () {
           var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
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
        },

        Save: function () {
           var $this = this;
            var record = Common.SetValue("#form-info");
            record.AccountTitle = $("#AccountId option:selected").text();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.ListView();
                       
                    }
                    else {
                        Common. Common.ShowError(res.Error);
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
                success: function (res) {
                    if (res.Success) {
                        Common.MapEditData(res.Data, "#form-info");
                       $this.DetailView();
                    }
                    else {
                        Common. Common.ShowError(res.Error);
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
                            DataTable.RefreshDatatable(DATATABLE_ID);
                        }
                        else {
                            Common. Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        }

    };
}();

