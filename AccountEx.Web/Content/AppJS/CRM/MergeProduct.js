
var MergeProduct = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "MergeProduct";
    var crmCommon;
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            crmCommon = CRMCommon;
            $(document).on("click", "#btn-save", function (event) {
                $this.Save();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            var config = [{ "Element": "#FromProductId" }, { "Element": "#ToProductId" }];
            crmCommon.LoadProducts(config);
            $this.ListView();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            Common.GoToTop();
        },

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            Common.Clear();
        },


        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var fromProductId = $("#FromProductId").val();
                var fromProductName = $("option:selected", $("#FromProductId")).text();
                var toProductId = $("#ToProductId").val();
                var toProductName = $("option:selected", $("#ToProductId")).text();
                if (fromProductId == toProductId)
                {
                    Common.ShowMessage(false, { message: "From & To Product can not be same." });
                    return;
                }
                var message = 'This will merge all records of "' + fromProductName + '" into "' + toProductName + '". Are you sure to continue?';
                if (!confirm(message))
                    return;
                var qs = "?key=MergeProducts";
                qs += "&fromProductId=" + fromProductId;
                qs += "&toProductId=" + toProductId;

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "CRMMisc" + qs,
                    type: "POST",
                   // data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "processing...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: "Product Merge completed successfully." });
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

        Edit: function ($element) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
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
        Delete: function ($element) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "deleting...please wait",
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

    };
}();
