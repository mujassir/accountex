var ReferralSummary = function () {
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
                var qs = "?key=ReferralSummary";
                qs += Common.MakeQueryStringAll($("#form-info"));

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var data = res.Data;
                            var departments = Enumerable.From(data).Select("$.DepartmentId").Distinct().OrderBy("$").ToArray();

                            var html = "";
                            var headerHtml = "";
                            headerHtml += '<tr>';
                            headerHtml += "<th>Date</th>";
                            var m = 1
                            for (var i in departments) {
                                var departmentId = departments[i];
                                var department = Enumerable.From(data).Where(function (x) { return x.DepartmentId == departmentId }).FirstOrDefault();
                                var code = "";
                                if (department != null)
                                    code = !Common.isNullOrWhiteSpace(department.DepartmentCode) ? department.DepartmentCode : Common.GetById(departmentId).Name

                                headerHtml += '<th>' + code + '</th>';
                            }
                            headerHtml += "<th>Total</th>";
                            headerHtml += '</tr>';
                            var groupRecord = Enumerable.From(data).GroupBy("$.Date", null,
                           function (key, g) {
                               var result = {
                                   Date: key,
                                   Department: g.FirstOrDefault(),
                                   Records: g.ToArray()
                               }
                               return result;
                           }).ToArray();

                            var sn = 1;
                            for (var i in groupRecord) {
                                var referral = groupRecord[i];
                                var totalPatient = 0;

                                html += "<tr>";
                                html += "<td>" + Common.FormatDate(referral.Date, "DD-MM-YYYY") + "</td>";
                                for (var j in departments) {
                                    var departmentId = departments[j];
                                    var patientCount = Enumerable.From(referral.Records).Where(function (x) { return x.DepartmentId == departmentId }).Sum("$.TotalPatient");
                                    totalPatient += patientCount;
                                    html += "<td>" + patientCount + "</td>";
                                }
                                
                                html += "<td>" + totalPatient + "</td>";
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