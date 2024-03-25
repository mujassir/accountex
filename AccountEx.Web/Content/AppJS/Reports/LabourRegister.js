
var Register = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            PageSetting = Common.LoadPageSetting();
            $this.BindSalesMan();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
        },
        BindSalesMan: function (key) {
            var $this = this;
            var employee = Common.GetLeafAccounts(PageSetting.Employee);
            Common.BindSelect(employee, $("#EmployeeId"), true)

        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var employeeId = $("#EmployeeId").val();
            var qs = "?key=GetLabourReport";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&employeeId=" + employeeId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading please wait...",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data.Records;

                        if (res.Data.Records.length == 0) {
                            html += "  <tr><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                        }
                        else {
                            html += data.map(e =>
                                `<tr>
                                    <td>${(new Date(e.Date)).toLocaleDateString('en-GB')}</td>
                                    <td>${e.VoucherNumber}</td>
                                    <td>${e.InvoiceNumber}</td>
                                    <td>${e.ItemCode}</td>
                                    <td>${e.ItemName}</td>
                                    <td>${e.Comments}</td>
                                    <td>${e.Quantity}</td>
                                    <td>${e.Rate}</td>
                                    <td>${e.Amount}</td>
                                </tr>`
                            )

                        }
                        const htmlFooter = `
                            <tr>
                                <th colspan="6" style="text-align:right">Total:</th>
                                <th class="text-right">${res.Data.TotalQuantity}</th>
                                <th></th>
                                <th class="text-right">${res.Data.TotalAmount}</th>
                            </tr>`

                        $(".report-table tbody").html(html);
                        $(".report-table tfoot").html(htmlFooter);
                        $('#column-hide-show-container select').trigger('change');

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