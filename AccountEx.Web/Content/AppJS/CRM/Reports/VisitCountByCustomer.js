var VisitCountByCustomer = function () {
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
            var year = $("#Year").val();
            var projectId = $("#ProjectId").val();
            if (!Common.isNullOrWhiteSpace(customerIds))
                customerIds = customerIds.join(",");

            var salePersonIds = $("#SalePersonIds").val();
            if (!Common.isNullOrWhiteSpace(salePersonIds))
                salePersonIds = salePersonIds.join(",");

            var qs = "?key=VisitCountByCustomer";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: { customerIds: customerIds, projectId: projectId, salePersonIds: salePersonIds },
                    success: function (res) {
                        if (res.Success) {

                            var html = "";
                            var records = res.Data.Table;
                            for (var i in records) {
                                var record = records[i];
                                var hours = 0;
                                if (record.TotalMinute)
                                    hours = (record.TotalMinute / 60).toFixed(1);
                                html += "<tr>";
                                html += "<td>" + record.Customer + "</td>";
                                html += "<td>" + record.SalePerson + "</td>";
                                html += "<td>" + record.NoOfVisits + "</td>";
                                html += "<td>" + hours + "</td>";
                                html += "<td>" + record.TotalMinute + "</td>";
                                html += "</tr>";
                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='3' style='text-align: center'>No record(s) found</td></tr>";
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