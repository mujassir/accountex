
var Ledger = function () {
    var apiController = "Stock";
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            $("#btnShowReport").click($this.LoadStock);

            $("#reportType").on("change", function () {
                $this.ShowHideSummary()
            })
            $("#priceType").on("change", function () {
                $this.ShowHideSummary()
            })
        },
        ShowHideSummary() {
            const summary = $("#reportType").val() === "Summary";
            const price = $("#priceType").val() === "Price";

            if (summary) {
                $(".detail-col").addClass("hide");
                $(".first-col").attr("colspan", 1);
                $(".row-price-detail").addClass("hide");
                $(".row-qty-detail").addClass("hide");

                if (price) {
                    $(".row-price").removeClass("hide");
                    $(".row-qty").addClass("hide");
                } else {
                    $(".row-price").addClass("hide");
                    $(".row-qty").removeClass("hide");
                }
            } else {
                $(".detail-col").removeClass("hide");
                $(".first-col").attr("colspan", 4);

                $(".row-price-detail").removeClass("hide");
                $(".row-qty-detail").removeClass("hide");

                if (price) {
                    $(".row-price").removeClass("hide");
                    $(".row-qty").addClass("hide");
                    $(".row-price-detail").removeClass("hide");
                    $(".row-qty-detail").addClass("hide");
                } else {
                    $(".row-price").addClass("hide");
                    $(".row-qty").removeClass("hide");
                    $(".row-price-detail").addClass("hide");
                    $(".row-qty-detail").removeClass("hide");
                }
            }
            if (price) {
                $(".pl-col").removeClass("hide");
            } else {
                $(".pl-col").addClass("hide");
            }


        },
        LoadStock: function () {
            var $this = this;
            var fromdate = $("#FromDate").val();
            var todate = $("#ToDate").val();
            $("#lblReportDate").html("Date: " + fromdate + " to " + todate);
            var qs = "?key=DailyActivityLedger";
            qs += "&fromdate=" + fromdate;
            qs += "&todate=" + todate;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  stock...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;

                        const records = [];
                        const products = PageSetting.Products;

                        const uniqueVoucher = [...new Set(data.map(e => e.VoucherNumber))];
                        for (let v in uniqueVoucher) {
                            const vNum = uniqueVoucher[v];
                            const voucherItems = data.filter(e => e.VoucherNumber == vNum);
                            const voucherItem = voucherItems[0];

                            const masterRecord = { DebitTotal: 0, CreditTotal: 0}
                            for (let k in voucherItem) {
                                if (!(k.startsWith('Debit') || k.startsWith('Credit'))) {
                                    masterRecord[k] = voucherItem[k];
                                    continue;
                                }
                                // Handle Code
                                if (k.endsWith('Code')) {
                                    masterRecord[k] = voucherItem[k];
                                    masterRecord[k + 'Name'] = products.find(x => x.Id == voucherItem[k])?.Name || voucherItem[k];
                                    continue;
                                }

                                // Handle Prices
                                if (k.endsWith('Price')) {
                                    masterRecord[k] = voucherItems.map(e => e[k]).reduce((a, b) => a + b, 0);
                                    if (k.startsWith('Debit')) masterRecord.DebitTotal += masterRecord[k]
                                    if (k.startsWith('Credit')) masterRecord.CreditTotal += masterRecord[k]

                                    continue;
                                }
                                // Handle Quantity
                                masterRecord[k] = voucherItems.map(e => e[k]).reduce((a, b) => a + b, 0);

                                masterRecord.Detail = []
                                voucherItems.forEach(x => {
                                    x.DebitTotal = 0;
                                    x.CreditTotal = 0;

                                    for (const key in x) {
                                        if (key.startsWith('Debit') && key.endsWith('Price')) {
                                            x.DebitTotal += x[key];
                                        }
                                        if (key.startsWith('Credit') && key.endsWith('Price')) {
                                            x.CreditTotal += x[key];
                                        }
                                    }
                                    masterRecord.Detail.push(x)
                                })
                            }
                            records.push(masterRecord)
                        }
                        
                        let html = ""
                        records.forEach(record => {
                            let validObj = {}
                            for (let key in record) {
                                if (record[key]) validObj[key] = record[key]
                            }
                            const keys = Object.keys(validObj);
                            const debitKeys = keys.filter(x => x.startsWith('Debit') && x.endsWith('Name'));
                            const creditKeys = keys.filter(x => x.startsWith('Credit') && x.endsWith('Name'));

                            html += "<thead>";
                            html += `<tr><th class="first-col"></th><th class="text-center" colspan="${debitKeys.length} ">Debit Items</th><th class="text-center" colspan="${creditKeys.length }">Credit Items</th><th class="pl-col"></th></tr>`;
                            html += `<tr><th>Voucher #</th><th class="detail-col">Code</th><th class="detail-col">Name</th><th class="detail-col">Comments</th>` + [...debitKeys, ...creditKeys].map(a => `<th>${record[a]}</th>`)
                                + `<th class="pl-col">P&L</th></tr>`;
                            html += "</thead>";
                            html += "<tbody>"

                            // Summary Price Column
                            html += `<tr class="row-price"><td class="first-col">${record.VoucherNumber}</td>`
                                + [...debitKeys, ...creditKeys].map(a => `<td><strong>${record[a.replace(/CodeName/g, 'Price')]}</strong></td>`) +
                                `<td class="pl-col">${record.CreditTotal - record.DebitTotal}</td></tr>`

                            // Summary Qty Column
                            html += `<tr class="row-qty"><td class="first-col">${record.VoucherNumber}</td>`
                                + [...debitKeys, ...creditKeys].map(a => `<td><strong>${record[a.replace(/CodeName/g, '')]}</strong></td>`) +
                                `<td class="pl-col">${record.CreditTotal - record.DebitTotal}</td></tr>`

                            // Detail records
                            record.Detail.forEach(child => {
                                // Detail Price Column
                                html += `<tr class="row-price-detail"><td>${child.VoucherNumber}</td><td class="detail-col">${child.ItemCode}</td><td class="detail-col">${child.ItemName}</td><td class="detail-col">${(child.Comment || "")}</td>`
                                    + [...debitKeys, ...creditKeys].map(a => `<td>${child[a.replace(/CodeName/g, 'Price')]}</td>`) +
                                    `<td class="pl-col">${child.CreditTotal - child.DebitTotal}</td></tr>`

                                // Detail Qty Column
                                html += `<tr class="row-qty-detail"><td>${child.VoucherNumber}</td><td class="detail-col">${child.ItemCode}</td><td class="detail-col">${child.ItemName}</td><td class="detail-col">${(child.Comment || "")}</td>`
                                    + [...debitKeys, ...creditKeys].map(a => `<td>${child[a.replace(/CodeName/g, '')]}</td>`) +
                                    `<td class="pl-col">${child.CreditTotal - child.DebitTotal}</td></tr>`
                            })

                            html += "</tbody>"
                            $("#mainTable").html(html);

                            $("#reportType").trigger("change");
                            $("#priceType").trigger("change");
                        })
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
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
    };
}();