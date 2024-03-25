var SaleReportByArea = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $("input[name='FromDate']").val($("#txtFromDate").val());
            //$("#btnShowReport").click($this.LoadData)

            $("#btnShowReport").click(function () {
                $this.LoadData();
            });

            $this.LoadData();
            $("#ReportType").change(function () {
                var type = $(this).val();
              
                if (type == "Qunatity") {
                    $("#report-table tbody tr td.Amount").addClass("hide");
                    $("#report-table tbody tr td.Quantity").removeClass("hide");
                }
                else {
                    $("#report-table tbody tr td.Quantity").addClass("hide");
                    $("#report-table tbody tr td.Amount").removeClass("hide");
                }
            });
            //$this.LoadData();
            //$this.LoadPageSetting();
        },
        LoadAccounts: function () {
            var $this = this;
            var type = Common.GetQueryStringValue("type").toLowerCase();
            var id = PageSetting.Customers;
            var tokens = Common.GetLeafAccounts(id);
            Common.BindSelect(tokens, "#AreaAccountId", true);
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
                var FromDate = $("#FromDate").val();
                var ToDate = $("#ToDate").val();
                var AreaAccountId = $("#AreaAccountId").val();

                var qs = "?key=GetAreaWiseSaleReport";
                qs += "&FromDate=" + FromDate;
                qs += "&ToDate=" + ToDate;
                qs += "&AccountId=" + AreaAccountId;
                //qs += "&OpeningStock=" + $("#OpeningStock").val();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    success: function (res) {
                        if (res.Success) {

                            var html = "";
                            var select = "";
                            var amount = 0;
                            var data = res.Data;


                            var products = Enumerable.From(res.Data).Select("$.Name").Distinct().ToArray();
                            var dates = Enumerable.From(res.Data).Select("$.Date").Distinct().ToArray();
                            var monthdata = new Array();

                            var formatedResults = new Array();
                            var headers = new Array("Date");
                            var k = 1;

                            for (var j in dates) {
                                var formatedResult = new Object();
                                formatedResult["Date"] = dates[j];


                                for (var r in products) {
                                    var date = dates[j];
                                    var product = products[r];
                                    //var transaction = data[r];
                                    var qtyTotal = Enumerable.From(data).Where(function (p) { return p.Name == product && p.Date == date }).Sum("$.SaleQty");
                                    var amountTotal = Enumerable.From(data).Where(function (p) { return p.Name == product && p.Date == date }).Sum("$.SaleAmount");
                                    formatedResult[product + "Qunatity"] = qtyTotal;
                                    formatedResult[product + "Amount"] = amountTotal;
                                    if (k == 1)
                                        headers.push(product);
                                }

                                k++;

                                //}


                                formatedResults.push(formatedResult);
                            }

                            var headerHtml = "";
                            headerHtml += '<tr>';
                            var m = 1;
                            var n=1;
                            for (var i in headers) {

                                if(n==1)
                                    headerHtml += '<th >' + headers[i] + '</th>';
                                else
                                    headerHtml += '<th class="align-right">' + headers[i] + '</th>';

                                n++

                            }
                            headerHtml += '</tr>';
                            var body = "";
                            var i = 0;
                         
                            // var type = "Qunatity"
                            var type = $("#ReportType").val();
                         
                                for (var j in formatedResults) {
                                    var r = 1;
                                    var record = formatedResults[j];
                                    body += (i % 2 == 0) ? '<tr >' : '<tr>';
                                    for (var key in headers) {
                                        var head = headers[key];
                                        if (r > 1)
                                            head += "Qunatity";
                                        var token = record[head];
                                        var token = token == null ? "" : token + "";
                                        if (token != null && token.indexOf("T00:00:00") > -1) {
                                            token = moment(token).format('DD/MM/YYYY');
                                            body += "<td class='Quantity'>" + token + '</td>';
                                        }
                                        else
                                            body += "<td class='Quantity align-right'>" + token.format() + '</td>';
                                        r++;
                                    }
                                    i++;

                                    var r = 1;
                                    var record = formatedResults[j];
                                    body += (i % 2 == 0) ? '<tr >' : '<tr>';
                                    for (var key in headers) {
                                        var head = headers[key];
                                        if (r > 1)
                                            head += "Amount";
                                        var token = record[head];
                                        var token = token == null ? "" : token + "";
                                        if (token != null && token.indexOf("T00:00:00") > -1) {
                                            token = moment(token).format('DD/MM/YYYY');
                                            body += "<td class='Amount hide'>" + token + '</td>';
                                        }
                                        else 
                                            body += "<td class='Amount hide align-right'>" + token.format() + '</td>';
                                        r++;
                                    }
                                    i++;
                                    body += '</tr>';
                                }
                            
                            $("#report-table thead").html(headerHtml);
                            $("#report-table tbody").html(body);

                            if (type == "Qunatity") {
                                $("#report-table tbody tr td.Amount").addClass("hide");
                                $("#report-table tbody tr td.Quantity").removeClass("hide");
                            } else {
                                $("#report-table tbody tr td.Quantity").addClass("hide");
                                $("#report-table tbody tr td.Amount").removeClass("hide");
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