var PeriodicBalances = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            var id = Common.GetQueryStringValue("accountId");
            if (id == undefined || id == "") {
                //$("#AccountId").select2("val", id);
                $(".row-account").removeClass("hide");
            }


        },

        LoadData: function () {

            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetPeriodicBalances";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[Common.GetQueryStringValue("type")];
            qs += "&OpeningStock=" + $("#OpeningStock").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {

                        //remove this later on
                        $(".report-table tbody").html('');
                        $("#div-profit").html('');
                        $("#div-expense").html('');
                        $("#div-netprofit").html('');



                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var mainAccounts = Enumerable.From(data).Where(function (x) { return x.LEVEL == 1 }).ToArray();
                        var headtotal = 0;

                        var totalGrandCredit = 0;
                        var totalGrandDebit = 0;
                        var totalGrandOpeningCredit = 0;
                        var totalGrandOpeningDebit = 0;
                        var totalGrandBalanceCredit = 0;
                        var totalGrandBalanceDebit = 0;

                        for (var i in mainAccounts) {
                            var totalMainOpeningCredit = 0;
                            var totalMainOpeningDebit = 0;
                            var totalMainBalanceCredit = 0;
                            var totalMainBalanceDebit = 0;
                            var mainAccount = mainAccounts[i];

                            if (mainAccount.BALANCE == 0) continue;
                            html += "<tr class='head'><td colspan='7'>" + mainAccount.MAINACCOUNT + "<span class='headBalance'></span></td></tr>";
                            var controlAccounts = Enumerable.From(data).Where(function (x) { return x.LEVEL == 2 && x.PARENTID == mainAccount.MAINACCOUNTID }).ToArray();

                            for (var j in controlAccounts) {

                                var totalCtrlOpeningCredit = 0;
                                var totalCtrlOpeningDebit = 0;
                                var totalCtrlBalanceCredit = 0;
                                var totalCtrlBalanceDebit = 0;
                                var controlAccount = controlAccounts[j];
                                if (controlAccount.BALANCE == 0) continue;
                                html += "<tr class='sub-head'><td colspan='7'>" + controlAccount.CONTROLACCOUNT + "<span class='headBalance'></span></td></tr>";
                                var SubAccounts = Enumerable.From(data).Where(function (x) { return x.LEVEL == 3 && x.PARENTID == controlAccount.CONTROLACCOUNTID }).ToArray();
                                for (var i in SubAccounts) {
                                    var SubAccount = SubAccounts[i];
                                    //if (SubAccount.BALANCE == 0) continue;
                                    html += "<tr class='controlheads'><td colspan='7'>" + SubAccount.SUBACCOUNT + "<span class='headBalance'></span></td></tr>";
                                    var Accounts = Enumerable.From(data).Where(function (x) { return x.LEVEL == 4 && x.PARENTID == SubAccount.SUBACCOUNTID }).ToArray();
                                    var Accountsheadtotal = 0;
                                    var totalOpeningCredit = 0;
                                    var totalOpeningDebit = 0;
                                    var totalBalanceCredit = 0;
                                    var totalBalanceDebit = 0;
                                    //html += "<tr><td colspan='2'><table class='subheads'>";
                                    html += "<tr><td>Account</td><td colspan='2' class='align-center'>OPENING BALANCE</td><td>DEBIT</td><td>CREDIT</td><td class='align-center' colspan='2'>BALANCE</td></tr>";
                                    html += "<tr><td></td> <td>DEBIT</td> <td>CREDIT</td><td></td><td></td><td class='align-right'>DEBIT</td><td class='align-right'>CREDIT</td></tr>";
                                    for (var k in Accounts) {

                                        var account = Accounts[k];
                                        if (account.BALANCE == 0) continue;
                                        if (account.YTDBALANCE == null)
                                            account.YTDBALANCE = 0;
                                        if (account.DEBIT == null)
                                            account.DEBIT = 0;
                                        if (account.CREDIT == null)
                                            account.CREDIT = 0;
                                        if (account.BALANCE == null)
                                            account.BALANCE = 0;

                                        html += "<tr><td>" + account.ACCOUNT + "</td>";

                                        if (account.YTDBALANCE > 0) {
                                            html += "<td class='align-right'>" + account.YTDBALANCE.format() + "</td><td></td>";
                                            totalOpeningDebit += account.YTDBALANCE;


                                        }
                                        else {
                                            html += "<td></td><td class='align-right'>" + account.YTDBALANCE.format() + "</td>";
                                            totalOpeningCredit += account.YTDBALANCE;


                                        }

                                        html += "<td class='align-right'>" + account.DEBIT.format() + "</td><td class='align-right'>" + account.CREDIT.format() + "</td>";

                                        if (account.BALANCE > 0) {
                                            html += "<td class='align-right'>" + account.BALANCE.format() + "</td><td></td>";
                                            totalBalanceDebit += account.BALANCE;


                                        }
                                        else {
                                            html += "<td></td><td class='align-right'>" + account.BALANCE.format() + "</td></tr>";
                                            totalBalanceCredit += account.BALANCE;


                                        }


                                        //html += "<tr><td>" + account.ACCOUNT + "</td><td class='align-right'>" + account.YTDBALANCE.format() + "</td><td></td><td class='align-right'>" + account.DEBIT.format() + "</td><td class='align-right'>" + account.CREDIT.format() + "</td><td class='align-right'>" + account.BALANCE.format() + "</td><td></td></tr>";
                                    }
                                    totalCtrlOpeningDebit += totalOpeningDebit
                                    totalCtrlOpeningCredit += totalOpeningCredit;
                                    totalCtrlBalanceDebit += totalBalanceDebit;
                                    totalCtrlBalanceCredit += totalBalanceCredit;
                                    html += "<tr class='total-sub-accouont bold'><td>" + SubAccount.SUBACCOUNT + " Total:</td><td class='align-right'>" + totalOpeningDebit.format() + "</td><td class='align-right'>" + totalOpeningCredit.format() + "</td><td class='align-right'>" + SubAccount.DEBIT.format() + "</td><td class='align-right'>" + SubAccount.CREDIT.format() + "</td><td class='align-right'>" + totalBalanceDebit.format() + "</td><td class='align-right'>" + totalBalanceCredit.format() + "</td></tr>";

                                    //html += "<tr class='total-sub-accouont bold'><td>Total</td><td class='align-right'>" + SubAccount.YTDBALANCE.format() + "</td><td class='align-right'>" + SubAccount.DEBIT.format() + "</td><td class='align-right'>" + SubAccount.CREDIT.format() + "</td><td class='align-right'>" + SubAccount.BALANCE.format() + "</td></tr>";
                                    //html += "<tr class='total-sub-accouont bold'><td  colspan='5'><span class='headBalance'>" + SubAccount.SUBACCOUNT + " Total: " + SubAccount.BALANCE.format() + "</span></td></tr>";
                                    //html += "<tr class='contolheads-total '><td colspan='2'>Total " + SubAccount.SUBACCOUNT + "</td><td colspan='2'>controlheadtotal.format() </td></tr>";
                                    //html += "<tr class='contolheads-total '><td colspan='2'>Total " + SubAccount.SUBACCOUNT + "</td><td colspan='2'>Total " + SubAccount.SUBACCOUNT + "</td></tr>";
                                    //html += "</table></td></tr>";

                                }

                                totalMainOpeningDebit += totalCtrlOpeningDebit;
                                totalMainOpeningCredit += totalCtrlOpeningCredit;
                                totalMainBalanceDebit += totalCtrlBalanceDebit;
                                totalMainBalanceCredit += totalCtrlBalanceCredit;
                                //html += "<tr class='head-total'><td>Total +head.Head </td><td>headtotal.format() </td></tr>";
                                //html += "<tr class='total-control-accouont bold'><td colspan='7'><span class='headBalance'>" + controlAccount.CONTROLACCOUNT + " Total: " + controlAccount.BALANCE.format() + "</span></td></tr>";
                                html += "<tr class='total-control-accouont bold'><td><span class='headBalance'>" + controlAccount.CONTROLACCOUNT + " Total:</span></td><td class='align-right'>" + totalCtrlOpeningDebit.format() + "</td><td class='align-right'>" + totalCtrlOpeningCredit.format() + "</td><td class='align-right'>" + controlAccount.DEBIT.format() + "</td><td class='align-right'>" + controlAccount.CREDIT.format() + "</td><td class='align-right'>" + totalCtrlBalanceDebit.format() + "</td><td class='align-right'>" + totalCtrlBalanceCredit.format() + "</td></tr>";
                            }

                            html += "<tr class='total-main-accouont bold'><td><span class='headBalance'>" + mainAccount.MAINACCOUNT + " Total:</span></td><td class='align-right'>" + totalMainOpeningDebit.format() + "</td><td class='align-right'>" + totalMainOpeningCredit.format() + "</td><td class='align-right'>" + mainAccount.DEBIT.format() + "</td><td class='align-right'>" + mainAccount.CREDIT.format() + "</td><td class='align-right'>" + totalMainBalanceDebit.format() + "</td><td class='align-right'>" + totalMainBalanceCredit.format() + "</td></tr>";

                            totalGrandOpeningDebit += totalMainOpeningDebit;
                            totalGrandOpeningCredit += totalMainOpeningCredit;
                            totalGrandBalanceDebit += totalMainBalanceDebit;
                            totalGrandBalanceCredit += totalMainBalanceCredit;
                            totalGrandDebit += mainAccount.DEBIT;
                            totalGrandCredit += mainAccount.CREDIT;
                            //html += "<tr class='total-main-accouont bold'><td colspan='7'><span class='headBalance'>" + mainAccount.MAINACCOUNT + " Total: " + mainAccount.BALANCE.format() + "</span></td></tr>";
                        }
                        html += "<tr class='total-grand-total bold'><td><span class='grand-total'>Grand Total:</span></td><td class='align-right'>" + totalGrandOpeningDebit.format() + "</td><td class='align-right'>" + totalGrandOpeningCredit.format() + "</td><td class='align-right'>" + totalGrandDebit.format() + "</td><td class='align-right'>" + totalGrandCredit.format() + "</td><td class='align-right'>" + totalGrandBalanceDebit.format() + "</td><td class='align-right'>" + totalGrandBalanceCredit.format() + "</td></tr>";
                        $(".report-table tbody").html(html);
                        $("#div-profit").html(res.Data.TotalProfit);
                        $("#div-expense").html(res.Data.TotalExpense);
                        $("#div-netprofit").html(res.Data.TotalNetAmount);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        }
    };
}();