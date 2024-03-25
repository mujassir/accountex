
var MiscCharges = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "MiscCharges";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var focusElement = "#VoucherNumber";
    return {
        init: function () {
            var $this = this;
            $(document).on("keyup", "input.Description", function (event) {
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
                    $(this).val() == "0" ? focusElement = "date" : "code";
                    $this.LoadVoucher("same");
                }
            });
            $(document).on("keyup", ".Amount", function (event) {
                $this.GetWholeTotal();
                if (event.which == 13)
                    $this.AddItem();
            });
            $(document).on("keyup", ".ItemCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $(this).closest("tr").find("input.Date").focus().select();
                    }
                }
            });
            $(document).on("keyup", ".ShopCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $(this).closest("tr").find("input.Date").focus().select();
                    }
                }
            });
            $(document).on("keyup", ".Date", function (event) {
                if (event.which == 13) {
                    if ($(this).val() != "") {
                        $(this).closest("tr").find("input.ChargesCode").focus().select();
                    }
                }
            });
            $(document).on("keyup", ".ChargesCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val() != "") {
                        $(this).closest("tr").find("input.Description").focus().select();
                    }
                }
            });
            $(document).on("blur", ".ItemCode", function () {
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {
                    $(tr).find("input.ItemId").val(account.Id);
                    $(tr).find("input.ItemName").val(account.Name);
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $(document).on("blur", ".ShopCode", function () {
                var shop = Enumerable.From(AppData.shops).Where("$.ShopCode=='" + $(this).val() + "'").FirstOrDefault();
                var tr = $(this).closest("tr");
                if (typeof shop != "undefined" && shop != null) {
                    $(tr).find("input.ShopId").val(shop.Id);
                    $(tr).find("input.ShopCode").val(shop.ShopCode);
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $(document).on("blur", ".ChargesCode", function () {
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {
                    $(tr).find("input.ChargesId").val(account.Id);
                    $(tr).find("input.ChargesName").val(account.Name);
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $this.LoadPageSetting();
            $this.GetShop();
            $this.GetNextVoucherNumber();


        },

        SelectCurrentMonthYear: function (type) {
            var $this = this;
            var date = new Date();
            m = date.getMonth() + 1;
            y = date.getFullYear();
            $("#Month").select2("val", m);
            $("#Year").select2("val", y);
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
            focusElement = "date";
            $this.LoadVoucher("nextvouchernumber");
        },

        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        AddItem: function () {
            var $this = this;
            var accode = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode ").val();
            if (typeof accode != "undefined" && accode.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode ").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.ItemCode").focus().select();
                }, 300);
                focusElement = "";
            }
            Common.InitDateMask();
            Common.InitDatePicker();
            $this.TenantAutoCompleteInit();
            $this.ChargesAutoCompleteInit();
            //$this.ShopsAutoCompleteInit();
            Common.InitNumerics();
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        GetWholeTotal: function () {
            var amount = 0;
            $("#item-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find("input.Amount").val());
            });
            $("#item-container tfoot tr td:nth-child(2) input").val(amount);
        },
        SetValidationAndControl: function () {

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
                Common.ShowMessage(true, { message: "Record saved successfully." });
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


            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.TenantAccountCode.trim()!=''").ToArray();
                if (id > 0) {
                    var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                    var newvoucherno = Common.GetInt(record.VoucherNumber);
                    if (prevoucherno != newvoucherno) {
                        err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                        Common.ShowError(err);
                        return;
                    }
                }
                var amounttotal = $("#item-container tfoot tr td:nth-child(2) input").val();
                var err = "";
                if (amounttotal == 0) {
                    err += "Can't add empty voucher.";
                }
                for (var i in Items) {
                    var item = Items[i];
                    if (item.TenantAccountCode.trim() == "") {
                        err += "Tenant is required.,";
                    }
                    else if (item.TenantAccountCode.trim() != "") {
                        var party = Common.GetByCode(item.TenantAccountCode);
                        if (typeof party == "undefined" || party == null) {
                            err += item.TenantAccountCode + " is not valid code.,";
                        }
                    }
                    if (item.ShopCode.trim() == "")
                    {
                        err += "Shop is required,";
                    }
                    else if (item.ShopCode.trim() != "")
                    {
                        var shop = Enumerable.From(AppData.shops).Where("$.ShopCode=='" + item.ShopCode + "'").FirstOrDefault();
                        if (typeof shop == "undefined" || shop == null) {
                            err += item.ShopCode + " is not valid code.,";
                        }
                    }

                    if (item.ChargesAccountCode.trim() == "") {
                        err += "Charge is required.,";
                    }
                    else if (item.ChargesAccountCode.trim() != "") {
                        var party = Common.GetByCode(item.ChargesAccountCode);
                        if (typeof party == "undefined" || party == null) {
                            err += item.ChargesAccountCode + " is not valid code.,";
                        }
                    }
                    if (item.Amount <= 0) {
                        err += item.TenantAccountName + " must have charges amount greater than zero.,";
                    }
                }
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                record["MiscChargeItems"] = Items;
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
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        $("#item-container tbody").html("");
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#savebutton").prop("disabled", false);
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                            $this.SelectCurrentMonthYear();
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (d.Id > 0 && d.MiscChargeItems != null && d.MiscChargeItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                var html = "";
                                var items = d.MiscChargeItems;
                                Common.MapItemData(items);
                                $("#savebutton").prop("disabled", true);
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
        },
        GetShop: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetAllShops&shops=shops",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        //$this.ShopsAutoCompleteInit(res.Data);
                        //AppData.shops = res.Data;
                        AppData.shops = res.Data.Shops;
                        AppData.agreedTenants = res.Data.Tenants;
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },
        TenantAutoCompleteInit: function () {
            var $this = this;
            var tenants = Common.GetLeafAccounts(PageSetting.Tenants);
            var agreedTenants = AppData.agreedTenants;
            var filteredTenants = Enumerable.From(tenants).Join(agreedTenants,
                                "tn=>tn.Id",
                                "tnAg=>tnAg.TenantAccountId",
                                "(tn,tnAg)=>{Id:tn.Id,AccountCode:tn.AccountCode,DisplayName:tn.DisplayName,ShopId:tnAg.ShopId}"
                                ).ToArray();

            var suggestion = new Array();
            for (var i in filteredTenants) {
                var tenant = filteredTenants[i];
                suggestion.push(
                    {
                        id: tenant.Id,
                        value: tenant.AccountCode,
                        label: tenant.AccountCode + "-" + tenant.DisplayName,
                        name: tenant.DisplayName,
                        shopid: tenant.ShopId
                    }
                );
            }
            $(".ItemCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    var shop = Enumerable.From(AppData.shops).Where(function (e) { return e.Id == ui.item.shopid }).FirstOrDefault();

                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.ItemName").val(account.Name);
                        $(tr).find("input.ShopId").val(shop.Id);
                        $(tr).find("input.ShopCode").val(shop.ShopCode);
                        $(tr).find("input.ShopNo").val(shop.ShopNo);
                    }

                }
            });

        },
        ChargesAutoCompleteInit: function () {
            var $this = this;
            var charges = Common.GetLeafAccounts(PageSetting.Charges);
            var suggestion = new Array();
            for (var i in charges) {
                var charge = charges[i];
                suggestion.push(
                    {
                        id: charge.Id,
                        value: charge.AccountCode,
                        label: charge.AccountCode + "-" + charge.DisplayName,
                        name: charge.DisplayName

                    }
                );
            }
            $(".ChargesCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.ChargesId").val(account.Id);
                        $(tr).find("input.ChargesName").val(account.Name);
                    }

                }
            });

        },
        ShopsAutoCompleteInit: function () {
            var $this = this;
            var suggestions = new Array();
            var shops=AppData.shops;
            for (var i in shops) {
                var shop = shops[i];
                suggestions.push(
                    {
                        id: shop.Id,
                        value: shop.ShopCode,
                        label: shop.ShopCode
                    }
                    );
            }

            $(".ShopCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    var shop = Enumerable.From(shops).Where("$.ShopCode=='" + ui.item.value + "'").FirstOrDefault();
                    if (typeof shop != "undefined" && shop != null) {
                        $(tr).find("input.ShopId").val(shop.Id);
                        $(tr).find("input.ShopCode").val(shop.ShopCode);
                    }
                }
            });

        }

    };
}();

