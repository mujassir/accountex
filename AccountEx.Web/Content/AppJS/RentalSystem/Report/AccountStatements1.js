

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
                $("#lbltype,#lbltype1").html($("#TransactionType option:selected").text());
            });
            $this.LoadAgrrement();
        },
        LoadData: function () {
            $(".tenant-info").addClass("hide");
            var qs = "?key=GetSecuirtyPossessionAccountStatement1";
            qs += "&agreemnetId=" + $("#TenantAccountId").val();
            qs += "&month=" + Common.GetInt($("#Month").val());
            qs += "&year=" + $("#Year").val();
            qs += "&toMonth=" + $("#ToMonth").val();
            qs += "&toYear=" + $("#ToYear").val();
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
                        var challans = res.Data.Challans;
                        var scope = $("#div-table");
                        var balance = 0;
                        var tenant = res.Data.Tenant;
                        var agreement = res.Data.Agreement;
                        Common.MapDataWithPrefixF(tenant, scope, "lbl", "html");
                        if ($("#TransactionType").val() == VoucherType.securitymoney) {
                            $(".possession-field").addClass("hide");
                            $("#lblTotal").html(agreement.SecurityMoneyAmount.format());
                            $("#lblHeld").html(agreement.ReceivedSecurityAmount.format());
                            $("#lblBalance").html(agreement.SecurityBalance.format());
                            balance = agreement.SecurityBalance;
                        }
                        else {
                            $(".possession-field").removeClass("hide");
                            $("#lblAlreadyPaid").html(agreement.AlreadyPaidPossessionAmount.format());
                            $("#lblNotPaid").html(agreement.NotPaidPossessionAmount.format());
                            $("#lblTotal").html(agreement.TotalPossessionAmount.format());
                            $("#lblHeld").html(agreement.PossessionReceived.format());
                            $("#lblBalance").html(agreement.PossessionBalance.format());
                            balance = agreement.PossessionBalance;

                        }
                        for (var i in challans) {

                            var rent = challans[i];
                            html += "<tr><td>" + rent.BillNo + "</td>";
                            html += " <td>" + Common.FormatDate(rent.DueDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + rent.NetAmount.format() + "</a></td>";
                            html += " <td>" + (rent.IsReceived ? Common.FormatDate(rent.PaidOn, "DD-MM-YYYY") : "Pending") + "</td>";
                            if (rent.IsReceived)
                                balance -= rent.NetAmount;
                            html += " <td>" + balance.format() + "</td>";
                            html += "</tr>";

                        }
                        $("#lblCurrentDue").html(balance != 0 ? balance.format() : balance);
                        if (challans.length == 0) {
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        }

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
