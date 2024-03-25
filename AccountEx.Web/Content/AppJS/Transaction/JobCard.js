
var JobCard = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "JobCard";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#AccountCode";
    return {
        init: function () {
            var $this = this;

            $("input[name='ac-type']").change(function () {

                if ($(this).val() == "cash") {
                    var acc = Common.GetById(PageSetting.CashAccount);
                    $("#AccountId").val(acc.Id);
                    $("#AccountCode").val(acc.AccountCode);
                    $("#AccountName").val(acc.DisplayName);
                    $("#AccountCode").attr("disabled", "disabled");
                    $this.MapComments();
                }
                else {
                    $("#AccountCode").removeAttr("disabled");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                    $("#AccountName").val("");
                }

            });
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
                    setTimeout(function () {
                        $('#DelayTime').select2('focus');
                    }, 500);

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

                $("#EmployeeCode").focus();

            });
            $("#BehaviourLevel").change(function () {

                $('#ServicesLevel').select2('focus');

            });
            $("#ServicesLevel").change(function () {

                $("#Problem").focus();

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
                        $("#PreviousReading").focus();
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
            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {

                        $("#tbl-saleservicesitms tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();


                    }
                    else {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity").focus().select();
                    }

                }
            });
            $(document).on("blur", ".Code", function () {
                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr")
                if (typeof account != "undefined" && account != null) {
                    var service = Common.GetAccountDetailByAccountId(account.Id);
                    $(tr).find(":nth-child(1) input.ItemId").val(service.AccountId);
                    $(tr).find(":nth-child(2) input.Name").val(account.Name);
                    $(tr).find(":nth-child(5) input.Rate").val(Common.GetFloat(service.Rate));
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
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();

                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Quantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0) {
                    if (rate > 0)
                        $this.AddItem();
                    else
                        $(tr).find("input.Rate").focus();
                }
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $(document).on("keyup", ".Rate", function (event) {
                var tr = $(this).closest("tr");
                var rate = Common.GetFloat($(tr).find("input.Rate").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && rate > 0) {
                    $this.AddItem();
                }
                else if (event.which == 13 && rate <= 0) {
                    var err = "Item " + code + " must have rate greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Rate").focus();

                }


            });
            $(document).on("keyup", ".SaleTaxNo", function (event) {
                if (event.which == 13)
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity").focus().select();
            });

            $("#SalesmanCode").keyup(function (e) {
                if (e.which == 13) {

                    var salesman = Common.GetByCode($(this).val());

                    if (typeof salesman != "undefined" && salesman != null) {
                        $("#ContactPerson").focus();
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
                        $("#tbl-saleservicesitms tbody tr:nth-last-child(1) td:nth-child(3) input.ProductQuantity ").focus().select();
                    }

                }
            });
            $(document).on("blur", ".ProductCode", function () {
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {
                    var product = Common.GetAccountDetailByAccountId(account.Id);
                    $(tr).find(":nth-child(1) input.ProductId").val(product.AccountId);
                    $(tr).find(":nth-child(2) input.Name").val(account.Name);
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
            $(document).on("keyup", "input.SERate", function () {
                var tr = $(this).closest("tr");
                var Quantity = $(tr).find("input.SEQuantity").val();
                var Rate = $(tr).find("input.SERate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.SEAmount").val(Amount.toFixed(2));
            });

            $(document).on("keyup", "input.ProductRate,input.ProductQuantity", function () {
                var tr = $(this).closest("tr");
                var Quantity = $(tr).find("input.ProductQuantity").val();
                var Rate = $(tr).find("input.ProductRate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.ProductAmount").val(Amount.toFixed(2));
            });
            /////Ends/////

            $(document).on("blur", ".ProductQuantity", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.ProductQuantity ").val());
                var code = $(tr).find("input.ProductCode").val();

                if (qty <= 0) {
                    $(tr).find(":nth-child(3) input.ProductQuantity").val("1");
                }
                $this.GetQuantityTotal();

            });
            $(document).on("keyup", ".ProductQuantity,.ProductRate ", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.ProductQuantity").val());
                var code = $(tr).find("input.ProductCode").val();
                $this.GetQuantityTotal();
                if (event.which == 13 && qty > 0)
                    $this.AddServiceItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.ProductQuantity").focus();

                }


            });

            ////////starts/////////
            $(document).on("keyup", ".SEQuantity, .SERate ", function (event) {


                var tr = $(this).closest("tr");
                var qty = $(tr).find("input.SEQuantity").val();
                var rate = $(tr).find("input.SERate").val();
                var amount = qty * rate;
                $(tr).find("input.SEAmount").val(amount.toFixed(2));

                var code = $(tr).find("input.SECode").val();
                $this.GetSEQuantityTotal();
                if (event.which == 13 && qty > 0)
                    $this.AddServiceExpenses();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.SEQuantity").focus();

                }
                $this.GetServicesExpencesWholeTotal();

            });
            ///////ends////////////

            this.LoadPageSetting();
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();

            AppData.AccountDetail = PageSetting.AccountDetails.concat(PageSetting.EquipmentDetails);
            $this.EmployeeAutoCompleteInit();
            $this.EquipmentAutoCompleteInit();
            $this.SalesmanAutoCompleteInit();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {

                this.Add();

            }



        },

        AddServiceItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#tbl-saleservicesitms tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-saleservicesitms tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#product-template-item").html();
            $("#tbl-saleservicesitms tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#tbl-saleservicesitms tbody tr:nth-last-child(1) input.ProductCode").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.ProductAutoCompleteInit();
            Common.InitNumerics();
        },
        /////Starts/////
        AddServiceExpenses: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {
            var code = $("#tbl-ServiceExpenses tbody tr:nth-last-child(1) td:nth-child(1) input.SECode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-ServiceExpenses tbody tr:nth-last-child(1) td:nth-child(1) input.SECode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#product-template-item-serviceExpenses").html();
            $("#tbl-ServiceExpenses tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#tbl-ServiceExpenses tbody tr:nth-last-child(1) input.SECode").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.SEAutoCompleteInit();
            Common.InitNumerics();
        },
        /////Ends/////
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
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

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
            setTimeout(function () {
                $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
            }, 300);
            //}
            $this.AutoCompleteInit();
            $this.GetNextSaleTaxNo();
            Common.InitNumerics();
        },
        GetNextSaleTaxNo: function () {
            var $this = this;
            var saltaxno = 0;
            if ($("#item-container tbody").children().length <= 1) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?keyvalue=nextbillno",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Loading  ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            saltaxno = res.Data.SaleTaxNo;
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.SaleTaxNo").val(saltaxno);

                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });


            }

            else {
                var prevbillno = Common.GetInt($("#item-container tbody tr:nth-last-child(2) td:nth-child(3) input.SaleTaxNo").val());
                saltaxno = prevbillno + 1;
                $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.SaleTaxNo").val(saltaxno);
            }



        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            $this.GetServicesExpencesWholeTotal();
            $this.GetQuantityTotal();
            $this.GetSEQuantityTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
            if ($("#tbl-saleservicesitms tbody").children().length <= 0)
                $this.AddServiceItem();
            if ($("#tbl-ServiceExpenses tbody").children().length <= 0)
                $this.AddServiceExpenses();
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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();
            var ServicesItems = new Array();
            var ServiceExpenses = new Array();
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
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                ServicesItems = Common.SaveItemData("#tbl-saleservicesitms");
                ServicesItems = Enumerable.From(ServicesItems).Where("$.ServiceItemCode.trim()!=''").ToArray();
                ServiceExpenses = Common.SaveItemData("#tbl-ServiceExpenses");
                ServiceExpenses = Enumerable.From(ServiceExpenses).Where("$.ServiceExpenseCode.trim()!=''").ToArray();
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

                    var service = Common.GetByCode(item.ItemCode);
                    if (typeof service == "undefined" || service == null) {
                        err += item.ItemCode + " is not valid code.";
                    }

                }

                for (var i in ServicesItems) {
                    var item = ServicesItems[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ServiceItemCode + "(" + item.ServiceItemName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ServiceItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ServiceItemCode + " is not valid code.";
                    }

                }
                //////////starts///////////
                for (var i in ServiceExpenses) {
                    var item = ServiceExpenses[i];
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ServiceExpenseCode + "(" + item.ServiceExpenseName + ") must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ServiceExpenseCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ServiceExpenseCode + " is not valid code.";
                    }

                }
                //////ends////////////
                if (Items.length <= 0) {
                    err += "Please add atleast one item in services delivered";
                }
                //if (ServicesItems.length <= 0) {
                //    err += "Please add atleast one item in material consumed";
                //}
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "Transaction total amount should be graeter then zero(0).";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }


                record["TransactionType"] = VoucherType[voucher],
                record["SaleItems"] = Items;
                record["SaleServicesItems"] = ServicesItems;
                record["ServiceExpenses"] = ServiceExpenses;
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
        GetQuantityTotal: function (tr) {
            var $this = this;
            var Quantity = 0;
            $("#tbl-saleservicesitms tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find(":nth-child(3) input.ProductQuantity").val());

            });
            $("#tbl-saleservicesitms tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity.toFixed(2));
        },
        GetSEQuantityTotal: function (tr) {
            var $this = this;
            var Quantity = 0;
            $("#tbl-ServiceExpenses tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find(":nth-child(3) input.SEQuantity").val());

            });
            $("#tbl-ServiceExpenses tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity.toFixed(2));
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find(":nth-child(3) input.Quantity").val());
            Rate = Common.GetFloat($(tr).find(":nth-child(4) input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(5) input.Amount").val(amount.toFixed(2));
            var gst = PageSetting.GSTPercent;

            var gstAmount = Common.GetFloat(amount) * gst / 100;
            var netAmount = Common.GetFloat(amount) + gstAmount;
            $(tr).find(":nth-child(6) input.GstAmount ").val(gstAmount.toFixed(2));
            $(tr).find(":nth-child(7) input.NetAmount").val(netAmount.toFixed(2));

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var price = 0.0;
            var gstamount = 0.0;
            var netamount = 0.0;
            $("#item-container tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find(":nth-child(3) input.Quantity").val());
                price += Common.GetFloat($(this).find(":nth-child(5) input.Amount").val());
                gstamount += Common.GetFloat($(this).find(":nth-child(6) input.GstAmount").val());
                netamount += Common.GetInt($(this).find(":nth-child(7) input.NetAmount").val());
            });
            $("#item-container tfoot tr").find(":nth-child(2) input.Quantity").val(quantity.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(3) input.Amount").val(price.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(4) input.GstAmount").val(gstamount.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(5) input.NetAmount").val(netamount);
            if (price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(price);
            $("#GstAmountTotal").val(gstamount.toFixed(2));
            $("#NetTotal").val(netamount);
            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetServicesExpencesWholeTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var price = 0.0;
            var gstamount = 0.0;
            var netamount = 0.0;
            $("#tbl-ServiceExpenses tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find(":nth-child(3) input.SEQuantity").val());
                price += Common.GetFloat($(this).find(":nth-child(5) input.SEAmount").val());
            });
            $("#tbl-ServiceExpenses tfoot tr").find(":nth-child(2) input.Quantity").val(quantity.toFixed(2));
            $("#tbl-ServiceExpenses tfoot tr").find(":nth-child(3) input.SETotalAmount").val(price.toFixed(2));
            $("#ServicesExpencesTotal").val(price);
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetFloat($("#GrossTotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);
            $("#NetTotal").val(nettotal);
        },
        CustomClear: function () {
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
                html += "<td>" + item.GSTAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);
            var qtyTotal = Common.GetInt(Enumerable.From(items).Sum("$.Quantity")).format();
            var amountTotal = Common.GetInt(Enumerable.From(items).Sum("$.Amount")).format();
            var gstamtTotal = Common.GetInt(Enumerable.From(items).Sum("$.GSTAmount")).format();
            var grandTotal = Common.GetInt(Enumerable.From(items).Sum("$.NetAmount")).format();
            var gstpercent = Common.GetFloat(Enumerable.From(items).FirstOrDefault().GSTPercent);
            $("#report-saleitem tfoot tr").find("td:nth-child(3)").html(qtyTotal);
            $("#report-saleitem tfoot tr").find("td:nth-child(5)").html(amountTotal);
            $("#report-saleitem tfoot tr").find("td:nth-child(6)").html(gstamtTotal);
            $("#report-saleitem tfoot tr").find("td:nth-child(7)").html(grandTotal);
            $("#tblAgingItems thead ").find("#totalamount").html(amountTotal);
            $("#tblAgingItems thead ").find("#salestax").html(gstpercent + "%");
            $("#tblAgingItems thead ").find("#grandtotal").html(grandTotal);

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

                        $("#item-container tbody").html("");
                        $("#tbl-saleservicesitms tbody").html("");
                        $("#tbl-ServiceExpenses tbody").html("");
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
                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                Common.MapItemData(d.SaleItems);


                                $this.GetWholeTotal();

                            }
                            if (d.Id > 0 && d.SaleServicesItems != null && d.SaleServicesItems.length > 0) {
                                $this.MapServicesItems(d);

                            }

                            //////////starts///////////

                            if (d.Id > 0 && d.ServiceExpenses != null && d.ServiceExpenses.length > 0) {
                                $this.MapServiceExpenses(d);
                                $this.GetServicesExpencesWholeTotal();
                            }

                            /////////ends/////////////

                            setTimeout(function () {
                                $("#tbl-saleservicesitms tbody tr:nth-last-child(1) td:nth-child(1) input.ProductCode").focus().select();
                            }, 300);

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
                        $this.AddServiceItem();
                        $this.AddServiceExpenses();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        MapServicesItems: function (data) {
            var $this = this;
            Common.MapItemData(data.SaleServicesItems, "#tbl-saleservicesitms", "#product-template-item");
            $this.GetQuantityTotal();
        },
        ///////////starts/////////

        MapServiceExpenses: function (data) {
            var $this = this;
            Common.MapItemData(data.ServiceExpenses, "#tbl-ServiceExpenses", "#product-template-item-serviceExpenses");
            $this.GetSEQuantityTotal();
        },

        ///////////ends//////////
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
        LoadAccounts: function () {
            var $this = this;
            var id = PageSetting.Customers;
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
            $this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function (partyid) {
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

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr")
                    if (typeof account != "undefined" && account != null) {
                        var service = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ItemId").val(service.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(service.Rate));
                        $(".container-message").hide();
                    }
                    //$(tr).find("input.SaleTaxNo ").focus();
                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                }
            });

        },
        /////Starts/////
        SEAutoCompleteInit: function (partyid) {
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

            $(".SECode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr")
                    if (typeof account != "undefined" && account != null) {
                        var service = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.SEProductId").val(service.AccountId);
                        $(tr).find("input.SEName").val(account.Name);
                        $(tr).find("input.SERate").val(Common.GetFloat(service.Rate));
                        $(".container-message").hide();
                    }
                    //$(tr).find("input.SaleTaxNo ").focus();
                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                }
            });

        },
        /////Ends/////
        EquipmentAutoCompleteInit: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Equipment;
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
                        $("#SrNo").val(machines.SrNo);
                        $(".container-message").hide();
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
    };
}();

