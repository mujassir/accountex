
var VehicleRecovery = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VehicleRecovery";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    var CURRENT_ROW = null;
    var SALE = null;
    var BRANCHID = 0;
    return {
        init: function () {
            var $this = this;
            BRANCHID = $("#BranchId").val();
            $(document).on("change", "select.StatusId", function (event) {
                var tr = $(this).closest("tr");
                var isFinal = $(tr).find("select.StatusId option:selected").attr("data-isfinal");
                if (isFinal == "True") {
                    $(tr).find("select.AssignedBranchId,input.PurchasePrice,input.SalePrice").prop("disabled", false);
                }
                else {
                    $(tr).find("select.AssignedBranchId,input.PurchasePrice,input.SalePrice").prop("disabled", true);
                }

            });
            $(document).on("change", "#PaymentMode", function () {
                var paymentOptions = $(this).val();
                var scope = $(this).closest("div[data-save='save']")
                if (paymentOptions != "Bank") {
                    $(".banks-options", scope).addClass("hide");
                }
                else {
                    $(".banks-options", scope).removeClass("hide");
                }
            });
            $(document).on("keyup change", "#Discount,#Amount", function () {
                $this.CalculateDiscount();
            });
            $(document).on("keyup change", "#reovery-modal-container #Amount", function () {
                var amount = Common.GetInt($(this).val());
                if (amount > 0) {
                    Common.UpdateRequired("#Amount", true);
                    Common.UpdateRequired("#RecievedDate", true);
                    Common.UpdateRequired("#AccountId", true);

                }
                else {
                    Common.UpdateRequired("#Amount", false);
                    Common.UpdateRequired("#RecievedDate", false);
                    Common.UpdateRequired("#AccountId", false);
                }
            });
            $this.LoadRecoveryDetail();
            $this.LoadPageSetting();
            $this.LoadBanks();
            $this.LoadSupplier();
            $("#BranchId").change(function () {
                LIST_LOADED = false;
                DataTable.DestroyDatatable(DATATABLE_ID);
                $this.init();
            });
        },

        ViewHistory: function (td) {
            var tr = $(td).closest("tr");
            var vehicleId = $(tr).find("input.VehicleId").val();
            var customerId = $(tr).find("input.AccountId").val();
            var saleId = $(tr).find("input.SaleId").val();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?vehicleId=" + vehicleId + "&customerId=" + customerId + "&saleId=" + saleId,
                type: "GET",
                blockUI: true,
                blockElement: "#item-container",
                blockMessage: "Loading history...please wait",
                success: function (res) {
                    if (res.Success) {
                        var saleDetails = Enumerable.From(PageData).Where(function (p) { return p.SaleId == saleId }).FirstOrDefault();
                        var recoveryStatus = saleDetails.RecoveryStatus;

                        //var label = "Chessis No:" + token.ChassisNo + " RegNo No:" + token.RegNo + " Brand:" + token.BrandName;
                        //label += " Color:" + token.Color + " CC:" + token.EnginePower + " Model:" + token.Model + " Car TYPE:" + token.CarType;
                        var followUps = res.Data.FollowUps;
                        var auction = res.Data.Auction;
                        var html = "";
                        for (var i in followUps) {
                            var item = followUps[i];
                            html += "<tr>";
                            html += "<td>" + (item.Date != null ? moment(item.Date).format("DD/MM/YYYY") : '') + "</td>";
                            html += "<td>" + item.Remarks + "</td>";
                            html += "<td>" + (item.NextFollowUp != null ? moment(item.NextFollowUp).format("DD/MM/YYYY") : '') + "</td>";
                            html += "</tr>";
                        }
                        $("#tbl-followUps tbody").html(html);


                        var scope = $("#modal-history");
                        $(scope).find("span[class*='lblAu']").html("");
                        if (auction != null) {
                            var auctionner = Common.GetById(auction.AcutionerId);
                            Common.MapDataWithPrefixFClasses(auction, scope, "lblAu", "html");
                            if (auctionner != null) {
                                $('span.lblAuAuctionner').text(auctionner.Name);
                            }
                        }
                        var status = "Recovery not Started";
                        switch (recoveryStatus) {
                            case RecoveryStatus.InProcess:
                                status = "Recovery Started";
                                break;
                            case RecoveryStatus.Recovered:
                                status = "Vehicle Repossessed";
                                break;
                            case RecoveryStatus.InventoryReturn:
                                status = "Vehicle Returned to Inventory";
                                break;
                            case RecoveryStatus.CustomerReturn:
                                status = "Vehicle Returned to Customer";
                                break;
                            case RecoveryStatus.Advertisement:
                                status = "Advertisement Started";
                                break;
                            case RecoveryStatus.NotficationLetter:
                                status = "Notfication Letter Issued";
                                break;
                            default:

                        }
                        $('span.lblAuStatus').text(status);

                        $("#modal-history").modal("show");
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.LoadBLStatuses();

            });
        },
        CalculateDiscount: function () {
            var $this = this;
            var totalOutstandingAmount = Common.GetInt($("#TotalOutStanding").val());
            var totalAmount = Common.GetInt($("#Amount").val());
            //var discount = Common.GetInt($("#Discount").val());
            //$("#TotalAmount").val(Common.GetInt(totalAmount - discount));
            $("#Discount").val(totalOutstandingAmount - totalAmount);
            $("#TotalAmount").val(totalOutstandingAmount - totalAmount);
        },
        SaveRecord: function (callback) {

            var $this = this;
            var mode = "add";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            Items = Common.SaveItemData();
            var err = "";
            if (Items.length <= 0) {
                err += "Please add atleast one item.,";
            }

            if (err.trim() != "") {
                Common.ShowError(err);
                return;
            }
            record[""] = Items
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Updating Statuses...please wait",
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

        },

        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadRecoveryDetail: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?branchId=" + BRANCHID,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Recovery Detail ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        Common.MapItemData(res.Data, null, null, true);
                        PageData = res.Data;
                        $this.CheckFinalStatus();

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },


        CheckFinalStatus: function () {
            var $this = this;
            $("#item-container tbody tr").each(function () {
                var tr = $(this);
                $(tr).find("td.td-actions").find(":button").addClass("hide").prop("disabled", true);
                var vehicleId = $(tr).find("input.VehicleId").val();
                var accountId = $(tr).find("input.AccountId").val();
                var saleDetails = Enumerable.From(PageData).Where(function (p) { return p.AccountId == accountId && p.VehicleId == vehicleId }).FirstOrDefault();
                var status = saleDetails.RecoveryStatus;
                $this.UpdateStatus(tr, status);
            });
        },
        UpdateStatus: function (tr, status) {
            $(tr).find("td.td-actions :button").addClass("hide").prop("disabled", true);
            if (status == RecoveryStatus.Default) {
                $(tr).find("td.td-actions").find(".btn-recovery-process").removeClass("hide").prop("disabled", false);
            }
            else if (status == RecoveryStatus.InProcess || status == RecoveryStatus.PrintPossession) {
                $(tr).find("td.td-actions").find(".btn-recovery-recovered,.btn-recovery-print,.btn-recovery-customer-return").removeClass("hide").prop("disabled", false);
                $(tr).find("td.td-actions").find(".btn-recovery-process").addClass("hide").prop("disabled", true);
                $("#modal-possession-letter").modal("hide");


            }
            else if (status == RecoveryStatus.Recovered) {
                $(tr).find("td.td-actions").find(".btn-recovery-customer-return,.btn-recovery-notifictaion-letter,.btn-recovery-final-charges").removeClass("hide").prop("disabled", false);
            }
            else if (status == RecoveryStatus.NotficationLetter || status == RecoveryStatus.PrintNotficationLetter) {
                $(tr).find("td.td-actions").find(".btn-recovery-customer-return,.btn-recovery-advertisement,.btn-recovery-print-notification-letter,.btn-recovery-final-charges").removeClass("hide").prop("disabled", false);
                $("#modal-notification-letter").modal("hide");
            }
            else if (status == RecoveryStatus.Advertisement) {
                $(tr).find("td.td-actions").find(".btn-recovery-inventory-return,.btn-recovery-customer-return,.btn-recovery-final-charges").removeClass("hide").prop("disabled", false);
                $("#modal-advertisement").modal("hide");
            }
            else if (status == RecoveryStatus.InventoryReturn) {
                $(tr).remove();
            }
            else if (status == RecoveryStatus.CustomerReturn || status == RecoveryStatus.PrintFurtherAgreement) {
                $("#insatallment-container tbody").html("");
                $(tr).find("td.td-actions").find(".btn-recovery-print-further-agreement,.btn-recovery-final-charges").removeClass("hide").prop("disabled", false);
            }



        },
        ConfirmInventoryReturn: function (element, status) {
            var $this = this;
            if (confirm("Are you sure to return this vehicle to inventory?")) {
                $this.ProcessRecovery(element, status);
                return true;
            }
            else
                return false;
        },
        ProcessRecovery: function (element, status) {

            var $this = this;

            var scope = $("#print-container");
            var tr = $(element).closest("tr");
            CURRENT_ROW = tr;
            var vehicleId = $(tr).find("input.VehicleId").val();
            var accountId = $(tr).find("input.AccountId").val();
            var saleId = $(tr).find("input.SaleId").val();
            var saleDetails = Enumerable.From(PageData).Where(function (p) { return p.SaleId == saleId }).FirstOrDefault();
            if (status == -1) {
                Common.MapDataWithPrefixF(saleDetails, scope, "lbl", "html");
                window.print();
            }
            else if (status == RecoveryStatus.InProcess) {
                $("#modal-possession-letter").modal("show");
            }
            else if (status == RecoveryStatus.Advertisement) {
                $("#modal-advertisement").modal("show");
            }
            else if (status == RecoveryStatus.NotficationLetter) {
                $("#modal-notification-letter").modal("show");
            }
                //else if (status == RecoveryStatus.CustomerReturn) {
                //    $("#modal-settlement").modal("show");
                //}

            else {

                var qs = "?id=" + saleDetails.SaleId;
                qs += "&status=" + status;
                if (Common.Validate($("#form-info"))) {
                    var record = Common.SetValue($("#form-info"));
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "POST",
                        data: record,
                        blockUI: true,
                        blockElement: "#form-info",
                        blockMessage: "Processing recovery...please wait",
                        success: function (res) {
                            if (res.Success) {
                                $(tr).find("td.td-actions").find(":button").addClass("hide").prop("disabled", true);
                                var dbSale = SALE = res.Data.Sale;
                                saleDetails.RecoveryStatus = dbSale.RecoveryStatus;
                                $this.UpdateStatus(CURRENT_ROW, status);

                                if (status == RecoveryStatus.FinalAuctionnerCharges) {
                                    var scope = $("#modal-auctionner-charges-finalization");
                                    $("#Charges", scope).val(res.Data.Auction.Charges);
                                    Common.SetDate($("#AuctionerFinalizationDate", scope), res.Data.Auction.AuctionerFinalizationDate);
                                    //Common.MapEditData(res.Data.Auction, "#modal-auctionner-charges-finalization");
                                    $("#Charges", scope).val(res.Data.Auction.Charges);
                                    $("#modal-auctionner-charges-finalization").modal("show");
                                    status = dbSale.RecoveryStatus;
                                    $this.UpdateStatus(CURRENT_ROW, dbSale.RecoveryStatus);
                                }
                                else if (status == RecoveryStatus.CustomerReturn) {
                                    $("#insatallment-container tbody").html("");
                                    var installments = dbSale.VehicleSaleDetails;
                                    for (var i in installments) {
                                        var item = installments[i];
                                        item["RecievedAmount"] = item.RecievedAmount.format();
                                        item["InstallmentDate"] = moment(item.InstallmentDate).format("DD/MM/YYYY");
                                        var currentStatusHtml = "<span class='label label-sm label-info'>Pending</span>"
                                        if (item.IsRecieved) {
                                            currentStatusHtml = "<span class='label label-sm label-success'>Paid</span>"
                                        }

                                        else {
                                            var today = new Date();
                                            today.setHours(0, 0, 0, 0);

                                            var instalmentDate = moment(item.InstallmentDate);
                                            instalmentDate = instalmentDate.toDate();
                                            instalmentDate.setHours(0, 0, 0, 0);

                                            if (instalmentDate < today) {
                                                currentStatusHtml = "<span class='label label-sm label-danger'>Overdue</span>"
                                            }
                                            else if (!item.IsRecieved && item.RecievedAmount > 0) {
                                                currentStatusHtml = "<span class='label label-sm label-warning'>Partialy Paid</span>"
                                            }


                                        }
                                        item["Status"] = currentStatusHtml;
                                    }
                                    Common.MapItemData(installments, "#insatallment-container", "#template-installments", true);

                                    var totalAmount = $("#insatallment-container").find("input.Amount").toEnumerable().Select("Common.GetInt($.val())").Sum();
                                    var totalRcvd = $("#insatallment-container").find("input.Received").toEnumerable().Select("Common.GetInt($.val())").Sum();
                                    var total = totalAmount - totalRcvd;
                                    $("#TotalOutStanding").val(total);
                                    $("#Amount").val(0).trigger("keyup");
                                    $("#Id").val(dbSale.Id);
                                    $this.CalculateDiscount();
                                    $("#reovery-modal-container").modal("show");




                                }
                                else if (status == RecoveryStatus.PrintPossession) {
                                    var scope = $("#print-repossession-letter-container");
                                    $(".print-area").removeClass("visible-print");
                                    $("#print-repossession-letter-container").addClass("visible-print");
                                    res.Data.Letter.OutStandingAmount = res.Data.Balance;
                                    Common.MapDataWithPrefixFClasses(res.Data.Letter, scope, "lblP", "html");
                                    $this.Print("#print-repossession-letter-container");
                                }
                                else if (status == RecoveryStatus.PrintNotficationLetter) {
                                    var scope = $("#print-notification-letter");
                                    $(".print-area").removeClass("visible-print");
                                    $("#print-notification-letter").addClass("visible-print");
                                    Common.MapDataWithPrefixFClasses(res.Data.Letter, scope, "lblP", "html");
                                    $(".lblPNotificationDaysWords").html(Common.WordsConversion(res.Data.Letter.NotificationDays));
                                    $this.Print("#print-notification-letter");
                                }
                                else if (status == RecoveryStatus.PrintFurtherAgreement) {
                                    var scope = $("#print-Agreement-letter");
                                    $(".print-area").removeClass("visible-print");
                                    $("#print-Agreement-letter").addClass("visible-print");
                                    res.Data.Letter.Balance = res.Data.Balance;
                                    //$(".lblPbBalance").html((res.Data.Letter.OutStandingAmount));
                                    Common.MapDataWithPrefixFClasses(res.Data.Letter, scope, "lblP", "html");
                                    $this.Print("#print-Agreement-letter");
                                }




                            }
                            else {
                                Common.ShowError(res.Error);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
            }
        },
        LoadBanks: function () {
            var banks = Common.GetLeafAccounts(PageSetting.Banks);
            var html = "";
            html += "<select class='form-control Banks'>";
            for (var i in banks) {
                var bank = banks[i];
                html += "<option value=" + bank.Id + ">" + bank.AccountCode + "</option>";
            }
            html += "</select>";
            $(".Banks").html(html);
        },
        Print: function (element) {
            $(".main-print-container").find(">div").addClass("hide").removeClass("visible-print");
            $(".main-print-container").find(element).removeClass("hide").addClass("visible-print");
            window.print();
        },
        ReturnToCustomer: function () {
            var $this = this;
            var scope = $(".installment-form");
            if (Common.Validate(scope)) {
                var record = Common.SetValue($(".installment-form"));
                var Id = $("#Id", scope).val();
                record.VehicleSaleId = Id;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?returntoCustomer=true",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "completing customer return process..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Process completed successfully." });
                            $(CURRENT_ROW).remove();
                            $("#reovery-modal-container").modal("hide");

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
        IssueLetter: function () {
            var $this = this;
            var scope = $("#modal-possession-letter");
            if (Common.Validate(scope)) {
                var record = Common.SetValue(scope);
                var record1 = Common.SaveItemDataBySelector(CURRENT_ROW, true);
                record = $.extend(true, record, record1);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?status=" + RecoveryStatus.InProcess,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#modal-possession-letter",
                    blockMessage: "Issuing possession letter..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Letter issued successfully." });
                            $this.UpdateStatus(CURRENT_ROW, RecoveryStatus.InProcess);

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
        CreateAdvertisement: function () {
            var $this = this;
            var scope = $("#modal-advertisement");
            if (Common.Validate(scope)) {
                var record = Common.SetValue(scope);
                var record1 = Common.SaveItemDataBySelector(CURRENT_ROW, true);
                record = $.extend(true, record, record1);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?status=" + RecoveryStatus.Advertisement,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#modal-advertisement",
                    blockMessage: "Advertising..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Advertisement Created successfully." });
                            $this.UpdateStatus(CURRENT_ROW, RecoveryStatus.Advertisement);

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
        CreateNotification: function () {
            var $this = this;
            var scope = $("#modal-notification-letter");
            if (Common.Validate(scope)) {
                var record = Common.SetValue(scope);
                var record1 = Common.SaveItemDataBySelector(CURRENT_ROW, true);
                record = $.extend(true, record, record1);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?status=" + RecoveryStatus.NotficationLetter,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#modal-notification-letter",
                    blockMessage: "Creating Notifiation..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Notification Created successfully." });
                            $this.UpdateStatus(CURRENT_ROW, RecoveryStatus.NotficationLetter);

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
        FinalizeAuctionnerCharges: function () {
            var $this = this;
            var scope = $("#modal-auctionner-charges-finalization");
            if (Common.Validate(scope)) {
                var record = Common.SetValue(scope);
                var record1 = Common.SaveItemDataBySelector(CURRENT_ROW, true);
                record = $.extend(true, record, record1);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?status=" + RecoveryStatus.FinalAuctionnerCharges,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#modal-auctionner-charges-finalization",
                    blockMessage: "Finalizing..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Charges finalized successfully." });
                            $("#modal-auctionner-charges-finalization").modal("hide");
                            $this.UpdateStatus(CURRENT_ROW, SALE.RecoveryStatus);

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


        ProcessSettlement: function () {
            var $this = this;
            var scope = $(".installment-form");
            if (Common.Validate(scope)) {
                var record = Common.SetValue($(".installment-form"));
                var record1 = Common.SaveItemDataBySelector(CURRENT_ROW, true);
                record = $.extend(true, record, record1);
                record.VehicleSaleId = record1.SaleId;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?status=" + RecoveryStatus.Advertisement + "&processSettlement=true",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#reovery-modal-container",
                    blockMessage: "settling..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Settlement done successfully." });
                            $this.UpdateStatus(CURRENT_ROW, RecoveryStatus.CustomerReturn);
                            $("#reovery-modal-container").modal("hide");

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
        LoadSupplier: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.suppliers").html(html).select2();


        },
        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#jsondata #data").html());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
    }

}();