
var BankVoucher = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VoucherTrans";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var Banks = new Array();
    var SetFocus = "date";
    return {
        init: function () {
            var $this = this;
            //$("ul.voucher.sub-menu li a").click(function () {
            //    var type = $(this).attr("data-type");
            //    $this.ChangeType(type);
            //    return false;
            //});
            var dataTypes = ["CashReceipts", "CashPayments", "BankReceipts", "BankPayments", "TransferVoucher"];
            //for (var i = 0; i < dataTypes.length; i++) {
            //    $("a[data-type='" + dataTypes[i] + "']").click(function () {
            //        var type = $(this).attr("data-type");
            //        $this.ChangeType(type);
            //        return false;
            //    });
            //}
            $(document).on("keyup", "input.Description,input.AccountName", function (event) {
                if (event.which == 13) {
                    $(this).parent().next("td").find("input").focus();
                }
            });
            $("#Date").keyup(function (e) {
                if (e.which == 13) {

                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
                }
            });
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "date" : "code";
                    $this.LoadVoucher("same");
                }
            });
            $(document).on("keyup", ".Debit,.Credit", function (event) {
                $this.GetWholeTotal();
                if (event.which == 13)
                    $this.AddItem();
                if ($(this).hasClass("Debit")) {
                    if (Common.GetFloat($(this).val().trim()) > 0) {
                        $(this).parent().next("td").find("input").prop("disabled", true);
                        $(this).parent().next("td").find("input").val("");
                    }
                    else
                        $(this).parent().next("td").find("input").removeProp("disabled");
                }
                else {

                    if (Common.GetFloat($(this).val().trim()) > 0) {
                        $(this).parent().prev("td").find("input").prop("disabled", true);
                        $(this).parent().prev("td").find("input").val("");
                    }
                    else
                        $(this).parent().prev("td").find("input").removeProp("disabled");
                }
                $this.SetDebitCreditControl();
            });
            $(document).on("keydown", "input.debit", function (event) {
            });
            $(document).on("keyup", ".AccountCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $(this).parent().parent().find("td:nth-child(3) input.Description").focus();
                    }
                }
            });
            $(document).on("blur", ".AccountCode", function (event) {
                var tr = $(this).parent().parent();
                $this.SetDebitCreditControl(tr);
            });
            var element = $("#qty,#Rate");
            $(element).keyup(function () {
                GetQuantityPriceTotal();
            });
            $this.SetPrintCheck();
            $this.MapJsonData();
            $this.SetFormLabel();
            $this.LoadPageSetting();
            $this.LoadTerritory();
            Banks = Common.GetLeafAccounts(PageSetting.Banks);
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                $this.GetNextVoucherNumber();
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
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber");
        },
        SetPrintCheck: function () {
            $("#PrintVoucher").prop("checked", localStorage.PrintVoucher == "true" ? true : false);
            $.uniform.update("#PrintVoucher");
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        AddItem: function () {
            var $this = this;
            var accode = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode ").val();
            var description = $("#item-container tbody tr:nth-last-child(1) input.Description").val();
            var chqNo = $("#item-container tbody tr:nth-last-child(1) input.ChequeNumber").val();
            var chqDate = $("#item-container tbody tr:nth-last-child(1) input.Dated").val();
            if (typeof accode != "undefined" && accode.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode ").focus().select();
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
                    $("#template-item tbody tr:nth-last-child(1) td:nth-child(1) input.AccountCode").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            $("#Item").focus();
            $("#item-container tbody tr:nth-last-child(1) input.Description").val(description);
            $("#item-container tbody tr:nth-last-child(1) input.ChequeNumber").val(chqNo);
            $("#item-container tbody tr:nth-last-child(1) input.Dated").val(chqDate);
            //}

            $this.LoadAccounts();
            Common.InitDateMask();
            Common.InitNumerics();
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
            if ($("#template-item tbody").children().length <= 0)
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
            var debit = 0;
            var credit = 0;
            $("#item-container tbody tr").each(function () {
                debit += Common.GetFloat($(this).find("input.Debit").val());
                credit += Common.GetFloat($(this).find("input.Credit ").val());

            });
            $("#item-container tfoot tr td:nth-child(2) input").val(debit);
            $("#item-container tfoot tr td:nth-child(3) input").val(credit);
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
        //LoadAccounts: function () {
        //   var $this = this;
        //    var voucher = $this.GetType();
        //    var Accounts = Setting.Account;
        //    var FromAccounts = Setting.Account;
        //    var ToAccounts = Setting.Account;
        //    Accounts = $.grep(Accounts, function (e) { return e.Level == Setting.AccountLevel; });
        //    if (Setting.Voucher == "cashreceipts") {
        //        var MasterAccountId = PageSetting.MasterAccountId;
        //        // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
        //        FromAccounts = Setting.Customers;
        //        var CashAccountId = PageSetting.CashAccountId;
        //        ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId });
        //    }
        //    else if (Setting.Voucher == "cashpayments") {
        //        var MasterAccountId = PageSetting.MasterAccountId;
        //        // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
        //        FromAccounts = Setting.Customers;
        //        var CashAccountId = PageSetting.CashAccountId;
        //        ToAccounts = $.grep(Accounts, function (e) { return e.Id == CashAccountId; });
        //    }
        //    else if (Setting.Voucher == "bankreceipts") {
        //        var MasterAccountId = PageSetting.MasterAccountId;
        //        // FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
        //        FromAccounts = Setting.Customers;
        //        var BankAccountId = PageSetting.BankAccountId;
        //        ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
        //    }
        //    else if (Setting.Voucher == "bankpayments") {
        //        var MasterAccountId = PageSetting.MasterAccountId;
        //        // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
        //        ToAccounts = Setting.Customers;
        //        var BankAccountId = PageSetting.BankAccountId;
        //        FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == BankAccountId });
        //    }
        //    else if (Setting.Voucher == "transfervoucher") {
        //        var MasterAccountId = PageSetting.MasterAccountId;
        //        //FromAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountId });
        //        FromAccounts = Setting.Customers;
        //        var MasterAccountIdTo = PageSetting.MasterAccountIdTo;
        //        // ToAccounts = $.grep(Accounts, function (e) { return e.ParentId == MasterAccountIdTo });
        //        ToAccounts = Setting.Customers;
        //    }

        //    var html = "<option value=''></option>";
        //    for (var i in FromAccounts) {
        //        html += "<option value='" + FromAccounts[i].Id + "'>" + FromAccounts[i].AccountCode + "-" + FromAccounts[i].Name + "</option>";
        //    }
        //    $("#AccountIdFrom").append(html);
        //    var html = "<option value=''></option>";
        //    for (var i in ToAccounts) {
        //        html += "<option value='" + ToAccounts[i].Id + "'>" + ToAccounts[i].AccountCode + "-" + ToAccounts[i].Name + "</option>";
        //    }
        //    $("#AccountIdTo").append(html);
        //    if (Setting.Voucher == "bankreceipts" || Setting.Voucher == "bankpayments")
        //        $this.LoadBanks();


        //},
        LoadAccounts: function () {
            var $this = this;
            //alert();
            //var tokens = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel; });
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            //exids.push(Common.GetInt(PageSetting.Customers));
            var tokens = Common.GetAllLeafAccounts(exids);
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

            $(".AccountCode").autocomplete({
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
                        $(tr).find(":nth-child(3) input.AccountName").focus();
                        $this.SetDebitCreditControl(tr);
                    }
                }
            });
        },
        LoadTerritory: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.TerritoryManagerHeadId);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.territories").html(html).select2();


        },
        SetValidationAndControl: function () {

        },
        SetDebitCreditControl: function (tr) {
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var accountId = Common.GetInt($(tr).find(":nth-child(1) input.AccountId").val());
            if (Enumerable.From(Banks).Any(function (x) { return x.Id == accountId })) {
                if (VoucherType[voucher] == VoucherType.bankreceipts) {
                    $(tr).find(":nth-child(7) input.Credit").prop("disabled", true);
                    $(tr).find(":nth-child(6) input.Debit").prop("disabled", false);
                }
                else if (VoucherType[voucher] == VoucherType.bankpayments) {
                    $(tr).find(":nth-child(7) input.Credit").prop("disabled", false);
                    $(tr).find(":nth-child(6) input.Debit").prop("disabled", true);
                }
            }
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

        },
        ListView: function () {
            var $this = this;
            $("#form-info,#div-report").addClass("hide");
            $("#div-table").removeClass("hide");
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
                SetFocus = "date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
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
            var err = "";

            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.AccountCode.trim()!=''").ToArray();
                var debittotal = $("#item-container tfoot tr td:nth-child(2) input").val();
                var credittotal = $("#item-container tfoot tr td:nth-child(3) input").val();
                if (debittotal == 0 || credittotal == 0) {
                    err += "Can't add empty voucher.,";
                }
                else {
                    if (id > 0) {
                        var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                        var newvoucherno = Common.GetInt(record.VoucherNumber);
                        if (prevoucherno != newvoucherno) {
                            err += "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,"
                        }
                    }
                    var accountIds = Enumerable.From(Items).Select("Common.GetInt($.AccountId)").ToArray();
                    //if (!Enumerable.From(Banks).Any(function (x) { return $.inArray(x.Id, accountIds) > -1 })) {
                    //    err += "Atleast one bank must be selected.,";
                    //}
                    if (debittotal != credittotal) {
                        err += "Voucher is not balance.,";
                    }
                    for (var i in Items) {
                        var item = Items[i];
                        if (item.AccountCode.trim() == "" && (item.Debit > 0 || item.Credit > 0)) {
                            err += "party is required.,";
                        }
                        else if (item.AccountCode.trim() != "") {
                            var party = Common.GetByCode(item.AccountCode);
                            if (typeof party == "undefined" || party == null) {
                                err += item.AccountCode + " is not valid code.,";
                            }
                            else if ((item.Debit <= 0 && item.Credit <= 0)) {
                                err += "debit/credit is required for account code " + item.AccountCode.trim() + ".,";
                            }
                        }
                    }
                }
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                Items = $.grep(Items, function (element) {

                    return element.AccountCode.trim() != "";
                });
                record["TransactionType"] = VoucherType[voucher],
                record["VoucherItems"] = Items;
                LIST_CHANGED = true;
                //"bankVoucher=true" must required to call sepecific bank voucher in controller
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?bankVoucher=true",
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

        Final: function (isfinal) {
            var $this = this;
            $(".container-message").hide();
            $(".container-message").hide();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?bankVoucher=true" + "&isfinal=" + isfinal,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Transaction is final successfully." });
                        SetFocus = "date";
                        if (isfinal) {
                            $("#btnunfinal").removeClass("hide");
                            $("#btnsaverecord,#btndelete,#btnfinal").addClass("hide");
                        }
                        else {
                            $("#btnsaverecord,#btndelete,#btnfinal").removeClass("hide");
                            $("#btnunfinal").addClass("hide");
                        }

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
            // }
        },

        CustomClear: function () {
            Common.Clear();
            $("#btnsaverecord,#btndelete").removeClass("hide");
            $("#btnfinal").addClass("hide");
            $("#btnunfinal").addClass("hide");
            $("#btndelete,#btnprint").prop("disabled", true);
            $("#item-container tbody").html("");
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
            if (d == null)
                return;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));
            var type = $this.GetType();
            var html = "";
            var items = d.VoucherItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            var debit = $("#Debit").val();
            var credit = $("#Credit").val();
            $("#lblDebit").html(debit.format());
            $("#lblCredit").html(credit.format());
            var nettotal = debit;

            if (PageSetting.AmountInWordType == AmountInWordType.Million)
                wordAmount = Common.WordsConversionInMillion(nettotal);
            else
                wordAmount = Common.WordsConversion(nettotal);

            $("#lblAmountInWords").html(wordAmount);
        },

        LoadVoucher: function (key) {
            var $this = this;
            $("#item-container tbody").html("");
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var allowFinal = false;
            if (VoucherType[$this.GetType()] == VoucherType.bankreceipts && PageSetting.IsBankReceiptAllowFinalization) {
                allowFinal = true;
            }
            else if (VoucherType[$this.GetType()] == VoucherType.bankpayments && PageSetting.IsBankPaymentAllowFinalization) {
                allowFinal = true;
            }
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
                        $this.CustomClear();
                        $("#item-container tbody").html("");
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
                            if (d.Id > 0 && d.VoucherItems != null && d.VoucherItems.length > 0) {
                                if (d.IsFinal && allowFinal) {
                                    $("#btnsaverecord,#btnfinal,#btndelete").addClass("hide");
                                    $("#btnunfinal").removeClass("hide");
                                }
                                else {
                                    $("#btnfinal").removeClass("hide");
                                    $("#btnunfinal").addClass("hide");
                                }
                                $("#btndelete,#btnprint").prop("disabled", false);
                                var html = "";
                                var items = d.VoucherItems;
                                Common.MapItemData(items);
                                $this.GetWholeTotal();
                            }
                            $this.GetWholeTotal();
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
                        Common.SetPageAccess(d);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
            Common.InitDateMask();
        },

        LoadBanks: function () {
            var $this = this;
            var bankAccountId = PageSetting.Banks;
            var banks = Common.GetLeafAccounts(bankAccountId);
            var tokens = banks;
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
                    var d = $this.GetByCode(ui.item.value);
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
            //var $this = this;
            //var d = "type=" + $this.GetType();
            //Common.WrapAjax({
            //    url: "../Transaction/GetNextVoucherNumber",
            //    type: "POST",
            //    data: d,
            //    success: function (res) {
            //        var q = JSON.parse(res);
            //        if (q.Success) {
            //            $("#VoucherNumber").val(q.Data);
            //            $("#lblVoucherNumber").html(q.Data);
            //        }
            //        else {
            //            Common.ShowError(res.Error);
            //        }
            //    },
            //    error: function (e) {
            //    }
            //});
        },
        Delete: function (id) {
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
                    blockMessage: "Deleting  " + $this.GetType() + " ...please wait",
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
        GetByCode: function (code) {

            var leafaccounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel; });
            var data = $.grep(leafaccounts, function (e) { return e.AccountCode.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#FormSetting").val());
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            this.LoadAccounts();
            //$(".page-title").html(PageSetting.FormTitle + " <small> Add/Edit/Delete records</small>");
        }
    };
}();

