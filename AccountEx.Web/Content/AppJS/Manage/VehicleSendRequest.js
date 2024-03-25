
var VehicleSendRequest = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "VehicleRequest";
    var PageSetting = new Object();
    var LIST_CHANGED = false;
    var LIST_LOADED = false;
    var BRANCHID = 0;
    return {
        init: function () {
            var $this = this;

            $this.CustomClear();
            BRANCHID = $("#BranchId").val();
            $this.VehicleAutoCompleteInit();
            $this.ListView();

            $("#BranchId").change(function () {
                LIST_LOADED = false;
                DataTable.DestroyDatatable(DATATABLE_ID);
                $this.init();
            });
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (LIST_LOADED) {
                DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?branchId=" + BRANCHID,
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }

            $("#Name").focus();
        },
        Close: function () {
            //$("#form-info").addClass("hide");
            //$("#div-table").removeClass("hide");
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
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        OpenTransferModal: function (element, id, vehicle, ownerBranch, recievingBranch, status) {
            $("#Id").val(id);
            $("#Status").val(status);
            $("#lblownerbranch").html(ownerBranch);
            $("#lbltobranch span").html(recievingBranch);
            $("#lblvehicle").html(vehicle);
            $("#inventroy-transfer-Container").modal("show");
        },
        SendTransferRequst: function (id) {
            var $this = this;
            if (Common.Validate($("#inventroy-transfer-Container"))) {
                var record = Common.SetValue("#inventroy-transfer-Container");
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?acceptReject=true",
                    type: "POST",
                    blockUI: true,
                    blockElement: "#inventroy-transfer-Container",
                    blockMessage: "processing ...please wait",
                    data: record,
                    success: function (res) {
                        if (res.Success) {
                            $("#inventroy-transfer-Container").modal("hide");
                            Common.ShowMessage(true, { message: "Record saved successfully." });
                            $this.ListView();
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

        Save: function () {
            debugger;
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Sale...please wait",
                    success: function (res) {
                        if (res.Success) {
                            //$this.ListView();
                            //DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $("#Name").focus();
                            LIST_CHANGED = true;
                            $this.ListView();

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
                blockMessage: "Loading Sale...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));

                        var vehicles = $.parseJSON($("#jsondatavehicle #data1").html());
                        var vehicle = Enumerable.From(vehicles).Where(function (p) { return p.Id == j.VehicleId }).FirstOrDefault();
                        $("#VehicleId").val(vehicle.Id);
                        $("#VehicleName").val(vehicle.BrandName);
                        $("#Description").val(vehicle.DoM);
                        $("#RegNo").val(vehicle.RegNo);
                        $("#ChassisNo").val(vehicle.ChassisNo);
                        $("#CC").val(vehicle.EnginePower);
                        $("#EngineNo").val(vehicle.EngineNo);
                        $("#Model").val(vehicle.Model);
                        $("#Fuel").val(vehicle.Fuel);
                        $("#Colour").val(vehicle.Color);


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
                    blockMessage: "Deleting Sale...please wait",
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
        VehicleAutoCompleteInit: function () {
            var $this = this;
            var vehicles = $.parseJSON($("#jsondatavehicle #data1").html());
            var suggestions = new Array();
            for (var i in vehicles) {
                var vehicle = vehicles[i];
                suggestions.push(
                    {
                        id: vehicle.Id,
                        value: vehicle.BrandName,
                        label: vehicle.BrandName + "-" + vehicle.Manufacturer
                    }
                    );
            }

            $("#VehicleName").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Enumerable.From(vehicles).Where(function (p) { return p.Id == ui.item.id }).FirstOrDefault();
                    $("#VehicleId").val(account.Id);
                    $("#VehicleName").val(account.BrandName);
                    $("#Description").val(account.DoM);
                    $("#RegNo").val(account.RegNo);
                    $("#ChassisNo").val(account.ChassisNo);
                    $("#CC").val(account.EnginePower);
                    $("#EngineNo").val(account.EngineNo);
                    $("#Model").val(account.Model);
                    $("#Fuel").val(account.Fuel);
                    $("#Colour").val(account.Color);
                }
            });
        },

    };
}();
