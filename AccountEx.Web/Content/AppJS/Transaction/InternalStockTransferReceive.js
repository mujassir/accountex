
var InternalStockTransferReceive = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "InternalStockTransferReceive";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                }
            });
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    if (type == "sale")

                        $("#DCNo").focus();
                    else
                        $("#AccountCode").focus();
                }
            });
            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
                if (e.which == 13) {
                    $("#Date").focus();
                }
            });
            $("#AuthLocationName").change(function (e) {
                $this.LoadVoucher("nextvouchernumber");
            });
            $("#FromLocationName").change(function (e) {
                $this.ResetFromWarehouse();
            });
            $("#ToLocationName").change(function (e) {
                $this.ResetToWarehouse();
            });
            $(document).on("keyup", ".Code", function (event) {
                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var productStock = $this.GetProductStock(account.Id);
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
                            $("#item-container tbody tr:nth-last-child(1) td input.Quantity").focus().select();
                        }
                    }

                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var account = Common.GetByCode($(this).val());
                    if (typeof account != "undefined" && account != null) {
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

                if (qty <= 0) {
                    //var err = "Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find(":nth-child(4) input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();

                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity, .Rate", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
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
            $this.LoadPageSetting();
            Common.LoadProductStock(PageSetting.ProductHeadId);
            Common.LoadProductStockWarehouseAndLocationWise();
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
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
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        GetProductStock: function (accountId) {
            const products = Common.GetData("ProductStock" + Common.LocalStoragePrefix)
            return products.find(x => x.AccountId === accountId)
        },
        GetProductStockWarehouseAndLocationWise: function (locationId, warehouseId, itemId) {
            const products = Common.GetData("ProductStockWarehouseAndLocationWise" + Common.LocalStoragePrefix)
            return products.find(x => x.LocationId == locationId && x.WarehouseId == warehouseId && x.ItemId === itemId)?.Quantity || 0
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            $('#FromLocationName').trigger('change');
            $('#ToLocationName').trigger('change');
            $('#FromWarehouseName').trigger('change');
            $('#ToWarehouseName').trigger('change');
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#item-container tbody tr:nth-last-child(1) td input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td input.Code").focus().select();
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
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
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
                focusElement = "#Date";
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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ") ";
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
                        err += + item.ItemCode + " is not valid code.";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }

                var fromLocation = $("#FromLocationName")?.find(":selected")?.data("custom") || null;
                if (!fromLocation && PageSetting.isMultipleLocationEnabled) {
                    err += " Please select from location.";
                }
                var fromWarehouse = $("#FromWarehouseName")?.find(":selected")?.data("custom") || null;
                if (!fromWarehouse && PageSetting.isMultipleLocationEnabled) {
                    err += " Please select from warehouse.";
                }
                var toLocation = $("#ToLocationName")?.find(":selected")?.data("custom") || null;
                if (!toLocation && PageSetting.isMultipleLocationEnabled) {
                    err += " Please select to location.";
                }
                var toWarehouse = $("#ToWarehouseName")?.find(":selected")?.data("custom") || null;
                if (!toWarehouse && PageSetting.isMultipleLocationEnabled) {
                    err += " Please select to warehouse.";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["FromLocationId"] = fromLocation || 0;
                record["FromWarehouseId"] = fromWarehouse || 0;
                record["ToLocationId"] = toLocation || 0;
                record["ToWarehouseId"] = toWarehouse || 0;

                record["TransactionType"] = VoucherType[voucher],
                    record["InternalStockTransferItems"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        Common.LoadProductStockWarehouseAndLocationWise();
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
            $(tr).find(":nth-child(7) input.Amount").val(amount);

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
                Quantity += Common.GetInt($(this).find("input.Quantity").val());
            });
            $("#QuantityTotal").val(Quantity);
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
            if (d == null)
                return;
            console.log(d)
            if (d.VoucherCode)
                d.VoucherCode = d.VoucherCode + "-"
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
            $("#lblInstructions").html(instruction);
            $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
            $("#lblDeliveryRequired").html((d.DeliveryRequired ? moment(d.DeliveryRequired).format("dddd, DD-MM-YYYY") : ''));
            var type = $this.GetType();
            var html = "";
            var items = d.InternalStockTransferItems;
            var index = 1;

            var totalamount = 0.0;

            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ArticleNo + "</td>";
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
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var locationId = Common.GetInt($("#AuthLocationId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?&key=" + key + "&voucher=" + voucherno + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    Common.LoadProductStockWarehouseAndLocationWise();
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");

                        if (d?.FromLocationId) {
                            $('#FromLocationId').val(d.FromLocationId)
                            $('#FromLocationName').val(d.FromLocationId)
                            $('#FromLocationName').attr('disabled', 'disabled')

                        }
                        $this.ResetFromWarehouse()
                        if (d?.FromWarehouseId) {
                            $('#FromWarehouseId').val(d.FromWarehouseId)
                            $('#FromWarehouseName').val(d.FromWarehouseId)
                            $('#FromWarehouseName').attr('disabled', 'disabled')
                        }
                        if (d?.ToLocationId) {
                            $('#ToLocationId').val(d.ToLocationId)
                            $('#ToLocationName').val(d.ToLocationId)
                            $('#ToLocationName').attr('disabled', 'disabled')
                        }
                        $this.ResetToWarehouse()
                        if (d?.ToWarehouseId) {
                            $('#ToWarehouseId').val(d.ToWarehouseId)
                            $('#ToWarehouseName').val(d.ToWarehouseId)
                            $('#ToWarehouseName').attr('disabled', 'disabled')
                        }

                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            //$("#DCNo").prop("disabled", "disabled");
                            //alert($("#Date").val());
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (!d.CashSale && d.Id > 0) {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }
                            if (d.Id > 0 && d.InternalStockTransferItems != null && d.InternalStockTransferItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                Common.MapItemData(d.InternalStockTransferItems);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();

                            }
                        }
                        if (d?.Status == 1) {
                            $("#saveBtn").addClass("disabled");
                        } else {
                            $("#saveBtn").removeClass("disabled");
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

                        if (d && d.AuthLocationId > 0)
                            $(`#AuthLocationName`).select2("val", d.AuthLocationId)
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
        Delete: function () {
            var $this = this;
            var type = VoucherType[$this.GetType()];
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var locationId = Common.GetInt($("#AuthLocationId").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno + "&locationId=" + locationId;
                var id = Common.GetInt($("#Id").val());
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
            var formSetting = $.parseJSON($("#jsondata #data").html());
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
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var productStock = $this.GetProductStock(account.Id);
                        let quantity = productStock?.Balance || 0
                        if (PageSetting.isMultipleLocationEnabled) {
                            const locationId = $("#FromLocationId").val();
                            const warehouseId = $("#FromWarehouseId").val();
                            quantity = $this.GetProductStockWarehouseAndLocationWise(locationId, warehouseId, account.Id)
                        }

                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.ArticleNo").val(product.ArticleNo);
                        $(tr).find("input.Stock").val(quantity || 0);
                        $(tr).find("input.Unit").val(product.UnitType);
                        $(tr).find("input.Rate").val((voucher == "saleorder" ? product.SalePrice : product.PurchasePrice));
                        $(".container-message").hide();
                    }
                }
            });
        },

        ResetFromWarehouse: function () {
            $(`#FromWarehouseName`).val(null);
            const locId = $("#FromLocationId").val();
            document.querySelectorAll('#FromWarehouseName option').forEach(el => {
                el.style.display = 'none';
                if (el.getAttribute('data-location') == locId) {
                    el.style.display = 'block';
                }
            });
        },
        ResetToWarehouse: function () {
            $(`#ToWarehouseName`).val(null);
            const locId = $("#ToLocationId").val();
            document.querySelectorAll('#ToWarehouseName option').forEach(el => {
                el.style.display = 'none';
                if (el.getAttribute('data-location') == locId) {
                    el.style.display = 'block';
                }
            });
        }
    };
}();

