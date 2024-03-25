
var IronSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "IronSale";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    var PriceType = "";
    return {
        init: function () {
            var $this = this;
            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            $("input[name='ac-type']").change(function () {

                if ($(this).val() == "cash") {
                    var acc = Common.GetById(PageSetting.CashAccount);
                    $("#AccountId").val(acc.Id);
                    $("#AccountCode").val(acc.AccountCode);
                    $("#AccountName").val(acc.DisplayName);
                    $("#AccountCode").attr("disabled", "disabled");
                    $this.MapComments();
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }
                else {
                    $("#AccountCode").removeAttr("disabled");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                    $("#AccountName").val("");
                    $("#InvoiceNumber").focus();
                }

            });
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");

                }
            });
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {

                    $("#AccountCode").focus();
                }

            });
            $("#BiltyNo").keyup(function (e) {

                if (e.which == 13) {
                    $("#BiltyDate").focus();

                }
            });
            $("#BiltyDate").keyup(function (e) {

                $("#ShipViaCode").focus();



            });
            $("#InvoiceNumber").keyup(function (e) {

                if (e.which == 13) {
                    $("#Date").focus();

                }
            });
            $("#ShipQty").keypress(function (e) {
                if (e.which == 13) {

                    $(".btn.btn-primary.green").focus();
                }
            });
            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
            });
            $("#DCNo").keyup(function (e) {

                if (e.which == 13) {

                    if ($(this).val() == "")
                        $("#AccountCode").focus();
                    else
                        $this.LoadDeliveryChallan("challan");



                }
                $this.MapComments();
            });
            $("#ShipViaCode").keyup(function (e) {
                if (e.which == 13) {
                    $(".btn.btn-primary.green").focus();
                }
            });
            $(document).on("keyup", ".Code", function (event) {

                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).closest("tr");
                            if (typeof product != "undefined" && product != null) {
                                var account = Commmon.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.Name").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.AddItem();
                            }
                            else {
                                if ($(this).val().trim() != "") {
                                    var err = $(this).val() + " is not valid code.,";
                                    Common.ShowError(err);

                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) input.Comment").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }

            });
            $(document).on("keyup", "#item-container tbody input.Godown", function (event) {
                var min = 1;
                var max = 3;
                var value = $(this).val();
                if (value == "" || value == " ") {
                    event.preventDefault();
                    return false;
                }
                else if (Common.GetInt(value) < min) {
                    event.preventDefault();
                    $(this).val(min);
                    return false;
                }

                else if (Common.isNullOrWhiteSpace(value)) {
                    event.preventDefault();
                    $(this).val(min);
                    return false;
                }
                else if (Common.GetInt(value) > max) {
                    event.preventDefault();
                    $(this).val(max);
                    return false;
                }
                else return value;
            });

            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {

                    var party = Common.GetByCode($(this).val());

                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = "" + $(this).val() + " is not valid party code.,";
                            Common.ShowError(err);
                        }
                    }




                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));

                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.,";
                            Common.ShowError(err);

                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).closest("tr");

                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var weight = Common.GetInt($(tr).find("input.Weight").val());
                var godown = Common.GetInt($(tr).find("input.Godown ").val());

                if (qty <= 0) {
                    $(tr).find("input.Quantity").val("0");
                }
                if (weight <= 0) {
                    $(tr).find("input.Weight").val("0");
                }
                if (godown <= 0) {
                    $(tr).find("input.Godown").val("1");
                }

                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("blur", ".Weight", function (event) {

                var tr = $(this).closest("tr");

                var weight = Common.GetInt($(tr).find("input.Weight").val());


                if (weight <= 0) {
                    $(tr).find("input.Weight").val("0");
                }
                $this.GetQuantityPriceTotal(tr);
                $this.MapComments();

            });
            $(document).on("keyup", ".Quantity", function (event) {
                var tr = $(this).closest("tr");
                var code = $(tr).find("input.Code").val();
                var size = $(tr).find("input.Size").val();
                if (event.which == 13) {
                    if (typeof size != "undefined" && size.trim() != "") {
                        focusElement = "size";
                        $this.AddItem();
                        $("#item-container tbody tr:nth-last-child(1) input.Code").val(code);
                        $("#item-container tbody tr:nth-last-child(1) input.Code").trigger("blur");
                    }
                    else {
                        $(tr).find("input.Weight").focus();
                    }

                }

                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Size", function (event) {
                var tr = $(this).closest("tr");

                if (event.which == 13) {
                    $(this).parent().next().find("input.Quantity").focus();
                }
                else if (event.which == 88) {
                    $(this).parent().next().find("input.Quantity").focus();
                    var size = $(this).val();
                    $(this).val(size.substr(0, (size.length - 1)));
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("keyup", ".Weight,.Rate,.Amount ", function (event) {
                var tr = $(this).closest("tr");
                if (event.which == 13) {
                    // $this.AddItem();
                    $(tr).find("input.Godown").focus();
                }


                $this.GetQuantityPriceTotal(tr);



            });
            $(document).on("keyup", ".Godown", function (event) {
                var tr = $(this).closest("tr");
                if (event.which == 13) {
                    $this.AddItem();

                }


                $this.GetQuantityPriceTotal(tr);



            });
            $(document).on("keyup", ".Comment", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var weight = Common.GetInt($(tr).find("input.Weight").val());

                //if (qty <= 0)
                //    $(tr).find("input.Quantity").val("1");
                if (event.which == 13) {
                    $this.AddItem();
                    if (weight <= 0)
                        $(tr).find("input.Weight").val("0");
                }
                $this.GetQuantityPriceTotal(tr);
            });
            $(document).on("blur", ".Rate", function (event) {
                $this.MapComments();
            });
            $("#Rate").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $this.AddItem();
            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $this.GetPreviousBalance(party.Id);
                    var address = party.Address;
                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $("#Cutting,#Loading,#Carriage,#WHT,#GST").keyup(function (e) {
                $this.GetNetTotal();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();

            this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            AppData.CustomerDiscount = PageSetting.Discounts;
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {

                if (Setting.PageLandingView == "DetailView") {
                    this.Add();
                }

            }

        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            var html = "Book No:" + $("#InvoiceNumber").val();
            $("#item-container tbody tr").each(function () {
                var code = $(this).find("input.Code").val()
                if (code != "undefined" && code.trim() != "") {
                    var weight = $(this).find("input.Weight").val();
                    var rate = $(this).find("input.Rate").val()
                    if (weight > 0 && rate > 0)
                    html += "," + weight + "/" + rate
                }


            })
            $("#Comments").val(html);

        },
        New: function () {

            var $this = this;
            focusElement = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {

            this.CustomClear();
            this.GetNextVoucherNumber();
        },
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#item-container tbody tr:nth-last-child(1) input.Code").val();


            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1)  input.Code").focus().select();
                }, 300);

                focusElement = "code";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            Common.InitNumerics();
            if (focusElement == "size") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1)  input.Size").focus().select();
                }, 300);
            }
            else if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.SalesmanCode").focus().select();
                    focusElement = "";
                }, 300);

            }
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
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
                var scope = $("#form-info-item");
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
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
                    err = "<li>You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ") </li>";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                $("#item-container tbody input.Rate").trigger("keyup");
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += "" + $("#AccountCode").val() + " is not valid party code.,";
                }

                for (var i in Items) {
                    var item = Items[i];
                    //if (item.Weight <= 0 && item.Quantity <= 0) {
                    //    err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have weight greater than zero(0).,";
                    //}

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.,";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                //else {
                //    var items = Enumerable.From(item).GroupBy("$.ItemId", null,
                //   function (key, g) {
                //       var result = {
                //           ItemId: key,
                //           TotalWeight: g.Sum(function (x) { return x.Weight }),
                //           TotalAmount: g.Sum(function (x) { return x.Amount }),
                //           Item: g.FirstOrDefault(null, function (x) { return x.Weight > 0 && x.Rate > 0 }),
                //           SubItems: g.ToArray(),
                //           TotalItem: g.Count(),
                //           SizeQuantity: g.Select(function (x) { return x.Size + "/" + x.Quantity }).ToArray(),
                //       }
                //       return result;
                //   }).ToArray();
                //}
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "Transaction total amount should be graeter then zero(0).,";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["TransactionType"] = VoucherType[voucher],
                record["SaleItems"] = Items;
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
            var Weight = 0;
            var Rate = 0;
            Weight = Common.GetFloat($(tr).find("input.Weight").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Common.GetInt(Weight * Rate);
            $(tr).find("input.Amount").val(amount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var size = 0;
            var Quantity = 0;
            var Price = 0;

            var Weight = 0;
            $("#item-container tbody tr").each(function () {
                size += Common.GetInt($(this).find("input.Size").val());
                Quantity += Common.GetInt($(this).find("input.Quantity").val());
                Weight += Common.GetFloat($(this).find("input.Weight").val());
                Price += Common.GetInt($(this).find("input.Amount").val());
            });
            $("#item-container tfoot tr").find("input.Size").val(size);
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity);
            $("#item-container tfoot tr").find("input.Weight").val(Weight);
            $("#item-container tfoot tr").find("input.Amount").val(Price);
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            this.GetNetTotal();

        },
        GetNetTotal: function () {
            var $this = this;
            var grosstotal = Common.GetFloat($("#GrossTotal").val());
            var cutting = Common.GetFloat($("#Cutting").val());
            var loading = Common.GetFloat($("#Loading").val());
            var carriage = Common.GetFloat($("#Carriage").val());
            var wht = Common.GetFloat($("#WHT").val());
            var gst = Common.GetFloat($("#GST").val());
            var nettotal = grosstotal + cutting + loading + carriage + wht + gst;
            $("#NetTotal").val(nettotal);
            $this.CalculatePreviousBalance();
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

        LoadDeliveryChallan: function (key) {

            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan/" + orderno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;


                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else if (d.Status == 5) {
                            Common.ShowError("Challan has already processed.");
                            focusElement = "dcnumber";
                        }
                        else {
                            Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#qtytotal1").val(d.QuantityTotal);
                            $("#AccountCode").trigger("blur");
                            $("#Id").val(0);
                            $("#item-container tbody").html("");
                            $("#OBNo").val(d.InvoiceNumber);
                            $("#OrderDate").val(d.Date);

                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {




                                var html = "";

                                for (var i in items) {
                                    var item = items[i];


                                    var product = Common.GetByCode(item.ItemCode);

                                    var discountpercent = 0;
                                    var price = 0;
                                    var discount = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        discountpercent = Common.GetFloat($this.GetDiscountDetail(product.AccountId));
                                        price = Common.GetFloat(product.SalePrice);

                                    }


                                    var amount = qty * price;
                                    var discountAmount = Common.GetInt(amount) * discountpercent / 100;
                                    var netAmount = Common.GetInt(amount) - discountAmount;


                                    html += "<tr>";
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden' id='Id' vvalue='" + item.Id + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td class='align-right'><input type='text' class='Quantity form-control input-small num3' value='" + item.Quantity + "'></td>";
                                    html += "<td class='align-right><input type='text' class='Rate form-control input-small num3' value='" + price + "' ></td>";
                                    html += "<td class='align-right><input type='text' value='" + amount + "' class='Amount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td><input type='text' value='" + discountpercent + "' class='DiscountPercent form-control input-small'></td>";
                                    html += "<td class='align-right><input type='text' value='" + discountAmount + "'  class='DiscountAmount  form-control input-small num3' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td class='align-right><input type='text' value='" + netAmount + "' class='NetAmount form-control input-small num3' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"IronSale.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#item-container tbody").append(html);
                                Common.InitNumerics();
                                $this.MapComments();


                            }
                            $this.GetWholeTotal();
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
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
            var type = $this.GetType();
            if (type == "sale") {
                $("#lblReportTitle").html("Sales Invoice");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "purchase") {
                $(".row-sale").addClass("hide");
                $("#lblReportTitle").html("Purchase Voucher");
                $("#lblFooterNotes").html(" * All imported items are not returnable");
            }

            var html = "";
            // Enumerable.From(g).FirstOrDefault(null, "$.Id == '" + jobcat.CategoryId + "'"),
            var items = d.SaleItems;
            var items = Enumerable.From(d.SaleItems).GroupBy("$.ItemId", null,
                   function (key, g) {
                       var result = {
                           ItemId: key,
                           TotalWeight: g.Sum(function (x) { return x.Weight }),
                           TotalAmount: g.Sum(function (x) { return x.Amount }),
                           Item: g.FirstOrDefault(g.FirstOrDefault(), function (x) { return x.Weight > 0 && x.Rate > 0 }),
                           SubItems: g.ToArray(),
                           TotalItem: g.Count(),
                           SizeQuantity: g.Select(function (x) { return x.Size + "/" + x.Quantity }).ToArray(),
                       }
                       return result;
                   }).ToArray();
            var index = 1;
            $("#report-item-container tbody").html("");
            for (var i in items) {
                var itemDetail = items[i];
                var item = itemDetail.Item;
                html += "<tr class='header-row'>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + itemDetail.TotalWeight + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + itemDetail.TotalAmount.format() + "</td>";
                html += "</tr>";
                var subItems = itemDetail.SubItems;
                html += "<tr class='sizequantiy-row'>";
                html += "<td colspan='5'><label><strong>Detail:</strong></label>" + itemDetail.SizeQuantity.join(",") + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").html(html);

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
            $("#tblAgingItems tbody").html(html);
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
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            $("#ManualVoucherNumber").val(res.Data.ManualVoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
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

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);

                                var items = d.SaleItems;
                                Common.MapItemData(items);


                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
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
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
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
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data).format());
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html((currentbalance - invoicetotal).format());

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

            $("#lblcurrentbalance").html((currentbalance).format());

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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno;
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;
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
                            //$this.CustomClear();
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
            //switch ($this.GetType().toLowerCase()) {
            //    case "sale":
            //        id = PageSetting.Customers;
            //        break;
            //    case "salereturn":
            //        id = PageSetting.Customers;
            //        break;
            //    case "purchase":
            //        id = PageSetting.Suppliers;

            //        break;
            //    case "purchasereturn":
            //        id = PageSetting.Suppliers;
            //        break;
            //}
            var inids = new Array();
            //inids.push(Common.GetInt(PageSetting.Customers));
            //inids.push(Common.GetInt(PageSetting.Suppliers));
            var tokens = Common.GetAllLeafAccounts(inids);

            //var tokens = Common.GetLeafAccounts(id);
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
                    var type = $this.GetType();

                    if (typeof d != "undefined" && d != null) {
                        if (type == "sale") {
                            $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("Sale Return From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Purchase From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.Code + "-" + d.Name);
                        }
                        $("#AccountName").val(d.Name);
                        $("#AccountId").val(d.Id);
                        Common.GetPartyAddress(d.Id);
                        $(".container-message").hide();
                    }
                    $this.MapComments();
                }
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
            $("#account-type-conatiner").addClass("hide");
            if (voucher == "sale") {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").removeClass("hide");
                $("#account-type-conatiner").removeClass("hide");
            }
            else
                $("#DCNo,#lbldc,#lblorderno,#OrderNo").addClass("hide");
            if (voucher == "sale") {
                $(".sale-content").removeClass("hide");
                $(".other-content").addClass("hide");
            }
            else {
                $(".other-content").removeClass("hide");
                $(".sale-content").addClass("hide");
            }

            this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
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

                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Godown").val(1);


                        $(tr).find("input.Rate").val(Common.GetFloat(voucher == "sale" || voucher == "salereturn" ? product.SalePrice : product.PurchasePrice));
                        $this.GetQuantityPriceTotal(tr);
                        //$(tr).find("input.Size").focus();
                    }

                }
            });

            $("#item-container tbody input.Godown").autocomplete({
                source: ["1", "2", "3"],
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                }
            });

        },
        TransporterAutoCompleteInit: function (data) {
            var $this = this;
            var tokens = data;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.Code,
                        label: token.Code + "-" + token.Name,
                        Name: token.Name

                    }
                );
            }

            $("#ShipViaCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    $("#ShipViaName").val(ui.item.Name);
                    $("#ShipViaId").val(ui.item.id);
                    $(".btn.btn-primary.green").focus();
                }


            });

        },
    };
}();