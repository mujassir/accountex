var BalanceSheet = function () {
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
            $this.LoadData();

        },

        LoadData: function () {

            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var qs = "?key=GetBalanceSheet";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&voutype=" + VoucherType[Common.GetQueryStringValue("type")];
            qs += "&OpeningStock=" + $("#OpeningStock").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {

                        var html = "";
                        var select = "";
                        var amount = 0;
                        var heads = res.Data.Records;
                        var headtotal = 0;
                        for (var i in heads) {
                            var head = heads[i];
                            headtotal = head.HeadTotal;
                            html += "<tr class='head'><td colspan='2'>" + head.Head + "</td></tr>";
                            var controlHeads = head.ControlHeads;
                            for (var j in controlHeads) {
                                var controlHead = controlHeads[j];
                                ControlHeadAc = Common.GetByName(controlHead.ControlHead);
                                html += "<tr class='control-head'><td colspan='2'><a href='accountbalances1?fromDate=" + date1 + "&toDate=" + date2 + "&accountId=" + ControlHeadAc.Id + "' target='_blank'>" + controlHead.ControlHead + "</a></td></tr>";
                                var controlheadtotal = 0;
                                var subHeads = controlHead.SubHeads;
                                controlheadtotal = controlHead.ControlHeadTotal;
                                //html += "<tr><td colspan='2'><table class='subheads'>";
                                // html += "<tr><td colspan='2'><table class='subheads'>";

                                for (var k in subHeads) {
                                    var subHead = subHeads[k];
                                    subHeadAc = Common.GetByName(subHead.SubHead);
                                    html += "<tr><td><a href='accountbalances1?fromDate=" + date1 + "&toDate=" + date2 + "&accountId=" + ControlHeadAc.Id + "&childAccountId=" + (subHeadAc != undefined ? subHeadAc.Id : 0) + "' target='_blank'>" + subHead.SubHead + "</a></td><td class='align-right'>" + Common.GetInt(subHead.Amount).format() + "</td></tr>";
                                }
                                html += "<tr class='contolheads-total'><td>Total " + controlHead.ControlHead + "</td><td class='align-right'>" + Common.GetInt(controlheadtotal).format() + "</td></tr>";
                                //html += "</table></td></tr>";
                            }
                            html += "<tr class='grand-total1'><td class='align-right'>Total " + head.Head + "</td><td class='align-right'>" + Common.GetInt(headtotal).format() + "</td></tr>";
                        }



                        $("#table-balance-sheet tbody").html(html);

                        $("#div-profit").html(res.Data.TotalProfit);
                        $("#div-expense").html(res.Data.TotalExpense);
                        $("#div-netprofit").html(res.Data.TotalNetAmount);

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