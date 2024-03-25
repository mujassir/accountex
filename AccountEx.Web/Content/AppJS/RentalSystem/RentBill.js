
var RentBill = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "RentChallan";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var Challan = new Object();
    var AlreadyChallaned = new Object();
    return {
        init: function () {
            var $this = this;
            Challan =
                {
                    MonthlyRent: 0,
                    UCPercent: 0,
                    RentArrears: 0,
                    UCPercentArears: 0,
                    SurCharge: 0,
                }
            $this.LoadPageSetting();
            $this.LoadData();
            $(document).on("keyup", ".tbl-net-billing :input,.adjustment-container :input,.adjustment-container select", function (event) {
                $this.CalculateBalance();
            });
            $(document).on("change", ".adjustment-container select#AdjustmentType", function (event) {
                $this.CalculateBalance();
                var adjustmentType = Common.GetInt($("#AdjustmentType").val());
                if (adjustmentType > 0) {
                    $("#AdjustmentAmount").prop("disabled", false).prop("readonly", false);
                }
                else
                    $("#AdjustmentAmount").prop("disabled", true).prop("readonly", true);
            });
            $(document).on("change", "#Month", function (event) {

                $("#ToMonth").select2("val", $(this).val());

            });
            $(document).on("change", "#Year", function (event) {

                $("#ToYear").select2("val", $(this).val());

            });

        },
        LoadData: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "RentAgreement?key=GetRentAgreementsWithTenants",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        var html = "<option></option>";
                        for (var i in data) {
                            var record = data[i];
                            html += "<option data-id='" + record.Id + "' data-shopid='" + record.ShopId + "' data-tenantid='" + record.TenantAccountId + "' value='" + record.Id + "'>" + record.TenantCode + "-" + record.TenantName + " (" + (record.Block != null ? "Block: " + record.Block + " ," : '') + " Shop No: " + record.ShopNo + ")" + "</option>";
                        }
                        $("#TenantAccountId").html(html);
                        $("#TenantAccountId").select2();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },
        New: function () {

            var $this = this;
            $this.CustomClear();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });

            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            var scope = $("#form-info");
            if (Common.Validate($("#form-info"))) {
                var netbalance = Common.GetInt($("#lblNetTotalBalance").attr("total"));
                var accountId = Common.GetInt($("#TenantAccountId option:selected").attr("data-tenantid"));
                var shopId = Common.GetInt($("#TenantAccountId option:selected").attr("data-shopid"));
                var id = Common.GetInt($("#TenantAccountId option:selected").attr("data-id"));
                var record = Common.SetValue(scope);
                record.TenantAccountId = accountId;
                record.RentAgreementId = id;
                record.ShopId = shopId;
                record.ElectricityUnitItemId = Challan.Id;
                var challanItem = $.extend(true, challanItem, record);
                challanItem.Id = 0;
                challanItem.Amount = record.NetAmount;
                record["ChallanItems"] = [challanItem];
                var err = "";
                if (Common.GetInt(record.NetAmount) <= 0) {
                    err += "Bill total amount should be graeter than zero(0).,";
                }
                if (!Common.GetBool(record.IsOpening) && netbalance < 0) {
                    err += "total challan amount can't exceed net billing.,";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?createChallan=true",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Creating rent challan ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            $this.Print(res.Data.Id);
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        CustomClear: function () {
            var $this = this;
            Challan = null;
            $(".tbl-net-billing").find("label").html('');
            $("#mainTable tbody").html('');
            $("#AdjustmentAmount").prop("disabled", true).prop("readonly", true);
            Common.Clear();
            $this.CalculateBalance();
            //$("#btndelete,#btnprint").prop("disabled", true);
        },
        LoadRentBill: function () {
            var $this = this;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var toMonth = Common.GetInt($("#ToMonth").val());
            var toYear = Common.GetInt($("#ToYear").val());
            var accountId = Common.GetInt($("#TenantAccountId option:selected").attr("data-tenantid"));
            var shopId = Common.GetInt($("#TenantAccountId option:selected").attr("data-shopid"));
            var id = Common.GetInt($("#TenantAccountId option:selected").attr("data-id"));
            var loadAllPrevious = $("#IsLoadAllPrevious").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?month=" + month + "&year=" + year + "&toMonth=" + toMonth + "&toYear=" + toYear + "&accountid=" + accountId + "&shopId=" + shopId + "&id=" + id + "&loadAllPrevious=" + loadAllPrevious,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = $(".tbl-net-billing");
                        var d = res.Data.data;
                        var rentDetail = d.RentDetail;
                        var paidChallan = d.PaidRentChallan
                        var challans = d.AllChallans;

                        var alreadyChallaned = {

                            MonthlyRent: Enumerable.From(challans).Where(function (x) { return x['Month'] === month && x['Year'] == year }).Sum("$.MonthlyRent"),
                            UCPercent: Enumerable.From(challans).Where(function (x) { return x['Month'] === month && x['Year'] == year }).Sum("$.UCPercent"),
                            RentArrears: Enumerable.From(challans).Where(function (x) { return x['Month'] === month && x['Year'] == year }).Sum("$.RentArrears"),
                            UCPercentArears: Enumerable.From(challans).Where(function (x) { return x['Month'] === month && x['Year'] == year }).Sum("$.UCPercentArears"),
                            SurCharge: Enumerable.From(challans).Where(function (x) { return x['Month'] === month && x['Year'] == year }).Sum("$.SurCharge"),
                            NetTotal: 0
                        }

                        var balancechallan = new Object();

                        if (alreadyChallaned != null) {

                            balancechallan.MonthlyRent = rentDetail.MonthlyRent - alreadyChallaned.MonthlyRent;
                            balancechallan.UCPercent = rentDetail.UCPercent - alreadyChallaned.UCPercent;
                            balancechallan.RentArrears = rentDetail.RentArrears - alreadyChallaned.RentArrears;
                            balancechallan.UCPercentArears = rentDetail.UCPercentArears - alreadyChallaned.UCPercentArears
                            balancechallan.SurCharge = rentDetail.SurCharge - alreadyChallaned.SurCharge,
                            alreadyChallaned.NetTotal = alreadyChallaned.MonthlyRent + alreadyChallaned.UCPercent + alreadyChallaned.RentArrears + alreadyChallaned.UCPercentArears + alreadyChallaned.SurCharge
                        }
                        else
                            balancechallan = $.parseJSON(JSON.stringify(rentDetail));

                        Challan = balancechallan;
                        AlreadyChallaned = alreadyChallaned;
                        Common.MapEditData(balancechallan, scope);
                        var scope = $(".tbl-net-billing");
                        Common.MapDataWithPrefixF(rentDetail, scope, "lbl", "html");


                        Common.MapDataWithPrefixF(alreadyChallaned, scope, "lblChallaned", "html");
                        var html = "";

                        for (var i in challans) {
                            var challan = challans[i];
                            html += "<tr>"
                            html += "<td>" + challan.Id + "" + (challan.IsAuto ? '&nbsp;<span class="badge badge-danger">Auto</span>' : "") + "</td>";
                            html += "<td>" + challan.Duration + "</td>";
                            html += "<td>" + moment(challan.DueDate).format("MMMM DD, YYYY") + "</td>";
                            html += "<td>" + Common.GetCeilInt(challan.TotalAmount).format() + "</td>";
                            html += "<td>" + (challan.IsReceived ? "Paid" : "Pending") + "</td>";
                            html += "<td>" + challan.RcvNo + "</td>";
                            html += "<td><button type='button' class='btn yellow btn-xs' onclick='RentBill.Print(" + challan.Id + ")'><i class='fa fa-print'></i>&nbsp;Print</button>";
                            if (!challan.IsReceived)
                                html += "<button type='button' class='btn red btn-xs' onclick='RentBill.Delete(this," + challan.Id + ")'><i class='fa fa-trash-o'></i>&nbsp;Delete</button>";
                            html += "</tr>"
                        }
                        $("#mainTable tbody").html(html);
                        $this.CalculateBalance();

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        Print: function (id) {
            var $this = this
            //var id = Challan.Id;
            //id = 13876;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?Id=" + id + "&printChallan=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "printing bill ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data.data;
                        var options = {
                            width: 2,
                            height: 30,
                            format: "CODE39",
                            displayValue: true
                        }
                        $(".barcode").JsBarcode(d.Id.toString(), options);
                        var month = Common.GetInt(d.Month);
                        var year = Common.GetInt(d.Year);
                        var toMonth = Common.GetInt(d.ToMonth);
                        var toYear = Common.GetInt(d.ToYear);
                        var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                        var shortMonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                        //var dueDate = moment(d.DueDate);
                        var dueDate = moment(d.DueDate).format("MMMM DD, YYYY");
                        d.DueDate = dueDate;
                        var fromMonthInString = shortMonthNames[month - 1];
                        var toMonthMonthInString = shortMonthNames[toMonth - 1];
                        if (fromMonthInString == toMonthMonthInString && year == toYear)
                            duration = fromMonthInString + ", " + year
                        else if (fromMonthInString != toMonthMonthInString && year == toYear)
                            duration = fromMonthInString + " to " + toMonthMonthInString + ", " + year
                        else
                            duration = fromMonthInString + " " + year + " to " + toMonthMonthInString + " " + toYear

                        $(".lblBillingMonth").html(duration);

                        var shopData = res.Data.ShopData;
                        var Tenant = res.Data.Tenant;
                        var partner = res.Data.Partner;
                        var rentTotal = Common.GetFloat(d.MonthlyRent) + Common.GetFloat(d.UCPercent);

                        if (d.AdjustmentType == AdjustmentType.Increase)
                            rentTotal = rentTotal + d.AdjustmentAmount;
                        else
                            rentTotal = rentTotal - d.AdjustmentAmount;

                        var surcharges = Common.GetFloat((rentTotal * 5)) / 100;
                        var amountWithSurcharges = rentTotal + surcharges;
                        var arearsTotal = Common.GetFloat(d.RentArrears) + Common.GetFloat(d.UCPercentArears) + Common.GetFloat(d.SurCharge);
                        var amountDue = rentTotal + arearsTotal;
                        Common.MapDataWithPrefixFClasses(d, ".div-report", "lbl", "html");
                        $(".lblArrearsTotal").html(Common.GetCeilInt(arearsTotal));
                        $(".lblSurCharge").html(Common.GetCeilInt(Common.GetCeilInt(d.SurCharge)));
                        $(".lblFutureSurcharge").html(Common.GetCeilInt(surcharges));
                        $(".lblTotal").html(Common.GetCeilInt(amountWithSurcharges));
                        $(".lblAfterDueDate").html(Common.GetCeilInt(amountWithSurcharges + arearsTotal));
                        $(".lblAmount").html(Common.GetCeilInt(rentTotal));
                        $(".lblAmountDue").html(Common.GetCeilInt(amountDue));
                        //$(".lblECharges").html("(" + d.PreviousReading + " / " + d.CurrentReading + " / " + d.Unit + ")");
                        $(".AdjustmentType").html(d.AdjustmentType == AdjustmentType.Increase ? "Plus Adjustment" : "Less Adjustment");
                        setTimeout(function () { window.print() }, 1500);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        CalculateBalance: function () {
            var currentRent = Common.GetInt($("#MonthlyRent").val());
            var currentUC = Common.GetInt($("#UCPercent").val());
            var currentElectricity = Common.GetInt($("#ElectricityCharges").val());
            var currentRentArrear = Common.GetInt($("#RentArrears").val());
            var currentUCArrear = Common.GetInt($("#UCPercentArears").val());
            var currentElectricityArrear = Common.GetInt($("#ElectricityArrears").val());
            var currentSurcharge = Common.GetInt($("#SurCharge").val());
            var currentTotal = currentRent + currentUC + currentRentArrear + currentUCArrear + currentSurcharge;
            $("#NetAmount").val(currentTotal);
            if (Challan != null) {
                var netTotal = Challan.MonthlyRent + Challan.UCPercent + Challan.RentArrears + Challan.UCPercentArears + Challan.SurCharge;
                $("#lblNetTotal").html((netTotal).toString().format());
                $("#lblMonthlyRentBalance").html((Challan.MonthlyRent - currentRent).toString().format());
                $("#lblUCPercentBalance").html((Challan.UCPercent - currentUC).toString().format());
                $("#lblElectricityChargesBalance").html((Challan.ElectricityCharges - currentElectricity).toString().format());
                $("#lblRentArrearsBalance").html((Challan.RentArrears - currentRentArrear).toString().format());
                $("#lblUCPercentArearsBalance").html((Challan.UCPercentArears - currentUCArrear).toString().format());
                $("#lblElectricityArrearsBalance").html((Challan.ElectricityArrears - currentElectricityArrear).toString().format());
                $("#lblSurChargeBalance").html((Challan.SurCharge - currentSurcharge).toString().format());
                var netBalance = netTotal - currentTotal;
                $("#lblNetTotalBalance").html(netBalance.toString().format()).attr("total", netBalance);
            }
            var adjustmentType = Common.GetInt($("#AdjustmentType").val());
            var adjustmentAmount = Common.GetFloat($("#AdjustmentAmount").val());
            var grandTotal = currentTotal - adjustmentAmount;
            if (adjustmentType == AdjustmentType.Increase)
                grandTotal = currentTotal + adjustmentAmount;
            $("#GrandTotal").val(grandTotal)
        },
        Delete: function (element, id) {
            var $this = this;
            var tr = $(element).closest("tr");
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $(tr).remove();
                            Common.ShowMessage(true, { message: "Challan deleted successfully!" });
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
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
    };
}();
