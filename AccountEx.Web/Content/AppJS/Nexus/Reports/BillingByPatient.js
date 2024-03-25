var BillingByPatient = function () {
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
                var qs = "?key=BillingByPatient";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var html = "";

                            var select = "";
                            var amount = 0;
                            var data = res.Data;
                            //data = Enumerable.From(data).OrderBy(function (p) { return p.TransactionType == 45 }).ThenBy(function (p) { return p.TransactionType == 57 })
                            //    .ThenBy(function (p) { return p.TransactionType == 46 })
                            var sn = 1;
                            var totalAmount = 0;
                            for (var i in data) {
                                var sale = data[i];
                                html += "<tr>";
                                html += " <td>" + sn + "</td>";
                                html += " <td>" + Common.FormatDate(sale.Date, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + sale.EmployeeName + "</td>";
                                html += " <td>" + sale.PatientName + "</td>";
                                html += "<td>" + sale.RefNo + "</td>";
                                html += "<td class='align-right'>" + sale.Amount.format() + "</td>";
                                html += "</tr>";
                                totalAmount += sale.Amount;
                                sn++;
                            }
                            if (data.length == 0)
                                html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                            html += "<tr class='bold'><td colspan='5' class='align-right'>Grand Total</td><td class='align-right'>" + totalAmount.format() + "</td></tr>";
                            $(".report-table tbody").html(html);

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