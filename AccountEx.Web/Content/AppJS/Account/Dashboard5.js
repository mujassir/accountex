
var Dashboard5 = function () {
    var API_CONTROLLER = "Dashboard";
    return {
        init: function () {
           var $this = this;
            App.addResizeHandler(function () {
                jQuery(".vmaps").each(function () {
                    var map = jQuery(this);
                    map.width(map.parent().width());
                });
            });
        },
        initDashboardDaterange: function () {
            $("#dashboard-report-range").daterangepicker({
                opens: (App.isRTL() ? "right" : "left"),
                startDate: moment().subtract("days", 6),
                endDate: moment(),
                minDate: "01/01/2012",
                maxDate: "12/31/2014",
                dateLimit: {
                    days: 60
                },
                showDropdowns: false,
                showWeekNumbers: true,
                timePicker: false,
                timePickerIncrement: 1,
                timePicker12Hour: true,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract("days", 1), moment().subtract("days", 1)],
                    'Last 7 Days': [moment().subtract("days", 6), moment()],
                    'Last 30 Days': [moment().subtract("days", 29), moment()],
                    'This Month': [moment().startOf("month"), moment().endOf("month")],
                    'Last Month': [moment().subtract("month", 1).startOf("month"), moment().subtract("month", 1).endOf("month")]
                },
                buttonClasses: ["btn btn-sm"],
                applyClass: " blue",
                cancelClass: "default",
                format: "DD/MM/YYYY",
                separator: " to ",
                locale: {
                    applyLabel: "Apply",
                    fromLabel: "From",
                    toLabel: "To",
                    customRangeLabel: "Custom Range",
                    daysOfWeek: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
                    monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                    firstDay: 1
                }
            },
                function (start, end) {
                    $("#dashboard-report-range span").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));

                   
                }
            );


            $("#dashboard-report-range span").html(moment().subtract("days", 6).format("MMMM D, YYYY") + " - " + moment().format("MMMM D, YYYY"));
            $("#dashboard-report-range").show();
        },
        initCharts: function () {
            if (!jQuery.plot) {
                return;
            }

            function showChartTooltip(x, y, xValue, yValue) {
                $("<div id=\"tooltip\" class=\"chart-tooltip\">" + yValue + "<\/div>").css({
                    position: "absolute",
                    display: "none",
                    top: y - 40,
                    left: x - 40,
                    border: "0px solid #ccc",
                    padding: "2px 6px",
                    'background-color': "#fff"
                }).appendTo("body").fadeIn(200);
            }

            var data = [];
            var totalPoints = 250;

            // random data generator for plot charts

            function getRandomData() {
                if (data.length > 0) data = data.slice(1);
                // do a random walk
                while (data.length < totalPoints) {
                    var prev = data.length > 0 ? data[data.length - 1] : 50;
                    var y = prev + Math.random() * 10 - 5;
                    if (y < 0) y = 0;
                    if (y > 100) y = 100;
                    data.push(y);
                }
                // zip the generated y values with the x values
                var res = [];
                for (var i = 0; i < data.length; ++i) res.push([i, data[i]]);
                return res;
            }

            function randValue() {
                return (Math.floor(Math.random() * (1 + 50 - 20))) + 10;
            }

            var visitors = [
                ["02/2013", 1500],
                ["03/2013", 2500],
                ["04/2013", 1700],
                ["05/2013", 800],
                ["06/2013", 1500],
                ["07/2013", 2350],
                ["08/2013", 1500],
                ["09/2013", 1300],
                ["10/2013", 4600]
            ];


            if ($("#site_statistics").size() != 0) {

                $("#site_statistics_loading").hide();
                $("#site_statistics_content").show();

                var plotStatistics = $.plot($("#site_statistics"),
                    [{
                        data: visitors,
                        lines: {
                            fill: 0.6,
                            lineWidth: 0
                        },
                        color: ["#f89f9f"]
                    }, {
                        data: visitors,
                        points: {
                            show: true,
                            fill: true,
                            radius: 5,
                            fillColor: "#f89f9f",
                            lineWidth: 3
                        },
                        color: "#fff",
                        shadowSize: 0
                    }],

                    {
                        xaxis: {
                            tickLength: 0,
                            tickDecimals: 0,
                            mode: "categories",
                            min: 0,
                            font: {
                                lineHeight: 14,
                                style: "normal",
                                variant: "small-caps",
                                color: "#6F7B8A"
                            }
                        },
                        yaxis: {
                            ticks: 5,
                            tickDecimals: 0,
                            tickColor: "#eee",
                            font: {
                                lineHeight: 14,
                                style: "normal",
                                variant: "small-caps",
                                color: "#6F7B8A"
                            }
                        },
                        grid: {
                            hoverable: true,
                            clickable: true,
                            tickColor: "#eee",
                            borderColor: "#eee",
                            borderWidth: 1
                        }
                    });

                var previousPoint = null;
                $("#site_statistics").bind("plothover", function (event, pos, item) {
                    $("#x").text(pos.x.toFixed(2));
                    $("#y").text(pos.y.toFixed(2));
                    if (item) {
                        if (previousPoint != item.dataIndex) {
                            previousPoint = item.dataIndex;

                            $("#tooltip").remove();
                            var x = item.datapoint[0].toFixed(2),
                                y = item.datapoint[1].toFixed(2);

                            showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1] + " visits");
                        }
                    } else {
                        $("#tooltip").remove();
                        previousPoint = null;
                    }
                });
            }


            if ($("#site_activities").size() != 0) {
                //site activities
                var previousPoint2 = null;
                $("#site_activities_loading").hide();
                $("#site_activities_content").show();

                var data1 = [
                    ["DEC", 300],
                    ["JAN", 600],
                    ["FEB", 1100],
                    ["MAR", 1200],
                    ["APR", 860],
                    ["MAY", 1200],
                    ["JUN", 1450],
                    ["JUL", 1800],
                    ["AUG", 1200],
                    ["SEP", 600]
                ];


                var plotStatistics = $.plot($("#site_activities"),

                    [{
                        data: data1,
                        lines: {
                            fill: 0.2,
                            lineWidth: 0,
                        },
                        color: ["#BAD9F5"]
                    }, {
                        data: data1,
                        points: {
                            show: true,
                            fill: true,
                            radius: 4,
                            fillColor: "#9ACAE6",
                            lineWidth: 2
                        },
                        color: "#9ACAE6",
                        shadowSize: 1
                    }, {
                        data: data1,
                        lines: {
                            show: true,
                            fill: false,
                            lineWidth: 3
                        },
                        color: "#9ACAE6",
                        shadowSize: 0
                    }],

                    {

                        xaxis: {
                            tickLength: 0,
                            tickDecimals: 0,
                            mode: "categories",
                            min: 0,
                            font: {
                                lineHeight: 18,
                                style: "normal",
                                variant: "small-caps",
                                color: "#6F7B8A"
                            }
                        },
                        yaxis: {
                            ticks: 5,
                            tickDecimals: 0,
                            tickColor: "#eee",
                            font: {
                                lineHeight: 14,
                                style: "normal",
                                variant: "small-caps",
                                color: "#6F7B8A"
                            }
                        },
                        grid: {
                            hoverable: true,
                            clickable: true,
                            tickColor: "#eee",
                            borderColor: "#eee",
                            borderWidth: 1
                        }
                    });

                $("#site_activities").bind("plothover", function (event, pos, item) {
                    $("#x").text(pos.x.toFixed(2));
                    $("#y").text(pos.y.toFixed(2));
                    if (item) {
                        if (previousPoint2 != item.dataIndex) {
                            previousPoint2 = item.dataIndex;
                            $("#tooltip").remove();
                            var x = item.datapoint[0].toFixed(2),
                                y = item.datapoint[1].toFixed(2);
                            showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1] + "M$");
                        }
                    }
                });

                $("#site_activities").bind("mouseleave", function () {
                    $("#tooltip").remove();
                });
            }
        },
        RunReport: function (el, id) {
            $("#Id").val(id);
            $("#tblReportData").html("");
            $(".page-title span").text($(el).parents("tr").find("a.report-name").text());
            var json = $(el).parents("tr").find("span.report-object").html();
            ReportObject = JSON.parse(json);
            var params = ReportObject.ReportParameters;
            var html = "";
            if (!ReportObject.DirectRun) {
                for (var i = 0; i < params.length; i++) {

                    var rptParam = params[i];
                    if (!rptParam.IsVisible)
                        continue;
                    html += '<div class="form-group">';
                    html += '<label class="col-md-2 control-label">' + rptParam.DisplayLabel + '</label>';
                    html += '<div class="col-md-3">';
                    if (rptParam.ControlType == "text") {
                        if (rptParam.Type == "Date" || rptParam.Type == "DateTime") {
                            var date = moment(new Date());
                            if (rptParam.Name.indexOf("FromDate") > -1)
                                date = new Date(new Date().getFullYear(), 6, 1);
                            date = moment(date).format("DD/MM/YYYY");
                            html += '<div class="input-group input-medium date  date-picker">';
                            html += '<input type="text" class="form-control date-mask" value="' + date + '" id="' + rptParam.Name + '" data-required="required" data-message="' + rptParam.DisplayLabel + ' is required">';
                            html += '<span class="input-group-btn"><button class="btn default" type="button"><i class="fa fa-calendar"></i></button></span></div>';
                        }
                        else
                            html += '<input type="text" class="form-control ' + rptParam.InplutCssClass + '" id="' + rptParam.Name + '" data-required="required" data-message="' + rptParam.Name + ' is required">';
                    }
                    else if (rptParam.ControlType = "select") {
                        html += '<select class="form-control select2" id="' + rptParam.Name + '" data-required="required" data-message="' + rptParam.DisplayLabel + ' is required" placeholder="select ' + rptParam.DisplayLabel + '">' + eval("Report." + rptParam.JSFucntion)() + '</select>';
                    }

                    html += '</div>';
                    html += '</div>';
                }
                $("#ReportParams").html(html);
            }

            if (ReportObject.DirectRun) {
                $("#report-conatiner,#ReportParams").html("");
                $('#div-table').addClass('hide');
                $('#div-report').removeClass('hide');
                $('#div-report .actions').removeClass('hide');
                Report.LoadReport();

            }
            else {
                $("#report-conatiner").html("");
                $('#div-form,#div-report').removeClass('hide');
                $('#div-report .actions').addClass('hide');
                $('#div-table').addClass('hide');
                Common.BindDatePicker(".date-picker");
                $(".date-mask").inputmask("d/m/y", {
                    "placeholder": "dd/mm/yyyy"
                });
                $("select.select2").select2();
            }


        },
        LoadReport: function (callback) {
            var _this = this;
            if (Common.Validate($("#div-form"))) {
                var parameters = Common.SetValue("#div-form");
                var params = new Array();
                for (var p in parameters) {
                    params.push({
                        Name: p,
                        Value: parameters[p],
                    });
                }
                var record = {
                    Id: $("#Id").val(),
                    Parameters: params,
                };
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    success: function (res) {
                        $("#info-message").addClass("hide");

                        if (res.Success) {
                            var html = "";

                            if (ReportObject.JsFunction != undefined && ReportObject.JsFunction != null
                                && ReportObject.JsFunction != "null" && ReportObject.JsFunction != "") {
                                html = eval("Report." + ReportObject.JsFunction)(res.Data, ReportObject);
                            }
                            else {
                                html = _this.CreateTableView(res.Data, true, null, ReportObject)
                                $("#report-conatiner").html(html);
                                if (res.Data.length > 0 && ReportObject.GraphEnabled)
                                    Report.MakeGraph(null, ReportObject, res.Data);
                                //$("a#full-screen").click();
                            }
                            if (res.Data.length == 0) {
                                $("#info-message").removeClass("hide");
                            }
                            if (!ReportObject.GraphEnabled) {
                                $("#report-conatiner .graph-container").remove();
                                $("#report-conatiner .data-container").removeClass("col-md-6").addClass("col-md-12")
                            }



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
        CreateTableView: function (data, enableHeader, theme, reportObject) {
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

            var reportColumns = reportObject.ReportColumns;

            var firstItem = data[0];
            var headerStart = (typeof reportObject.HeaderStart == "undefined" || reportObject.HeaderStart == null) ? 0 : reportObject.HeaderStart;
            var headerEnd = (typeof reportObject.HeaderEnd == "undefined" || reportObject.HeaderEnd == null || reportObject.HeaderEnd == 0) ? keys.length : reportObject.HeaderEnd > 50 ? keys.length : reportObject.HeaderEnd;
            var skipHeaders = (typeof reportObject.SkipHeaders == "undefined" || reportObject.SkipHeaders == null) ? new Array() : reportObject.SkipHeaders.split(",");

            skipHeaders = Enumerable.From(skipHeaders).Select(function (x) { return Common.GetInt(x) }).ToArray();
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
                    var dt = record[reqKey.Key];
                    var dt = dt == null ? "" : dt + "";
                    if (dt != null && dt.indexOf("T00:00:00") > -1)
                        dt = moment(dt).format('DD/MM/YYYY');
                    body += '<td>' + dt + '</td>';

                }
                i++;
                body += '</tr>';
            }
            var templatehtml = $("#template-default").html();
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers
            }
            templatehtml = templatehtml.allReplace(obj);
            return templatehtml;
        },
        MakeGraph: function (newOptions, reportObject, data) {
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
                var reportColumns = reportObject.ReportColumns;
                var tempSkipHeader = new Array("1", "2", "3", "4", "5", "6", "7");
                for (var key in firstItem) {
                    if (typeof key == "undefined" || key == null || key.trim() == "" || $.inArray(key.trim(), tempSkipHeader) != -1)
                        continue;
                    keys.push({ Index: i, Key: key });
                    i++;
                }
                var legendStart = reportObject.LegendStart;
                var legendEnd = reportObject.LegendEnd > 50 ? keys.length : reportObject.LegendEnd;
                var skiplegends = (typeof reportObject.SkipLegends == "undefined" || reportObject.SkipLegends == null) ? new Array() : reportObject.SkipLegends.split(",");
                skiplegends = Enumerable.From(skiplegends).Select(function (x) { return Common.GetInt(x) }).ToArray();
                var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= legendStart && x.Index <= legendEnd && $.inArray(x.Index, skiplegends) === -1 }).ToArray();
                var categoryKey = Enumerable.From(keys).Where(function (x) { return x.Index == reportObject.CategoryIndex }).FirstOrDefault().Key;
                var categories = Enumerable.From(data).Take(Setting.GraphRecords).Select("$." + categoryKey).ToArray()
                var legends = Enumerable.From(requiredKeys).Select("$.Key").ToArray();
                data = Enumerable.From(data).Take(Setting.GraphRecords).ToArray();
                for (var key in requiredKeys) {
                    var reqKey = requiredKeys[key];
                    var name = reqKey.Key;
                    if (Enumerable.From(reportColumns).Any(function (x) { return x.Name == reqKey.Key }))
                        legends[reqKey.Key] = name = Enumerable.From(reportColumns).Where(function (x) { return x.Name == reqKey.Key }).FirstOrDefault().HeaderText;

                    values.push(
                        {
                            name: name,
                            type: 'bar',
                            data: Enumerable.From(data).Select(function (x) { return x['' + reqKey.Key + ''] }).ToArray()

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
         //boundaryGap: true,
         //axisLine: {   
         //    show: true,
         //    lineStyle: {
         //        color: 'green',
         //        type: 'solid',
         //        width: 2
         //    }
         //},
         //axisTick: {    // 轴标记
         //    show: true,
         //    length: 10,
         //    lineStyle: {
         //        color: 'red',
         //        type: 'solid',
         //        width: 2
         //    }
         //},
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
    };
}();