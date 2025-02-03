
var Ledger = function () {
    var apiController = "Stock";
    var PageSetting = new Object();
    var allData = [];
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
            $("#cosumeItems").on("change", function () {
                $(".consum-col").toggleClass("hide", this.value == "No");
            })
        },
        ShowHideSummary() {
            // Get values from dropdowns
            const summary = $("#reportType").val() === "Summary";
            const price = $("#priceType").val() === "Price";

            // Handle common logic for both Summary and non-Summary cases
            $(".detail-col").toggleClass("hide", summary);
            $(".first-col").attr("colspan", summary ? 4 : 7);
            $(".row-price-detail").toggleClass("hide", summary || !price);
            $(".row-qty-detail").toggleClass("hide", summary || price);

            // Handle row visibility based on `price`
            $(".row-price").toggleClass("hide", !price);
            $(".row-qty").toggleClass("hide", price);

            // Handle PL column visibility
            $(".pl-col").toggleClass("hide", !price);

            $("#cosumeItems").trigger("change");
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
                        if (data.length == 0) return
                        allData = data;

                        const records = [];
                        const products = PageSetting.Products;

                        let html = "";
                        let sr = 0;
                        const uniqueGroupNames = [...new Set(data.map(e => e.ItemGroupName))];
                        for (let g in uniqueGroupNames) {
                            const gName = uniqueGroupNames[g];
                            const groupItems = data.filter(e => e.ItemGroupName === gName)
                            const uniqueStatus = [...new Set(groupItems.map(e => e.Status))];
                            for (let s in uniqueStatus) {
                                ++sr;
                                const sName = uniqueStatus[s];
                                const statusItems = groupItems.filter(e => e.Status == sName);
                                const statusItem = statusItems[0];

                                const masterRecord = { DebitTotal: 0, CreditTotal: 0 }
                                for (let k in statusItem) {
                                    if (!(k.startsWith('Debit') || k.startsWith('Credit'))) {
                                        masterRecord[k] = statusItem[k];
                                        continue;
                                    }
                                    // Handle Code
                                    if (k.endsWith('Code')) {
                                        masterRecord[k] = statusItem[k];
                                        masterRecord[k + 'Name'] = products.find(x => x.Id == statusItem[k])?.Name || statusItem[k];
                                        continue;
                                    }

                                    // Handle Prices
                                    if (k.endsWith('Price')) {
                                        masterRecord[k] = statusItems.map(e => e[k]).reduce((a, b) => a + b, 0);
                                        if (k.startsWith('Debit')) masterRecord.DebitTotal += masterRecord[k]
                                        if (k.startsWith('Credit')) masterRecord.CreditTotal += masterRecord[k]

                                        continue;
                                    }
                                    // Handle Quantity
                                    masterRecord[k] = statusItems.map(e => e[k]).reduce((a, b) => a + b, 0);

                                    masterRecord.Detail = []
                                    statusItems.forEach(x => {
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

                                let validObj = {}
                                for (let key in masterRecord) {
                                    if (masterRecord[key]) validObj[key] = masterRecord[key]
                                }
                                const keys = Object.keys(validObj);
                                const debitKeys = keys.filter(x => x.startsWith('Debit') && x.endsWith('Name'));
                                const creditKeys = keys.filter(x => x.startsWith('Credit') && x.endsWith('Name'));
                                if (sr == 1) {
                                    html += "<thead>";
                                    html += `<tr><th class="first-col"></th><th class="text-center" colspan="${debitKeys.length} ">Debit Items</th><th class="text-center consum-col" colspan="${creditKeys.length}">Credit Items</th><th class="pl-col"></th></tr>`;
                                    html += `<tr><th>Sr #</th><th>Animals</th><th>Category</th><th>Status</th><th class="detail-col">Code</th><th class="detail-col">Name</th><th class="detail-col">Comments</th>`
                                        + [...debitKeys, ...creditKeys].map(a => `<th class="${(a.startsWith("Credit") ? "consum-col" : "")}">${masterRecord[a]}</th>`)
                                        + `<th class="pl-col consum-col">P&L</th></tr>`;
                                    html += "</thead>";
                                    html += "<tbody>"
                                }
                                // Summary Price Column
                                html += `<tr class="row-price"><td>${sr}</td><td>${statusItems.length}</td><td>${masterRecord.ItemGroupName}</td><td>${masterRecord.Status}</td>`
                                    + `<td class="detail-col" colspan="3"></td>`
                                    + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}"><strong>${masterRecord[a.replace(/CodeName/g, 'Price')]}</strong></td>`) +
                                    `<td class="pl-col consum-col">${masterRecord.DebitTotal - masterRecord.CreditTotal}</td></tr>`

                                // Summary Qty Column
                                html += `<tr class="row-qty"><td>${sr}</td><td>${statusItems.length}</td><td>${masterRecord.ItemGroupName}</td><td>${masterRecord.Status}</td>`
                                    + `<td class="detail-col" colspan="3"></td>`
                                    + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}"><strong>${masterRecord[a.replace(/CodeName/g, '')]}</strong></td>`) +
                                    `<td class="pl-col consum-col">${masterRecord.DebitTotal - masterRecord.CreditTotal}</td></tr>`

                                // Detail records
                                masterRecord.Detail.forEach(child => {
                                    // Detail Price Column
                                    html += `<tr class="row-price-detail"><td colspan="4"></td><td class="detail-col">${child.ItemCode}</td><td class="detail-col">${child.ItemName}</td><td class="detail-col">${(child.Comment || "")}</td>`
                                        + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}">${child[a.replace(/CodeName/g, 'Price')]}</td>`) +
                                        `<td class="pl-col consum-col">${child.DebitTotal - child.CreditTotal}</td></tr>`

                                    // Detail Qty Column
                                    html += `<tr class="row-qty-detail"><td colspan="4"></td><td class="detail-col">${child.ItemCode}</td><td class="detail-col">${child.ItemName}</td><td class="detail-col">${(child.Comment || "")}</td>`
                                        + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}">${child[a.replace(/CodeName/g, '')]}</td>`) +
                                        `<td class="pl-col consum-col">${child.DebitTotal - child.CreditTotal}</td></tr>`
                                })

                            }

                        }
                        html += "</tbody>"

                        let validObj2 = {}
                        let totalCredit = 0;
                        let totalDebit = 0;

                        const item = data[0]
                        for (let key in item) {
                            if ((key.startsWith('Credit') || key.startsWith('Debit')) && key.endsWith('Code') && item[key]) {
                                validObj2[key + 'Name'] = products.find(x => x.Id == item[key])?.Name || item[key];
                                validObj2[key + 'TotalPrice'] = data.map(x => x[key.replace("Code", "Price")]).reduce((a, b) => a + b, 0);
                                validObj2[key + 'TotalQty'] = data.map(x => x[key.replace("Code", "")]).reduce((a, b) => a + b, 0);

                                if (key.startsWith('Credit')) {
                                    totalCredit = validObj2[key + 'TotalPrice']
                                } else {
                                    totalDebit = validObj2[key + 'TotalPrice']
                                }
                            }
                            if (item[key]) validObj2[key] = item[key]
                        }
                        const keys = Object.keys(validObj2);
                        const debitKeys = keys.filter(x => x.startsWith('Debit') && x.endsWith('Name'));
                        const creditKeys = keys.filter(x => x.startsWith('Credit') && x.endsWith('Name'));
                        
                        html += "<tfoot>";
                        html += `<tr class="row-price"><td colspan="4" class="text-right"><strong>Total</strong></td>`
                            + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}">${validObj2[a.replace(/Name/g, 'TotalPrice')]}</td>`)
                            + `<td class="pl-col consum-col">${"c"}</td></tr>`;
                        html += `<tr class="row-qty"><td colspan="4" class="text-right"><strong>Total</strong></td>`
                            + [...debitKeys, ...creditKeys].map(a => `<td class="${(a.startsWith("Credit") ? "consum-col" : "")}">${validObj2[a.replace(/Name/g, 'TotalQty')]}</td>`)
                            + `<td class="pl-col consum-col">${totalDebit - totalCredit}</td></tr>`;
                        html += "</tfoot>";
                        $("#mainTable").html(html);
                        $("#reportType").trigger("change");
                        $("#priceType").trigger("change");
                        $("#cosumeItems").trigger("change");
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