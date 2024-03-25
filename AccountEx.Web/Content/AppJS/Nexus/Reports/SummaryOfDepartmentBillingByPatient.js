var SummaryOfDepartmentBillingByPatient = function () {
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
            $("#lblFromMonth").html("From: " + moment(fromDate).format('MMMM YYYY'));
            var toDate = new Date(toYear, toMonth - 1, 1);;
            $("#lblToMonth").html("To: " + moment(toDate).format('MMMM YYYY'));
        },

        LoadData: function () {

            var $this = this;

            if (Common.Validate($("#form-info"))) {
                $this.SetPrintParameter();
                var qs = "?key=SummaryOfDepartmentBillingByPatient";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var data = res.Data;
                            var years = Enumerable.From(res.Data).Select("$.Year").Distinct().OrderBy("$").ToArray();
                            var monthYears = new Array();

                            for (var y in years) {
                                var year = years[y];

                                var months = Enumerable.From(res.Data).Where(function (p) { return p.Year == year }).Select("$.Month").Distinct().OrderBy("$").ToArray();
                                for (var m in months) {
                                    var month = months[m];

                                    monthYears.push(
                                        {
                                            Year: year,
                                            Month: month,
                                            Name: moment(new Date(year, month - 1, 1)).format('MMM YYYY')
                                        });
                                }

                            }
                            var html = "";
                            var headerHtml = "";
                            headerHtml += '<tr>';
                            headerHtml += "<th rowspan='2'>Sr #</th>";
                            headerHtml += "<th rowspan='2'>Department</th>";


                            var m = 1
                            for (var i in monthYears) {
                                var monthYear = monthYears[i];
                                headerHtml += '<th colspan="2" class="align-center">' + monthYear.Name + '</th>';
                            }
                            headerHtml += '</tr>';
                            headerHtml += '<tr>';
                            for (var i in monthYears) {

                                headerHtml += "<th>Patient</th>";
                                headerHtml += "<th class='align-right'>Amount</th>";

                            }
                            headerHtml += '</tr>';

                            var groupRecord = Enumerable.From(data).GroupBy("$.DepartmentId", null,
                           function (key, g) {
                               var result = {
                                   DepartmentId: key,
                                   DepartmentName: Common.GetById(key).Name,
                                   Records: g.ToArray()
                               }
                               return result;
                           }).ToArray();

                            var sn = 1;
                            for (var i in groupRecord) {
                                var sale = groupRecord[i];
                                var totalAmount = 0;

                                //$("tr[data-row='summary']").removeClass("hide");
                                //$("tr[data-row='detail']").addClass("hide");

                                html += "<tr>";
                                html += "<td>" + sn + "</td>";
                                html += "<td>" + sale.DepartmentName + "</td>";
                                for (var i in monthYears) {
                                    var monthYear = monthYears[i];
                                    var amount = Enumerable.From(sale.Records).Where(function (x) { return x.Month == monthYear.Month && x.Year == monthYear.Year }).Sum("$.Amount");
                                    var totalPatient = Enumerable.From(sale.Records).Where(function (x) { return x.Month == monthYear.Month && x.Year == monthYear.Year }).Sum("$.TotalPatient");
                                    totalAmount += amount;
                                    html += "<td>" + totalPatient + "</td>";
                                    html += "<td class='align-right'>" + amount.format() + "</td>";
                                }

                                html += "</tr>";
                                sn++;

                            }

                            $("#table-yearly-billing-sumamry thead").html(headerHtml);
                            $("#table-yearly-billing-sumamry tbody").html(html);

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