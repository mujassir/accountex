var MarketShare = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () { $this.LoadData(); });
            $(document).on("change", "#GroupId", function (event) {
                $this.GetSubGroupByGroupId();
            });

        },

        GetSubGroupByGroupId: function () {
            var $this = this;
            var groupId = $("#GroupId").val();
            var qs = "?key=GetSubGroupByGroupId";
            qs += "&groupId=" + groupId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#SubGroupId"), true);


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
            var groupId = $("#GroupId").val();
            var reportType = $("#ReportType").val();
            var recordCount = $("#RecordCount").val();
            var subGroupId = $("#SubGroupId").val();
            var year = $("#Year").val();
            var regionIds = $("#RegionIds").val();
            if (!Common.isNullOrWhiteSpace(regionIds))
                regionIds = regionIds.join(",");

            var divisionIds = $("#DivisionIds").val();
            if (!Common.isNullOrWhiteSpace(divisionIds))
                divisionIds = divisionIds.join(",");
            var deliveryTypeIds = $("#DeliveryTypeIds").val();

            if (!Common.isNullOrWhiteSpace(deliveryTypeIds))
                deliveryTypeIds = deliveryTypeIds.join(",");


            var qs = "?key=MarketShareSituation";
            qs += "&year=" + year;
            qs += "&groupId=" + groupId;
            qs += "&subGroupId=" + subGroupId;
            qs += "&reportType=" + reportType;
            qs += "&recordCount=" + recordCount;
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: { regionIds: regionIds, divisionIds: divisionIds, deliveryTypeIds: deliveryTypeIds, },
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var RegionRecords = res.Data.Table;
                            var Summaryrecords = res.Data.Table1;
                            var total = 85000;
                            var headerHtml = "<tr>";
                            headerHtml += "<th>Vendor</th>";
                            headerHtml += "<th>Region</th>";
                            if (reportType == 0)
                                headerHtml += "<th class='align-right'>Sales</th>";
                            else
                                headerHtml += "<th class='align-right'>Quantity</th>";
                            headerHtml += "<th>%</th>";
                            $("#tbl-report  thead").html(headerHtml);
                            html = "";

                            var regions = Enumerable.From(RegionRecords).Select("$.Region").Distinct().ToArray();
                            var groupRecords = Enumerable.From(Summaryrecords).GroupBy("$.Vendor", null,
                   function (key, g) {
                       var result = {
                           Vendor: key,
                           Records: g.ToArray(),
                           Total: g.Sum("$.Value")
                       }
                       return result;
                   }).OrderByDescending("$.Total").Take(recordCount).ToArray();
                            var labels = new Array();
                            var dataSets = new Array();

                            for (var i in groupRecords) {
                                var groupRecord = groupRecords[i];
                                var records = groupRecord.Records;
                                records = Enumerable.From(records).OrderBy("$.Region").ToArray();
                                labels.push(groupRecord.Vendor)
                                html += "<tr data-row='detail' class='bold'><td colspan='4'>" + groupRecord.Vendor + "</td></tr>";

                                for (var j in records) {
                                    var record = records[j];
                                    html += "<tr>";
                                    html += "<td></td>";
                                    html += "<td>" + record.Region + "</td>";
                                    html += "<td class='align-right'>" + record.Value.format() + "</td>";
                                    var regionRecord = Enumerable.From(RegionRecords).FirstOrDefault(null, "$.Region === '" + record.Region + "'");
                                    if (regionRecord != null)
                                        total = regionRecord.Value;
                                    var percent = 0;
                                    if (total > 0)
                                        percent = Common.GetInt((record.Value / total) * 100);
                                    html += "<td>" + percent + "</td>";
                                    html += "</tr>";
                                }
                                html += "</tr>";
                                html += "<tr class='bold'>";
                                html += "<td class='align-right' colspan='2'>Grand Total</td>";
                                html += "<td class='align-right'>" + groupRecord.Total.format() + "</td>";
                                html += "<td></td>";
                            }

                            $("#tbl-report tbody").html(html);


                            for (var r in regions) {
                                total = 0;
                                var region = regions[r];
                                var data = new Array();
                                for (var l in labels) {
                                    var label = labels[l];



                                    var regionRecord = Enumerable.From(RegionRecords).FirstOrDefault(null, "$.Region === '" + region + "'");
                                    var value = Enumerable.From(Summaryrecords).Where(function (x) { return x.Region == region && x.Vendor == label }).Sum("$.Value");
                                    if (regionRecord != null)
                                        total = regionRecord.Value;
                                    var percent = 0;
                                    if (total > 0)
                                        percent = Common.GetInt((value / total) * 100);
                                    data.push(percent);
                                }
                                dataSets.push({ label: region, backgroundColor: $this.GetDynamicColors(), data: data });
                            }

                            var ctx = document.getElementById('canvas').getContext('2d');
                            window.myBar = new Chart(ctx, {
                                type: 'bar',
                                data: { labels: labels, datasets: dataSets },
                                options: {
                                    title: {
                                        display: true,
                                        text: 'Market Share Situation'
                                    },
                                    tooltips: {
                                        mode: 'index',
                                        intersect: false
                                    },
                                    responsive: true,
                                    scales: {
                                        xAxes: [{
                                            stacked: true,
                                        }],
                                        yAxes: [{
                                            stacked: true
                                        }]
                                    }
                                }
                            });


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
        GetDynamicColors: function () {
            var r = Math.floor(Math.random() * 255);
            var g = Math.floor(Math.random() * 255);
            var b = Math.floor(Math.random() * 255);
            return "rgb(" + r + "," + g + "," + b + ")";
        },
        DrawGraph: function (res) {

        },
    };
}();