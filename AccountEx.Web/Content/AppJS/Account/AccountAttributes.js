
var AccountAttributes = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "AccountAttribute";
    var LIST_LOADED = false;
    return {
        init: function () {
           var $this = this;
            $("select").select2();
            $("#Label").keyup(function () {
                $("#Name").val($("#Label").val().replace(/ /g, "").replace(/\./g, "").replace(/\:/g, "").replace(/\,/g, ""));
            });
            $("#AccountTypeId").change(function () {
                $this.Filter();
            });
            //if (Setting.PageLandingView == "DetailView") {
            //    Add();
            //}
            //else {
            $this.ListView();
            //}

        },
        Add: function () {
           var $this = this;
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
           var $this = this;
            // $('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
        },
        ListView: function () { //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?AccountTypeId=" + $("#AccountTypeId").val();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        Filter: function () {
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?AccountTypeId=" + $("#AccountTypeId").val();
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
        },
        Close: function () {
           var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
           var $this = this;
            //$("#saleitem tbody .chooseninner").chosen();
            AllowNumerics();
            //$(".select2").select2();
            $("select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },
        CustomClear: function () {
           var $this = this;
            $(".container-message").hide();
        },

        Save: function () {
           var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue("#form-info");
                record.TypeName = $("#TypeId option:selected").text();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    success: function (res) {
                        if (res.Success) {
                            //DataTable.RefreshDatatable(DATATABLE_ID);
                            //$this.ListView();
                            $this.Filter();
                            $this.Add();
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
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(res.Data, "#form-info");
                        this.DetailView();
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


