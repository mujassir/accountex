

var OverallRecoveryReport = function () {
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
            var qs = "?key=GetOverallRecoveryReport";

            qs += "&month=" + $("#Month").val();
            qs += "&year=" + $("#Year").val();
            qs += "&blockid=" + $("#BlockId").val();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Overall Recovery Report Charge Statement...please wait",
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
                            var total = charge.RentUCE + charge.PossessionCharges + charge.SecurityMoney;
                            var totalRecpt = charge.RentUCEReceived + charge.PossessionReceived + charge.SecurityReceived;
                            var balance = total - totalRecpt;
                           
                            html += "<tr>";
                            html += "<td>" + sn + "</td>";
                            html += "<td>" + charge.ShopNo + "</td>";
                            html += "<td>" + charge.Tenant + "</td>";
                            html += "<td>" + charge.RentUCEArrears.format() + "</td>";
                            html += "<td>" + charge.PossessionArrears.format() + "</td>";
                            html += "<td>" + charge.SecurityArrears.format() + "</td>";
                            html += "<td>" + charge.RentUCE.format() + "</td>";
                            html += "<td>" + charge.PossessionCharges.format() + "</td>";
                            html += "<td>" + charge.SecurityMoney.format() + "</td>";
                            html += "<td>" + (total).format() + "</td>";
                            html += "<td>" + charge.RentUCEReceived.format() + "</td>";
                            html += "<td>" + charge.PossessionReceived.format() + "</td>";
                            html += "<td>" + charge.SecurityReceived.format() + "</td>";
                            html += "<td>" + balance.format() + "</td>";




                            html += "</tr>";
                            sn++;
                        }
                        if (charges.length == 0)
                            html += "  <tr><td colspan='14' style='text-align: center'>No record(s) found</td></tr>";

                        var RentArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.RentArrears"));
                        var UCPercentArearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.UCPercentArears"));
                        var ElectricityArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ElectricityArrears"));
                        var MonthlyRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.MonthlyRent"));
                        var UCPercentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.UCPercent"));
                        var ElectricityChargesTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ElectricityCharges"));
                        var TotalRentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalRent"));
                        var TotalUCTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalUC"));
                        var TotalElectricityTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalElectricity"));
                        var ReceivedRentArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedRentArrears"));

                        html += "<tr class='bold'>";
                        //html += "<td></td>";
                        //html += "<td></td>";
                        html += "<td colspan='3' class='align-right'>Total</td>";
                        html += " <td>" + RentArrearsTotal.format() + "</td>";
                        html += "<td>" + UCPercentArearsTotal.format() + "</td>";
                        html += "<td>" + ElectricityArrearsTotal.format() + "</td>";
                        html += "<td>" + MonthlyRentTotal.format() + "</td>";
                        html += "<td>" + UCPercentTotal.format() + "</td>";
                        html += "<td>" + ElectricityChargesTotal.format() + "</td>";
                        html += "<td>" + TotalRentTotal.format() + "</td>";
                        html += "<td>" + TotalUCTotal.format() + "</td>";
                        html += "<td>" + TotalElectricityTotal.format() + "</td>";


                        html += "<td></td>";
                        html += "<td></td>";


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
