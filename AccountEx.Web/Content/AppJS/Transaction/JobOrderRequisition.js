
var JobOrderRequisition = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "JobOrderRequisition";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "OrderTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "date";
    return {
        init: function () {
            var $this = this;

            $this.LoadData(VoucherType.customerserviceorder);
            $("#Customers").change(function () {
                var type = $(this).val();
                $this.LoadData(type);
            });

            $("#OrderNo").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#Description").focus();
                    else
                        $this.LoadOrder("challan");
                }
            });
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
                    $this.LoadVoucher("same");

                }
            });
            $("#Remarks").keypress(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    setTimeout(function () {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                    }, 500);

                }
            });
            $(document).on("keyup", ".Code", function (event) {
                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        if (type != "sale")
                            $(".btn.btn-primary.green").focus();
                        else
                            $("#BiltyNo").focus();
                    }
                    else {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).closest("tr");
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
                                    $(this).focus();
                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();
                        }
                    }

                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof product != "undefined" && product != null) {
                        $(tr).find("input.ItemId").val(product.Id);
                        $(tr).find("input.Name").val(product.Name);
                        $(tr).find("input.Quantity").val(1);
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.</li>";
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity,.Amount", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $("#qty").keyup(function (e) {

                if (e.keyCode == 13)
                    $("#Rate").focus();
            });
            $(document).on("click", "#OrderTable > tbody tr", function () {
                $this.SelectOrder(this);
            });

            $(document).on("click", "#CustomerOrderTable,#SiteDataTable,#RepairOrderTable > tbody tr", function () {
                $this.SelectOrder(this);
            });

            $this.LoadPageSetting();
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
        },

        LoadData: function (type) {
            var $this = this;
            $("#modal-body > .row .table-container").addClass("hide");
            if (Url.type == "services") {
                //$this.BindOrdersDatatableForTypes(type);

                if (type == VoucherType.siteserviceorder) {
                    $("#modal-body table#SiteDataTable").parents(".table-container").removeClass("hide");
                    $this.BindSiteDatatable(type);
                }
                else if (type == VoucherType.repairingserviceorder) {
                    $("#modal-body table#RepairOrderTable").parents(".table-container").removeClass("hide");
                    $this.BindRapairDatatable(type);
                }
                else if (type == VoucherType.customerserviceorder) {
                    $("#modal-body table#CustomerOrderTable").parents(".table-container").removeClass("hide");
                    $this.BindCustomerDatatable(type);
                }
            }
            else {
                $("#modal-body table#OrderTable").parents(".table-container").removeClass("hide");
                $this.BindOrdersDatatable(type);
            }
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        LoadOrder: function (key) {
            var $this = this;
            var orderno = Common.GetInt($("#OrderNo").val());
            var type = VoucherType[$this.GetType()];
            if (type == VoucherType.requisition) {
                type = VoucherType.saleorder;
            }
            else if (type == VoucherType.services) {
                //type=$("#Transtype").val();
                type = $("#Customers").val();
            }
            else {
                type = VoucherType.purchaseorder;
            }
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "OrderBooking/" + orderno + "?type=" + type + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container-order tbody").html("");
                        $(".portlet .container-message").addClass("hide");
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("Order no is not valid.");
                        }
                        else if (d.Status == 4) {
                            Common.ShowMessage(true, { message: "Order has already processed." });
                            // Common.ShowMessage("Order has already processed.");
                        }
                        else {
                            $("#OrderId").val(d.Id);
                            $("#VoucherNumber").val(voucherno);
                            if (type == VoucherType.services) {
                                $("#TransactionType").val(d.TransactionType);
                            }
                            Common.SetDate("#DeliveryDate", d.DeliveryDate);
                            Common.SetDate("#OrderDate", d.Date);
                            var items = d.OrderItems;
                            if (d.Id > 0 && items != null && items.length > 0) {

                                Common.MapItemData(d.OrderItems, "#item-container-order", "#template-item-order", true);
                            }
                        }

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val() + ", Dc No:" + $("#DCNo").val() + ", Order No:" + $("#OrderNo").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            SetFocus = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
            $("#item-container-order tbody").html("");
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        Add: function () {
            Common.Clear();
            this.DetailView();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

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
            else if (SetFocus == "booknumber") {
                setTimeout(function () {
                    $("#InvoiceNumber").focus();
                }, 300);
            }
            else if (SetFocus == "dcnumber") {
                setTimeout(function () {
                    $("#DCNo").focus();
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
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                SetFocus = "date";
                $this.GetNextVoucherNumber();
                $("#item-container-order tbody").html("");
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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change requisition no.Please save with previous  requisition no (" + prevoucherno + ").,";
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
                        err += "" + item.ItemCode + " is not valid code.";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                //record["TransactionType"] = VoucherType[voucher],
                record["RequisitionItems"] = Items;

                if (VoucherType[voucher] == VoucherType.requisition) {
                    record["TransactionType"] = VoucherType[voucher]
                }

                if (VoucherType[voucher] == VoucherType.services) {
                    record["TransactionType"] = $("#TransactionType").val();
                }

                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
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
            Quantity = Common.GetInt($(tr).find(":nth-child(3) input.Quantity").val());
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
            });
            $("#qtytotal1").val(Quantity);
            $("#TotalQuantity").val(Quantity);
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);
            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            $("#item-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            $("#report-saleitem tbody").html("");
            if (d && d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
                $("#lblOrderDate").html((d.OrderDate ? moment(d.OrderDate).format("dddd, DD-MM-YYYY") : ''));
                $("#lblDeliveryDate").html((d.DeliveryDate ? moment(d.DeliveryDate).format("dddd, DD-MM-YYYY") : ''));
                var type = $this.GetType();
                var html = "";
                var items = d.RequisitionItems;
                var index = 1;

                Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);

            }
            var nettotal = $("#TotalQuantity").val();
            $("#lblNetTotal").html(nettotal);
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $("#OrderNo").prop("disabled", false);
                            $("#SearchIconOrder").removeClass("hide");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {

                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $("#DCNo").prop("disabled", "disabled");

                            //alert($("#Date").val());
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.RequisitionItems != null && d.RequisitionItems.length > 0) {
                                $("#OrderNo").prop("disabled", true);
                                $("#SearchIconOrder").addClass("hide");
                                $("#btndelete,#btnprint").prop("disabled", false);
                                if (res.Data.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.Date);
                                    Common.SetDate("#DeliveryDate", res.Data.OrderInfo.Deliverydate);
                                }
                                Common.MapItemData(d.RequisitionItems);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();
                                //Commented due to service orders
                                //$this.LoadOrder("challan");
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
                    Common.ShowError(e.responseText);
                }
            });
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
        },
        DeleteMultiple: function (id) {
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            RebindData();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
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
                    blockMessage: "Deleting  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                            $("#item-container-order tbody").html("");
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
            var key = Common.GetKeyFromEnum(VoucherType.services, VoucherType);
            var formSetting = $.parseJSON($("#jsondata #data").html());
            if (voucher == key) {
                $(".goodsdetail-container").addClass('hide');
                $(".saleitem-container").removeClass('col-md-6');
                $(".saleitem-container").addClass('col-md-12');
            }
            else {
                $(".goodsdetail-container").removeClass('hide');
                $(".goodsdetail-container").addClass('col-md-6');
                $(".saleitem-container").removeClass('col-md-12');
                $(".saleitem-container").addClass('col-md-6');
            }
            $("#jsondata #data").remove();
            var tokens = formSetting;
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
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find(":nth-child(1) input.ItemId").val(account.AccountId);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(".container-message").hide();
                    }
                }
            });
        },
        GetOrders: function () {
            var $this = this;

            $("#modal-body > .row .table-container").addClass("hide");
            // //$("#Id").val('');
            //$("#SearchScope").val('responsive');
            // $("#PatientTable > tbody").html('');
            //if ($('#PatientSearch-Icon').hasClass("green") == true)
            $("#Orders-Container").modal("show");
            if (Url.type == "services") {
                //32 is enum for customer service order
                $("#Customers").select2("val", 32);
                $("#modal-body table#CustomerOrderTable").parents(".table-container").removeClass("hide");
                var type = VoucherType[$this.GetType()]
                if (type == VoucherType.requisition) {
                    $("#servicetype").addClass("hide");
                    type = VoucherType.saleorder;

                }
                else if (type == VoucherType.services) {
                    $("#servicetype").removeClass("hide");

                    type = VoucherType.services;
                }
                else {
                    type = VoucherType.purchaseorder;
                }

            }
            else {
                $("#modal-body table#OrderTable").parents(".table-container").removeClass("hide");
                $this.BindOrdersDatatable();
            }


        },
        BindOrdersDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.requisition) {
                $("#servicetype").addClass("hide");
                type = VoucherType.saleorder;
            }
            else if (type == VoucherType.services) {
                $("#servicetype").removeClass("hide");
                type = VoucherType.services;
            }
            else {
                type = VoucherType.purchaseorder;
            }
            var url = Setting.APIBaseUrl + "OrderBooking?type=" + type + "&key=" + VoucherType[$this.GetType()]
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
        BindCustomerDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var dataTableId = "CustomerOrderTable";
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.requisition) {
                $("#servicetype").addClass("hide");
                type = VoucherType.saleorder;
            }
            else if (type == VoucherType.services) {
                $("#servicetype").removeClass("hide");
                type = VoucherType.services;
            }
            else {
                type = VoucherType.purchaseorder;
            }
            var url = Setting.APIBaseUrl + "OrderBooking?type=" + VoucherType.customerserviceorder + "&key=" + VoucherType[$this.GetType()]
            var options =
                       {
                           "bPaginate": false,
                           "bSort": false,

                       }
            DataTable.DestroyDatatable(dataTableId);
            DataTable.BindDatatable(dataTableId, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        BindSiteDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var dataTableId = "SiteDataTable";
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.requisition) {
                $("#servicetype").addClass("hide");
                type = VoucherType.saleorder;
            }
            else if (type == VoucherType.services) {
                $("#servicetype").removeClass("hide");
                type = VoucherType.services;
            }
            else {
                type = VoucherType.purchaseorder;
            }
            var url = Setting.APIBaseUrl + "OrderBooking?type=" + VoucherType.siteserviceorder + "&key=" + VoucherType[$this.GetType()]
            var options =
                       {
                           "bPaginate": false,
                           "bSort": false,

                       }
            DataTable.DestroyDatatable(dataTableId);
            DataTable.BindDatatable(dataTableId, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        BindRapairDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var dataTableId = "RepairOrderTable";
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.requisition) {
                $("#servicetype").addClass("hide");
                type = VoucherType.saleorder;
            }
            else if (type == VoucherType.services) {
                $("#servicetype").removeClass("hide");
                type = VoucherType.services;
            }
            else {
                type = VoucherType.purchaseorder;
            }
            var url = Setting.APIBaseUrl + "OrderBooking?type=" + VoucherType.repairingserviceorder + "&key=" + VoucherType[$this.GetType()]
            var options =
                       {
                           "bPaginate": false,
                           "bSort": false,

                       }
            DataTable.DestroyDatatable(dataTableId);
            DataTable.BindDatatable(dataTableId, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        BindOrdersDatatableForTypes: function (type) {
            var $this = this;
            LIST_LOADED = false;

            var url = Setting.APIBaseUrl + "OrderBooking?type=" + type + "&key=" + VoucherType.services
            var options =
                      {
                          "bPaginate": false,
                          "bSort": false,
                          "bDestroy": true,

                      }
            if (type == VoucherType.siteserviceorder) {
                $("#OrderTable thead").find("th").removeClass("hide");
                $("#OrderTable thead").find("th:nth-child(3)").addClass("hide");
            }
            else if (type == VoucherType.repairingserviceorder) {
                $("#OrderTable thead").find("th").removeClass("hide");
                $("#OrderTable thead").find("th:nth-child(3),th:nth-child(2)").addClass("hide");
            }
            else if (type == VoucherType.customerserviceorder) {
                $("#OrderTable thead").find("th").removeClass("hide");
            }

            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            $("#" + ORDER_DATATABLE_ID + " tbody").html("");

            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        SelectOrder: function (tr) {
            var $this = this;
            var orderno = $(tr).find("input.OrderNo ").val();
            var transactiontype = $(tr).find("input.TransactionType ").val();
            if (orderno.trim() != "" && orderno != null) {
                $("#OrderNo").val(orderno);
                $("#TransactionType").val(transactiontype);
                $this.LoadOrder("challan");
                $('#btnOrderClose').click();
            }
        },
    };
}();
