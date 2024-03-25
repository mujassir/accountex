

var ClasificationOfAsserts = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Vehicle";
    var LIST_LOADED = false;
    var UPLOAD_FOLDER = "VehicleFiles";
    var LIST_CHANGED = false;
    return {


        init: function () {
            var $this = this;
            this.ListView();
            $("#Brand Name").focus();
            //Common.BindFileInput("TeacherPic", "PictureUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });

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
        OpenTransferModal: function (element, fromBranchId, vehicleId, vehcile, branch, type, toBranchId, isAdmin, toBranchName) {
            $("#FromBranchId").val(fromBranchId);
            $("#VehicleId").val(vehicleId);
            $("#Type").val(type);
            $("#lblownerbranch").html(branch);
            $("#lbltobranch span").html(toBranchName);
            $("#lblvehicle").html(vehcile);
            if (isAdmin) {
                $("#lbltobranch").addClass("hide");
                $("#ToBranchId").removeClass("hide");
            }
            else {
                if (type == VehicleRequestType.Send) {
                    $("#lbltobranch").addClass("hide");
                    $("#ToBranchId").removeClass("hide");
                }
                else {
                    $("#lbltobranch").removeClass("hide");
                    $("#ToBranchId").addClass("hide").select2("val", toBranchId);
                }
            }
            $("#inventroy-transfer-Container").modal("show");
        },
        SendTransferRequst: function (callback) {
            var $this = this;
            if (Common.Validate($("#inventroy-transfer-Container"))) {
                var record = Common.SetValue("#inventroy-transfer-Container");
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?sendRequest=true",
                    type: "POST",
                    blockUI: true,
                    blockElement: "#inventroy-transfer-Container",
                    blockMessage: "sending request ...please wait",
                    data: record,
                    success: function (res) {
                        if (res.Success) {
                            $("#inventroy-transfer-Container").modal("hide");
                            Common.ShowMessage(true, { message: "Request sent successfully." });
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

        ReinializePlugin: function () {
            var $this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
            Common.Clear();
            $("#Brand Name").focus();
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
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=SaveVehicle",
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
                        $("#Brand Name").focus();
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

        DeleteFile: function (element) {
            var $this = this;
            $(element).parent().parent().remove();


        },

        DeleteAllFile: function (element) {
            var $this = this;
            $("tbody.files tr.template-download td:nth-child(4) input:checked").each(function () {
                _this.DeleteFile($(this));
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




