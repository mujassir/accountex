
var CustomerRecovery = function () {
    var API_CONTROLLER = "CustomerRecovery";
    var REPORT_HTML = "";
    return {
        init: function () {
            REPORT_HTML = $(".report-div").html();
            $("#Account").select2();
           var $this = this;
            $("#btnShowReport").click(CustomerRecovery.LoadBalances);
        },
        LoadBalances: function (accountId, date) {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();

            $("#lblReportDate").html("Date: " + date1 + " To " + date2);
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?date1=" + date1 + "&date2=" + date2,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading   cutomers recovery...please wait",
                success: function (res) {
                    if (res.Success) {
                        CustomerRecovery.Data = res.Data;
                        CustomerRecovery.FilterReport();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        FilterReport: function () {
            $("#div-table").removeClass("hide");
            $(".report-div").html("");
            var cityName = "";
            var code = "";
            var cityTotal = 0;
            var accountTotal = 0;
            for (var i = 0; i < CustomerRecovery.Data.length; i++) {
                var data = CustomerRecovery.Data[i];
                if (cityName != data.CityName) {
                    if (cityName != "")
                        $(".report-div .report-table:last-child tbody").append("<tr class='group'><td  colspan='3'></td><td  >Total of: " + cityName + "</td><td style='text-align:right !important'>" + cityTotal.format() + "</td></tr>");
                    $(".report-div").append(REPORT_HTML);
                    cityName = data.CityName;
                    $(".report-div h2.title").last().text(cityName);
                    groupName = "";
                    cityTotal = 0;
                }
                if (code != data.Code) {
                    code = data.Code;
                    $(".report-div .report-table:last-child tbody").append("<tr class='group'><td colspan='2'>Code: " + code + "</td><td colspan='3'>Name: " + data.Name + "</td></tr>");
                    accountTotal = 0;
                }
                cityTotal += data.Amount;
                accountTotal += data.Amount;
                var url = Common.GetTransactionUrl(data.TransactionType, data.VoucherNumber);
                var vType = VoucherTypes[data.TransactionType];
                var html = "";
                html += "<tr><td>" + moment(data.Date).format("DD/MM/YYYY") + "</td><td><a href='" + url + "' title='" + vType.Description + "'>" + vType.Code + "-" + data.VoucherNumber + "</a></td><td>"
                    + data.Comments + "</td><td>" + (data.Amount > 0 ? data.Amount.format() : "") + "</td></tr>";
                $(".report-div .report-table:last-child tbody").append(html);
                if (CustomerRecovery.Data[i + 1] != undefined && code != CustomerRecovery.Data[i + 1].Code) {
                    $(".report-div .report-table:last-child tbody").append("<tr class='group'  ><td  colspan='3'></td><td  >Total of: " + data.Name + "</td><td style='text-align:right !important'>" + accountTotal.format() + "</td></tr>");
                }
            }
            if (CustomerRecovery.Data.length > 0) {
                $(".report-div .report-table:last-child tbody").append("<tr class='group' ><td  colspan='3'></td><td  >Total of: " + CustomerRecovery.Data[CustomerRecovery.Data.length - 1].Name + "</td><td style='text-align:right !important'>" + accountTotal.format() + "</td></tr>");
                $(".report-div .report-table:last-child tbody").append("<tr class='group' ><td  colspan='3'></td><td  >Total of: " + CustomerRecovery.Data[CustomerRecovery.Data.length - 1].CityName + "</td><td style='text-align:right !important'>" + cityTotal.format() + "</td></tr>");
            }
        }
    };
}();