

var DetailOfOverallBillsIssueToTenants = function () {
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
            var qs = "?key=GetDetailOfOverallBillsIssueToTenants";
            qs += "&month=" + $("#Month").val();
            qs += "&year=" + $("#Year").val();
            qs += "&blockId=" + $("#BlockId").val();
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
                blockMessage: "Loading Detail Of Overall BillsIssue To Tenants...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        $(".tenant-info").removeClass("hide");
                        var select = "";
                        var amount = 0;
                        var charges = res.Data.Charges;
                        var blocks = Enumerable.From(charges).GroupBy("$.BlockName", null,
                     function (key, g) {
                         var result = {
                             blockName: key,
                             blockData: g.OrderBy(function (p) { return Common.GetInt(p.ShopNo); }).ToArray(),
                         }
                         return result;
                     }).ToArray();

                        var scope = $("#div-table");
                        Common.MapDataWithPrefixF(res.Data.Tenant, scope, "lbl", "html");
                        var sn = 1;

                        var fine = 0;
                        var grandTotal = 0;
                        for (var i in blocks) {
                            var block = blocks[i];
                            var records = block.blockData;
                            sn = 1;
                            var total = 0;
                            html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'><td colspan='11' style='text-align:center;'>" + block.blockName + "</td></tr>";
                            for (var i in records) {

                                var charge = records[i];
                                html += "<tr>";
                                html += "<td>" + sn + "</td>";
                                html += "<td>" + charge.ShopNo + "</td>";
                                html += "<td>" + charge.Tenant + "</td>";
                                html += "<td class='align-right'>" + charge.MonthlyRent.format() + "</td>";
                                html += "<td class='align-right'>" + charge.UCPercent.format() + "</td>";
                                html += "<td class='align-right'>" + charge.ElectricityCharges.format() + "</td>";
                                html += "<td class='align-right'>" + fine.format() + "</td>";
                                html += "<td class='align-right'>" + charge.MiscCharges.format() + "</td>";

                                html += "<td class='align-right'>" + charge.PossessionCharges.format() + "</td>";
                                html += "<td class='align-right'>" + charge.SecurityMoney.format() + "</td>";
                                total += charge.MonthlyRent + charge.UCPercent + charge.ElectricityCharges + fine + charge.MiscCharges + charge.PossessionCharges + charge.SecurityMoney;
                                html += "<td class='align-right'>" + (charge.MonthlyRent + charge.UCPercent + charge.ElectricityCharges + fine + charge.MiscCharges + charge.PossessionCharges + charge.SecurityMoney).format() + "</td>";

                                html += "</tr>";
                                sn++;
                            }
                            var TotalmonthlyRent = Common.GetInt(Enumerable.From(records).Sum("$.MonthlyRent"));
                            var TotaluCPercent = Common.GetInt(Enumerable.From(records).Sum("$.UCPercent"));
                            var TotalelectricityCharges = Common.GetInt(Enumerable.From(records).Sum("$.ElectricityCharges"));
                            var Totaltfine = Common.GetInt(Enumerable.From(records).Sum("$.fine"));
                            var TotalmiscCharges = Common.GetInt(Enumerable.From(records).Sum("$.MiscCharges"));
                            var TotalpossessionCharges = Common.GetInt(Enumerable.From(records).Sum("$.PossessionCharges"));
                            var TotalsecurityMoney = Common.GetInt(Enumerable.From(records).Sum("$.SecurityMoney"));
                            html += "<tr class='bold subtotal'>";
                            html += "<td colspan = '3' class='align-right'>Total</td>";
                            html += " <td class='align-right'>" + TotalmonthlyRent.format() + "</td>";
                            html += "<td class='align-right'>" + TotaluCPercent.format() + "</td>";
                            html += "<td class='align-right'>" + TotalelectricityCharges.format() + "</td>";
                            html += "<td class='align-right'>" + Totaltfine.format() + "</td>";
                            html += "<td class='align-right'>" + TotalmiscCharges.format() + "</td>";
                            html += "<td class='align-right'>" + TotalpossessionCharges.format() + "</td>";
                            html += "<td class='align-right'>" + TotalsecurityMoney.format() + "</td>";
                            html += "<td class='align-right'> " + total.format() + "</td>";
                            html += "</tr>";
                            grandTotal += total;
                        }
                        if (charges.length == 0)
                            html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";

                        var TotalmonthlyRent = Common.GetInt(Enumerable.From(charges).Sum("$.MonthlyRent"));
                        var TotaluCPercent = Common.GetInt(Enumerable.From(charges).Sum("$.UCPercent"));
                        var TotalelectricityCharges = Common.GetInt(Enumerable.From(charges).Sum("$.ElectricityCharges"));
                        var Totaltfine = Common.GetInt(Enumerable.From(charges).Sum("$.fine"));
                        var TotalmiscCharges = Common.GetInt(Enumerable.From(charges).Sum("$.MiscCharges"));
                        var TotalpossessionCharges = Common.GetInt(Enumerable.From(charges).Sum("$.PossessionCharges"));
                        var TotalsecurityMoney = Common.GetInt(Enumerable.From(charges).Sum("$.SecurityMoney"));

                        html += "<tr class='bold grand-total'>";

                        html += "<td colspan = '3' class='align-right'>Total</td>";
                        html += " <td class='align-right'>" + TotalmonthlyRent.format() + "</td>";
                        html += "<td class='align-right'>" + TotaluCPercent.format() + "</td>";
                        html += "<td class='align-right'>" + TotalelectricityCharges.format() + "</td>";
                        html += "<td class='align-right'>" + Totaltfine.format() + "</td>";
                        html += "<td class='align-right'>" + TotalmiscCharges.format() + "</td>";
                        html += "<td class='align-right'>" + TotalpossessionCharges.format() + "</td>";
                        html += "<td class='align-right'>" + TotalsecurityMoney.format() + "</td>";
                        html += "<td class='align-right'> " + grandTotal.format() + "</td>";


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
