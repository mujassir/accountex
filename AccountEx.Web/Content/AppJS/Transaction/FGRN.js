
var FGRN = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "FGRN";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "WPTable";
    var PageSetting = new Object();
    var PageData = new Object();

    return {
        init: function () {
            var $this = this;
            $(".date-picker").on("show", function (e) {
                if (e.date) {
                    $(this).data("stickyDate", e.date);
                }
                else {
                    $(this).data("stickyDate", null);
                }
                $(this).datepicker("hide");
            });
            $(".date-picker").inputmask("d/m/y", {
                "placeholder": "dd/mm/yyyy"
            }); //multi-char placeholder
            $("#VoucherNumber").keypress(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0"
                    $this.LoadVoucher("same");
                }
            });
            $("#WPNo").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#InvoiceNumber").focus();
                    else
                        $this.LoadWorkInProgress("challan");

                }
            });
            $(document).on("click", "#WPTable > tbody tr", function () {
                $this.SelectWorkInProgress(this);
            });
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

            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        LoadWorkInProgress: function (key) {
            var $this = this;
            var wpno = Common.GetInt($("#WPNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.fgrn)
                type = EntryType.FinishedGoods;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "WorkInProgress/" + wpno + "?type=" + type + "&key=" + key + "&voucher=" + wpno,
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
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("WP no is not valid.");
                        }
                        else if (d.Status == 4) {
                            Common.ShowError("Order has already processed.");
                        }
                        else {
                            //Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#Id").val(0);
                            $("#item-container tbody").html("");
                            $("#OrderNo").val(d.OrderNo);
                            $("#OrderId").val(d.OrderId);
                            Common.SetDate("#OrderDate", res.Data.OrderInfo.orderdate);
                            Common.SetDate("#WPDate", d.Date);
                            var items = d.WPItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                var finisheditems = $.grep(d.WPItems, function (e) {
                                    return e.EntryType == EntryType.FinishedGoods;
                                });
                                Common.MapItemData(finisheditems, null, null, true);
                                $this.GetWholeTotal();
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
                var prevDCno = Common.GetInt(record.PreVoucherNumber);
                var newDCno = Common.GetInt(record.VoucherNumber);
                if (prevDCno != newDCno) {
                    err = "You cannot change DC no.Please save with previous  DC no (" + prevDCno + ")";
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
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
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
                record["TransactionType"] = VoucherType[$this.GetType()];
                record["DCItems"] = Items;
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
            Rate = Common.GetInt($(tr).find(":nth-child(4) input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(5) input.Amount").val(amount);

            var discount = Common.GetInt($(tr).find(":nth-child(6) input.DiscountPercent").val());
            var discountAmount = Common.GetInt(amount) * discount / 100;
            var netAmount = Common.GetInt(amount) - discountAmount;
            $(tr).find(":nth-child(7) input.DiscountAmount ").val(discountAmount);
            $(tr).find(":nth-child(8) input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
                Price += Common.GetInt($(this).find(":nth-child(5) input.Amount").val());
                discount += Common.GetInt($(this).find(":nth-child(7) input.DiscountAmount").val());
            });
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrassTotal").val("");
            else
                $("#GrassTotal").val(Price);
            var incAmount = Price - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(incAmount + "");

        },
        CustomClear: function () {
            $("#item-container tbody").html("");
            Common.Clear();
            $("#btndelete,#btnprint").prop("disabled", true);
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            if (d && d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
                $("#lblWPDate").html((d.WPDate ? moment(d.WPDate).format("dddd, DD-MM-YYYY") : ''));
                $("#lblOrderDate").html((d.OrderDate ? moment(d.OrderDate).format("dddd, DD-MM-YYYY") : ''));
                var type = $this.GetType();
                var html = "";
                var items = d.DCItems;
                var index = 1;
                $("#report-saleitem tbody").html("");
                Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            }
            var nettotal = $("#QuantityTotal").val();
            $("#lblNetTotal").html(nettotal);
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
                        var d = res.Data.WP;
                        var order = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#WPNo").prop("disabled", false);
                            $("#SearchIconWP").removeClass("hide");
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#WPNo").prop("disabled", true);
                                if (res.Data.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.Date);
                                }
                                $("#SearchIconWP").addClass("hide");
                                $("#btndelete,#btnprint").prop("disabled", false);
                                Common.MapItemData(items, null, null, true);
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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucherNo=" + voucherno;
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;

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
        Print: function () {
            window.print();
        },
        GetWorkInProgress: function () {
            var $this = this;
            $("#Orders-Container").modal("show");
            $this.BindWPsDatatable();
        },
        BindWPsDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.fgrn)
                type = EntryType.FinishedGoods;

            //else
            //    type = VoucherType.purchaseorder
            var url = Setting.APIBaseUrl + "WorkInProgress?type=" + type
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
        SelectWorkInProgress: function (tr) {
            var $this = this;
            var wpno = $(tr).find("input.VoucherNumber").val();
            if (wpno.trim() != "" && wpno != null) {
                $("#WPNo").val(wpno);
                $this.LoadWorkInProgress("challan");
                $('#btnOrderClose').click();
            }
        },
    };
}();

