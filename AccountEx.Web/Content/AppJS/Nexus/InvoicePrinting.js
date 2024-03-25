var InvoicePrinting = function () {
    var API_CONTROLLER = "NexusInvoice";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            $this.LoadPageSetting();
            $this.LoadDepartment();
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
        },
        
        LoadPageSetting: function () {
            var $this = this;
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        LoadData: function () {

            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var qs = "?key=printInvoice";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "printing...please wait",
                success: function (res) {
                    if (res.Success) {
                        window.open(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        LoadDepartment: function (products) {
            var $this = this;
            var customers = Common.GetLeafAccounts(PageSetting.Customers);
            Common.BindSelect(customers, "#AccountId", true)


        },
    };
}();