var SaleSummaryByProduct = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;
    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $(document).on("change", "#GroupId", function (event) {
                $reportCommon.GetSubGroupByGroupId();
            });
            $("#CustomerType").change(function () {
                $reportCommon.LoadCustomersByType();
            });
            $reportCommon.LoadCustomersByType();
            $reportCommon.LoadProducts();

        },
      
        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var fromdate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                var groupId = $("#GroupId").val();
                var subGroupId = $("#SubGroupId").val();
                var industryTypeId = $("#IndustryTypeId").val();
                var customerType = $("#CustomerType").val();


                var customerIds = $("#CustomerIds").val();
                var productIds = $("#ProductIds").val();
                if (!Common.isNullOrWhiteSpace(customerIds))
                    customerIds = customerIds.join(",");


                var regionIds = $("#RegionIds").val();
                if (!Common.isNullOrWhiteSpace(regionIds))
                    regionIds = regionIds.join(",");

                var divisionIds = $("#DivisionIds").val();
                if (!Common.isNullOrWhiteSpace(divisionIds))
                    divisionIds = divisionIds.join(",");

                if (!Common.isNullOrWhiteSpace(productIds))
                    productIds = productIds.join(",");

                var industryTypeIds = $("#IndustryTypeIds").val();
                if (!Common.isNullOrWhiteSpace(industryTypeIds))
                    industryTypeIds = industryTypeIds.join(",");

                $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
                var qs = "?key=SaleSummaryProductWise";
                qs += "&fromDate=" + fromdate;
                qs += "&toDate=" + toDate;
                qs += "&groupId=" + groupId;
                qs += "&subGroupId=" + subGroupId;
                qs += "&industryTypeId=" + industryTypeId;
                qs += "&customerType=" + customerType
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: {
                            customerIds: customerIds,
                            productIds: productIds,
                            regionIds: regionIds,
                            divisionIds: divisionIds,
                            industryTypeIds: industryTypeIds
                        },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var amount = 0;
                                var detailrecords = res.Data.Table;
                                var sn = 1;
                                var exStockTotal = 0;
                                var exStockqtyTotal = 0;
                                var importTotal = 0;
                                var importqtyTotal = 0;
                                var totalAmount = 0;
                                var totalqty = 0;
                                var regions = Enumerable.From(detailrecords).Select("$.Region").Distinct().ToArray();
                                var groupSummaryRecords = Enumerable.From(detailrecords)
