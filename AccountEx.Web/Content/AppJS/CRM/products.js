
var Product = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMProduct";
    var UPLOAD_FOLDER = "CRMProducts"
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
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
            $(document).on("change", "#UserTypeId", function (event) {
                $this.ManageFormControl($(this).val());
            });
            $(document).on("change", "#CategoryId", function (event) {
                $this.GetDivisionByCategoryId();
            });
            $(document).on("change", "#GroupId", function (event) {
                $this.GetSubGroupByGroupId();
            });
            $(document).on("change", "#DivisionId", function (event) {
                $this.GetGroupByDivisionId();
            });
            Common.BindFileInput("TDS", "TDSFileUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
            Common.BindFileInput("MSDS", "MSDSFileUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
            $this.ListView();

        },

        GetDivisionByCategoryId: function (divisionId, data) {
            var $this = this;
            var categoryId = $("#CategoryId").val();
            var qs = "?key=GetDivisionByCategoryId";
            qs += "&categoryId=" + categoryId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#DivisionId"), true);
                        if (divisionId > 0) {
                            $("#DivisionId").select2("val", divisionId);
                            if (data != undefined && data != null)
                                $this.GetGroupByDivisionId(data.GroupId, data);
                        }
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        GetGroupByDivisionId: function (groupId, data) {
            var $this = this;
            var divisionId = $("#DivisionId").val();
            var qs = "?key=GetGroupByDivisionId";
            qs += "&divisionId=" + divisionId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#GroupId"), true);
                        if (groupId > 0) {
                            $("#GroupId").select2("val", groupId);
                            if (data != undefined && data != null)
                                $this.GetSubGroupByGroupId(data.SubGroupId);
                        }
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        GetSubGroupByGroupId: function (subGroupId) {
            var $this = this;
            var groupId = $("#GroupId").val();
            var qs = "?key=GetSubGroupByGroupId";
            qs += "&groupId=" + groupId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#SubGroupId"), true);
                        if (subGroupId > 0) {
                            $("#SubGroupId").select2("val", subGroupId);
                        }

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
            $("#Hash").attr("data-required", "required");
            $("#Username").prop("disabled", false);
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
        ManageFormControl: function (type) {
            $("div.user-type").addClass("hide");
            if (type == CRMUserType.DivisionalHead) {
                $("#DivisionId").closest("div.user-type").removeClass("hide");
            }
            else if (type == CRMUserType.SalesExecutive) {
                $("#RSMId").closest("div.user-type").removeClass("hide");
            }
            else if (type == CRMUserType.RSM) {
                $("#RegionId").closest("div.user-type").removeClass("hide");
            }

        },

        CustomClear: function () {
            Common.Clear();
        },

        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
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
                        var data = res.Data;
                        Common.MapEditData(data, $("#form-info"));
                        Common.SetCheckValue(data);
                        $this.GetDivisionByCategoryId(data.DivisionId, data);



                        Common.RefreshFileInput("TDS", "TDSFileUrl", res.Data.TDSFileUrl, UPLOAD_FOLDER);
                        Common.RefreshFileInput("MSDS", "MSDSFileUrl", res.Data.MSDSFileUrl, UPLOAD_FOLDER);
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
