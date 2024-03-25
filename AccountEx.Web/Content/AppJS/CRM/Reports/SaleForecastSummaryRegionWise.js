var SaleForecastSummaryRegionWise = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            PageSetting = Common.LoadPageSetting();
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $(document).on("change", "#GroupId", function (event) {
                $reportCommon.GetSubGroupByGroupId();
            });
            $reportCommon.LoadProducts();
            $reportCommon.LoadCustomers();


        },


        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {

                var year = $("#Year").val();
                var month = $("#Month").val();
                var categoryId = $("#CategoryId").val();
                var secCategoryId = $("#SecCategoryId").val();
                var groupId = $("#GroupId").val();
                var subGroupId = $("#SubGroupId").val();

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

                var productIds = $("#ProductIds").val();
                if (!Common.isNullOrWhiteSpace(productIds))
                    productIds = productIds.join(",");



                var qs = "?key=SaleForecastSummaryRegionWise";
                qs += "&year=" + year;
                qs += "&month=" + month;
                qs += "&categoryId=" + categoryId;
                qs += "&secCategoryId=" + secCategoryId;
                qs += "&groupId=" + groupId;
                qs += "&subGroupId=" + subGroupId;
                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: {
                            customerIds: customerIds,
                            salePersonIds: salePersonIds,
                            regionIds: regionIds,
                            divisionIds: divisionIds,
                            productIds: productIds,
                        },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var month = Common.GetInt($("#Month").val());
                                var year = Common.GetInt($("#Year").val());
                                var saleForecastRecrods = res.Data.Table;
                                var regions = Enumerable.From(saleForecastRecrods).Select("$.Region").Distinct().ToArray();
                                var headerColSpan = regions.length + 1;
                                //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupByProductRecords = Enumerable.From(saleForecastRecrods)
.GroupBy(
"{ Product: $.Product, Category: $.Category, SecCategory: $.SecCategory}",
null,
"{Product: $.Product,Category: $.Category,SecCategory: $.SecCategory,Records:$$.ToArray() }",
"$.Product  + ' ' + $.Category + ' ' + $.SecCategory").OrderBy("$.Product")
.ToArray();
                                var months = $this.GetMonths()
                                var headerHtml = "";
                                headerHtml = "<tr>";
                                headerHtml += '<th rowspan="2">Product</th>';
                                headerHtml += '<th rowspan="2">Category</th>';
                                if (PageSetting.ViewProductSecondaryCategory)
                                    headerHtml += '<th rowspan="2">Category 2</th>';
                                for (var i in months) {
                                    var month = months[i];
                                    var monthNo = month.Month;
                                    var year = month.Year;
                                    headerHtml += '<th colspan=' + headerColSpan + ' style="width:3%" class="Month1 align-center">' + moment(month.Date).format('MMM-YY') + '</th>';
                                }
                                headerHtml += '<th colspan=' + headerColSpan + ' style="width:3%" class="Month1 align-center">Grand Total</th>';

                                headerHtml += '</tr>';

                                headerHtml += "<tr>";
                                for (var i in months) {
                                    var month = months[i];
                                    var monthNo = month.Month;
                                    for (var r in regions) {
                                        var region = regions[r];
                                        headerHtml += '<th>' + region + '</th>';
                                    }
                                    headerHtml += '<th>Total</th>';
                                }
                                for (var r in regions) {
                                    var region = regions[r];
                                    headerHtml += '<th>' + region + '</th>';
                                }
                                headerHtml += '<th>Total</th>';

                                headerHtml += '</tr>';
                                //html += headerHtml;


                                for (var j in groupByProductRecords) {

                                    var saleForecastByProduct = groupByProductRecords[j];
                                    var records = saleForecastByProduct.Records;

                                    html += "<tr>";
                                    html += "<td>" + saleForecastByProduct.Product + "</td>";
                                    html += "<td>" + saleForecastByProduct.Category + "</td>";
                                    if (PageSetting.ViewProductSecondaryCategory)
                                        html += "<td>" + saleForecastByProduct.SecCategory + "</td>";
                                    for (var m in months) {
                                        var month = months[m];
                                        var monthNo = month.Month;
                                        var year = month.Year;
                                        for (var r in regions) {
                                            var region = regions[r];
                                            var qty = Enumerable.From(records).Where(function (x) { return x.Month == monthNo && x.Year == year && x.Region == region }).Sum("$.Quantity");
                                            html += "<td class='align-right'>" + qty + "</td>"
                                        }
                                        var qty = Enumerable.From(records).Where(function (x) { return x.Month == monthNo && x.Year == year }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + qty + "</td>"
                                    }
                                    //grand Total

                                    for (var r in regions) {
                                        var region = regions[r];
                                        var qty = Enumerable.From(records).Where(function (x) { return x.Region == region }).Sum("$.Quantity");
                                        html += "<td class='align-right bold'>" + qty + "</td>"
                                    }
                                    var qty = Enumerable.From(records).Sum("$.Quantity");
                                    html += "<td class='align-right bold'>" + qty + "</td>"
                                    html += "</tr>";

                                    //html += "<tr class='bold grand-total'>";
                                    //html += "<td class='align-right' colspan='6'>" + productRecord.Product + " Total</td>";
                                    //for (var m in months) {
                                    //    var m = months[m];
                                    //    var qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.Product == productRecord.Product }).Sum("$.Quantity");
                                    //    html += "<td class='align-right'>" + qty + "</td>"
                                    //}
                                }

                                $("#tbl-sale-detail thead").html(headerHtml);
                                $("#tbl-sale-detail tbody").html(html);
                              
                                Common.BindStickyTableHeaders();

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
        InitSorter: function () {
            $(".report-table").tablesorter(
                               {
                                   theme: 'bootstrap',
                                   sortList: [[1, 0]],
                                   dateFormat: "ddmmyyyy",
                                   sortInitialOrder: 'desc',
                                   widthFixed: true,
                                   headerTemplate: '{content} {icon}', // Add icon for various themes

                                   widgets: ['zebra12', 'stickyHeaders', 'filter12'],
                                   widgetOptions: {
                                       // jQuery selector or object to attach sticky header to
                                       //stickyHeaders_attachTo: '.mfp-wrap',
                                       stickyHeaders_offset: 25,
                                       //// caption set by demo button value
                                       //stickyHeaders_includeCaption: includeCaption
                                   }
                               }

                               );
        },

        GetHeaderHtml: function () {
            var $this = this;
            var html = "<tr>";
            html += '<th>Product</th>';
            html += '<th>Customer</th>';
            html += '<th>Region</th>';
            html += '<th>Category</th>';
            html += '<th>Category 2</th>';
            html += '<th>Sale Person</th>';
            var months = $this.GetMonths();
            var k = 1;
            for (var i in months) {
                var month = months[i];
                var monthNo = month.Month;
                var year = month.Year;
                var customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"';
                if (k > 12)
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-next="true"';
                else
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-prev="true"';

                html += '<th style="width:3%" class="Month1 align-right" ' + customeraAttr + '>' + moment(month.Date).format('MMM-YY') + '</th>';
                k++;
            }
            html += '</tr>';
            return html;
        },
        GetDataRowHtml: function (productId, divisionId, customerId, customer, remarks, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' data-product='" + productId + "' data-body='true'>";
            html += "<td>";
            html += "<input type='hidden' class='ProductId' data-db-column='ProductId' value='" + productId + "'>"
            html += "<input type='hidden' class='CustomerId' data-db-column='CustomerId' value='" + customerId + "'>"
            html += "<input type='text' class='Customer form-control' data-db-column='Customer' value='" + customer + "' >"
            html += "</td>";
            var monthCounter = 1;
            var d = new Date();
            var currentMonth = d.getMonth() + 1;
            var currentYear = d.getFullYear();
            for (var m in Months) {
                var m = Months[m];
                var disable = "";

                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.CustomerId == customerId }).Sum("$.Quantity");
                if (monthCounter <= 12)
                    html += "<td  class='align-right'>" + qty + "</td>";
                else {
                    if (m.Month == currentMonth && m.Year == currentYear)
                        disable = "disabled=disabled";
                    html += "<td  class='align-right'><input type='text' " + disable + " class='form-control input-small num4 Month" + monthCounter + "' data-db-column='Month" + monthCounter + "' data-db-type='int'  value='" + qty + "'></td>";
                }

                monthCounter++;
            }
            if (remarks == null)
                remarks = "";
            html += "<td><input type='text' class='Remarks form-control input-medium' data-db-column='Remarks' value='" + remarks + "' ></td>"
            html += "<td class='td-delete'><span class='action'><i class='fa fa-trash-o action-delete' data-original-title='Delete Item'></i></span></td>";
            html += "</tr>";
            return html;
        },
        GetDataRowTotalHtml: function (productId, divisionId, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' class='bold grand-total' data-product='" + productId + "'>";
            html += "<td class='align-right' colspan='1'> <a id='btn-add-customer' class='btn btn-xs green pull-left' href='javascript:;'>Add Customer</a>&nbspTotal</td>";
            var monthCounter = 1
            for (var m in Months) {
                var m = Months[m];
                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year }).Sum("$.Quantity");
                html += "<td  class='align-right'>" + qty + "</td>";
                monthCounter++;
            }
            html += "<td colspan='2'>";
            html += "</tr>";
            return html;
        },
        GetMonths: function () {
            var $this = this;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            var d = new Date($("#Year").val(), month - 1, 30);
            var startDate = d;
            var endDate = moment(d).add(3, 'M').toDate();
            var k = 1
            var startMonth = month - 3
            var Months = new Array();
            while (startDate <= endDate) {
                var monthNo = startDate.getMonth() + 1;
                var year = startDate.getFullYear();
                Months.push({ Month: monthNo, Year: year, Date: startDate })
                startDate = moment(startDate).add(1, 'M').toDate();
                k++;
            }
            return Months;
        },
    };
}();