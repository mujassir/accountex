
var OverDueAmount = function () {
    var API_CONTROLLER = "VehicleReport";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#BranchId").change(function () {
                $this.LoadData();
            })
            $this.InitSorter();
        },
        InitSorter: function () {
            $(".report-table").tablesorter(
                               {
                                   theme: 'dropbox',
                                   sortList: [[1, 0]],
                                   dateFormat: "ddmmyyyy",
                                   sortInitialOrder: 'desc',
                                   widthFixed: true,
                                   headerTemplate: '{content} {icon}', // Add icon for various themes

                                   widgets: ['zebra12', 'stickyHeaders', 'filter12'],
                                   widgetOptions: {
                                       // jQuery selector or object to attach sticky header to
                                       //stickyHeaders_attachTo: '.mfp-wrap',
                                       stickyHeaders_offset: 25,
                                       //// caption set by demo button value
                                       //stickyHeaders_includeCaption: includeCaption
                                   }
                               }

                               );
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetVehicleOverDueAmounts";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            qs += "&isBadDebit=" + $("#IsBadDebit").is(":checked");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading overdue detail...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var j = 1;
                        var records = res.Data;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";

                            html += "<td>" + j + "</td>";
                            html += "<td>" + Common.FormatDate(record.SaleDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + (record.ChassisNo + "," + record.RegNo) + "</td>";
                            html += "<td>" + record.SalePrice.format() + "</td>";
                            html += "<td class='align-right'>" + record.Deposit.format() + "</td>";
                            html += "<td class='align-right'>" + record.OtherBalance.format() + "</td>";
                            html += "<td class='align-right'>" + record.Balance.format() + "</td>";
                            html += "<td>" + Common.FormatDate(record.MonthlyPaymentDate, "DD-MM-YYYY") + "</td>";
                            html += "<td class='align-right'>" + record.PerMonthInstallment.format() + "</td>";
                            html += "<td>" + record.NoOfMonths + "</td>";
                            html += "<td class='align-right'>" + record.TotalDue.format() + "</td>";
                            html += "<td class='align-right'>" + record.TotalReceivingWithoutDeposit.format() + "</td>";
                            html += "<td class='align-right'>" + record.TotalOverdueTillNow.format() + "</td>";
                            html += "<td class='align-right'>" + record.Penalty.format() + "</td>";
                            html += "<td class='align-right'>" + record.TotalBalanceAllRemaining.format() + "</td>";
                            html += "<td>" + record.CustomerName + "</td>";
                            html += "<td>" + record.ContactNumber + "</td>";
                            html += "<td>" + record.PoBoxNo + " " + record.Address + "</td>";
                            html += "<td>" + record.AgreementRemarks + "</td>";
                            html += "</tr>";
                            j++;
                        }
                        $(".report-table tbody").html(html);
                        if (records.length == 0)
                            html += "  <tr><td colspan='19' style='text-align: center'>No record(s) found</td></tr>";


                        var totalSalePrice = Common.GetInt(Enumerable.From(records).Sum("$.SalePrice"));
                        var totalDeposit = Common.GetInt(Enumerable.From(records).Sum("$.Deposit"));
                        var totalOtherBalance = Common.GetInt(Enumerable.From(records).Sum("$.OtherBalance"));
                        var totalBalance = Common.GetInt(Enumerable.From(records).Sum("$.Balance"));
                        var totalPerMonthInstallment = Common.GetInt(Enumerable.From(records).Sum("$.PerMonthInstallment"));
                        var totalNoOfMonths = Common.GetInt(Enumerable.From(records).Sum("$.NoOfMonths"));
                        var totalDue = Common.GetInt(Enumerable.From(records).Sum("$.TotalDue"));
                        var totalReceivingWithoutDeposit = Common.GetInt(Enumerable.From(records).Sum("$.TotalReceivingWithoutDeposit"));
                        var totalOverdueTillNow = Common.GetInt(Enumerable.From(records).Sum("$.TotalOverdueTillNow"));
                        var totalPenalty = Common.GetInt(Enumerable.From(records).Sum("$.Penalty"));
                        var totalBalanceAllRemaining = Common.GetInt(Enumerable.From(records).Sum("$.TotalBalanceAllRemaining"));
                        var totalMonthlyDue = Common.GetInt(Enumerable.From(records).Sum("$.MonthlyDue"));
                        var totalMonthlyReceiving = Common.GetInt(Enumerable.From(records).Sum("$.MonthlyReceiving"));
                        var totalMonthlyOverdue = Common.GetInt(Enumerable.From(records).Sum("$.MonthlyOverdue"));
                        html = "";
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan = '3' class='align-right'>Total</td>";
                        html += " <td class='align-right'>" + totalSalePrice.format() + "</td>";
                        html += " <td class='align-right'>" + totalDeposit.format() + "</td>";
                        html += " <td class='align-right'>" + totalOtherBalance.format() + "</td>";
                        html += " <td class='align-right'>" + totalBalance.format() + "</td>";
                        html += "<td class='align-right'></td>";
                        html += " <td class='align-right'></td>";
                        html += " <td class='align-right'></td>";
                        html += " <td class='align-right'>" + totalDue.format() + "</td>";
                        html += " <td class='align-right'>" + totalReceivingWithoutDeposit.format() + "</td>";
                        html += " <td class='align-right'>" + totalOverdueTillNow.format() + "</td>";
                        html += " <td class='align-right'>" + totalPenalty.format() + "</td>";
                        html += " <td class='align-right'>" + totalBalanceAllRemaining.format() + "</td>";
                        html += "<td class='align-right'></td>";
                        html += "<td class='align-right'></td>";
                        html += "<td class='align-right'></td>";
                        html += "<td class='align-right'></td>";

                        html += "</tr>";
                        $(".report-table tfoot").html(html);
                        $(".report-table").trigger('update');
                        COUNTER++;


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