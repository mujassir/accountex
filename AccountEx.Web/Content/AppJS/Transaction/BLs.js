
var BLs = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "BL";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            Common.BindShortKeys($this);

            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                }
            });

            $("#TotalUnits").keyup(function (e) {

                $this.CalculateUnits();

            });


            $("#Date").keyup(function (e) {
                if (e.which == 13) {
                    if (type == "sale")
                        $("#DCNo").focus();
                    else
                        $("#ShipperCode").focus();
                }
            });
            $("#Date").focus(function (e) {
                $(this).select();
            });
            $("#InvoiceNumber").keyup(function (e) {
                if (e.which == 13) {
                    $("#Date").focus();
                }
                $this.MapComments();
            });
            $("#InvoiceNumber").keyup(function (e) {
                $this.MapComments();
            });
            $("#ShipperCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = +$(this).val() + " is not valid party code.,";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("keyup", ".ChassisNo", function (event) {

                var tr = $(this).closest("tr");
                var ChassisNo = $(tr).find("input.ChassisNo").val();
                if (event.which == 13 && ChassisNo != "") {
                    $this.AddItem();
                }

                $this.CalculateUnits();

            });
            $(document).on("change", "#ShipperId", function (event) {
                $this.LoadShips();
            });




            $("#ShipperCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
                    $("#ShipperId").val(party.Id);
                    //$this.GetPreviousBalance(party.Id);
                    $(".container-message").hide();
                    var address = party.Address;
                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid party code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $this.LoadPageSetting();

            $(document).on("click", "#select-dc", function () {
                $this.AddSelectedChallans();
            });

            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.AddItem();
            $this.DrawExpensesTable();
            $this.VehicleAutoCompleteInit();
            $this.ListView();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                this.Add();
                //} else {
                //    this.ListView();
                //}
            }
        },
        MapComments: function () {
            var $this = this;
        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        ListView: function () {
            var $this = this;
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }

            $("#Name").focus();
        },
        AddItem: function () {
            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.ChassisNo").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.CalculateUnits();
            $this.AutoCompleteInit();
            $this.VehicleAutoCompleteInit(PageSetting.Vehicles);
            Common.InitNumerics();
        },

        DeleteRow: function (element) {
            var $this = this;
            var tr = $(element).closest("tr");
            var Id = $(tr).find("input.Id").val();
            if (Id > 0) {
                Common.ConfirmDelete(function () {
                    var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + Id + "?key=DeleteSinglevehicle";

                    Common.WrapAjax({
                        url: url,
                        type: "DELETE",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        blockUI: true,
                        blockElement: "#form-info",
                        blockMessage: "Deleting ...please wait",
                        success: function (res) {
                            if (res.Success) {
                                $(tr).remove();
                                Common.ShowMessage(true, { message: "Record deleted successfully." });
                            }
                            else {
                                Common.ShowError(res.Error);
                            }

                        },
                        error: function (e) {
                        }
                    });
                });

            }

            else {
                $(elment).closest("tr").remove();
            }
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
            $this.CalculateUnits();
        },

        CalculateUnits: function () {
            var $this = this;
            var added = 0;
            $("#item-container tbody tr").each(function () {
                if (Common.GetInt($(this).find("input.Id").val()) > 0)
                    added++;
            });
            var total = Common.GetInt($("#TotalUnits").val());
            var remaining = total - added;
            $("#AddedUnits").val(total);
            $("#RemainingUnits").val(remaining);
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                DataTable.RefreshDatatable(DATATABLE_ID);
                Common.ShowMessage(true, { message: Messages.RecordSaved });

            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            var BLCharges = new Array();


            if (id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                $("#item-container tbody input.Rate").trigger("keyup");

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ChassisNo.trim()!=''").ToArray();
                var err = "";
                var party = Common.GetByCode($("#SupplierCode").val());
                if (typeof party == "undefined" || party == null) {
                    err += "" + $("#SupplierCode").val() + " is not valid supplier code.,";
                }

                for (var i in Items) {
                    var item = Items[i];
                    if (item.VehicleId == 0) {
                        err += "Item " + item.ChassisNo + "(" + item.BrandName + ") is not valid vehicle to add.,";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one vehicle.,";
                }
                record["BlItems"] = Items;
                BLCharges = Common.SaveItemData("#tbl-blcharges");
                BLCharges = Enumerable.From(BLCharges).Where("Common.GetInt($.Amount)>0").ToArray();

                for (var i in BLCharges) {
                    var item = BLCharges[i];
                    if (item.SupplierId == 0) {
                        err += "Suppleir must be selected for  " + item.ChargeName + " when amount is greater than zero(0).,";
                    }
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["BLCharges"] = BLCharges;
                //var BLExtra =
                //    {
                //        BL: record,
                //        BLItemExtra: Items
                //    }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  Bill of Lading...please wait",
                    success: function (res) {
                        if (res.Success) {

                            if (record.Id > 0 && $("#ChkGetNextVoucher").is(":checked")) {
                                $("#VoucherNumber").val(Common.GetInt(record.VoucherNumber) + 1);
                                $this.LoadVoucher("same");
                            }
                            else {
                                $this.GetNextVoucherNumber();
                            }

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
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
            Rate = Common.GetFloat($(tr).find("input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.Amount").val(amount.toFixed(2));
            var discount = Common.GetFloat($(tr).find("input.DiscountPercent").val());
            var discountAmount = Common.GetFloat(amount) * discount / 100;
            var netAmount = Common.GetInt(amount - discountAmount);
            $(tr).find("input.DiscountAmount ").val(discountAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var discount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find("input.Quantity").val());
                Price += Common.GetFloat($(this).find("input.Amount").val());
                discount += Common.GetFloat($(this).find("input.DiscountAmount").val());
                netamount += Common.GetInt($(this).find("input.NetAmount").val());
            });
            $("#item-container tfoot tr").find("input.Quantity").val(Quantity.toFixed(2));
            $("#item-container tfoot tr").find("input.Amount").val(Price.toFixed(2));
            $("#item-container tfoot tr").find("input.DiscountAmount").val(discount.toFixed(2));
            $("#item-container tfoot tr").find("input.NetAmount").val(netamount);
            $("#qtytotal1").val(Quantity.toFixed(2));
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var incAmount = Common.GetInt(Price) - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(Common.GetInt(incAmount));
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
            var $this = this;
            $("input:radio[value='credit']").prop("checked", true);

            $("#item-container tbody tr").each(function () {

                $(tr).find("input").removeClass("hide");
                $(tr).find("span.value-conatiner").addClass("hide");

            });
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#ShipperCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            $this.SetFinalButton(false, 0);
            Common.Clear();
        },
        Edit: function (id, voucherNo) {
            var $this = this;
            $("#VoucherNumber").val(voucherNo);
            $this.LoadVoucher("same");
        },
        DrawExpensesTable: function (noOfInstallments) {
            var $this = this;
            var html = "";
            $("#item-container tbody").html("");
            var records = $this.GetBlExpenses();
            Common.MapItemData(records, "#tbl-blcharges", "#template-blcharges", true);
            Common.InitDateMask();
            Common.InitNumerics();
        },
        GetBlExpenses: function (noOfInstallments) {
            var html = "";

            var accounts = Common.GetLeafAccounts(PageSetting.BLExpensesHead);
            var records = new Array();
            for (var i in accounts) {
                var account = accounts[i];
                records.push(
                    {
                        ChargeId: account.Id,
                        ChargeName: account.AccountCode + "-" + account.DisplayName,

                    }
                );
            }
            return records;
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Voucher ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;

                        Common.MapEditData(d, "#form-info");
                        if (d != null) {
                            $this.LoadShips(d.ShipId);
                        }
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $("#DCNo").prop("disabled", "disabled");
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            $this.SetFinalButton(d.IsFinal, d.Id);
                            if (d.Id > 0 && d.BLItems != null && d.BLItems.length > 0) {
                                var items = d.BLItems;
                                for (var i in items) {
                                    var item = items[i];
                                    var vehicles = $.parseJSON(JSON.stringify(res.Data.Vehicles));
                                    var vehicle = Enumerable.From(vehicles).Where(function (p) { return p.Id == item.VehicleId }).FirstOrDefault();
                                    if (vehicle != null) {
                                        //delete vehicle.Id;
                                        vehicle["VehicleId"] = item.VehicleId;
                                        vehicle["Id"] = item.Id;
                                        $.extend(true, item, vehicle);
                                    }
                                }
                                var charges = $this.GetBlExpenses();
                                var dbCharges = d.BLCharges;
                                for (var i in charges) {
                                    var charge = charges[i];
                                    var dbCharge = Enumerable.From(dbCharges).Where(function (p) { return p.ChargeId == charge.ChargeId }).FirstOrDefault();
                                    if (dbCharge != null) {
                                        $.extend(true, charge, dbCharge);
                                    }
                                }
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                Common.MapItemData(items, null, null, true);
                                $("#tbl-blcharges tbody").html("");
                                Common.MapItemData(charges, "#tbl-blcharges", "#template-blcharges", true);
                                Common.InitDateMask();
                                Common.InitNumerics();
                                $this.CalculateUnits();
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $("#item-container tbody tr").each(function () {
                                    var tr = $(this);
                                    var $vehicleId = $(tr).find("input.Id");
                                    var Id = $vehicleId.val();
                                    if (Id > 0) {

                                        $(tr).find("input.ChassisNo").prop("disabled", true);
                                        $(tr).find("input").addClass("hide");
                                        $(tr).find("span.value-conatiner").removeClass("hide");
                                    }

                                });
                                $("#tbl-blcharges tbody tr").each(function () {
                                    var tr = $(this);
                                    var isPaid = Common.GetBool($(tr).find("input.IsPaid").val());
                                    if (isPaid) {

                                        $(tr).find("input.Amount").prop("disabled", true);
                                        $(tr).find("select.SupplierId").prop("disabled", true);
                                        $(tr).attr("title", "This invoice is paid and can't be updated.")

                                    }

                                });
                                //$this.GetWholeTotal();
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
        Final: function (isfinal) {
            var $this = this;
            $(".container-message").hide();
            $(".container-message").hide();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?isfinal=" + isfinal,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "finalizing...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "BL is final successfully." });
                        SetFocus = "date";
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.SetFinalButton(isfinal, id);

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
            // }
        },


        SetFinalButton: function (isfinal, id) {
            var $this = this;
            $(".btnUnFinal,.btnUnFinal").addClass("hide");
            if (isfinal) {
                $(".btnUnFinal").removeClass("hide");
                $(".btnSave,.btnDelete,.btnFinal").addClass("hide");
            }
            else {
                $(".btnSave,.btnDelete").removeClass("hide");
                if (id > 0) {
                    $(".btnFinal").removeClass("hide");
                }
                $(".btnUnFinal").addClass("hide");
            }

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
            //var type = VoucherType[$this.GetType()];

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var id = Common.GetInt($("#Id").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + id
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;
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
                    blockMessage: "Deleting BL ...please wait",
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


        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }


        },
        Print: function (allowAging) {
            if (allowAging) {
                $("#tblAgingItems").removeClass("hide");
            }
            else {
                $("#tblAgingItems").addClass("hide");
            }
            window.print();
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Suppliers);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName,
                        name: product.DisplayName

                    }
                );
            }
            $("#SupplierCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var shipper = Common.GetAccountDetailByAccountId(account.Id);
                        $("#SupplierName").val(account.Name);
                        $("#SupplierId").val(account.Id);
                        $("#Address").val(shipper.Address);
                    }
                }
            });
        },
        LoadShips: function (shipId) {

            var shipperId = Common.GetInt($("#ShipperId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GettingShips&shipperId=" + shipperId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading ships..please wait",
                success: function (res) {
                    if (res.Success) {
                        var ships = res.Data;
                        Common.BindSelect(ships, "#ShipId", true);
                        if (typeof shipId != "undefined" || shipId != null)
                            $("#ShipId").select2("val", shipId);

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        },
        VehicleAutoCompleteInit: function (data) {
            var $this = this;
            var tokens = data;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                var label = "Chessis No:" + token.ChassisNo + " RegNo No:" + token.RegNo + " Manufacturer:" + token.Manufacturer;
                label += " Color:" + token.Color + " CC:" + token.EnginePower + " Model:" + token.Model + " Car TYPE:" + token.CarType;
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.ChassisNo,
                        label: label,
                        Vehicle: token
                    }
                );
            }

            $(".ChassisNo").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.VehicleId").val(ui.item.id);

                    delete ui.item.Vehicle.Id;
                    Common.MapEditData(ui.item.Vehicle, tr, true)
                    $this.CalculateUnits(tr);
                },
            });
        }
    }
}();