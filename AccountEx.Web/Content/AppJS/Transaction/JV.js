
var JV = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VoucherTrans";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var SetFocus = "voucher";
    var ALLOWFINALIZATION = false;
    var BRANCHID = 0;
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
            BRANCHID = $("#BranchId").val();
            $("#AuthLocationId").change(function (e) {
                $this.LoadVoucher("nextvouchernumber");
            });
            $("#BranchId").change(function () {
                LIST_LOADED = false;
                BRANCHID = $("#BranchId").val();
                //DataTable.DestroyDatatable(DATATABLE_ID);
                //$this.init();
            });
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
                    $this.AddItem($(this));
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
            $this.ListView();
            $this.SetPrintCheck();
            $this.MapJsonData();
            $this.SetFormLabel();
            $this.LoadPageSetting();
            if (VoucherType[$this.GetType()] == VoucherType.transfervoucher && PageSetting.IsJvAllowFinalization) {
                ALLOWFINALIZATION = true;
            }
            var element = $("#qty,#Rate");
            $(element).keyup(function () {
                GetQuantityPriceTotal();
            });

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
        Clone: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber", true);
        },
        SetPrintCheck: function () {
            $("#PrintVoucher").prop("checked", localStorage.PrintVoucher == "true" ? true : false);
            $.uniform.update("#PrintVoucher");
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        AddItem: function ($element) {
            var $this = this;

            var rowPosition = Common.GetBool($("#newrownPosition").val());
            var $tr = $("#item-container tbody tr:nth-last-child(1)");
            if ($element != null && $element != undefined && rowPosition)
                $tr = $element.closest("tr");
            var accode = $tr.find("input.AccountCode ").val();
            var description = $tr.find("input.Description").val();


            if (typeof accode != "undefined" && accode.trim() == "") {
                setTimeout(function () {
                    $tr.find("input.AccountCode ").focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }
            var $newTr = $($("#template-item").html());
            $newTr.insertAfter($tr);
            if ($("#item-container tbody tr").length <= 0)
                $("#item-container tbody").append($newTr);
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
                    $newTr.find("input.AccountCode").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            $("#Item").focus();
            $newTr.find("input.Description").val(description);
            //}

            $this.LoadAccounts();
            Common.InitNumerics();
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
        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?name=name",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading Promotions...please wait",
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
        GetWholeTotal: function () {

            var Quantity = 0;
            var debit = 0;
            var credit = 0;
            $("#item-container tbody tr").each(function () {
                debit += Common.GetFloat($(this).find(":nth-child(4) input.Debit").val());
                credit += Common.GetFloat($(this).find(":nth-child(5) input.Credit ").val());

            });
            $("#item-container tfoot tr td:nth-child(2) input").val(debit);
            $("#item-container tfoot tr td:nth-child(3) input").val(credit);
            $("#NetTotal").val(debit);

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
                    var ac = $this.GetByCode(ui.item.value);

                    var tr = $(this).parent().parent();
                    if (typeof ac != "undefined" && ac != null) {

                        $(tr).find(":nth-child(1) input.AccountId").val(ac.Id);
                        $(tr).find(":nth-child(2) input.AccountName").val(ac.DisplayName);
                        $(tr).find(":nth-child(3) input.AccountName").focus();

                    }
                }
            });



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
            $('#AuthLocationId').trigger('change');
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
            //$("#form-info,#div-report").addClass("hide");
            //$("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + VoucherType.transfervoucher + "&branchId=" + BRANCHID;
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
                Common.ShowMessage(true, { message: "Record saved successfully." });
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
        Final: function (isfinal) {
            var $this = this;
            $(".container-message").hide();
            $(".container-message").hide();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "VoucherTrans?allVoucher=true" + "&isfinal=" + isfinal,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Processing...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Record Saved successfully." });
                        SetFocus = "date";
                        if (isfinal) {
                            $(".btnUnFinal").removeClass("hide");
                            $("#btnsaverecord,#btndelete,.btnFinal").addClass("hide");
                        }
                        else {
                            $("#btnsaverecord,#btndelete,.btnFinal").removeClass("hide");
                            $(".btnUnFinal").addClass("hide");
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


            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.AccountCode.trim()!=''").ToArray();
                if (id > 0) {
                    var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                    var newvoucherno = Common.GetInt(record.VoucherNumber);
                    if (prevoucherno != newvoucherno) {
                        err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                        Common.ShowError(err);
                        return;
                    }
                }
                var debittotal = $("#item-container tfoot tr td:nth-child(2) input").val();
                var credittotal = $("#item-container tfoot tr td:nth-child(3) input").val();
                var err = "";
                if (debittotal == 0 || credittotal == 0) {
                    err += "Can't add empty voucher.";
                }
                if (debittotal != credittotal) {
                    err += "Voucher is not balance.";
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
                    }
                }
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                Items = $.grep(Items, function (element) {

                    return element.AccountCode.trim() != "";
                });
                record["BranchId"] = BRANCHID;
                record["TransactionType"] = VoucherType[voucher],
                record["VoucherItems"] = Items;
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
        CustomClear: function () {
            Common.Clear();
            $("#btndelete,#btnprint").prop("disabled", true);
            $("#item-container tbody").html("");
            $(".btnUnFinal,.btnFinal").addClass("hide");
        },
        //Edit: function (id) {
        //    var $this = this;
        //    Common.WrapAjax({
        //        url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType(),
        //        type: "GET",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (res) {
        //            if (res.Success) {
        //                var data = $.parseJSON(res.Data);
        //                $("#form-info").removeClass("hide");
        //                $("#div-table").addClass("hide");

        //                var masterdetail = $.grep(data, function (e) { return e.EntryType == EntryType.MasterDetail })[0];
        //                var item = $.grep(data, function (e) { return e.EntryType == EntryType.Item })[0];
        //                for (var prop in masterdetail) {
        //                    $("#" + prop).val(masterdetail[prop]);
        //                }
        //                Common.SetDate("#Date", masterdetail.Date);
        //                $("#lblVoucherNumber").html(masterdetail.VoucherNumber);
        //                $("#AccountIdFrom").val(masterdetail.AccountId);
        //                $("#AccountIdTo").val(item.AccountId);
        //                var amount = parseFloat(masterdetail.Credit) + parseFloat(masterdetail.Debit);
        //                $("#Amount").val(amount);

        //                $("select", "#form-info").each(function () {
        //                    $(this).select2();
        //                });
        //                $this.DetailView();

        //            }
        //            else {
        //                Common.ShowError(res.Error);
        //            }

        //        },
        //        error: function (e) {
        //        }
        //    });
        //},


        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            if (d == null)
                return;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            var type = $this.GetType();
            var html = "";
            var items = d.VoucherItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            Common.MapItemData(items, "#report-saleitem", "#template-item-print", true);
            var debit = $("#Debit").val();
            var credit = $("#Credit").val();
            $("#lblDebit").html(debit);
            $("#lblCredit").html(credit);

            if (PageSetting.AmountInWordType == AmountInWordType.Million)
                wordAmount = Common.WordsConversionInMillion(debit);
            else
                wordAmount = Common.WordsConversion(debit);

            $("#lblAmountInWords").html(wordAmount);
        },


        LoadVoucher: function (key, isClone) {
            var $this = this;
            var locationId = $("#AuthLocationId").val();
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno + "&locationId=" + locationId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {



                        if (isClone != null && isClone != undefined) {
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                            $("#Id").val(0);
                            $("#btndelete,#btnprint").prop("disabled", false);
                            $(".btnFinal,.btnUnFinal").addClass("hide");
                            $("#item-container tbody").find("input.Id").val(0);
                            $("#btndelete,#btnprint").prop("disabled", true);
                        }
                        else {
                            $("#item-container tbody").html("");
                            $this.CustomClear();
                            $("#item-container tbody").html("");
                            var d = res.Data.Order;
                            var defaultLocationId = d?.AuthLocationId || 0;
                            if (!d?.AuthLocationId) {
                                defaultLocationId = $("#AuthLocationId")?.val()
                            }
                            Common.MapEditData(d, "#form-info");
                            $(`#AuthLocationId`).val(defaultLocationId)
                            if (d == null) {
                                $this.CustomClear();
                                $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            }
                            else {
                                $("#PreVoucherNumber").val(d.VoucherNumber);
                                $("#BranchId").select2("val", d.BranchId);
                                $(".date-picker,.ac-date").each(function () {
                                    Common.SetDate(this, d[$(this).attr("Id")]);
                                });
                                if (d.VoucherCode) {
                                    $(`#AuthLocationId`).val(d.AuthLocationId)
                                }
                                if (d.Id > 0 && d.VoucherItems != null && d.VoucherItems.length > 0) {
                                    $(".btnClone").removeClass("hide");
                                    if (d.IsFinal && ALLOWFINALIZATION) {
                                        $("#btnsaverecord,.btnFinal,#btndelete").addClass("hide");
                                        $(".btnUnFinal").removeClass("hide");
                                    }
                                    else {
                                        $(".btnFinal").removeClass("hide");
                                        $(".btnUnFinal").addClass("hide");
                                    }
                                    $("#btndelete,#btnprint").prop("disabled", false);
                                    var html = "";
                                    var items = d.VoucherItems;
                                    Common.MapItemData(items);
                                    $this.GetWholeTotal();

                                }
                                $this.GetWholeTotal();
                                $this.LoadReportData(res);
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
                        Common.SetPageAccess(d);

                    } else {
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
            var locationId = Common.GetInt($("#AuthLocationId").val());
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno + "&locationId=" + locationId;
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

