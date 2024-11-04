
var DeliveryChallan = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "DeliveryChallan";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "OrderTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#Date";
    var requiredOrder = false;
    var getOrder = false;
    return {
        init: function () {

            var $this = this;
            jQuery('.dataTable').wrap('<div class="dataTables_scroll" />');
            $this.LoadPageSetting();
            $("#VoucherNumber").keypress(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#Date" : "#Date";
                    $this.LoadVoucher("same");
                }
            });
            $("#OrderNo").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#InvoiceNumber").focus();
                    else
                        $this.LoadOrder("challan");

                }
            });
            $("#AuthLocationName").change(function (e) {
                $this.LoadVoucher("nextvouchernumber");
                $this.ResetWarehouse();

            });

            if (requiredOrder) {
                this.MarkRequiredOrder();
            }

            $(document).on("keyup", "input.Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
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

                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }
            });
            $(document).on("keyup change", "input.Rate", function () {
                var tr = $(this).closest("tr");
                var Quantity = $(tr).find("input.Quantity").val();
                var Rate = $(tr).find("input.Rate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.Amount").val(Amount);
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid party code.";

                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    console.log(account);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);

                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find("input.Article").val(product.ArticleNo);
                        $(tr).find("input.Quantity").val(1);
                        $(tr).find(":nth-child(5) input.Unit").val(product.UnitType);
                        $(tr).find(":nth-child(6) input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice));


                        //$(tr).find("input.ItemId").val(product.Id);
                        //$(tr).find("input.Name").val(product.Name);

                        //$(tr).find("input.Article").val(product.Article);
                        //$(tr).find(":nth-child(5) input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice));
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.";
                            Common.ShowError(err);
                            $(this).focus();
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (qty <= 0) {
                    $(tr).find(":nth-child(4) input.Quantity").val("1");
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    //$(tr).find("input.Quantity").focus();
                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity, .Rate", function (event) {

                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());

                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }

            });

            $("#AccountCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
                    $(".container-message").hide();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.</li>";
                        Common.ShowError(err);
                    }
                }
            });
            $(document).on("click", "#OrderTable > tbody tr", function () {
                $this.SelectOrder(this);
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.SalesmanAutoCompleteInit();
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        New: function () {
            var $this = this;
            focusElement = "#Date";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            $('#AuthLocationName').trigger('change');
            $('#WarehouseName').trigger('change');
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            if (requiredOrder) return
            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            Common.InitNumerics();
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }, 300);
                focusElement = "";
            }
            //}
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
        },
        LoadOrder: function (key) {
            var $this = this;
            getOrder = false;
            var orderno = $("#OrderNo").val();
            if (!orderno) return;
            var locationId = $("#AuthLocationId").val();
            const token = orderno.split("-");
            var voucherNumber = orderno;
            if (token.length > 1) {
                voucherNumber = token.slice(1).join("-");
            }

            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.goodissue)
                type = VoucherType.saleorder;
            else
                type = VoucherType.purchaseorder

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "OrderBooking/" + voucherNumber + "?type=" + type + "&key=" + key + "&voucher=" + orderno + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $(".portlet .container-message").addClass("hide");
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var date = $("#Date").val();
                        var d = res.Data.Order;
                        var defaultLocationId = d?.AuthLocationId || 0;
                        if (!d?.AuthLocationId) {
                            defaultLocationId = $("#AuthLocationName")?.find(":selected")?.data("custom")
                        }
                        $(`#AuthLocationId`).val(defaultLocationId)
                        $this.ResetWarehouse();
                        if (d == null) {
                            if (!getOrder)
                                $this.CustomClear();

                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("Order no is not valid.");
                        }
                        else if (d.Status == 4) {
                            Common.ShowError("Order has already processed.");
                        }
                        else {

                            Common.MapEditData(d, "#form-info");
                            $("#OrderId").val(d.Id);
                            $("#VoucherNumber").val(voucherno);
                            if (d.VoucherCode) {
                                $(`#AuthLocationName`).val(d.VoucherCode)
                                $(`#AuthLocationId`).val(d.AuthLocationId)
                            }
                            if (d?.WareHouseId) {
                                $(`#WarehouseName`).val(d.WareHouseId)
                            }
                            $("#Date").val(date);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#Id").val(0);
                            $("#item-container tbody").html("");
                            //$("#OBNo").val(d.InvoiceNumber);
                            Common.SetDate("#OrderDate", d.Date);
                            //$(".date-picker,.ac-date").each(function () {
                            //    Common.SetDate(this, $(this).val());
                            //});
                            var items = d.OrderItems;
                            if (d.Id > 0 && items != null && items.length > 0) {

                                items = Enumerable.From(items).Where(function (x) { return (x.Quantity - x.QtyDelivered) > 0 }).ToArray();
                                Enumerable.From(items).ForEach(function (x) {
                                    x.Quantity = x.Quantity - x.QtyDelivered;
                                    x.Article = x.ArticleNo;

                                });

                                Common.MapItemData(items);
                                $this.GetWholeTotal();
                            }
                        }
                        //if (res.Data.Next)
                        //    $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        //else
                        //    $(".form-actions .next,.form-actions .last").addClass("disabled");
                        //if (res.Data.Previous)
                        //    $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        //else
                        //    $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        $this.AddItem();
                        $this.MarkRequiredOrder();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

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
        Print: function () {
            window.print();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });


            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                $this.CustomClear();


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
                const voucherCode = $("#AuthLocationName")?.find(":selected")?.data("code") || null;

                let batchNumber = "";
                if (voucherCode) batchNumber += voucherCode
                if (record["VoucherNumber"]) batchNumber += record["VoucherNumber"]
                if (record["Date"]) batchNumber += record["Date"].replace(/\//g, '');

                console.log(batchNumber)

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += $("#AccountCode").val() + " is not valid party code.";
                }
                var warehouse = $("#WarehouseName")?.find(":selected")?.data("custom") || null;
                var authLocationId = $("#AuthLocationName")?.find(":selected")?.data("custom") || null;
                if (!warehouse && PageSetting.IsMultipleLocationEnabled) {
                    err += " Please select warehouse.";
                }
                record["WarehouseId"] = warehouse || 0;
                record["AuthLocationId"] = authLocationId || 0;
                for (var i in Items) {
                    Items[i]["BatchNo"] = batchNumber;
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
                record["TransactionType"] = VoucherType[$this.GetType()];
                record["VoucherCode"] = voucherCode
                if (record["OrderNo"]) {
                    const token = record["OrderNo"].split("-");
                    if (token.length > 1) {
                        record["OrderNo"] = token.splice(1).join("-")
                    }
                }
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
            var Quantity = 0.0;
            var Rate = 0;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetInt($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(5) input.Amount").val(amount);

            $(tr).find("input.Amount").val(amount.toFixed(2));

            //var discount = Common.GetInt($(tr).find(":nth-child(6) input.DiscountPercent").val());
            //var discountAmount = Common.GetInt(amount) * discount / 100;
            //var netAmount = Common.GetInt(amount) - discountAmount;
            //$(tr).find(":nth-child(7) input.DiscountAmount ").val(discountAmount);
            //$(tr).find(":nth-child(8) input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var discount = 0.0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(4) input.Quantity").val());
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
            if (d == null)
                return;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
            $("#lblDescription").html(instruction);
            $(".lblDate").html((d.Date ? moment(d.Date).format("DD-MM-YYYY") : ''));
            $("#lblDeliveryRequired").html((d.DeliveryRequired ? moment(d.DeliveryRequired).format("dddd, DD-MM-YYYY") : ''));
            var type = $this.GetType();
            var html = "";
            var items = d.DCItems;
            var index = 1;

            var totalamount = 0.0;

            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.Article + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                //html += "<td>" + item.Rate.format() + "</td>";
                //html += "<td>" + item.Amount.format() + "</td>";
                //html += "<td>" + item.DiscountAmount.format() + "</td>";
                //html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";

                totalamount += item.Amount;
            }
            $("#lblTotalAmount").html(totalamount.format());
            $("#report-saleitem tbody").append(html);


            // Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            //  var nettotal = $("#QuantityTotal").val();
            // $("#lblNetTotal").html(nettotal);
        },

        LoadReportData2: function (res) {
            var $this = this;
            var d = res.Data.Order;
            var accountdetail = res.Data.AccountDetail;
            var salesmanInfo = res.Data.salesmanInfo;
            var productMappings = res.Data.ProductMappings;

            if (d == null)
                return;

            Common.MapDataWithPrefixF(d, "#div-report2", "lbl", "html");
            var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
            $("#lblDescription").html(instruction);
            //$("#lblDate").html("");
            //$("#lblDate").html($("#Date").val());
            $(".lblDate").html((d.Date ? moment(d.Date).format("DD-MM-YYYY") : ''));
            $("#lblOrderDate").html($("#OrderDate").val());
            $("#lblDeliveryRequired").html((d.DeliveryRequired ? moment(d.DeliveryRequired).format("dddd, DD-MM-YYYY") : ''));
            $("#lblEmail").html(accountdetail.Email);
            $("#lblCustomerAddress").html(accountdetail.Address);
            $("#lblCustomerContactNumber").html(accountdetail.ContactNumber);
            $("#lblSalesmanContact").html(salesmanInfo);

            var type = $this.GetType();
            var html = "";
            var items = d.DCItems;
            var index = 1;

            var totalamount = 0.0;

            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                var prodMap = Enumerable.From(productMappings).Where(function (p) {
                    return p.COAProductId == item.ItemId
                }).FirstOrDefault();
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.Article + "</td>";
                html += "<td>" + (prodMap ? prodMap.ManualCode : item.ItemCode) + " " + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Unit + "</td>";
                html += "</tr>";

                totalamount += item.Amount;
            }
            $("#lblTotalAmount").html(totalamount.format());
            $("#report-saleitem tbody").append(html);

            html = "";
            $("#report-transportInfo tbody").html("");

            html += "<tr>";
            html += "<td>" + (d.MOT ? d.MOT : "") + "</td>";
            html += "<td>" + (d.DriverName ? d.DriverName : "") + "</td>";
            html += "<td>" + (d.DriverContact ? d.DriverContact : "") + "</td>";
            html += "<td>" + (d.VehicleNo ? d.VehicleNo : "") + "</td>";
            html += "</tr>";
            $("#report-transportInfo tbody").append(html);

            // Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            //  var nettotal = $("#QuantityTotal").val();
            // $("#lblNetTotal").html(nettotal);
        },

        //LoadReportData: function (res) {
        //    var $this = this;
        //    var d = res.Data.Order;
        //    if (d != null) {
        //        Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
        //        $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
        //        $("#lblOrderDate").html((d.OrderDate ? moment(d.OrderDate).format("dddd, DD-MM-YYYY") : ''));
        //        var type = $this.GetType();
        //        var html = "";
        //        var items = d.DCItems;
        //        var index = 1;
        //        $("#report-saleitem tbody").html("");
        //        Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
        //    }
        //    var nettotal = $("#QuantityTotal").val();
        //    $("#lblNetTotal").html(nettotal);
        //},


        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var locationId = Common.GetInt($("#AuthLocationId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading delivery challan ...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (!getOrder)
                            $this.CustomClear();
                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        var defaultLocationId = d?.AuthLocationId || 0;
                        if (!d?.AuthLocationId) {
                            defaultLocationId = $("#AuthLocationName")?.find(":selected")?.data("custom")
                        }
                        const orderNo = $("#OrderNo").val();
                        Common.MapEditData(d, "#form-info");
                        if (orderNo)
                            $("#OrderNo").val(orderNo);

                        if (d?.OrderNo && d?.VoucherCode) {
                            $("#OrderNo").val(d.VoucherCode + '-' + d.OrderNo);
                        }

                        $this.MarkRequiredOrder();
                        $(`#AuthLocationId`).val(defaultLocationId)

                        if (d?.WareHouseId) {
                            const warehouseCode = $(`#WarehouseName option[data-custom='${d.WareHouseId }']`)?.attr("data-code")
                            $('#WarehouseId').val(d.WareHouseId)
                            if (warehouseCode) $('#WarehouseName').val(warehouseCode)
                        }
                        if (d == null || !d.AccountId) {
                            if (!getOrder)
                                $this.CustomClear();
                            $("#OrderNo").prop("disabled", false);
                            $("#SearchIconOrder").removeClass("hide");
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            if (getOrder)
                                $this.LoadOrder("challan");
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#OrderNo").prop("disabled", true);
                                if (res.Data.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.Date);
                                    Common.SetDate("#DeliveryDate", res.Data.OrderInfo.DeliveryDate);
                                }
                                $("#SearchIconOrder").addClass("hide");
                                $("#btndelete,#btnprint").prop("disabled", false);
                                //Enumerable.From(items).ForEach(function (x) {
                                //    x.Article = x.ArticleNo

                                //});
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
                        //$this.LoadReportData(res);
                        $this.LoadReportData2(res);

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

            //var d = "type=" + Common.GetQueryStringValue("type").toLowerCase();
            //Common.WrapAjax({
            //    url: "../Transaction/GetNextVoucherNumber",
            //    type: "POST",
            //    data: d,
            //    success: function (res) {
            //        var q = JSON.parse(res);
            //        if (q.Success) {
            //            $("#VoucherNumber,#InvoiceNumber").val(q.Data);
            //            $("#lblVoucherNumber").html(q.Data);
            //        } else {
            //            Common.ShowError(q.Data);
            //        }
            //    },
            //    error: function (e) {
            //    }
            //});
        },
        Delete: function () {
            var $this = this;
            var type = VoucherType[$this.GetType()];

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var id = Common.GetInt($("#Id").val());
                var locationId = Common.GetInt($("#AuthLocationId").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucher=" + voucherno + "&locationId=" + locationId;
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
        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            switch ($this.GetType().toLowerCase()) {
                case "goodissue":
                    id = PageSetting.Customers;
                    break;
                case "goodreceive":
                    id = PageSetting.Suppliers;
                    break;
            }

            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var tokens = Common.GetAllLeafAccounts(exids);
            
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var d = Common.GetByCode(ui.item.value);

                    if (typeof d != "undefined" && d != null) {
                        $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
                        $("#AccountName").val(d.Name);
                        $("#AccountId").val(d.Id);
                        Common.GetPartyAddress(d.Id);
                        $(".container-message").hide();
                    }
                }
            });



        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var tokens = $.parseJSON($("#FormSetting").val());

            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            if (this.GetType() == "goodreceive") {
                requiredOrder = PageSetting?.RequiredPurchaseOrder || false;
            } else if (this.GetType() == "goodissue") {
                requiredOrder = PageSetting?.RequiredSaleOrder || false;
            }
            this.LoadAccounts();

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
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        // $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);

                        //if (typeof account != "undefined" && account != null) {
                        //    var product = Common.GetAccountDetailByAccountId(account.Id);
                        //    $(tr).find(":nth-child(1) input.ItemId").val(product.Id);
                        //    $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(tr).find("input.Article").val(product.ArticleNo);
                        $(tr).find("input.Unit").val(product.UnitType);
                        $(tr).find("input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice));

                        $(".container-message").hide();
                        //}
                    }
                }
            });

        },
        GetOrders: function () {
            var $this = this;
            $("#Orders-Container").modal("show");
            $this.BindOrdersDatatable();
        },
        BindOrdersDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()]
            //if (type == VoucherType.goodissue)
            //    type = VoucherType.saleorder;

            //else
            //    type = VoucherType.purchaseorder
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + type + "&orders=loadorders";
            var options =
              {
                  "bPaginate": false,
                  "bSort": false,
                  //"scrollY": 200,
                  //"sDom": 'r<"H"lf><"datatable-scroll"t><"F"ip>',
                  //"sScrollX": "100%"
              }
            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
            });
        },
        SelectOrder: function (tr) {
            var $this = this;
            var orderno = $(tr).find("input.OrderNo ").val();
            var locationId = Common.GetInt($(tr).find("input.OrderNo ").attr("data-location-id"));
            var locationCode = $(tr).find("input.OrderNo ").attr("data-location-code");
            if (locationId > 0 && !PageSetting.IsMultipleLocationEnabled) {
                Common.ShowError("Multiple location wise voucher not enabled.");
                return
            }
            if (locationId == 0 && PageSetting.IsMultipleLocationEnabled) {
                Common.ShowError("Location is not selected in this voucher");
                return
            }
            if (orderno.trim() != "" && orderno != null) {
                $("#OrderNo").val(orderno);
                if (PageSetting.IsMultipleLocationEnabled) {
                    const existingAutLocId = $("#AuthLocationId").val();
                    if (existingAutLocId != locationId) {
                        getOrder = true
                        $("#AuthLocationId").val(locationId)
                        $('#AuthLocationName').val(locationCode);
                        this.LoadVoucher("nextvouchernumber");
                    } else {
                        $this.LoadOrder("challan");
                    }
                } else {
                    $this.LoadOrder("challan");
                }
                $('#btnOrderClose').click();
            }
        },
        SalesmanAutoCompleteInit: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Salesman;
            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#SalesmanCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var d = Common.GetByCode(ui.item.value);
                    if (typeof d != "undefined" && d != null) {
                        $("#SalesmanName").val(d.Name);
                        $("#SalesmanId").val(d.Id);
                        $(".container-message").hide();
                    }
                }
            });



        },
        MarkRequiredOrder: function () {
            if (!requiredOrder) return
            setTimeout(() => {
                //$("#AuthLocationName").select2("enable", false);
            }, 200)
        },
        ResetWarehouse: function() {
            $(`#WarehouseName`).val(null);
            const locId = $("#AuthLocationId").val();
            document.querySelectorAll('#WarehouseName option').forEach(el => {
                el.style.display = 'none';
                if (el.getAttribute('data-location') == locId) {
                    el.style.display = 'block';
                }
            });
        }


    };
}();

