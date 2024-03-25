var StudentRecord = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var _this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                _this.LoadData();
            });
        },

        LoadData: function () {

            var branchId = $("#BranchId").val();
            var qs = "?key=GetStudentRecord";
            qs += "&BranchId=" + branchId;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        var GrandTotal = Enumerable.From(data).Sum("$.NetAmount");

                        for (var i in data) {
                            var item = data[i];
                            var dateOfAddmission = moment(item.DateOfAdmission).format("DD/MM/YYYY");
                            var dateOfBirth = moment(item.DateOfBirth).format("DD/MM/YYYY");
                            html += "<tr><td>" + item.BLNo + "</td><td>" + item.ShipName + "</td><td>" + item.ShippingCompany + "</td><td>" + item.Units + "</td><td>" + item.Units + "</td><td>" + item.Status
                        }
                        //html += "<tr style='text-align:right;'><td colspan='6'><b>Grand Total</b></td><td><b>" + GrandTotal + "</b></td>";

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
    }
}();