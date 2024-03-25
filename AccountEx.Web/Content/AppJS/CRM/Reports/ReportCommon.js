var ReportCommon = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
           
        },

        LoadProducts: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetProductsIdName",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#ProductIds"), true)

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadCustomersByType: function (key) {
            var $this = this;
            var customerType = Common.GetInt($("#CustomerType").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetCustomerIdNameByUserId&customerType=" + customerType,
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
        GetSubGroupByGroupId: function () {
            var $this = this;
            var groupId = $("#GroupId").val();
            var qs = "?key=GetSubGroupByGroupId";
            qs += "&groupId=" + groupId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#SubGroupId"), true);


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
    };
}();