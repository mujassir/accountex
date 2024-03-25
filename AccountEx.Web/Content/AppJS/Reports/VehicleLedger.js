
var VehicleLedger = function () {
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
            if (typeof Url.saleId != undefined && Url.saleId != null) {
                $("#VehicleId").select2("val", Url.saleId);
                $this.LoadData();
            }

        },
        LoadData: function () {
            var date1 = $("input[name='FromDate']").val();
            var date2 = $("input[name='ToDate']").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetVehicleLedger";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&saleId=" + $("#VehicleId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading vehicle ledgar...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        var sale = data.Sale;
                        var saleDetails = new Array();
                        if (sale != null)
                            saleDetails = Enumerable.From(sale.VehicleSaleDetails).OrderBy("$.InstalmentNo").ToArray();
                        var payments = data.Payments;
                        var info = data.Info;
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        html += "";
                        var totalRcvd = Enumerable.From(payments).Sum(" Common.GetInt($.Amount)");
                        var totalDue = Enumerable.From(saleDetails).Sum(" Common.GetInt($.Amount)");
                        for (var i in saleDetails) {
                            html += "<tr>";
                            var saleDetail = saleDetails[i];

                            html += "<td>" + saleDetail.InstalmentNo + "</td>";
                            html += "<td>" + Common.FormatDate(saleDetail.InstallmentDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + saleDetail.Amount.format() + "</td>";
                            html += "<td>" + saleDetail.RecievedAmount.format() + "</td>";
                            var Installmentpayments = Enumerable.From(payments).Where(function (x) { return x.VehicleSaleDetailId == saleDetail.Id }).ToArray();
                            var amounts = "";
                            var dates = "";
                            var modes = "";
                            var j = 0;
                            for (j in Installmentpayments) {
                                var payment = Installmentpayments[j];
                                amounts += Common.GetInt(payment.Amount).format() + "+";
                                dates += Common.FormatDate(payment.RecievedDate, "DD-MM-YYYY") + ",";
                                var account = Common.GetById(payment.RcvAccountId);
                                modes += account.Name + ",";

                            }
                            html += "<td>" + amounts.replace(/\+\s*$/, "") + "</td>";
                            html += "<td>" + dates.replace(/,\s*$/, "") + "</td>";
                            html += "<td>" + modes.replace(/,\s*$/, "") + "</td>";
                            html += "</tr>";
                        }
                        var scope = $("#div-table");
                        Common.MapDataWithPrefixFClasses(info, scope, "lbl", "html");
                        var total = 0;
                        if (info != null)
                            total = info.Balance;
                        $(".lblBalance").html(total.format());
                        $("table.tbl-vehicle-detail").removeClass("hide");
                        if (saleDetails.length == 0)
                            html = "  <tr><td colspan='7' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold grand-total'><td colspan='2' class='align-right'>Total</td><td>" + totalDue.format() + "</td><td>" + totalRcvd.format() + "</td><td colspan='2' class='align-right'>Remaining Balance after Installments</td><td class='align-right'>" + (total - totalRcvd).format() + "</td></tr>";
                        //html += "<tr class='bold'><td colspan='3' class='align-right'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td><td>" + res.Data.TotalBalance + "</td></tr>";
                        $("table.report-table tbody").html(html);
                        ///Other expenses and payment related to this vehicle and customer

                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        html = "";
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
                        html += "<tr class='bold'><td colspan='3' class='align-right'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td><td>" + res.Data.TotalBalance.format() + "</td></tr>";
                        var otherBalance = res.Data.TotalBalance;
                        //if (otherBalance < 0)
                        //    otherBalance = Math.abs(otherBalance)
                        html += "<tr class='bold grand-total'><td colspan='3' class='align-right'>Final Balance</td><td></td><td></td><td>" + ((total - totalRcvd) + res.Data.TotalBalance).format() + "</td></tr>";
                        $(".table-other-detail tbody").html(html);


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