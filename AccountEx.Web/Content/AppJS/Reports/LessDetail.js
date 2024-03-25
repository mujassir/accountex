var LessDetail = function () {
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
                var ReportType = $("#ReportType").val();
                var ParentAccountId = $("#AreaAccountId").val();
                var qs = "?key=GetLessDetail";
                qs += "&FromDate=" + FromDate;
                qs += "&ToDate=" + ToDate;
                qs += "&ReportType=" + ReportType;
                qs += "&ParentAccountId=" + ParentAccountId;
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
                           

                            var groups = Enumerable.From(res.Data).Select("$.Group").Distinct().ToArray();
                            var customers = Enumerable.From(res.Data).Select("$.AccountName").Distinct().ToArray();
                            var monthdata = new Array();

                            var formatedResults = new Array();
                            //var headers = new Array("Customer", "Code");
                            var headers = new Array("Code", "Customer");
                            var k = 1;

                            for (var j in customers) {
                                var formatedResult = new Object();
                                formatedResult["Code"] = customers[j];
                                formatedResult["Customer"] = customers[j];

                                for (var r in groups) {
                                    var customer = customers[j];
                                    var group = groups[r];
                                    //var transaction = data[r];
                                    var qtyTotal = Enumerable.From(data).Where(function (p) { return p.Group == group && p.AccountName == customer }).Sum("$.Qty");
                                    var amountTotal = Enumerable.From(data).Where(function (p) { return p.Group == group && p.AccountName == customer }).Sum("$.NetAmount");
                                    var customer = Enumerable.From(data).FirstOrDefault(null, function (p) { return p.Group == group && p.AccountName == customer });
                                    if (customer != null)
                                        formatedResult["Code"] = customer.AccountCode;
                                    var rate = 0;
                                    if (qtyTotal > 0)
                                        rate = amountTotal / qtyTotal;
                                    rate = rate.toFixed("2");
                                    formatedResult[group + "Qunatity"] = qtyTotal;
                                    formatedResult[group + "Amount"] = amountTotal;
                                    formatedResult[group + "Rate"] = rate;
                                    if (k == 1)
                                        headers.push(group);
                                }

                                k++;

                                //}


                                formatedResults.push(formatedResult);
                            }

                            var headerHtml = "";
                            headerHtml += '<tr>';
                            var m = 1
                            for (var i in headers) {

                                if (i <= 1)
                                    headerHtml += '<td >' + headers[i] + '</td>';
                                else {
                                    headerHtml += '  <td colspan="2" class="group-head">' + headers[i] + '<table class="innertable"><thead><tr><th class="align-right">QTY</th><th class="align-right">RATE</th></tr></thead> </table></td>'

                                }

                            }
                            headerHtml += '<td class="align-right">Amount</td>';
                            headerHtml += '</tr>';
                            var body = "";
                            var i = 0;



                            for (var j in formatedResults) {
                                var r = 0;
                                var totalAmount = 0;
                                var record = formatedResults[j];
                                body += (i % 2 == 0) ? '<tr >' : '<tr>';
                                for (var key in headers) {
                                    var head = headers[key];
                                    if (r >= 2) {
                                        totalAmount += Common.GetFloat(record[head + "Amount"]);
                                        head += "Qunatity";
                                        var token = record[head];
                                        var token = token == null ? "" : token + "";
                                        if (token != null && token.indexOf("T00:00:00") > -1) {
                                            token = moment(token).format('DD/MM/YYYY');
                                        }
                                        body += "<td class='quantity align-right num3'>" + token.format() + '</td>';

                                        var head = headers[key];

                                        head += "Rate";
                                        var token = record[head];
                                        var token = token == null ? "" : token + "";
                                        if (token != null && token.indexOf("T00:00:00") > -1) {
                                            token = moment(token).format('DD/MM/YYYY');
                                        }
                                        body += "<td class='rate align-right num3'>" + token.format() + '</td>';
                                    }

                                    else {
                                        var token = record[head];
                                        var token = token == null ? "" : token + "";
                                        if (token != null && token.indexOf("T00:00:00") > -1) {
                                            token = moment(token).format('DD/MM/YYYY');

                                        }
                                        body += "<td>" + token + '</td>';
                                    }
                                    r++;
                                }
                                body += "<td class='align-right'>" + totalAmount.format() + '</td>';
                                i++;
                                body += '</tr>';

                            }
                           
                            
                           
                            
                            $("#report-table thead").html(headerHtml);
                            $("#report-table tbody").html(body);



                            //////start of footer total calculation
                            var body = "";
                            body += "<tr class='bold'>";
                            body += "<td colspan='2' class='align-right'>Total</td>";
                            var rowIndex; var totalqty = 0; var totalrate = 0; 
                            $("#report-table tbody tr:first").find('td.quantity').each(function () {
                                rowIndex = $(this).index();
                                $("#report-table tbody tr").each(function () {
                                    totalqty += Common.GetFloatHtml($("td", this).eq(rowIndex).text());
                                    totalrate += Common.GetFloatHtml($("td", this).eq(rowIndex + 1).text());
                                });
                                body += "<td class='align-right'>" + totalqty.format() + '</td>';
                                body += "<td class='align-right'>" + totalrate.format() + '</td>';
                                totalqty = 0;
                                totalrate = 0;
                            });

                            ///Calculating amount
                            var totalamount = 0;
                            $("#report-table tbody tr").each(function () {
                                totalamount += Common.GetFloatHtml($("td:last", this).text());
                            });
                            body += "<td class='align-right'>" + totalamount.format() + '</td>';
                            body += '</tr>';
                            $("#report-table tbody").append(body);
                            
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