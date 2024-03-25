var UsmanBrosCustomers = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            //$("#btnShowReport").click($this.LoadData)

            $("#btnShowReport").click(function () {
                $this.LoadData();
            });

            $this.LoadData();
        },
        //LoadAccounts: function () {
        //    var $this = this;
        //    var type = Common.GetQueryStringValue("type").toLowerCase();
        //    var id = PageSetting.Customers;
        //    var tokens = Common.GetLeafAccounts(id);
        //    Common.BindSelect(tokens, "#AreaAccountId", true);
        //},
        LoadPageSetting: function () {

            var tokens = $.parseJSON($("#FormSetting").val());

            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }
            var type = Common.GetQueryStringValue("type");

            this.LoadAccounts();

        },
        LoadData: function () {

            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var FromDate = $("#FromDate").val();
                var ToDate = $("#ToDate").val();
                var AreaAccountId = $("#AreaAccountId").val();

                var qs = "?key=GetUsmanBrosCustomers";
                qs += "&AccountId=" + AreaAccountId;

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {
                            var data = res.Data;
                            $("#UsmanBrosCustomers tbody").html("");
                            var parentCustomers = Enumerable.From(data).Where("$.ParentVendorAccountId==null").ToArray();
                            var childCustomers = Enumerable.From(data).Where("$.ParentVendorAccountId!=null").ToArray();
                            for (var i in parentCustomers) {
                                var parentCustomer = parentCustomers[i];
                                var customersOfParent = $.grep(childCustomers, function (e) { return e.ParentVendorAccountId == parentCustomer.AccountId });
                                var html = "";
                                html += "<tr style='font-weight:bold'>";
                                html += "<td>" + parentCustomer.Area + "</td>";
                                html += "<td>" + (parentCustomer.Code != null ? parentCustomer.Code : '') + "</td>";
                                html += "<td>" + (parentCustomer.Name != null ? parentCustomer.Name : '') + "</td>";
                                html += "<td>" + (parentCustomer.GroupName != null ? parentCustomer.GroupName : '') + "</td>";
                                html += "<td>" + (parentCustomer.Ordertaker != null ? parentCustomer.Ordertaker : '') + "</td>";
                                html += "<td>" + (parentCustomer.Saleman != null ? parentCustomer.Saleman : '') + "</td>";
                                html += "</tr>";
                                for (var j in customersOfParent) {
                                    var childCustomer = customersOfParent[j];
                                    html += "<tr>";
                                    html += "<td>" + childCustomer.Area + "</td>";
                                    html += "<td>" + (childCustomer.Code != null ? childCustomer.Code : '') + "</td>";
                                    html += "<td>" + (childCustomer.Name != null ? childCustomer.Name : '') + "</td>";
                                    html += "<td>" + (childCustomer.GroupName != null ? childCustomer.GroupName : '') + "</td>";
                                    html += "<td>" + (childCustomer.Ordertaker != null ? childCustomer.Ordertaker : '') + "</td>";
                                    html += "<td>" + (childCustomer.Saleman != null ? childCustomer.Saleman : '') + "</td>";
                                    html += "</tr>";
                                }

                                $("#UsmanBrosCustomers tbody").append(html);
                            }

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
        Format: function (num, requireMinusSign) {
            if (requireMinusSign)
                return commafy(num, 0);
            else {
                if (num > -1)
                    return commafy(num, 0);
                else
                    return commafy(num * -1, 0);
            }

        },
    };
}();