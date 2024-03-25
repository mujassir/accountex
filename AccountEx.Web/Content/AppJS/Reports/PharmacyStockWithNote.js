
var PharmacyStockWithNote = function () {
    var apiController = "Report";
    var DECIMAL_POINTS_FOR_TOTALS = 0;
    var DECIMAL_POINTS_FOR_STOCKTOTALS = 4;
    return {
        init: function () {
            var $this = this;
            $this.LoadStock();
            $("#btnShowReport").click($this.LoadStock);
            $("#AccountId").change($this.LoadStock);
            $("#GroupName").change($this.FilterByGroup);
            $("input[name='stock-type']").change($this.FilterByGroup);
            $('#txtSearch').keyup(function () {

                $this.Filter();
            });

            setInterval($this.LoadStock, 60000);

        },
        FilterByGroup: function () {

            var group = $("#GroupName option:selected").text();
            if (group == undefined || group == "" || group.trim() == "All Groups") {
                $("#tbldetail tbody tr").removeClass("hide");
            }
            else {
                var gr = group.replace(/\s+/g, "-").toLowerCase();
                $("#tbldetail tbody tr:not(.group" + gr + ")").addClass("hide");
                $("#tbldetail tbody tr.group" + gr).removeClass("hide");
            }
            if ($("input[value='stock']").is(":checked")) {
                $(".stock-value").removeClass("hide");
                $(".book-value").addClass("hide");

            }
            else {
                $(".stock-value").addClass("hide");
                $(".book-value").removeClass("hide");
            }
        },

        Filter: function (searchText) {
            searchText = $("#txtSearch").val();
            var $rows = $('#tbldetail tbody tr');
            var val = '^(?=.*\\b' + $.trim(searchText).split(/\s+/).join('\\b)(?=.*\\b') + ').*$',
                                reg = RegExp(val, 'i'), text;

            $rows.show().filter(function () {
                var found = false;
                $(this).find("td").each(function () {

                    text = $(this).text().replace(/\s+/g, ' ');
                    if (reg.test(text))
                        found = true;
                });
                return !found
            }).hide();

        },
        LoadStock: function () {
            var $this = this;
            var fromdate = $("#FromDate").val();
            var todate = $("#ToDate").val();
            //$("#lblReportDate").html("Date: " + date);
            $("#lblReportDate").html("Date: " + fromdate + " to " + todate);
            var qs = "?key=GetPharmacyStockWithNote";
            qs += "&fromdate=" + fromdate;
            qs += "&todate=" + todate;
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  stock...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        var stocks = data.Stocks;
                        var medicines = data.Medicines;
                        var j = 1;
                        var group = "";


                        for (var i in stocks) {
                            var product = stocks[i];
                            var medicine = Enumerable.From(medicines).FirstOrDefault(null, "$.AccountId ==" + product.AccountId + "");
                            if (medicine == null || typeof medicine == undefined)
                                continue;
                            html += "<tr>";
                            html += "<td>" + product.Code + "</td>";
                            html += "<td>" + product.Name + "</td>";
                            html += "<td>" + (!Common.isNullOrWhiteSpace(medicine.MedicineNote) ? medicine.MedicineNote : "") + "</td>";
                            html += "<td class='hide'>" + (!Common.isNullOrWhiteSpace(medicine.Generic) ? medicine.Generic : "") + "</td>";
                            html += "<td class='hide'>" + (!Common.isNullOrWhiteSpace(medicine.Brand) ? medicine.Brand : "") + "</td>";
                            html += "<td>" + product.Balance.format() + "</td>";
                            html += "</tr>";
                        }




                        $("#tbldetail tbody").html(html);
                        PharmacyStockWithNote.FilterByGroup();
                        PharmacyStockWithNote.Filter();
                        //html = "<tr><td colspan='2'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td></tr>";
                        //html += "<tr><td colspan='2'></td><td style='text-align: center;' colspan='2' >Difference = " + res.Data.Difference + "</td></tr>";
                        //$("#tbldetail tfoot").html(html);
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