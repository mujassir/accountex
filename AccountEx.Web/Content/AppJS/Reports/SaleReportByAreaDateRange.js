var SaleReportByAreaDateRange = function () {
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
            $("#ReportType").change(function () {
                var type = $(this).val();

                if (type == "Qunatity") {
                    $("#report-table tbody tr td.Amount").addClass("hide");
                    $("#report-table tbody tr td.Quantity").removeClass("hide");
                }
                else {
                    $("#report-table tbody tr td.Quantity").addClass("hide");
                    $("#report-table tbody tr td.Amount").removeClass("hide");
                }
            });
            $("#ReportType").change(function () {
                var type = $(this).val();
                if (type.toLocaleLowerCase() == "summary") {
                    $("tr[data-row='summary']").removeClass("hide");
                    $("tr[data-row='detail']").addClass("hide");
                }
                else {
                    $("tr[data-row='summary']").addClass("hide");
                    $("tr[data-row='detail']").removeClass("hide");
                }
            });
            //$this.LoadData();
            //$this.LoadPageSetting();
        },
        LoadAccounts: function () {
            var $this = this;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = PageSetting.Customers;
            var tokens = Common.GetLeafAccounts(id);
            Common.BindSelect(tokens, "#AreaAccountId", true);
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

            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var FromDate = $("#FromDate").val();
                var ToDate = $("#ToDate").val();
                var AreaAccountId = $("#AreaAccountId").val();

                var qs = "?key=SaleReportByAreaDateRange";
                qs += "&FromDate=" + FromDate;
                qs += "&ToDate=" + ToDate;
                qs += "&AccountId=" + AreaAccountId;
                //qs += "&OpeningStock=" + $("#OpeningStock").val();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            $("#report-table tbody").html("");

                            var data = res.Data;

                            var groupdata = Enumerable.From(data).GroupBy("$.Group", null,
                            function (key, g) {
                                var result = {
                                    Group: key,
                                    GroupDetail: g.FirstOrDefault(),
                                    TotalSaleQty: g.Sum("$.SaleQty"),
                                    TotalRate: g.Sum("$.Rate"),
                                    TotalSaleAmount: g.Sum("$.SaleAmount"),

                                    Sales: g.ToArray()
                                }
                                return result;
                            }).ToArray();


                            var html = "";
                            
                            for (var i in groupdata) {
                                var rate = 0;
                                var group = groupdata[i];
                                if (group.TotalSaleQty > 0)
                                    rate = group.TotalSaleAmount / group.TotalSaleQty;

                              
                                if (group.Group == null)
                                    group.Group = "Misc Group";
                                html += "<tr class='hide group" + group.Group.replace(/\s+/g, "-").toLowerCase() + " group-tr'  data-row='detail'><td colspan='10' class='group'>" + group.Group + "</td></tr>";
                                html += "<tr data-row='summary'  class='hide'><td >" + group.Group + "</td><td class='align-right'>" + group.TotalSaleQty.format() + "</td><td class='align-right'>" + rate.format() + "</td><td class='align-right'>" + group.TotalSaleAmount.format() + "</td></tr>";
                                var sales = group.Sales;
                                for (var j in sales) {
                                    var sale = sales[j];

                                    //$("tr[data-row='summary']").removeClass("hide");
                                    //$("tr[data-row='detail']").addClass("hide");
                                    html += "<tr data-row='detail' class='hide'>";

                                    html += "<td>" + sale.Code + "</td>";
                                    html += "<td>" + sale.Name + "</td>";
                                    html += "<td class='align-right'>" + sale.SaleQty.format() + "</td>";
                                    html += "<td class='align-right'>" + sale.Rate.format() + "</td>";
                                    html += "<td class='align-right'>" + sale.SaleAmount.format() + "</td>";
                                    html += "</tr>";




                                }

                                var qtyTotal = Common.GetInt(Enumerable.From(group.Sales).Sum("$.SaleQty"));
                                var amountTotal = Common.GetInt(Enumerable.From(group.Sales).Sum("$.SaleAmount"));

                                var totalrate = 0;
                                if (qtyTotal > 0)
                                    totalrate= amountTotal / qtyTotal;
                                 


                                html += "<tr class='bold align-right subtotal' data-row='detail'><td colspan='2'>Sub Total</td>";
                                html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td class='align-right'>" + totalrate.format() + "</td>";
                                html += "<td class='align-right'>" + amountTotal.format() + "</td>";
                                html += "</tr>";
                              

                            }


                         
                            if (res.Data.length == 0) {
                                html += "  <tr data-row='detail' class='hide'><td colspan='5' style='text-align: center'>No record(s) found</td></tr>";
                                html += "  <tr data-row='summary' class='hide'><td colspan='4' style='text-align: center'>No record(s) found</td></tr>";
                            }
                            else {

                                var qtyTotal = Common.GetInt(Enumerable.From(data).Sum("$.SaleQty"));
                                var amountTotal = Common.GetInt(Enumerable.From(data).Sum("$.SaleAmount"));

                                var totalrate = 0;
                                if (qtyTotal > 0)
                                    totalrate = amountTotal / qtyTotal;



                                html += "<tr style='background-color:skyblue' class='bold grand-total hide align-right'' data-row='detail'><td colspan=2>Grand Total</td>";
                                html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td class='align-right'>" + totalrate.format() + "</td>";
                                html += "<td class='align-right'>" + amountTotal.format() + "</td>";
                                html +=  "</tr>";

                                html += "<tr style='background-color:skyblue' class='bold grand-total hide align-right'' data-row='summary'><td colspan=1>Grand Total</td>";
                                html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                html += "<td class='align-right'>" + totalrate.format() + "</td>";
                                html += "<td class='align-right'>" + amountTotal.format() + "</td>";
                                html += "</tr>";
                            }
                            $("#report-table tbody").html(html);
                            $("#ReportType").trigger("change");
                            
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