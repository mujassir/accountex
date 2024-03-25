
var ProjectReceipts = function () {
    var DATATABLE_ID = "PR_mainTable";
    var API_CONTROLLER = "ProjectReceipt";
    var LIST_LOADED = false;
    return {
        ProjectId: 0,
        init: function () {
           var $this = this;
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            else {
                $this.ListView();
            }
            $("#GrossCost").keyup(function () {
                $this.CalculateTax();
            });
            $("#SalesTax,#Miscellaneous").keyup(function () {
                $this.CalculateNetCost();
            });
            // $this.GetClients();
            $this.GetEmployees();
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
        },
        ListView: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            var $this = this;

            var url = Setting.APIBaseUrl + API_CONTROLLER + "?ProjectId=" + this.ProjectId;
            var options =
               {
                   "bPaginate": false,
                   "bSort": false,

               }
            if (!LIST_LOADED) {
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url, options); 
            } else {
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            }
            $(document).ajaxComplete(function () {
                $this.CalculateBalance();
            });
            //setTimeout(function () {
            //    $this.CalculateBalance();
            //}, 2000);
        },
        CalculateTax: function () {
            var tax = Common.GetFloat($("#GrossCost").val()) * Setting.SalesTax;
            $("#SalesTax").val(tax);
            CalculateNetCost();
        },
        CalculateNetCost: function () {
            var sum = 0;
            $("#GrossCost,#SalesTax,#Miscellaneous").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#NetCost").val(sum);

        },
        CalculateBalance: function () {
            var netTotal = Common.GetFloat($("#GrossCost").val());
            var receipts = 0;
            $("#PR_mainTable tbody tr").each(function () {
                receipts += Common.GetFloat($(this).find("td:eq(4)").text().replace(/,/g,""));
            });
            var balance = netTotal - receipts;
            //$("#Balance").val(balance.toFixed(2));
            $("#lblBalance").html("Balance = " + balance.toFixed(2));
        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
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
        },

        Save: function () {
           var $this = this;
            var record = new Object();
            record.VoucherNumber = Common.GetInt($("#ExpenseVoucherNumber").val());
            record.TransactionType = Common.GetInt($("#ExpenseType").val());
            record.ProjectId = $this.ProjectId;
            if (record.VoucherNumber > 0) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);

                        } else {
                           Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            } else {
                Common.ShowError("Please provide a valid Voucher Number");
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
                blockElement: "#div-form",
                blockMessage: "Loading project receipt ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        for (var property in res.Data) {
                            $("#" + property).val(res.Data[property]);
                        }
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
                    blockElement: "#div-form",
                    blockMessage: "Deleting project receipt ...please wait",
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

        GetClients: function () {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
                    }
                    else {
                         Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetEmployees: function () {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetEmployees",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindMultiSelect(res.Data, "#Employees", true);
                    }
                    else {
                         Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        }
    };
}();
