var SaleForecastByProductCustomer = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;

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



                var qs = "?key=SaleForecastByProductCustomer";
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

                                //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupByProductRecords = Enumerable.From(saleForecastRecrods)
.GroupBy(
"{ Product: $.Product, Category: $.Category}",
null,
"{Product: $.Product,Category: $.Category,Records:$$.ToArray() }",
"$.Product  + ' ' + $.Category").OrderBy("$.Product")
.ToArray();
                                var months = $this.GetMonths();
                                var colSpan = 5;
                                if (PageSetting.ViewProductSecondaryCategory)
                                    colSpan = 6;
                                var headerHtml = '<tr>';
                                headerHtml += '<th colspan="' + colSpan + '"></th>';
                                headerHtml += '<th colspan="12" class="align-center Bold" style="background-color: powderblue;">Previous 12 months sale</th>';
                                headerHtml += '<th colspan="4" class="align-center Bold" style="background-color: skyblue;">Next 4 months</th>';
                                headerHtml += '</tr>';
                                headerHtml += $this.GetHeaderHtml();
                                // html += headerHtml;
                                for (var j in groupByProductRecords) {

                                    var productRecord = groupByProductRecords[j];
                                    //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                    var groupRecords = Enumerable.From(productRecord.Records)
    .GroupBy(
    "{ Product: $.Product, Customer: $.Customer , Region: $.Region, SalePerson: $.SalePerson , Category: $.Category, SecCategory: $.SecCategory}",
    null,
    "{Product: $.Product, Customer: $.Customer, Region: $.Region, SalePerson: $.SalePerson, Category: $.Category, SecCategory: $.SecCategory,Records:$$.ToArray() }",
    "$.Product  + ' ' + $.Customer + ' ' + $.Region+ ' ' + $.SalePerson + ' ' + $.Category + ' ' + $.SecCategory").OrderBy("$.Customer")
    .ToArray();



                                    for (var i in groupRecords) {
                                        var rate = 0;
                                        var saleForecastByProduct = groupRecords[i];
                                        var records = saleForecastByProduct.Records;

                                        html += "<tr data-divisionid='" + saleForecastByProduct.DivisionId + "'>";
                                        html += "<td>" + saleForecastByProduct.Product + "</td>";
                                        html += "<td>" + saleForecastByProduct.Customer + "</td>";
                                        html += "<td>" + saleForecastByProduct.Region + "</td>";
                                        html += "<td>" + saleForecastByProduct.Category + "</td>";
                                        if (PageSetting.ViewProductSecondaryCategory)
                                            html += "<td>" + saleForecastByProduct.SecCategory + "</td>";
                                        html += "<td>" + saleForecastByProduct.SalePerson + "</td>";
                                        var k = 1;
                                        var customeraAttr = '';
                                        for (var m in months) {
                                            var m = months[m];

                                            if (k > 12)
                                                customeraAttr = '" data-next="true"';
                                            else
                                                customeraAttr = 'data-prev="true"';

                                            var qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.Customer == saleForecastByProduct.Customer }).Sum("$.Quantity");
                                            html += "<td class='align-right bold' " + customeraAttr + ">" + qty + "</td>"
                                            k++;
                                        }
                                        html += "</tr>";

                                    }

                                    var colSpan = 4;
                                    if (PageSetting.ViewProductSecondaryCategory)
                                        colSpan = 5;

                                    html += "<tr class='bold grand-total'>";
                                    html += "<td>" + productRecord.Product + " Total:</td>";
                                    html += "<td colspan='" + colSpan + "'>Customer Total:</td>";
                                    for (var m in months) {
                                        var m = months[m];
                                        var qty = Enumerable.From(productRecord.Records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.Product == productRecord.Product }).Sum("$.Quantity");
                                        html += "<td class='align-right'>" + qty + "</td>"
                                    }



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

        GetHeaderHtml: function () {
            var $this = this;
            var html = "<tr>";
            html += '<th>Product</th>';
            html += '<th>Customer</th>';
            html += '<th>Region</th>';
            html += '<th>Category</th>';
            if (PageSetting.ViewProductSecondaryCategory)
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
            var startDate = moment(d).subtract(12, 'months').toDate();
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