.GroupBy(
"{ Product: $.Product , Division: $.Division, Group: $.Group,SubGroup:$.SubGroup}",
null,
"{ Product: $.Product, Division: $.Division, Group: $.Group,SubGroup: $.SubGroup,Records:$$.ToArray() }",
"$.Product + ' ' + $.Division+ ' ' + $.Group + ' ' + $.SubGroup").OrderBy("$.Product")
.ToArray();

                                var headerHtml = "<tr>";
                                headerHtml += "<th rowspan='2'>Product</th>";
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
                                $("#tbl-summary thead").html(headerHtml);
                                for (var i in groupSummaryRecords) {
                                    var record = groupSummaryRecords[i];
                                    html += "<tr>";
                                    html += "<td>" + record.Product + "</td>";
                                    html += "<td>" + record.Division + "</td>";
                                    html += "<td>" + record.Group + "</td>";
                                    html += "<td>" + record.SubGroup + "</td>";
                                    var productRecords = record.Records;
                                    for (var i in regions) {
                                        var region = regions[i];
                                        var valueTotal = Enumerable.From(productRecords).Where(function (x) { return x.Region == region }).Sum("$.Value");
                                        var qtyTotal = Enumerable.From(productRecords).Where(function (x) { return x.Region == region }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + valueTotal.format() + "</td>";
                                        

                                    }

                                    var valueTotal = Enumerable.From(productRecords).Sum("$.Value");
                                    var qtyTotal = Enumerable.From(productRecords).Sum("$.Quantity");
                                    html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + valueTotal.format() + "</td>";
                                  
                                    html += "</tr>";
                                }
                                if (detailrecords.length == 0)
                                    html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='4'>Grand Total</td>";
                                    for (var i in regions) {
                                        var region = regions[i];
                                        var valueTotal = Enumerable.From(detailrecords).Where(function (x) { return x.Region == region }).Sum("$.Value");
                                        var qtyTotal = Enumerable.From(detailrecords).Where(function (x) { return x.Region == region }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + valueTotal.format() + "</td>";
                                      

                                    }
                                   
                                    var valueTotal = Enumerable.From(detailrecords).Sum("$.Value");
                                    var qtyTotal = Enumerable.From(detailrecords).Sum("$.Quantity");
                                    html += "<td class='align-right'>" + qtyTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + valueTotal.format() + "</td>";
                                    html += "</tr>";

                                }
                               $("#tbl-summary tbody").html(html);
                                html = "";


                                for (var i in groupSummaryRecords) {
                                    var record = groupSummaryRecords[i];
                                    var productRecords = record.Records;
                                    html += "<tr class='tr-group bold'>";
                                    html += "<td colspan='3' class='align-center'>" + record.Product + "</td>";
                                    html += "<td colspan='3' class='align-center'>" + record.Division + "</td>";
                                    html += "<td colspan='2' class='align-center'>" + record.Group + "</td>";
                                    html += "<td colspan='2' class='align-center'>" + record.SubGroup + "</td>";
                                    html += "</tr>";


                                    //html += "<tr>";
                                    //html += "<td colspan='4'>";




                                    //html += "<table class='table table-striped table-bordered'>";
                                    //html += "<thead>";

                                    html += "<tr class='tr-head'>";
                                    html += "<th rowspan='2'>Region</th>";
                                    html += "<th rowspan='2'>Organization</th>";
                                    html += "<th rowspan='2'>Industry</th>";
                                    html += "<th rowspan='2'>Sale Person</th>";
                                    html += "<th class='align-center' colspan='2'>Ex-Stock</th>";
                                    html += "<th class='align-center' colspan='2'>Import</th>";
                                    html += "<th class='align-center' colspan='2'>Total</th>";
                                    html += "</tr>";

                                    html += "<tr>";
                                    html += "<th class='align-right'>Qty</th>";
                                    html += "<th class='align-right'>Value</th>";
                                    html += "<th class='align-right'>Qty</th>";
                                    html += "<th class='align-right'>Value</th>";
                                    html += "<th class='align-right'>Qty</th>";
                                    html += "<th class='align-right'>Value</th>";
                                    html += "</tr>";
                                    //html += "</thead>";

                                    var pgroupRecords = Enumerable.From(productRecords)
.GroupBy(
"{ Organization: $.Organization , Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry }",
null,
"{ Organization: $.Organization, Region: $.Region, SalePerson: $.SalePerson,Industry:$.Industry,Records:$$.ToArray() }",
"$.Organization + ' ' + $.Region+ ' ' + $.SalePerson + ' ' + $.Industry").OrderBy("$.Organization")
.ToArray();
                                    for (var j in pgroupRecords) {
                                        html += "<tr>";
                                        var precord = pgroupRecords[j];
                                        var organizationrecords = precord.Records;

                                        html += "<td>" + precord.Region + "</td>";
                                        html += "<td>" + precord.Organization + "</td>";
                                        html += "<td>" + precord.Industry + "</td>";
                                        html += "<td>" + precord.SalePerson + "</td>";
                                        var exStockValueTotal = Enumerable.From(organizationrecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.ExStock }).Sum("$.Value");
                                        var exStockQtyTotal = Enumerable.From(organizationrecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.ExStock }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + exStockQtyTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + exStockValueTotal.format() + "</td>";
                                        var importValueTotal = Enumerable.From(organizationrecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.Import }).Sum("$.Value");
                                        var importQtyTotal = Enumerable.From(organizationrecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.Import }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + importQtyTotal.format() + "</td>";
                                        html += "<td class='align-right'>" + importValueTotal.format() + "</td>";

                                        html += "<td class='align-right'>" + (exStockqtyTotal + importQtyTotal).format() + "</td>";
                                        html += "<td class='align-right'>" + (exStockValueTotal + importValueTotal).format() + "</td>";
                                        html += "</tr>";
                                    }

                                    html += "<tr class='bold grand-total'>";
                                    html += "<td class='align-right' colspan='4'>Grand Total</td>";
                                    var exStockValueTotal = Enumerable.From(productRecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.ExStock }).Sum("$.Value");
                                    var exStockQtyTotal = Enumerable.From(productRecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.ExStock }).Sum("$.Quantity");
                                    html += "<td class='align-right'>" + exStockQtyTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + exStockValueTotal.format() + "</td>";
                                    var importValueTotal = Enumerable.From(productRecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.Import }).Sum("$.Value");
                                    var importQtyTotal = Enumerable.From(productRecords).Where(function (x) { return x.DeliveryType == CRMSaleDeliveryType.Import }).Sum("$.Quantity");
                                    html += "<td class='align-right'>" + importQtyTotal.format() + "</td>";
                                    html += "<td class='align-right'>" + importValueTotal.format() + "</td>";

                                    html += "<td class='align-right'>" + (exStockqtyTotal + importQtyTotal).format() + "</td>";
                                    html += "<td class='align-right'>" + (exStockValueTotal + importValueTotal).format() + "</td>";
                                    html += "</tr>";
                                    //html += "</table>";

                                    //    sn++;
                                }
                                //html += "</td>";
                                //html += "</tr>";

                                if (detailrecords.length == 0)
                                    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
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