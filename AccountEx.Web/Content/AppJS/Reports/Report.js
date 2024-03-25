var Report = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Report";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var PageSetting = new Object();
    var ReportObject;
    return {
        init: function () {
            var _this = this;
            _this.LoadPageSetting();

            $("#btnShowReport").click(function () {
                _this.LoadReport();
            });
            $("#btnCancel,#actionBackList,#linkBackList,.actionBackList").click(function () {
                _this.ListView();
            });
            $("#btnAdd").click(function () {
                _this.Add();
            });
            if (typeof Url.Name != undefined && Url.Name != null) {
                // _this.GetReportByName();
                if (PageSetting.Report != null)
                    _this.RunReport(null, PageSetting.Report.Id, PageSetting.Report);
                else
                    Common.ShowError("No report found.please contact admin.");
            }
            else {
                _this.ListView();
            }

        },
        GetReportByName: function () {
            var $this = this;

            var name = Url.Name;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetReportByName&name=" + name,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $this.RunReport(null, res.Data.Id, res.Data);

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        Get: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=load",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading report ...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data != null)
                            $this.RunReport(null, res.Data.Id, res.Data);
                        else
                            Common.ShowError("No report found.please contact admin.");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        RunReport: function (el, id, data) {
            $("#Id").val(id);
            $("#tblReportData").html("");
            if (typeof el === "undefined" || el == null)
                ReportObject = data;
            else {
                var json = $(el).parents("tr").find("span.report-object").html();
                ReportObject = JSON.parse(json);

            }
            var params = ReportObject.ReportParameters;
            $(".page-title span").text(ReportObject.Name);
            $(document).prop('title', ReportObject.Name + " | " + PageSetting.ApplicationTitle);
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
                            if (rptParam.Name.indexOf("FromDate") > -1 || rptParam.Name.indexOf("@StartDate") > -1 || rptParam.AdvanceType == 3)
                                date = Common.Fiscal.FromDate;
                            else if (rptParam.Name.indexOf("ToDate") > -1 || rptParam.Name.indexOf("@EndDate") > -1 || rptParam.AdvanceType == 5)
                                date = Common.Fiscal.ToDate;
                            date = moment(date).format("DD/MM/YYYY");
                            //html += '<div class="input-group input-medium date  date-picker">';
                            html += '<input type="text" class="form-control date-mask date-picker" value="' + date + '" id="' + rptParam.Name + '" data-fiscal-date="true" data-required="required" data-message="' + rptParam.DisplayLabel + ' is required">';
                            // html += '<span class="input-group-btn"><button class="btn default" type="button"><i class="fa fa-calendar"></i></button></span></div>';
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
                Common.InitDateMask();
                Common.InitDatePicker();
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
                $('#div-report .actions').removeClass('hide');
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
                                if (res.Data.length > 0 && ReportObject.GraphEnabled && PageSetting.AllowDynamicReportGraph) {
                                    var type = ReportObject.GraphType;
                                    switch (type) {
                                        case GraphType.PieChart:
                                            Report.MakePieChart(null, ReportObject, res.Data);
                                            break;
                                        default:
                                            Report.MakeGraph(null, ReportObject, res.Data);
                                            break;

                                    }

                                }
                                //$("a#full-screen").click();
                            }
                            if (res.Data.length == 0) {
                                $("#info-message").removeClass("hide");
                            }
                            if (!ReportObject.GraphEnabled || !PageSetting.AllowDynamicReportGraph) {
                                $("#report-conatiner .graph-container").remove();
                                $("#report-conatiner .data-container").removeClass("col-md-6").addClass("col-xs-12").removeClass("col-xs-6").addClass("col-xs-12")
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
                            token = Report.FormatNumber(token, mapping.BodyFormatingType, mapping.Precision);


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
                                footerText = Report.FormatNumber(footerText, mapping.FooterFormatingType, mapping.Precision);

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
            else
                templatehtml = $("#template-default").html();
            var obj = {
                '[TableBody]': body,
                '[TableHead]': headers,
                '[TableFooter]': tfoot
            }
            templatehtml = templatehtml.allReplace(obj);
            return templatehtml;
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
                var noOfRecords = Setting.GraphRecords;
                if (data.length == 12) noOfRecords = 12;
                var legendStart = reportObject.LegendStart;
                var legendEnd = reportObject.LegendEnd > 50 ? keys.length : reportObject.LegendEnd;
                var skiplegends = (typeof reportObject.SkipLegends == "undefined" || reportObject.SkipLegends == null) ? new Array() : reportObject.SkipLegends.split(",");
                skiplegends = Enumerable.From(skiplegends).Select(function (x) { return Common.GetInt(x) }).ToArray();
                var requiredKeys = Enumerable.From(keys).Where(function (x) { return x.Index >= legendStart && x.Index <= legendEnd && $.inArray(x.Index, skiplegends) === -1 }).ToArray();
                var categoryKey = Enumerable.From(keys).Where(function (x) { return x.Index == reportObject.CategoryIndex }).FirstOrDefault().Key;
                var categories = Enumerable.From(data).Take(noOfRecords).Select("$." + categoryKey).ToArray()
                var legends = Enumerable.From(requiredKeys).Select("$.Key").ToArray();
                data = Enumerable.From(data).Take(noOfRecords).ToArray();
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
                var reportColumns = reportObject.ReportColumns;
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
                var skiplegends = (typeof reportObject.SkipLegends == "undefined" || reportObject.SkipLegends == null) ? new Array() : reportObject.SkipLegends.split(",");
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
    var myChart = ec.init(document.getElementById('echarts_bar'));
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
        JobInvoiceSummary: function (records) {
            var _this = Report;
            var html = '<thead> <tr>';
            html += '<th>Job No</th>';
            html += '<th>Job Date</th>';
            html += '<th>Inv. No</th>';
            html += '<th>Inv. Date</th>';
            html += '<th>Receivable</th>';
            html += '<th>Payable</th>';
            html += '<th>Custom</th>';
            html += '<th>Assessable Value</th>';
            html += '<th>Customer</th>';
            html += '</tr>  </thead>';
            html += '<tbody>';
            var customerName = "";
            var customerId = -1;
            var totalPayables = 0;
            var totalReceiveables = 0;
            var totalCustomDuty = 0;
            var totalAV = 0;
            for (var i = 0; i < records.length; i++) {
                var dt = records[i];
                if (customerId != dt.CustomerId) {
                    if (customerId != -1) {
                        html += "<tr class='bold'>";
                        html += "<td colspan='4'>Total for " + dt.CustomerName + "</td>";
                        html += "<td>" + totalPayables + "</td>";
                        html += "<td>" + totalReceiveables + "</td>";
                        html += "<td>" + totalCustomDuty + "</td>";
                        html += "<td>" + totalAV + "</td>";
                        html += "<td></td>";
                        html += "</tr>";
                    }
                    customerName = dt.CustomerName;
                    customerId = dt.CustomerId;
                    html += "<tr class='bold'>";
                    html += "<td colspan='9'>Customer: " + dt.CustomerName + "</td>";
                    html += "</tr>";
                    totalPayables = 0;
                    totalReceiveables = 0;
                    totalCustomDuty = 0;
                    totalAV = 0;
                }

                html += "<tr>";
                html += "<td>" + dt.JobNo + "</td>";
                html += "<td>" + _this.ChangeDate(dt.JobDate) + "</td>";

                html += "<td>" + (dt.InvoiceNumber != null ? dt.InvoiceNumber : "") + "</td>";
                html += "<td>" + _this.ChangeDate(dt.InvoiceDate) + "</td>";
                html += "<td>" + (dt.Payables != null ? dt.Payables : "") + "</td>";
                html += "<td>" + (dt.Receiveables != null ? dt.Receiveables : "") + "</td>";
                html += "<td>" + (dt.CustomDuty != null ? dt.CustomDuty : "") + "</td>";
                html += "<td>" + (dt.AssessableValue != null ? dt.AssessableValue : "") + "</td>";
                html += "<td>" + (dt.GoodsDescription != null ? dt.GoodsDescription : "") + "</td>";
                html += "</tr>";
                totalPayables += dt.Payables;
                totalReceiveables += dt.Receiveables;
                totalCustomDuty += dt.CustomDuty;
                totalAV += dt.AssessableValue;
            }
            if (customerId != -1) {
                html += "<tr class='bold'>";
                html += "<td colspan='4'>Total for " + customerName + "</td>";
                html += "<td>" + totalPayables + "</td>";
                html += "<td>" + totalReceiveables + "</td>";
                html += "<td>" + totalCustomDuty + "</td>";
                html += "<td>" + totalAV + "</td>";
                html += "<td></td>";
                html += "</tr>";
            }
            if (records == null || records.length == 0)
                html += "<tr ><td colspan='9' class='align-center'>No record(s) found</td></tr>";
            html += '</tbody>';
            return html;
        },
        GetBalances: function (records) {
            var html = "";
            var transactions = records;
            var categories = new Array();
            var values = new Array();
            for (var i in transactions) {
                var transaction = transactions[i];
                html += "<tr><td>" + transaction.Code + "-" + transaction.Name + "</td>";
                html += "<td>" + transaction.Balance.format() + "</td></tr>";
                categories.push(transaction.Name);
                values.push(transaction.Balance);
            }
            var templatehtml = $("#template-account-balances").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
            var options =
                      {

                          legend: {
                              data: ['Balance']
                          },
                          xAxis: [{
                              type: 'category',
                              data: categories
                          }],
                          yAxis: [{
                              type: 'value',
                              splitArea: {
                                  show: true
                              }
                          }],
                          series: [{
                              name: 'Balance',
                              type: 'bar',
                              data: values
                          }]
                      }

            Report.MakeGraph(options);
        },
        GetSalesByVoucherType: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.Name", null,
                    function (key, g) {
                        var result = {
                            Name: key,
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            var categories = new Array();
            var values = new Array();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Total.format() + "</td></tr>";
                categories.push(transaction.Name);
                values.push(transaction.Total);

            }
            var templatehtml = $("#template-customer-sales").html();
            var obj = {
                '[TableBody]': html,
                "[Balance]": transaction.Balance
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);

            var options =
                       {

                           legend: {
                               data: ['Sales']
                           },
                           xAxis: [{
                               type: 'category',
                               data: categories
                           }],
                           yAxis: [{
                               type: 'value',
                               splitArea: {
                                   show: true
                               }
                           }],
                           series: [{
                               name: 'Sales',
                               type: 'bar',
                               data: values
                           }]
                       }

            Report.MakeGraph(options);
        },
        GetPurchaseByVoucherType: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.Name", null,
                    function (key, g) {
                        var result = {
                            Name: key,
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Total.format() + "</td></tr>";

            }
            var templatehtml = $("#template-supplier-purchase").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetCutomerCollection: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.AccountId", null,
                    function (key, g) {
                        var result = {
                            AccountId: key,
                            Name: g.FirstOrDefault().Name,
                            Total: g.Sum("$.Credit")
                        }
                        return result;
                    }).ToArray();
            var categories = new Array();
            var values = new Array();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Total.format() + "</td></tr>";
                categories.push(transaction.Name);
                values.push(transaction.Total);

            }
            var templatehtml = $("#template-customer-collections").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
            var options =
                     {

                         legend: {
                             data: ['Collection']
                         },
                         xAxis: [{
                             type: 'category',
                             data: categories
                         }],
                         yAxis: [{
                             type: 'value',
                             splitArea: {
                                 show: true
                             }
                         }],
                         series: [{
                             name: 'Collection',
                             type: 'bar',
                             data: values
                         }]
                     }

            Report.MakeGraph(options);
        },
        GetSupplierPyaments: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.AccountId", null,
                    function (key, g) {
                        var result = {
                            AccountId: key,
                            Name: g.FirstOrDefault().Name,
                            Total: g.Sum("$.Debit")
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Total.format() + "</td></tr>";

            }
            var templatehtml = $("#template-supplier-payments").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetProductWiseSaleAndCustomerComparison: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.AccountId", null,
                    function (key, g) {
                        var result = {
                            AccountId: key,
                            Name: g.FirstOrDefault().Name,
                            Sales: g.ToArray(),
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            var formatedResults = new Array();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var formatedResult = new Object();
                formatedResult["Customer"] = transaction.Name
                var ItemTotals = Enumerable.From(transaction.Sales).GroupBy("$.ItemId", null,
                   function (key, g) {
                       var result = {
                           ItemId: key,
                           Item: g.FirstOrDefault(),
                           Total: g.Sum("$.Amount")
                       }
                       return result;
                   }).ToArray();
                for (var j in ItemTotals) {
                    var itemTotal = ItemTotals[j];
                    var itemName = itemTotal.Item.ItemCode + "-" + itemTotal.Item.ItemName;
                    formatedResult[itemName] = itemTotal.Total;
                }
                formatedResults.push(formatedResult);


            }
            var html = "";
            for (var i in formatedResults) {
                var transaction = formatedResults[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Total + "</td></tr>";

            }
            var templatehtml = $("#template-productwise-sales-and-customer-comparison").html();
            var obj = {
                '[TableBody]': html,
                "[Balance]": transaction.Balance
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetCutomerCollectionAndSales: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.AccountId", null,
                    function (key, g) {
                        var result = {
                            AccountId: key,
                            Name: g.FirstOrDefault().Name,
                            Credit: g.Sum("$.Credit"),
                            Debit: g.Sum("$.Debit")
                        }
                        return result;
                    }).ToArray();
            var categories = new Array();
            var credits = new Array();
            var debits = new Array();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Debit.format() + "</td>"
                html += "<td>" + transaction.Credit.format() + "</td></tr>";
                categories.push(transaction.Name);
                credits.push(transaction.Credit);
                debits.push(transaction.Debit);

            }
            var templatehtml = $("#template-customer-collections-and-sales").html();
            var obj = {
                '[TableBody]': html,
                "[Balance]": transaction.Balance
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
            var options =
                     {

                         legend: {
                             data: ['Debits', 'Credits']
                         },
                         xAxis: [{
                             type: 'category',
                             data: categories
                         }],
                         yAxis: [{
                             type: 'value',
                             splitArea: {
                                 show: true
                             }
                         }],
                         series: [{
                             name: 'Debits',
                             type: 'bar',
                             data: debits
                         },
                         {
                             name: 'Credits',
                             type: 'bar',
                             data: credits
                         }
                         ]
                     }

            Report.MakeGraph(options);
        },
        GetSupplierPurchasePayments: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.AccountId", null,
                    function (key, g) {
                        var result = {
                            AccountId: key,
                            Name: g.FirstOrDefault().Name,
                            Credit: g.Sum("$.Credit"),
                            Debit: g.Sum("$.Debit")
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                html += "<tr><td>" + transaction.Name + "</td>";
                html += "<td>" + transaction.Debit.format() + "</td>"
                html += "<td>" + transaction.Credit.format() + "</td></tr>";

            }
            var templatehtml = $("#template-customer-collections-and-sales").html();
            var obj = {
                '[TableBody]': html,
                "[Balance]": transaction.Balance
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        //for single customer only
        GetCustomerProductWisePerofrmance: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.ItemId", null,
                    function (key, g) {
                        var result = {
                            ItemId: key,
                            Item: g.FirstOrDefault(),
                            Sales: g.ToArray(),
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            var monthdata = new Array();

            var formatedResults = new Array();
            var headers = new Array("Item");
            var currentmonth = new Date().getMonth() + 1;
            var k = 1;
            for (var i in groupdata) {

                var transaction = groupdata[i];
                var formatedResult = new Object();
                formatedResult["Item"] = transaction.Item.ItemCode + "-" + transaction.Item.ItemName;
                for (j = 7; j <= currentmonth; j++) {

                    var date = new Date(new Date().getFullYear(), j - 1, 1);
                    var monthName = moment(date).format("MMMM");
                    var total = Enumerable.From(transaction.Sales).Where(function (x) { return x.Month == j }).Sum("$.Amount");
                    formatedResult[monthName] = total;
                    if (k == 1)
                        headers.push(monthName);
                    monthdata.push(
                        {
                            Month: j,
                            FullMonth: monthName,
                            ShortMonth: moment(date).format("MMM"),
                            Total: total
                        });


                }
                k++;

                formatedResults.push(formatedResult);




            }
            monthdata = Enumerable.From(monthdata).OrderBy("$.Month").GroupBy("$.Month", null,
                function (key, g) {
                    var result = {
                        name: g.FirstOrDefault().FullMonth,
                        type: "bar",
                        data: g.Select("$.Total").ToArray(),

                    }
                    return result;
                }).ToArray();
            var categories = new Array();

            var values = monthdata;
            var headHtml = "<tr>";
            for (var i in headers) {
                headHtml += "<th>" + headers[i] + "</th>";

            }
            headHtml += "<tr>";
            var legends = Enumerable.From(headers).Where(function (x) { return x != "Item" }).ToArray();
            var html = "";
            var l = 0;
            for (var i in formatedResults) {
                var m = 0;
                var transaction = formatedResults[i];

                html += "<tr>";
                for (var k in transaction) {
                    html += "<td>" + transaction[k] + "</td>";
                    if (m == 0)
                        categories.push(transaction[k]);
                    //else
                    //    monthData.push(transaction[k]);
                    m++;
                }
                //values.push({
                //    name: legends[l],
                //    type: 'bar',
                //    data: monthData
                //})
                html += "</tr>";
                k++;

            }
            var templatehtml = $("#template-productwise-sale-performance").html();
            var obj = {
                '[TableBody]': html,
                '[TableHead]': headHtml,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);

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
                            type: 'value',
                            splitArea: {
                                show: true
                            }
                        }],
                        series: values
                    }

            Report.MakeGraph(options);
        },
        GetSuppierProductWisePerofrmance: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.ItemId", null,
                    function (key, g) {
                        var result = {
                            ItemId: key,
                            Item: g.FirstOrDefault(),
                            Sales: g.ToArray(),
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            var formatedResults = new Array();
            var headers = new Array("Item");
            var currentmonth = new Date().getMonth() + 1;
            var k = 1;
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var formatedResult = new Object();
                formatedResult["Item"] = transaction.Item.ItemCode + "-" + transaction.Item.ItemName;
                for (j = 7; j <= currentmonth; j++) {

                    var date = new Date(new Date().getFullYear(), j - 1, 1);
                    var monthName = moment(date).format("MMMM");
                    var total = Enumerable.From(transaction.Sales).Where(function (x) { return x.Month == j }).Sum("$.Amount");
                    formatedResult[monthName] = total;
                    if (k == 1)
                        headers.push(monthName);

                }
                formatedResults.push(formatedResult);
                k++;


            }
            var headHtml = "<tr>";
            for (var i in headers) {
                headHtml += "<th>" + headers[i] + "</th>";
            }
            headHtml += "<tr>";
            var html = "";
            for (var i in formatedResults) {
                var transaction = formatedResults[i];
                html += "<tr>";
                for (var k in transaction) {
                    html += "<td>" + transaction[k] + "</td>";
                }
                html += "</tr>";

            }
            var templatehtml = $("#template-productwise-purchase-performance").html();
            var obj = {
                '[TableBody]': html,
                '[TableHead]': headHtml,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetItemWiseSaleComparison: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.ItemId", null,
                    function (key, g) {
                        var result = {
                            ItemId: key,
                            Item: g.FirstOrDefault(),
                            Sales: g.ToArray(),
                            Total: g.Sum("$.NetTotal")
                        }
                        return result;
                    }).ToArray();
            groupdata = Enumerable.From(groupdata).OrderByDescending(function (x) { return x.Total }).ToArray();
            var formatedResults = new Array();
            var headers = new Array("Item");
            var currentmonth = new Date().getMonth() + 1;
            currentmonth = 6;
            var k = 1;
            var formatedResults = new Array();
            for (var i in groupdata) {
                var transaction = groupdata[i];

                var headers = new Array("Item");
                var currentmonth = new Date().getMonth() + 1;
                currentmonth = 6;
                var k = 1;
                var formatedResult = new Object();
                formatedResult["Item"] = transaction.Item.ItemName;
                if (currentmonth < 7) currentmonth += 12;
                for (j = 7; j <= currentmonth; j++) {
                    //if (j > 12)
                    //    j = 1;
                    var month = j;
                    if (month > 12) month -= 12;
                    var date = new Date(new Date().getFullYear(), month - 1, 1);
                    var monthName = moment(date).format("MMM");
                    var total = Enumerable.From(transaction.Sales).Where(function (x) { return x.MonthNo == month }).Sum("$.NetTotal");
                    formatedResult[monthName] = total;
                    if (k == 1)
                        headers.push(monthName);

                }
                formatedResults.push(formatedResult);
                k++;
            }
            html = Report.CreateTableView(formatedResults, true, null, ReportObject)
            $("#report-conatiner").html(html);
            if (formatedResults.length > 0)
                Report.MakeGraph(null, ReportObject, formatedResults);

        },
        GetItemWisePurchaseComparison: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.ItemId", null,
                    function (key, g) {
                        var result = {
                            ItemId: key,
                            Item: g.FirstOrDefault(),
                            Sales: g.ToArray(),
                            Total: g.Sum("$.Amount")
                        }
                        return result;
                    }).ToArray();
            var formatedResults = new Array();
            var headers = new Array("Item");
            var currentmonth = new Date().getMonth() + 1;
            var k = 1;
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var formatedResult = new Object();
                formatedResult["Item"] = transaction.Item.ItemCode + "-" + transaction.Item.ItemName;
                for (j = 7; j <= currentmonth; j++) {

                    var date = new Date(new Date().getFullYear(), j - 1, 1);
                    var monthName = moment(date).format("MMMM");
                    var total = Enumerable.From(transaction.Sales).Where(function (x) { return x.Month == j }).Sum("$.Amount");
                    formatedResult[monthName] = total;
                    if (k == 1)
                        headers.push(monthName);

                }
                formatedResults.push(formatedResult);
                k++;


            }
            var headHtml = "<tr>";
            for (var i in headers) {
                headHtml += "<th>" + headers[i] + "</th>";
            }
            headHtml += "<tr>";
            var html = "";
            for (var i in formatedResults) {
                var transaction = formatedResults[i];
                html += "<tr>";
                for (var k in transaction) {
                    html += "<td>" + transaction[k] + "</td>";
                }
                html += "</tr>";

            }
            var templatehtml = $("#template-productwise-purchase-performance").html();
            var obj = {
                '[TableBody]': html,
                '[TableHead]': headHtml,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetSaleComparisonByYear: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var total = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + total.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-sale").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetPurchaseComparisonByYear: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var total = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + total.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-purchase").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetProductSalePerformance: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var total = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + total.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-sale").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetProductPurchasePerformance: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var total = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + total.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-sale").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetProductSalePerformanceByRate: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var totalamount = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                var totalquantity = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Quantity");
                var rate = 0;
                if (totalamount > 0 && totalquantity > 0)
                    rate = totalamount / totalquantity;
                rate = Common.GetFloat(rate.toFixed(2));
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + rate.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-sale").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetProductPurchasePerformanceByRate: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var totalamount = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Amount");
                var totalquantity = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.Quantity");
                var rate = 0;
                if (totalamount > 0 && totalquantity > 0)
                    rate = totalamount / totalquantity;
                rate = Common.GetFloat(rate.toFixed(2));
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + rate.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-purchase").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        EmployeeComparisonByJob: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.EmplopyeeId", null,
                    function (key, g) {
                        var result = {
                            EmplopyeeId: key,
                            Employee: g.FirstOrDefault(),
                            TotalJobs: g.Count(),
                            Jobs: g.ToArray()
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var jobs = transaction.Jobs;
                html += "<tr><td>" + transaction.Employee.EmployeeCode + "-" + transaction.Employee.EmployeeName + "</td>";
                var points = 0;
                for (var j in jobs) {
                    var job = jobs[j];
                    points += Report.GetFeedBackPoints(job.ServicesLevel)
                }
                html += "<td>" + transaction.TotalJobs + "</td>";
                html += "<td>" + points + "</td></tr>";

            }
            var templatehtml = $("#employee-comparison-by-job").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        EmployeeComparisonByRevenue: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.EmplopyeeId", null,
                    function (key, g) {
                        var result = {
                            EmplopyeeId: key,
                            Employee: g.FirstOrDefault(),
                            TotalJobs: g.Count(),
                            TotalRevinue: g.Sum("$.NetTotal"),
                            Jobs: g.ToArray()
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var jobs = transaction.Jobs;
                html += "<tr><td>" + transaction.Employee.EmployeeCode + "-" + transaction.Employee.EmployeeName + "</td>";
                var points = 0;
                //for (var j in jobs) {
                //    var job = jobs[j];
                //    points += Report.GetFeedBackPoints(job.ServicesLevel)
                //}
                html += "<td>" + transaction.TotalJobs + "</td>";
                html += "<td>" + transaction.TotalRevinue + "</td></tr>";

            }
            var templatehtml = $("#employee-comparison-by-job").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetSalesmenPerformanceBySales: function (records) {
            var html = "";
            var transactions = records;
            var groupdata = Enumerable.From(transactions).GroupBy("$.EmplopyeeId", null,
                    function (key, g) {
                        var result = {
                            EmplopyeeId: key,
                            Employee: g.FirstOrDefault(),
                            TotalJobs: g.Count(),
                            TotalRevinue: g.Sum("$.NetTotal"),
                            Jobs: g.ToArray()
                        }
                        return result;
                    }).ToArray();
            for (var i in groupdata) {
                var transaction = groupdata[i];
                var jobs = transaction.Jobs;
                //   html += "<tr><td>" + transaction.Employee.EmployeeCode + "-" + transaction.Employee.EmployeeName + "</td>";
                var points = 0;
                for (var j in jobs) {
                    var job = jobs[j];
                    html += "<tr><td>" + job.AccountName + "</td>";
                    html += "<td>" + job.NetTotal + "</td></tr>";
                }
                //html += "<td>" + transaction.TotalJobs + "</td>";
                //html += "<td>" + transaction.TotalRevinue + "</td></tr>";

            }
            var templatehtml = $("#Salesmen-Performance-by-Sales").html();
            var obj = {
                '[TableBody]': html,
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        GetSalesmanPerformanceByMonth: function (records) {
            var html = "";
            var transactions = records;
            var html = "";
            for (var j = 0; j <= 11; j++) {

                var date = new Date(new Date().getFullYear(), j, 1);
                var monthName = moment(date).format("MMMM");
                var total = Enumerable.From(transactions).Where(function (x) { return x.Month == j + 1 }).Sum("$.NetTotal");
                html += "<tr>";
                html += "<td>" + monthName + "</td>";
                html += "<td>" + total.format() + "</td>";
                html += "</tr>";

            }
            var templatehtml = $("#template-yearly-sale").html();
            var obj = {
                '[TableBody]': html
            }
            templatehtml = templatehtml.allReplace(obj);
            $("#report-conatiner").html(templatehtml);
        },
        ChangeDate: function (date) {
            var dt = moment(date).format('DD/MM/YYYY');
            if (dt == "Invalid date") dt = "";
            return dt;
        },
        ListView: function () {
            var _this = this;
            $(".page-title span").text("Custom Reports");
            $('#div-form,#div-report').addClass('hide');
            $('#div-table').removeClass('hide');
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);

            }
            else {
                var options = {
                    //"scrollY": 300,
                    "aLengthMenu": [
                    [10, 20, 50, 100, 200],
                    [10, 20, 50, 100, 200] // change per page values here
                    ],
                    "iDisplayLength": 100
                };
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?&module=" + Url.Module;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url, options);
            }
        },
        Close: function () {
            $('#div-form,#div-report').addClass('hide');
            $('#div-table').removeClass('hide');
        },
        ReinializePlugin: function () {
            var _this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
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
        BindAllLeafAccountsSelect: function () {
            var $this = this;
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products))
            var accounts = Common.GetAllLeafAccounts(exids);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        BindProductSelect: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Products);
            var html = Report.GetSelectHTML(accounts);
            return html;
        },
        BindWheatTypeSelect: function () {
            var $this = this;
            var html = "";
            var html = "<option value='1'>GOVT</option><option value='0'>Private</option>";
            return html;
        },
        BindMonthSelect: function () {
            var $this = this;
            var html = "";
            var html = "<option value='1'>January</option>";
            html += "<option value='2'>February</option>";
            html += "<option value='3'>March</option>";
            html += "<option value='4'>April</option>";
            html += "<option value='5'>May</option>";
            html += "<option value='6'>June</option>";
            html += "<option value='7'>July</option>";
            html += "<option value='8'>August</option>";
            html += "<option value='9'>September</option>";
            html += "<option value='10'>October</option>";
            html += "<option value='11'>November</option>";
            html += "<option value='12'>December</option>";
            return html;
        },

        BindYearSelect: function () {
            var $this = this;
            var html = "";
            var html = "<option value='2015'>2015</option>";
            html += "<option value='2016'>2016</option>";
            html += "<option value='2017'>2017</option>";
            html += "<option value='2018'>2018</option>";
            html += "<option value='2019'>2019</option>";
            html += "<option value='2020'>2020</option>";
            return html;
        },
        BindOrderTypeSelect: function () {
            var $this = this;
            var html = "";
            html += "<option></option>";
            html += "<option value='" + VoucherType.saleorder + "'>Sale Order</option>";
            html += "<option value='" + VoucherType.purchaseorder + "'>Purchase Order</option>";
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
        }
    }
}();