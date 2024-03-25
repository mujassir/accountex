
var InvoiceClearings = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "InvoiceClearing";
    var PageSetting = new Object();
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $this.LoadPageSetting();

            $("#Types").change(function () {
                var type = $("#Types").val();
                if (type == VoucherType.sale) {
                    var customers = Common.GetLeafAccounts(PageSetting.Customers);
                    Common.BindSelect(customers, "#AccountId", true);
                }
                else {
                    var suppliers = Common.GetLeafAccounts(PageSetting.Suppliers);
                    Common.BindSelect(suppliers, "#AccountId", true);
                }
                $("#item-container tbody").html("");
            });

            $("#AccountId").change(function () {
                var accountid = Common.GetInt($("#AccountId").val());
                var type = $("#Types").val();
                $this.GetInvoices(accountid, type);
            });

        },

        GetInvoices: function (accountid, type) {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Trans/?accountid=" + accountid + "&type=" + type,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Invoices ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        $("#item-container tbody").html("");
                        var accountid = $("#AccountId").val();
                        for (var items in res.Data) {
                            var item = res.Data[items];
                            //item.InvoiceId = item.Id;
                            item.AccountId = accountid;
                            item.Date = item.Date ? moment(item.Date).format("DD-MM-YYYY") : '';
                            item.TransactionType = Common.GetKeyFromEnum(item.TransactionType, VoucherType);
                        }
                        Common.MapItemData(data);
                        var nettotal = 0.0, totalpaid = 0.0, balance = 0.0;
                        $("#item-container tr").each(function () {
                            nettotal = Common.GetFloat($(this).find(":nth-child(4) input.NetTotal").val())
                            totalpaid = Common.GetFloat($(this).find(":nth-child(5) input.TotalPaid").val())
                            balance = nettotal - totalpaid;
                            $(this).find(":nth-child(6) input.Amount").val(balance)
                        });
                        Common.InitNumerics();
                    }
                },
                error: function (e) {
                }
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
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
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
            $("#item-container tbody").html("");
            Common.Clear();
        },

        Save: function () {
            debugger;
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var date = $("#Date").val();
                var Items = Common.SaveItemData();
                for (var i in Items) {
                    var item = Items[i];
                    //item.InvoiceNo = item.VoucherNumber;
                    item.Date = date;
                }
                var record = new Object();
                record = { '': Items };
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Invoices...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
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

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading Invoices...please wait",
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

        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

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
                    blockMessage: "Deleting Invoices...please wait",
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
