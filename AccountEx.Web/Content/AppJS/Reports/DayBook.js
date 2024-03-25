
var DayBook = function () {
    var API_CONTROLLER = "DayBook";
    var REPORT_HTML = "";
    return {
        init: function () {
            REPORT_HTML = $(".report-div").html();
            $("#Account").select2();
           var $this = this;
            $("#btnShowReport").click(DayBook.LoadData);
            //$("body").addClass("page-full-width");
            //$("#GroupName").change(SupplierBalances.FilterReport);
        },
        LoadData: function (accountId, date) {
            var monthNames = ["January", "February", "March", "April", "May", "June",
                              "July", "August", "September", "October", "November", "December"];
            var d = new Date(Common.ChangeDateFormate($("#FromDate").val()));
            $("h4 .month").html(monthNames[d.getMonth()] + " " + d.getFullYear());

            $(".debit-credit").html($("#Type option:selected").text());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var record = {
                date1: Common.ChangeDateFormate($("#FromDate").val()),
                    date2: Common.ChangeDateFormate($("#ToDate").val()),
                isDebit: $("#Type").val()
            };
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "Get",
                data: record,
                success: function (res) {
                    if (res.Success) {
                        DayBook.MapReortData(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        MapReortData: function (collection) {
            $("#div-table-data").removeClass("hide");
            $("#report-table-debit > tbody").html("");
            //$(".report-div").html("");
            if (collection == null || collection.length == 0) {
                var th = $("#report-table-debit > thead th").length;
                $("#report-table-debit > tbody").html("<tr><td colspan='" + th + "' style='text-align:center'>No data to display!</td></tr>");
                return;
            }
            for (var i = 0; i < collection.length; i++) {
                var data = collection[i];
                var date = moment(data.Date).format("DD/MM/YYYY");
                if (data.FromWhomeReceived == null || data.FromWhomeReceived == "undefined")
                    data.FromWhomeReceived = "";
                if (data.OnWhatAccount == null || data.OnWhatAccount == "undefined")
                    data.OnWhatAccount = "";

                var voucherTypes = new Array();
                voucherTypes[5] = "CRV";
                voucherTypes[6] = "CPV";
                voucherTypes[7] = "BRV";
                voucherTypes[8] = "BPV";
                voucherTypes[12] = "JV";
                var html = "";
                html += "<tr><td>" + date + "</td><td>" + voucherTypes[data.VoucherType] + "-" + data.VoucherNumber + "</td><td>" + data.Remarks +
                    "</td><td>" + data.Cash + "</td><td>" + data.Bank + "</td><td>" +
                     data.Rent + "</td><td>" + data.UtitlityCharges + "</td><td>" +
                     data.Electricity + "</td><td>" + data.PossessionCharges + "</td><td>" +
                     data.TfrFee + "</td><td>" + data.SecurityMoney + "</td><td>" +
                      data.PromoActivity + "</td><td>" + data.CarParking + "</td><td>" +
                    data.BankProfit + "</td><td>" + data.Surcharge + "</td><td>" +
                    data.Misc + "</td><td>" + data.CRS + "</td><td>" +
                     data.DRS + "</td></tr>";
                //$("#report-table-data > tbody").append(html);
                $("#div-table-data .report-table:last-child tbody").append(html);
            }
            var th = $("#report-table-debit > thead th").length;
            $("#div-table-data .report-table tbody").append("<tr class='tr-total'></tr>");
            for (var i = 0; i < th; i++) {
                $(".tr-total").append("<td></td>");
            }
            $(".tr-total td:eq(2)").html("Total");
            var sums = new Array();
            $("#div-table-data .report-table tbody tr td").each(function () {
                if ($(this).index() < 3) return;
                if ($(this).text() == "0") {
                    $(this).text("-");
                }
                else {
                    var number = Common.GetInt($(this).text());
                    $(this).text(number.format());
                    if (sums[$(this).index()] == undefined) sums[$(this).index()] = number;
                    else sums[$(this).index()] += number;
                }
            });
            $("tr.tr-total td").each(function () {
                if ($(this).index() < 3) return;
                $(this).html(sums[$(this).index()].format());
            });
            //if ($("#div-table-data .report-table tbody tr").length > 24) {
            //    this.Split($("#div-table-data .report-table"), 22);
            //}
        },
        Split: function ($mainTable, splitBy) {
            alert($mainTable.height());
            var rows = $mainTable.find("tr").slice(splitBy);
            var html = "<table class='" + $($mainTable).attr("class") + "'><thead>" + $($mainTable).find("thead").html() + "</thead><tbody></tbody></table>";
            var $secondTable = $($mainTable).parent().append(html);
            $secondTable.find("tbody").append(rows);
            $mainTable.find("tr").slice(splitBy).remove();
            $(".report-table").wrap("<div class='div-page-break'>");
            alert($mainTable.height());
        }
    };
}();