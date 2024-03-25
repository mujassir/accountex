var XDashboard = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "XDashboard";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var PageSetting = new Object();
    var PageSetting = new Object();
    var widgetObject;
    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            $this.GetDashboard();
            $(document.body).on("click", "a[data-toggle]", function (event) {
                setTimeout(function () {
                    App.destroySlimScroll(".scroller");
                    App.initSlimScroll('.scroller');
                }, 100);
            });

        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        GetDashboard: function () {
            var $this = this;
            var lastLoadedAt = moment(new Date()).format("YYYY-MM-DD hh:mm:ss");
            var dashboardName = Common.GetQueryStringValue("name");
            var lsKey = $this.GetLocalStorageKey();
            var lastLoadedAtValue = localStorage.getItem(lsKey);
            if (lastLoadedAtValue != undefined && lastLoadedAtValue != null && lastLoadedAtValue != "")
                lastLoadedAt = lastLoadedAtValue;


            var record = {
                Key: "GetDashboard",
                lastLoadTime: lastLoadedAt,
                DashboardName: dashboardName
            };
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "GET",
                data: record,
                success: function (res) {
                    $("#widget-container").html('');
                    localStorage.setItem(lsKey, moment(new Date()).format("YYYY-MM-DD hh:mm:ss"));


                    var tileWidget = Enumerable.From(res.Data.Widgets).Where(function (x) { return x.IsTileWidget == true }).FirstOrDefault();
                    if (tileWidget != undefined && tileWidget != null) {

                        $this.LoadWidgetData(tileWidget, function (records) {
                            var html = $this.CreateTiles(records, tileWidget);
                            $("#tile-container").append(html);
                            App.destroySlimScroll(".scroller");
                            App.initSlimScroll('.scroller');
                            Common.InitCounterup();
                        });
                    }

                    var otherWidgets = Enumerable.From(res.Data.Widgets).Where(function (x) { return x.IsTileWidget == false }).ToArray();
                    otherWidgets = Enumerable.From(otherWidgets).OrderBy(function (x) { return x.SequenceNo }).ToArray();
                    $.each(otherWidgets, function (index, widget) {
                        $this.LoadWidgetData(widget, function (records) {
                            if (widget.JsFunction != undefined && widget.JsFunction != null
                                 && widget.JsFunction != "null" && widget.JsFunction != "") {
                                html = eval("XDashboard." + widget.JsFunction)(records, widget);
                            }
                            else {

                                html = $this.CreateTableView(records, true, null, widget)
                                $("#widget-container").append(html);
                                if (records.length > 0 && widget.GraphEnabled) {
                                    var type = widget.GraphType;
                                    switch (type) {
                                        case GraphType.PieChart:
                                            XDashboard.MakePieChart(null, widget, records);
                                            break;
                                        default:
                                            XDashboard.MakeGraph(null, widget, records);
                                            break;

                                    }

                                }
                                $("a#full-screen").click();
                            }
                            if (records.length == 0) {
                                $("#info-message").removeClass("hide");
                            }
                            if (!widget.GraphEnabled) {
                                $("#widget-container .graph-container").remove();
                                $("#widget-container .data-container").removeClass("col-md-6").addClass("col-md-12")
                            }

                            App.destroySlimScroll(".scroller");
                            App.initSlimScroll('.scroller');

                        });

                    });

                }
            });

        },

        LoadWidgetData: function (widget, callback) {
            var $this = this;
            if (!widget.IsReloadRequired) {
                var records = null;
                try {
                    records = JSON.parse(localStorage.getItem($this.GetLocalStorageKeyForWidget(widget.WidgetName)));

                } catch (e) {

                }

                if (records != undefined && records != null) {
                    callback(records, widget);
                    return;
                }
            }


            var record = {
                Id: widget.Id,
                //Parameters: params,
            };
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                async: false,
                data: record,
                success: function (res) {
                    $("#info-message").addClass("hide");
                    if (res.Success) {
                        localStorage.setItem($this.GetLocalStorageKeyForWidget(widget.WidgetName), JSON.stringify(res.Data));
                        callback(res.Data, widget);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        CreateTiles: function (records, widgetObject) {
            var html = "";
            $.each(records, function (index, el) {
                var pStart = "", pEnd = "";
                if (el.Balance < 0) {
                    // By Formating Balance to accounting number in case less than zero number paranthesis were added
                    // that counter up couldn't read and shows NaN until counter goes to highest number
                    // To avoid that NaN Balance multiplied with -1 to make it always +ve number and paranthesis added manually
                    el.Balance = el.Balance * -1;
                    pStart = "(";
                    pEnd = ")";
                }
                var templatehtml = $("#template-tile").html();
                var obj = {
                    '[Label]': el.Label,
                    '[Balance]': Common.GetInt(el.Balance).format(),
                    '[Icon]': el.Icon,
                    '[Color]': el.Color,
                    '[PStart]': pStart,
                    '[PEnd]': pEnd,

                }
                html += templatehtml.allReplace(obj);
            });
            return html;
        },
        CreateTableView: function (data, enableHeader, theme, reportObject) {
            var $this = this;
            var regex = /([^A-Za-z0-9\.\$])|([A-Z])(?=[A-Z][a-z])|([^\-\$\.0-9])(?=\$?[0-9]+(?:\.[0-9]+)?)|([0-9])(?=[^\.0-9])|([a-z])(?=[A-Z])/g;
            // set optional theme parameter
            if (theme === undefined) {
                theme = 'mediumTable'; //default theme
            }

            if (enableHeader === undefined) {
                enableHeader = true; //default enable headers
            }

            // If the returned data is an object do nothing, else try to parse
            var data = typeof data != 'object' ? JSON.parse(data) : data;
            var i = 0;
            firstItem = data[0];
            var keys = new Array();
            var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
            for (var key in firstItem) {
                if (typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                    continue;
                keys.push({ Index: i, Key: key });
                i++;
            }

            var reportColumns = reportObject.WidgetColumns;

            var firstItem = data[0];
            var headerStart = (typeof reportObject.HeaderStart == "undefined" || reportObject.HeaderStart == null) ? 0 : reportObject.HeaderStart;
            var headerEnd = (typeof reportObject.HeaderEnd == "undefined" || reportObject.HeaderEnd == null || reportObject.HeaderEnd == 0) ? keys.length : reportObject.HeaderEnd > 50 ? keys.length : reportObject.HeaderEnd;
            var skipHeaders = (typeof reportObject.SkipHeaders == "undefined" || reportObject.SkipHeaders == null) ? new Array() : reportObject.SkipHeaders.split(",");

            skipHeaders = Enumerable.From(skipHeaders).Where("$.trim()!=''").Select(function (x) { return Common.GetInt(x) }).ToArray();
            var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= headerStart && x.Index <= headerEnd && $.inArray(x.Index, skipHeaders) === -1 }).ToArray();
            var selectSelector = "x => {";
            for (var key in requiredKeys) {
                var reqKey = requiredKeys[key];
                selectSelector += "'" + reqKey.Key + "': x['" + reqKey.Key + "'],"
            }
            selectSelector = selectSelector.replace(/,\s*$/, "");
            selectSelector += "}";

            var data = Enumerable.From(data)
        .Select(selectSelector) // object initializer
        .ToArray();

            var headers = "";
            var body = "";
            var t;
            // table head

            if (enableHeader) {
                headers += '<tr>';
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var index = reqKey.Key;
                    if (Enumerable.From(reportColumns).Any(function (x) { return x.Name == index }))
                        index = Enumerable.From(reportColumns).Where(function (x) { return x.Name == index }).FirstOrDefault().HeaderText;
                    else
                        var index = index.replace(regex, '$2$3$4$5 ');

                    headers += '<th >' + index + '</th>';
                }
                headers += '</tr>';
            }

            // table body
            var i = 0;
            for (var j in data) {
                var record = data[j];
                body += (i % 2 == 0) ? '<tr >' : '<tr>';
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var token = record[reqKey.Key];
                    var token = token == null ? "" : token + "";
                    if (token != null && token.indexOf("T00:00:00") > -1)
                        token = moment(token).format('DD/MM/YYYY');
                    var formattingType = 0;
                    var mapping = new Object();
                    mapping = Enumerable.From(reportColumns).FirstOrDefault(null, function (x) { return x.Name == reqKey.Key });
                    if (mapping != null) {

                        if (mapping.BodyFormatingType > 0 && mapping.BodyFormatingType != null)
                            token = $this.FormatNumber(token, mapping.BodyFormatingType, mapping.Precision);


                    }
                    else {
                        var intValue = Common.GetInt(token);
                        if (intValue > 0)
                            token = intValue.format();
                    }
                    body += '<td>' + token + '</td>';

                }
                i++;
                body += '</tr>';
            }
            var tfoot = "";
            if (reportObject.ShowFooter) {
                var totalColspan = 0;
                tfoot += '<tr class="bold grand-total">';
                for (var key in requiredKeys) {
                    if (totalColspan > 0) {
                        totalColspan = totalColspan - 1;
                        continue;
                    }
                    var reqKey = requiredKeys[key];
                    var index = reqKey.Key;
                    var colspan = "";
                    var footerText = "";
                    var formattingType = 0;
                    var mapping = new Object();
                    mapping = Enumerable.From(reportColumns).FirstOrDefault(null, function (x) { return x.Name == index });
                    if (mapping != null) {
                        colspan = mapping.Colspan != null ? 'colspan="' + mapping.Colspan + '"' : "";
                        totalColspan = mapping.Colspan != null ? mapping.Colspan : 0;
                        footerText = mapping.FooterText != null ? mapping.FooterText : "";
                        if (mapping.ShowSum) {
                            footerText = Enumerable.From(data).Sum(function (x) { return x['' + reqKey.Key + ''] });
                            if (mapping.FooterFormatingType > 0 && mapping.FooterFormatingType != null)
                                footerText = $this.FormatNumber(footerText, mapping.FooterFormatingType, mapping.Precision);

                        }
                    }
                    totalColspan = totalColspan - 1;
                    tfoot += '<td ' + colspan + '>' + footerText + '</td>';


                }
                tfoot += '</tr>';

            }
            var templatehtml = "";
            if (reportObject.Template != null && reportObject.Template.trim() != "")
                templatehtml = $(reportObject.Template).html();
            else if (!reportObject.GraphEnabled) {
                templatehtml = $("#template-default-without-graph").html();
            }
            else
                templatehtml = $("#template-default").html();
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers,
                '[TableFooter]': tfoot,
                '[Name]': reportObject.Name,
                '[Id]': reportObject.Id,
            }
            templatehtml = templatehtml.allReplace(obj);
            return templatehtml;
        },

        CreateTableView12old: function (data, enableHeader, theme, widgetObject) {
            var regex = /([^A-Za-z0-9\.\$])|([A-Z])(?=[A-Z][a-z])|([^\-\$\.0-9])(?=\$?[0-9]+(?:\.[0-9]+)?)|([0-9])(?=[^\.0-9])|([a-z])(?=[A-Z])/g;
            // set optional theme parameter
            if (theme === undefined) {
                theme = 'mediumTable'; //default theme
            }

            if (enableHeader === undefined) {
                enableHeader = true; //default enable headers
            }

            // If the returned data is an object do nothing, else try to parse
            var data = typeof data != 'object' ? JSON.parse(data) : data;
            var i = 0;
            firstItem = data[0];
            var keys = new Array();
            var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
            for (var key in firstItem) {
                if (typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                    continue;
                keys.push({ Index: i, Key: key });
                i++;
            }

            var widgetColumns = widgetObject.WidgetColumns;

            var firstItem = data[0];
            var headerStart = (typeof widgetObject.HeaderStart == "undefined" || widgetObject.HeaderStart == null) ? 0 : widgetObject.HeaderStart;
            var headerEnd = (typeof widgetObject.HeaderEnd == "undefined" || widgetObject.HeaderEnd == null || widgetObject.HeaderEnd == 0) ? keys.length : widgetObject.HeaderEnd > 50 ? keys.length : widgetObject.HeaderEnd;
            var skipHeaders = (typeof widgetObject.SkipHeaders == "undefined" || widgetObject.SkipHeaders == null) ? new Array() : widgetObject.SkipHeaders.split(",");

            skipHeaders = Enumerable.From(skipHeaders).Select(function (x) { return Common.GetInt(x) }).ToArray();
            var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= headerStart && x.Index <= headerEnd && $.inArray(x.Index, skipHeaders) === -1 }).ToArray();
            var selectSelector = "x => {";
            for (var key in requiredKeys) {
                var reqKey = requiredKeys[key];
                selectSelector += "'" + reqKey.Key + "': x['" + reqKey.Key + "'],"
            }
            selectSelector = selectSelector.replace(/,\s*$/, "");
            selectSelector += "}";


            data = Enumerable.From(data).Take(Setting.GraphRecords).Select(selectSelector).ToArray();

            //Enumerable.From(data).Take(Setting.GraphRecords).Select("$." + categoryKey).ToArray()

            var headers = "";
            var body = "";
            var t;
            // table head

            if (enableHeader) {
                headers += '<tr>';
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var index = reqKey.Key;
                    if (Enumerable.From(widgetColumns).Any(function (x) { return x.Name == index }))
                        index = Enumerable.From(widgetColumns).Where(function (x) { return x.Name == index }).FirstOrDefault().HeaderText;
                    else
                        var index = index.replace(regex, '$2$3$4$5 ');

                    headers += '<th >' + index + '</th>';
                }
                headers += '</tr>';
            }

            // table body
            var i = 0;
            for (var j in data) {
                var record = data[j];
                body += (i % 2 == 0) ? '<tr >' : '<tr>';
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var dt = record[reqKey.Key];
                    var dt = dt == null ? "" : dt + "";
                    if (dt != null && dt.indexOf("T00:00:00") > -1)
                        dt = moment(dt).format('DD/MM/YYYY');
                    body += '<td>' + dt + '</td>';

                }
                i++;
                body += '</tr>';
            }
            var templatehtml = "";
            if (widgetObject.Template != null && widgetObject.Template.trim() != "")
                templatehtml = $(widgetObject.Template).html();
            else if (!widgetObject.GraphEnabled) {
                templatehtml = $("#template-default-without-graph").html();
            }
            else {
                templatehtml = $("#template-default").html();
            }
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers,
                '[Name]': widgetObject.Name,
                '[Id]': widgetObject.Id,

            }
            templatehtml = templatehtml.allReplace(obj);
            return templatehtml;
        },
        GetRandomInt: function (min, max) {
            return Math.floor(Math.random() * (max - min + 1)) + min;
        },
        MakeGraph: function (newOptions, widgetObject, data) {


            require.config({
                paths: {
                    //echarts: '../Content/metronic/assets/global/plugins/echarts/'
                    //zrColor: 'zrender/tool/color',
                    echarts: '../Content/metronic/assets/global/plugins/echarts/'
                }
            });



            //var zrColor = require('zrender/tool/color');
            //var colorList = [
            //  '#87cefa', '#da70d6', '#32cd32', '#6495ed', '#ff7f50',
            //  '#ff69b4', '#ba55d3', '#cd5c5c', '#ffa500', '#40e0d0'
            //];
            //var itemStyle = {
            //    normal: {
            //        color: function (params) {
            //            if (params.dataIndex < 0) {
            //                // for legend
            //                return '#6495ed';
            //                return colorList[params.dataIndex];
            //                return zrColor.lift(
            //                  colorList[colorList.length - 1], params.seriesIndex * 0.1
            //                );
            //            }
            //            else {
            //                // for bar
            //                return '#6495ed';
            //                return colorList[params.dataIndex];
            //                return zrColor.lift(
            //                  colorList[params.dataIndex], params.seriesIndex * 0.1
            //                );
            //            }
            //        }
            //    }
            //};




            if (!newOptions) {
                var keys = new Array();
                var values = new Array();

                var i = 0;
                var firstItem = data[0];
                var widgetColumns = widgetObject.WidgetColumns;
                var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
                for (var key in firstItem) {
                    if (typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                        continue;
                    keys.push({ Index: i, Key: key });
                    i++;
                }
                var noOfRecords = Setting.GraphRecords;
                if (data.length == 12) noOfRecords = 12;
                var legendStart = widgetObject.LegendStart;
                var legendEnd = widgetObject.LegendEnd > 50 ? keys.length : widgetObject.LegendEnd;
                var skiplegends = (typeof widgetObject.SkipLegends == "undefined" || widgetObject.SkipLegends == null) ? new Array() : widgetObject.SkipLegends.split(",");
                skiplegends = Enumerable.From(skiplegends).Select(function (x) { return Common.GetInt(x) }).ToArray();
                var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= legendStart && x.Index <= legendEnd && $.inArray(x.Index, skiplegends) === -1 }).ToArray();
                var categoryKey = Enumerable.From(keys).Where(function (x) { return x.Index == widgetObject.CategoryIndex }).FirstOrDefault().Key;
                var categories = Enumerable.From(data).Take(noOfRecords).Select("$." + categoryKey).ToArray();
                categories = Enumerable.From(categories).Where(function (x) { return x != null }).ToArray();
                var legends = Enumerable.From(requiredKeys).Select("$.Key").ToArray();
                data = Enumerable.From(data).Take(noOfRecords).ToArray();
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var name = reqKey.Key;
                    if (Enumerable.From(widgetColumns).Any(function (x) { return x.Name == reqKey.Key }))
                        legends[reqKey.Key] = name = Enumerable.From(widgetColumns).Where(function (x) { return x.Name == reqKey.Key }).FirstOrDefault().HeaderText;

                    values.push(
                        {
                            name: name,
                            type: 'bar',
                            //itemStyle: itemStyle,
                            itemStyle: {
                                normal: {
                                    backgroundColor: '#6495ed',
                                },
                            },
                            data: Enumerable.From(data).Select(function (x) { return x['' + reqKey.Key + ''] }).ToArray()

                        });
                }
                for (var i = 0; i < legends.length; i++) {
                    if (Enumerable.From(widgetColumns).Any(function (x) { return x.Name == legends[i] })) {
                        legends[i] = Enumerable.From(widgetColumns).Where(function (x) { return x.Name == legends[i] }).FirstOrDefault().HeaderText;
                    }
                }
                if ((legends == null || legends.length <= 0) || (categories == null || categories.length <= 0))
                    return;
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
            'echarts/chart/wordCloud',
            'zrender/tool/color'

                ],
                function (ec) {
                    //--- BAR ---
                    var myChart = ec.init(document.getElementById('echarts_bar_' + widgetObject.Id));
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
                    //var options = {
                    //    tooltip: {
                    //        show: true
                    //    },
                    //    legend: {
                    //        data: ['Sales']
                    //    },
                    //    xAxis: [
                    //        {
                    //            type: 'category',
                    //            data: ["Shirts", "Sweaters", "Chiffon Shirts", "Pants", "High Heels", "Socks"]
                    //        }
                    //    ],
                    //    yAxis: [
                    //        {
                    //            type: 'value'
                    //        }
                    //    ],
                    //    series: [
                    //        {
                    //            "name": "Sales",
                    //            "type": "bar",
                    //            "data": [5, 20, 40, 10, 10, 20]
                    //        }
                    //    ]
                    //};

                    myChart.setOption(options);


                }
            );
        },
        MakePieChart: function (newOptions, reportObject, data) {
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
                var reportColumns = reportObject.WidgetColumns;
                var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
                for (var key in firstItem) {
                    if (typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                        continue;
                    keys.push({ Index: i, Key: key });
                    i++;
                }
                var noOfRecords = Setting.GraphRecords;
                if (data.length == 12) noOfRecords = 12;
                var legendStart = reportObject.LegendStart;
                var legendEnd = reportObject.LegendEnd > 50 ? keys.length : reportObject.LegendEnd;
                var skiplegends = (typeof reportObject.SkipLegends == "undefined" || reportObject.SkipLegends == null || reportObject.SkipLegends == "") ? new Array() : reportObject.SkipLegends.split(",");
                skiplegends = Enumerable.From(skiplegends).Select(function (x) { return Common.GetInt(x) }).ToArray();
                var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= legendStart && x.Index <= legendEnd && $.inArray(x.Index, skiplegends) === -1 }).ToArray();
                var categoryKey = Enumerable.From(keys).Where(function (x) { return x.Index == reportObject.CategoryIndex }).FirstOrDefault().Key;
                var categories = Enumerable.From(data).Take(noOfRecords).Select("$." + categoryKey).ToArray()
                var legends = Enumerable.From(requiredKeys).Select("$.Key").ToArray();
                data = Enumerable.From(data).Take(noOfRecords).ToArray();
                for (var key in categories) {

                    var token = Enumerable.From(data).Where(function (x) { return x['' + categoryKey + ''] == categories[key] }).Select(function (p) { return p['' + requiredKeys[0].Key + ''] }).FirstOrDefault();
                    values.push(
                        {

                            value: token,
                            name: categories[key],

                        });
                }
                for (var i = 0; i < legends.length; i++) {
                    if (Enumerable.From(reportColumns).Any(function (x) { return x.Name == legends[i] })) {
                        legends[i] = Enumerable.From(reportColumns).Where(function (x) { return x.Name == legends[i] }).FirstOrDefault().HeaderText;
                    }
                }
                newOptions =
                                       {

                                           legend: {
                                               data: categories
                                           },
                                           series: [{ data: values }]
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
    var myChart = ec.init(document.getElementById('echarts_bar_' + reportObject.Id));
    var options = {
        title: {
            text: reportObject.Name,
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b} : {c} ({d}%)"
        },
        legend: {
            orient: 'vertical',
            x: 'left',
            data: []
        },
        toolbox: {
            show: true,
            feature: {
                mark: { show: false },
                dataView: { show: false, readOnly: false },
                magicType: {
                    show: true,
                    type: ['pie', 'funnel'],
                    option: {
                        funnel: {
                            x: '25%',
                            width: '50%',
                            funnelAlign: 'left',
                            max: 1548
                        }
                    }
                },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        calculable: true,
        series: [
            {
                name: reportObject.Name,
                type: 'pie',
                radius: '' + reportObject.Radius + '%',
                center: ['' + reportObject.Right + '%', '' + reportObject.Top + '%'],
                data: [

                ]
            }
        ]
    };
    if (typeof newOptions != "undefined")
        $.extend(true, options, newOptions);
    myChart.setOption(options);


}
            );
        },

        GetUserSaleByDivision: function (records, widget) {
            var $this = this;
            var html = "";
            var years = Enumerable.From(records).Select("$.Year").Distinct().OrderBy("$").ToArray();
            var divisions = Enumerable.From(records).Select("$.Division").Distinct().ToArray();
            var headers = '';
            headers += '<tr>';
            var body = '';
            var headercounter = 1;
            var categories = new Array();
            var values = new Array();
            var legends = new Array();
            for (var i in divisions) {
                var divisionValues = new Array();
                if (headercounter == 1)
                    headers += '<th >Division</th>';
                body += '<tr>';
                var division = divisions[i];
                body += '<td>' + division + '</td>';
                categories.push(division);
                for (var j in years) {
                    var year = years[j];
                    var value = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Division == division && x.Year == year }).Sum("$.Value"));
                    if (headercounter == 1) {
                        headers += '<th>' + year + '</th>';
                        legends.push(year + "");
                    }

                    body += '<td>' + value.format() + '</td>';

                }


                headercounter++;
                body += '</tr>';
            }




            var templatehtml = "";
            if (widget.Template != null && widget.Template.trim() != "")
                templatehtml = $(widget.Template).html();
            else if (!widget.GraphEnabled) {
                templatehtml = $("#template-default-without-graph").html();
            }
            else
                templatehtml = $("#template-default").html();
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers,
                '[Name]': widget.Name,
                '[TableFooter]': '',
                '[Id]': widget.Id,
            }
            templatehtml = templatehtml.allReplace(obj);
            var html = templatehtml;
            $("#widget-container").append(html);
            if (records.length > 0 && widget.GraphEnabled) {


                for (var j in years) {
                    var year = years[j];
                    var divisionValues = new Array();
                    for (var i in divisions) {
                        var division = divisions[i];
                        var value = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Division == division && x.Year == year }).Sum("$.Value"))
                        divisionValues.push(value)
                    }
                    values.push(
              {
                  name: year,
                  type: 'bar',
                  data: divisionValues

              });
                }


                var options =
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






                var type = widget.GraphType;
                switch (type) {
                    case GraphType.PieChart:
                        XDashboard.MakePieChart(null, widget, records);
                        break;
                    default:
                        XDashboard.MakeGraph(options, widget, records);
                        break;

                }
            }

        },

        GetUserSaleByRegion: function (records, widget) {
            var $this = this;
            var html = "";
            var years = Enumerable.From(records).Select("$.Year").Distinct().OrderBy("$").ToArray();
            var regions = Enumerable.From(records).Select("$.Region").Distinct().ToArray();
            var headers = '';
            headers += '<tr>';
            var body = '';
            var headercounter = 1;
            var categories = new Array();
            var values = new Array();
            var legends = new Array();
            for (var i in regions) {
                var divisionValues = new Array();
                if (headercounter == 1)
                    headers += '<th>Region</th>';
                body += '<tr>';
                var region = regions[i];
                body += '<td>' + region + '</td>';
                categories.push(region);
                for (var j in years) {
                    var year = years[j];
                    var value = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Region == region && x.Year == year }).Sum("$.Value"));
                    if (headercounter == 1) {
                        headers += '<th>' + year + '</th>';
                        legends.push(year + "");
                    }

                    body += '<td>' + value.format() + '</td>';

                }


                headercounter++;
                body += '</tr>';
            }




            var templatehtml = "";
            if (widget.Template != null && widget.Template.trim() != "")
                templatehtml = $(widget.Template).html();
            else if (!widget.GraphEnabled) {
                templatehtml = $("#template-default-without-graph").html();
            }
            else
                templatehtml = $("#template-default").html();
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers,
                '[Name]': widget.Name,
                '[TableFooter]': '',
                '[Id]': widget.Id,
            }
            templatehtml = templatehtml.allReplace(obj);
            var html = templatehtml;
            $("#widget-container").append(html);
            if (records.length > 0 && widget.GraphEnabled) {


                for (var j in years) {
                    var year = years[j];
                    var divisionValues = new Array();
                    for (var i in regions) {
                        var region = regions[i];
                        var value = Common.GetInt(Enumerable.From(records).Where(function (x) { return x.Region == region && x.Year == year }).Sum("$.Value"))
                        divisionValues.push(value)
                    }
                    values.push(
              {
                  name: year,
                  type: 'bar',
                  data: divisionValues

              });
                }


                var options =
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






                var type = widget.GraphType;
                switch (type) {
                    case GraphType.PieChart:
                        XDashboard.MakePieChart(null, widget, records);
                        break;
                    default:
                        XDashboard.MakeGraph(options, widget, records);
                        break;

                }
            }

        },

        FormatNumber: function (number, type, precision) {
            if (typeof precision == 'undefined' || precision == null || precision == 0)
                precision = 2;
            switch (type) {
                case FormatingType.Number:
                    return Common.GetInt(number);
                    break;
                case FormatingType.Float:
                    return Common.GetFloat(number).toFixed(precision);
                    break;
                case FormatingType.NumberWithThousandSeprator:
                    return Common.GetInt(number).format();
                    break;
                case FormatingType.FloatWithThousandSeprator:
                    return Common.GetFloat(number).toFixed(precision).format();
                    break;
                case FormatingType.ThousandSeprator:
                    return number.format();
                    break;
                default:
                    return number;
                    break;
            }
        },
        ChangeDate: function (date) {
            var dt = moment(date).format('DD/MM/YYYY');
            if (dt == "Invalid date") dt = "";
            return dt;
        },
        BindCustomerSelect: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Customers);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        BindSalesmanSelect: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Salesman);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        BindSupplierSelect: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        BindProductSelect: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Products);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        GetFeedBackPoints: function (feedback) {
            return 2.5;
        },
        GetSelectHTML: function (accounts) {
            var $this = this;
            var html = "";
            html += "<option></option>";
            for (var i = 0; i < accounts.length; i++) {
                var account = accounts[i];
                html += "<option value='" + account.Id + "'>" + account.AccountCode + "-" + account.Name + "</option>";
            }
            return html;
        },
        GetLocalStorageKey: function () {
            var $this = this;
            var dashboardName = Common.GetQueryStringValue("name");
            var lsKey = "";
            if (!Common.isNullOrWhiteSpace(dashboardName))
                lsKey = 'Dashboard_' + dashboardName + "_User_" + PageSetting.UserId + "_FiscalId_" + Common.Fiscal.Id + "_LastLoadedAt";
            else
                lsKey = 'Dashboard_' + PageSetting.DashBaordId + "_User_" + PageSetting.UserId + "_FiscalId_" + Common.Fiscal.Id + "_LastLoadedAt";

            return lsKey;
        },
        GetLocalStorageKeyForWidget: function (widgetName) {
            var $this = this;
            var dashboardName = Common.GetQueryStringValue("name");
            var lsKey = $this.GetLocalStorageKey();
            lsKey = lsKey + "_Widget_" + widgetName
            return lsKey;
        }
    }
}();



//function EncodeControlsToAscii() {
//    var elements = $('input[type=text], input[type=password]');

//    for (i = 0; i < elements.length; i++) {
//        var element = elements[i];
//        var string = $(element).val(),
//          substring1 = "<",
//          substring2 = "&#";
//        if (string.indexOf(substring1) > -1 || string.indexOf(substring2) > -1) {
//            return false;
//            break;
//        }

//    }
//}
//OnClientClick = "return EncodeControlsToAscii();"