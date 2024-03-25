
var DiscountTransaction = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "DiscountTransaction";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    return {
        init: function () {
           var $this = this;
            //$("ul.transaction.sub-menu li a").click(function () {
            //    var type = $(this).attr("data-type");
            //    $this.ChangeType(type);
            //    return false;
            //});
            var dataTypes = ["Sale", "Purchase", "SaleReturn", "PurchaseReturn"];
            for (var i = 0; i < dataTypes.length; i++) {
                $("a[data-type='" + dataTypes[i] + "']").click(function () {
                    var type = $(this).attr("data-type");
                    $this.ChangeType(type);
                    return false;
                });
            }

            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            $("#qty").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $("#Rate").focus();
            });
            $("#Rate").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $this.AddItem();
            });
            $("#AccountId").change(function () {
                $("#Comments").val("Sold To: " + $("#AccountId option:selected").text());
                $this.GetCustomerProducts();
                var address = $("option:selected", $(this)).attr("data-address");
                if (typeof address != "undefined" && address != "null")
                    $("#PartyAddress").val(address);
            });

            $("#Item").change(function () {
                $this.GetProductDetail();
            });



            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            }
            else {
                this.ListView();
            }
            this.LoadPageSetting();
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        ChangeType: function (type) {
           var $this = this;
            window.history.pushState(type, document.title + " | " + type, "index?type=" + type);
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
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            Common.Clear();
            this.DetailView();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        ListView: function () {
           var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (LIST_LOADED) {
                if (LIST_CHANGED) {
                    this.RebindData();
                    LIST_CHANGED = false;
                }
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        AddItem: function () {
            if (Common.Validate($("#addrow"))) {
                var qty = $("#qty").val();
                var rate = $("#Rate").val();
                var amount = $("#Amount").val();
                var discount = $("#DiscountPercent").val();
                var discountAmount = Common.GetInt(amount) * discount / 100;
                var IncAmount = Common.GetInt(amount) - discountAmount;

                var item = $("#Item").val();
                var itemtext = $("#Item option:Selected").text();
                var html = "<tr>";
                html += "<td><input type='hidden' id='ItemId' value='" + item + "'>";
                html += "<input type='hidden' id='Id' value=''>";
                html += "" + itemtext + "</td>";
                html += "<td>" + qty + "</td>";
                html += "<td>" + rate + "</td>";
                html += "<td>" + amount + "</td>";
                html += "<td>" + discountAmount + "</td>";
                html += "<td>" + IncAmount + "</td>";
                html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"DiscountTransaction.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                html += "</tr>";
                $("#saleitem tbody").append(html);
                $("#qty,#Rate,#Amount").val("");
                $("#lbldiscount").html("0 %");
                this.GetWholeTotal();
                $("#Item").focus();
            }
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();
        },
        DeleteRow: function (elment) {
            $(elment).parent().parent().parent().remove();
            this.GetWholeTotal();
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
                $this.Add();
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
        SaveRecord: function (callback) {

           var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            if (id > 0)
                mode = "update";

            var Items = new Array();

            if (Common.Validate($("#mainform"))) {
                var Currentsetting = PageSetting.ItemAccTr;
                $("#saleitem tbody tr").each(function () {
                    var qty = Common.GetInt($(this).children(":nth-child(2)").text());
                    var price = Common.GetInt($(this).children(":nth-child(3)").text());
                    var productdiscount = Common.GetInt($(this).children(":nth-child(5)").text());
                    var amount = Common.GetInt($(this).children(":nth-child(6)").text());
                    Items.push({
                        InvoiceNumber: $("#InvoiceNumber").val(),
                        VoucherNumber: $("#VoucherNumber").val(),
                        Date: Common.ChangeDateFormate($("#Date").val()),
                        Quantity: qty,
                        Price: price,
                        Id: $(this).children(":nth-child(1)").children("#Id").val(),
                        TransactionType: VoucherType[voucher],
                        EntryType: EntryType.Item,
                        AccountId: $(this).children(":nth-child(1)").children("#ItemId").val(),
                        AccountTitle: $(this).children(":nth-child(1)").text(),
                        Discount: productdiscount,
                        Credit: voucher == "sale" || voucher == "purchasereturn" ? amount : null,
                        Debit: voucher == "salereturn" || voucher == "purchase" ? amount : null
                    });
                });
                var disocunt = Common.GetInt($("#Discount").val());
                if (disocunt > 0) {
                    Currentsetting = PageSetting.DiscountTr;
                    var disuocuntentery = Common.SetValue();
                    disuocuntentery["AccountId"] = PageSetting.DiscountAccountId;
                    disuocuntentery["AccountTitle"] = PageSetting.DiscountAccountTitle;
                    disuocuntentery["Quantity"] = 1;
                    disuocuntentery["Price"] = Common.GetInt($("#Discount").val());
                    disuocuntentery["EntryType"] = EntryType.Discount;
                    disuocuntentery["TransactionType"] = VoucherType[voucher];
                    disuocuntentery["Credit"] = voucher == "salereturn" || voucher == "purchase" ? Common.GetInt($("#Discount").val()) : null;
                    disuocuntentery["Debit"] = voucher == "sale" || voucher == "purchasereturn" ? Common.GetInt($("#Discount").val()) : null;
                    Items.push(disuocuntentery);
                }
                Currentsetting = PageSetting.MasterAccTr;
                record = new Object();

                record = Common.SetValue($("#form-info"));
                record["AccountTitle"] = $("#AccountId option:selected").text();
                record["TransactionType"] = VoucherType[voucher];
                record["EntryType"] = EntryType.MasterDetail;
                record["Quantity"] = 1;
                record["Price"] = Common.GetInt($("#Nettotal").val());
                record["Credit"] = voucher == "salereturn" || voucher == "purchase" ? Common.GetInt($("#Nettotal").val()) : null;
                record["Debit"] = voucher == "sale" || voucher == "purchasereturn" ? Common.GetInt($("#Nettotal").val()) : null;
                record["Items"] = Items;
                //console.log(JSON.stringify(record));
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/?mode=" + mode,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving discount transaction ...please wait",
                    success: function (res) {
                        if (res.Success) {
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
        GetQuantityPriceTotal: function () {
            var Quantity = 0;
            var Rate = 0;
            Quantity = Common.GetInt($("#qty").val());
            Rate = Common.GetInt($("#Rate").val());
            $("#Amount").val(Quantity * Rate);

        },
        GetWholeTotal: function () {
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            $("#saleitem tbody tr").each(function () {
                Quantity += Common.GetInt($(this).children(":nth-child(2)").text());
                Price += Common.GetInt($(this).children(":nth-child(4)").text());
                discount += Common.GetInt($(this).children(":nth-child(5)").text());
            });
            $("#qtytotal").val(Quantity);
            if (Price === 0)
                $("#amounttotal").val("");
            else
                $("#amounttotal").val(Price);
            var IncAmount = Price - discount;
            $("#Discount").val(discount);
            $("#Nettotal").val(IncAmount + "");
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("#saleitem tbody").html("");
        },
        Edit: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading discount transaction ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.DetailView();
                        $this.GetCustomerProducts();
                        var allsale = res.Data;
                        var masterdetail = $.grep(allsale, function (e) { return e.EntryType == EntryType.MasterDetail; })[0];
                        var items = $.grep(allsale, function (e) { return e.EntryType == EntryType.Item; });
                        var discount = $.grep(allsale, function (e) { return e.EntryType == EntryType.Discount; });
                        for (var prop in masterdetail) {
                            $("#" + prop).val(masterdetail[prop]);
                        }
                        Common.SetDate("#Date", masterdetail.Date);
                        $("#lblVoucherNumber").html(masterdetail.VoucherNumber);
                        //$("#ExpectedDate").val(GetFormatedDate(res.Data.ExpectedDate));
                        //$("#PODate").val(GetFormatedDate(res.Data.PODate));
                        //$('#AccountId').trigger("chosen:updated");
                        $("#AccountId").select2("val", masterdetail.AccountId);
                        if (discount != null && discount.length) {
                            discount = discount[0];
                            $("#Discount").val(discount.Price);
                        }
                        $("#saleitem tbody").html("");
                        var html = "";
                        for (var i in items) {
                            var item = items[i];
                            var account = $.grep(PageData.ItemAccounts, function (e) { return e.Id == item.AccountId; })[0];
                            var amount = Common.GetInt(item.Credit) > 0 ? Common.GetInt(item.Credit) : Common.GetInt(item.Debit);
                            var discountamount = Common.GetInt(item.Discount);
                            var netAmount = amount + discountamount;
                            html += "<tr>";
                            html += "<td><input type='hidden' id='ItemId' value='" + item.AccountId + "'>";
                            html += "<input type='hidden' id='Id' value='" + item.Id + "'>";
                            html += "" + item.Account + "</td>";
                            html += "<td>" + item.Quantity + "</td>";
                            html += "<td>" + item.Price + "</td>";
                            html += "<td>" + netAmount + "</td>";
                            html += "<td>" + discountamount + "</td>";
                            html += "<td>" + amount + "</td>";
                            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"DiscountTransaction.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                            html += "</tr>";
                        }
                        $("#saleitem tbody").html(html);

                        $this.ReinializePlugin();
                        $this.GetWholeTotal();
                        //window.history.pushState("", "EditPO", window.location.href + "/edit/" + id);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetCustomerProducts: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?type=" + $this.GetType() + "&key=GetCustomerProducts&customer=" + $("#AccountId").val(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var html = "";
                        var products = res.Data;
                        html += "<option></option>";
                        for (var i = 0; i < products.length; i++) {
                            var product = products[i];
                            html += "<option data-detail='" + JSON.stringify(product) + "' value='" + product.COAProductId + "'>" + product.ProductCode + "-" + product.ProductTitle + "</option>";
                        }
                        $("#Item").html(html).select2();

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
            var d = "type=" + Common.GetQueryStringValue("type").toLowerCase();
            Common.WrapAjax({
                url: "../Transaction/GetNextVoucherNumber",
                type: "POST",
                data: d,
                success: function (res) {
                    var q = JSON.parse(res);
                    if (q.Success) {
                        $("#VoucherNumber,#InvoiceNumber").val(q.Data);
                        $("#lblVoucherNumber").html(q.Data);
                    }
                    else {
                        Common.ShowError(q.Data);
                    }
                },
                error: function (e) {
                }
            });
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
        Delete: function (id) {
           var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting discount transaction ...please wait",
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
        ShowAttachments: function (el) {
            // MEDIA_ELEMENT = el;
            $("#dialogAttachments").addClass("in");
        },
        LoadAccounts: function () {
           var $this = this;
            var qs = "key=GetBothLeafAccounts";
            qs += "&MasterAccountId=" + PageSetting.MasterAccountId;
            qs += "&ItemAccountId=" + PageSetting.ItemAccountId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/" + "?" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        PageData.MasterAccounts = res.Data.MasterAccounts;
                        PageData.ItemAccounts = res.Data.ItemAccounts;
                        Common.BindSelectWithAddress(res.Data.MasterAccounts, "#AccountId", true);
                        //Common.BindSelect(res.Data.ItemAccounts, '#Item', false);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#FormSetting").val());
            var tokens = $.grep(formSetting, function (e) { return e.VoucherType == voucher });
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            this.LoadAccounts();
            //$(".page-title").html(PageSetting.FormTitle + " <small> Add/Edit/Delete records</small>");
        }
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};