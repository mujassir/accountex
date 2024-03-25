var YearlyComparison = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () { $this.LoadData(); });
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
            $this.LoadData();
        },
        LoadAccounts: function () {
            var $this = this;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = 0;
            if (type == "sale") {
                id = PageSetting.Customers;
                $(".main-title").html("Customer Wise Sale");
            }
            else if (type == "salereturn") {
                id = PageSetting.Customers;
                $(".main-title").html("Customer Wise Sale Return");
            }
            else if (type == "purchase") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Supplier Wise Purchase");
            }
            else if (type == "purchasereturn") {
                id = PageSetting.Suppliers;
                $(".main-title").html("Supplier Wise Purchase Return");
            }


            var tokens = Common.GetLeafAccounts(id);
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
                    $("input[name='all-product']").prop("checked", false);
                    $.uniform.update();
                }
            });



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
                var ReportType = $("#ReportType").val();

                var qs = "?key=GetYearlyComparison";


                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var data = res.Data;
                            var i = 0;
                            var firstItem = data[0];
                            var keys = new Array();
                            var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
                            for (var key in firstItem) {
                                if (i == 0) {

                                    i++;
                                    continue;
                                }
                                //keys.push({ Index: i, Key: key });
                                keys.push(key);
                                i++;
                            }

                            var years = Enumerable.From(res.Data).Select("$.Year").Distinct().ToArray();
                            var monthdata = new Array();

                            var formatedResults = new Array();
                            var headers = new Array("Account");
                            var k = 1;
                            //for (var i in data) {




                            //for (var j in years) {
                            //    var year = years[j];
                            //    var key = keys[j];
                            //    var total = transaction[key];
                            //    formatedResult[year] = total;
                            //    if (k == 1)
                            //        headers.push(year);


                            //}
                            for (var j in keys) {
                                var formatedResult = new Object();
                                formatedResult["Account"] = keys[j];


                                for (var r in years) {
                                    var key = keys[j];
                                    var year = years[r];
                                    var transaction = data[r];
                                    var total = transaction[key];
                                    formatedResult[year] = total;
                                    if (k == 1)
                                        headers.push(year);
                                }

                                k++;

                                //}


                                formatedResults.push(formatedResult);
                            }

                            var headerHtml = "";
                            headerHtml += '<tr>';
                            var m = 1
                            for (var i in headers) {


                                headerHtml += '<th >' + headers[i] + '</th>';
                                if (m <= 2) {
                                    if (m % 2 == 0 && i > 0)
                                        headerHtml += '<th >% Change</th>';
                                }
                                else if (m > 2) {
                                    headerHtml += '<th >% Change</th>';
                                }
                                if (i > 0)
                                    m++;
                            }

                            headerHtml += '</tr>';
                            var body = "";
                            var i = 0;
                            formatedResults = Enumerable.From(formatedResults).OrderByDescending("$.Account=='Sales'")
                                .ThenByDescending("$.Account=='CGS'").ThenByDescending("$.Account=='GDP'")
                                .ThenByDescending("$.Account=='Expeneses'").ThenByDescending("$.Account=='NetProfit'").ToArray();
                            for (var j in formatedResults) {
                                var record = formatedResults[j];
                                body += (i % 2 == 0) ? '<tr >' : '<tr>';
                                var m = 1
                                for (var k in headers) {

                                    var token = record[headers[k]];
                                    var token = token == null ? "" : token + "";
                                    if (k > 0)
                                        token = Common.GetInt(token).format();

                                    body += '<td>' + token + '</td>';
                                    if (m <= 2) {
                                        if (m % 2 == 0 && k > 0) {

                                            var firstyear = headers[m - 1];
                                            var lastyear = headers[m];
                                            var firstyearValue = Common.GetInt(record[firstyear]);
                                            var lastyearValue = Common.GetInt(record[lastyear]);
                                            var denomontr = (firstyearValue);
                                            var change = ((lastyearValue - firstyearValue) / denomontr) * 100;
                                            if (firstyearValue == 0)
                                                change = 0.0;
                                            change = change.toFixed("2");
                                            body += '<td>' + change + '</td>';
                                        }
                                    }
                                    else if (m > 2) {

                                        var firstyear = headers[m - 1];
                                        var lastyear = headers[m];

                                        var firstyearValue = Common.GetInt(record[firstyear]);
                                        var lastyearValue = Common.GetInt(record[lastyear]);

                                        var change = ((lastyearValue - firstyearValue) / (firstyearValue)) * 100;
                                        if (firstyearValue == 0)
                                            change = 0.0;
                                        change = change.toFixed("2");
                                        body += '<td>' + change + '</td>';
                                    }
                                    if (k > 0)
                                        m++;
                                }
                                i++;
                                body += '</tr>';
                            }
                            $("#table-yearly-sale-purchase-sumamry thead").html(headerHtml);
                            $("#table-yearly-sale-purchase-sumamry tbody").html(body);
                            $this.MakeGraph(undefined, formatedResults);

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

        MakeGraph: function (newOptions, data) {
            require.config({
                paths: {
                    //echarts: '../Content/metronic/assets/global/plugins/echarts/'
                    echarts: '../Content/metronic/assets/global/plugins/echarts/'
                }
            });
            if (!newOptions) {
                var keys = new Array();
                var values = new Array();

                var i = 0;
                var firstItem = data[0];
                var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
                for (var key in firstItem) {
                    if (key == "Account" || typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                        continue;
                    keys.push({ Index: i, Key: key });
                    i++;
                }
                var requiredKeys = keys;
                var categoryKey = "Account";
                var categories = Enumerable.From(data).Take(Setting.GraphRecords).Select("$.Account").ToArray()
                var legends = Enumerable.From(requiredKeys).Select("$.Key").ToArray();
                data = Enumerable.From(data).ToArray();
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var name = reqKey.Key;
                    values.push(
                        {
                            name: name,
                            type: 'bar',
                            data: Enumerable.From(data).Select(function (x) { return x['' + reqKey.Key + ''] }).ToArray()

                        });
                }
                //for (var i = 0; i < legends.length; i++) {
                //    if (Enumerable.From(reportColumns).Any(function (x) { return x.Name == legends[i] })) {
                //        legends[i] = Enumerable.From(reportColumns).Where(function (x) { return x.Name == legends[i] }).FirstOrDefault().HeaderText;
                //    }
                //}
                newOptions =
                                       {

                                           legend: {
                                               data: legends
                                           },
                                           xAxis: [{
                                               type: 'category',
                                               data: categories
                                           }],
                                           yAxis: [{
                                               type: 'value'

                                           }],
                                           series: values
                                       }
            }

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
            'echarts/chart/wordCloud'

                ],
                function (ec) {
                    //--- BAR ---
                    var myChart = ec.init(document.getElementById('echarts_bar'));
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
                            xAxis: [
     {
         type: 'category',
         position: 'bottom',
         boundaryGap: [0, 0.1],
         axisLabel: {
             show: true,
             interval: 'auto',    // {number}
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