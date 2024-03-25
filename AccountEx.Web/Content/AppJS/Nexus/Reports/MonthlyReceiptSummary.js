var MonthlyReceiptSummary = function () {
    var API_CONTROLLER = "NexusReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.SetInitialValue();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },


        SetInitialValue: function () {
            var fromDate = new Date();
            var fromMonth = Common.GetInt(moment(fromDate).format('MM'));
            var fromYear = Common.GetInt(moment(fromDate).format('YYYY'));
            $("#FromMonth").select2("val", fromMonth);
            $("#FromYear").val(fromYear);

        },
        SetPrintParameter: function () {
            var fromMonth = $("#FromMonth").val();
            var fromYear = $("#FromYear").val();

            var toMonth = $("#ToMonth").val();
            var toYear = $("#ToYear").val();

            var fromDate = new Date(fromYear, fromMonth - 1, 1);
            $("#lblFromMonth").html("Month: " + moment(fromDate).format('MMMM YYYY'));

        },

        LoadData: function () {

            var $this = this;

            if (Common.Validate($("#form-info"))) {
                $this.SetPrintParameter();
                var qs = "?key=MonthlyReceiptSummary";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var data = res.Data;
                            var amountTotal = 0
                            var sn = 1;
                            for (var i in data) {
                                var sale = data[i]
                                html += "<tr>";
                                html += "<td>" + sn + "</td>";
                                html += "<td>" + Common.FormatDate(sale.Date, "DD-MM-YYYY") + "</td>";
                                html += "<td class='align-right'>" + sale.ChequeNumber + "</td>";
                                html += "<td class='align-right'>" + Common.FormatDate(sale.ChqDate, "DD-MM-YYYY") + "</td>";
                                html += "<td class='align-right'>" + Common.GetById(sale.DepartmentId).Name + "</td>";
                                html += "<td class='align-right'>" + sale.Amount.format() + "</td>";
                                html += "</tr>";
                                amountTotal += sale.Amount;
                                sn++;
                            }
                            html += "<tr class='bold'>";
                            html += "<td colspan='5' class='align-right'>Grand Total</td>";
                            html += "<td class='align-right'>" + amountTotal.format() + "</td>";
                            html += "</tr>";
                            $("#table-report tbody").html(html);

                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },


    };
}();