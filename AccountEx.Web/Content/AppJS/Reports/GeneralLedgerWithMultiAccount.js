
var GeneralLedgerWithMultiAccount = function () {
    var API_CONTROLLER = "Report";
    return {
        init: function () {
            var $this = this;
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
            //if (type == "customers") {
            //    $(".main-title").html("Customer Ledger");
            //    $(document).prop('title', 'Customer Ledger');
            //}
            //else if (type == "suppliers") {
            //    $(".main-title").html("Supplier Ledger");
            //    $(document).prop('title', 'Supplier Ledger ');
            //}
            //else if (type == "products") {
            //    $(".main-title").html("Product Ledger");
            //    $(document).prop('title', 'Product Ledger');
            //}
            //else if (type == "cash") {
            //    $(".main-title").html("Cash Book");
            //    $(document).prop('title', 'Cash Book');
            //}
            //else if (type == "banks") {
            //    $(".main-title").html("Bank Book");
            //    $(document).prop('title', 'Bank Book');
            //}
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
        },
        LoadData: function () {
            var date1 = $("input[name='FromDate']").val();
            var date2 = $("input[name='ToDate']").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
            $("#lblParty").html($("#AccountId option:selected").text());
            var qs = "?key=GetGeneralLedgerWithMultiAccouts";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            var accountIds = $("#AccountId").val();
            if (accountIds != null)
                qs += "&accountIds=" + accountIds.join(",");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ledger...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";

                        var select = "";
                        var amount = 0;
                        var AllRecords = res.Data;
                        for (var j in AllRecords) {

                            var allrcord = AllRecords[j];
                            var data = allrcord.Records;
                            html += "<tr class='bold align-center head'><td colspan='6'>" + allrcord.Account + "</td></tr>";
                            html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td></td><td>" + allrcord.OpeningBalances + "</td></tr>";
                            for (var i in data) {
                                var trans = data[i];
                                var vouchertype = Common.GetKeyFromEnum(trans.TransactionType, VoucherType);
                                var url = Common.GetTransactionUrl(trans.TransactionType, trans.VoucherNumber);
                                var vouchershortname = Common.GetKeyFromEnum(trans.TransactionType, VoucherShortName);
                                var vType = VoucherTypes[trans.TransactionType];
                                html += "<tr>";
                                html += " <td>" + trans.Date + "</td>";
                                html += "<td><a href='" + url + "' data-original-title='" + (vType != null ? vType.Description : "") + "' data-toggle='tooltip'>" + (vType != null ? vType.Code + "-" : "") + trans.VoucherNumber + "</a></td>";
                                html += "<td>" + trans.Description + "</td>";
                                html += " <td>" + trans.Debit + "</td>";
                                html += "<td>" + trans.Credit + "</td>";
                                html += "<td>" + trans.Balance + "</td>";
                                html += "</tr>";
                            }
                            html += "<tr class='bold grand-total'><td colspan='3' class='align-right'>Total</td><td>" + allrcord.TotalDebit.format() + "</td><td>" + allrcord.TotalCredit.format() + "</td><td>" + allrcord.TotalBalance.format() + "</td></tr>";
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

    };
}();