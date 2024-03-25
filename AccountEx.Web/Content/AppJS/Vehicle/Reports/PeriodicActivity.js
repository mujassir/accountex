
var PeriodicActivity = function () {
    var API_CONTROLLER = "VehicleReport";
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#BranchId").change(function () {
                $this.LoadData();
            })
        },
        LoadData: function () {
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var branchId = $("#BranchId").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var accountId = $("#Account").val();
            var qs = "?key=GetVehiclePeriodicActivity";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&branchId=" + branchId;
            qs += "&accountId=" + accountId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Vehicle Sale report...please wait",
                success: function (res) {
                    if (res.Success) {

                        var data = res.Data;
                        var i = 0;

                        var years = Enumerable.From(res.Data).Select("$.Year").Distinct().OrderBy("$").ToArray();
                        var months = Enumerable.From(res.Data).Select("$.Month").Distinct().OrderBy("$").ToArray();
                        var accounts = Enumerable.From(res.Data).Select("$.AccountCode").Distinct().OrderBy("$").ToArray();
                        var monthdata = new Array();

                        var formatedResults = new Array();
                        var headers = new Array("Account Code", "Account Name");
                        var k = 1;
                        for (var j in years) {
                            var year = years[j];

                            for (var l in months) {
                                var month = months[l];
                                var d = new Date(year, month - 1, 10);
                                var headerHtml = "(" + moment(d).format('MM') + ") " + moment(d).format('MMM-YYYY');
                                if (k == 1)
                                    headers.push(headerHtml);
                            }


                        }
                        for (var j in accounts) {
                            var formatedResult = new Object();
                            var accountcode = accounts[j];

                            var trans = Enumerable.From(data).FirstOrDefault(null, "$.AccountCode == '" + accountcode + "'");
                            formatedResult["Account Code"] = accountcode;
                            formatedResult["Account Name"] = trans.Account
                            for (var r in years) {
                                var year = years[r];
                                for (var l in months) {
                                    var month = months[l];
                                    var d = new Date(year, month - 1, 10);
                                    var headerHtml = "(" + moment(d).format('MM') + ") " + moment(d).format('MMM-YYYY');
                                    var amount = Enumerable.From(res.Data).Where(function (p) {
                                        return p.AccountCode == accountcode && p.Year == year && p.Month == month
                                    }).Sum("Common.GetInt($.Balance)");
                                    formatedResult[headerHtml] = amount;
                                }
                            }

                            k++;
                            formatedResults.push(formatedResult);
                        }

                        var headerHtml = "";
                        headerHtml += '<tr>';
                        var m = 1
                        for (var i in headers) {


                            headerHtml += '<th >' + headers[i] + '</th>';

                        }

                        headerHtml += '</tr>';
                        var body = "";
                        var i = 0;

                        for (var j in formatedResults) {
                            var record = formatedResults[j];
                            body += (i % 2 == 0) ? '<tr>' : '<tr>';
                            var m = 1
                            for (var k in headers) {

                                var token = record[headers[k]];
                                var token = token == null ? "" : token + "";
                                if (k > 1)
                                    token = Common.GetInt(token).format();

                                body += '<td>' + token + '</td>';

                            }
                            i++;
                            body += '</tr>';
                        }
                        $(".report-table thead").html(headerHtml);
                        $(".report-table tbody").html(body);


                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        }
    };
}();