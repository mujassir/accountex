
var Voucher = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "Voucher";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();

    return {
        init: function () {
           var $this = this;
            //$("ul.voucher.sub-menu li a").click(function () {
            //    var type = $(this).attr("data-type");
            //    $this.ChangeType(type);
            //    return false;
            //});
            var dataTypes = ["CashReceipts", "CashPayments", "BankReceipts", "BankPayments", "TransferVoucher"];
            for (var i = 0; i < dataTypes.length; i++) {
                $("a[data-type='" + dataTypes[i] + "']").click(function () {
                    var type = $(this).attr("data-type");
                    $this.ChangeType(type);
                    return false;
                });
            }

            $("#Amount,#Tax").keyup(function () {
                if ($("#IncomeTax").is(":checked"))
                    $this.CalculateIncomeTax();
                else
                    $this.CalculateTotal();
            });
            $("#PrintVoucher").change(function () {
                localStorage.PrintVoucher = $("#PrintVoucher").is(":checked");
            });
            $("#IncomeTax").change(function () {
                if ($(this).is(":checked")) {
                    $("#Tax").val("").attr("readonly", "readonly");
                    $this.CalculateIncomeTax();
                }
                else
                    $("#Tax").removeAttr("readonly");
            });
            $this.SetPrintCheck();
            $this.MapJsonData();
            $this.SetFormLabel();
            $this.LoadPageSetting();
            var element = $("#qty,#Rate");
            $(element).keyup(function () {
                GetQuantityPriceTotal();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + Setting.Voucher;
            Setting.PageLandingView = "ListView";
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            else {
                $this.ListView();
            }
            if (Setting.Voucher == "bankreceipts" || Setting.Voucher == "bankpayments")
                $(".input-tax").removeClass("hide");
            else
                $(".input-tax").addClass("hide");
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $this.Edit(Common.GetInt(Url.voucherno), false, true);
            }
        },
        ChangeType: function (type) {
           var $this = this;
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
           var $this = this;
            var voucher = $this.GetType();
            var Accounts = Setting.Account;
            var FromAccounts = Setting.Account;
            var ToAccounts = Setting.Account;
            Accounts = $.grep(Accounts, function (e) { return e.Level == Setting.AccountLevel; });
            if (Setting.Voucher == "cashreceipts") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var CashAccountId = PageSetting.CashAccountId;
                ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId });
            }
            else if (Setting.Voucher == "cashpayments") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var CashAccountId = PageSetting.CashAccountId;
                ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId });
            }
            else if (Setting.Voucher == "bankreceipts") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var BankAccountId = PageSetting.BankAccountId;
                ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
            }
            else if (Setting.Voucher == "bankpayments") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                ToAccounts = Setting.Customers;
                var BankAccountId = PageSetting.BankAccountId;
                FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
            }
            else if (Setting.Voucher == "transfervoucher") {
                var MasterAccountId = PageSetting.MasterAccountId;
                //FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var MasterAccountIdTo = PageSetting.MasterAccountIdTo;
                // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountIdTo });
                ToAccounts = Setting.Customers;
            }

            var html = "<option value=''></option>";
            for (var i in FromAccounts) {
                html += "<option value='" + FromAccounts[i].Id + "'>" + FromAccounts[i].AccountCode + "-" + FromAccounts[i].Name + "</option>";
            }
            $("#AccountIdFrom").append(html);
            var html = "<option value=''></option>";
            for (var i in ToAccounts) {
                html += "<option value='" + ToAccounts[i].Id + "'>" + ToAccounts[i].AccountCode + "-" + ToAccounts[i].Name + "</option>";
            }
            $("#AccountIdTo").append(html);
            if (Setting.Voucher == "bankreceipts" || Setting.Voucher == "bankpayments")
                $this.LoadBanks();


        },
        LoadAccounts2: function () {
           var $this = this;
            var Accounts = $.parseJSON($("#Accounts").val());
            for (var i in Accounts) {
                Accounts[i].Name = Accounts[i].AccountCode + "-" + Accounts[i].Name;
            }
            $(".row-to-account,.row-from-account,.input-tax").addClass("hide");
            $("#AccountIdTo").attr("placeholder", "To");
            $("#lblto").html("To <span class='required'>*</span>");
            $("#AccountIdFrom").attr("placeholder", "From");
            $("#lblfrom").html("From <span class='required'>*</span>");
            switch ($this.GetType()) {
                case "cashreceipts":
                    Common.BindSelect(Accounts, "#AccountIdFrom", true);
                    $(".row-from-account").removeClass("hide");
                    var cashAccount = $.grep(Accounts, function (e) { return e.Id == PageSetting.CashAccountId; });
                    Common.BindSelect(cashAccount, "#AccountIdTo", false);
                    break;
                case "cashpayments":
                    Common.BindSelect(Accounts, "#AccountIdTo", true);
                    $(".row-to-account").removeClass("hide");
                    var cashAccount = $.grep(Accounts, function (e) { return e.Id == PageSetting.CashAccountId; });
                    Common.BindSelect(cashAccount, "#AccountIdFrom", false);
                    break;
                case "bankreceipts":
                    $this.LoadBanks();
                    Common.BindSelect(Accounts, "#AccountIdFrom", true);
                    $("#AccountIdTo").attr("placeholder", "Bank");
                    $("#lblto").html("Bank <span class='required'>*</span>");
                    $(".row-to-account,.row-from-account,.input-tax").removeClass("hide");
                    break;
                case "bankpayments":
                    $this.LoadBanks();
                    Common.BindSelect(Accounts, "#AccountIdTo", true);
                    $("#AccountIdFrom").attr("placeholder", "Bank");
                    $("#lblfrom").html("Bank <span class='required'>*</span>");
                    $(".row-to-account,.row-from-account,.input-tax").removeClass("hide");
                    break;
                case "transfervoucher":
                    Common.BindSelect(Accounts, "#AccountIdTo", true);
                    Common.BindSelect(Accounts, "#AccountIdFrom", true);
                    $(".row-to-account,.row-from-account").removeClass("hide");
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
            $this.LoadAccounts2();
            //$("#Date").datepicker().val(new Date());
            //$("#Date").datepicker("setDate", GetTodayDate(2));
            //$(".date-picker").datepicker();
        },
        ListView: function () {
           var $this = this;
            $("#form-info,#div-report").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                var option =
                    {
                        "aaSorting": [[6, "desc"]],
                    };
                DataTable.BindDatatable(DATATABLE_ID, url, option);
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
            $("#lblDate").html($("#Date").val());
            $("#lblVoucherNumber-print").html($("#lblVoucherNumber").html());
            $("#lblAccountTitle").html($("#AccountIdFrom option:selected").text());
            $("#lblComments").html($("#Comments").val());
            $(".label-amount").html($("#Amount").val());
            $("#form-info").addClass("hide");
            $("#div-report").removeClass("hide");
        },
        Print1: function () {
            $("#lblDate").html($("#Date").val());
            $("#lblVoucherNumber-print").html($("#lblVoucherNumber").html());
            $("#lblAccountTitle").html($("#AccountIdFrom option:selected").text());
            $("#lblComments").html($("#Comments").val());
            $(".label-amount").html($("#Amount").val());
            $("#form-info").addClass("hide");
            $("#div-report").removeClass("hide");
            setTimeout(function () {
                window.print();
                $("#div-report").addClass("hide");
            }, 500);


        },
        Save: function () {
           var $this = this;
            $this.SaveRecord(function () {
                if ($("#PrintVoucher").is(":checked")) {
                    //$this.Print();
                    //Common.ShowMessage(true, { message: "Record saved successfully." });
                    $this.Add();
                    Common.ShowMessage(true, { message: Messages.RecordSaved });
                }
                else {
                    Common.ShowMessage(true, { message: "Record saved successfully." });
                    $this.Add();
                }
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
                $this.RebindData();
                $this.CustomClear();
            });
        },

        SaveRecord: function (callback) {
           var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var record = new Object();
            var Items = new Array();
            var id = Common.GetInt($("#Id").val());
            if (id > 0)
                mode = "update";
            if (Common.Validate($("#mainform"))) {
                var Currentsetting = PageSetting.FromAccountTr;
                var Item = new Object();
                Item["TransactionType"] = VoucherType[$this.GetType()];
                Item["Id"] = $("#Id").val();
                Item["Comments"] = $("#Comments").val();
                Item["EntryType"] = EntryType.MasterDetail;
                Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                Item["AccountId"] = $("#AccountIdFrom").val();
                Item["AccountTitle"] = $("#AccountIdFrom option:selected").text();
                Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                if (Setting.Voucher == "bankpayments") {
                    Item["Credit"] = Common.GetFloat($("#Total").val());
                }
                else {
                    Item["Credit"] = Common.GetFloat($("#Amount").val());
                }
                //if (Setting.Voucher == "bankreceipts") {
                //    Item["Credit"] = Common.GetFloat($("#Amount").val());
                //}
                //else {
                //    Item["Credit"] = Common.GetFloat($("#Total").val());
                //}
                Items.push(Item);
                Item = new Object();
                Currentsetting = PageSetting.ToAccountTr;
                Item["TransactionType"] = VoucherType[$this.GetType()];
                Item["Id"] = $("#Id").val();
                Item["EntryType"] = EntryType.Item;
                Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                Item["AccountId"] = $("#AccountIdTo").val();
                Item["AccountTitle"] = $("#AccountIdTo option:selected").text();
                Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                if (Setting.Voucher == "bankreceipts") {
                    Item["Debit"] = Common.GetFloat($("#Total").val());
                }
                else {
                    Item["Debit"] = Common.GetFloat($("#Amount").val());
                }
                Item["Comments"] = $("#Comments").val();
                Items.push(Item);
                taxSetting = PageSetting.TaxAccountId;
                if ((Setting.Voucher == "bankreceipts" || Setting.Voucher == "bankpayments") && taxSetting != undefined && taxSetting != null) {
                    Item = new Object();
                    Currentsetting = PageSetting.TaxAccountTr;
                    Item["TransactionType"] = VoucherType[Setting.Voucher];
                    Item["Id"] = $("#Id").val();
                    Item["EntryType"] = EntryType.Tax;
                    Item["Quantity"] = 1,
                    Item["Price"] = Common.GetInt($("#Amount").val());
                    Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                    Item["AccountId"] = taxSetting.Value;
                    Item["AccountTitle"] = "Tax";
                    Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                    //Item["Credit"] =Common. GetFloat($("#TaxAmount").val()),
                    if (Setting.Voucher == "bankreceipts")
                        Item["Debit"] = Common.GetFloat($("#TaxAmount").val());
                    else if (Setting.Voucher == "bankpayments")
                        Item["Credit"] = Common.GetFloat($("#TaxAmount").val());

                    Item["Comments"] = $("#Comments").val();
                    Items.push(Item);
                }
                var record = new Object();
                record["Items"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/?mode=" + mode,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + "   voucher ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            //GetNextVoucherNumber();
                            callback();
                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            }
        },
        CustomClear: function () {
            $("#saleitem tbody").html("");
        },
        Edit: function (id, isprint, loadwithvoucher) {
           var $this = this;
            if (typeof loadwithvoucher == undefined || loadwithvoucher == null)
                loadwithvoucher = false;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType() + "&loadwithvoucherno=" + loadwithvoucher,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + "  voucher ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;


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
                        if (typeof isprint != "undefined" && isprint) {
                            $this.Print1();
                        }
                        else {
                            $("#form-info").removeClass("hide");
                            $("#div-table").addClass("hide");
                            $this.DetailView();
                        }

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },


        LoadBanks: function () {
           var $this = this;
            var BankAccountId = PageSetting.BankAccountId;
            var qs = "key=GetLeafAccounts";
            qs += "&AccountId=" + BankAccountId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/" + "?" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    var voucher = $this.GetType();
                    if (res.Success) {
                        var accounts = res.Data;
                        //for (var i in accounts) {
                        //    accounts[i].Name = accounts[i].AccountCode + "-" + accounts[i].Name;
                        //}

                        if (voucher == "bankpayments")
                            Common.BindSelect(accounts, "#AccountIdFrom", true);
                        else if (voucher == "bankreceipts")
                            Common.BindSelect(accounts, "#AccountIdTo", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetNextVoucherNumber: function () {
           var $this = this;
            var d = "type=" + $this.GetType();
            Common.WrapAjax({
                url: "../Transaction/GetNextVoucherNumber",
                type: "POST",
                data: d,
                success: function (res) {
                    var q = JSON.parse(res);
                    if (q.Success) {
                        $("#VoucherNumber").val(q.Data);
                        $("#lblVoucherNumber").html(q.Data);
                    }
                    else {
                        ShowError(q.Data);
                    }
                },
                error: function (e) {
                }
            });
        },
        Delete: function (id) {
           var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting  " + $this.GetType() + "  voucher ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.RebindData();
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
            this.LoadAccounts2();
        }
    };
}();

