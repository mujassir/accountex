
var WorkInProgress = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "WorkInProgress";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "OrderTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "voucher";
    return {
        init: function () {
            var $this = this;
            $(".date-picker").on("show", function (e) {
                //console.debug('show', e.date, $(this).data('stickyDate'));

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
            $("#OrderNo").keypress(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "")
                        $("#Comments").focus();
                    else
                        $this.LoadOrder();
                }
            });

            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
                    $this.LoadVoucher("same");
                    //setTimeout(function () {
                    //    $('#InvoiceNumber').focus();
                    //}, 1000);
                    //$('#InvoiceNumber').focus();
                }
            });
            $("#Comments").keypress(function (e) {

                if (e.which == 13) {
                    $("#rawitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }
            });
            $(document).on("keypress", ".Code", function (event) {

                var parent = $(this).closest("div[data-save='save']");

                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {

                        if ($(parent).find("#rawitem").length) {

                            $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", $("#finisheditem-container")).focus();

                        }
                        else {
                            $(".btn.btn-primary.green").focus();
                        }

                    }
                    else {

                        $("table tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity", parent).focus();

                    }

                }
            });
            $(document).on("blur", ".Code", function () {

                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var product = $this.GetByCode($(this).val());

                var tr = $(this).parent().parent();
                var parent = $(this).parent("div[data-save='save']");
                if (typeof product != "undefined" && product != null) {

                    $(tr).find(":nth-child(1) input.ItemId").val(product.AccountId);
                    $(tr).find(":nth-child(2) input.Name").val(product.Name);
                    $(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.PurchasePrice));

                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "<li>" + $(this).val() + " is not valid code.</li>";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                    $(tr).find("input.Quantity").focus();

                }

            });
            $(document).on("keyup", ".Quantity,.Rate,.Amount", function (event) {

                var tr = $(this).parent().parent();
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem(parent);
                else if (event.which == 13 && qty <= 0) {
                    var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $(document).on("click", "#OrderTable > tbody tr", function () {
                $this.SelectOrder(this);
            });
            this.LoadPageSetting();
            $this.GetCustomerProducts();
            $this.LoadAccountDetail();

            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }


        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        New: function () {

            var $this = this;
            SetFocus = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
        },

        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },

        GetByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },

        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            Common.Clear();
            this.DetailView();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        ListView: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (LIST_LOADED) {
                if (LIST_CHANGED) {
                    this.RebindData();
                    LIST_CHANGED = false;
                }
            } else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        AddItem: function (parent) {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).val();

            var code = $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }
            var html = "<tr>";
            html += "<td><input type='hidden' class='ItemId' id='ItemId'>";
            html += "<input type='hidden' id='Id' value=''>";
            html += "<input type='text' class='Code form-control typeahead input-small' data-plus-as-tab='false'></td>";
            html += "<td><input type='text' class='Name form-control input-medium'></td>";
            html += "<td class='align-right'><input type='text' class='Quantity form-control input-small'></td>";
            html += "<td class='align-right'><input type='text' class='Rate form-control input-small'></td>";
            html += "<td class='align-right'><input type='text' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WorkInProcess.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
            html += "</tr>";
            $("table tbody", parent).append(html);
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
            else {

                setTimeout(function () {
                    $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");

            $("#Item").focus();
            //}
            $this.AutoCompleteInit();
            Common.InitNumerics();

        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            var parent = $(elment).closest("div[data-save]");
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal(parent);
            if ($("table tbody", parent).children().length <= 0)
                $this.AddItem(parent);
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
                SetFocus = "date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                //$this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                //$this.CustomClear();
                $this.ListView();
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = "production";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Items.concat(Common.SaveItemData("#finishedgoods-item-container"));
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = $this.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += "<li>" + item.ItemCode + " is not valid code.</li>";
                    }

                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }


                record["TransactionType"] = VoucherType[voucher],
                record["WPItems"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving workin process  ...please wait",
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
            var parent = $(tr).closest("div[data-save]");
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

            $this.GetWholeTotal(parent);
        },
        GetWholeTotal: function (parent, israw) {

            var $this = this;
            var Quantity = 0;
            var Price = 0;
            if (!israw) {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
                    Price += Common.GetInt($(this).find(":nth-child(5) input.Amount").val());

                });
                $("table tfoot tr", parent).find(":nth-child(2) input.Quantity").val(Quantity);
                $("table tfoot tr", parent).find(":nth-child(3) input.Amount").val(Price);
                $("#FinishedQuantityTotal").val(Quantity);
                $("#FinishedNetTotal").val(Price);
            }
            else {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetInt($(this).find(":nth-child(5) input.Quantity").val());
                    Price += Common.GetInt($(this).find(":nth-child(7) input.Amount").val());
                });
                $("table tfoot tr", parent).find(":nth-child(2) input.Quantity").val(Quantity);
                $("table tfoot tr", parent).find(":nth-child(3) input.Amount").val(Price);
                $("#QuantityTotal").val(Quantity);
                $("#NetTotal").val(Price);

            }
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {

            $("#item-container tbody").html("");
            $("#finishedgoods-item-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            Common.Clear();
        },
        LoadOrder: function () {

            var $this = this;
            var orderno = Common.GetInt($("#OrderNo").val());
            var type = VoucherType.ginp;
            //var type = VoucherType[$this.GetType()]
            //if (type == VoucherType.goodissue)
            //    type = VoucherType.saleorder;

            //else
            //    type = VoucherType.purchaseorder

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?orderno=" + orderno + "&type=" + type,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $(".portlet .container-message").addClass("hide");
                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var rawitems = res.Data.RawMaterial;
                        var finisheditems = res.Data.Order.OrderItems;
                        if (rawitems == null && finisheditems == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("Order no is not valid.");
                        }
                            //else if (d.Status == 4) {
                            //    Common.ShowError("Order has already processed.");
                            //}
                        else {
                            var RawItems = new Array();
                            var FinishedItems = new Array();
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#Id").val(0);
                            $("#OrderId").val(res.Data.Order.Id);
                            Common.SetDate("#OrderDate", res.Data.Order.Date);
                            $("#item-container tbody").html("");
                            $("#finishedgoods-item-container tbody").html("");

                            if (rawitems != null && rawitems.length > 0) {
                                for (var i in rawitems) {
                                    var item = rawitems[i];
                                    var product = $this.GetByCode(item.ItemCode);
                                    var price = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        price = Common.GetFloat(product.SalePrice);
                                    }
                                    var amount = qty * price;
                                    item["Rate"] = price;
                                    item["Amount"] = amount;
                                    RawItems.push(item)
                                }
                                Common.MapItemData(RawItems, null, null, true);
                                $this.GetWholeTotal("#rawitem-container", true);
                            }
                            if (finisheditems != null && finisheditems.length > 0) {
                                for (var i in finisheditems) {
                                    var item = finisheditems[i];
                                    var product = $this.GetByCode(item.ItemCode);
                                    var price = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        price = Common.GetFloat(product.SalePrice);
                                    }
                                    var amount = qty * price;
                                    item["Rate"] = price;
                                    item["Amount"] = amount;
                                    FinishedItems.push(item)
                                }
                                Common.MapItemData(FinishedItems, "#finishedgoods-item-container", "#template-item-finishedgoods", true);
                                $this.GetWholeTotal("#finisheditem-container", false);
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


        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
            var type = $this.GetType();
            if (type == "sale") {
                $("#lblReportTitle").html("Sales Estimate");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "purchase") {
                $(".row-sale").addClass("hide");
                $("#lblReportTitle").html("Purchase Voucher");
                $("#lblFooterNotes").html(" * All imported items are not returnable");
            }

            var html = "";
            var items = d.SaleItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "<td>" + item.DiscountAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

            html = "";
            $("#tblAgingItems tbody").html("");
            for (var i in res.Data.AgingItems) {
                var item = res.Data.AgingItems[i];
                html += "<tr>";
                html += "<td></td>";
                html += "<td>" + item.VoucherNumber + "</td>";
                html += "<td></td>";
                html += "<td>" + moment(item.Date).format("DD/MM/YYYY") + "</td>";
                html += "<td></td>";
                html += "<td>" + item.DueAmount.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Balance.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Age + "</td>";
                html += "</tr>";
            }
            html += "<tr style='font-size:1.2em;'><th colspan='6' class='align-right' style='padding-top:10px;'>Current Balance :</th><th colspan='4' class='align-center' style='padding-top:10px;'>" + res.Data.Balance.format() + "</th></tr>";
            $("#tblAgingItems tbody").append(html);
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var voucher = "production";
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[voucher] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading workin process  ...please wait",
                success: function (res) {
                    if (res.Success) {

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

                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.WPItems != null && d.WPItems.length > 0) {
                                $("#OrderNo").prop("disabled", true);
                                if (res.Data.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.OrderDate);
                                }
                                $("#SearchIconOrder").addClass("hide");
                                var items = d.WPItems;
                                var rawitems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.RawMaterial;
                                });
                                Common.MapItemData(rawitems, null, null, true);
                                var finisheditems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.FinishedGoods;
                                });
                                Common.MapItemData(finisheditems, "#finishedgoods-item-container", "#template-item-finishedgoods", true);
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

                        //$this.AddItem($("#rawitem-container"));
                        //SetFocus = "booknumber";
                        //$this.AddItem($("#finisheditem-container"));
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetPreviousBalance: function (customerid) {
            var $this = this;
            var type = $this.GetType();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetPreviousBalance&accountid=" + customerid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var id = Common.GetInt($("#Id").val());
                        if (id == 0) {
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data));
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html(currentbalance - invoicetotal);

                        }
                        $this.CalculatePreviousBalance();


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        CalculatePreviousBalance: function () {

            var $this = this;
            var type = $this.GetType().toLowerCase();
            var currentbalance = 0;
            var invoicetotal = Common.GetFloat($("#NetTotal").val());
            var previousbalance = Common.GetFloat($("#lblpreviousbalance").html());

            var currentbalance = 0;
            if (type == "sale" || type == "purchasereturn") {
                currentbalance = previousbalance + invoicetotal;
            }
            else {
                currentbalance = previousbalance - invoicetotal;
            }

            $("#lblcurrentbalance").html(currentbalance);

        },
        GetCustomerProducts: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?&key=GetCustomerProducts",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        AppData.CustomerDiscount = res.Data;

                        //var html = "";
                        //var products = res.Data;
                        //html += "<option></option>";
                        //for (var i = 0; i < products.length; i++) {
                        //    var product = products[i];
                        //    html += "<option data-detail='" + JSON.stringify(product) + "' value='" + product.COAProductId + "'>" + product.ProductCode + "-" + product.ProductTitle + "</option>";
                        //}
                        //$("#Item").html(html).select2();

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
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
            var type = "production";
            Common.ConfirmDelete(function () {
                var id = Common.GetInt($("#Id").val());
                var voucherno = Common.GetInt($("#VoucherNumber").val());
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
                    blockMessage: "Deleting workin process  ...please wait",
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
                    var product = $this.GetByCode(ui.item.value);

                    var tr = $(this).parent().parent();
                    if (typeof product != "undefined" && product != null) {

                        $(tr).find(":nth-child(1) input.ItemId").val(product.AccountId);
                        $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.PurchasePrice));
                        //$(tr).find(":nth-child(3) input.Quantity").val(Common.GetFloat(product.PurchasePrice));
                        setTimeout(function () {
                            $(tr).find(":nth-child(3) input.Quantity").focus();
                        }, 500);
                        $(".container-message").hide();
                    }

                }
            });

        },
        GetOrders: function () {
            var $this = this;
            // //$("#Id").val('');
            //$("#SearchScope").val('responsive');
            // $("#PatientTable > tbody").html('');
            //if ($('#PatientSearch-Icon').hasClass("green") == true)

            $this.BindOrdersDatatable();
        },
        BindOrdersDatatable: function () {
            var $this = this;
            LIST_LOADED = false;
            var type = VoucherType.ginp;
            var url = Setting.APIBaseUrl + "GINP?type=" + type
            var options =
               {
                   "bPaginate": false,
                   "bSort": false,
               }
            DataTable.DestroyDatatable(ORDER_DATATABLE_ID);
            Common.BlockUI({
                blockUI: true,
                blockElement: "body",
                blockMessage: "Logging orders...please wait",
            });
            DataTable.BindDatatable(ORDER_DATATABLE_ID, url, options, function () {
                $("#Orders-Container").modal("show");
                if ($('.dataTable').parents(".dataTables_scroll").length == 0)
                    $('.dataTable').wrap('<div class="dataTables_scroll" />');
                App.unblockUI($("body"));
            });
        },
        SelectOrder: function (tr) {
            var $this = this;
            var orderno = $(tr).find("input.OrderNo").val();
            if (orderno.trim() != "" && orderno != null) {
                $("#OrderNo").val(orderno);
                $this.LoadOrder("challan");
                $('#btnOrderClose').click();
            }
        },

    };
}();
