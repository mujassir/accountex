var DetailBillPayment = function () {
    var API_CONTROLLER = "NexusReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            $this.LoadDepartment();
            $this.SetInitialValue();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },

        LoadPageSetting: function () {
            var $this = this;
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        LoadDepartment: function (products) {
            var $this = this;
            var customers = Common.GetLeafAccounts(PageSetting.Customers);
            Common.BindSelect(customers, "#DepartmentId", true)


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
            $("#lblFromMonth").html("From: " + moment(fromDate).format('MMMM YYYY'));
            var toDate = new Date(toYear, toMonth - 1, 1);;
            $("#lblToMonth").html("To: " + moment(toDate).format('MMMM YYYY'));

            $("#lblDepartment").html("Department: " + $("option:selected", $("#DepartmentId")).text());
        },

        LoadData: function () {

            var $this = this;

            if (Common.Validate($("#form-info"))) {
                $this.SetPrintParameter();
                var qs = "?key=DetailBillPayment";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var data = res.Data;
                            var billTotal = 0;
                            var discountTotal = 0;
                            var appTotal = 0;
                            var taxTotal = 0;
                            var netTotal = 0;
                            var totalReferral = 0;
                            for (var i in data) {
                                var sale = data[i]

                                var billAmount = sale.BillingAmount;
                                var discountAmount = sale.Discount;
                                var appAmount = sale.BillingAmount - sale.Discount;
                                var taxAmount = sale.Tax;
                                var netAmount = sale.NetReceived;
                                var referral = sale.TotalPatient;
                                html += "<tr>";
                                html += "<td>" + moment(new Date(sale.Year, sale.Month - 1, 1)).format('MMM-YYYY') + "</td>";
                                html += "<td class='align-right'>" + billAmount.format() + "</td>";
                                html += "<td class='align-right'>" + discountAmount.format() + "</td>";
                                html += "<td class='align-right'>" + appAmount.format() + "</td>";
                                html += "<td class='align-right'>" + taxAmount.format() + "</td>";
                                html += "<td class='align-right'>" + netAmount.format() + "</td>";
                                html += "<td>" + referral + "</td>";
                                html += "</tr>";
                                billTotal += billAmount;
                                discountTotal += discountAmount;
                                appTotal += appAmount;
                                taxTotal += taxAmount
                                netTotal += netAmount;
                                totalReferral = referral;
                            }

                            html += "<tr class='bold'>";
                            html += "<td class='align-right'>Grand Total</td>";
                            html += "<td class='align-right'>" + billTotal.format() + "</td>";
                            html += "<td class='align-right'>" + discountTotal.format() + "</td>";
                            html += "<td class='align-right'>" + appTotal.format() + "</td>";
                            html += "<td class='align-right'>" + taxTotal.format() + "</td>";
                            html += "<td class='align-right'>" + netTotal.format() + "</td>";
                            html += "<td>" + totalReferral + "</td>";
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