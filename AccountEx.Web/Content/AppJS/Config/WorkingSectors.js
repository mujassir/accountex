

var WorkingSectors = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "WorkingSector";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            this.ListView();
        },
        Add: function () {
            var _this = this;
            Common.Clear();
            _this.CustomClear();
            _this.DetailView();
        },
        DetailView: function () {
            //$('#div-form').removeClass('hide');
            //$('#div-table').addClass('hide');
        },
        ListView: function () {
            debugger
            var _this = this;
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

        //Close: function () {
        //    $('#div-form').addClass('hide');
        //    $('#div-table').removeClass('hide');
        //},
        ReinializePlugin: function () {
            var _this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
            Common.Clear();
        },

        Save: function () {
            var _this = this;
            _this.SaveRecord(function () {
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                LIST_CHANGED = true;
                _this.Add();
                _this.ListView();
                $("#Name").focus();
            });
        },
        SaveClose: function () {
            var _this = this;
            _this.SaveRecord(function () {
                LIST_CHANGED = true;
                _this.ListView();
                $("#Name").focus();
            });
        },
        SaveRecord: function (callback) {
            var _this = this;
            if (Common.Validate($("#div-form"))) {
                var record = Common.SetValue("#div-form");
                //record.Plan = $("#PlanId option:selected").text();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
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
            var _this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(res.Data, "#div-form");
                        _this.DetailView();
                        $("#Name").focus();
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
            var _this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            _this.CustomClear();
                            $("#Name").focus();
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




