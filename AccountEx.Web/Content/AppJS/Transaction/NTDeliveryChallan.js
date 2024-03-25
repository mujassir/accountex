
var NTDeliveryChallan = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "NTDeliveryChallan";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "OrderTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#Date";
    return {
        init: function () {

            var $this = this;
            jQuery('.dataTable').wrap('<div class="dataTables_scroll" />');
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
                            $("#item-container tbody tr:nth-last-child(1) input.Width").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }
            });
            $(document).on("keyup change", "input.Rate", function () {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Rolls,.Meters,.GSTPercent ", function (event) {
                var tr = $(this).parent().parent();
                var value = Common.GetInt($(tr).find("input.NetAmount").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                //if (event.which == 13 && value > 0)
                //    $this.AddItem();
                //else if (event.which == 13 && value <= 0) {
                //    var err = "Item " + code + " must have Value greater than zero(0).,";
                //    Common.ShowError(err);
                //}
            });
            $(document).on("change", ".Type", function () {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Rate,.ActualWeight,.Quantity", function () {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
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
            $(document).on("change", "input[name='sale-type']", function () {
                var type = $('input[name=sale-type]:checked').val();
                if (type == VoucherType.gstsale || type == VoucherType.gstpurchase) {
                    $("tr [data-sale-type='gst']").removeClass("hide");
                    //$("#item-container  tbody  tr").find("input.GSTPercent[value=''],input.GSTPercent[value='0']").val(PageSetting.GST);

                    $("#item-container  tbody  tr").find("input.GSTPercent").each(function () {
                        var gstPercent = Common.GetInt($(this).val());
                        if (gstPercent <= 0) {
                            $(this).val(PageSetting.GST)
                        }
                    });
                }
                else {

                    $("tr [data-sale-type='gst']").addClass("hide");
                    $("#item-container  tbody tr input.GSTPercent").val(0);
                }
                Common.InitNumerics();
                $("#item-container tbody tr").each(function () {
                    $(this).find("input.Rate").trigger("change");
                });

            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    //console.log(account);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);

                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(3) input.Article").val(product.Article);
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
                var code = $(tr).find("input.Code").val();
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var netamount = Common.GetInt($(tr).find("input.NetAmount").val());

                if (event.which == 13 && netamount > 0)
                    $this.AddItem();
                else if (event.which == 13 && netamount <= 0) {
                    var err = "Item " + code + " must have Value greater than zero(0).,";
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
            $this.LoadPageSetting();
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.VehicleAutoCompleteInit();
            $('input[name=sale-type]:checked').trigger("change");
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
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
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
            Common.InitNumerics();
            $('input[name=sale-type]:checked').trigger("change");
        },
        LoadOrder: function (key) {
            var $this = this;
            var orderno = Common.GetInt($("#OrderNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.goodissue)
                type = VoucherType.saleorder;
            else
                type = VoucherType.purchaseorder

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "OrderBooking/" + orderno + "?type=" + type + "&key=" + key + "&voucher=" + orderno,
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
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            Common.ShowError("Order no is not valid.");
                        }
                        else if (d.Status == 4) {
                            Common.ShowError("Order has already processed.");
                        }
                        else {

                            Common.MapEditData(d, "#form-info");
                            $("#Comments").val("");
                            $("#OrderId").val(d.Id);
                            $("#VoucherNumber").val(voucherno);
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
                                    x.Quantity = x.Quantity - x.QtyDelivered

                                });

                                for (var i in items) {
                                    var item = items[i];
                                    var product = Common.GetAccountDetailByAccountId(item.ItemId);
                                    item.Weight = product.Weight;
                                }

                                Common.MapItemData(items);
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
                $("#item-container tbody tr").find("input.Rate").trigger("change");
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += $("#AccountCode").val() + " is not valid party code.";
                }

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
                record["InvoiceTransactionType"] = $('input[name=sale-type]:checked').val();
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
        GetAmountTotal: function (tr) {
            var type = $('input[name=sale-type]:checked').val();
            if (type == VoucherType.gstsale || type == VoucherType.sale) {


            }
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var quantity = 0.0;
            var rate = 0.0;
            var Rolls = 0.0;
            var Meters = 0.0;
            var type = $('input[name=sale-type]:checked').val();
            quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            rate = Common.GetFloat($(tr).find("input.Rate").val());
            if (type == VoucherType.gstsale || type == VoucherType.sale) {
                var code = $(tr).find("input.Code").val();
                Rolls = Common.GetFloat($(tr).find("input.Rolls").val());
                Meters = Common.GetFloat($(tr).find("input.Meters").val());
                var totalmeters = Rolls * Meters;
                $(tr).find("input.TotalMeters").val(totalmeters.toFixed(2));
                var width = Common.GetFloat($(tr).find("input.Width").val());
                var weight = Common.GetFloat($(tr).find("input.Weight").val());
                var standardweight = Common.GetFloat((((width * weight) / 39.37) / 1000) * totalmeters);
                $(tr).find("input.StandardWeight").val(standardweight.toFixed(2));


                var actualweight = Common.GetFloat($(tr).find("input.ActualWeight").val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var totalmeters = Common.GetFloat($(tr).find("input.TotalMeters").val());
                var type = $(tr).find(".Type").val();
                var amount = 0;
                if (type == "Weight") {
                    amount = actualweight * rate;
                }
                if (type == "Meter") {
                    amount = totalmeters * rate;
                }
                if (type == "Quantity") {
                    amount = quantity * rate;
                }
                $(tr).find("input.Amount").val(amount);
                var netamount = Common.GetInt($(tr).find("input.NetAmount").val());

                amount = Common.GetInt($(tr).find("input.Amount").val());

            }
            else {
                amount = quantity * rate;
            }


            $(tr).find("input.Amount").val(amount);

            var gstpercent = Common.GetInt($(tr).find("input.GSTPercent").val());
            var amount = Common.GetInt($(tr).find("input.Amount").val());
            var gstamount = Common.GetFloat(amount * gstpercent / 100);
            var netAmount = Common.GetInt(amount + gstamount);
            $(tr).find("input.GSTAmount ").val(gstamount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var amount = 0.0;
            var netAmount = 0.0;
            var gstAmount = 0.0;
            var quantity = 0.0;
            var totalMeters = 0.0;
            var totalStdWeight = 0.0;
            var totalActWeight = 0.0;
            $("#item-container tbody tr").each(function () {
                totalMeters += Common.GetFloat($(this).find("input.TotalMeters").val());
                totalStdWeight += Common.GetFloat($(this).find("input.StandardWeight").val());
                totalActWeight += Common.GetFloat($(this).find("input.ActualWeight").val());
                quantity += Common.GetFloat($(this).find("input.Quantity").val());
                netAmount += Common.GetFloat($(this).find("input.NetAmount").val());
                amount += Common.GetFloat($(this).find("input.Amount").val());
                gstAmount += Common.GetInt($(this).find("input.GSTAmount").val());
            });
            $("#item-container tfoot tr").find("input.TotalMeters").val(totalMeters.toFixed(2));
            $("#item-container tfoot tr").find("input.StandardWeight").val(totalStdWeight.toFixed(2));
            $("#item-container tfoot tr").find("input.ActualWeight").val(totalActWeight.toFixed(2));
            $("#item-container tfoot tr").find("input.Quantity").val(quantity);
            $("#item-container tfoot tr").find("input.NetAmount").val(netAmount);
            $("#item-container tfoot tr").find("input.Amount").val(amount);
            $("#item-container tfoot tr").find("input.GSTAmount").val(gstAmount);



            if (totalActWeight > 0) {
                $("#NTQtyTotal").val(quantity.toFixed(2));
                quantity = totalActWeight;
            }
            $("#QuantityTotal").val(quantity);
            $("#GrossTotal").val(amount);
            $("#GstAmountTotal").val(gstAmount);
            $("#NetTotal").val(netAmount);
        },
        CustomClear: function () {
            $("#item-container tbody").html("");
            Common.Clear();
            $("#btndelete,#btnprint").prop("disabled", true);
        },

        //LoadReportData: function (res) {
        //    var $this = this;
        //    var d = res.Data.Order;
        //    if (d == null)
        //        return;
        //    Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
        //    var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
        //    $("#lblDescription").html(instruction);
        //    $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
        //    $("#lblDeliveryRequired").html((d.DeliveryRequired ? moment(d.DeliveryRequired).format("dddd, DD-MM-YYYY") : ''));
        //    var type = $this.GetType();
        //    var html = "";
        //    var items = d.DCItems;
        //    var index = 1;

        //    var totalamount = 0.0;

        //    $("#report-saleitem tbody").html("");
        //    for (var i in items) {
        //        var item = items[i];
        //        html += "<tr>";
        //        html += "<td>" + (index++) + "</td>";
        //        html += "<td>" + item.Article + "</td>";
        //        html += "<td>" + item.ItemName + "</td>";
        //        html += "<td>" + item.Quantity + "</td>";
        //        //html += "<td>" + item.Rate.format() + "</td>";
        //        //html += "<td>" + item.Amount.format() + "</td>";
        //        //html += "<td>" + item.DiscountAmount.format() + "</td>";
        //        //html += "<td>" + item.NetAmount.format() + "</td>";
        //        html += "</tr>";

        //        totalamount += item.Amount;
        //    }
        //    $("#lblTotalAmount").html(totalamount.format());
        //    $("#report-saleitem tbody").append(html);

        //},

        LoadReportData: function (res) {
            var $this = this;
            var qty = 0.0;
            var totalMeters = 0.0;
            var standardWeight = 0.0;
            var actualWeight = 0.0;
            var amount = 0.0;
            var gstAmount = 0.0;
            var netAmount = 0.0;

            var d = res.Data.Order;
            var companyPartner = res.Data.CompanyPartner;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDCNo").html($("#DCNo").val());
            if(d != null)
                $("#lblDate").html((d.Date != null ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
            //$("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            //$("#lblContactPerson").html(res.Data.ContactPerson);
            if (companyPartner != null) {
                $("#lblPartnerName").html(companyPartner.Name);
                $("#lblPartnerAddress").html(companyPartner.Address);
                $("#lblNTNNo").html(companyPartner.NTNNo);
                $("#lblGSTNo").html(companyPartner.GSTNo);
            }
            var type = VoucherType[$this.GetType()];
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
            var items = d.DCItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            if (d.InvoiceTransactionType == VoucherType.sale || d.InvoiceTransactionType == VoucherType.purchase) {
                $("#report-saleitem thead tr").find(":nth-child(12),:nth-child(13),:nth-child(14)").addClass("hide");
            }
            else {
                $("#report-saleitem thead tr").find(":nth-child(12),:nth-child(13),:nth-child(14)").removeClass("hide");
            }
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Weight + "</td>";
                if (type == VoucherType.goodreceive) {
                    $("#report-saleitem thead tr").find(":nth-child(4),:nth-child(5),:nth-child(6),:nth-child(7),:nth-child(8),:nth-child(9)").addClass("hide");
                    html += "<td>" + item.Quantity + "</td>";
                    qty += item.Quantity;
                }
                if (type == VoucherType.goodissue) {
                    $("#report-saleitem thead tr").find(":nth-child(4),:nth-child(5),:nth-child(6),:nth-child(7),:nth-child(8),:nth-child(9)").removeClass("hide");
                    html += "<td>" + item.Width + "</td>";
                    html += "<td>" + item.Rolls + "</td>";
                    html += "<td>" + item.Meters + "</td>";
                    html += "<td>" + item.TotalMeters + "</td>";
                    html += "<td>" + item.StandardWeight + "</td>";
                    html += "<td>" + item.Quantity + "</td>";
                    html += "<td>" + item.NTQty + "</td>";
                    qty += item.NTQty;
                    totalMeters += item.TotalMeters;
                    standardWeight += item.StandardWeight;
                    actualWeight += item.Quantity;
                    //if (item.UnitType == "Weight") {
                    //    html += "<td>" + item.Quantity + "</td>";
                    //    $("#report-saleitem thead tr").find(":nth-child(8)").text("Actual Weight");
                    //}
                    //else if (item.UnitType == "Meter") {
                    //    html += "<td>" + item.TotalMeters + "</td>";
                    //    $("#report-saleitem thead tr").find(":nth-child(8)").text("Total Meter");
                    //}
                    //else if (item.UnitType == "Quantity") {
                    //    html += "<td>" + item.NTQty + "</td>";
                    //    $("#report-saleitem thead tr").find(":nth-child(8)").text("Quantity");
                    //}
                }
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                if (d.InvoiceTransactionType == VoucherType.gstsale || d.InvoiceTransactionType == VoucherType.gstpurchase) {
                    html += "<td>" + item.GSTPercent + "</td>";
                    html += "<td>" + item.GSTAmount.format() + "</td>";
                    html += "<td>" + item.NetAmount.format() + "</td>";
                }
                html += "</tr>";

                amount += item.Amount;
                gstAmount += item.GSTAmount;
                netAmount += item.NetAmount;
            }

            $("#report-saleitem tbody").append(html);
            if (d.InvoiceTransactionType == VoucherType.gstsale || d.InvoiceTransactionType == VoucherType.gstpurchase) {
                $("#report-saleitem tfoot tr").find(":nth-last-child(1),:nth-last-child(2),:nth-last-child(3)").removeClass("hide");
            }
            else {
                $("#report-saleitem tfoot tr").find(":nth-last-child(1),:nth-last-child(2),:nth-last-child(3)").addClass("hide");
            }


            if (type == VoucherType.goodreceive) {
                $("#report-saleitem tfoot tr").find(":nth-child(3)").html(qty);
                $("#report-saleitem tfoot tr").find(":nth-child(5)").html(amount.format());
                $("#report-saleitem tfoot tr").find(":nth-child(7)").html(gstAmount.format());
                $("#report-saleitem tfoot tr").find(":nth-child(8)").html(netAmount.format());
            }
            if (type == VoucherType.goodissue) {
                $("#report-saleitem tfoot tr").find(":nth-child(6)").html(totalMeters);
                $("#report-saleitem tfoot tr").find(":nth-child(7)").html(standardWeight);
                $("#report-saleitem tfoot tr").find(":nth-child(8)").html(actualWeight);
                $("#report-saleitem tfoot tr").find(":nth-child(9)").html(qty);
                $("#report-saleitem tfoot tr").find(":nth-child(11)").html(amount.format());
                $("#report-saleitem tfoot tr").find(":nth-child(13)").html(gstAmount.format());
                $("#report-saleitem tfoot tr").find(":nth-child(14)").html(netAmount.format());
            }

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
            html += "<tr style='font-size:1.2em;'><th colspan='6' class='align-right' style='padding-top:10px;'>Current Balance :</th><th colspan='4' class='align-center' style='padding-top:10px;'>" + res.Data.Balance + "</th></tr>";
            $("#tblAgingItems tbody").append(html);
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
                            $("#OrderNo").prop("disabled", false);
                            $("#SearchIconOrder").removeClass("hide");
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            if (d.InvoiceTransactionType == VoucherType.gstsale) {
                                $("input:radio[value='" + VoucherType.gstsale + "']").prop("checked", true);
                            }
                            if (d.InvoiceTransactionType == VoucherType.sale) {
                                $("input:radio[value='" + VoucherType.sale + "']").prop("checked", true);
                            }
                            if (d.InvoiceTransactionType == VoucherType.gstpurchase) {
                                $("input:radio[value='" + VoucherType.gstpurchase + "']").prop("checked", true);
                            }
                            if (d.InvoiceTransactionType == VoucherType.purchase) {
                                $("input:radio[value='" + VoucherType.purchase + "']").prop("checked", true);
                            }
                            $.uniform.update();
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
                        $("#item-container tbody tr").find("input.Rate").trigger("change");
                        $this.AddItem();
                        if (res.Data.Order != null)
                            $this.LoadReportData(res);
                        Common.SetPageAccess(d);
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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucher=" + voucherno;
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
        VehicleAutoCompleteInit: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.VehicleHeadId);
            var suggestion = new Array();
            for (var i in accounts) {
                var account = accounts[i];
                suggestion.push(
                    {
                        id: account.Id,
                        value: account.AccountCode,
                        name: account.DisplayName,
                        label: account.AccountCode + "-" + account.DisplayName

                    }
                );
            }

            $("#VehicleCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    $("#VehicleNo").val(ui.item.name);
                    $("#VehicleId").val(ui.item.id);
                    $(".btn.btn-primary.green").focus();
                }
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
            var exIds = new Array(PageSetting.Products);
            var tokens = Common.GetAllLeafAccounts(exIds);
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
                        //$("#Comments").val("Sold To: " + d.AccountCode + "-" + d.Name);
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
                        $(tr).find("input.Weight").val(product.Weight);
                        // $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);

                        //if (typeof account != "undefined" && account != null) {
                        //    var product = Common.GetAccountDetailByAccountId(account.Id);
                        //    $(tr).find(":nth-child(1) input.ItemId").val(product.Id);
                        //    $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(tr).find(":nth-child(3) input.Article").val(product.Article);
                        $(tr).find(":nth-child(5) input.Unit").val(product.UnitType);
                        $(tr).find(":nth-child(6) input.Rate").val((voucher == "goodissue" ? product.SalePrice : product.PurchasePrice));

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
            if (orderno.trim() != "" && orderno != null) {
                $("#OrderNo").val(orderno);
                $this.LoadOrder("challan");
                $('#btnOrderClose').click();
            }
        }



    };
}();

