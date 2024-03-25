
var BankReceipts = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "BankReceipt";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $("#PaymentDate").change(function () {


                $("#item-container tbody").find("tr").each(function () { //get all rows in table

                    var tr = $(this);
                    var paymentDate = $("#PaymentDate").val();
                    var challanNumber = Common.GetInt($(tr).find(".ChallanNumber").val());
                    var paymentDate = moment(paymentDate, 'DD/MM/YYYY');
                    Common.SetDate($(this).find("input.ReceiveDate"), paymentDate);
                    if (challanNumber > 0)
                        $this.CalculateSurcharge(tr);

                });
            });
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                }
            });
            $(document).on("keyup change", ".ReceiveDate", function (event) {
                var tr = $(this).closest("tr");
                $this.CalculateSurcharge(tr);
            });
            $(document).on("keyup change", ".LateSurCharge", function (event) {
                var tr = $(this).closest("tr");
                $this.CalculateNetAmount(tr);
            });
            $this.Add();
            //$this.LoadChallan();

            $(document).on("keyup", ".ChallanNumber ", function (event) {
                var tr = $(this).closest("tr");
                var challanNumber = $(tr).find("input.ChallanNumber").val();
                if (event.keyCode == 13)
                    $this.GetChallInfo(tr, challanNumber);
                if ($(tr).is(':last-child') && event.keyCode == 13 && challanNumber != 0) {
                    $this.AddItem();
                }
            });
            $(document).on("keyup", ".LateSurCharge ", function (event) {
                var tr = $(this).closest("tr");
                $this.CalculateNetAmount(tr);

            });
        },
        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        GetChallInfo: function (tr, challanNumber) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?loadchallan=true&challanNumber=" + challanNumber + "&key=chInfo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading challan...please wait",
                success: function (res) {
                    if (res.Success) {
                        var record = res.Data.Challans;
                        if (record == null || record == undefined)
                            return;

                        var challanItem = record.ChallanItems[0];
                        $.extend(true, challanItem, record);
                        var comment = "";
                        if (challanItem.TransactionType == VoucherType.RC)
                            comment = "Rent receiving ";
                        else if (challanItem.TransactionType == VoucherType.securitymoney)
                            comment = "Security receiving ";
                        else if (challanItem.TransactionType == VoucherType.possessioncharges)
                            comment = "Possession receiving ";
                        else if (challanItem.TransactionType == VoucherType.misccharges)
                            comment = "Misc Charges receiving ";


                        challanItem["Description"] = comment + challanItem.TenantAccountName + " against shop no. " + challanItem.ShopNo;
                        challanItem.ReceiveDate = moment($("#PaymentDate").val(), "DD/MM/YYYY");
                        Common.MapItemDataBySelector(challanItem, tr);
                        $this.CalculateSurcharge(tr);
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

        LoadChallan: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?loadchallan=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Receipts...please wait",
                success: function (res) {
                    if (res.Success) {
                        AppData.Challans = res.Data.Challans;
                        $this.ChallanAutoComplete();
                        //$this.AddItem();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $this.CustomClear();
            $this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {

            var $this = this;
            focusElement = "";

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ChallanNumber").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ChallanNumber").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.ChallanNumber").focus().select();
                }, 300);
                focusElement = "";
            }

            Common.InitDateMask();
            Common.InitDatePicker();
            $this.ChallanAutoComplete();
        },
        DeleteRow: function (elment) {
            var $this = this;
            $this.DeleteByChallanId(elment)
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
                //$this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                //$this.CustomClear();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;

            var scope = $("#form-info");
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.Id > 0").ToArray();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: { '': Items },
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  record ...please wait",
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
        GetWholeTotal: function () {
            var $this = this;
            var Amount = 0.0
            $("#item-container tbody tr").each(function () {
                Amount += Common.GetFloat($(this).find("input.Fine").val());
            });
            $("#item-container tfoot tr").find("input.").val(Amount.toFixed(2));
            $("#NetTotal").val(Amount);


        },
        CustomClear: function () {
            var $this = this;
            AppData.Challans = new Array();
            $("#item-container :input,.btnSave").prop("disabled", false);
            Common.Clear();
            $this.AddItem();
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
                blockMessage: "Loading  record ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        var challans = res.Data.Challans;
                        $("#VoucherNumber").val(res.Data.VoucherNumber);
                        if (challans == null || challans.length <= 0) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            var voucherNo = challans[0].RcvNo;
                            $("#VoucherNumber").val(voucherNo);
                            var items = new Array();
                            for (var i in challans) {

                                var challan = challans[i];
                                var challanItem = challan.ChallanItems[0];
                                //challanItem.ChallanNumber = challan.Id;
                                $.extend(true, challanItem, challan);
                                var comment = "";
                                if (challanItem.TransactionType == VoucherType.RC)
                                    comment = "Rent receiving ";
                                else if (challanItem.TransactionType == VoucherType.securitymoney)
                                    comment = "Security receiving ";
                                else if (challanItem.TransactionType == VoucherType.possessioncharges)
                                    comment = "Possession receiving ";
                                else if (challanItem.TransactionType == VoucherType.misccharges)
                                    comment = "Misc Charges receiving ";


                                challanItem["Description"] = comment + challanItem.TenantAccountName + " against shop no. " + challanItem.ShopNo;
                                items.push(challanItem);
                            }
                            Common.MapItemData(items, "#item-container", "#template-item", true);
                            $("#item-container :input,.btnSave").prop("disabled", true);
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");

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
            var id = $("#VoucherNumber").val();
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=byvoucherNo",
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting  record ...please wait",
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
        DeleteByChallanId: function (elment) {
            var $this = this;
            var challanId = Common.GetInt($(elment).closest("tr").find("td:first-child input.ChallanNumber").val());
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + challanId + "?key=byId",
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting  record ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $(elment).closest("tr").remove();
                            $this.GetWholeTotal();
                            if ($("#item-container tbody").children().length <= 0)
                                $this.AddItem();


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
            $("#account-type-conatiner").addClass("hide");
            //this.LoadAccounts();
        },
        Print: function () {
            $("#div-report").removeClass('hidden-print');
            $("#div-gstreport").addClass('hidden-print');
            window.print();
        },

        ChallanAutoComplete: function () {
            var $this = this;
            var challans = AppData.Challans;
            var suggestion = new Array();
            for (var i in challans) {
                var challan = challans[i];
                suggestion.push
                    (
                    {
                        id: challan.Id,
                        value: challan.Id,
                        label: challan.Id + "-" + challan.TenantAccountName + "-" + Common.GetKeyFromEnum(challan.TransactionType, VoucherType),
                    }
                    );
            }
            $(".ChallanNumber").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var record = Enumerable.From(AppData.Challans).Where(function (e) { return e.Id == ui.item.id }).FirstOrDefault();
                    var tr = $(this).closest("tr");
                    $(tr).find("input.TenantAccountId").val(record.TenantAccountId);
                    $(tr).find("input.TenantAccountName").val(record.TenantAccountName);
                    $(tr).find("input.ShopNo").val(record.ShopNo);
                    $(tr).find("input.Description").val(record.Description);
                    $(tr).find("input.MonthlyRent").val(record.MonthlyRent);
                    $(tr).find("input.UCPercent").val(record.UCPercent);
                    $(tr).find("input.RentArrears").val(record.RentArrears);
                    $(tr).find("input.UCPercentArears").val(record.UCPercentArears);
                    $(tr).find("input.ArrearUnit").val(record.ArrearUnit);
                    $(tr).find("input.Unit").val(record.Unit);
                    $(tr).find("input.ElectricityCharges").val(record.ElectricityCharges);
                    $(tr).find("input.ElectricityArrears").val(record.ElectricityArrears);
                    $(tr).find("input.NetAmount").val(record.NetAmount);
                }
            });
        },

        CalculateSurcharge: function (tr) {
            var $this = this;
            var transactionType = $(tr).find("input.TransactionType").val();
            if (transactionType == VoucherType.RC) {
                var paiddate = $(tr).find("input.ReceiveDate").val();
                var amount = Common.GetInt($(tr).find("input.MonthlyRent").val()) + Common.GetInt($(tr).find("input.UCPercent").val()) + Common.GetInt($(tr).find("input.ElectricityCharges").val());
                var arrearAmount = Common.GetInt($(tr).find("input.RentArrears").val()) + Common.GetInt($(tr).find("input.UCPercentArears").val())
                    + Common.GetInt($(tr).find("input.ElectricityArrears").val()) + Common.GetInt($(tr).find("input.SurCharge").val());
                var dueDate = moment($(tr).find("input.DueDate").val(), 'DD/MM/YYYY');
                var paidDate = moment(paiddate, 'DD/MM/YYYY');
                var days = paidDate.diff(dueDate, 'days');
                days = days <= 0 ? 0 : days;
                var surcharge = 0.05;
                var totalSurcahrge = 0;
                if (days > 0)
                    totalSurcahrge = Common.GetCeilInt(surcharge * amount);
                $(tr).find("input.LateSurCharge").val(totalSurcahrge);
                var netAmount = Common.GetCeilInt(totalSurcahrge + amount + arrearAmount);
                $(tr).find("input.NetAmount").val(netAmount);
            }
            if (transactionType == VoucherType.electictychallan) {
                var paiddate = $(tr).find("input.ReceiveDate").val();
                var arrear = Common.GetInt($(tr).find("input.ElectricityArrears").val());
                var previoussurcharge = Common.GetInt($(tr).find("input.SurCharge").val());
                var amount = Common.GetInt($(tr).find("input.ElectricityCharges").val());
                var dueDate = moment($(tr).find("input.DueDate").val(), 'DD/MM/YYYY');
                var paidDate = moment(paiddate, 'DD/MM/YYYY');
                var days = paidDate.diff(dueDate, 'days');
                days = days <= 0 ? 0 : days;
                var surcharge = 0.05;
                var totalSurcahrge = 0;
                if (days > 0)
                    totalSurcahrge = Common.GetCeilInt(surcharge * amount);
                $(tr).find("input.LateSurCharge").val(totalSurcahrge);
                var netAmount = Common.GetCeilInt(totalSurcahrge + amount + arrear + previoussurcharge);
                $(tr).find("input.NetAmount").val(netAmount);
            }
            else
                $this.CalculateNetAmount(tr);


        },
        CalculateNetAmount: function (tr) {

            var transactionType = $(tr).find("input.TransactionType").val();
            if (transactionType == VoucherType.RC) {
                var amount = Common.GetInt($(tr).find("input.MonthlyRent").val()) + Common.GetInt($(tr).find("input.UCPercent").val()) + Common.GetInt($(tr).find("input.ElectricityCharges").val());
                var arrearAmount = Common.GetInt($(tr).find("input.RentArrears").val()) + Common.GetInt($(tr).find("input.UCPercentArears").val())
                    + Common.GetInt($(tr).find("input.ElectricityArrears").val()) + Common.GetInt($(tr).find("input.SurCharge").val());
                totalSurcahrge = Common.GetInt($(tr).find("input.LateSurCharge").val());
                var netAmount = Common.GetCeilInt(totalSurcahrge + amount + arrearAmount);
                $(tr).find("input.NetAmount").val(netAmount);
            }
            else if (transactionType == VoucherType.electictychallan) {
                var amount = Common.GetInt($(tr).find("input.ElectricityCharges").val());
                totalSurcahrge = Common.GetInt($(tr).find("input.LateSurCharge").val());
                var netAmount = Common.GetCeilInt(totalSurcahrge + amount);
                $(tr).find("input.NetAmount").val(netAmount);
            }
        }
    };
}();

