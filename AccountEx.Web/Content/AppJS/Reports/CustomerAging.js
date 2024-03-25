﻿var CustomerAging = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
           var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            var id = Common.GetQueryStringValue("accountId");
            if (id == undefined || id == "") {
                //$("#AccountId").select2("val", id);
                $(".row-account").removeClass("hide");
            }
            $("#AccountCode").keyup(function () {

                var ac = Common.GetByCode($(this).val());
                if (typeof ac == "undefined" || ac == null) {
                    $("#AccountName").val("");
                    $("#AccountId").val("");
                    $("input[name='all-product']").prop("checked", true);
                    $.uniform.update();
                }
                else {
                    $("#AccountName").val(ac.DisplayName);
                    $("#AccountId").val(ac.Id);
                    $("input[name='all-product']").prop("checked", false);
                    $.uniform.update();
                }
            });
            $("input[name='all-product']").change(function () {
                if ($(this).is(":checked")) {
                    $("#AccountName").val("");
                    $("#AccountId").val("");
                    $("#AccountCode").val("");
                }

            });
            $this.LoadPageSetting();

        },
        LoadAccounts: function () {
           var $this = this;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = 0;

            id = PageSetting.Customers;


            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        Name: token.DisplayName,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    $("#AccountName").val(ui.item.Name);
                    $("#AccountId").val(ui.item.id);
                    $("input[name='all-product']").prop("checked", false);
                    $.uniform.update();
                }
            });



        },
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

            var date1 = $("#FromDate").val();
            $("#lblReportDate").html("Date: " + date1);
            var qs = "?key=GetCustomerAging";
            qs += "&date1=" + date1;
            qs += "&accountId=" + $("#AccountId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var account = "";
                        for (var i in data) {



                            html += "<tr><td>" + data[i].Code + "-" + data[i].Name + "</td><td class='align-right'>" + data[i].Current.format()
                                + "</td><td class='align-right'>" + data[i].Day7.format() + "</td><td class='align-right'>" + data[i].Day15.format()
                                  + "</td><td class='align-right'>" + data[i].Day30.format() + "</td><td class='align-right'>" + data[i].Day60.format()
                                    + "</td><td class='align-right'>" + data[i].Day90.format() + "</td><td class='align-right'>" + data[i].Day120.format() + "</td></tr>";

                        }
                        
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td class='align-right'>Total</td><td class='align-right'>" + res.Data.CurrentTotal + "</td><td class='align-right'>" + res.Data.Day7Total + "</td><td class='align-right'>" + res.Data.Day15Total + "</td><td class='align-right'>" + res.Data.Day30Total + "</td><td class='align-right'>" + res.Data.Day60Total + "</td><td class='align-right'>" + res.Data.Day90Total + "</td><td class='align-right'>" + res.Data.Day120Total + "</td></tr>";
                        $(".report-table tbody").html(html);
                        //$(".report-table tfoot").html(html);
                        //$(".opening-balance").html(res.Data.OpeningBalance);
                        //$(".total-debit").html(res.Data.TotalDebit);
                        //$(".total-credit").html(res.Data.TotalCredit);
                        //$(".total-balance").html(res.Data.TotalBalance);
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