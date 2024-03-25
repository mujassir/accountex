
var IrisSale = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "IrisSale";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;

            var dataTypes = ["Sale", "Purchase", "SaleReturn", "PurchaseReturn"];
            $("input[name='ac-type']").change(function () {


                //var acc = Common.GetById(PageSetting.CashAccount);
                $("#AccountId").val("");
                $("#AccountCode").val("");
                $("#AccountName").val("");
                $("#PartyAddress").val("");
                //$("#AccountCode").attr("disabled", "disabled");
                $this.MapComments();

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
            $("#InvoiceNumber").keyup(function (e) {

                if (e.which == 13) {
                    $("#Date").focus();

                }
                $this.MapComments();
            });
            $(document).on("keyup", ".SalesmanCode", function (event) {
                if (event.which == 13) {
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find("input.SalesmanId").val(account.Id);
                        $(tr).find("input.SalesmanName").val(account.Name);
                        $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code for attendee.";
                            Common.ShowError(err);
                        }
                        else
                            $(".btn.btn-primary.green").focus();
                    }
                }

            });
            $(document).on("blur", ".SalesmanCode", function () {
               
                var account = Common.GetByCode($(this).val());
                var tr = $(this).closest("tr");
                if (typeof account != "undefined" && account != null) {
                    $(tr).find("input.SalesmanId").val(account.Id);
                    $(tr).find("input.SalesmanName").val(account.Name);
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code for attendee.";
                        Common.ShowError(err);
                    }
                    else
                        $(".btn.btn-primary.green").focus();
                }
            });
            $(document).on("click", ".action i.fa-trash-o", function () {
                $this.DeleteRow($(this));

            });
            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        //$("#item-container tbody tr:nth-last-child(1) td:nth-child(3) input.Amount").focus().select();
                        $this.GetWholeTotal();
                        $this.AddItem();
                    }
                }

            });
            $(document).on("blur", ".Code", function () {

                var account = Common.GetByCode($(this).val());
                var tr = $(this).parent().parent();
                if (typeof account != "undefined" && account != null) {
                    var service = Common.GetAccountDetailByAccountId(account.Id);
                    $(tr).find("input.ItemId").val(service.AccountId);
                    $(tr).find("input.Name").val(account.Name);
                    $(tr).find("input.Amount").val(Common.GetFloat(service.Rate));
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code for service.";
                        Common.ShowError(err);
                    }
                }
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.SalesmanCode").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid party code.";
                            Common.ShowError(err);
                        }
                    }

                }
            });
            $("#AccountCode").blur(function () {

                var party = Common.GetByCode($(this).val());

                if (typeof party != "undefined" && party != null) {
                    $("#AccountId").val(party.Id);
                    $this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
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
            $("#Discount").keyup(function (e) {
                var discount = Common.GetInt($("#Discount").val());
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                if (discount < grossTotal) {
                    var percent = Common.GetFloat((discount * 100) / grossTotal);
                    $("#DiscountPercent").val(percent.toFixed(2));
                }
                else {
                    $("#Discount").val(grossTotal);
                    $("#DiscountPercent").val(100);
                }
                //$("#DiscountPercent").val(0)
                $this.GetNetTotal();
                //if(e.which==13)
                //    $(".btn.btn-primary.green").focus();
            });
            $("#DiscountPercent").keyup(function (e) {
                var percent = Common.GetFloat($("#DiscountPercent").val());
                var grossTotal = Common.GetFloat($("#GrossTotal").val());
                var discount = Common.GetInt((grossTotal * percent) / 100);
                $("#Discount").val(discount);
                $this.GetNetTotal();
            });

            //$("#Discount").keydown(function (e) {
            //    //$("#DiscountPercent").val("0");
            //    $this.GetNetTotal();
            //    //if (e.which == 13) 
            //    //    $(".btn.btn-primary.green").focus();
            //});
            //$("#DiscountPercent").keyup(function (e) {

            //    var percent = Common.GetFloat($("#DiscountPercent").val());
            //    var grossTotal = Common.GetFloat($("#GrossTotal").val());
            //    var discount = Common.GetInt((grossTotal * percent) / 100);
            //    $("#Discount").val(discount);
            //    $this.GetNetTotal();
            //});

            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            }
            this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.SalesmanAutoCompleteInit();
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
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
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        ChangeType: function (type) {
            var $this = this;
            window.history.pushState(type, document.title + " | " + type, "index?type=" + type);
            //document.title = document.title + " | " + type;
            $this.LoadPageSetting();
            if ($("#div-table").is(":visible")) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            } else if ($("#form-info").is(":visible")) {
                $this.Add();
            }
            Common.HighlightMenu();
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {

            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) input.SalesmanCode").val();
            var code = $("#item-container tbody tr:nth-last-child(1)  input.SalesmanCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.SalesmanCode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            Common.InitNumerics();
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);
                
            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.SalesmanCode").focus().select();
                }, 300);
                focusElement = "";
            }
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            $this.AutoCompleteInit();
            $this.SalesmanAutoCompleteInit();
        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parents("tr").remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "booknumber";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });

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
            record["CashSale"] = $("input[value='cash']").is(":checked");
            var Items = new Array();
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + "),";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += $("#AccountCode").val() + " is not valid party code.,";
                }
                for (var i in Items) {
                    var item = Items[i];
                    var salesman = Common.GetByCode(item.SalesmanCode);
                    if (typeof salesman == "undefined" || salesman == null) {
                        err += item.SalesmanCode + " is not valid code.,";
                    }
                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.,";
                    }
                }

                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
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

                            $this.LoadReportData(res);
                            setTimeout(function () {
                                $this.Print();
                            }, 500);

                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        GetWholeTotal: function () {
            var $this = this;
            var Price = 0;
            $("#item-container tbody tr").each(function () {
                Price += Common.GetInt($(this).find("input.Amount").val());
            });
            $("#item-container tfoot tr").find("input.Amount").val(Price);

            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            $this.CalculatePreviousBalance();
            this.GetNetTotal();
        },
        //GetNetTotal: function () {
        //    var total = Common.GetFloat($("#GrossTotal").val());
        //    var discount = Common.GetInt($("#Discount").val());
        //    var nettotal = Common.GetInt(total - discount);
        //    $("#NetTotal").val(nettotal);
        //    $("#lblcurrentbalance").html(nettotal);
        //},
        GetNetTotal: function () {
            var total = Common.GetFloat($("#GrossTotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);
            if (nettotal >= 0) {
                $("#NetTotal").val(nettotal);
                $("#lblcurrentbalance").html(nettotal);
            }
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
            $("#lblDate").html(moment(d.Date).format("DD-MM-YYYY"));
            $("#lblTime").html(moment(d.CreatedDate).format("hh:mm:ss"));
            var type = $this.GetType();
            var html = "";
            var items = d.SaleItems;
            var index = 1;
            $("#lblVoucherNumber").text(d.VoucherNumber);
            $("#lblAccountName").html(d.AccountName);
            //$("#lblDate").text(d.Data);
            $("#lblDate").html((d.Date ? moment(d.Date).format("Do MMM YYYY") : ''));
            //moment().format('MMMM Do YYYY, h:mm:ss a');
            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td class='align-right'>" + item.Amount + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

            if (res.Data.AgingItems != null && res.Data.AgingItems.length > 0) {
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
            }
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
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
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

                    var d = Common.GetByCode(ui.item.value);
                    var type = $this.GetType();
                    if (typeof d != "undefined" && d != null) {
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
        },
        Print: function () {
            window.print();
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

            $(".SalesmanCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var salesman = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof salesman != "undefined" && salesman != null) {
                        $(tr).find("input.SalesmanId").val(salesman.Id);
                        $(tr).find("input.SalesmanName").val(salesman.Name);
                        $(".container-message").hide();
                    }
                    //$(tr).find("input.Code").focus();
                }
            });



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
                        label: service.AccountCode + "-" + service.DisplayName,
                        name: service.DisplayName

                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var service = Common.GetAccountDetailByAccountId(ui.item.id);
                    var tr = $(this).closest("tr");
                    if (typeof service != "undefined" && service != null) {
                        $(tr).find("input.ItemId").val(ui.item.id);
                        $(tr).find("input.Name").val(ui.item.name);
                        $(tr).find("input.Amount").val(Common.GetFloat(service.Rate));
                        $(".container-message").hide();
                        $this.GetWholeTotal();
                    }
                    //$(tr).find("input.Quantity").focus();
                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                }
            });

        },

    };
}();