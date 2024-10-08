﻿
var NTDetailedGeneralLedger = function () {
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
            var $this = this;
            var date1 = $("input[name='FromDate']").val();
            var date2 = $("input[name='ToDate']").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
            $("#lblParty").html($("#AccountId option:selected").text());
            var qs = "?key=GetNTDetailedGeneralLedger";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + Common.GetQueryStringValue("type").toLowerCase() + " ledger...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var voucherTypes = new Array(VoucherType.goodissue, VoucherType.goodreceive);
                        var amount = 0;
                        var data = res.Data.Records;
                        html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td></td><td>" + res.Data.OpeningBalance + "</td></tr>";
                        for (var i in data) {
                            var vouchertype = Common.GetKeyFromEnum(data[i].TransactionType, VoucherType);
                            var url = Common.GetTransactionUrl(data[i].TransactionType, data[i].VoucherNumber);
                            var vouchershortname = Common.GetKeyFromEnum(data[i].TransactionType, VoucherShortName);
                            var vType = VoucherTypes[data[i].TransactionType];
                            html += "<tr>";
                            html += "<td>" + data[i].Date + "</td>";
                            html += "  <td><a href='" + url + "' data-original-title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + data[i].VoucherNumber + "</a></td>";
                            if ($.inArray(data[i].TransactionType, voucherTypes) !== -1) {
                                html += "<td>" + $this.GetVoucherDetail(data[i].VoucherNumber, data[i].TransactionType, res.Data.DCDetail) + "</td>";
                            } else {
                                html += "<td>" + data[i].Description + "</td>";
                            }
                            html += "<td class='align-right'>" + data[i].Debit + "</td>";
                            html += "<td class='align-right'>" + data[i].Credit + "</td>";
                            html += "<td class='align-right'>" + data[i].Balance + "</td>";
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

        GetVoucherDetail: function (voucherNo, transactionType, deliveryChallans) {
            var records = Enumerable.From(deliveryChallans).Where(function (p) { return p.VoucherNumber == voucherNo && p.TransactionType == transactionType }).ToArray();
            var html = "<table class='boldR'>";
            for (var i in records) {
                var dc = records[i];
                html += "<tr>";
                html += "<td>By Bill No. " + dc.InvoiceNumber + ":" + dc.ItemCode + "_" + dc.ItemName + ":" + dc.Quantity + "X" + dc.Rate + "+" + dc.GSTAmount + "</td>";
                html += "</tr>";

            }
            html += "</table>";
            return html;
        }

    };
}();