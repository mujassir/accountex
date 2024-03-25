
var GstTransaction = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "GstTransaction";
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
            //var dataTypes = ["Sale", "Purchase", "SaleReturn", "PurchaseReturn"];
            //for (var i = 0; i < dataTypes.length; i++) {
            //    $("a[data-type='" + dataTypes[i] + "']").click(function () {
            //        var type = $(this).attr("data-type");
            //        $this.ChangeType(type);
            //        return false;
            //    });
            //}
            $("#AccountId").change(function () {
                $("#Comments").val("Sold To: " + $("#AccountId option:selected").text());
            });
            $("#Discount").keyup(function () {
                $this.GetNetTotal();
            });
            $("#qty").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $("#Rate").focus();
            });

            $("#GSTPercent").keyup(function (e) {
                var qty = Common.GetFloat($("#qty").val());
                var rate = Common.GetFloat($("#Rate").val());
                var amount = qty * rate;
                var gst = Common.GetFloat($("#GSTPercent").val());
                var gstAmount = Common.GetFloat(amount) * gst / 100;
                var IncAmount = Common.GetFloat(amount) + gstAmount;
                $("#GSTAmount").val(gstAmount);
                $("#Amount").val(IncAmount);
            });

            $("#Rate").keyup(function (e) {
                $this.GetQuantityPriceTotal();
                if (e.keyCode == 13)
                    $this.AddItem();
            });


            this.LoadPageSetting();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                this.Add(false);
                $this.Edit(Common.GetInt(Url.voucherno), true);
            }
            else {
                if (Setting.PageLandingView == "DetailView") {
                    this.Add(true);
                }
                else {
                    this.ListView();
                }
            }
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
                $this.Add(true);
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
        Add: function (LoadNextVoucher) {
            Common.Clear();
            this.DetailView();
            this.CustomClear();
            if (LoadNextVoucher)
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
                var qty = Common.GetFloat($("#qty").val());
                var rate = Common.GetFloat($("#Rate").val());
                var amount = qty * rate;
                var gst = Common.GetFloat($("#GSTPercent").val());
                var gstAmount = Common.GetFloat(amount) * gst / 100;
                var IncAmount = Common.GetFloat(amount) + gstAmount;

                var item = $("#Item").val();
                var itemtext = $("#Item option:Selected").text();
                var code = $("#Item option:Selected").attr("data-code");
                var name = $("#Item option:Selected").attr("data-name");
                var html = "<tr>";
                html += "<td><input type='hidden' id='ItemId' value='" + item + "'>";
                html += "<input type='hidden' id='Id' value=''>";
                html += "<input type='hidden' id='Code' value='" + code + "'>";
                html += "<input type='hidden' id='Name' value='" + name + "'>";
                html += "" + itemtext + "</td>";
                html += "<td class='align-right'>" + qty + "</td>";
                html += "<td class='align-right'>" + rate + "</td>";
                html += "<td class='align-right'>" + amount + "</td>";
                html += "<td>" + gst + "</td>";
                html += "<td class='align-right'>" + gstAmount + "</td>";
                html += "<td class='align-right'>" + IncAmount + "</td>";
                html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"GstTransaction.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                html += "</tr>";
                $("#saleitem tbody").append(html);
                Common.InitNumerics();
                $("#qty,#Rate,#Amount,#GSTPercent,#GSTAmount").val("");
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
                $this.Add(true);
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
                var scope = $("#mainform");
                var record = Common.SetValue($("#form-info"));

                var Currentsetting = PageSetting.ItemAccTr;
                $("#saleitem tbody tr").each(function () {
                    var qty = Common.GetFloat($(this).children(":nth-child(2)").text());
                    var price = Common.GetFloat($(this).children(":nth-child(3)").text());
                    var gstpercent = Common.GetFloat($(this).children(":nth-child(5)").text());
                    var gstamount = Common.GetFloat($(this).children(":nth-child(6)").text());
                    var amount = Common.GetFloat($(this).children(":nth-child(4)").text());
                    var itemtext = $(this).children(":nth-child(1)").text();
                    var netamount = Common.GetFloat($(this).children(":nth-child(7)").text());
                    Items.push({
                        SaleId: Common.GetInt(record.Id),
                        InvoiceNumber: $("#InvoiceNumber").val(),
                        VoucherNumber: $("#VoucherNumber").val(),
                        Date: Common.ChangeDateFormate($("#Date").val()),
                        Quantity: qty,
                        Rate: price,
                        Amount: amount,
                        GSTAmount: gstamount,
                        GSTPercent: gstpercent,
                        NetAmount: netamount,
                        Id: $(this).children(":nth-child(1)").children("#Id").val(),
                        TransactionType: VoucherType[voucher],
                        EntryType: EntryType.Item,
                        ItemCode: $(this).children(":nth-child(1)").children("#Code").val(),
                        ItemName: $(this).children(":nth-child(1)").children("#Name").val(),
                        ItemId: $(this).children(":nth-child(1)").children("#ItemId").val(),
                    });
                });
                var code = $("#AccountId option:Selected").attr("data-code");
                var name = $("#AccountId option:Selected").attr("data-name");
                record["TransactionType"] = VoucherType[voucher],
                record["AccountCode"] = code,
                record["AccountName"] = name,
                record["SaleItems"] = Items;
                LIST_CHANGED = true;
                $.ajax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/?mode=" + mode,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + "  GstTransaction ...please wait",
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
            var gst = 0;
            $("#saleitem tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).children(":nth-child(2)").text());
                Price += Common.GetFloat($(this).children(":nth-child(4)").text());
                gst += Common.GetFloat($(this).children(":nth-child(6)").text());
            });
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var IncAmount = Price + gst;
            $("#GST").val(gst);
            $("#NetTotal").val(IncAmount + "");
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
        Edit: function (id, loadwithvoucher) {
           var $this = this;
            if (typeof loadwithvoucher == undefined || loadwithvoucher == null)
                loadwithvoucher = false;
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?editid=" + id + "&type=" + $this.GetType() + "&loadwithvoucherno=" + loadwithvoucher,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading  " + $this.GetType() + "  GstTransaction ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $this.DetailView();
                        var d = res.Data;
                        if (d == null) {
                            Common.ShowError("No Voucher Found.");
                            return;
                        }
                        Common.MapEditData(d, "#form-info");
                        $(".date-picker,.ac-date").each(function () {

                            Common.SetDate(this, d[$(this).attr("Id")]);
                        });
                        var items = d.SaleItems;
                        $("#lblVoucherNumber").html(d.VoucherNumber);
                        $("#saleitem tbody").html("");
                        var html = "";
                        for (var i in items) {
                            var item = items[i];


                            html += "<tr>";
                            html += "<td><input type='hidden' id='ItemId' value='" + item.ItemId + "'>";
                            html += "<input type='hidden' id='Id' value='" + item.Id + "'>";
                            html += "<input type='hidden' id='Code' value='" + item.ItemCode + "'>";
                            html += "<input type='hidden' id='Name' value='" + item.ItemName + "'>";
                            html += "" + item.ItemCode + "-" + item.ItemName + "</td>";
                            html += "<td class='align-right'>" + item.Quantity + "</td>";
                            html += "<td class='align-right'>" + item.Rate + "</td>";
                            html += "<td class='align-right'>" + item.Amount + "</td>";
                            html += "<td>" + item.GSTPercent + "</td>";
                            html += "<td class='align-right'>" + item.GSTAmount + "</td>";
                            html += "<td class='align-right'>" + item.NetAmount + "</td>";
                            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"GstTransaction.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                            html += "</tr>";
                        }
                        $("#saleitem tbody").html(html);
                        Common.InitNumerics();
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
        GetNextVoucherNumber: function () {
           var $this = this;
            var voucherno = 0;
            var type = VoucherType[$this.GetType()];
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&key=nextvouchernumber&voucher=0",
                type: "GET",
                success: function (res) {

                    if (res.Success) {
                        $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        $("#lblVoucherNumber").html(res.Data.VoucherNumber);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        DeleteMultiple: function (id) {
            Common.ConfirmDelete(function () {
                $.ajax({
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
            var type = VoucherType[$this.GetType()];
            Common.ConfirmDelete(function () {
                $.ajax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + type,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting  " + $this.GetType() + "  GstTransaction ...please wait",
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
        LoadAccounts: function (callback) {
           var $this = this;
            
            $this.BindSelect(PageSetting.AccountDetails.Customers, "#AccountId", true);
            $this.BindSelect(PageSetting.AccountDetails.ItemAccounts, "#Item", false);

        },
        BindSelect: function (accounts, element, addBlankRow) {
           var $this = this;
            var html = "";
            if (addBlankRow)
                html += "<option></option>";
            for (var i = 0; i < accounts.length; i++) {
                var token = accounts[i];
                html += "<option data-code='" + token.Code + "' data-name='" + token.Title + "' data-address='" + token.Address + "' value='" + token.Id + "'>" + token.Name + "</option>";
            }
            $(element).html(html).select2();
        },
        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#FormSetting").val());
            var tokens = $.grep(formSetting, function (e) { return e.VoucherType == voucher; });
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            PageSetting["AccountDetails"] = $.parseJSON($("#AccountDetails").val());
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