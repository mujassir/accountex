
var WheatWorkInProcess = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "WheatWorkInProcess";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "voucher";
    return {
        init: function () {
           var $this = this;
            $(".date-picker").on("show", function (e) {
                //console.debug('show', e.date, $(this).data('stickyDate'));

                if (e.date) {
                    $(this).data("stickyDate", e.date);
                }
                else {
                    $(this).data("stickyDate", null);
                }
                $(this).datepicker("hide");
            });
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
                    $this.LoadVoucher("same");
                    
                }
            });
            $("#Date").keyup(function (e) {

                if (e.which == 13) {
                    $("#DCNo").focus();

                }
            });
            $("#InvoiceNumber").keyup(function (e) {

                if (e.which == 13) {
                    $("#Date").focus();

                }
            });

            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
            });

            $(document).on("keyup", ".Code", function (event) {
                debugger;
                var parent = $(this).closest("div[data-save='save']");

                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {

                        if ($(parent).find("#rawitem").length) {

                            $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", $("#finisheditem-container")).focus();

                        }
                        else {
                            $(".btn.btn-primary.green").focus();
                        }

                    }
                    else {

                        $("table tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity", parent).focus();

                    }

                }
            });

            $(document).on("blur", ".Code", function () {

                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var product = $this.GetByCode($(this).val());
                var tr = $(this).parent().parent();
                var parent = $(this).parent("div[data-save='save']");
                if (typeof product != "undefined" && product != null) {

                    $(tr).find(":nth-child(1) input.ItemId").val(product.AccountId);
                    $(tr).find(":nth-child(2) input.Name").val(product.Name);
                    $(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.PurchasePrice));

                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = "<li>" + $(this).val() + " is not valid code.</li>";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });

            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (qty <= 0) {
                    //var err = "<li>Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    //$(tr).find("input.Quantity").focus();
                    $(tr).find(":nth-child(2) input.Quantity").val("1");

                }

            });


            $(document).on("keyup", ".Weight,.Amount", function (event) {
                var type = "";
                var tr = $(this).parent().parent();
                var rowtype = $(this).closest("table").attr("id");
                if (rowtype == "rawitem")
                    type = "issued";
                else
                    type = "received";
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (qty <= 0 && event.which == 13) {
                    qty = 1;
                    $(tr).find(":nth-child(2) input.Quantity").val("1");
                }
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddItem(parent, type);
               


            });
            $(document).on("keyup", ".Quantity", function (event) {
                var type = "";
                var tr = $(this).parent().parent();
                var parent = $(this).closest("div[data-save]");
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (qty <= 0 && event.which == 13) {
                    qty = 1;
                    $(tr).find(":nth-child(2) input.Quantity").val("1");
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                }
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0) {
                    $(tr).find(":nth-child(4) input.Weight").focus();
                    $(tr).find(":nth-child(3) input.Weight").focus();
                }
            });
            this.LoadPageSetting();
            $this.GetCustomerProducts();
            $this.LoadAccountDetail();

            var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }


        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        New: function () {

           var $this = this;
            SetFocus = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
        },

        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },

        GetByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
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
            } else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        AddItem: function (parent, type) {

           var $this = this;
            if (type == "issued") {
                var ttlkg = $("table tbody tr:nth-last-child(1) td:nth-child(4) input.TotalWeight", parent).val();
                if (typeof ttlkg != "undefined" && ttlkg<=0) {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(3) input.Weight", parent).focus().select();
                    }, 300);

                    return;
                }
            }
            else {
                var code = $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).val();
                if (typeof code != "undefined" && code.trim() == "") {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                    }, 300);

                    SetFocus = "code";
                    return;
                }
            }
            var html = "<tr>";

            html += "<td><input type='hidden' id='Id' value=''>";
            if (type == "issued") {
                html += "<input type='hidden' class='ItemId' id='ItemId' value='" + PageSetting.WheatAccountHeadId + "'>";
                html += "<input type='text' disabled='disabled' class='Code form-control typeahead input-small' data-plus-as-tab='false' value='" + PageSetting.WheatAccount + "'></td>";
            }
            else {
                html += "<input type='hidden' class='ItemId' id='ItemId'>";
                html += "<input type='text' class='Code form-control typeahead input-small' data-plus-as-tab='false'></td>";
                html += "<td><input type='text' class='Name form-control input-medium'></td>";
            }

            html += "<td><input type='text' class='Quantity form-control input-small'></td>";
            html += "<td><input type='text' class='Weight form-control input-small'></td>";
            html += "<td><input type='text' class='TotalWeight form-control input-small' disabled='disabled' readonly='readonly'></td>";
            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WheatWorkInProcess.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
            html += "</tr>";
            $("table tbody", parent).append(html);
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
            else if (SetFocus == "booknumber") {
                setTimeout(function () {
                    $("#InvoiceNumber").focus();
                }, 300);
            }
            else {
                if (type == "issued") {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(2) input.Quantity", parent).focus().select();
                    }, 300);

                }
                else {
                    setTimeout(function () {
                        $("table tbody tr:nth-last-child(1) td:nth-child(1) input.Code", parent).focus().select();
                    }, 300);
                }
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");

            $("#Item").focus();
            $this.AutoCompleteInit();


        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

           var $this = this;
            var rowtype = $(elment).closest("table").attr("id");
            var parent = $(elment).closest("div[data-save]");
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal(parent, rowtype);
            if (rowtype == "rawitem") {
                if ($("#rawitem tbody").children().length <= 0)
                    $this.AddItem(parent, "issued");
            }
            else {
                ($("#finisheditem tbody").children().length <= 0);
                $this.AddItem(parent, "received");

            }
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
                $this.GetNextVoucherNumber();
                SetFocus = "date";
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
            var voucher = "production";
            var id = Common.GetInt($("#Id").val()); 
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {
                var WheatAccount = Common.GetById(PageSetting.WheatAccountHeadId);
                $("#rawitem tbody tr").each(function () {

                    var itemcode = $(this).children(":nth-child(1)").find("input.Code").val();
                    var qty = Common.GetInt($(this).children(":nth-child(2)").find("input.Quantity").val());
                    var totalweight = Common.GetFloat($(this).children(":nth-child(4)").find("input.TotalWeight").val());
                    if (typeof itemcode != "undefined" && itemcode.trim() != "") {
                        var itemid = Common.GetInt($(this).children(":nth-child(1)").find("input.ItemId").val());

                        var weight = Common.GetFloat($(this).children(":nth-child(3)").find("input.Weight").val());

                        Items.push({
                            ProductionId: Common.GetInt(record.Id),
                            InvoiceNumber: $("#InvoiceNumber").val(),
                            VoucherNumber: $("#VoucherNumber").val(),
                            Date: Common.ChangeDateFormate($("#Date").val()),
                            Quantity: qty,
                            Weight: weight,
                            TotalWeight: totalweight,
                            Id: $(this).children(":nth-child(1)").children("#Id").val(),
                            TransactionType: VoucherType[voucher],
                            EntryType: EntryType.RawMaterial,
                            ItemCode: WheatAccount.AccountCode,
                            ItemName: itemcode,
                            ItemId: itemid,
                        });
                    }
                });
                $("#finisheditem tbody tr").each(function () {

                    var itemcode = $(this).children(":nth-child(1)").find("input.Code").val();
                    if (typeof itemcode != "undefined" && itemcode.trim() != "") {
                        var itemname = $(this).children(":nth-child(2)").find("input.Name").val();
                        var itemid = Common.GetInt($(this).children(":nth-child(1)").find("input.ItemId").val());
                        var qty = Common.GetInt($(this).children(":nth-child(3)").find("input.Quantity").val());
                        var weight = Common.GetFloat($(this).children(":nth-child(4)").find("input.Weight").val());
                        var totalweight = Common.GetFloat($(this).children(":nth-child(5)").find("input.TotalWeight").val());
                        Items.push({
                            ProductionId: Common.GetInt(record.Id),
                            InvoiceNumber: $("#InvoiceNumber").val(),
                            VoucherNumber: $("#VoucherNumber").val(),
                            Date: Common.ChangeDateFormate($("#Date").val()),
                            Quantity: qty,
                            Weight: weight,
                            TotalWeight: totalweight,
                            Id: $(this).children(":nth-child(1)").children("#Id").val(),
                            TransactionType: VoucherType[voucher],
                            EntryType: EntryType.FinishedGoods,
                            ItemCode: itemcode,
                            ItemName: itemname,
                            ItemId: itemid,
                        });
                    }
                });
                var err = "";


                for (var i in Items) {
                    var item = Items[i];
                    if (item.TotalWeight <= 0) {
                        err += "<li>Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).";
                    }

                    //var product = $this.GetByCode(item.ItemCode);
                    //if (typeof product == "undefined" || product == null) {
                    //    err += "<li>" + item.ItemCode + " is not valid code.</li>";
                    //}

                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }


                record["TransactionType"] = VoucherType[voucher],
                record["ProductionItems"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving workin process  ...please wait",
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
            var parent = $(tr).closest("div[data-save]");
            var rowtype = $(tr).closest("table").attr("id");
            var Quantity = 0;
            var Weight = 0;
            if (rowtype == "rawitem") {
                Quantity = Common.GetInt($(tr).find(":nth-child(2) input.Quantity").val());
                Rate = Common.GetInt($(tr).find(":nth-child(3) input.Weight").val());
                var amount = Quantity * Rate;
                $(tr).find(":nth-child(4) input.TotalWeight").val(amount);
            }
            else {
                Quantity = Common.GetInt($(tr).find(":nth-child(3) input.Quantity").val());
                Rate = Common.GetInt($(tr).find(":nth-child(4) input.Weight").val());
                var amount = Quantity * Rate;
                $(tr).find(":nth-child(5) input.TotalWeight").val(amount);
            }


            $this.GetWholeTotal(parent, rowtype);
        },
        GetWholeTotal: function (parent, rowtype) {
           var $this = this;
            var Quantity = 0;
            var Price = 0;
            if (rowtype == "rawitem") {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetInt($(this).find(":nth-child(2) input.Quantity").val());
                    Price += Common.GetInt($(this).find(":nth-child(4) input.TotalWeight").val());

                });

            }
            else {
                $("table tbody tr", parent).each(function () {
                    Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
                    Price += Common.GetInt($(this).find(":nth-child(5) input.TotalWeight").val());

                });

            }
            $("table tfoot tr", parent).find(":nth-child(2) input.Quantity").val(Quantity);
            $("table tfoot tr", parent).find(":nth-child(3) input.Weight").val(Price);
            if ($(parent).find("#rawitem").length) {

                $("#QuantityTotal").val(Quantity);
                $("#NetTotal").val(Price);
                $("#IssuedTotal").val(Price);
            }
            else {
                $("#FinishedQuantityTotal").val(Quantity);
                $("#FinishedNetTotal").val(Price);
                var issuedtotal = Common.GetInt($("#NetTotal").val());
                var gain = Price - issuedtotal;
                $("#IssuedTotal").val(issuedtotal);
                $("#NetWeightGain").val(gain);
            }


        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("#saleitem tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            Common.Clear();
        },



        LoadReportData: function (res) {
           var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
            var type = $this.GetType();
            if (type == "sale") {
                $("#lblReportTitle").html("Sales Estimate");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "purchase") {
                $(".row-sale").addClass("hide");
                $("#lblReportTitle").html("Purchase Voucher");
                $("#lblFooterNotes").html(" * All imported items are not returnable");
            }

            var html = "";
            var items = d.SaleItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "<td>" + item.DiscountAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

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
        },
        LoadVoucher: function (key) {
           var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var voucher = "production";
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[voucher] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading workin process  ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#rawitem tbody").html("");
                        $("#finisheditem tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#DCNo").prop("disabled", "disabled");
                            //alert($("#Date").val());
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });

                            if (d.Id > 0 && d.ProductionItems != null && d.ProductionItems.length > 0) {
                                //$this.LoadReportData(res);
                                //$this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.ProductionItems;
                                var rawitems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.RawMaterial;
                                });
                                for (var i in rawitems) {
                                    var item = rawitems[i];
                                    html += "<tr>";
                                    html += "<td><input type='hidden' id='Id' vvalue='" + item.Id + "'>";
                                    html += "<input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='text' disabled='disabled' class='Code form-control typeahead input-small' value='" + item.ItemName + "'></td>";
                                    //html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Weight form-control input-small'value='" + item.Weight + "' ></td>";
                                    html += "<td><input type='text' value='" + item.TotalWeight + "' class='TotalWeight form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=WheatWorkInProcess.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#rawitem tbody").append(html);
                                var finisheditems = $.grep(items, function (e) {
                                    return e.EntryType == EntryType.FinishedGoods;
                                });
                                html = "";
                                for (var i in finisheditems) {
                                    var item = finisheditems[i];
                                    html += "<tr>";
                                    html += "<td><input type='hidden' id='Id' vvalue='" + item.Id + "'>";
                                    html += "<input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Weight form-control input-small'value='" + item.Weight + "' ></td>";
                                    html += "<td><input type='text' value='" + item.TotalWeight + "' class='TotalWeight form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WheatWorkInProcess.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#finisheditem tbody").append(html);
                                //setTimeout(function () {
                                //    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                //}, 500);

                            }
                        }
                        $this.GetWholeTotal();
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");

                        $this.AddItem($("#rawitem-container"), "issued");
                        SetFocus = "booknumber";
                        $this.AddItem($("#finisheditem-container"), "received");
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
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
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data));
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html(currentbalance - invoicetotal);

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

            $("#lblcurrentbalance").html(currentbalance);

        },
        GetCustomerProducts: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?&key=GetCustomerProducts",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        AppData.CustomerDiscount = res.Data;

                        //var html = "";
                        //var products = res.Data;
                        //html += "<option></option>";
                        //for (var i = 0; i < products.length; i++) {
                        //    var product = products[i];
                        //    html += "<option data-detail='" + JSON.stringify(product) + "' value='" + product.COAProductId + "'>" + product.ProductCode + "-" + product.ProductTitle + "</option>";
                        //}
                        //$("#Item").html(html).select2();

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        LoadAccountDetail: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?&key=LoadAccountDetail",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        AppData.AccountDetail = res.Data;

                        
                    } else {
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

            var type = "production";
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
                    blockMessage: "Deleting workin process  ...please wait",
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
            var id = 0;
            switch ($this.GetType().toLowerCase()) {
                case "sale":
                    id = PageSetting.Customers;
                    break;
                case "salereturn":
                    id = PageSetting.Customers;
                    break;
                case "purchase":
                    id = PageSetting.Suppliers;

                    break;
                case "purchasereturn":
                    id = PageSetting.Suppliers;
                    break;
            }


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

                    var d = $this.GetByCode(ui.item.value);
                    var type = $this.GetType();

                    if (typeof d != "undefined" && d != null) {
                        if (type == "sale") {
                            $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("Sale Return From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Purchase From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.Code + "-" + d.Name);
                        }
                        $("#AccountName").val(d.Name);

                        $("#AccountId").val(d.AccountId);
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
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
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

            $("#finisheditem .Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = $this.GetByCode(ui.item.value);

                    var tr = $(this).parent().parent();
                    if (typeof product != "undefined" && product != null) {

                        $(tr).find(":nth-child(1) input.ItemId").val(product.AccountId);
                        $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.PurchasePrice));
                        //$(tr).find(":nth-child(3) input.Quantity").val(Common.GetFloat(product.PurchasePrice));
                        setTimeout(function () {
                            $(tr).find(":nth-child(3) input.Quantity").focus();
                        }, 500);
                        $(".container-message").hide();
                    }

                }
            });

        },

    };
}();
