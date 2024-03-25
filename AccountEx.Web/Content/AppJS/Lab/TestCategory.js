var TestCategory = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "TestCategory";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            var _this = this;
            _this.ListView();
            $("#Name").focus();
            $(".test").select2(
                {
                    minimumInputLength: 2,
                });
            var suggestion = new Array();
            for (var i = 1; i < 70000; i++) {
                suggestion.push(i + "_ActionScript");
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                }
            });


            //$("#Name").keyup(function (e) {
            //    if (e.which == 13)
            //        _this.Save();
            //});
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

        Close: function () {
            $('#div-form').addClass('hide');
            $('#div-table').removeClass('hide');
        },
        ReinializePlugin: function () {
            var _this = this;
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

            var _this = this;
            this.SaveRecord(function () {
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                LIST_CHANGED = true;
                _this.Add();
                _this.ListView();
            });
        },
        SaveClose: function () {
            var _this = this;
            this.SaveRecord(function () {
                LIST_CHANGED = true;
                _this.ListView();
            });
        },
        SaveRecord: function (callback) {
            var _this = this;
            if (Common.Validate($("#div-form"))) {
                var record = Common.SetValue("#div-form");

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




