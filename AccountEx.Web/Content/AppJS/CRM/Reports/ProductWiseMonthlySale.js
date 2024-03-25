var ProductWiseMonthlySale = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    var $reportCommon;
    return {
        init: function () {
            var $this = this;
            $reportCommon = ReportCommon;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $("#CustomerType").change(function () {
                $reportCommon.LoadCustomersByType();
            });
            $reportCommon.LoadCustomersByType();
            $reportCommon.LoadProducts();
            //$('#ToDate').datepicker({
            //    format: 'dd/mm/yyyy',
            //    //startDate: startDate.toDate(),
            //    //endDate: endDate.toDate()
            //    //startDate: new Date('2019-12-5'),
            //    //endDate: new Date('2020-7-12')
            //});

            $("#FromDate").change(function () {
                var startDate = moment(this.value, 'DD-MM-YYYY');
                var endDate = moment(this.value, 'DD-MM-YYYY').add(11, 'M');
                var $toDate = $('#ToDate');
                $toDate.datepicker('setEndDate', endDate.toDate());
                $toDate.datepicker('setDate', endDate.toDate());
            });


        },



        LoadData: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var fromdate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                $("#lblReportDate").html("Date: " + fromdate + " to " + toDate);

                var groupId = $("#GroupId").val();
                var subGroupId = $("#SubGroupId").val();
                var invoiceType = $("#InvoiceType").val();
                var reportType = $("#ReportType").val();

                var productIds = $("#ProductIds").val();

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


                var qs = "?key=ProductWiseMonthlySale";
                qs += "&fromDate=" + fromdate;
                qs += "&toDate=" + toDate;
                qs += "&groupId=" + groupId;
                qs += "&subGroupId=" + subGroupId;
                qs += "&invoiceType=" + invoiceType;
                qs += "&reportType=" + reportType;;
                var deliveryTypeIds = $("#DeliveryTypeIds").val();
                if (!Common.isNullOrWhiteSpace(deliveryTypeIds))
                    deliveryTypeIds = deliveryTypeIds.join(",");


                if (Common.Validate($("#form-info"))) {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                        type: "Post",
                        data: {
                            deliveryTypeIds: deliveryTypeIds,
                            productIds: productIds,
                            regionIds: regionIds,
                            divisionIds: divisionIds,
                            industryTypeIds: industryTypeIds
                        },
                        success: function (res) {
                            if (res.Success) {

                                var html = "";
                                var select = "";
                                var records = res.Data.Table;
                                debugger;
                                var graphMonth = new Array();
                                var years = Enumerable.From(records).Distinct("$.Year").Select("$.Year").OrderBy("$").ToArray();
                                var monthYears = new Array();
                                var i = 1;
                                for (var y in years) {
                                    var year = years[y];

                                    var months = Enumerable.From(records).Where("$.Year=='" + year + "'").Distinct("$.Month").Select("$.Month").OrderBy("$").ToArray();
                                    for (var m in months) {
                                        var month = months[m];
                                        var date = new Date(year, month - 1, 1)
                                        monthYears.push(
                                            {
                                                Year: year,
                                                Month: month,
                                                MonthName: moment(date).format('MMM-YY')
                                            });
                                        i++;
                                    }
                                }

                                var headerHtml = "<tr>";
                                headerHtml += "<th>Region</th>";
                                headerHtml += "<th>Product</th>";
                                headerHtml += "<th>Division</th>";
                                headerHtml += "<th>Group</th>";
                                headerHtml += "<th>Sub Group</th>";
                                for (var my in monthYears) {
                                    var monthYear = monthYears[my];
                                    var date = new Date(monthYear.Year, monthYear.Month - 1, 1)
                                    headerHtml += "<th class='align-right'>" + monthYear.MonthName; + "</th>";
                                }
                                headerHtml += "<th class='align-right'>Total</th>";
                                headerHtml += "</tr>";
                                $("#tbl-summary thead").html(headerHtml);


                                // Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
                                var groupRecords = Enumerable.From(records)
    .GroupBy(
        "{ Region: $.Region , Product: $.Product, Division: $.Division,Group:$.Group,SubGroup:$.SubGroup }",
        "{Value:$.Value,Year:$.Year,Month:$.Month}",
        "{ Region: $.Region , Product: $.Product, Division: $.Division,Group:$.Group,SubGroup:$.SubGroup,Records:$$.ToArray()}",
        "$.Region + ' ' + $.Product+ ' ' + $.Division+ ' ' + $.Group + ' ' + $.SubGroup") // this must be included
    .ToArray();

                                for (var i in groupRecords) {
                                    var groupRecord = groupRecords[i];
                                    var sales = groupRecord.Records;
                                    html += "<tr>";
                                    html += "<td>" + groupRecord.Region + "</td>";
                                    html += "<td>" + groupRecord.Product + "</td>";
                                    html += "<td>" + groupRecord.Division + "</td>";
                                    html += "<td>" + groupRecord.Group + "</td>";
                                    html += "<td>" + groupRecord.SubGroup + "</td>";
                                    for (var my in monthYears) {
                                        var monthYear = monthYears[my];
                                        var value = Enumerable.From(sales).Where(function (x) { return x.Month == monthYear.Month && x.Year == monthYear.Year }).Sum("$.Value");
                                     
                                        html += "<td class='align-right'>" + value.format() + "</td>";


                                    }
                                    var totalValue = Enumerable.From(sales).Sum("$.Value");
                                    html += "<td class='align-right'>" + totalValue.format() + "</td>";
                                    html += "</tr>";

                                }
                                if (records.length == 0)
                                    html += "  <tr><td colspan='11' style='text-align: center'>No record(s) found</td></tr>";
                                else {
                                    html += "<tr class='bold'>";
                                    html += "<td class='align-right' colspan='5'>Grand Total</td>";
                                    for (var my in monthYears) {
                                        var monthYear = monthYears[my];
                                        var value = Enumerable.From(records).Where(function (x) { return x.Month == monthYear.Month && x.Year == monthYear.Year }).Sum("$.Value");
                                        html += "<td class='align-right'>" + value.format() + "</td>";


                                    }
                                    var totalValue = Enumerable.From(records).Sum("$.Value");
                                    html += "<td class='align-right'>" + totalValue.format() + "</td>";
                                    html += "</tr>";
                                }
                                $("#tbl-summary tbody").html(html);






                                //Make Graph Value

                               
                                var values = new Array();
                                for (var my in monthYears) {
                                    var monthYear = monthYears[my];
                                    var value = Enumerable.From(records).Where(function (x) { return x.Month == monthYear.Month && x.Year == monthYear.Year }).Sum("$.Value");
                                    monthYear["Value"] = value;
                                }
                                values.push(
                          {
                              name: "Sale",
                              type: 'bar',
                              data: Enumerable.From(monthYears).Select("$.Value").ToArray()

                          });
                                var options =
                                     {

                                         //legend: {
                                         //    data: ["Sale"]
                                         //},
                                         xAxis: [{
                                             type: 'category',
                                             data: Enumerable.From(monthYears).Select("$.MonthName").ToArray()
                                         }],
                                         yAxis: [{
                                             type: 'value'

                                         }],
                                         series: values
                                     }



                                $this.MakeGraph(options, "echarts_bar_productWiseSale")

                            }

                        },
                        error: function (e) {
                        }
                    });
                }
            }
        },


        MakeGraph: function (newOptions, graphId) {
            debugger;
            require.config({
                paths: {
                    //echarts: '../Content/metronic/assets/global/plugins/echarts/'
                    //zrColor: 'zrender/tool/color',
                    echarts: '../Content/metronic/assets/global/plugins/echarts/'
                }
            });


            // DEMOS
            require(
                [
                    'echarts',
                    'echarts/chart/bar',
                     'echarts/chart/line',
            'echarts/chart/chord',
            'echarts/chart/eventRiver',
            'echarts/chart/force',
            'echarts/chart/funnel',
            'echarts/chart/gauge',
            'echarts/chart/heatmap',
            'echarts/chart/k',
            'echarts/chart/map',
            'echarts/chart/pie',
            'echarts/chart/radar',
            'echarts/chart/scatter',
            'echarts/chart/tree',
            'echarts/chart/treemap',
            'echarts/chart/venn',
            'echarts/chart/wordCloud',
            'zrender/tool/color'

                ],
                function (ec) {
                    //--- BAR ---
                    var myChart = ec.init(document.getElementById(graphId));
                    var options =
                        {
                            tooltip: {
                                trigger: 'axis',
                                orient: 'vertical'
                            },


                            toolbox: {
                                show: true,
                                feature: {
                                    mark: {
                                        show: false
                                    },
                                    dataView: {
                                        show: false,
                                        readOnly: false
                                    },
                                    magicType: {
                                        show: true,
                                        type: ['line', 'bar']
                                    },
                                    restore: {
                                        show: false
                                    },
                                    saveAsImage: {
                                        show: true
                                    }
                                }
                            },

                            calculable: true,
                            xAxis: [{
                                type: 'category',
                                position: 'bottom',
                                boundaryGap: [0, 0.1],
                                axisLabel: {
                                    show: true,
                                    interval: 'auto',
                                    rotate: -20,
                                    margin: 4,
                                    formatter: '{value}',
                                    textStyle: {
                                        //color: 'blue',
                                        //fontFamily: 'sans-serif',
                                        fontSize: 11,
                                        height: 300,
                                        //fontStyle: 'no',
                                        //fontWeight: 'bold'
                                    }
                                }
                            }],
                            yAxis: [{
                                type: 'value',
                                splitArea: {
                                    show: true
                                },
                                //position: 'left',
                                //min: 0,
                                //max: 300,
                                splitNumber: 5,
                                boundaryGap: [0, 0.1],
                                axisLine: {    // 轴线
                                    show: false,
                                    lineStyle: {
                                        color: 'red',
                                        type: 'dashed',
                                        width: 2
                                    }
                                }
                            }],
                        };
                    if (typeof newOptions != "undefined")
                        $.extend(true, options, newOptions);
                    myChart.setOption(options);


                }
            );
        },


    };
}();