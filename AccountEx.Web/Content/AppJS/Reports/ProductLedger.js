var ProductLedger = function () {
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
            $this.LoadPageSetting();

        },
        LoadAccounts: function () {
           var $this = this;
            var id = 0;



            var tokens = Common.GetLeafAccounts(PageSetting.ProductAccount);
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

                }
            });



        },
        LoadPageSetting: function () {

            var tokens = $.parseJSON($("#FormSetting").val());

            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.KeyName] = token.Value;
            }

            this.LoadAccounts();

        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetProductLedger";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
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
                        html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td></td><td></td><td>" + res.Data.OpeningBalance + "</td></tr>";
                        for (var i in data) {

                            html += "<tr><td>" + data[i].Date1 + "</td><td>" + data[i].TransactionType
                                + "</td><td>" + data[i].VoucherNumber + "</td><td>" + data[i].Comments
                                + "</td><td>" + (data[i].StockIn > 0 ? data[i].StockIn : "") + "</td><td>" + (data[i].StockOut > 0 ? data[i].StockOut : "") + "</td><td>"
                                + data[i].Balance1 + "</td></tr>";
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='7' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td>Total</td><td></td><td></td><td class='align-right'>" + res.Data.TotalStockIn + "</td><td class='align-right'>" + res.Data.TotalStockOut + "</td><td class='align-right'>" + res.Data.TotalBalance + "</td></tr>";
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