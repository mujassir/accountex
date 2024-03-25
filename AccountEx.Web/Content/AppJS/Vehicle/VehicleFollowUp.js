
var VehicleFollowUp = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VehicleFollowUp";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    var CURRENT_ROW = null;
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
            $this.LoadRecoveryDetail();
            $this.LoadBanks();
            $("#BranchId").change(function () {
                LIST_LOADED = false;
                DataTable.DestroyDatatable(DATATABLE_ID);
                $this.init();
            });
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $("#followup-container").modal("hide");

            });
        },
        ViewLedger: function (td) {
            $this = this;

            var tr = $(td).closest("tr");
            var vehicleId = $(tr).find("input.VehicleId").val();
            var saleId = $(tr).find("input.SaleId").val();
            var customerId = $(tr).find("input.AccountId").val();
            window.open("../Reports/vehicleledger?saleId=" + saleId);

        },
        GetFollowUp: function (td) {
            $this = this;
            Common.ClearByScope($("#followup-container"));
            var tr = $(td).closest("tr");
            var vehicleId = $(tr).find("input.VehicleId").val();
            var customerId = $(tr).find("input.AccountId").val();
            $("#VehicleId").val(vehicleId);
            $("#CustomerId").val(customerId);

            $this.GetFollowUpData();
            $("#followup-container").modal("show");

        },
        GetFollowUpData: function () {
            var vehicleId = $("#VehicleId").val();
            var customerId = $("#CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?vehicleId=" + vehicleId + "&customerId=" + customerId,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        var html = "";
                        for (var i in data) {
                            var item = data[i];
                            html += "<tr>";
                            html += "<td>" + (item.Date != null ? moment(item.Date).format("DD/MM/YYYY") : '') + "</td>";
                            html += "<td>" + item.Remarks + "</td>";
                            html += "<td>" + (item.NextFollowUp != null ? moment(item.NextFollowUp).format("DD/MM/YYYY") : '') + "</td>";
                            html += "</tr>";
                        }
                        $("#FollowUpTable tbody").html(html);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
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
            var scope = $("#followup-container");
            var record = Common.SetValue(scope);
            var err = "";
            if (Common.Validate(scope)) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#followup-container",
                    blockMessage: "creating Follow Up...please wait",
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
                blockMessage: "Loading follow Up ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        Common.MapItemData(res.Data, null, null, true);
                        PageData = res.Data;
                        // $this.CheckFinalStatus();

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
            $("#item-container tbody tr").each(function () {
                var tr = $(this);
                $(tr).find("td.td-actions").find(":button").addClass("hide").prop("disabled", true);
                var vehicleId = $(tr).find("input.VehicleId").val();
                var accountId = $(tr).find("input.AccountId").val();
                var saleDetails = Enumerable.From(PageData).Where(function (p) { return p.AccountId == accountId && p.VehicleId == vehicleId }).FirstOrDefault();
                var status = saleDetails.RecoveryStatus;
                if (status == RecoveryStatus.Default) {
                    $(tr).find("td.td-actions").find(".btn-recovery-process").removeClass("hide").prop("disabled", false);
                }
                else if (status == RecoveryStatus.InProcess) {
                    $(tr).find("td.td-actions").find(".btn-recovery-recovered,.btn-recovery-print").removeClass("hide").prop("disabled", false);
                }
                else if (status == RecoveryStatus.Recovered) {
                    $(tr).find("td.td-actions").find(".btn-recovery-inventory-return,.btn-recovery-customer-return").removeClass("hide").prop("disabled", false);
                }


            })
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
                                var dbSale = res.Data;
                                saleDetails.RecoveryStatus = dbSale.RecoveryStatus;
                                if (status == RecoveryStatus.InProcess) {
                                    $(tr).find("td.td-actions").find(".btn-recovery-recovered,.btn-recovery-print").removeClass("hide").prop("disabled", false);
                                }
                                else if (status == RecoveryStatus.Recovered) {
                                    $(tr).find("td.td-actions").find(".btn-recovery-inventory-return,.btn-recovery-customer-return").removeClass("hide").prop("disabled", false);
                                }
                                else if (status == RecoveryStatus.InventoryReturn || status == RecoveryStatus.InventoryReturn) {
                                    $(tr).remove();
                                }
                                else if (status == RecoveryStatus.CustomerReturn) {
                                    $("#insatallment-container tbody").html("");
                                    $(tr).find("td.td-actions").find(".btn-recovery-inventory-return,.btn-recovery-customer-return").removeClass("hide").prop("disabled", false);
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
                                    $("#TotalOutStanding,#Amount").val(total);
                                    $("#Id").val(dbSale.Id);
                                    $this.CalculateDiscount();
                                    Common.ClearByScope($("#followup-container"));
                                    $("#reovery-modal-container").modal("show");




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
        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#jsondata #data").html());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
    }

}();