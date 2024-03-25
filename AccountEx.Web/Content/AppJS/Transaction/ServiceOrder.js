
var ServiceOrder = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "ServiceOrder";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#AccountCode";
    return {
        init: function () {
            var $this = this;
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "booknumber" : "date";
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
            $("#AccountCode").keyup(function (e) {
                if (e.which == 13) {

                    var party = Common.GetByCode($(this).val());

                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        $("#SalesmanCode").focus();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid party code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $("#ServicesRequired").keydown(function (e) {
                if (e.which == 13) {
                    $("#EmployeeCode").focus();

                }
            });
            $("#EstimatedTime").keypress(function (e) {
                if (e.which == 13) {
                    setTimeout(function () {
                        $('#BehaviourLevel').select2('focus');
                    }, 500);

                }
            });
            $("#DelayTime").change(function () {
                $("#Problem").focus();
            });
            $("#BehaviourLevel").change(function () {

                $('#ServicesLevel').select2('focus');

            });
            $("#ServicesLevel").change(function () {
                setTimeout(function () {
                    $('#DelayTime').select2('focus');
                }, 500);



            });


            $("#Comments").keypress(function (e) {
                $this.MapComments();
                if (e.which == 13) {
                    setTimeout(function () {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                    }, 300);

                }
            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    var address = party.Address;

                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid party code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            $("#EmployeeCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $(".container-message").hide();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid service representative code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }

            });
            $("#MachineCode").blur(function () {
                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $(".container-message").hide();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid equipment code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }

            });
            $("#MachineCode").keyup(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());

                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                        $("#ContactPerson").focus();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid equipment code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }

                }
            });
            $("#CurrentReading").keyup(function () {
                $this.CalculateConsumption();
            });
            $("#EmployeeCode").keyup(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());

                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                        $("#InitialObservation").focus();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid equipment code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }

                }
            });

            $(document).on("blur", "#tbl-order-expenses .Quantity,#tbl-order-expenses .GSTPercent", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (qty <= 0) {
                    $(tr).find("input.Quantity").val("1");

                }
                $this.GetExpensesQuantityPriceTotal(tr);

            });
            $(document).on("keyup", "#tbl-order-expenses .GSTPercent", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetExpensesQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0) {
                    if (rate > 0)
                        $this.AddExpenseItem();

                }
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $(document).on("keyup", "#tbl-order-expenses .Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetExpensesQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0) {
                    if (rate > 0)
                        $this.AddExpenseItem();
                    else
                        $(tr).find("input.PurchasePrice").focus();
                }
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $(document).on("keyup", "#tbl-order-expenses .PurchasePrice", function (event) {
                var tr = $(this).closest("tr");
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetExpensesQuantityPriceTotal(tr);
                if (event.which == 13 && rate > 0) {
                    $this.AddExpenseItem();
                }
                else if (event.which == 13 && rate <= 0) {
                    var err = "Item " + code + " must have rate greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Rate").focus();

                }


            });

            $(document).on("keyup", "#tbl-order-services .Amount", function (event) {
                $this.GetServicesNetTotal();

            });
            $(document).on("keyup", "#tbl-order-services .Amount,#tbl-order-services .ItemCode", function (event) {
                var tr = $(this).closest("tr");
                var amount = Common.GetInt($(tr).find("input.Amount").val());
                var code = $(tr).find("input.ItemCode").val();
                if (event.which == 13 && amount > 0) {

                    $this.AddServiceItem();

                }
                else if (event.which == 13 && amount <= 0) {
                    var err = "Service " + code + " must have amount greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find("input.Amount").focus();

                }

            });
            $(document).on("keyup", ".Date", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {

                    }
                    else {
                        $("#tbl-order-expenses tbody tr:nth-last-child(1)").find("input.Quantity").focus().select();
                    }

                }
            });
            $(document).on("keyup", ".PurchasePrice", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {

                    }
                    else {
                        $("#tbl-order-expenses tbody tr:nth-last-child(1)").find("input.Rate").focus().select();
                    }

                }
            });

            $("#SalesmanCode").keyup(function (e) {
                if (e.which == 13) {

                    var salesman = Common.GetByCode($(this).val());

                    if (typeof salesman != "undefined" && salesman != null) {
                        $("#MachineCode").focus();
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid salesman code.";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $("#SalesmanCode").blur(function () {
                var salesman = Common.GetByCode($(this).val());
                if (typeof salesman != "undefined" && salesman != null) {
                    //$("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid salesman code.";
                        $(this).focus();
                        Common.ShowError(err);
                    }
                }
            });
            //product table events

            $(document).on("keyup", ".ProductCode", function (event) {

                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $("#item-container tbody tr:nth-last-child(1)").find("input.ProductQuantity").focus().select();
                    }

                }
            });
            $(document).on("blur", ".ProductCode", function () {
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {
                    var product = Common.GetAccountDetailByAccountId(account.Id);
                    $(tr).find("input.ProductId").val(product.AccountId);
                    $(tr).find("input.Name").val(account.Name);
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });

            /////Starts/////

            $(document).on("keyup", "input.PurchaseRate", function () {
                var tr = $(this).closest("tr");
                var Quantity = $(tr).find("input.ProductQuantity").val();
                var Rate = $(tr).find("input.PurchaseRate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.ProductAmount").val(Amount.toFixed(2));
                $this.GetMaterialConsumedTotal();
            });
            /////Ends/////

            $(document).on("blur", ".ProductQuantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.ProductQuantity ").val());
                var code = $(tr).find("input.ProductCode").val();

                if (qty <= 0) {
                    $(tr).find("input.ProductQuantity").val("1");
                }
                $this.GetMaterialConsumedTotal();

            });
            $(document).on("keyup", ".ProductQuantity, .PurchaseRate,.ProductAmount", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.ProductQuantity").val());
                var code = $(tr).find("input.ProductCode").val();
                $this.GetMaterialConsumedTotal();
                if (event.which == 13 && qty > 0)
                    $this.AddItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.ProductQuantity").focus();

                }
                $this.GetMaterialConsumedTotal();

            });
            this.LoadPageSetting();
            $this.ExpensesAutoCompleteInit();
            $this.ListView();
            $("#TransactionType").change(function () {
                var ordertype = $(this).val();
                $("#AccountCode,#AccountId,#AccountName,#PartyAddress,#MachineCode,#MachineId,#MachineName").val("");
                $this.BindAutoComplete(ordertype);
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            AppData.AccountDetail = PageSetting.AccountDetails.concat(PageSetting.EquipmentDetails);
            $this.EmployeeAutoCompleteInit();
            $this.SalesmanAutoCompleteInit();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            $("#btnSearch").click(function () {
                $this.Filter();
            });
            $("#TransactionType").trigger('change');
            //else {
            //    $this.GetNextVoucherNumber();
            //}



        },
        ListView: function () {

            var $this = this;
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        Filter: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            url += "?type=" + VoucherType[$this.GetType()] + Common.MakeQueryStringAll($("#Job-Filters"));
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
        },
        BindAutoComplete: function (ordertype) {
            var $this = this;
            if (ordertype == VoucherType.customerserviceorder) {
                $(".customer-feild,.customer-site-feild").removeClass('hide');
                $(".master-account-label").text("Customer")
                Common.UpdateRequired("#SalesmanCode", true);
                Common.UpdateRequired("#AccountCode", true);
                $this.LoadAccounts(PageSetting.Customers);
                $this.EquipmentAutoCompleteInit(PageSetting.ExternalEquipment);
            }
            else if (ordertype == VoucherType.siteserviceorder) {
                $(".master-account-label").text("Sites")
                $(".customer-feild").addClass('hide');
                $(".customer-site-feild").removeClass('hide');
                Common.UpdateRequired("#SalesmanCode", false);
                Common.UpdateRequired("#AccountCode", true);
                $this.LoadAccounts(PageSetting.Sites);
                $this.EquipmentAutoCompleteInit(PageSetting.SiteEquipment);

            }
            else if (ordertype == VoucherType.repairingserviceorder) {
                Common.UpdateRequired("#AccountCode", false);
                Common.UpdateRequired("#SalesmanCode", false);
                $(".customer-feild,.customer-site-feild").addClass('hide');
                $this.EquipmentAutoCompleteInit(PageSetting.InHouseEquipment);
            }
        },
        AddExpenseItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#tbl-order-expenses tbody tr:nth-last-child(1) td:nth-child(1) input.EmployeeCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-order-expenses tbody tr:nth-last-child(1) td:nth-child(1) input.EmployeeCode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#order-expenses-template").html();
            $("#tbl-order-expenses tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#tbl-order-expenses tbody tr:nth-last-child(1) input.EmployeeCode").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.EmployeeAutoCompleteForServicesInit();
            $this.ExpensesAutoCompleteInit();
            Common.InitNumerics();
            Common.InitDateMask();
            Common.InitDatePicker();
        },
        AddServiceItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#tbl-order-services tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-order-services tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#order-services-template").html();
            $("#tbl-order-services tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#tbl-order-services tbody tr:nth-last-child(1) input.ItemCode").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.ServicesAutoCompleteInit();
            Common.InitNumerics();
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        CalculateConsumption: function () {
            var previousreading = Common.GetFloat($("#PreviousReading").val());
            var currentreading = Common.GetFloat($("#CurrentReading").val());
            var consumption = currentreading - previousreading;
            if (consumption >= 0)
                $("#Consumption").val(consumption);
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            focusElement = "#Date";
            $(".customer-feild,.row-servicesdelivered").removeClass('hide');
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            //Item wil be loaded for material requistion and GIN



            //var $this = this;
            //var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").val();
            //if (typeof code != "undefined" && code.trim() == "") {
            //    setTimeout(function () {
            //        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();
            //    }, 300);

            //    focusElement = "";
            //    return;
            //}
            //var html = $("#template-item").html();
            //$("#item-container tbody").append(html);
            //setTimeout(function () {
            //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();
            //}, 300);
            //$this.ProductAutoCompleteInit();
            //Common.InitNumerics();
        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            $this.GetMaterialConsumedTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
            if ($("#tbl-order-expenses tbody").children().length <= 0)
                $this.AddServiceItem();
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
                focusElement = "#AccountCode";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.ListView();

            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                var scope = $("#form-info-item");
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
            record.BehaviourLevel = BehaviourLevel[record.BehaviourLevel];
            record.ServicesLevel = BehaviourLevel[record.ServicesLevel];
            var MaterialItems = new Array();
            var ServicesItems = new Array();
            var OrderExpenseItems = new Array();


            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                var err = "";

                var transactiontype = $("#TransactionType").val();
                // if (transactiontype == VoucherType.customerserviceorder) {
                OrderExpenseItems = Common.SaveItemData("#tbl-order-expenses");
                OrderExpenseItems = Enumerable.From(OrderExpenseItems).Where("$.EmployeeCode.trim()!=''").ToArray();
                for (var i in OrderExpenseItems) {
                    var item = OrderExpenseItems[i];
                    if (item.Quantity <= 0) {
                        err += "" + item.EmployeeCode + "(" + item.EmployeeName + ") must have quantity greater than zero(0).";
                    }

                    var service = Common.GetByCode(item.EmployeeCode);
                    if (typeof service == "undefined" || service == null) {
                        err += item.EmployeeCode + " is not valid account code.";
                    }
                    var expense = Common.GetByCode(item.ExpenseCode);
                    if (typeof expense == "undefined" || expense == null) {
                        err += expense.ExpenseCode + " is not valid expense code.";
                    }

                }
                //if (ServicesItems.length <= 0) {
                //    err += "Please add atleast one item in services delivered";
                //}
                record["OrderExpenseItems"] = OrderExpenseItems;
                //     }
                MaterialItems = Common.SaveItemData();
                MaterialItems = Enumerable.From(MaterialItems).Where("$.ItemCode.trim()!=''").ToArray();

                var party = Common.GetByCode($("#AccountCode").val());
                var trantype = $("#TransactionType").val();
                if (trantype != 34)   // for reparing
                {
                    if (typeof party == "undefined" || party == null) {
                        err += $("#AccountCode").val() + " is not valid party code.";
                    }
                }


                for (var i in MaterialItems) {
                    var item = MaterialItems[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }
                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.";
                    }
                }
                ServicesItems = Common.SaveItemData("#tbl-order-services");
                ServicesItems = Enumerable.From(ServicesItems).Where("$.ItemCode.trim()!=''").ToArray();
                for (var i in ServicesItems) {
                    var item = ServicesItems[i];
                    if (item.Amount <= 0) {
                        err += "" + item.ItemCode + "(" + item.ItemName + ") must have amount greater than zero(0).";
                    }
                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid service code.";
                    }
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                Array.prototype.push.apply(MaterialItems, ServicesItems);
                record["OrderItems"] = MaterialItems;
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

        SaveTransaction: function () {
            var $this = this;
            var id = Common.GetInt($("#Id").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?orderid=" + id + "&key=finalize",
                type: "POST",
                data: id,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {

                        Common.ShowMessage(true, { message: "Record finalized successfully" });
                        $this.LoadVoucher("same");
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetMaterialConsumedTotal: function (tr) {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find("input.ProductQuantity").val());
                Price += Common.GetFloat($(this).find("input.ProductAmount").val());
            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity.toFixed(2));
            $("#item-container tfoot tr").find("input.Amount").val(Price.toFixed(2));
            $this.GetNetTotal();
        },

        GetExpensesQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.PurchasePrice").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(amount.toFixed(2));
            var gstpercent = Common.GetInt($(tr).find("input.GSTPercent").val());

            var gstAmount = Common.GetFloat(amount) * gstpercent / 100;
            var netAmount = Common.GetFloat(amount) + gstAmount;
            $(tr).find("input.GstAmount ").val(gstAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount.toFixed(2));

            $this.GetExpensesTotal();
        },
        GetExpensesTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var price = 0.0;
            var gstamount = 0.0;
            var netamount = 0.0;
            $("#tbl-order-expenses tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find("input.Quantity").val());
                price += Common.GetFloat($(this).find("input.Amount").val());
                gstamount += Common.GetFloat($(this).find("input.GstAmount").val());
                netamount += Common.GetInt($(this).find("input.NetAmount").val());
            });
            $("#tbl-order-expenses tfoot tr").find("input.Quantity").val(quantity.toFixed(2));
            $("#tbl-order-expenses tfoot tr").find("input.Amount").val(price.toFixed(2));
            $("#tbl-order-expenses tfoot tr").find("input.GstAmount").val(gstamount.toFixed(2));
            $("#tbl-order-expenses tfoot tr").find("input.NetAmount").val(netamount);

            $this.GetNetTotal();
        },
        GetServicesNetTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var price = 0.0;
            var gstamount = 0.0;
            var netamount = 0.0;
            $("#tbl-order-services tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find("input.Quantity").val());
                price += Common.GetFloat($(this).find("input.Amount").val());

            });
            $("#tbl-order-services tfoot tr").find("input.Quantity").val(quantity.toFixed(2));
            $("#tbl-order-services tfoot tr").find("input.Amount").val(price.toFixed(2));
            $this.GetNetTotal();
        },

        GetNetTotal: function () {
            var $this = this;



            var expenseTotal = Common.GetInt($("#tbl-order-expenses tfoot tr").find("input.Amount").val());
            var servicesTotal = Common.GetInt($("#tbl-order-services tfoot tr").find("input.Amount").val());
            var materialTotal = Common.GetInt($("#item-container tfoot tr").find("input.Amount").val());
            var netTotal = expenseTotal + servicesTotal + materialTotal;
            $("#ExpensesTotal").val(expenseTotal);
            $("#ServiceNetTotal").val(servicesTotal);
            $("#MaterialConsumedTotal").val(materialTotal);
            $("#NetTotal").val(netTotal);

        },
        CustomClear: function () {
            $.uniform.update();
            $("#item-container tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint,#btnfinal").prop("disabled", true);
            Common.Clear();
        },
        LoadReportData: function (res, orderitems, services) {
            var $this = this;
            var d = res.Data.Order;
            if (d == null)
                return;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            Common.MapDataWithPrefixF(d, "#div-workshopjobcard-report", "lbl", "html");
            Common.MapDataWithPrefixF(d, "#div-fieldservice-report", "lbl", "html");
            Common.MapDataWithPrefixF(d, "#div-services-invoice", "lblInv", "html");
            $("#lblDate,#lblInvoiceDate").html(moment(d.Date).format("DD-MM-YYYY"));
            $("#lblCurrentDate").html(moment(new Date).format("DD-MM-YYYY"))
            var jobtype = $("#TransactionType option[value='" + d.TransactionType + "']").text();
            $("#lblJobType").html(jobtype);
            var type = $this.GetType();
            var html = "";
            var items = d.OrderExpenseItems;
            var index = 1;
            $("#report-serviceitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ServiceItemName + "</td>";
                html += "<td>" + (item.PerformedBy != null ? item.PerformedBy : "") + "</td>";
                html += "<td>" + item.PurchasePrice.format() + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-serviceitem tbody").append(html);
            var serviceamountTotal = Common.GetInt(Enumerable.From(items).Sum("$.Amount"));
            $("#report-serviceitem tfoot tr").find("td:nth-child(2)").html(serviceamountTotal.format());

            //This is for Workshop job card print
            var html = "";
            var items = d.OrderExpenseItems;
            var index = 1;
            $("#report-serviceitemdelivered tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + moment(d.Date).format("DD-MM-YYYY") + "</td>";
                html += "<td>" + (item.PerformedBy != null ? item.PerformedBy : "") + "</td>";
                html += "<td>" + item.ServiceItemName + "</td>";
                html += "<td></td>";
                html += "</tr>";
            }
            $("#report-serviceitemdelivered tbody").append(html);


            html = "";
            index = 1;
            $("#report-saleitem tbody").html("");
            for (var i in orderitems) {
                var item = orderitems[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.DcNo + "</td>";
                html += "<td>" + moment(item.DcDate).format("DD-MM-YYYY") + "</td>";
                html += "<td>" + item.ItemCode + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.PurchaseRate.format() + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

            $("#report-services-Items tbody").html("");
            html = "";
            index = 1;
            for (var i in services) {
                var item = services[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td  class='align-right'>" + item.Amount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-services-Items tbody").append(html);
            var amountTotal = Common.GetInt(Enumerable.From(orderitems).Sum("$.Amount"));
            $("#report-saleitem tfoot tr").find("td:nth-child(2)").html(amountTotal.format());
            $("#totalAmount").html((amountTotal + serviceamountTotal).format());
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
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container,#tbl-order-services,#tbl-order-expenses").find("tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            $("#BehaviourLevel").select2('val', Common.GetKeyFromEnum(d.BehaviourLevel, BehaviourLevel));
                            $("#ServicesLevel").select2('val', Common.GetKeyFromEnum(d.ServicesLevel, BehaviourLevel));
                            $this.BindAutoComplete(d.TransactionType);
                            if (d.Id > 0 && d.OrderItems != null && d.OrderItems.length > 0) {
                                $("#btndelete,#btnprint,#btnfinal").prop("disabled", false);

                                $this.GetPreviousBalance(d.AccountId);



                            }
                            var materialConsumed = Enumerable.From(d.OrderItems).Where(function (p) { return p.EntryType == EntryType.Item }).ToArray();
                            var orderItemExtra = new Array();
                            var ginps = res.Data.GINP;
                            var avgRates = res.Data.AvgRate;
                            for (var k in ginps) {
                                var ginp = ginps[k];
                                var ginpItems = ginp.DCItems;
                                for (var r in ginpItems) {
                                    var ginpItem = ginpItems[r];

                                    var orderItem = Enumerable.From(materialConsumed).FirstOrDefault({ Id: 0, PurchaseRate: 0, Rate: 0, Amount: 0 }, function (p) { return p.DcId == ginp.DcId && p.ItemId == ginpItem.ItemId });
                                    var avgRate = Enumerable.From(avgRates).FirstOrDefault({ ItemId: 0, Rate: 0 }, function (p) { return p.ItemId == ginpItem.ItemId });
                                    var purchaseRate = Common.GetFloat(orderItem.PurchaseRate);
                                    var ad = Common.GetAccountDetailByAccountId(ginpItem.ItemId);
                                    if (purchaseRate == 0)
                                        purchaseRate = Common.GetFloat(avgRate.Rate);

                                    if (purchaseRate == 0)
                                        purchaseRate = Common.GetFloat(ad.PurchasePrice);

                                    orderItemExtra.push(
                                        {
                                            Id: orderItem.Id,
                                            ItemId: ginpItem.ItemId,
                                            ItemCode: ginpItem.ItemCode,
                                            ItemName: ginpItem.ItemName,
                                            Quantity: ginpItem.Quantity,
                                            PurchaseRate: purchaseRate,
                                            Rate: orderItem.Rate,
                                            Amount: Common.GetFloat(purchaseRate) * ginpItem.Quantity,
                                            DcId: ginp.DcId,
                                            DcNo: ginp.DcNo,
                                            Date: ginp.Date
                                        })
                                }
                            }

                            Common.MapItemData(orderItemExtra);
                            if (d.Id > 0 && d.OrderExpenseItems != null && d.OrderExpenseItems.length > 0) {
                                $("#btndelete,#btnprint,#btnfinal").prop("disabled", false);
                                Common.MapItemData(d.OrderExpenseItems, "#tbl-order-expenses", "#order-expenses-template");



                            }
                            var services = Enumerable.From(d.OrderItems).Where(function (p) { return p.EntryType == EntryType.Services }).ToArray();
                            Common.MapItemData(services, "#tbl-order-services", "#order-services-template");
                            if (d.Id > 0)
                                $("#btndelete").prop("disabled", false);

                            setTimeout(function () {
                                $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();
                            }, 300);

                        }
                        $this.LoadReportData(res, orderItemExtra, services);
                        $this.AddItem();

                        $this.AddExpenseItem();
                        $this.AddServiceItem();


                        $this.GetExpensesTotal();
                        $this.GetServicesNetTotal();
                        $this.GetMaterialConsumedTotal();
                        $this.GetNetTotal();
                        if (d == null) {
                            $this.SetFormControl(TransactionStatus.Pending);
                        }
                        else {
                            $this.SetFormControl(d.Status)
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        SetFormControl: function (status) {
            //var $tables = $("#tbl-order-expenses,#item-container");
            //if (status != TransactionStatus.Delivered) {

            //    $($tables).find("input").prop("disabled", false);
            //    $("#btnfinal").html("<i class='fa fa-check'></i> Finalize").removeClass("red").addClass("yellow");
            //    $("#btndelete,#btnsave").removeClass("hide");
            //    //$("#btnprint").addClass("hide");


            //}
            //else {

            //    $($tables).find("span.action").remove();
            //    $($tables).find("input").prop("disabled", true);
            //    $("#btnfinal").html("<i class='fa  fa-lock'></i> Un  Final").removeClass("yellow").addClass("red");
            //    $("#btndelete,#btnsave").addClass("hide");
            //    $("#btnprint").removeClass("hide");
            //}
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
            var transactiontype = $("#TransactionType").val();
            if (transactiontype == VoucherType.customerserviceorder)
                type = transactiontype;
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var id = Common.GetInt($("#Id").val());
                if (id <= 0) {
                    Common.ShowError("No Voucher found for deletion.");
                    return;
                }
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type + "&voucher=" + voucherno;
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
        LoadAccounts: function (id) {
            var $this = this;
            //var id = PageSetting.Customers;
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

            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var account = Common.GetByCode(ui.item.value);
                    var type = $this.GetType();
                    if (typeof account != "undefined" && account != null) {
                        $("#AccountName").val(account.Name);
                        $("#AccountId").val(account.Id);
                        Common.GetPartyAddress(account.Id);
                        $(".container-message").hide();
                    }
                    $this.MapComments();
                }
            });



        },
        EmployeeAutoCompleteForServicesInit: function (id) {
            var $this = this;
            var id = PageSetting.ExpensesHeadId;
            var tokens = Common.GetAllLeafAccounts();
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

            $(".EmployeeCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {


                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {

                        $(tr).find("input.EmployeeId").val(account.Id);
                        $(tr).find("input.EmployeeName").val(account.Name);

                    }


                }
            });



        },
        ExpensesAutoCompleteInit: function (id) {
            var $this = this;
            var id = PageSetting.ExpensesHeadId;
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

            $(".ExpenseCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {


                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {

                        $(tr).find("input.ExpenseId").val(account.Id);
                        $(tr).find("input.ExpenseName").val(account.Name);

                    }


                }
            });



        },
        EmployeeAutoCompleteInit: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Employee;
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

            $("#EmployeeCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        $("#EmployeeName").val(account.Name);
                        $("#EmployeeId").val(account.Id);

                        $(".container-message").hide();
                    }
                }
            });



        },
        ProductAutoCompleteInit: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Products;
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

            $(".ProductCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ProductId").val(product.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.ProductRate").val(product.Rate);
                        $(".container-message").hide();
                    }
                }
            });



        },
        LoadPageSetting: function () {
            var $this = this;
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            //$this.LoadAccounts(PageSetting.Customers);
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },

        Edit: function (id, element) {
            var $this = this;
            var voucherNo = Common.GetInt($(element).closest("tr").find("td:nth-child(1)").text());
            $("#VoucherNumber").val(voucherNo)
            $this.LoadVoucher("same");
        },
        Print: function (element) {
            $("#service-print-conatiner > div").addClass("hide").removeClass("visible-print");
            $("#service-print-conatiner > div#" + element).removeClass("hide").addClass("visible-print");
            window.print();
        },
        ServicesAutoCompleteInit: function (partyid) {
            var $this = this;
            var services = Common.GetLeafAccounts(PageSetting.Services);
            var suggestion = new Array();
            for (var i in services) {
                var service = services[i];
                suggestion.push(
                    {
                        id: service.Id,
                        value: service.AccountCode,
                        label: service.AccountCode + "-" + service.DisplayName

                    }
                );
            }

            $("#tbl-order-services .ItemCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr")
                    if (typeof account != "undefined" && account != null) {
                        var service = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.ItemName").val(account.Name);

                    }

                }
            });

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
        EquipmentAutoCompleteInit: function (id) {
            var $this = this;
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

            $("#MachineCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        var machines = Common.GetAccountDetailByAccountId(account.Id);
                        $("#MachineName").val(account.Name);
                        $("#MachineId").val(account.Id);
                        if (typeof machines != "undefined" && machines != null) {
                            $("#SrNo").val(machines.SrNo);
                        }
                        $(".container-message").hide();
                    }
                }
            });



        },
    };
}();

