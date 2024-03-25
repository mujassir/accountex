var CustomerAndDivisionWiseSaleSummaryDetail = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $("#CustomerType").change(function () { $this.LoadCustomers(); });
            $this.LoadCustomers();
            $this.LoadProducts();

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
        LoadCustomers: function (key) {
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

        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var year = $("#Year").val();
                var customerType = $("#CustomerType").val();
                var customerIds = $("#CustomerIds").val();
                if (!Common.isNullOrWhiteSpace(customerIds))
                    customerIds = customerIds.join(",");

                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");


                var qs = "?key=GetCustomerAndDivisionWiseSaleSummaryDetail";
                qs += "&year=" + year;
                qs += "&customerType=" + customerType
                //qs += "&customerIds=" + customerIds;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: { customerIds: customerIds, salePersonIds: salePersonIds, regionIds: regionIds, divisionIds: divisionIds },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var amount = 0;
                                var sn = 1;
                                var summaryrecords = res.Data.Table;
                                var detailrecords = res.Data.Table1;
                                detailrecords = Enumerable.From(detailrecords).OrderBy("$.Customer").ThenBy("$.Date").ToArray();

                                var divisions = Enumerable.From(summaryrecords).Select("$.Division").Distinct().ToArray();
                                var customers = Enumerable.From(summaryrecords).Select("$.Organization").Distinct().ToArray();
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Customer</th>";
                                headerHtml += "<th rowspan='2'>Sale Person</th>";
                                headerHtml += "<th rowspan='2'>Region</th>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th colspan='3' class='align-center'>" + division + "</th>";

                                }
                                headerHtml += "<th colspan='3' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in divisions) {
                                    var division = divisions[i];
                                    headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                    headerHtml += "<th class='align-right'>Import</th>";
                                    headerHtml += "<th class='align-right'>Total</th>";
                                }
                                headerHtml += "<th class='align-right'>Ex-Stock</th>";
                                headerHtml += "<th class='align-right'>Import</th>";
                                headerHtml += "<th class='align-right'>Total</th>";
                                $("#tbl-summary thead").html(headerHtml);
                                headerHtml += "</tr>";



                                //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupRecords = Enumerable.From(summaryrecords)
.GroupBy(
"{ Customer: $.Customer , Region: $.Region, SalePerson: $.SalePerson }",
null,
"{ Customer: $.Customer, Region: $.Region, SalePerson: $.SalePerson,Records:$$.ToArray() }",
"$.Customer + ' ' + $.Region+ ' ' + $.SalePerson").OrderBy("$.Customer")
.ToArray();


                                for (var g in groupRecords) {
                                    html += "<tr>";
                                    var record = groupRecords[g];
                                    var customerRecords = record.Records;
                                    html += "<tr>";
                                    html += "<td>" + record.Customer + "</td>";
                                    html += "<td>" + record.SalePerson + "</td>";
                                    html += "<td>" + record.Region + "</td>";
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var exStockTotal = Enumerable.From(customerRecords).Where(function (x) { return x.Division == division }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(customerRecords).Where(function (x) { return x.Division == division }).Sum("$.ImportValue");
                                        var total = Enumerable.From(customerRecords).Where(function (x) { return x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var exStockTotal = Enumerable.From(customerRecords).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(customerRecords).Sum("$.ImportValue");
                                    var total = Enumerable.From(customerRecords).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";

                                }

                                if (summaryrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='3'>Grand Total</td>";
                                    for (var i in divisions) {
                                        var division = divisions[i];
                                        var exStockTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.ExStockValue");
                                        var importTotal = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.ImportValue");
                                        var total = Enumerable.From(summaryrecords).Where(function (x) { return x.Division == division }).Sum("$.TotalValue");
                                        html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + total.format() + "</td>";

                                    }
                                    var exStockTotal = Enumerable.From(summaryrecords).Sum("$.ExStockValue");
                                    var importTotal = Enumerable.From(summaryrecords).Sum("$.ImportValue");
                                    var total = Enumerable.From(summaryrecords).Sum("$.TotalValue");
                                    html += "<td class='align-right'>" + exStockTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + total.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);
                                html = "";
                                for (var i in detailrecords) {
                                    var record = detailrecords[i];
                                    html += "<tr>";
                                    html += "<td>" + record.Customer + "</td>";
                                    html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                                    html += "<td>" + record.OGPNo + "</td>";
                                    html += "<td>" + record.InvoiceNumber + "</td>";
                                    html += "<td>" + record.Product + "</td>";
                                    html += "<td>" + record.Division + "</td>";
                                    html += "<td class='align-right'>" + record.Quantity.format() + "</td>";
                                    html += "<td>" + record.Currency + "</td>";
                                    html += "<td>" + record.Price + "</td>";
                                    html += "<td class='align-right'>" + record.Value.format() + "</td>";
                                    html += "<td>" + (record.DeliveryType == 1 ? "Ex-Stock" : "Import") + "</td>";
                                    html += "<td>" + record.Region + "</td>";
                                    html += "</tr>";

                                    sn++;
                                }
                                if (detailrecords.length == 0)
                                    html += "  <tr><td colspan='12' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='6'>Grand Total</td>";

                                    var qtyTotal = Enumerable.From(detailrecords).Sum("$.Quantity");
                                    var netTotal = Enumerable.From(detailrecords).Sum("$.Value");
                                    html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                    html += "<td colspan='2'></td>";
                                    html += "<td class='align-right'>" + netTotal.format() + "</td>";
                                    html += "<td colspan='2'></td>";
                                    html += "</tr>";
                                }
                                $("#tbl-sale-detail tbody").html(html);


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