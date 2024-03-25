var ProductWisePotential = function () {
    var API_CONTROLLER = "CRMReport";
    var $reportCommon;

    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            $reportCommon.LoadProducts();
            $reportCommon.LoadCustomers();
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $('#chkselect_all-select2-option-db').click(function () {
                var $Container = $(this).closest(".form-group");
                var $select2 = $Container.find("select.select2");
                if ($(this).is(':checked')) {
                    $select2.select2("enable", false);
                    Common.UpdateRequired($select2, false);
                }
                else {
                    $select2.select2("enable", false)
                    Common.UpdateRequired($select2, true);
                }


            });
            $(document).on("change", "#GroupId", function (event) {
                $reportCommon.GetSubGroupByGroupId();
            });

        },
        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var groupId = $("#GroupId").val();
                var subGroupId = $("#SubGroupId").val();
                var industryTypeId = $("#IndustryTypeId").val();

                var productIds = $("#ProductIds").val();
                if (!Common.isNullOrWhiteSpace(productIds))
                    productIds = productIds.join(",");

                var salePersonIds = $("#SalePersonIds").val();
                if (!Common.isNullOrWhiteSpace(salePersonIds))
                    salePersonIds = salePersonIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var customerIds = $("#CustomerIds").val();
                if (!Common.isNullOrWhiteSpace(customerIds))
                    customerIds = customerIds.join(",");

                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");

                var vendorIds = $("#VendorIds").val();
                if (!Common.isNullOrWhiteSpace(vendorIds))
                    vendorIds = vendorIds.join(",");

                var industryTypeIds = $("#IndustryTypeIds").val();
                if (!Common.isNullOrWhiteSpace(industryTypeIds))
                    industryTypeIds = industryTypeIds.join(",");

                var currencyIds = $("#CurrencyIds").val();
                if (!Common.isNullOrWhiteSpace(currencyIds))
                    currencyIds = currencyIds.join(",");


                var year = $("#Year").val();
                var excludeOwn = $("#chk_exclude-own-product").is(":checked");

                var qs = "?key=ProductWisePotential";
                qs += "&year=" + year;
                qs += "&groupId=" + groupId;
                qs += "&subGroupId=" + subGroupId;
                qs += "&industryTypeId=" + industryTypeId;
                qs += "&excludeOwnProduct=" + (excludeOwn ? "1" : "0");
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: {
                            productIds: productIds,
                            salePersonIds: salePersonIds,
                            regionIds: regionIds,
                            divisionIds: divisionIds,
                            customerIds: customerIds,
                            vendorIds: vendorIds,
                            industryTypeIds: industryTypeIds,
                            currencyIds: currencyIds
                        },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var allrecords = res.Data.Table;
                                var regions = Enumerable.From(allrecords).Select("$.Region").Distinct().ToArray();
                                var groupRecords = Enumerable.From(allrecords).GroupBy("$.Product", null,
                  function (key, g) {
                      var result = {
                          ProductName: key,
                          Product: g.FirstOrDefault(),
                          AnnualQty: g.Sum("$.AnnualQty"),
                          AnnualPotential: g.Sum("$.AnnualPotential"),
                          Records: g.ToArray()
                      }
                      return result;
                  }).ToArray();

                                for (var j in groupRecords) {
                                    var groupRecord = groupRecords[j];

                                    html += "<tr class='group-tr'><td colspan='11' class='group'>" + groupRecord.ProductName + "</td></tr>";
                                    var records = groupRecord.Records;
                                    for (var i in records) {
                                        html += "<tr>";
                                        var potential = records[i];
                                        html += "<td>" + potential.Customer + "</td>";
                                        html += "<td>" + potential.Industry + "</td>";
                                        html += "<td>" + potential.SalePerson + "</td>";
                                        html += "<td>" + potential.Region + "</td>";
                                        html += "<td>" + potential.ProductGroup + "</td>";
                                        html += "<td>" + potential.ProductSubGroup + "</td>";
                                        html += "<td>" + potential.Division + "</td>";
                                        html += "<td>" + potential.Vendor + "</td>";
                                        html += "<td class='align-right'>" + potential.AvgPrice.format() + "</td>";
                                        html += "<td>" + potential.AnnualQty.format() + "</td>";
                                        html += "<td class='align-right'>" + potential.AnnualPotential.format() + "</td>";
                                        html += "</tr>";

                                    }
                                    html += "<tr class='bold subtotal'><td class='align-right' colspan='9'>Total</td><td>" + groupRecord.AnnualQty + "</td><td class='align-right'>" + groupRecord.AnnualPotential + "</td></tr>";
                                }
                                //if (detailrecords.length == 0)
                                //    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                                $("#tbl-sale-detail tbody").html(html);

                                //Summary draw

                                html = "";
                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Product</th>";
                                headerHtml += "<th rowspan='2'>Vendor</th>";
                                headerHtml += "<th rowspan='2'>Division</th>";
                                headerHtml += "<th rowspan='2'>Group</th>";
                                headerHtml += "<th rowspan='2'>Sub Group</th>";
                                for (var i in regions) {
                                    var region = regions[i];
                                    headerHtml += "<th colspan='2' class='align-center'>" + region + "</th>";

                                }
                                headerHtml += "<th colspan='2' class='align-center'>Grand Total</th>";
                                headerHtml += "</tr>";
                                headerHtml += "<tr>";
                                for (var i in regions) {
                                    var region = regions[i];
                                    headerHtml += "<th class='align-right'>Qty</th>";
                                    headerHtml += "<th class='align-right'>Value</th>";
                                }
                                headerHtml += "<th class='align-right'>Qty</th>";
                                headerHtml += "<th class='align-right'>Value</th>";
                                headerHtml += "</tr>";
                                $("#tbl-summary thead").html(headerHtml);
                                groupRecords = Enumerable.From(groupRecords).OrderByDescending("$.AnnualPotential").ToArray();
                                for (var j in groupRecords) {
                                    var groupRecord = groupRecords[j];
                                    var product = groupRecord.Product
                                    html += "<tr></tr>";
                                    html += "<td>" + product.Product + "</td>";
                                    html += "<td>" + product.Vendor + "</td>";
                                    html += "<td>" + product.Division + "</td>";
                                    html += "<td>" + product.ProductGroup + "</td>";
                                    html += "<td>" + product.ProductSubGroup + "</td>";

                                    var productRecords = groupRecord.Records;

                                    for (var i in regions) {
                                        var region = regions[i];
                                        var qty = Enumerable.From(productRecords).Where(function (x) { return x.Region == region }).Sum("$.AnnualQty");
                                        var potentail = Enumerable.From(productRecords).Where(function (x) { return x.Region == region }).Sum("$.AnnualPotential");
                                        html += "<td class='align-right'>" + qty.format() + "</td>";
                                        html += "<td class='align-right'>" + potentail.format() + "</td>";
                                    }
                                    var qty = Enumerable.From(productRecords).Sum("$.AnnualQty");
                                    var potentail = Enumerable.From(productRecords).Sum("$.AnnualPotential");
                                    html += "<td class='align-right'>" + qty.format() + "</td>";
                                    html += "<td class='align-right'>" + potentail.format() + "</td>";
                                }

                                if (allrecords.length == 0)
                                    html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td colspan='3' class='align-right'>Grand Total</td>";
                                    for (var i in regions) {
                                        var region = regions[i];
                                        var qty = Enumerable.From(allrecords).Where(function (x) { return x.Region == region }).Sum("$.AnnualQty");
                                        var potentail = Enumerable.From(allrecords).Where(function (x) { return x.Region == region }).Sum("$.AnnualPotential");
                                        html += "<td class='align-right'>" + qty.format() + "</td>";
                                        html += "<td class='align-right'>" + potentail.format() + "</td>";
                                    }
                                    var qty = Enumerable.From(allrecords).Sum("$.AnnualQty");
                                    var potentail = Enumerable.From(allrecords).Sum("$.AnnualPotential");
                                    html += "<td class='align-right'>" + qty.format() + "</td>";
                                    html += "<td class='align-right'>" + potentail.format() + "</td>";
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