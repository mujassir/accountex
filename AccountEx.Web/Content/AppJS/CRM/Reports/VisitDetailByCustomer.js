var VisitDetailByCustomer = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.LoadCustomers();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },

        LoadCustomers: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetCustomerIdNameByUserId",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#CustomerIds"), true)
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadData: function () {

            var $this = this;
            var fromdate = $("#FromDate").val();
            var toDate = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
            var customerIds = $("#CustomerIds").val();
            var projectId = $("#ProjectId").val();
            var year = $("#Year").val();
            if (!Common.isNullOrWhiteSpace(customerIds))
                customerIds = customerIds.join(",");
            var qs = "?key=VisitDetailByCustomer";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: { customerIds: customerIds, projectId: projectId },
                    success: function (res) {
                        if (res.Success) {

                            var html = "";
                            var sn = 1;
                            var records = res.Data.Table;
                            for (var i in records) {
                                var record = records[i];
                                html += "<tr>";
                                html += "<td>" + record.Customer + "</td>";
                                html += "<td>" + record.SalePerson + "</td>";
                                html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + Common.FormatDate(record.StartTime, "hh:mm A") + "</td>";
                                html += "<td>" + Common.FormatDate(record.EndTime, "hh:mm A") + "</td>";
                                html += "<td>" + record.Mode + "</td>";
                                html += "<td>" + record.Project + "</td>";
                                html += "<td>" + record.Status + "</td>";
                                html += "<td>" + record.Description + "</td>";
                                html += "</tr>";
                                sn++;
                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                            $("#tbl-sale-detail tbody").html(html);


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