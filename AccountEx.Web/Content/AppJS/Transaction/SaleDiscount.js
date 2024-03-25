﻿
var SaleDiscounts = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "SaleDiscount";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var SetFocus = "date";
    return {
        init: function () {
            var $this = this;
            var dataTypes = ["CashReceipts", "CashPayments", "BankReceipts", "BankPayments", "TransferVoucher"];

            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "date" : "code";
                    $this.LoadVoucher("same");
                }
            });
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    var isdate = $("#Date").val();
                    if (isdate == "" || isdate == null) {
                        var currentdate = Common.FormateDate(new Date(), true);
                        $("#Date").val(currentdate);
                    }
                    if (type == "cashreceipts" || type == "cashpayments")
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
                    else
                        $("#AccountCode").focus();
                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {

                    setTimeout(function () {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
                    }, 500);
                }
            });

            $(document).on("keyup", "input.Description,input.AccountName", function (event) {

                if (event.which == 13) {
                    $(this).parent().next("td").find("input").focus();
                }
            });

            $(document).on("keyup", ".AccountCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Description ").focus()
                    }
                }
            });
            $(document).on("keyup", ".Amount", function (event) {
                $this.GetWholeTotal();
                if (event.which == 13) {
                    $this.AddItem();
                }
            });
            $this.SetPrintCheck();
            $this.MapJsonData();
            $this.SetFormLabel();
            $this.LoadPageSetting();
            var element = $("#qty,#Rate");
            $(element).keyup(function () {
                GetQuantityPriceTotal();
            });
            $this.CustomClear();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                $this.GetNextVoucherNumber();
            }

            $this.GetWholeTotal();
        },

        ChangeType: function (type) {
            var $this = this;
            SetFocus = "voucher";
            window.history.pushState(type, document.title + " | " + type, "voucher?type=" + type);
            //document.title = document.title + " | " + type;
            $this.LoadPageSetting();
            if ($("#div-table").is(":visible")) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            }
            else if ($("#form-info").is(":visible")) {
                $this.Add();
            }
            Common.HighlightMenu();
        },
        SetPrintCheck: function () {
            $("#PrintVoucher").prop("checked", localStorage.PrintVoucher == "true" ? true : false);
            $.uniform.update("#PrintVoucher");
        },
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber");
        },
        CalculateIncomeTax: function () {
            var monthlySalary = parseFloat($("#Amount").val());
            var yearlySalary = monthlySalary * 12;
            var tax = 0.0, taxableAmount;
            if (yearlySalary > 750000) {
                taxableAmount = yearlySalary - 750000;
                tax = 17500 + taxableAmount * 0.10;
            }
            else if (yearlySalary > 400000) {
                taxableAmount = yearlySalary - 400000;
                tax = taxableAmount * 0.05;
            }
            var monthlyTax = (tax / 12).toFixed(0);
            var net = (monthlySalary - monthlyTax).toFixed(0);
            $("#TaxAmount").val(monthlyTax);
            $("#Total").val(net);
        },
        CalculateTotal: function () {
            var amount = Common.GetFloat($("#Amount").val());
            var tax = Common.GetFloat($("#Tax").val());
            var taxAmount = (amount * tax / 100).toFixed(0);
            var taxedAmount = (amount - taxAmount).toFixed(0);
            var total = 0.0;
            if (Setting.Voucher == "cashreceipts" || Setting.Voucher == "bankreceipts")
                total = amount - (amount * tax) / 100;
            if (Setting.Voucher == "cashpayments" || Setting.Voucher == "bankpayments")
                total = amount + (amount * tax) / 100;

            $("#TaxAmount").val(taxAmount);
            $("#Total").val(taxedAmount);
        },
        CalculateTotal_Backup: function () {
            var amount = Common.GetFloat($("#Amount").val());
            var tax = Common.GetFloat($("#Tax").val());
            var taxedAmount = (100 * amount / (100 + tax)).toFixed(2);
            var taxAmount = (amount - taxedAmount).toFixed(2);
            var total = 0.0;
            if (Setting.Voucher == "cashreceipts" || Setting.Voucher == "bankreceipts")
                total = amount - (amount * tax) / 100;
            if (Setting.Voucher == "cashpayments" || Setting.Voucher == "bankpayments")
                total = amount + (amount * tax) / 100;

            $("#TaxAmount").val(taxAmount);
            $("#Total").val(taxedAmount);
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        AddItem: function () {
            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").val();

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
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
            else {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            $("#Item").focus();
            //}
            $this.LoadAccounts();
        },

        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        SetFormLabel: function () {
            return;
            var $this = this;
            var CashAccount = PageSetting.CashAccountId;
            var FormTitle = PageSetting.FormTitle;
            $(".formtitle").html(FormTitle);
            var FromLabel = PageSetting.FromLabel;
            $("#lblfrom").html(FromLabel);
            $("#AccountIdFrom").attr("placeholder", FromLabel);
            var ToLabel = PageSetting.ToLabel;
            $("#lblto").html(ToLabel);
            $("#AccountIdTo").attr("placeholder", ToLabel);
            var ShowToLabel = PageSetting.ShowToLabel;
            if (ShowToLabel == "true")
                $("#ShowToLabel").removeClass("offscreen");

            if (Setting.Voucher == "cashreceipts") {
                //$("#AccountIdTo").removeAttr("data-required");
                $("#AccountIdFrom").attr("data-message", FromLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").select2("val", CashAccount != null ? CashAccount.Value : "");
            }
            else if (Setting.Voucher == "cashpayments") {
                // $("#AccountIdTo").removeAttr("data-required");
                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").select2("val", CashAccount != null ? CashAccount.Value : "");
            }
            else if (Setting.Voucher == "bankreceipts") {

                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdTo").attr("data-message", ToLabel + " is rquired");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").attr("placeholder", ToLabel);
            }
            else if (Setting.Voucher == "bankpayments") {

                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdTo").attr("data-message", ToLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").attr("placeholder", ToLabel);
            }
            else if (Setting.Voucher == "transfervoucher") {

                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdTo").attr("data-message", ToLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").attr("placeholder", ToLabel);
            }
            //} catch (e) {

            //}

        },
        GetWholeTotal: function () {

            var Quantity = 0;
            var amount = 0;
            var credit = 0;
            $("#item-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find(":nth-child(4) input.Amount").val());


            });
            $("#item-container tfoot tr td:nth-child(2) input").val(amount);
        },
        AccountIdFrom_Click: function () {
            //var commentTemplate = PageSetting.CommentTemplate;
            var comment = "";
            var voucher = this.GetType();
            //if (commentTemplate == null) {
            if (voucher == "cashpayments")
                comment = "Cash Payment to " + $("#AccountIdTo option:selected").text();
            else if (voucher == "cashreceipts")
                comment = "Cash received from " + $("#AccountIdFrom option:selected").text();
            $("#Comments").val(comment);
            //}
            //else {
            //    comment = commentTemplate.Value.replace("[AccountTitle]", $("#AccountIdFrom option:selected").text());
            //    $("#Comments").val(comment);
            //}
        },
        AccountIdTo_Click: function () {
            this.AccountIdFrom_Click();
        },
        LoadAccounts: function () {
            var accounts = AppData.COA;
            var filteraccount = new Array();
            var exids = new Array();
            var exids = new Array();
            var $this = this;
            var cashAccount = $.grep(accounts, function (e) { return e.Id == PageSetting.CashAccount; })[0];
            if (Common.isNullOrWhiteSpace(cashAccount)) {
                cashAccount =
                    {

                        Id: 0,
                        ParentId: 0,
                        AccountCode: "",
                        DisplayName: ""


                    };
            };
            switch ($this.GetType()) {
                case "cashreceipts":


                    exids = new Array();
                    exids.push(Common.GetInt(PageSetting.Products));
                    exids.push(Common.GetInt(cashAccount.ParentId));
                    //exids.push(Common.GetInt(PageSetting.Customers));
                    filteraccounts = Common.GetAllLeafAccounts(exids);
                    $(".master-account-label").html("Cash A/c Code<span class='required'>*</span>");

                    $("#AccountCode").val(cashAccount.AccountCode).prop("disabled", true);
                    $("#AccountName").val(cashAccount.DisplayName);
                    $("#AccountId").val(cashAccount.Id);
                    $("#thamount").html("Receipt");
                    break;
                case "cashpayments":

                    exids = new Array();
                    exids.push(Common.GetInt(PageSetting.Products));
                    exids.push(Common.GetInt(cashAccount.ParentId));
                    filteraccounts = Common.GetAllLeafAccounts(exids);
                    $(".master-account-label").html("Cash A/c Code<span class='required'>*</span>");
                    $("#AccountCode").val(cashAccount.AccountCode).prop("disabled", true);
                    $("#AccountName").val(cashAccount.DisplayName);
                    $("#AccountId").val(cashAccount.Id);
                    $("#thamount").html("Payment");
                    break;
                case "bankreceipts":

                    exids = new Array();
                    exids.push(Common.GetInt(PageSetting.Products));
                    filteraccounts = Common.GetAllLeafAccounts(exids);
                    $("#AccountCode").removeProp("disabled", true);
                    $(".master-account-label").html("Debit A/c Code<span class='required'>*</span>");
                    $("#thamount").html("Credit");
                    var bankAccountId = PageSetting.Banks;
                    var banks = Common.GetLeafAccounts(bankAccountId);
                    if (banks.length == 1) {
                        $("#AccountCode").val(banks[0].AccountCode);
                        $("#AccountName").val(banks[0].DisplayName);
                        $("#AccountId").val(banks[0].Id);
                    }
                    $this.LoadBanks();

                    break;
                case "bankpayments":
                    exids = new Array();
                    exids.push(Common.GetInt(PageSetting.Products));
                    filteraccounts = Common.GetAllLeafAccounts(exids);
                    $("#AccountCode").removeProp("disabled", true);
                    $(".master-account-label").html("Credit A/c Code<span class='required'>*</span>");
                    $("#thamount").html("Debit");
                    var bankAccountId = PageSetting.Banks;
                    var banks = Common.GetLeafAccounts(bankAccountId);
                    if (banks.length == 1) {
                        $("#AccountCode").val(banks[0].AccountCode);
                        $("#AccountName").val(banks[0].DisplayName);
                        $("#AccountId").val(banks[0].Id);
                    }
                    $this.LoadBanks();
                    break;

            }
            var tokens = $.grep(filteraccounts, function (e) { return e.Level == Setting.AccountLevel; });
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

            $("table tr .AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var ac = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof ac != "undefined" && ac != null) {
                        $(tr).find(":nth-child(1) input.AccountId").val(ac.Id);
                        $(tr).find(":nth-child(2) input.AccountName").val(ac.DisplayName);
                        $(tr).find(":nth-child(2) input.AccountName").focus();
                        $this.MapDescription(tr);
                    }
                }
            });



        },
        MapDescription: function (element) {
            var $this = this;
            var code = $("#AccountCode").val();
            var name = $("#AccountName").val();
            party = $(element).find(":nth-child(2) input.AccountName").val();
            switch ($this.GetType()) {
                case "cashreceipts":
                    $(element).find(":nth-child(3) input.Description").val("Cash Received from:" + party);
                    break;
                case "cashpayments":
                    $(element).find(":nth-child(3) input.Description").val("Cash paid to:" + party);

                    break;
                case "bankreceipts":
                    $(element).find(":nth-child(3) input.Description").val("Cheque Received from:" + party);
                    break;
                case "bankpayments":
                    $(element).find(":nth-child(3) input.Description").val("Cheque issued to:" + party);
                    break;

            }

        },
        SetValidationAndControl: function () {

        },
        MapJsonData: function () {
            $("#jsondata :input").each(function () {
                try {
                    Setting[$(this).attr("Id")] = $.parseJSON($(this).val());
                } catch (e) {
                    Setting[$(this).attr("Id")] = $(this).val();
                }
            });
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $("#form-info").removeClass("hide");
            $("#div-table,#div-report").addClass("hide");
            $("select", "#form-info").each(function () {
                $(this).val("").select2();
            });
            $this.CustomClear();
            $this.SetFormLabel();
            $this.GetNextVoucherNumber();
            $this.DetailView();
            $(".container-message").hide();
            $this.LoadAccounts();
            //$("#Date").datepicker().val(new Date());
            //$("#Date").datepicker("setDate", GetTodayDate(2));
            //$(".date-picker").datepicker();
        },
        ListView: function () {
            var $this = this;
            $("#form-info,#div-report").addClass("hide");
            $("#div-table").removeClass("hide");

            $("#form-info").removeClass("hide");
            $("#div-report").addClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            //$("#saleitem tbody .chooseninner").chosen();
            AllowNumerics();
            //SetDropDown();
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
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: "Record saved successfully." });
                SetFocus = "date";
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                if ($("#PrintVoucher").is(":checked")) {
                    $this.Print();
                }
                else {
                    var scope = $("#form-info-item");
                    // $this.Close();
                    $this.ListView();
                }
                $this.CustomClear();
            });
        },

        SaveRecord: function (callback) {
            var $this = this;
            $(".container-message").hide();
            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            var total = $("#item-container tfoot tr td:nth-child(2) input").val();
            var err = "";
            if (total <= 0) {
                err += "Can't add empty voucher.,";
            }
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err += "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                }
            }
            if (err != "") {
                Common.ShowError(err);
                return;
            }
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.AccountCode.trim()!=''").ToArray();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Amount <= 0) {
                        err += "Item " + item.AccountCode + "(" + item.AccountCode + ") must have amount greater than zero(0).";
                    }
                    var account = Common.GetByCode(item.AccountCode);
                    if (typeof account == "undefined" || account == null) {
                        err += item.AccountCode + " is not valid code.";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["TransactionType"] = VoucherType[voucher],
                record["VoucherItems"] = Items;
                record["NetTotal"] = $("#NetTotal").val();
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + "  voucher ...please wait",
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
        CustomClear: function () {
            $("#item-container tbody").html("");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var data = $.parseJSON(res.Data);
                        $("#form-info").removeClass("hide");
                        $("#div-table").addClass("hide");

                        var masterdetail = $.grep(data, function (e) { return e.EntryType == EntryType.MasterDetail })[0];
                        var item = $.grep(data, function (e) { return e.EntryType == EntryType.Item })[0];
                        for (var prop in masterdetail) {
                            $("#" + prop).val(masterdetail[prop]);
                        }
                        Common.SetDate("#Date", masterdetail.Date);
                        $("#lblVoucherNumber").html(masterdetail.VoucherNumber);
                        $("#AccountIdFrom").val(masterdetail.AccountId);
                        $("#AccountIdTo").val(item.AccountId);
                        var amount = parseFloat(masterdetail.Credit) + parseFloat(masterdetail.Debit);
                        $("#Amount").val(amount);

                        $("select", "#form-info").each(function () {
                            $(this).select2();
                        });
                        $this.DetailView();
                    }
                    else {
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
            if (d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
                var type = $this.GetType();
                var html = "";
                var items = d.VoucherItems;
                var index = 1;
                $("#report-saleitem tbody").html("");
                Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            }
            var nettotal = $("#NetTotal").val();
            $("#lblNetTotal").html(nettotal);
        },

        LoadVoucher: function (key) {
            var $this = this;
            $("#item-container tbody").html("");
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + "  voucher ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").focus();
                        } else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            Common.SetDate($("#Date"), d.Date);
                            if (d.Id > 0 && d.VoucherItems != null && d.VoucherItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                var html = "";
                                var items = d.VoucherItems;
                                Common.MapItemData(items);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
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
                        $this.GetWholeTotal();
                        $this.LoadReportData(res);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        LoadBanks: function () {
            console.log(PageSetting);
            var $this = this;
            var bankAccountId = PageSetting.Banks;
            var banks = Common.GetLeafAccounts(bankAccountId);
            var tokens = banks;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.AccountId,
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
                        $("#AccountName").val(d.DisplayName);
                        $("#AccountId").val(d.Id);




                    }
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
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno;
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
                    blockMessage: "Deleting  " + $this.GetType() + "  voucher ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Record deleted successfully." });
                            $this.GetNextVoucherNumber();

                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#FormSetting").val());
            var tokens = $.grep(formSetting, function (e) { return e.VoucherType.toLowerCase() == voucher; });
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            this.LoadAccounts();
            $(".voucher-title").html(PageSetting.FormTitle);
        }
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};