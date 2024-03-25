
var UltratechPurchase = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "UltratechPurchase";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                }
            });
            $("#Date").keyup(function (e) {
                var type = $this.GetType();
                if (e.which == 13) {
                    if (type != "gstpurchase")
                        $("#AccountCode").focus();
                    else
                        $("#DCNo").focus();
                }
            });
            $("#DCNo").keyup(function (e) {
                if (e.which == 13) {
                    if ($(this).val() == "" || $(this).val() <= 0)
                        $("#AccountCode").focus();
                    else
                        $this.LoadDeliveryChallan("challan");
                }
                $this.MapComments();
            });
            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
                if (e.which == 13) {
                    $("#Date").focus();
                }
            });
            $(document).on("keyup", ".ItemCode", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        if (PageSetting.BarCodeEnabled) {
                            var product = Common.GetByBarCode($(this).val());
                            var tr = $(this).closest("tr");
                            if (typeof product != "undefined" && product != null) {
                                var account = Common.GetById(product.AccountId);
                                $(this).val(account.AccountCode);
                                $(tr).find("input.ItemId").val(product.AccountId);
                                $(tr).find("input.ItemName").val(account.Name);
                                $(tr).find("input.Quantity").val(1);
                                $(tr).find("input.Rate").val(product.PurchasePrice);
                                $(".container-message").hide();
                                $this.GetQuantityPriceTotal(tr);
                                $this.AddItem();
                            }
                            else {
                                if ($(this).val().trim() != "") {
                                    var err = $(this).val() + " is not valid code.";
                                    Common.ShowError(err);
                                    $(this).focus();
                                }
                            }
                        }
                        else {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity").focus().select();
                        }
                    }
                    else {
                        $(".btn.btn-primary.green").focus();
                    }

                }

            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode").focus().select();
                        }, 500);
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
            $(document).on("blur", ".ItemCode", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Id.Name);
                        $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        //$(tr).find("input.Unit ").val(Common.GetFloat(product.Unit));
                        $(tr).find("input.Rate").val(Common.GetFloat(product.PurchasePrice));
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.";
                            Common.ShowError(err);
                            $(this).focus();
                        }
                    }
                }
            });
            $(document).on("blur", ".Quantity", function (event) {
                var tr = $(this).closest("tr");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    $(tr).find(":nth-child(3) input.Quantity").val("1");

                }
                $this.GetQuantityPriceTotal(tr);

            });
            $(document).on("keyup", ".Rate,.AIT,.CD ,.RD ,.SED ,.Freight ,.Fwd", function (event) {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13)
                    $(this).parent().next().find("input").focus().select();

            });
            $(document).on("keyup", ".Quantity ", function (event) {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13)
                    $(this).parent().next().next().next().find("input").focus().select();
            });
            $(document).on("keyup", ".Others", function (event) {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13)
                    $(this).parent().next().next().next().find("input").focus().select();
            });
            $(document).on("keyup", ".GSTAmount ", function (event) {
                var tr = $(this).closest("tr");
                $this.GetWholeTotal();
                if (event.which == 13) {
                    //$this.GetQuantityPriceTotal(tr);
                    $this.AddItem();

                }
            });
            $("#AccountCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
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
            $this.LoadPageSetting();

            //$(document).on("click", "#DCTable > tbody tr", function () {
            //    $this.SelectDC(this);
            //});
            $(document).on("click", "#select-dc", function () {
                $this.AddSelectedChallans();
            });
            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                $this.Add();
                // }
            }
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
        AddSelectedChallans: function () {
            var $this = this;
            var html = "";
            var dcIds = "";
            var dcnumbers = "";
            var partycode = "";
            var companyPartnerId = "";
            $("#DCTable > tbody tr").each(function () {
                var tr = $(this);
                if ($(tr).find("td:nth-child(1) input").is(":checked")) {
                    var dcId = $(tr).find("td:nth-child(2) input.DcId").val();
                    dcIds += dcId + ",";
                    var dcnumber = $(tr).find("td:nth-child(2) input.DCNo").val();
                    dcnumbers += dcnumber + ",";
                    partycode = $(tr).find("td:nth-child(3)").text();
                    companyPartnerId = $(tr).find("td:nth-child(2) input.CompanyPartnerId").val();
                    html += "<tr>" + $(tr).html() + "</tr>"
                }
            });
            dcIds = dcIds.replace(/(^,)|(,$)/g, "");
            dcnumbers = dcnumbers.replace(/(^,)|(,$)/g, "");
            $("#table-dc-detail > tbody").html(html);
            $("#item-container tbody").html("");
            $("#table-dc-detail > tbody tr").find("td:nth-child(1)").remove();
            $("#DC-Container").modal("hide");
            if (dcIds.trim() == "") {
                $("#item-container tbody,#table-dc-detail > tbody").html("");
                $this.AddItem();
                return;
            }
            $("#DCNo").val(dcnumbers);

            var account = Common.GetByCode(partycode);
            $("#AccountId").val(account.Id);
            $("#AccountCode").val(account.AccountCode);
            $("#AccountName").val(account.Name);
            Common.GetPartyAddress(account.Id);
            $("#CompanyPartnerId").select2("val", companyPartnerId);

            $this.LoadDeliveryChallan("loadcs", dcIds);
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "gstsale") {

                var html = "Book No:" + $("#InvoiceNumber").val() + ", Dc No:" + $("#DCNo").val() + ", Order No:" + $("#OrderNo").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        GetProductDetail: function () {
            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            Common.Clear();
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode ").val();
            var qty = $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity ").val();

            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ItemCode").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            else if (typeof qty != "undefined" && qty.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(4) input.Quantity").focus().select();
                }, 300);

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

            //}
            //$this.AutoCompleteInit();
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitNumerics();
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
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
                focusElement = "#Date";
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
            var Items = new Array();
            var invoiceDcs = new Array();
            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ")";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {

                $("#table-dc-detail > tbody tr").each(function () {
                    var tr = $(this);
                    invoiceDcs.push(
                        {
                            SaleId: Common.GetInt($("#Id").val()),
                            DcId: Common.GetInt($(tr).find("td:nth-child(1) input.DcId").val()),
                            DcNumber: Common.GetInt($(tr).find("td:nth-child(1) input.DCNo").val())
                        });
                });

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#AccountCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += $("#AccountCode").val() + " is not valid party code.";
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.Quantity <= 0) {
                        err += "Item must have quantity greater than zero(0).";
                    }

                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err += item.ItemCode + " is not valid code.</li>";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                if (Common.GetInt(record.NetTotal) <= 0) {
                    err += "Transaction total amount should be graeter then zero(0).";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["TransactionType"] = VoucherType[voucher],
                record["SaleItems"] = Items;
                record["InvoiceDcs"] = invoiceDcs;
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
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var qty = 0.0;
            var rate = 0.0;
            var ait = 0.0;
            var cd = 0;
            var rd = 0.0;
            var sed = 0.0;
            var freight = 0.0;
            var fwd = 0.0;
            var others = 0.0;
            var gstpercent = PageSetting.GSTPercent;
            qty = Common.GetFloat($(tr).find(":nth-child(3) input.Quantity").val());
            rate = Common.GetFloat($(tr).find(":nth-child(5) input.Rate").val());
            ait = Common.GetFloat($(tr).find(":nth-child(7) input.AIT").val());
            cd = Common.GetFloat($(tr).find(":nth-child(8) input.CD").val());
            rd = Common.GetInt($(tr).find(":nth-child(9) input.RD").val());
            sed = Common.GetFloat($(tr).find(":nth-child(10) input.SED").val());
            freight = Common.GetFloat($(tr).find(":nth-child(11) input.Freight").val());
            fwd = Common.GetFloat($(tr).find(":nth-child(12) input.Fwd").val());
            others = Common.GetFloat($(tr).find(":nth-child(13) input.Others").val());
            var value = qty * rate
            var netamount = value + ait + cd + rd + sed + freight + fwd + others;
            var unitamount = Common.GetFloat(netamount / qty);
            var gstamount = Common.GetInt(netamount) * gstpercent / 100;
            $(tr).find(":nth-child(6) input.Amount ").val(value.toFixed(2));
            $(tr).find(":nth-child(14) input.NetAmount ").val(netamount.toFixed(2));
            $(tr).find(":nth-child(15) input.UnitCost ").val(unitamount.toFixed(2));
            $(tr).find(":nth-child(16) input.GSTAmount").val(gstamount.toFixed(2));

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var qty = 0.0;
            var value = 0.0;
            var ait = 0.0;
            var cd = 0.0;
            var rd = 0.0;
            var sed = 0.0;
            var freight = 0.0;
            var fwd = 0.0;
            var others = 0.0;
            var costamount = 0.0;
            var gstamount = 0.0;

            $("#item-container tbody tr").each(function () {
                qty += Common.GetFloat($(this).find(":nth-child(3) input.Quantity").val());
                value += Common.GetFloat($(this).find(":nth-child(6) input.Amount").val());
                ait += Common.GetFloat($(this).find(":nth-child(7) input.AIT").val());
                cd += Common.GetFloat($(this).find(":nth-child(8) input.CD").val());
                rd += Common.GetFloat($(this).find(":nth-child(9) input.RD").val());
                sed += Common.GetFloat($(this).find(":nth-child(10) input.SED").val());
                freight += Common.GetFloat($(this).find(":nth-child(11) input.Freight").val());
                fwd += Common.GetFloat($(this).find(":nth-child(12) input.Fwd").val());
                others += Common.GetFloat($(this).find(":nth-child(13) input.Others ").val());
                costamount += Common.GetFloat($(this).find(":nth-child(14) input.NetAmount ").val());
                gstamount += Common.GetFloat($(this).find(":nth-child(16) input.GSTAmount").val());

            });
            $("#item-container tfoot tr").find(":nth-child(2) input.QuantityTotal").val(qty);
            $("#item-container tfoot tr").find(":nth-child(3) input.ValueTotal").val(value.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(4) input.AITTotal").val(ait.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(5) input.CDTotal").val(cd.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(6) input.RDTotal").val(rd.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(7) input.SEDTotal").val(sed.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(8) input.FreightTotal").val(freight.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(9) input.FwdTotal").val(fwd.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(10) input.OthersTotal").val(others.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(11) input.CostAmountTotal").val(costamount.toFixed(2));
            $("#item-container tfoot tr").find(":nth-child(12) input.GstAmountTotal").val(gstamount.toFixed(2));
            $("#GrossTotal").val(costamount.toFixed(0));
            $("#GstAmountTotal").val(gstamount.toFixed(0));
            var netamount = Common.GetInt(costamount + gstamount);
            $("#NetTotal").val(netamount);
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("#item-container tbody").html("");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            var accountdetail = res.Data.AccountDetail;
            if (d != null) {
                Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
                $("#lblDate").html(moment(d.Date).format("DD/MM/YYYY"));
                $("#lblOrderDate").html(moment(d.OrderDate).format("DD/MM/YYYY"));
                //$("#lblSTRN").html(accountdetail.GST);
                //$("#lblNTN").html(accountdetail.NTN);
                var type = $this.GetType();
                var html = "";
                var items = d.SaleItems;
                var index = 1;
                $("#report-saleitem tbody").html("");
                for (var i in items) {
                    var item = items[i];
                    html += "<tr>";
                    html += "<td>" + (index++) + "</td>";
                    html += "<td>" + item.ItemName + "  " + item.ItemCode + "</td>";
                    html += "<td>" + item.Quantity + "</td>";
                    html += "<td>" + item.Rate.format() + "</td>";
                    html += "<td>" + item.Amount.format() + "</td>";
                    html += "<td>" + (item.NetAmount - item.Amount) + "</td>";
                    html += "<td>" + item.NetAmount.format() + "</td>";
                    html += "<td>" + item.UnitCost.format() + "</td>";
                    html += "<td>" + item.GSTAmount.format() + "</td>";
                    html += "</tr>";
                }

                $("#report-saleitem tbody").append(html);
                var qtyTotal = Common.GetInt(Enumerable.From(items).Sum("$.Quantity")).format();
                var amountTotal = Common.GetInt(Enumerable.From(items).Sum("$.Amount"));
                var gstamtTotal = Common.GetInt(Enumerable.From(items).Sum("$.GSTAmount"));
                var grandTotal = Common.GetInt(Enumerable.From(items).Sum("$.NetAmount"));
                var duties = Common.GetInt(grandTotal - amountTotal).format();
                //var gstpercent = Common.GetFloat(Enumerable.From(items).FirstOrDefault().GSTPercent);
                $("#report-saleitem tfoot tr").find("td:nth-child(3)").html(qtyTotal);
                $("#report-saleitem tfoot tr").find("td:nth-child(5)").html(amountTotal.format());
                $("#report-saleitem tfoot tr").find("td:nth-child(6)").html(duties);
                $("#report-saleitem tfoot tr").find("td:nth-child(7)").html(grandTotal.format());
                $("#report-saleitem tfoot tr").find("td:nth-child(9)").html(gstamtTotal.format());
                $("#tblAgingItems thead ").find("#totalamount").html(grandTotal.format());
                $("#tblAgingItems thead ").find("#salestax").html(PageSetting.GSTPercent + "%");
                $("#tblAgingItems thead ").find("#grandtotal").html($("#NetTotal").val().format());
            }
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
                        $this.CustomClear();
                        var d = res.Data.Order;
                        var dcs = res.Data.DeliveryChallans;
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
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);

                                Common.MapItemData(d.SaleItems);
                                var dchtml = "";
                                var dcnumbers = "";
                                for (var i in dcs) {
                                    var item = dcs[i];
                                    dcnumbers += item.VoucherNumber + ",";
                                    dchtml += "<tr>";
                                    dchtml += "<td>";
                                    dchtml += "<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' /><input type='text' class='DcId hide' value='" + item.Id + "' />";
                                    dchtml += item.VoucherNumber + "</td>";
                                    dchtml += "<td>" + item.AccountCode + "</td>";
                                    dchtml += "<td>" + item.AccountName + "</td>";
                                    dchtml += "<td>" + item.OrderNumber + "</td>";
                                    dchtml += "<td>" + Common.FormatDate(item.Date, "ddd, MMM DD, YYYY") + "</td>";
                                    dchtml += "</tr>";
                                }
                                dcnumbers = dcnumbers.replace(/(^,)|(,$)/g, "");
                                $("#table-dc-detail tbody").html(dchtml);
                                $("#DCNo").val(dcnumbers);
                                //Common.MapItemData(d.SaleItems, null, "#template-usman", true);
                                //Common.MapItemData(d.SaleItems, "#data-conatiner", null);
                                //Common.MapItemData(d.SaleItems, null, null,true);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();

                            }
                            //if (d.Id > 0 && d.SaleItems != null && d.SaleItems.length > 0) {
                            //    $("#btndelete,#btnprint").prop("disabled", false);
                            //    var html = "";
                            //    $this.LoadReportData(res);
                            //    var items = d.SaleItems;
                            //    Common.MapItemData(items);
                            //    setTimeout(function () {
                            //        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                            //    }, 500);
                            //    $this.GetWholeTotal();

                            //}
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
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;
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
                            focusElement = "#InvoiceNumber";
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
            var id = 0;
            switch ($this.GetType().toLowerCase()) {
                case "gstsale":
                    id = PageSetting.Customers;
                    break;
                case "gstsalereturn":
                    id = PageSetting.Customers;
                    break;
                case "gstpurchase":
                    id = PageSetting.Suppliers;

                    break;
                case "gstpurchasereturn":
                    id = PageSetting.Suppliers;
                    break;
            }


            var tokens = Common.GetLeafAccounts(id);
            if (PageSetting.CompanyId = 36) {
                var exids = new Array();
                exids.push(Common.GetInt(PageSetting.Products));
                tokens = Common.GetAllLeafAccounts(exids);
            }
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
                        if (type == "gstsale") {
                            $("#Comments").val("Sold To: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstsalereturn") {
                            $("#Comments").val("Sale Return From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstpurchase") {
                            $("#Comments").val("Purchase From: " + d.AccountCode + "-" + d.Name);
                        }
                        else if (type == "gstpurchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.AccountCode + "-" + d.Name);
                        }
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

            if (voucher == "gstpurchase") {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo,#dc-search").removeClass("hide");

            }
            else {
                $("#DCNo,#lbldc,#lblorderno,#OrderNo,#dc-search").addClass("hide");
            }
            this.LoadAccounts();

        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            $(".ItemCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.ItemName").val(account.Name);
                        $(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        //$(tr).find(":nth-child(3) input.Unit ").val(Common.GetFloat(product.Unit));
                        $(tr).find(":nth-child(5) input.Rate").val(Common.GetFloat(product.PurchasePrice));
                        $(".container-message").hide();
                    }

                }
            });

        },
        LoadDeliveryChallan: function (key, dcIds) {
            var $this = this;
            //var orderno = Common.GetInt($("#DCNo").val());
            var type = VoucherType[$this.GetType()]
            if (type == VoucherType.sale || type == VoucherType.purchasereturn)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.salereturn)
                type = VoucherType.goodreceive
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan" + "?type=" + type + "&key=" + key + "&dcIds=" + dcIds,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var items = res.Data;
                        $("#item-container tbody").html("");
                        if (items != null && items.length > 0) {
                            var html = "";
                            for (var i in items) {
                                var item = items[i];
                                var account = Common.GetByCode(item.ItemCode);
                                var product = Common.GetAccountDetailByAccountId(account.Id);
                                var price = 0;

                                items[i].Quantity = item.Quantity = item.Quantity;
                                var qty = item.Quantity;
                                if (typeof product != "undefined" && product != null) {
                                    price = Common.GetFloat(product.PurchasePrice);
                                }

                                var amount = qty * price;
                                var gstpercent = PageSetting.GSTPercent;
                                var gstamount = Common.GetInt(amount) * gstpercent / 100;
                                var netAmount = Common.GetInt(amount) + gstamount;
                                item["Rate"] = price;
                                item["Amount"] = amount;
                                item["GSTPercent"] = gstpercent;
                                item["GSTAmount"] = gstamount;
                                item["NetAmount"] = amount;

                            }
                            Common.MapItemData(items);

                            $this.MapComments();

                            //setTimeout(function () {
                            //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                            //}, 500);

                        }
                        $this.GetWholeTotal();

                        $this.AddItem();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetDC: function () {
            var $this = this;
            $("#DC-Container").modal("show");
            $this.BindDCDatatable();
        },
        BindDCDatatable: function () {
            dcNumbers = "";
            var $this = this;
            $("#table-dc-detail > tbody tr").each(function () {
                var tr = $(this);

                var dcNumber = $(tr).find("td:nth-child(1) input.DCNo").val();
                dcNumbers += dcNumber + ",";

            });
            dcNumbers = dcNumbers.replace(/(^,)|(,$)/g, "")
            LIST_LOADED = false;
            var type = VoucherType[$this.GetType()];
            var pagetype = type;
            if (type == VoucherType.sale || type == VoucherType.gstsale)
                type = VoucherType.goodissue;
            else if (type == VoucherType.purchase || type == VoucherType.gstpurchase)
                type = VoucherType.goodreceive
            var url = Setting.APIBaseUrl + "DeliveryChallan?type=" + type + "&dcNumbers=" + dcNumbers;
            var options =
             {
                 "bPaginate": false,
                 "bSort": false,
             }
            DataTable.DestroyDatatable(DC_DATATABLE_ID);
            DataTable.BindDatatable(DC_DATATABLE_ID, url, options);
        },
        SelectDC: function (tr) {
            var $this = this;
            var dcno = $(tr).find("input.DCNo ").val();
            if (dcno.trim() != "" && dcno != null) {
                $("#DCNo").val(dcno);
                $this.LoadDeliveryChallan("challan");
                $('#btnDCClose').click();
            }
        },
    };
}();

