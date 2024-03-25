var InvoicePerformaDateWise = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },


        LoadData: function () {

            var $this = this;
            var fromdate = $("#FromDate").val();
            var toDate = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
            var qs = "?key=InvoicePerformaDateWise";
            qs += "&fromDate=" + fromdate;
            qs += "&toDate=" + toDate;
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: '',
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var records = res.Data.Table;
                            $("#tbl-summary tbody").html(html);
                            html = "";
                            for (var i in records) {
                                var record = records[i];
                                html += "<tr>";

                                html += "<td>" + record.Region + "</td>";
                                html += "<td>" + record.Division + "</td>";
                                html += "<td>" + record.SalePerson + "</td>";
                                html += "<td>" + record.Customer + "</td>";
                                html += "<td>" + record.PINo + "</td>";
                                html += "<td>" + Common.FormatDate(record.PIDate, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + record.Currency + "</td>";
                                html += "<td class='align-right'>" + record.PIValue.format() + "</td>";
                                html += "<td>" + record.PIQty + "</td>";
                                html += "<td>" + Common.FormatDate(record.PIInvoiceDate, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + record.PIInvoiceQty + "</td>";
                                html += "<td>" + record.PIInvoiceValue + "</td>";
                                html += "<td>" + Common.GetKeyFromEnum(record.Status, CRMImportRequisitionStatus) + "</td>";
                                html += "<td>" + record.PILink + "</td>";
                                html += "</tr>";
                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='14' style='text-align: center'>No record(s) found</td></tr>";
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