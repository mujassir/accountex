
var WheatPurchase = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "WheatPurchase";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "booknumber";
    return {
        init: function () {
            var $this = this;
            $("#VoucherNumber").keyup(function (e) {

                if (e.which == 13) {
                    $(this).val() == "0" ? SetFocus = "booknumber" : "date";
                    $this.LoadVoucher("same");
                    //setTimeout(function () {
                    //    $('#InvoiceNumber').focus();
                    //}, 1000);
                    //$('#InvoiceNumber').focus();
                }
            });
            $(document).on("keyup", ".Party", function (event) {

                var type = $this.GetType();
                if (event.which == 13) {
                    if ($(this).val() == "") {
                        if (type != "sale")
                            $(".btn.btn-primary.green").focus();
                        else
                            $("#BiltyNo").focus();

                    }
                    else {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(4) input.Bags").focus().select();
                    }

                }
            });
            $(document).on("keyup", ".WheatRate", function () {
                var tr = $(this).parent().parent();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13)
                    $this.AddItem();
                //$(this).parent().next().next().find("select.select2").select2("focus");

            });
            $(document).on("keyup", ".BardanaType", function () {

                if (event.which == 13)
                    $this.AddItem();

            });
            $(document).on("blur", ".BardanaWeight ", function (event) {
                var bardanaweight = $(this).val();
                var tr = $(this).parent().parent();
                if (bardanaweight <= 1) {

                    $(tr).find(":nth-child(15) input.BardanaType ").val("P");
                }
                else {
                    $(tr).find(":nth-child(15) input.BardanaType ").val("J");

                }


            });

            $(document).on("keyup", ".BillNo,.StandardWeight,.TruckNo,.Bags,.GrossWeight ,.Khot ,.BardanaWeight,.BardanaRate,.BardanaReturned,.Freight ", function (event) {

                var tr = $(this).parent().parent();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13)
                    $(this).parent().next().find("input").focus().select();
            });
            $("input[name='ac-type']").change(function () {
                $this.GetNextVoucherNumber();
                //if ($(this).val() == "cash") {
                //    var acc = Common.GetById(PageSetting.CashAccount);
                //    $("#AccountId").val(acc.Id);
                //    $("#AccountCode").val(acc.AccountCode);
                //    $("#AccountName").val(acc.DisplayName);
                //    $("#AccountCode").attr("disabled", "disabled");
                //    $this.MapComments();
                //    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                //}
                //else {
                //    $("#AccountCode").removeAttr("disabled");
                //    $("#AccountId").val("");
                //    $("#AccountCode").val("");
                //    $("#AccountName").val("");
                //    $("#InvoiceNumber").focus();
                //}

            });

            $this.LoadPageSetting();
            $this.Add();
            $("#Comments").keyup(function (event) {
                if (event.which == 13)
                    $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party ").focus().select();

            });
            //$("#purchaseitem").stickyTableHeaders();

        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val() + ", Dc No:" + $("#DCNo").val() + ", Order No:" + $("#OrderNo").val();
                $("#Comments").val(html);
            }
        },
        New: function () {

            var $this = this;
            SetFocus = "booknumber";
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
        GetDiscountDetail: function (productid) {

            var customerid = $("#AccountId").val();
            var discount = $.grep(AppData.CustomerDiscount, function (e) { return e.CustomerId == customerid && e.COAProductId == productid; })[0];
            if (discount != null)
                return discount.Discount;
            else
                return 0;
        },
        GetByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetByCodeFromCOA: function (code) {
            var accounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel; });
            var data = $.grep(accounts, function (e) { return e.AccountCode.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetSoldRateByPartyId: function (accountId, itemId) {
            var data = Enumerable.From(PageSetting.SoldRates).FirstOrDefault(null, function (p) { return p.AccountId == accountId && p.ItemId == itemId });
            if (data != null)
                return data.WheatRate
            else
                return 0;
        },
        UpdateSoldRates: function (items) {
            for (var i in items) {
                var item = items[i];
                var data = Enumerable.From(PageSetting.SoldRates).FirstOrDefault(null, function (p) { return p.AccountId == item.PartyId && p.ItemId == item.ItemId });
                if (data != null) {
                    data.WheatRate = item.WheatRate;
                }
            }
        },

        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },

        Add: function () {
            var $this = this;
            Common.Clear();
            $this.DetailView();
            $this.CustomClear();
            // $this.AddItem();
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
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party").val();
            var bags = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(4) input.Bags").val();
            var barweight = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(6) input.BardanaWeight ").val();
            var grsweight = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(8) input.GrossWeight").val();
            var barrate = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(12) input.BardanaRate").val();
            var barreturned = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(10) input.BardanaReturned").val();
            var whrate = $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(13) input.WheatRate").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party").focus().select();
                }, 300);
                SetFocus = "code";
                return;
            }
            if (typeof grsweight != "undefined" && grsweight.trim() == "") {

                if (typeof barweight != "undefined" && barweight.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(6) input.BardanaWeight ").focus().select();
                    }, 300);

                    return;
                }
                if (typeof barreturned != "undefined" && barreturned.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(10) input.BardanaReturned").focus().select();
                    }, 300);

                    return;
                }

            }
            else {
                if (typeof bags != "undefined" && bags.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(4) input.Bags").focus().select();
                    }, 300);

                    return;
                }
                else if (typeof barweight != "undefined" && barweight.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(6) input.BardanaWeight ").focus().select();
                    }, 300);
                    return;
                }
                    //else if (typeof grsweight != "undefined" && grsweight.trim() == "") {
                    //    setTimeout(function () {
                    //        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(8) input.GrossWeight").focus().select();
                    //    }, 300);
                    //    return;
                    //}
                else if (typeof barrate != "undefined" && barrate.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(12) input.BardanaRate").focus().select();
                    }, 300);

                    return;
                }
                else if (typeof whrate != "undefined" && whrate.trim() == "") {
                    setTimeout(function () {
                        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(13) input.WheatRate").focus().select();
                    }, 300);

                    return;
                }
            }


            var html = "<tr>";

            if ($("input[name='ac-type']:checked").val() == "GOVT") {
                SetFocus = "bags";
                var acc = Common.GetById(PageSetting.FoodDepartmentHeadId);
                html += "<td><input type='hidden' id='Id' value=''><input type='hidden' class='PartyId' value='" + acc.Id + "' />";
                html += "<input type='text' class='Party form-control typeahead input-small' value='" + acc.AccountCode + "' title='" + acc.Name + "' data-toggle='tooltip' /></td>";
            }
            else {
                html += "<td><input type='hidden' id='Id' value=''><input type='hidden' class='PartyId'/>";
                html += "<input type='text' class='Party form-control typeahead input-small' /></td>";
            }

            html += "<td><input type='text' disabled='disabled' class='Item form-control input-small' value='" + PageSetting.WheatAccount + "'>";
            html += "<input type='hidden' disabled='disabled' class='ItemId form-control input-small' value='" + PageSetting.WheatAccountHeadId + "'></td>";
            html += "<td><input type='text' class='BillNo form-control input-medium' ></td>";
            html += "<td><input type='text' class='Bags form-control input-small'></td>";
            html += "<td><input type='text' class='StandardWeight form-control input-small'></td>";
            html += "<td><input type='text' class='BardanaWeight form-control input-small'></td>";
            html += "<td><input type='text' class='TruckNo form-control input-small'></td>";
            html += "<td><input type='text' class='GrossWeight form-control input-small'></td>";
            html += "<td><input type='text' class='Khot form-control input-small'></td>";
            html += "<td><input type='text' class='BardanaReturned form-control input-small'></td>";
            html += "<td><input type='text' class='Freight form-control input-small'></td>";
            html += "<td><input type='text' class='BardanaRate form-control input-small'></td>";
            html += "<td><input type='text' class='WheatRate form-control input-small'></td>";
            html += "<td><input type='text' disabled='disabled' class='NetWeight form-control input-xsmall'></td>";
            html += "<td><input type='text' maxlength='1' class='BardanaType form-control input-small'></td>";
            html += "<td><input type='text' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WheatPurchase.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
            html += "</tr>";
            $("#purchaseitem tbody").append(html);
            $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party").focus().select();
            if (SetFocus == "bags") {
                setTimeout(function () {
                    $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(4) input.Bags").focus().select();
                }, 300);

            }
            //else if (SetFocus == "voucher") {
            //    setTimeout(function () {
            //        $("#VoucherNumber").focus();
            //    }, 300);
            //}
            //else if (SetFocus == "booknumber") {
            //    setTimeout(function () {
            //        $("#InvoiceNumber").focus();
            //    }, 300);
            //}
            //else if (SetFocus == "dcnumber") {
            //    setTimeout(function () {
            //        $("#DCNo").focus();
            //    }, 300);
            //}
            //else {

            //    setTimeout(function () {
            //        $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party").focus().select();
            //    }, 300);
            //}
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            $this.AutoCompleteInit();
            $this.GetNextBillNo()


        },
        GetNextBillNo: function () {
            var billno = 0;
            var $this = this;
            var type = $('input[name="ac-type"]:checked', '#account-type-conatiner').val()
            if ($("#purchaseitem tbody").children().length <= 1) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?keyvalue=nextbillno&type=" + type,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Loading  ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            billno = res.Data.BillNo;
                            $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(3) input.BillNo ").val(billno);

                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });


            }

            else {
                var prevbillno = Common.GetInt($("#purchaseitem tbody tr:nth-last-child(2) td:nth-child(3) input.BillNo ").val());
                billno = prevbillno + 1;
                $("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(3) input.BillNo ").val(billno);
            }



        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#purchaseitem tbody").children().length <= 0)
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
                $("input:radio[value='PVT']").prop("checked", true);
                SetFocus = "date";
                $this.GetNextVoucherNumber();
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
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            record["IsGovt"] = $("input[value='GOVT']").is(":checked");
            var Items = new Array();
            if (id > 0) {
                if (record.PreVoucherNumber != record.VoucherNumber) {
                    err = "<li>You cannot change voucher no.Please save with previous  voucher no (" + record.PreVoucherNumber + ") </li>";
                    Common.ShowError(err);
                    return;
                }
            }
            if (Common.Validate($("#mainform"))) {
                $("#purchaseitem tbody input.StandardWeight").trigger("keyup");

                var TtlBags = $("#purchaseitem tfoot tr").find(":nth-child(2) input.BagsTotal").val();
                var TtlStdWeight = $("#purchaseitem tfoot tr").find(":nth-child(3) input.StdWeightTotal ").val();
                var TtlBardanaWeight = $("#purchaseitem tfoot tr").find(":nth-child(4) input.BardanaWeightTotal").val();
                var TtlGrossWeight = $("#purchaseitem tfoot tr").find(":nth-child(5) input.GrossWeightTotal").val();
                var TtlKhot = $("#purchaseitem tfoot tr").find(":nth-child(6) input.KhotTotal").val();
                var TtlBardanaRetuned = $("#purchaseitem tfoot tr").find(":nth-child(7) input.BardanaReturnTotal").val();
                var TtlFreight = $("#purchaseitem tfoot tr").find(":nth-child(8) input.FreightTotal").val();
                var TtlNetWeight = $("#purchaseitem tfoot tr").find(":nth-child(9) input.NetWeightTotal").val();
                var TtlNetAmount = $("#purchaseitem tfoot tr").find(":nth-child(10) input.NetAmountTotal").val();
                record["BagsTotal"] = TtlBags;
                record["StdWeightTotal"] = TtlStdWeight;
                record["BardanaWeightTotal"] = TtlBardanaWeight;
                record["GrossWeightTotal"] = TtlGrossWeight;
                record["KhotTotal"] = TtlKhot;
                record["BardanaReturnTotal"] = TtlBardanaRetuned;
                record["FreightTotal"] = TtlFreight;
                record["NetWeightTotal"] = TtlNetWeight;
                record["NetAmountTotal"] = TtlNetAmount;

                $("#purchaseitem tbody tr").each(function () {

                    var party = $(this).children(":nth-child(1)").find("input.Party").val();
                    var partyId = $(this).children(":nth-child(1)").find("input.PartyId").val();

                    if (typeof party != "undefined" && party.trim() != "") {
                        var bags = Common.GetInt($(this).children(":nth-child(4)").find("input.Bags").val());
                        var itemname = $(this).children(":nth-child(2)").find("input.Item").val();
                        var billno = Common.GetInt($(this).children(":nth-child(3)").find("input.BillNo").val());
                        var stdweight = Common.GetFloat($(this).children(":nth-child(5)").find("input.StandardWeight").val());
                        var barweight = Common.GetFloat($(this).children(":nth-child(6)").find("input.BardanaWeight").val());
                        var truckno = $(this).children(":nth-child(7)").find("input.TruckNo").val();
                        var grsweight = $(this).children(":nth-child(8)").find("input.GrossWeight ").val();
                        var khot = $(this).children(":nth-child(9)").find("input.Khot").val();
                        var barreturned = $(this).children(":nth-child(10)").find("input.BardanaReturned ").val();
                        var freight = Common.GetInt($(this).children(":nth-child(11)").find("input.Freight").val());
                        var barrate = Common.GetFloat($(this).children(":nth-child(12)").find("input.BardanaRate ").val());
                        var whtrate = Common.GetFloat($(this).children(":nth-child(13)").find("input.WheatRate").val());
                        var netweight = Common.GetFloat($(this).children(":nth-child(14)").find("input.NetWeight ").val());
                        var bartype = $(this).children(":nth-child(15)").find("input.BardanaType").val();
                        var netamount = Common.GetFloat($(this).children(":nth-child(16)").find("input.Amount").val());
                        Items.push({
                            Id: $(this).children(":nth-child(1)").children("#Id").val(),
                            PurchaseId: record.Id,
                            VoucherNumber: $("#VoucherNumber").val(),
                            Party: party,
                            PartyId: partyId,
                            Item: itemname,
                            ItemId: PageSetting.WheatAccountHeadId,
                            BillNo: billno,
                            Bags: bags,
                            StandardWeight: stdweight,
                            BardanaWeight: barweight,
                            TruckNo: truckno,
                            GrossWeight: grsweight,
                            Khot: khot,
                            BardanaReturned: barreturned,
                            Freight: freight,
                            BardanaRate: barrate,
                            WheatRate: whtrate,
                            NetWeight: netweight,
                            BardanaType: bartype == "P" || bartype == "p" ? BardanaType.P : BardanaType.J,
                            Amount: netamount,


                        });
                    }
                });
                if (record.IsGovt) {
                    Items = Enumerable.From(Items).Where("$.BardanaReturned > 0 || $.Amount > 0 ").ToArray();
                }
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Bags <= 0 && item.GrossWeight > 0) {
                        err += "Wheat must have bags greater than zero(0).";
                    }

                    var party = Common.GetById(item.PartyId);
                    if (typeof party == "undefined" || party == null) {
                        err += item.Party + " is not valid party code.";
                    }
                    if (item.Amount <= 0 && item.GrossWeight > 0) {
                        err += item.Party + " must have amount greater than zero(0).";
                    }
                    if (item.BardanaReturned <= 0 && item.GrossWeight <= 0) {
                        err += item.Party + " must have bardanareturned or groos weight  greater than zero(0).";
                    }

                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }

                record["WheatPurchaseItems"] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving purchase  ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.UpdateSoldRates(Items);
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
            var Bags = 0;
            var BardanaWeight = 0.0;
            var GrossQty = 0;
            var Khot = 0;
            var BardanaRate = 0.0;
            var WheatRate = 0.0;
            var amount = 0.0;
            Bags = Common.GetInt($(tr).find(":nth-child(4) input.Bags").val());
            BardanaWeight = Common.GetFloat($(tr).find(":nth-child(6) input.BardanaWeight ").val());
            GrossQty = Common.GetFloat($(tr).find(":nth-child(8) input.GrossWeight  ").val());
            Khot = Common.GetFloat($(tr).find(":nth-child(9) input.Khot").val());
            BardanaRate = Common.GetFloat($(tr).find(":nth-child(12) input.BardanaRate").val());
            WheatRate = Common.GetFloat($(tr).find(":nth-child(13) input.WheatRate").val());
            var netweight = GrossQty - ((Bags * BardanaWeight) + (Bags * Khot));
            if ($("input[value='GOVT']").is(":checked"))
                amount = netweight / 40 * WheatRate + (BardanaRate * Bags);
            else {
                if (netweight>30000)
                    amount = netweight / 40 * WheatRate - (PageSetting.WheatPurcaseWeighBridgeAmount1 + PageSetting.WheatTradeAmount) + (BardanaRate * Bags);
                else
                    amount = netweight / 40 * WheatRate - (PageSetting.WheatPurcaseWeighBridgeAmount + PageSetting.WheatTradeAmount) + (BardanaRate * Bags);
            }
            if (netweight > 0)
                $(tr).find(":nth-child(14) input.NetWeight ").val(netweight);
            else {
                $(tr).find(":nth-child(14) input.NetWeight ").val("0");
            }
            if (amount > 0)
                $(tr).find(":nth-child(16) input.Amount ").val(amount);
            else
                $(tr).find(":nth-child(16) input.Amount ").val("0");

            //var discount = Common.GetInt($(tr).find(":nth-child(6) input.DiscountPercent").val());
            //var discountAmount = Common.GetInt(amount) * discount / 100;
            //var netAmount = Common.GetInt(amount) - discountAmount;
            //$(tr).find(":nth-child(7) input.DiscountAmount ").val(discountAmount);
            //$(tr).find(":nth-child(8) input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Bags = 0;
            var StdWeight = 0.0;
            var BardanaWeight = 0.0;
            var GrossWeight = 0.0;
            var Khot = 0;
            var BardanaRetuned = 0;
            var Freight = 0;
            var NetWeight = 0.0;
            var netamount = 0.0;
            $("#purchaseitem tbody tr").each(function () {
                Bags += Common.GetInt($(this).find(":nth-child(4) input.Bags").val());
                StdWeight += Common.GetFloat($(this).find(":nth-child(5) input.StandardWeight ").val());
                BardanaWeight += Common.GetFloat($(this).find(":nth-child(6) input.BardanaWeight").val());
                GrossWeight += Common.GetFloat($(this).find(":nth-child(8) input.GrossWeight").val());
                Khot += Common.GetFloat($(this).find(":nth-child(9) input.Khot").val());
                BardanaRetuned += Common.GetInt($(this).find(":nth-child(10) input.BardanaReturned ").val());
                Freight += Common.GetInt($(this).find(":nth-child(11) input.Freight").val());
                NetWeight += Common.GetFloat($(this).find(":nth-child(14) input.NetWeight").val());
                netamount += Common.GetFloat($(this).find(":nth-child(16) input.Amount").val());
            });
            $("#purchaseitem tfoot tr").find(":nth-child(2) input.BagsTotal").val(Bags);
            $("#purchaseitem tfoot tr").find(":nth-child(3) input.StdWeightTotal ").val(StdWeight);
            $("#purchaseitem tfoot tr").find(":nth-child(4) input.BardanaWeightTotal").val(BardanaWeight);
            $("#purchaseitem tfoot tr").find(":nth-child(5) input.GrossWeightTotal").val(GrossWeight);
            $("#purchaseitem tfoot tr").find(":nth-child(6) input.KhotTotal").val(Khot);
            $("#purchaseitem tfoot tr").find(":nth-child(7) input.BardanaReturnTotal").val(BardanaRetuned);
            $("#purchaseitem tfoot tr").find(":nth-child(8) input.FreightTotal").val(Freight);
            $("#purchaseitem tfoot tr").find(":nth-child(9) input.NetWeightTotal").val(NetWeight);
            $("#purchaseitem tfoot tr").find(":nth-child(10) input.NetAmountTotal").val(netamount);
            // $("#qtytotal1").val(Quantity);
            //$("#QuantityTotal").val(Quantity);
            //if (Price === 0)
            //    $("#GrossTotal").val("");
            //else
            //    $("#GrossTotal").val(Price);
            //var incAmount = Price - discount;
            //$("#Discount").val(discount);
            //$("#NetTotal").val(incAmount + "");
            //$this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            //$("input:radio[value='PVT']").prop("checked", true);
            $.uniform.update();
            $("#saleitem tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },

        LoadDeliveryChallan: function (key) {

            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan/" + orderno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;


                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else if (d.Status == 5) {
                            Common.ShowError("Challan has already processed.");
                            SetFocus = "dcnumber";
                        }
                        else {
                            Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#qtytotal1").val(d.QuantityTotal);
                            $("#AccountCode").trigger("blur");
                            $("#Id").val(0);
                            $("#saleitem tbody").html("");
                            $("#OBNo").val(d.InvoiceNumber);
                            $("#OrderDate").val(d.Date);

                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {




                                var html = "";

                                for (var i in items) {
                                    var item = items[i];


                                    var product = $this.GetByCode(item.ItemCode);

                                    var discountpercent = 0;
                                    var price = 0;
                                    var discount = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        discountpercent = Common.GetFloat($this.GetDiscountDetail(product.AccountId));
                                        price = Common.GetFloat(product.SalePrice);

                                    }


                                    var amount = qty * price;
                                    var discountAmount = Common.GetInt(amount) * discountpercent / 100;
                                    var netAmount = Common.GetInt(amount) - discountAmount;


                                    html += "<tr>";
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden' id='Id' value='" + item.Id + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Rate form-control input-small' value='" + price + "' ></td>";
                                    html += "<td><input type='text' value='" + amount + "' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td><input type='text' value='" + discountpercent + "' class='DiscountPercent form-control input-small'></td>";
                                    html += "<td><input type='text' value='" + discountAmount + "'  class='DiscountAmount  form-control input-small' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td><input type='text' value='" + netAmount + "' class='NetAmount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"Trans.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#saleitem tbody").append(html);
                                $this.MapComments();
                                //setTimeout(function () {
                                //    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                //}, 500);

                            }
                            $this.GetWholeTotal();
                        }

                        $this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetTransporters: function () {

            var $this = this;
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);

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
            var type = $("input[value='GOVT']").is(":checked");
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key + "&voucher=" + voucherno + "&type=" + type,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading purchase  ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#purchaseitem tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);

                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            Common.SetDate($("#Date"), d.Date);
                            if (d.Id > 0) {
                                if (!d.IsGovt) {
                                    $("input:radio[value='PVT']").prop("checked", true);
                                    $.uniform.update();
                                }
                                else {
                                    $("input:radio[value='GOVT']").prop("checked", true);
                                    $.uniform.update();
                                    //if (d.Id == 0)
                                    //    $("input:radio[value='credit']").trigger("change");
                                }
                            }
                            if (d.Id > 0 && d.WheatPurchaseItems != null && d.WheatPurchaseItems.length > 0) {

                                $("#btndelete,#btnprint").prop("disabled", false);
                                var html = "";
                                var items = d.WheatPurchaseItems;
                                for (var i in items) {
                                    var item = items[i];
                                    var account = $this.GetByCodeFromCOA(item.Party);
                                    var bardanatype = Common.GetKeyFromEnum(item.BardanaType, BardanaType);
                                    var html = "<tr>";
                                    html += "<td><input type='hidden' id='Id' class='Id' value='" + item.Id + "'/><input type='hidden' class='PartyId'  value='" + item.PartyId + "'/>";
                                    html += "<input type='text' class='Party form-control typeahead input-small' data-toggle='tooltip' data-original-title='" + account.Name + "' value='" + item.Party + "'></td>";
                                    html += "<td><input type='text' disabled='disabled' class='Item form-control input-small' value='" + item.Item + "'>";
                                    html += "<input type='hidden' disabled='disabled' class='ItemId form-control input-small' value='" + item.ItemId + "'></td>";
                                    html += "<td><input type='text' class='BillNo form-control input-medium' value='" + item.BillNo + "'></td>";
                                    html += "<td><input type='text' class='Bags form-control input-small' value='" + item.Bags + "'></td>";
                                    html += "<td><input type='text' class='StandardWeight form-control input-small' value='" + item.StandardWeight + "'></td>";
                                    html += "<td><input type='text' class='BardanaWeight form-control input-small' value='" + item.BardanaWeight + "'></td>";
                                    html += "<td><input type='text' class='TruckNo form-control input-small' value='" + (item.TruckNo != null ? item.TruckNo : "") + "'></td>";
                                    html += "<td><input type='text' class='GrossWeight form-control input-small' value='" + item.GrossWeight + "'></td>";
                                    html += "<td><input type='text' class='Khot form-control input-small' value='" + item.Khot + "'></td>";
                                    html += "<td><input type='text' class='BardanaReturned form-control input-small' value='" + item.BardanaReturned + "'></td>";
                                    html += "<td><input type='text' class='Freight form-control input-small' value='" + item.Freight + "'></td>";
                                    html += "<td><input type='text' class='BardanaRate form-control input-small' value='" + item.BardanaRate + "'></td>";
                                    html += "<td><input type='text' class='WheatRate form-control input-small' value='" + item.WheatRate + "'></td>";
                                    html += "<td><input type='text' disabled='disabled' class='NetWeight form-control input-small' value='" + item.NetWeight + "'></td>";
                                    html += "<td><input type='text' maxlength='1' class='BardanaType form-control input-small' value='" + bardanatype + "'></td>";
                                    html += "<td><input type='text' class='Amount form-control input-small' disabled='disabled' readonly='readonly' value='" + item.Amount + "'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"WheatPurchase.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                    $("#purchaseitem tbody").append(html);
                                    //$("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(15) select.BardanaType").val(item.BardanaType);
                                    //$("#purchaseitem tbody tr:nth-last-child(1) td:nth-child(15) select.BardanaType").select2();
                                    //$("#purchaseitem tbody").find("tr:last-child select.'BardanaType").val(BardanaType);

                                }

                                setTimeout(function () {
                                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Party").focus().select();
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
        GetCustomerProducts: function (id) {
            var $this = this;
            AppData.CustomerDiscount = PageSetting.Discounts;
        },
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
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
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?voucher=" + voucherno;
                var id = $("#purchaseitem tbody tr:nth-child(1) td:nth-child(1) input.Id").val();
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
                    blockMessage: "Deleting purchase  ...please wait",
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
        ShowAttachments: function (el) {
            $("#dialogAttachments").addClass("in");
        },
        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            id = PageSetting.Suppliers;
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

            this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function () {
            var $this = this;
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var accounts = Common.GetAllLeafAccounts(exids);
            //var accounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var suggestion = new Array();
            for (var i in accounts) {
                var account = accounts[i];
                suggestion.push(
                    {
                        id: account.Id,
                        value: account.AccountCode,
                        label: account.AccountCode + "-" + account.DisplayName

                    }
                );
            }

            $(".Party").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).parent().parent();
                    var itemId = $(tr).find("input.ItemId").val();
                    // $(tr).find("td:nth-child(1) input.PartyId").val(ui.item.id);
                    var account = $this.GetByCodeFromCOA(ui.item.value);
                    var rate = $this.GetSoldRateByPartyId(ui.item.id, itemId);
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find(":nth-child(1) input.PartyId").val(account.Id);
                        $(tr).find("input.WheatRate ").val(rate);
                        $(this).attr('data-toggle', 'tooltip');
                        $(this).attr('data-original-title', account.Name);
                        // $(tr).find(":nth-child(2) input.PartyName").val(account.Name);
                        $this.GetQuantityPriceTotal(tr);
                        $(".container-message").hide();
                    }

                }
            });

        },
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};