
var NTDataExport = function () {
    var API_CONTROLLER = "Report";
    var excelFile = "";
    return {
        init: function () {
            var $this = this;
            $("#btnExcelExport").click(function () {
                $this.LoadData();
            });
        },
        LoadData: function () {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var dataExportType = $("input[type='radio'][name='order-type']:checked").val();
            var qs = "?key=GetNTDataExport";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&DataExportType=" + dataExportType;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        excelFile = res.Data.FilePath;
                        if (!Common.isNullOrWhiteSpace(excelFile))
                            window.open(excelFile);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        ExportExcel: function () {
            var $this = this;
            Common.ConvertToPDF("ConvertExcel");
        },
    };
}();