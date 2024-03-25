

var RecoveryOfPossessionCharges = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            $("#TransactionType").change(function () {
                $("#lbltype").html($("#TransactionType option:selected").text());
                $this.LoadData();
            });
            $this.LoadAgrrement();
        },
        LoadData: function () {
            $(".tenant-info").addClass("hide");
            var qs = "?key=GetRecoveryOfPossessionCharges";
            qs += "&agreemnetId=" + $("#TenantAccountId").val();
            qs += "&agreemnetid=" + $("#tenantaccountid").val();
            qs += "&month=" + $("#Month").val();
            qs += "&year=" + $("#Year").val();
            qs += "&blockid=" + $("#BlockId").val();
            qs += "&transactionType=" + $("#TransactionType").val();


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
                blockMessage: "loading recovery statement...please wait",
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
                            html += "<td>" + charge.Tenant + "</td>";
                            html += "<td>" + charge.DueArrears + "</td>";
                            html += "<td>" + charge.DueCurrent + "</td>";
                            html += "<td>" + charge.TotalDue + "</td>";
                            html += "<td>" + charge.ReceivedArrears + "</td>";
                            html += "<td>" + charge.ReceivedCurrent + "</td>";
                            html += "<td>" + charge.TotalReceived + "</td>";
                            html += "<td>" + charge.ReceivedInstallments + "</td>";
                            html += "<td>" + charge.TotalInstallments + "</td>";
                            html += "<td>" + charge.OutstandingBalance + "</td>";
                            html += "</tr>";
                            sn++;
                        }
                        if (charges.length == 0)
                            html += "  <tr><td colspan='12' style='text-align: center'>No record(s) found</td></tr>";

                        var dueArrearsTotal = Common.GetInt(Enumerable.From(charges).Sum("$.DueArrears"));
                        var dueCurrentTotal = Common.GetInt(Enumerable.From(charges).Sum("$.DueCurrent"));
                        var dueTotal = Common.GetInt(Enumerable.From(charges).Sum("$.TotalDue"));
                        var receivedArrears = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedArrears"));
                        var receivedCurrent = Common.GetInt(Enumerable.From(charges).Sum("$.ReceivedCurrent"));
                        var totalReceived = Common.GetInt(Enumerable.From(charges).Sum("$.TotalReceived"));
                        var outstandingBalance = Common.GetInt(Enumerable.From(charges).Sum("$.OutstandingBalance"));
                        html += "<tr class='bold'>";

                        html += "<td colspan = '3' class='align-right'>Total</td>";
                        html += " <td>" + dueArrearsTotal.format() + "</td>";
                        html += "<td>" + dueCurrentTotal.format() + "</td>";
                        html += "<td>" + dueTotal.format() + "</td>";
                        html += "<td>" + receivedArrears.format() + "</td>";
                        html += "<td>" + receivedCurrent.format() + "</td>";
                        html += "<td>" + totalReceived.format() + "</td>";
                        html += "<td></td>";
                        html += "<td></td>";
                        html += "<td>" + outstandingBalance.format() + "</td>";
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
