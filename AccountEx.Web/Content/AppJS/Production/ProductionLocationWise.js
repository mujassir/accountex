
var Production = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "ProductionUnit";
    var DATATABLE_ID = "mainTable";
    var ORDER_DATATABLE_ID = "OrderTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "voucher";
    var stockRequired = false;
    var record = null;
    var VOUCHER_TYPE = VoucherType.production;
    return {
        init: function () {
            var $this = this;
            VOUCHER_TYPE = Common.GetInt($("#VoucherType").val());
            Common.LoadProductStockWarehouseAndLocationWise();
            $("#Quantity").keyup(function (e) {
                if (e.which == 13) {
                    $this.GetReceipe();
                }
            });

            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
                    $this.LoadVoucher("same");
                }
            });
            $(document).on("change", "#LocationId", function (event) {
                $this.ResetMachine();
                $this.ResetWarehouse();
                $this.LoadVoucher("nextvouchernumber");
            })
            $(document).on("change", "#LocationId, #MachineId", function (event) {
                var qty = Common.GetFloat($("#Quantity").val());
                const locationId = Common.GetInt($("#LocationId").val());
                const machineId = Common.GetInt($("#MachineId").val());
                if (qty && locationId && machineId) {
                    $('#ProductId').select2("enable", true);
                }
                else {
                    $('#ProductId').select2("enable", false);
                }
            })
            $("#Quantity").keyup(function (e) {
                var qty = Common.GetFloat($("#Quantity").val());
                const locationId = Common.GetInt($("#LocationId").val());
                const machineId = Common.GetInt($("#MachineId").val());
                if (qty && locationId && machineId) {
                    $('#ProductId').select2("enable", true);
                }
                else {
                    $('#ProductId').select2("enable", false);
                }
            });
            $("#Comments").keypress(function (e) {

                if (e.which == 13) {
                    $("#rawitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }
            });
            $(document).on("keypress", ".Code", function (event) {

                var parent = $(this).closest("div.parent-conatiner");


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
            $(document).on("blur", "#finisheditem-container .Quantity, #item-container .WasteQty", function () {
                $this.calculateQty();
            })
            $(document).on("blur", ".Code", function () {
                var parent = $(this).closest("div.parent-conatiner");
                if ($(parent).attr("id") != "expensesitem-container") {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = Common.GetAccountDetailByCode($(this).val());

                    var tr = $(this).parent().parent();
                    var parent = $(this).closest("div.parent-conatiner");
                    if (typeof product != "undefined" && product != null) {

                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(product.Name);
                        if ($(parent).attr("id") == "rawitem-container") {
                            $this.GetAvgRate(product.AccountId, tr);
                            $this.GetStockPreviousBalance(tr, product.AccountId);

                        }

                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = "<li>" + $(this).val() + " is not valid code.</li>";
                            Common.ShowError(err);
                            $(this).focus();
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();
                var parent = $(this).closest("div.parent-conatiner");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find("input.Quantity").val("1");
                    $(tr).find("input.Quantity").focus();

                }

            });
            $(document).on("keyup", ".Quantity,.Rate,#item-container .Amount,#finishedgoods-item-container .Amount", function (event) {

                var tr = $(this).parent().parent();
                var parent = $(this).closest("div.parent-conatiner");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
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


            $(document).on("keyup", "#expense-container .Amount", function (event) {
                var tr = $(this).parent().parent();
                var parent = $(this).closest("div.parent-conatiner");
                var amount = Common.GetInt($(tr).find("input.Amount").val());
                var code = $(tr).find("input.Code").val();
                $this.GetExpensesTotal();
                if (event.which == 13 && amount > 0)
                    $this.AddItem(parent);
                else if (event.which == 13 && amount <= 0) {
                    var err = "<li>Item " + code + " must have amount greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Amount").focus();

                }


            });

            $(document).on("click", "#OrderTable > tbody tr", function () {
                $this.SelectOrder(this);
            });

            $(document).on("change", "#ProductId", function (event) {
                $this.GetReceipe()
            });


            this.LoadPageSetting();
            $this.GetCustomerProducts();
            $this.LoadAccountDetail();
            $this.AutoCompleteInit(true);
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
                this.ListView();
            } else {
                this.ListView();
            }


        },

        ResetMachine: function () {
            $(`#MachineId`).val(null);
            const locId = $("#LocationId").val();
            document.querySelectorAll('#MachineId option')?.forEach(el => {
                el.style.display = 'none';
                if (el.getAttribute('data-location') == locId) {
                    el.style.display = 'block';
                }
            });
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        New: function () {
            $("#Quantity").prop("disabled", false);
            $("#MachineId").prop("disabled", false);
            $("#LocationId").prop("disabled", false);
            var $this = this;
            SetFocus = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
        },

        RebindData: function () {
            DataTable.DestroyDatatable(DATATABLE_ID);
            DataTable.RefreshDatatable(DATATABLE_ID);
        },

        GetByCode: function (code) {
            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },

        DetailView: function () {
            //$("#form-info").removeClass("hide");
            //$("#div-table").addClass("hide");
        },
        GetProductDetail: function () {
            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
            $this.GetNextVoucherNumber();

        },
        ListView: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            LIST_LOADED = true;
            DataTable.DestroyDatatable(DATATABLE_ID);
            DataTable.BindDatatable(DATATABLE_ID, url);
        },
        AddItem: function (parent) {

            var $this = this;
            var isRaw = false;

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



            var html = "";
            if ($(parent).attr("id") == "rawitem-container")
                html = $("#template-item").html();
            else if ($(parent).attr("id") == "finisheditem-container")
                html = $("#template-item-finishedgoods").html();
            else
                html = $("#template-item-expenses").html();


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
            var date = Common.GetTodayDate(2);
            SetFocus = "code";
            $("#Item").focus();
            $("table tbody tr:nth-last-child(1) input.date-picker", parent).val(date);
            //}
            if ($(parent).attr("id") == "expensesitem-container")
                $this.ExpensesCompleteInit();
            else
                $this.AutoCompleteInit();
            //Common.InitNumerics();
            Common.InitDateMask();
            Common.InitDatePicker();

        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            var parent = $(elment).closest(".parent-conatiner");
            var isRaw = false;
            if ($(parent).attr("id") == "rawitem-container")
                isRaw = true;

            $(elment).closest("tr").remove();
            $this.GetWholeTotal(parent, isRaw);
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
        Save: function (status) {
            var $this = this;
            $this.SaveRecord(status, function () {
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
        SaveRecord: function (status, callback) {

            var $this = this;

            var mode = "add";
            var voucher = "production";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var masterProduct = Common.GetById(Common.GetInt($("#ProductId").val()));
            record.ItemId = masterProduct.Id;
            record.ItemCode = masterProduct.AccountCode;
            record.ItemName = masterProduct.Name;
            record.Status = status;
            if (status == 2 && !record.StockWarehouseId) {
                Common.ShowError("Please selection warehouse");
                return
            }
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Items.concat(Common.SaveItemData("#finishedgoods-item-container"));
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0 && item.EntryType != EntryType.ServiceExpense) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += "<li>" + item.ItemCode + " is not valid code.</li>";
                    }

                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }


                record["Items"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving production...please wait",
                    success: function (res) {
                        if (res.Success) {
                            if (status == 3) {
                                Common.LoadProductStockWarehouseAndLocationWise();
                            }
                            callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                        $this.ListView();
                    },
                    error: function (e) {
                    }
                });
            }
        },
        calculateQty: function () {
            const mainQty = Number($("#Quantity").val()) || 0;
            const finishQty = Number($("#finisheditem-container .Quantity")?.val()) || 0;

            // Get all rows from the table
            const rows = $("#item-container tbody tr");

            rows.each(function () {
                const $row = $(this);

                // Retrieve quantity values from the row
                const qty = Number($row.find('.Quantity').val()) || 0;
                const availQty = Number($row.find('.AvailQty').val()) || 0;
                const consQty = mainQty > 0 ? (qty / mainQty) * finishQty : 0;

                // Set ConsumeQty value
                $row.find('.ConsumeQty').val(consQty);

                // Set WasteQty value (default to 0 if not a valid number)
                const wasteQty = Number($row.find('.WasteQty').val()) || 0;
                $row.find('.WasteQty').val(wasteQty);

                // Calculate and set RemainingQty
                const remainingQty = availQty - (wasteQty + consQty);
                $row.find('.RemainingQty').val(remainingQty);
            });
            $this.GetWholeTotal("#finisheditem-container", false);
        },

        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var parent = $(tr).closest("#rawitem-container");
            var isRaw = false;
            if (parent.length > 0)
                isRaw = true;
            else
                parent = $("#finisheditem-container");
            var Quantity = 0.00;
            var Rate = 0.00;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(amount);

            var discount = Common.GetInt($(tr).find("input.DiscountPercent").val());
            var discountAmount = Common.GetInt(amount) * discount / 100;
            var netAmount = Common.GetInt(amount) - discountAmount;
            $(tr).find("input.DiscountAmount ").val(discountAmount);
            $(tr).find("input.NetAmount").val(netAmount);

            $this.GetWholeTotal(parent, isRaw);
        },
        GetExpensesTotal: function (tr) {
            var $this = this;

            var amount = 0;

            $("#expense-container tbody tr").each(function () {
                amount += Common.GetInt($(this).find("input.Amount").val());
            });
            $("#expense-container tfoot tr").find("input.Amount").val(amount);
            $this.GetNetTotal();

        },
        GetWholeTotal: function (parent, israw) {

            var $this = this;
            var Quantity = 0;
            var Price = 0;
            if (!israw) {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                    Price += Common.GetFloat($(this).find("input.Amount").val());

                });
                Price = Common.GetInt(Price);
                $("table tfoot tr", parent).find("input.Quantity").val(Quantity);
                $("table tfoot tr", parent).find("input.Amount").val(Price);
                $("#FinishedQuantityTotal").val(Quantity);
                $("#FinishedNetTotal,#TotalReceived").val(Price);
            }
            else {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                    Price += Common.GetFloat($(this).find("input.Amount").val());
                });
                Price = Common.GetInt(Price);
                $("table tfoot tr", parent).find("input.Quantity").val(Quantity);
                $("table tfoot tr", parent).find("input.Amount").val(Price);
                $("#QuantityTotal").val(Quantity);
                $("#NetTotal,#TotalIssue").val(Price);

            }
            $this.GetNetTotal();
        },
        GetNetTotal: function () {
            var totalIssue = Common.GetInt($("#NetTotal").val());
            var totalreceived = Common.GetInt($("#FinishedNetTotal").val());
            var totalcharges = Common.GetInt($("#expense-container tfoot tr").find("input.Amount").val());
            $("#TotalCharges").val(totalcharges);
            var difference = totalIssue + totalcharges - totalreceived;


            $("#Difference").val(difference);
        },
        CustomClear: function () {

            $("#item-container tbody").html("");
            $("#finishedgoods-item-container tbody").html("");
            $("#expense-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            Common.Clear();
        },
        GetStockPreviousBalance: function (row, itemid) {
            return;
            var $this = this;
            var type = $this.GetType();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetStockPreviousBalance&accountid=" + itemid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $(row).find("input.Quantity ").val(res.Data);
                        $this.GetQuantityPriceTotal(row);
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        GetAvgRate: function (itemId, tr) {
            return;
            var tableId = "#" + $(tr).closest(".parent-conatiner").attr("id");;
            var $this = this;
            if (itemId <= 0)
                return;
            $.ajax({
                url: Setting.APIBaseUrl + "Misc" + "/?Key=GetAvgRate&&itemID=" + itemId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: false,
                blockElement: "#form-info",
                blockMessage: "Loading rates...please wait",
                success: function (res) {
                    if (res.Success) {
                        $(tr).find("input.Rate").val(res.Data.Rate);
                        $this.GetQuantityPriceTotal(tr);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
                                    var product = Common.GetByCode(item.ItemCode);
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
                                    var product = Common.GetByCode(item.ItemCode);
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
        Edit: function (voucherNo) {
            var $this = this;
            $("#VoucherNumber").val(voucherNo);
            $this.LoadVoucher("same");
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var voucher = "production";
            const locationId = Common.GetInt($("#LocationId").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VOUCHER_TYPE + "&key=" + key + "&voucher=" + voucherno + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading prodcution...please wait",
                success: function (res) {
                    if (res.Success) {
                        stockRequired = false;
                        var d = res.Data.Order;
                        record = d;
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $("#OrderNo").prop("disabled", false);
                            $("#SearchIconOrder").removeClass("hide");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            const existingLocationId = $('#LocationId').val();
                            $this.CustomClear();
                            Common.MapEditData(d, "#form-info");
                            $('#LocationId').val(d?.LocationId || existingLocationId)
                            $this.ResetMachine()
                            if (d?.MachineId) {
                                $('#MachineId').val(d.MachineId)
                            }
                            $this.ResetWarehouse();
                            if (d?.StockWarehouseId) {
                                $('#StockWarehouseId').val(d.StockWarehouseId)
                            }
                            $("#Quantity").prop("disabled", true);
                            $("#MachineId").prop("disabled", true);
                            $("#LocationId").prop("disabled", true);
                            $("#ProductId").select2("val", d.ItemId);
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.Items != null && d.Items.length > 0) {
                                $("#OrderNo").prop("disabled", true);
                                if (res.Data?.OrderInfo != null) {
                                    Common.SetDate("#OrderDate", res.Data.OrderInfo.OrderDate);
                                }
                                $("#SearchIconOrder").addClass("hide");
                                var items = d.Items;
                                var rawitems = $.grep(items, function (e) {
                                    return e.Type == 1;
                                });
                                const stockData = Common.ProductStockWarehouseAndLocationWise();

                                for (let i in rawitems) {
                                    const item = rawitems[i];
                                    var stock = stockData.find(e => e.LocationId === d.LocationId && e.MachineId === d.MachineId && item.ItemId == e.ItemId);
                                    item.AvailQty = stock?.Quantity || 0;
                                    item.Balance = item.AvailQty - item.Quantity;
                                    if (item.Balance < 0 && !stockRequired) {
                                        stockRequired = true;
                                    }
                                    if (d.Status === 1) {
                                        item.IssueQty = Math.abs(item.Balance)
                                    }
                                }
                                Common.MapItemData(rawitems, null, null, true);
                                var finisheditems = $.grep(items, function (e) {
                                    return e.Type == 2;
                                });
                                Common.MapItemData(finisheditems, "#finishedgoods-item-container", "#template-item-finishedgoods", true);
                                const totalFinishQty = finisheditems.map(x => x.Quantity).reduce((accumulator, current) => accumulator + current)
                                $('#FinishedQuantityTotal').val(totalFinishQty)
                                var expenses = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.ServiceExpense;
                                });
                                Common.MapItemData(expenses, "#expense-container", "#template-item-expenses", true);
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

                        SetFocus = "booknumber";
                        $("#item-container .Amount,#finishedgoods-item-container .Amount").trigger("keyup");
                        $this.GetExpensesTotal();
                        $this.GetNetTotal();
                        $this.LoadPrintData(res);
                        $("#Quantity").trigger("keyup");
                        $this.ManageButton();
                        $this.calculateQty();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        GetReceipe: function () {
            var $this = this;
            var productId = Common.GetInt($("#ProductId").val());
            var qty = Common.GetInt($("#Quantity").val());
            if (qty <= 0) {
                Common.ShowError("Badge quantity must be greater than zero (0).");
                return;
            }
            if (productId > 0) {
                $("#Quantity").prop("disabled", true);
                $("#LocationId").prop("disabled", true);
                $("#MachineId").prop("disabled", true);
            }
            else {
                $("#Quantity").prop("disabled", false);
                $("#item-container tbody").html("");
                $("#finishedgoods-item-container tbody").html("");
                $this.GetWholeTotal("#rawitem-container", true);
                $this.GetWholeTotal("#finisheditem-container", false);
                return;
            }

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?productId=" + productId + "&loadReceipe=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $("#finishedgoods-item-container tbody").html("");
                        stockRequired = false;
                        var d = res.Data.Receipe;
                        var rawItems = d?.ProductReceipeitems || [];
                        var badgeProductRate = res.Data.BadgeProductRate;
                        var finishItems = new Array();
                        var masterProduct = Common.GetById(Common.GetInt($("#ProductId").val()));

                        var receipeQty = d?.Quantity || 0;
                        var totalAmount = 0;
                        const stockData = Common.ProductStockWarehouseAndLocationWise();
                        const locationId = Common.GetInt($("#LocationId").val());
                        const machineId = Common.GetInt($("#MachineId").val());

                        for (var i in rawItems) {
                            var item = rawItems[i];

                            var product = Common.GetAccountDetailByAccountId(item.ItemId);
                            var stock = stockData.find(e => e.LocationId === locationId && e.MachineId === machineId && product.AccountId == e.ItemId);
                            item.Rate = product.PurchasePrice;
                            item.Quantity = ((item.Quantity / d.Quantity) * Common.GetFloat($("#Quantity").val())).toFixed(3);
                            item.AvailQty = stock?.Quantity || 0;
                            item.Balance = item.AvailQty - item.Quantity;
                            totalAmount += item.Quantity * item.Rate;
                            item.Type = 1; // Raw Items
                            if (item.Balance < 0 && !stockRequired) {
                                stockRequired = true;
                            }
                        }
                        var avgFinsihedRate = (totalAmount / Common.GetFloat($("#Quantity").val())).toFixed(3);
                        finishItems.push(
                            {
                                Id: 0,
                                ItemId: masterProduct.Id,
                                ItemCode: masterProduct.AccountCode,
                                ItemName: masterProduct.Name,
                                totalAmount: totalAmount, 
                                Rate: avgFinsihedRate,
                                Type: 2, // Finish Items
                                Quantity: Common.GetFloat($("#Quantity").val())
                            });
                        Common.MapItemData(rawItems, null, null, true);
                        Common.MapItemData(finishItems, "#finishedgoods-item-container", "#template-item-finishedgoods", true);

                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                        }, 500);
                        $(document).find("table input.Quantity").trigger("keyup");
                        $this.GetWholeTotal();
                        $this.ManageButton();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        LoadPrintData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            if (d == null)
                return;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblComments").html(res.Data.Comments);
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


            if (d.Id > 0 && d.Items != null && d.Items.length > 0) {
                var html = "";
                $("#OrderNo").prop("disabled", true);
                if (res.Data?.OrderInfo != null) {
                    Common.SetDate("#OrderDate", res.Data.OrderInfo.OrderDate);
                }
                $("#SearchIconOrder").addClass("hide");
                var items = d.Items;
                var rawitems = $.grep(items, function (e) {
                    return e.EntryType == EntryType.RawMaterial;
                });

                $("#report-issued-item tbody").html("");
                for (var i in rawitems) {
                    var item = rawitems[i];
                    html += "<tr>";
                    html += "<td>" + item.ItemCode + "</td>";
                    html += "<td>" + item.ItemName + "</td>";
                    html += "<td>" + Common.FormatDate(item.Date, "DD-MM-YYYY") + "</td>";
                    html += "<td class='align-right'>" + item.Quantity + "</td>";
                    html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                    html += "<td class='align-right'>" + item.Amount.format() + "</td>";
                    html += "</tr>";
                }
                html += "<tr class='bold'>";
                html += "<td colspan='3' class='align-right'>Total</td>";
                html += "<td class='align-right'>" + d.QuantityTotal + "</td>";
                html += "<td colspan='2' class='align-right'>" + d?.NetTotal?.format() + "</td>";
                html += "</tr>";
                $("#report-issued-item tbody").append(html);

                html = "";
                var finisheditems = $.grep(items, function (e) {
                    return e.EntryType == EntryType.FinishedGoods;
                });

                $("#report-finished-item tbody").html("");
                for (var i in finisheditems) {
                    var item = finisheditems[i];
                    html += "<tr>";
                    html += "<td>" + item.ItemCode + "</td>";
                    html += "<td>" + item.ItemName + "</td>";
                    html += "<td>" + Common.FormatDate(item.Date, "DD-MM-YYYY") + "</td>";
                    html += "<td class='align-right'>" + item.Quantity + "</td>";
                    html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                    html += "<td class='align-right'>" + item.Amount.format() + "</td>";
                    html += "</tr>";
                }
                html += "<tr class='bold'>";
                html += "<td colspan='3' class='align-right'>Total</td>";
                html += "<td class='align-right'>" + d.FinishedQuantityTotal + "</td>";
                html += "<td colspan='2' class='align-right'>" + d?.FinishedNetTotal?.format() + "</td>";
                html += "</tr>";

                $("#report-finished-item tbody").append(html);



                var expenses = $.grep(items, function (e) {
                    return e.EntryType == EntryType.ServiceExpense;
                });
                html = "";
                var expensesTotal = 0;
                $("#report-expenses-item tbody").html("");
                for (var i in expenses) {
                    var item = expenses[i];
                    expensesTotal += item.Amount;
                    html += "<tr>";
                    html += "<td>" + item.ItemCode + "</td>";
                    html += "<td>" + item.ItemName + "</td>";
                    html += "<td>" + Common.FormatDate(item.Date, "DD-MM-YYYY") + "</td>";
                    html += "<td>" + (Common.isNullOrWhiteSpace(item.Comments) ? "" : item.Comments) + "</td>";
                    html += "<td class='align-right'>" + item.Amount.format() + "</td>";
                    html += "</tr>";
                }

                html += "<tr class='bold'>";
                html += "<td colspan='4' class='align-right'>Total</td>";
                html += "<td class='align-right'>" + expensesTotal.format() + "</td>";
                html += "</tr>";

                $("#report-expenses-item tbody").append(html);



                var difference = d.NetTotal + expensesTotal - d.FinishedNetTotal;
                $("#lblexpensesTotal").html(expensesTotal.format());
                $("#lbldifference").html(difference.format());
                $("#lblQuantityDifference").html((d.QuantityTotal - d.FinishedQuantityTotal).format());
            }

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
        AutoCompleteInit: function (bindProduct) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                product.Name = product.AccountCode + "-" + product.DisplayName;
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            if (bindProduct)
                Common.BindSelect(products, $("#ProductId"), true)


            $("#item-container .Code,#finishedgoods-item-container .Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = Common.GetAccountDetailByCode(ui.item.value);

                    var tr = $(this).closest("tr");
                    if (typeof product != "undefined" && product != null) {

                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(product.Name);
                        setTimeout(function () {
                            $(tr).find("input.Quantity").focus();
                        }, 500);
                        var parent = $(this).closest("div.parent-conatiner");
                        if ($(parent).attr("id") == "rawitem-container") {
                            $this.GetAvgRate(product.AccountId, tr);
                            $this.GetStockPreviousBalance(tr, product.AccountId);

                        }

                    }

                }
            });

        },
        ExpensesCompleteInit: function (partyid) {
            var $this = this;


            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            expenses = Common.GetAllLeafAccounts(exids);

            var suggestion = new Array();
            for (var i in expenses) {
                var expense = expenses[i];
                suggestion.push(
                    {
                        id: expense.Id,
                        value: expense.AccountCode,
                        label: expense.AccountCode + "-" + expense.DisplayName,
                        name: expense.DisplayName

                    }
                );
            }

            $("#expense-container .Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ItemId").val(ui.item.id);
                    $(tr).find("input.Name").val(ui.item.name);
                    setTimeout(function () {
                        $(tr).find("input.Comments").focus();
                    }, 200);



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
        ResetWarehouse: function () {
            $(`#StockWarehouseId`).val(null);
            const locId = $("#LocationId").val();
            document.querySelectorAll('#StockWarehouseId option')?.forEach(el => {
                const loc = el.getAttribute('data-location');
                if (loc) {
                   el.style.display = loc == locId ? 'block' : 'none';
                }
            });
        },
        ManageButton: function () {
            $(".IssueQtyCol").addClass("hide");
            $(".ConsumeQtyCol").addClass("hide");
            $(".WasteQtyCol").addClass("hide");
            $(".RemainingQtyCol").addClass("hide");

            $("#reqStock").addClass("hide");
            $("#issueStock").addClass("hide");
            $("#receiveStock").addClass("hide");
            $("#startProduction").addClass("hide");
            $("#complete").addClass("hide");
            $("#save").addClass("hide");
            $("#StockWarehouseDiv").addClass("hide");
            $(".IssueQty").prop("disabled", true);
            $(".WasteQty").prop("disabled", true);
            $("#RawStockWarehouseDiv").addClass("hide");
            $("#FinishStockWarehouseDiv").addClass("hide");

            if (!record) {
                $("#save").removeClass("hide");
            }
            if (record?.Items?.length) {
                switch (record.Status) {
                    case 0:
                        $("#save").removeClass("hide");
                        break;
                    case 1:
                        $("#issueStock").removeClass("hide");
                        $("#StockWarehouseDiv").removeClass("hide");
                        $(".IssueQtyCol").removeClass("hide");
                        $(".IssueQty").prop("disabled", false);
                        break;
                    case 2:
                        $("#receiveStock").removeClass("hide");
                        break;
                    case 4:
                        $("#complete").removeClass("hide");
                        $(".ConsumeQtyCol").removeClass("hide");
                        $(".WasteQtyCol").removeClass("hide");
                        $(".RemainingQtyCol").removeClass("hide");
                        $(".WasteQty").prop("disabled", false);
                        $("#RawStockWarehouseDiv").removeClass("hide");
                        $("#FinishStockWarehouseDiv").removeClass("hide");
                        break;
                    case 5:
                        $(".ConsumeQtyCol").removeClass("hide");
                        $(".WasteQtyCol").removeClass("hide");
                        $(".RemainingQtyCol").removeClass("hide");
                        break;
                    default:
                        break;
                }
                if (record.Status > 1) {
                    $(".IssueQtyCol").removeClass("hide");
                }
                if (stockRequired && record.Status == 0) {
                    $("#reqStock").removeClass("hide");
                }
                if (record.Status < 4) {
                    $("#startProduction").removeClass("hide");
                }
            }
        }

    };
}();
