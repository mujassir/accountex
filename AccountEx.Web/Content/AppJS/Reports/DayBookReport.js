
var DayBookReport = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
            var $this = this;
            //Common.InitNumerics();
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            var id = Common.GetQueryStringValue("accountId");
            var fromDate = Common.GetQueryStringValue("fromDate");
            var toDate = Common.GetQueryStringValue("toDate");
            if (id == undefined || id == "") {

                $(".row-account").removeClass("hide");
                $(".row-info").addClass("hide");
            }
            else {
                $(".row-info").removeClass("hide");
                $("input[name='FromDate']").val(fromDate);
                $("input[name='ToDate']").val(toDate);
                $("#btnShowReport").trigger("click");
            }
            var type = Common.GetQueryStringValue("type").toLowerCase();
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetDayBook";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + Common.GetQueryStringValue("type").toLowerCase() + " DayBook...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var TotalCredit = 0;
                        var TotalDebit = 0;
                        var data = res.Data.Records;
                        var cashreciepts = Enumerable.From(data).Where("$.TransactionType==" + VoucherType.cashreceipts).ToArray();
                        var cashpayments = Enumerable.From(data).Where("$.TransactionType==" + VoucherType.cashpayments).ToArray();
                        //html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td>" + res.Data.OpeningBalance + "</td></tr>";

                        //for cash payments
                        for (var i in cashreciepts) {
                            var Items = cashreciepts[i].VoucherItems;
                            for(var j in Items){
                            var vouchertype = Common.GetKeyFromEnum(Items[j].TransactionType, VoucherType);
                            var vType = VoucherTypes[Items[j].TransactionType];
                            html += "<tr><td>" + moment(cashreciepts[i].Date).format("DD-MM-YYYY") + "</td><td>" + cashreciepts[i].VoucherNumber
                                + "</td><td>" + Items[j].AccountName
                                + "</td><td>" + Items[j].Description + "</td><td class='align-right'>" + Common.GetInt(Items[j].Credit).format() + "</td></tr>";
                            TotalCredit += Items[j].Credit;
                        }
                                
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td></td><td></td><td class='align-right'>Total</td><td>" + TotalCredit.format() + "</td></tr>";
                        $(".report-table-cashreciepts tbody").html(html);

                        //for cashpayments
                        html = "";
                        for (var i in cashpayments) {
                            var Items = cashpayments[i].VoucherItems;
                            for (var j in Items) {
                                var vouchertype = Common.GetKeyFromEnum(Items[j].TransactionType, VoucherType);
                                var vType = VoucherTypes[Items[j].TransactionType];
                                html += "<tr><td>" + moment(cashpayments[i].Date).format("DD-MM-YYYY") + "</td><td>" + cashpayments[i].VoucherNumber
                                    + "</td><td>" + Items[j].AccountName
                                    + "</td><td>" + Items[j].Description + "</td><td class='align-right'>" + Common.GetInt(Items[j].Debit).format() + "</td></tr>";
                                TotalDebit += Items[j].Debit;
                            }

                        }

                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td></td><td></td><td class='align-right'>Total</td><td>" + TotalDebit.format() + "</td></tr>";
                        $(".report-table-cashpayments tbody").html(html);
                        var balance = Common.GetInt(TotalCredit - TotalDebit);
                        balance=Common.GetInt(balance + Common.GetInt(res.Data.OpeningBalance));
                        $("#lblTotalCredit").html(TotalCredit.format());
                        $("#lblTotalDebit").html(TotalDebit.format());
                        $("#lblBalance").html(balance.format());
                        $("#lblOpeningBalance").html(res.Data.OpeningBalance.format());
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