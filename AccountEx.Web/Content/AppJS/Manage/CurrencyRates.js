

var CurrencyRates = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CurrencyRate";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            var $this = this;
            this.ListView();
            $("#Name").focus();
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $this.CustomClear();
            $this.DetailView();
        },
        DetailView: function () {
            //$('#div-form').removeClass('hide');
            //$('#div-table').addClass('hide');
        },
        ListView: function () {

            var $this = this;
            //$('#div-form').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },

        Close: function () {
            $('#div-form').addClass('hide');
            $('#div-table').removeClass('hide');
        },
        ReinializePlugin: function () {
            var $this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
            Common.Clear();
            $("#Name").focus();
        },

        Save: function () {

            var $this = this;
            $this.SaveRecord(function () {
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                LIST_CHANGED = true;
                $this.ListView();
                $this.CustomClear();
            });
        },
        SaveClose: function () {
            var $this = this;
            this.SaveRecord(function () {
                LIST_CHANGED = true;
                $this.ListView();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            if (Common.Validate($("#div-form"))) {
                var record = Common.SetValue("#div-form");

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#div-form",
                    blockMessage: "Saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
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
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        Common.MapEditData(res.Data, "#div-form");
                        Common.SetDate("#FromDate", data.FromDate);
                        Common.SetDate("#ToDate", data.ToDate);
                        $this.DetailView();
                        $("#Name").focus();
                        Common.SetCheckValue(data);
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
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
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
    }
}();




