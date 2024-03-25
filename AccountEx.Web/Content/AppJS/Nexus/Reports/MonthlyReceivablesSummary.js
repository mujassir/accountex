var MonthlyReceivablesSummary = function () {
    var API_CONTROLLER = "NexusReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.SetInitialValue();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },


        SetInitialValue: function () {
             var fromDate = new Date(Common.Fiscal.FromDate);
            var fromMonth = Common.GetInt(moment(fromDate).format('MM'));
            var fromYear = Common.GetInt(moment(fromDate).format('YYYY'));
            $("#FromMonth").select2("val", fromMonth);
            $("#FromYear").val(fromYear);
            var toDate = new Date(Common.Fiscal.ToDate);
            var toMonth = Common.GetInt(moment(toDate).format('MM'));
            var toYear = Common.GetInt(moment(toDate).format('YYYY'));
            $("#ToMonth").select2("val", toMonth);
            $("#ToYear").val(toYear);
        },
        SetPrintParameter: function () {
            var fromMonth = $("#FromMonth").val();
            var fromYear = $("#FromYear").val();

            var toMonth = $("#ToMonth").val();
            var toYear = $("#ToYear").val();

            var fromDate = new Date(fromYear, fromMonth - 1, 1);
            $("#lblFromMonth").html("Month: " + moment(fromDate).format('MMMM YYYY'));

            //var toDate = new Date(toYear, toMonth - 1, 1);;
            //$("#lblToMonth").html("To: " + moment(toDate).format('MMMM YYYY'));
        },

        LoadData: function () {

            var $this = this;

            if (Common.Validate($("#form-info"))) {
                $this.SetPrintParameter();
                var qs = "?key=ReceivablesSummary";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var data = res.Data;

                            var sn = 1;
                            for (var i in data) {
                                var sale = data[i]
                                html += "<tr>";
                                html += "<td>" + Common.GetById(sale.DepartmentId).Name + "</td>";
                                html += "<td class='align-right'>" + sale.BillingAmount.format() + "</td>";
                                html += "<td class='align-right'>" + sale.Discount.format() + "</td>";
                                html += "<td class='align-right'>" + (sale.BillingAmount - sale.Discount).format() + "</td>";
                                html += "<td class='align-right'>" + sale.Tax.format() + "</td>";
                                html += "<td class='align-right'>" + sale.NetReceived.format() + "</td>";
                              
                               
                                html += "</tr>";
                                sn++;

                            }

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