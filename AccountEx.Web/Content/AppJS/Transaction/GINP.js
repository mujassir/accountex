
var GINP = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "GINP";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "RequisitionTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "date";
    return {
        init: function () {
            var $this = this;
            $("#Customers").change(function () {
                var type = $(this).val();
                $this.BindRequisitionsDatatableForTypes(type);
            });
            $("#VoucherNumber").keypress(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "date" : "date";
                    $this.LoadVoucher("same");
                }
            });
            $("#RequisitionNo").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#InvoiceNumber").focus();
                    else
                        $this.LoadRequisition("challan");

                }
            });
            $(document).on("keyup", "input.Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).parent().parent();
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.AddItem();
                            }
                            else {
                                if ($(this).val().trim() != "") {
                                    var err = $(this).val() + " is not valid code.";
                                    Common.ShowError(err);
                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Quantity").val(1);
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.";
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Quantity", function (event) {
                var tr = $(this).parent().parent();
                $this.GetQuantityPriceTotal(tr);
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();
                }

            });
            $(document).on("click", "#RequisitionTable > tbody tr", function () {
                $this.SelectRequisitions(this);
            });
            $this.LoadPageSetting();
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (SetFocus == "date") {
                setTimeout(function () {
                    $("#Date").focus();
                }, 300);

            }
            else if (SetFocus == "voucher") {
                setTimeout(function () {
                    $("#VoucherNumber").focus();
                }, 300);
            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");


            //}
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitNumerics();
        },
        LoadRequisition: function (key) {
            var $this = this;
            var requisitionno = Common.GetInt($("#RequisitionNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.ginp) {
                type = VoucherType.requisition;
            }
            else if (type == VoucherType.services) {
                //type = $("#Transtype").val();
                type = $("#TransactionType").val();
            }
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "JobOrderRequisition/" + requisitionno + "?type=" + type + "&key=" + key + "&voucher=" + requisitionno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $(".portlet .container-message").addClass("hide");
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;
                        if (d == null) {
                            $this.CustomClear();
                            $("#RequisitionNo").val(res.Data.VoucherNumber);

                            Common.ShowError("Requisition no is not valid.");
                        }
                            //else if (d.Status == 4) {
                            //    Common.ShowError("Order has already processed.");
                            //}
                        else {
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#Id").val(0);
                            $("#RequisitionId").val(d.Id);
                            $("#OrderId").val(d.OrderId);
                            $("#item-container tbody").html("");
                            $("#OrderNo").val(d.OrderNo);
                            $("#OBNo").val(d.InvoiceNumber);
                            Common.SetDate("#OrderDate", res.Data.OrderInfo.date);
                            Common.SetDate("#DeliveryDate", res.Data.OrderInfo.deliverydate);
                            Common.SetDate("#RequisitionDate", d.Date);
                            var items = d.RequisitionItems;
                            if (type == VoucherType.services) {
                                $("#TransactionType").val(d.TransactionType);
                            }
                            if (d.Id > 0 && items != null && items.length > 0) {
                                //items = Enumerable.From(items).ForEach("$.Quantity=$.Quantity-$.QtyDelivered").ToArray();
                                items = Enumerable.From(items).Where(function (x) { return x.Quantity - x.QtyDelivered > 0 }).ToArray();
                                Enumerable.From(items).ForEach(function (x) {
                                    console.log(x);
                                    x.Quantity = x.Quantity - x.QtyDelivered

                                });
                                Common.MapItemData(items);
                                $this.GetWholeTotal();
                            }
                        }
                        $this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        CloseItem: function () {
            Common.Clear();
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            $("#form-info-item").addClass("hide");
            $("#masterdetail").removeClass("hide");
            $("#div-table-item").addClass("hide");
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                SetFocus = "date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });


            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (id > 0) {
                var prevvoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevvoucherno != newvoucherno) {
                    err = "You cannot change GINP no.Please save with previous  GINP no (" + prevvoucherno + ")";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                //record["TransactionType"] = VoucherType[$this.GetType()];
                record["DCItems"] = Items;

                if (VoucherType[voucher] == VoucherType.ginp) {
                    record["TransactionType"] = VoucherType[voucher]
                }

                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving delivery challan ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0;
            var Rate = 0;
            Quantity = Common.GetInt($(tr).find(":nth-child(3) input.Quantity").val());
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
            });
            $("#QuantityTotal").val(Quantity);
        },
        CustomClear: function () {
            $("#item-container tbody").html("");
            Common.Clear();
            $("#btndelete,#btnprint").prop("disabled", true);
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            if (d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
                $("#lblRequisitionDate").html((d.RequisitionDate ? moment(d.RequisitionDate).format("dddd, DD-MM-YYYY") : ''));
                $("#lblOrderDate").html((d.OrderDate ? moment(d.OrderDate).format("dddd, DD-MM-YYYY") : ''));
                $("#lblDeliveryDate").html((d.DeliveryDate ? moment(d.DeliveryDate).format("dddd, DD-MM-YYYY") : ''));
                var type = $this.GetType();
                var html = "";
                var items = d.DCItems;
                var index = 1;
                $("#report-saleitem tbody").html("");
                Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
                var nettotal = $("#QuantityTotal").val();
                $("#lblNetTotal").html(nettotal);
            }
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading delivery challan ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#RequisitionNo").prop("disabled", false);
                            $("#SearchIconRequisition").removeClass("hide");
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#RequisitionNo").prop("disabled", true);
                                $("#SearchIconRequisition").addClass("hide");
                                $("#btndelete,#btnprint").prop("disabled", false);
                                if (res.Data.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.Date);
                                    Common.SetDate("#DeliveryDate", res.Data.OrderInfo.DeliveryDate);
                                    Common.SetDate("#RequisitionDate", res.Data.RequisitionDate.Date);
                                }
                                Common.MapItemData(items);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();
                            }
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        $this.AddItem();
                        $this.LoadReportData(res);
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
        },
        Delete: function () {
            var $this = this;
            var type = VoucherType[$this.GetType()];
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var id = Common.GetInt($("#Id").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucher=" + voucherno;
                if (id <= 0) {
                    Common.ShowError("No Voucher found for deletion.");
                    return;
                }

                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting delivery challan ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var tokens = $.parseJSON($("#FormSetting").val());

            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(".container-message").hide();
                    }
                }
            });

        },
        GetRequisitions: function () {
            var $this = this;
            $("#Orders-Container").modal("show");
            $this.BindRequisitionsDatatable();
        },
        BindRequisitionsDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.ginp) {
                $("#servicetype").addClass("hide");
                type = VoucherType.requisition;
            }
            else if (type = VoucherType.services) {
                $("#servicetype").removeClass("hide");
                type = VoucherType.services;
            }
            var url = Setting.APIBaseUrl + "JobOrderRequisition?type=" + type + "&key="+VoucherType[$this.GetType()]
            var options =
                {
                    "bPaginate": false,
                    "bSort": false,

                }
            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        BindRequisitionsDatatableForTypes: function (type) {
            var $this = this;
            LIST_LOADED = false;
            //if (type == VoucherType.siteserviceorder || type == VoucherType.repairingserviceorder) {
            //    $("#OrderTable thead th:nth-child(3)").addClass("hide");
            //}
            //else {
            //    $("#OrderTable thead th:nth-child(3)").removeClass("hide");
            //}
            var url = Setting.APIBaseUrl + "JobOrderRequisition?type=" + type + "&key=" + VoucherType.services
            var options =
                      {
                          "bPaginate": false,
                          "bSort": false,

                      }
            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        SelectRequisitions: function (tr) {
            var $this = this;
            var requisitionno = $(tr).find("input.RequisitionNo").val();
            var transactiontype = $(tr).find("input.TransactionType").val();
            if (requisitionno.trim() != "" && requisitionno != null) {
                $("#RequisitionNo").val(requisitionno);
                $("#TransactionType").val(transactiontype);
                $this.LoadRequisition("challan");
                $('#btnOrderClose').click();
            }
        },
    };
}();
