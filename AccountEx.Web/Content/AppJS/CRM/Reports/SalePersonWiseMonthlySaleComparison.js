var SalePersonWiseMonthlySaleComparison = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },



        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var year = $("#Year").val();
                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");

                var qs = "?key=SalePersonWiseMonthlySaleComparison";
                qs += "&year=" + year;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { salePersonIds: salePersonIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var records = res.Data.Table;
                                var salePersons = Enumerable.From(records).Select("$.SalePerson").Distinct().ToArray();
                                var months = Enumerable.From(records).Select("$.Month").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Month</th>";
                                for (var i in salePersons) {
                                    var salePerson = salePersons[i];
                                    headerHtml += "<th colspan='3' class='align-center'>" + salePerson + "</th>";

                                }
                                headerHtml += "<th colspan='3' class='align-center'>Gran Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in salePersons) {
                                    var salePerson = salePersons[i];
                                    headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                    headerHtml += "<th class='align-right'>Import</th>";
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }
                                headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                headerHtml += "<th class='align-right'>Import</th>";
                                headerHtml += "<th class='align-right'>Total</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";


                                for (var m in months) {
                                    html += "<tr>";
                                    var month = months[m];
                                    html += "<td>" + moment.months(month - 1) + "</td>";

                                    for (var i in salePersons) {
                                        var salePerson = salePersons[i];
                                        var exStockTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.SalePerson == salePerson }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(records).Where(function (x) { return x.Month == month && x.SalePerson == salePerson }).Sum("$.ImportValue");
                                        var total = Enumerable.From(records).Where(function (x) { return x.Month == month && x.SalePerson == salePerson }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var exStockTotal = Enumerable.From(records).Where(function (x) { return x.Month == month }).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(records).Where(function (x) { return x.Month == month }).Sum("$.ImportValue");
                                    var total = Enumerable.From(records).Where(function (x) { return x.Month == month }).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";

                                }
                                if (records.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right'>Grand Total</td>";
                                    for (var i in salePersons) {
                                        var salePerson = salePersons[i];
                                        var exStockTotal = Enumerable.From(records).Where(function (x) { return x.SalePerson == salePerson }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(records).Where(function (x) { return x.SalePerson == salePerson }).Sum("$.ImportValue");
                                        var total = Enumerable.From(records).Where(function (x) { return x.SalePerson == salePerson }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var salePerson = salePersons[i];
                                    var exStockTotal = Enumerable.From(records).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(records).Sum("$.ImportValue");
                                    var total = Enumerable.From(records).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);


                            }
                            else {
                                Common.ShowError(res.Error);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
            }
        },


    };
}();