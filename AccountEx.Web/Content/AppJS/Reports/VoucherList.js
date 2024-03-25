
var VoucherList = function () {
    var apiController = "Report";
    var title = "";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {
                var date1 = $("input[name='FromDate']").val();
                var date2 = $("input[name='ToDate']").val();
                var accountId =
                $this.LoadData(date1, date2);
            });
            $this.SetPageData();
            var vtype = Common.GetQueryStringValue("vocuhertype");
            if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                $("#vouchertype-container").addClass("hide");
                $.uniform.update($("input:checkbox[value='" + vtype + "']").prop("checked", true));

            }
            else {
                $("vouchertype-container").removeClass("hide");
            }
        },
        LoadData: function (date1, date2) {

            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var selectedvoucher = $("#vouchertype-container input:checkbox:checked").map(function () {
                return this.value;
            }).get();

            var qs = "?key=GetVoucherList";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&vouchertype=" + selectedvoucher;
            qs += "&accountId=" + $("select[name='AccountId']").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + title + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var vouchers = res.Data;
                        var vtype = Common.GetQueryStringValue("vocuhertype");
                        var vclass = "show";
                        if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                            vclass = "hide";
                        }
                        for (var i in vouchers) {
                            var voucher = vouchers[i];
                            var voucherItems = voucher.VoucherItems;

                            if (voucher.AccountId > 0) {
                                var url = Common.GetTransactionUrl(voucher.TransactionType, voucher.VoucherNumber);
                                var vType = VoucherTypes[voucher.TransactionType];
                                html += "<tr>";
                                html += "<td>" + window.moment(voucher.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td><a href='" + url + "' title='" + vType.Description + "'>" + vType.Code + "-" + voucher.VoucherNumber + "</a></td>";
                                //html += "<td>" + window.moment(voucher.Date).format("DD/MM/YYYY") + "</td>";
                                html += "<td>" + voucher.FromCode + "-" + voucher.From + "</td>";
                                html += "<td>" + voucher.ToCode + "-" + voucher.To + "</td>";
                                html += "<td>" + voucher.Comments + "</td>";
                                html += "<td>" + voucher.Amount + "</td>";
                                html += "</tr>";
                                for (var j in vouchers) {

                                }
                            }
                        }
                        if (res.Data.length == 0)
                            html += "  <tr><td colspan='7' style='text-align: center'>No record(s) found</td></tr>";

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
        SetPageData: function () {

            var vtype = Common.GetQueryStringValue("vocuhertype");
            if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                $("#vouchertype-container").addClass("hide");
                $.uniform.update($("input:checkbox[value='" + vtype + "']").prop("checked", true));

                $("th.allth").addClass("hide");
                // var title = "";
                if (vtype == "5") {
                    title = "Cash Receipts Voucher";
                }
                else if (vtype == "6") {
                    title = "Cash Payment Voucher";

                }
                else if (vtype == "7") {
                    title = "Bank Receipts Voucher";

                }
                else if (vtype == "8") {
                    title = "Bank Payment Voucher";

                }
                else if (vtype == "12") {
                    title = "Transfer Voucher";

                }


            }
            else {
                $("#vouchertype-container").removeClass("hide");
                title = "All Voucher List";
                $("th.allth").removeClass("hide");
            }
            $(".page-title .main-title").html(title);
            $("#pageurl").html("<a href='" + window.location.href + "'>" + title + "</a>");
        },

    };
}();