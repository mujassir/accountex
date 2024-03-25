

var Vehicles = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Vehicle";
    var LIST_LOADED = false;
    var UPLOAD_FOLDER = "VehicleFiles";
    var LIST_CHANGED = false;

    var PageSetting = new Object();
    var DATA = null;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $this.LoadPageSetting();
            $this.LoadClearingCompanies();
            $this.GetNextFileNo();
            $this.LoadVendors(VehicleType.New);
            $("#Brand Name").focus();
            //Common.BindFileInput("TeacherPic", "PictureUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });

            $(document).on("change", "#Type", function () {
                var type = $(this).val();
                if (type != VehicleType.New) {
                    $(".tradein-lp-container").removeClass("hide");
                    Common.UpdateRequired("#VendorId", true);
                    Common.UpdateRequired("#PurchasePrice", true);
                    $this.LoadVendors(type);
                }
                else {


                    $this.LoadVendors(type);
                    Common.UpdateRequired("#VendorId", false);
                    $(".tradein-lp-container").addClass("hide");
                }
                Common.UpdateRequired("#PurchasePrice", false);
            });
            $(document).on("change", "#VendorId", function () {
                var supplierId = $(this).val();
                if ($this.IsForexSupplier(supplierId)) {
                    $(".forex-container").removeClass("hide");
                }
                else {
                    $(".forex-container").addClass("hide");
                }

            });
            $(document).on("keyup", "#ForexPrice,#ExcRate", function () {

                $this.CalculatePurchasePrice();
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
        CalculatePurchasePrice: function () {
            var $this = this;
            var forexPrice = Common.GetFloat($("#ForexPrice").val());
            var excRate = Common.GetFloat($("#ExcRate").val());
            var totalPrice = forexPrice * excRate;
            $("#PurchasePrice").val(totalPrice.toFixed(2))
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
        OpenTransferModal: function (element, fromBranchId, vehicleId, vehcile, branch, type, isAdmin) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?gettransferDetail=true&transferDetail=transferDetail&vehicleId=" + vehicleId + "&fromBranchId=" + fromBranchId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-form",
                blockMessage: "loading transfer detail ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = $("#inventroy-transfer-Container");
                        var data = res.Data;
                        var branches = data.Branches;

                        $("#FromBranchId").val(fromBranchId);
                        $("#VehicleId", scope).val(vehicleId);
                        $("#Type", scope).val(type);
                        $("#lblownerbranch").html(branch);

                        $("#lblvehicle").html(vehcile);

                        if (type == VehicleRequestType.Send) {
                            var allbranches = data.AllBranches;
                            $("#lbltobranch").addClass("hide");
                            allbranches = Enumerable.From(allbranches).Where(function (x) { return x.Id != fromBranchId }).ToArray();
                            Common.BindSelect(allbranches, "#ToBranchId", true);
                            $("#ToBranchId").removeClass("hide");
                        }
                        else {
                            var transferToVehicleDetail = data.TransferToVehicleDetail;
                            //if (transferToVehicleDetail != null) {
                            //    $("#lbltobranch").removeClass("hide");
                            //    $("#ToBranchId").addClass("hide").select2("val", transferToVehicleDetail.Id);
                            //    $("#lbltobranch span").html(transferToVehicleDetail.Name);
                            //}
                            //else {
                            branches = Enumerable.From(branches).Where(function (x) { return x.Id != fromBranchId }).ToArray();
                            Common.BindSelect(branches, "#ToBranchId", true);
                            $("#ToBranchId").removeClass("hide");
                            //}
                        }

                        $("#inventroy-transfer-Container").modal("show");

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        IsForexSupplier: function (supplierId) {

            return Enumerable.From(PageSetting.ForexSuppliers).Any(function (x) { return x == supplierId });
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
                    blockMessage: "processing request ...please wait",
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
            var $this = this;
            Common.Clear();
            $(".form-actions .purchase-return").addClass("hide");
            $("#IsTradeIn").trigger("change");
            $("#Brand Name").focus();
            $this.GetNextFileNo();
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
                record.IsForex = $this.IsForexSupplier(record.VendorId);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=SaveVehicle",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#div-form",
                    blockMessage: "Saving ...please wait",
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
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading vehicle Info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        DATA = data;
                        Common.MapEditData(res.Data, "#div-form");
                        Common.SetDate("#PurchaseDate", data.PurchaseDate);
                        $("#Brand Name").focus();
                        Common.SetCheckValue(data);
                        $("#Type").trigger("change");
                        $("select#VendorId").select2("val", data.VendorId);
                        if (data.BranchId > 0) {
                            $(".form-actions .purchase-return").removeClass("hide");
                        }
                        else {
                            $(".form-actions .purchase-return").addClass("hide");
                        }
                        $("#VendorId").trigger("change");
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
        PurchaseReturnInfo: function (id) {

            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=loadPurchaseReturnInfo&loadInfo=true&id=" + DATA.Id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#model-purchase-return",
                blockMessage: "Loading purchase return Info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        var scope = ("#model-purchase-return");
                        Common.MapDataWithPrefixFClasses(data, scope, "", "html");
                        $("#model-purchase-return").modal("show");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });


        },
        PurchaseReturn: function () {
            var $this = this;
            if (Common.Validate($("#model-purchase-return"))) {
                var record = Common.SetValue("#div-form");
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?purchaseReturn=true&id=" + DATA.Id,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#model-purchase-return",
                    blockMessage: "Returning vehicle ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Vehicle return successfully." });
                            LIST_CHANGED = true;
                            $this.ListView();
                            $this.CustomClear();
                            $("#model-purchase-return").modal("hide");
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
        GetNextFileNo: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=getNextFileNo&loadInfo=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-form",
                blockMessage: "Generating next file no ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#FileNo").val(res.Data);

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
        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        LoadVendors: function (type) {
            var $this = this;
            var accountId = PageSetting.Customers;
            if (type == VehicleType.LocalPurchase || type == VehicleType.New)
                accountId = PageSetting.Suppliers;
            var accounts = Common.GetLeafAccounts(accountId);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select#VendorId").html(html).select2();


        },
        LoadClearingCompanies: function (type) {
            var $this = this;
            var accountId = PageSetting.Suppliers;
            var accounts = Common.GetLeafAccounts(accountId);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select#ClearingCompanyId").html(html).select2();


        },
    }
}();




