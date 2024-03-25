

var RentOpeningBalance = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "RentOpeningBalance";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            var $this = this;
            $this.LoadData();
            this.ListView();
            $("#Name").focus();
            $(document).on("keyup", "#form-info :input,.adjustment-container :input", function (event) {
                $this.CalculateBalance();
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
        LoadData: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "RentAgreement?key=GetRentAgreementsWithTenants",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        var html = "<option></option>";
                        for (var i in data) {
                            var record = data[i];
                            html += "<option data-id='" + record.Id + "' data-shopid='" + record.ShopId + "' data-tenantid='" + record.TenantAccountId + "' value='" + record.TenantAccountId + "'>" + record.TenantCode + "-" + record.TenantName + " (" + (record.Block != null ? "Block: " + record.Block + " ," : '') + " Shop No: " + record.ShopNo + ")" + "</option>";
                        }
                        $("#TenantAccountId").html(html);
                        $("#TenantAccountId").select2();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },
        CalculateBalance: function () {
            var $this = this;
            var rent = Common.GetInt($("#Rent").val());
            var uc = Common.GetInt($("#UC").val());
            var electricity = Common.GetInt($("#Electricity").val());
            var surcharge = Common.GetInt($("#SurCharge").val());
            var netAmount = rent + uc + electricity + surcharge;
            $("#NetAmount").val(netAmount)
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
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue("#form-info");
                record.RentAgreementId = Common.GetInt(Common.GetInt($("#TenantAccountId option:selected").attr("data-id")));
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
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        Common.MapEditData(res.Data, ".form-horizontal");
                        $this.CalculateBalance();
                        $this.DetailView();
                       
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




