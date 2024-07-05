var CustomerAging = function () {
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

            $("#GroupName").change(function (e) {
                $this.FilterReport()
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
                        var records = Enumerable.From(data).GroupBy("$.GroupName", null,
                            function (key, g) {
                                var result =
                                {
                                    GroupName: key,
                                    Transactions: g.ToArray(),
                                };
                                return result;
                            }).ToArray();

                        console.log(records)
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr data-group='" + record.GroupName + "' style='font-weight: bold;'><td class='group' colspan='4'>Group Name: " + record.GroupName + "</td></tr>";
                            for (var y in record.Transactions) {
                                const tran = record.Transactions[y]
                                html += "<tr data-group='" + record.GroupName + "'><td>" + tran.Code + "-" + tran.Name + "</td><td class='align-right'>" + tran.Current.format()
                                    + "</td><td class='align-right'>" + tran.Day7.format() + "</td><td class='align-right'>" + tran.Day15.format()
                                    + "</td><td class='align-right'>" + tran.Day30.format() + "</td><td class='align-right'>" + tran.Day60.format()
                                    + "</td><td class='align-right'>" + tran.Day90.format() + "</td><td class='align-right'>" + tran.Day120.format() + "</td></tr>";
                            }

                            let Current = Enumerable.From(record.Transactions).Sum("$.Current").format();
                            let Day7 = Enumerable.From(record.Transactions).Sum("$.Day7").format();
                            let Day15 = Enumerable.From(record.Transactions).Sum("$.Day15").format();
                            let Day30 = Enumerable.From(record.Transactions).Sum("$.Day30").format();
                            let Day60 = Enumerable.From(record.Transactions).Sum("$.Day60").format();
                            let Day90 = Enumerable.From(record.Transactions).Sum("$.Day90").format();
                            let Day120 = Enumerable.From(record.Transactions).Sum("$.Day120").format();
                            
                            html += "<tr data-group='" + record.GroupName + " class='bold'><td class='align-right'>Total</td><td class='align-right'>" + Current + "</td><td class='align-right'>" + Day7 + "</td><td class='align-right'>" + Day15 + "</td><td class='align-right'>" + Day30 + "</td><td class='align-right'>" + Day60 + "</td><td class='align-right'>" + Day90 + "</td><td class='align-right'>" + Day120 + "</td></tr>";
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
        },
        FilterReport: function () {

            var group = $("#GroupName").val();
            
            if (group == "0") {
                $("[data-group]").removeClass("hide");
            }
            else {
                $("[data-group]").addClass("hide");
                $("[data-group='" + group + "']").removeClass("hide");
            }
            $("#div-table").removeClass("hide");

        }
    };
}();