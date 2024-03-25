

var AccountStatement = function () {
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
            });
            $this.LoadAgrrement();
        },
        LoadData: function () {
            $(".tenant-info").addClass("hide");
            var qs = "?key=GetSecuirtyPossessionAccountStatement";
            qs += "&agreemnetId=" + $("#TenantAccountId").val();
            qs += "&transactionType=" + $("#TransactionType").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Account Statement...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        $(".tenant-info").removeClass("hide");
                        var select = "";
                        var amount = 0;
                        var rents = res.Data.Rents;
                        var scope = $("#div-table");
                        Common.MapDataWithPrefixF(res.Data.Tenant, scope, "lbl", "html");
                        for (var i in rents) {

                            var rent = rents[i];
                            html += "<tr><td>" + rent.NoOfInstallment + "</td>";
                            html += "<td>" + rent.DueAmount.format() + "</a></td>";
                            html += " <td>" + Common.FormatDate(rent.DueDate, "DD-MM-YYYY") + "</td>";
                            html += " <td>" + Common.FormatDate(rent.PaidOn, "DD-MM-YYYY") + "</td>";
                            html += " <td>" + rent.RcvNo + "</td>";
                            html += "<td>" + rent.BillNo.format() + "</td>";
                            html += "<td>" + rent.OutStanding.format() + "</td>";

                        }
                        if (rents.length == 0) {
                            html += "  <tr><td colspan='8' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        var DueAmountTotal = Common.GetInt(Enumerable.From(rents).Sum("$.DueAmount"));
                        var OutStandingTotal = Common.GetInt(Enumerable.From(rents).Sum("($.OutStanding)"));
                        html += "<tr class='bold'>";
                        html += "<td class='align-right'>Total</td>";
                        html += " <td>" + DueAmountTotal.format() + "</td>";
                        html += "<td></td>";
                        html += "<td></td>";
                        html += "<td></td>";
                        html += "<td>" + OutStandingTotal.format() + "</td>";
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
