

var RecoveryOfRent = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            $this.LoadAgrrement();
        },
        LoadData: function () {
            $(".tenant-info").addClass("hide");
            var qs = "?key=GetRecoveryOfRent";


            qs += "&month=" + $("#Month").val();
            qs += "&year=" + $("#Year").val();
            qs += "&blockid=" + $("#BlockId").val();

            var monthName = $("#Month option:selected").text();
            var yearName = $("#Year option:selected").text();
            var blockName = $("#BlockId option:selected").text();
            if (blockName == "")
                blockName = "All";
            $("#lblperiod").html(monthName + " " + yearName);
            $("#lblblock").html(blockName);
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Recovery Of Rent...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        $(".tenant-info").removeClass("hide");
                        var select = "";
                        var amount = 0;
                        var charges = res.Data.Charges;
                        var scope = $("#div-table");
                        Common.MapDataWithPrefixF(res.Data.Tenant, scope, "lbl", "html");
                        var sn = 1;
                        for (var i in charges) {

                            var charge = charges[i];
                            html += "<tr>";
                            html += "<td>" + sn + "</td>";
                            html += "<td>" + charge.ShopNo + "</td>";
                            html += "<td style='width:10%'>" + charge.Tenant + "</td>";
                            html += "<td>" + charge.RentArrears.format() + "</td>";
                            html += "<td>" + charge.UCPercentArears.format() + "</td>";
                            html += "<td>" + charge.ElectricityArrears.format() + "</td>";
                            html += "<td>" + charge.SurCharge.format() + "</td>";
                            html += "<td>" + charge.MonthlyRent.format() + "</td>";
                            html += "<td>" + charge.UCPercent.format() + "</td>";
                            html += "<td>" + charge.ElectricityCharges.format() + "</td>";
                            html += "<td>" + charge.LateSurcharge.format() + "</td>";
                            html += "<td>" + charge.TotalRent.format() + "</td>";
                            html += "<td>" + charge.TotalUC.format() + "</td>";
                            html += "<td>" + charge.TotalElectricity.format() + "</td>";
                            html += "<td>" + charge.ReceivedRentArrears.format() + "</td>";
                            html += "<td>" + charge.ReceivedUCPercentArears.format() + "</td>";
                            html += "<td>" + charge.ReceivedElectricityArrears.format() + "</td>";
                            html += "<td>" + charge.ReceivedSurChargeArrears.format() + "</td>";
                            html += "<td>" + charge.ReceivedMonthlyRent.format() + "</td>";
                            html += "<td>" + charge.ReceivedUCPercent.format() + "</td>";
                            html += "<td>" + charge.ReceivedElectricityCharges.format() + "</td>";
                            html += "<td>" + charge.ReceivedCurrentSurCharge.format() + "</td>";
                            html += "<td>" + charge.BalanceRent.format() + "</td>";
                            html += "<td>" + charge.BalanceUC.format() + "</td>";
                            html += "<td>" + charge.BalanceElectricity.format() + "</td>";
                            html += "<td>" + charge.BalanceSurCharge.format() + "</td>";
                            html += "</tr>";
                            sn++;
                        }
                        if (charges.length == 0)
                            html += "  <tr><td colspan='21' style='text-align: center'>No record(s) found</td></tr>";
                        var RentArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.RentArrears"));
                        var UCPercentArearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.UCPercentArears"));
                        var ElectricityArrearsTotal = Common.GetInt(Enumerable.From(charge).Sum("$.ElectricityArrears"));
                        var surChargeTotal = Common.GetInt(Enumerable.From(charge).Sum("$.SurCharge"));
                        var MonthlyRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.MonthlyRent"));
                        var UCPercentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.UCPercent"));
                        var ElectricityChargesTotal = Common.GetInt(Enumerable.From(charge).Sum("$.ElectricityCharges"));
                        var lateSurchargeTotal = Common.GetInt(Enumerable.From(charge).Sum("$.LateSurcharge"));
                        var TotalRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalRent"));
                        var TotalUCTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalUC"));
                        var TotalElectricityTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalElectricity"));
                        var ReceivedRentArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedRentArrears"));
                        var ReceivedUCPercentArearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedUCPercentArears"));
                        var ReceivedElectricityArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedElectricityArrears"));
                        var receivedSurChargeArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedSurChargeArrears"));
                        var ReceivedMonthlyRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedMonthlyRent"));
                        var ReceivedUCPercentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedUCPercent"));
                        var ReceivedElectricityChargesTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedElectricityCharges"));
                        var receivedCurrentSurChargeTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedCurrentSurCharge"));
                        var BalanceRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.BalanceRent"));
                        var BalanceUCTotal = Common.GetInt(Enumerable.From(charges).Sum("$.BalanceUC"));
                        var BalanceElectricityTotal = Common.GetInt(Enumerable.From(charges).Sum("$.BalanceElectricity"));
                        var balanceSurChargeTotal = Common.GetInt(Enumerable.From(charges).Sum("$.BalanceSurCharge"));

                        html += "<tr class='bold'>";

                        html += "<td colspan = '3' class='align-right'>Total</td>";
                        html += " <td>" + RentArrearsTotal.format() + "</td>";
                        html += "<td>" + UCPercentArearsTotal.format() + "</td>";
                        html += "<td>" + ElectricityArrearsTotal.format() + "</td>";
                        html += "<td>" + surChargeTotal.format() + "</td>";
                        html += "<td>" + MonthlyRentTotal.format() + "</td>";
                        html += "<td>" + UCPercentTotal.format() + "</td>";
                        html += "<td>" + ElectricityChargesTotal.format() + "</td>";
                        html += "<td>" + lateSurchargeTotal.format() + "</td>";
                        html += "<td>" + TotalRentTotal.format() + "</td>";
                        html += "<td>" + TotalUCTotal.format() + "</td>";
                        html += "<td>" + TotalElectricityTotal.format() + "</td>";
                        html += "<td>" + ReceivedRentArrearsTotal.format() + "</td>";
                        html += "<td>" + ReceivedUCPercentArearsTotal.format() + "</td>";
                        html += "<td>" + ReceivedElectricityArrearsTotal.format() + "</td>";
                        html += "<td>" + receivedSurChargeArrearsTotal.format() + "</td>";
                        html += "<td>" + ReceivedMonthlyRentTotal.format() + "</td>";
                        html += "<td>" + ReceivedUCPercentTotal.format() + "</td>";
                        html += "<td>" + ReceivedElectricityChargesTotal.format() + "</td>";
                        html += "<td>" + receivedCurrentSurChargeTotal.format() + "</td>";
                        html += "<td>" + BalanceRentTotal.format() + "</td>";
                        html += "<td>" + BalanceUCTotal.format() + "</td>";
                        html += "<td>" + BalanceElectricityTotal.format() + "</td>";
                        html += "<td>" + balanceSurChargeTotal.format() + "</td>";

                        html += "</tr>";
                        $(".report-table tbody").html(html);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        LoadAgrrement: function () {
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
                            html += "<option data-shopid='" + record.ShopId + "' data-tenantid='" + record.TenantAccountId + "' value='" + record.Id + "'>" + record.TenantCode + "-" + record.TenantName + " (" + (record.Block != null ? "Block: " + record.Block + " ," : '') + " Shop No: " + record.ShopNo + ")" + "</option>";
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
    };
}();
