
var GeneralLedger = function () {
    var API_CONTROLLER = "VehicleReport";
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
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            var currencyId = $("#CurrencyId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
            $("#lblParty").html($("#AccountId option:selected").text());
            var qs = "?key=GetGeneralLedger";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&accountId=" + $("#AccountId").val();
            qs += "&branchId=" + branchId;
            qs += "&currencyId=" + currencyId;
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
                        var data = res.Data.Records;
                        //data = Enumerable.From(data).OrderBy(function (p) { return p.TransactionType == 45 }).ThenBy(function (p) { return p.TransactionType == 57 })
                        //    .ThenBy(function (p) { return p.TransactionType == 46 })

                        html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td></td><td>" + res.Data.OpeningBalance + "</td></tr>";
                        for (var i in data) {
                            var vouchertype = Common.GetKeyFromEnum(data[i].TransactionType, VoucherType);
                            var url = Common.GetTransactionUrl(data[i].TransactionType, data[i].VoucherNumber);
                            var vouchershortname = Common.GetKeyFromEnum(data[i].TransactionType, VoucherShortName);
                            var vType = VoucherTypes[data[i].TransactionType];
                            html += "<tr>";
                            html += " <td>" + data[i].Date + "</td>";
                            html += "<td><a href='" + url + "' data-original-title='" + (vType != null ? vType.Description : "") + "' data-toggle='tooltip'>" + (vType != null ? vType.Code + "-" : "") + data[i].VoucherNumber + "</a></td>";
                            html += "<td>" + data[i].Description + "</td>";
                            html += " <td>" + data[i].Debit + "</td>";
                            html += "<td>" + data[i].Credit + "</td>";
                            html += "<td>" + data[i].Balance + "</td>";
                            html += "</tr>";
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td colspan='3' class='align-right'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td><td>" + res.Data.TotalBalance + "</td></tr>";
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