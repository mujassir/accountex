
var SalarySheet = function () {
    var apiController = "SalarySheet";
    return {
        init: function () {
           var $this = this;
           // $(".select2").select2();
            $("#btnShowReport").click(function () {
                var month = $("#Month").val();
                $this.LoadSalarySheet(month);
            });
        },
        LoadSalarySheet: function (month) {
            $("#lblReportDate").html("Month: " + $("#Month option:selected").text() + " " + $("#Year").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "?month=" + month,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  salary sheet...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        var j = 1;
                        for (var i in data) {
                            html += "<tr>";
                            html += "<td>" + (j++) + "</td>";
                            html += "<td>" + data[i].Code + "</td>";
                            html += "<td>" + data[i].Name + "</td>";
                            html += "<td>" + data[i].Designation + "</td>";
                            html += "<td>" + data[i].BasicSalary + "</td>";
                            html += "<td>" + data[i].Absents + "</td>";
                            html += "<td>" + data[i].Allowances + "</td>";
                            html += "<td>" + data[i].OverTime + "</td>";
                            html += "<td>" + data[i].Deductions + "</td>";
                            html += "<td>" + data[i].IncomeTax + "</td>";
                            html += "<td>" + data[i].NetSalary + "</td>";
                            html += "</tr>";
                        }
                        $("#tbldetail tbody").html(html);
                        //html = "<tr><td colspan='2'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td></tr>";
                        //html += "<tr><td colspan='2'></td><td style='text-align: center;' colspan='2' >Difference = " + res.Data.Difference + "</td></tr>";
                        //$("#tbldetail tfoot").html(html);
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