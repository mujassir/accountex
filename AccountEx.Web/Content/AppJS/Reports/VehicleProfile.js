
var VehicleProfile = function () {
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
            var vehicle = $("#VehicleId option:selected").text();
            $("#lblVehicleTitle").html("Vehilce: " + vehicle);
            $("#lblvehicle").html(vehicle);
            var qs = "?key=GetVehicleProfile";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&vehicleId=" + $("#VehicleId").val();
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

                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var info = res.Data.Info;

                        //html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td>" + res.Data.OpeningBalance + "</td></tr>";
                        for (var i in data) {
                            var trans = data[i];
                            var vouchertype = Common.GetKeyFromEnum(trans.TransactionType, VoucherType);
                            var url = Common.GetTransactionUrl(trans.TransactionType, trans.VoucherNumber);
                            var vouchershortname = Common.GetKeyFromEnum(trans.TransactionType, VoucherShortName);
                            var vType = VoucherTypes[trans.TransactionType];
                            var account = Common.GetById(trans.AccountId);
                            html += "<tr>";
                            html += "<td>" + trans.Date + "</td>";
                            html += " <td>" + (account != null ? account.Name : "") + "</td>";
                            html += " <td>" + trans.Description + "</td>";
                            html += " <td class='align-right'>" + trans.Debit + "</td>";
                            html += "</tr>";

                        }
                        var grandTotal = Common.GetIntHtml(res.Data.TotalDebit);
                        if (info != null)
                            grandTotal = grandTotal + info.PurchasePrice;
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='4' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td colspan='3' class='align-right'>Total</td><td class='align-right'>" + res.Data.TotalDebit + "</td></tr>";
                        html += "<tr class='bold'><td colspan='3' class='align-right'>Grand Total</td><td class='align-right'>" + grandTotal.format() + "</td></tr>";
                        $("table.report-table tbody").html(html);

                        var scope = $("#div-table");
                        Common.MapDataWithPrefixFClasses(info, scope, "lbl", "html");
                        $("table.tbl-vehicle-detail").removeClass("hide");
                        var total = 0;
                        if (info != null)
                            total = info.Total - info.TradeInPrice - info.Received
                        $(".lblBalance").html(total.format());

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