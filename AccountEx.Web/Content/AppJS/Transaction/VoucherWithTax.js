
var VoucherWithTax = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VoucherWithTax";
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
            $("#Amount,#Tax,#GSTPercent,#WHTPercent,#ITPercent").keyup(function () {
                $this.CalculateTotal();
            });
            $("#PrintVoucher").change(function () {
                localStorage.PrintVoucher = $("#PrintVoucher").is(":checked");
            });
            $(".chk-tax").change(function () {

                var element = $(this).parent().parent().parent().closest("div").find(".col-md-1 input[type='text']");
                if ($(this).is(":checked"))
                    $(element).prop("disabled", false).val($(element).attr("data-tax"));
                else
                    $(element).prop("disabled", true).val(0);
                $this.CalculateTotal();
            });



            $this.SetPrintCheck();
            $this.MapJsonData();
            $this.SetFormLabel();
            $this.LoadPageSetting();
            var element = $("#qty,#Rate");
            $(element).keyup(function () {
                GetQuantityPriceTotal();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            Setting.PageLandingView = "ListView";
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            else {
                $this.ListView();
            }
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
            var GSTPercent = Common.GetFloat($("#GSTPercent").val());
            var WHTPercent = Common.GetFloat($("#WHTPercent").val());
            var ITPercent = Common.GetFloat($("#ITPercent").val());
            var gstConstant = Common.GetFloat((100 + GSTPercent) / 100);
            var gstcalamount = amount / gstConstant;
            var whtConstant = (amount * WHTPercent) / 100;
            var ITAmount = Common.GetFloat((amount * ITPercent) / 100);
            //var gstAmount = (amount - amount * 100 / (100 + GSTPercent)) / 4;
            //var gstAmount = amount - (amount / gstConstant);
            //var whtAmount = amount - (amount / whtConstant);

            var gstAmount = amount - gstcalamount;
            gstAmount = Common.GetFloat(gstAmount);
            //var amountWithGST = amount + gstAmount;
            //var whtAmount = amountWithGST * WHTPercent / 100;
            var totalAmount = amount - gstAmount - whtConstant - ITAmount;
            //if (this.GetType() == "bankreceipts") {
            //    totalAmount = amountWithGST - whtAmount;
            //} else {
            //    totalAmount = amountWithGST + whtAmount;
            //}


            $("#GST").val(gstAmount.toFixed(2));
            $("#WHT").val(whtConstant.toFixed(2));
            $("#IT").val(ITAmount.toFixed(2));
            //$("#AmountWithGST").val(amountWithGST.toFixed());
            $("#Total").val(totalAmount.toFixed(2));
        },

        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        SetFormLabel: function () {
            return;
           var $this = this;
            var CashAccount = PageSetting.CashAccount;
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

            if ($this.GetType() == "cashreceipts") {
                //$("#AccountIdTo").removeAttr("data-required");
                $("#AccountIdFrom").attr("data-message", FromLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").select2("val", CashAccount != null ? CashAccount.Value : "");
            }
            else if ($this.GetType() == "cashpayments") {
                // $("#AccountIdTo").removeAttr("data-required");
                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").select2("val", CashAccount != null ? CashAccount.Value : "");
            }
            else if ($this.GetType() == "bankreceipts") {

                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdTo").attr("data-message", ToLabel + " is rquired");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").attr("placeholder", ToLabel);
            }
            else if ($this.GetType() == "bankpayments") {

                $("#AccountIdFrom").attr("data-message", ToLabel + " is required");
                $("#AccountIdTo").attr("data-message", ToLabel + " is required");
                $("#AccountIdFrom").attr("placeholder", FromLabel);
                $("#AccountIdTo").attr("placeholder", ToLabel);
            }
            else if ($this.GetType() == "transfervoucher") {

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
            Accounts = $.grep(Accounts, function (e) { return e.Level == Setting.AccountLevel });
            if ($this.GetType() == "cashreceipts") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var CashAccountId = PageSetting.CashAccount;
                ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId });
            }
            else if ($this.GetType() == "cashpayments") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var CashAccountId = PageSetting.CashAccount;
                ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId });
            }
            else if ($this.GetType() == "bankreceipts") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var BankAccountId = PageSetting.Banks;
                ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
            }
            else if ($this.GetType() == "bankpayments") {
                var MasterAccountId = PageSetting.MasterAccountId;
                // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                ToAccounts = Setting.Customers;
                var BankAccountId = PageSetting.Banks;
                FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
            }
            else if ($this.GetType() == "transfervoucher") {
                var MasterAccountId = PageSetting.MasterAccountId;
                //FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
                FromAccounts = Setting.Customers;
                var MasterAccountIdTo = PageSetting.MasterAccountIdTo;
                // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountIdTo });
                ToAccounts = Setting.Customers;
            }

            var html = "<option value=''></option>";
            for (var i in FromAccounts) {
                html += "<option value='" + FromAccounts[i].Id + "'>" + FromAccounts[i].Name + "</option>";
            }
            $("#AccountIdFrom").append(html);
            var html = "<option value=''></option>";
            for (var i in ToAccounts) {
                html += "<option value='" + ToAccounts[i].Id + "'>" + ToAccounts[i].Name + "</option>";
            }
            $("#AccountIdTo").append(html);
            if ($this.GetType() == "bankreceipts" || $this.GetType() == "bankpayments")
                $this.LoadBanks();


        },
        LoadAccounts2: function () {
           var $this = this;
            var Accounts = $.parseJSON($("#Accounts").val());
            $(".row-to-account,.row-from-account,.input-tax").addClass("hide");
            $("#AccountIdTo").attr("placeholder", "To");
            $("#lblto").html("To <span class='required'>*</span>");
            $("#AccountIdFrom").attr("placeholder", "From");
            $("#lblfrom").html("From <span class='required'>*</span>");
            switch ($this.GetType()) {
                case "cashreceipts":
                    Common.BindSelect(Accounts, "#AccountIdFrom", true);
                    $(".row-from-account").removeClass("hide");
                    var cashAccount = $.grep(Accounts, function (e) { return e.Id == PageSetting.CashAccount });
                    Common.BindSelect(cashAccount, "#AccountIdTo", false);
                    break;
                case "cashpayments":
                    Common.BindSelect(Accounts, "#AccountIdTo", true);
                    $(".row-to-account").removeClass("hide");
                    var cashAccount = $.grep(Accounts, function (e) { return e.Id == PageSetting.CashAccount });
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
           var $this = this;
            var type = $this.GetType().toLowerCase();
            $("#lblDate").html($("#Date").val());
            $("#lblVoucherNumber-print").html($("#lblVoucherNumber").html());
            $("#lblAccountTitle").html($("#AccountIdFrom option:selected").text());
            $("#lblComments").html($("#Comments").val());
            $(".label-amount").html($("#Amount").val());
            $("#lblAccountIdTo").html($("#AccountIdTo option:selected").text());
            $("#form-info").addClass("hide");
            $("#div-report").removeClass("hide");
            if (type == "bankreceipts") {
                $(".bank-rec").removeClass("hide");
                $(".bank-pay").addClass("hide");
            }
            else {
                $(".bank-rec").addClass("hide");
                $(".bank-pay").removeClass("hide");
            }
        },
        Print1: function () {
           var $this = this;
            var type = $this.GetType().toLowerCase();
            $("#lblDate").html($("#Date").val());
            $("#lblVoucherWithTaxNumber-print").html($("#lblVoucherNumber").html());
            $(".lblAccountTitle").html($("#AccountIdFrom option:selected").text());
            $("#lblComments").html($("#Comments").val());
            $(".label-amount").html($("#Amount").val());
            $(".label-gstamount").html($("#GST").val());
            $(".label-whtamount").html($("#WHT").val());
            $(".label-itamount").html($("#IT").val());
            $(".label-incamount").html($("#Total").val());
            $(".label-credit-total").html($("#Amount").val());




            $("#lblgstpercent").html($("#GSTPercent").val());
            $("#lblWHtxpercent").html($("#WHTPercent").val());
            $("#lblitpercent").html($("#ITPercent").val());
            $(".lblAccountIdTo").html($("#AccountIdTo option:selected").text());
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            $("#div-report").removeClass("hide");
            if (type == "bankreceipts") {
                $(".bank-rec").removeClass("hide");
                $(".bank-pay").addClass("hide");
            }
            else {
                $(".bank-rec").addClass("hide");
                $(".bank-pay").removeClass("hide");
            }
            setTimeout(function () {
                window.print();
                $("#div-report").addClass("hide");
            }, 500);


        },
        Save: function () {
           var $this = this;
            $this.SaveRecord(function () {
                if ($("#PrintVoucher").is(":checked")) {
                   // Common.ShowMessage(true, { message: "Record saved successfully." });
                    $this.Add();
                    Common.ShowMessage(true, { message: Messages.RecordSaved });
                    //$this.Print();
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

                if ($this.GetType() == "bankpayments") {
                    Item["Credit"] = Common.GetFloat($("#Total").val());        // Credit to bank Total = Amount-Tax
                }
                else {
                    Item["Credit"] = Common.GetFloat($("#Amount").val());       // Credit to party Amount
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
                if ($this.GetType() == "bankreceipts") {
                    Item["Debit"] = Common.GetFloat($("#Total").val());          // Debit to bank Total = Amount-Tax
                }
                else {
                    Item["Debit"] = Common.GetFloat($("#Amount").val());        // Debit to party Amount
                }
                Item["Comments"] = $("#Comments").val();
                Items.push(Item);


                taxSetting = PageSetting.GstAccountId;
                if (taxSetting != undefined && taxSetting != null) {
                    Item = new Object();
                    Item["TransactionType"] = VoucherType[$this.GetType()];
                    Item["Id"] = $("#Id").val();
                    Item["EntryType"] = EntryType.GST;
                    Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                    Item["AccountId"] = taxSetting;
                    Item["AccountTitle"] = "GST";
                    Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                    if ($this.GetType() == "bankreceipts")
                        Item["Debit"] = Common.GetFloat($("#GST").val());
                    else
                        Item["Credit"] = Common.GetFloat($("#GST").val());
                    Item["Comments"] = $("#Comments").val();
                    Items.push(Item);
                }
                taxSetting = PageSetting.WHTAccountId;
                if (taxSetting != undefined && taxSetting != null) {
                    Item = new Object();
                    Item["TransactionType"] = VoucherType[$this.GetType()];
                    Item["Id"] = $("#Id").val();
                    Item["EntryType"] = EntryType.WHT;
                    Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                    Item["AccountId"] = taxSetting;
                    Item["AccountTitle"] = "WHT";
                    Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                    if ($this.GetType() == "bankreceipts")
                        Item["Debit"] = Common.GetFloat($("#WHT").val());
                    else
                        Item["Credit"] = Common.GetFloat($("#WHT").val());
                    Item["Comments"] = $("#Comments").val();
                    Items.push(Item);
                }
                taxSetting = PageSetting.ITAccountId;

                if (taxSetting != undefined && taxSetting != null) {
                    Item = new Object();
                    Item["TransactionType"] = VoucherType[$this.GetType()];
                    Item["Id"] = $("#Id").val();
                    Item["EntryType"] = EntryType.IncomeTax;
                    Item["VoucherNumber"] = Common.GetInt($("#VoucherNumber").val());
                    Item["AccountId"] = taxSetting;
                    Item["AccountTitle"] = "IncomeTax";
                    Item["Date"] = Common.ChangeDateFormate($("#Date").val());
                    if ($this.GetType() == "bankreceipts")
                        Item["Debit"] = Common.GetFloat($("#IT").val());
                    else
                        Item["Credit"] = Common.GetFloat($("#IT").val());
                    Item["Comments"] = $("#Comments").val();
                    Items.push(Item);
                }
                var record = new Object();
                record["Items"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/?mode=" + mode,
                    type: "POST",
                    data: record,
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
            //if (this.GetType() == "bankreceipts") {
            //    $("#WHT").val("");
            //    $(".tr-gst,.tr-gst-total").addClass("hide");
            //} else {
            //    $(".tr-gst,.tr-gst-total").removeClass("hide");
            //}
            $("#GSTPercent").prop("disabled", false).val("16");
            $("#WHTPercent").prop("disabled", false).val("8");
            $("#ITPercent").prop("disabled", false).val("");
            $(".chk-tax").prop("checked", true);
            $.uniform.update();
        },
        Edit: function (id, isprint, loadwithvoucher) {
           var $this = this;
            if (typeof loadwithvoucher == undefined || loadwithvoucher == null || !loadwithvoucher)
                loadwithvoucher = false;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType() + "&loadwithvoucherno=" + loadwithvoucher,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var data = res.Data;
                        $("#form-info").removeClass("hide");
                        $("#div-table").addClass("hide");

                        var masterdetail = $.grep(data, function (e) { return e.EntryType == EntryType.MasterDetail; })[0];
                        var item = $.grep(data, function (e) { return e.EntryType == EntryType.Item; })[0];
                        for (var prop in masterdetail) {
                            $("#" + prop).val(masterdetail[prop]);
                        }
                        Common.SetDate("#Date", masterdetail.Date);
                        $("#lblVoucherNumber").html(masterdetail.VoucherNumber);
                        $("#AccountIdFrom").val(masterdetail.AccountId);
                        $("#AccountIdTo").val(item.AccountId);
                        var amount = parseFloat(masterdetail.Credit) + parseFloat(masterdetail.Debit);
                        var total = parseFloat(item.Credit) + parseFloat(item.Debit);


                        var GST = $.grep(data, function (e) { return e.EntryType == EntryType.GST; })[0];
                        var WHT = $.grep(data, function (e) { return e.EntryType == EntryType.WHT; })[0];
                        var IncomeTax = $.grep(data, function (e) { return e.EntryType == EntryType.IncomeTax; })[0];

                        if (typeof GST != "undefined" && GST != null)
                            $("#GST").val($this.GetType() == "bankreceipts" ? Common.GetFloat(GST.Debit).toFixed(2) : Common.GetFloat(GST.Credit).toFixed(2));
                        if (typeof WHT != "undefined" && WHT != null)
                            $("#WHT").val($this.GetType() == "bankreceipts" ? Common.GetFloat(WHT.Debit).toFixed(2) : Common.GetFloat(WHT.Credit).toFixed(2));
                        if (typeof IncomeTax != "undefined" && IncomeTax != null)
                            $("#IT").val($this.GetType() == "bankreceipts" ? Common.GetFloat(IncomeTax.Debit).toFixed(2) : Common.GetFloat(IncomeTax.Credit).toFixed(2));
                        if ($this.GetType() == "bankreceipts") {
                            $("#Amount").val(amount);
                            $("#Total").val(total);
                        } else {
                            $("#Total").val(amount);
                            $("#Amount").val(total);
                        }
                        $("#GSTPercent").val(Common.GetFloat(($("#GST").val() / ($("#Amount").val() - $("#GST").val())) * 100).toFixed(2));
                        $("#WHTPercent").val(Common.GetFloat($("#WHT").val() * 100 / $("#Amount").val()).toFixed(2));
                        $("#ITPercent").val(Common.GetFloat($("#IT").val() * 100 / $("#Amount").val()).toFixed(2));
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
            var BankAccountId = PageSetting.Banks;
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
                        if (voucher == "bankpayments")
                            Common.BindSelect(res.Data, "#AccountIdFrom", true);
                        else if (voucher == "bankreceipts")
                            Common.BindSelect(res.Data, "#AccountIdTo", true);
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
            var tokens = $.grep(formSetting, function (e) { return e.VoucherType.toLowerCase() == voucher });
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            this.LoadAccounts2();
        }
    };
}();

