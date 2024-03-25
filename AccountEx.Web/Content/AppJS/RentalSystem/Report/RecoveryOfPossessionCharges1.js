

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
            var qs = "?key=GetRecoveryOfPossessionCharges1";
            qs += "&agreemnetId=" + $("#TenantAccountId").val();
            qs += "&month=" + Common.GetInt($("#Month").val());
            qs += "&year=" + $("#Year").val();
            qs += "&toMonth=" + $("#ToMonth").val();
            qs += "&toYear=" + $("#ToYear").val();
            qs += "&transactionType=" + $("#TransactionType").val();
            qs += "&blockid=" + $("#BlockId").val();


            var fromMonth = $("#Month option:selected").text();
            var fromYear = $("#Year option:selected").text();

            var toMonth = $("#ToMonth option:selected").text();
            var toYear = $("#ToYear option:selected").text();

            var blockName = $("#BlockId option:selected").text();
            if (blockName == "")
                blockName = "All";
            $("#lblperiod").html(Common.GetChallanPeriod(fromMonth, fromYear, toMonth, toYear));
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
                        var charges = res.Data;
                        var blocks = Enumerable.From(charges).GroupBy("$.Block", null,
                     function (key, g) {
                         var result = {
                             Name: key,
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
                            html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'><td colspan='5' style='text-align:center;'>" + block.Name + "</td></tr>";
                            for (var i in records) {

                                var charge = records[i];
                                html += "<tr>";
                                html += "<td>" + charge.Tenant + "</td>";
                                html += "<td>" + charge.ShopNo + "</td>";
                                html += "<td class='align-right'>" + charge.Due.format() + "</td>";
                                html += "<td class='align-right'>" + charge.Received.format() + "</td>";
                                html += "<td class='align-right'>" + (charge.Due - charge.Received).format() + "</td>";
                                html += "</tr>";
                                sn++;
                            }
                            var totalDue = Common.GetInt(Enumerable.From(records).Sum("$.Due"));
                            var totalReceived = Common.GetInt(Enumerable.From(records).Sum("$.Received"));

                            html += "<tr class='bold subtotal'>";
                            html += "<td colspan = '2' class='align-right'>Total</td>";
                            html += " <td class='align-right'>" + totalDue.format() + "</td>";
                            html += "<td class='align-right'>" + totalReceived.format() + "</td>";
                            html += "<td class='align-right'>" + (totalDue - totalReceived).format() + "</td>";

                            html += "</tr>";
                            grandTotal += total;
                        }
                        if (charges.length == 0)
                            html += "  <tr><td colspan='5' style='text-align: center'>No record(s) found</td></tr>";

                        var totalDue = Common.GetInt(Enumerable.From(charges).Sum("$.Due"));
                        var totalReceived = Common.GetInt(Enumerable.From(charges).Sum("$.Received"));

                        html += "<tr class='bold grand-total'>";

                        html += "<td colspan = '2' class='align-right'>Total</td>";
                        html += " <td class='align-right'>" + totalDue.format() + "</td>";
                        html += "<td class='align-right'>" + totalReceived.format() + "</td>";
                        html += "<td class='align-right'>" + (totalDue - totalReceived).format() + "</td>";
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
