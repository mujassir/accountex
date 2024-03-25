
var StaticLedger = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $this.LoadPageSetting();
            $("input[name='FromDate']").val($("#txtFromDate").val());
            $("select.select2").select2();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $this.LoadAccounts();
            $("#TransactionType,input[name='sale-posted-type']").change(function () {
                var transactiontype = $("#TransactionType").val();
                var salepostedtype = $("input[name='sale-posted-type']:checked").val();
                if (transactiontype == "sale" && salepostedtype == "withinvoice")
                    $("#div-ClearingType").removeClass("hide");
                else
                    $("#div-ClearingType").addClass("hide");


            });

            //$("#TransactionType").change(function () {
            //    var transactiontype = $(this).val();
            //    if (transactiontype == VoucherType.sale) {
            //        var customers = PageSetting.Customers;
            //        $this.LoadAccounts(customers);
            //    }
            //    else {
            //        var suppliers = PageSetting.Suppliers;
            //        $this.LoadAccounts(suppliers);
            //    }

            //});

            //$("#TransactionType").change(function () {
            //    var transactiontype = $(this).val();
            //    if (transactiontype == VoucherType.sale) {
            //        $("#purchase-type-conatiner").addClass("hide");
            //        $("#sale-type-conatiner").removeClass("hide");
            //    }
            //    else {
            //        $("#sale-type-conatiner").addClass("hide");
            //        $("#purchase-type-conatiner").removeClass("hide");
            //    }
            //});

            var id = Common.GetQueryStringValue("accountId");
            var fromDate = Common.GetQueryStringValue("fromDate");
            var toDate = Common.GetQueryStringValue("toDate");
            if (id == undefined || id == "") {

                $(".row-account").removeClass("hide");
                $(".row-info").addClass("hide");
            }
            else {
                $(".row-info").removeClass("hide");
                $("input[name='FromDate']").val(fromDate);
                $("input[name='ToDate']").val(toDate);
                $("#btnShowReport").trigger("click");
            }
            $("#TransactionType").trigger("change");
            var type = Common.GetQueryStringValue("type").toLowerCase();
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
        },
        LoadData: function () {
            var $this = this;
            var date1 = $("#FromDate").val();
            var date2 = $("#ToDate").val();
            var accountid = $("#AccountId").val();
            var partnerId = $("#CompanyPartnerId").val();

            var transactiontype = $("#TransactionType").val();
            var salepostedtype = $("input[name='sale-posted-type']:checked").val();
            var voucherpostedtype = $("input[name='voucher-posted-type']:checked").val();
            var clearingType = $("#ClearingType").val();

            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            $("#lblPartyName").html("Account Title: " + $("#AccountId option:selected").text());
            $("#lblParty").html($("#AccountId option:selected").text());
            var qs = "?key=GetStaticLedger";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&accountid=" + accountid;
            qs += "&partnerId=" + partnerId;
            qs += "&transactiontype=" + transactiontype;
            qs += "&salepostedtype=" + salepostedtype;
            qs += "&voucherpostedtype=" + voucherpostedtype;
            qs += "&clearingType=" + clearingType;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Static Ledger...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var dcVoucherTypes = new Array(VoucherType.goodissue, VoucherType.goodreceive);
                        var voucherTypes = new Array(VoucherType.sale, VoucherType.purchase, VoucherType.salereturn, VoucherType.purchasereturn, VoucherType.gstsale,
                            VoucherType.gstpurchase, VoucherType.gstsalereturn, VoucherType.gstpurchasereturn);
                        html += "<tr class='bold'><td></td><td>Opening Balance</td><td></td><td></td><td></td><td class='align-right'>" + res.Data.OpeningBalance + "</td></tr>";
                        for (var i in data) {
                            var trans = data[i];
                            var vouchertype = Common.GetKeyFromEnum(trans.TransactionType, VoucherType);
                            var url = Common.GetTransactionUrl(trans.TransactionType, trans.VoucherNumber);
                            var vouchershortname = Common.GetKeyFromEnum(trans.TransactionType, VoucherShortName);
                            var vType = VoucherTypes[trans.TransactionType];
                            html += "<tr>";
                            html += "<td>" + trans.Date + "</td>";
                            html += "<td><a href='" + url + "' title='" + vType.Description + "' data-toggle='tooltip'>" + vType.Code + "-" + trans.VoucherNumber + "</a></td>";

                            if ($.inArray(data[i].TransactionType, voucherTypes) !== -1) {
                                html += "<td>" + $this.GetSaleVoucherDetail(data[i].VoucherNumber, data[i].TransactionType, res.Data) + "</td>";
                            }
                            else if ($.inArray(data[i].TransactionType, dcVoucherTypes) !== -1) {
                                html += "<td>" + $this.GetChallanVoucherDetail(data[i].VoucherNumber, data[i].InvoiceNumber, data[i].TransactionType, res.Data) + "</td>";
                            }
                            else {
                                html += "<td>" + data[i].Description + "</td>";
                            }
                            html += "   <td class='align-right'>" + trans.Debit + "</td>";
                            html += "  <td class='align-right'>" + trans.Credit + "</td>";
                            html += " <td class='align-right'>" + trans.Balance + "</td>";
                            html += "</tr>";
                        }
                        if (res.Data.Records.length == 0)
                            html += "  <tr><td colspan='6' style='text-align: center'>No record(s) found</td></tr>";
                        html += "<tr class='bold'><td></td><td>Total</td><td></td><td class='align-right'>" + res.Data.TotalDebit + "</td><td class='align-right'>" + res.Data.TotalCredit + "</td><td class='align-right'>" + res.Data.TotalBalance + "</td></tr>";
                        $(".report-table tbody").html(html);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetSaleVoucherDetail: function (voucherNo, transactionType, data) {
            var recordDetails = new Array();
            var dcVoucherTypes = new Array(VoucherType.goodissue, VoucherType.goodreceive);

            if ($.inArray(transactionType, dcVoucherTypes) !== -1) {
                recordDetails = data.Challans;
            }
            else {
                recordDetails = data.Sales;
            }
            var records = Enumerable.From(recordDetails).Where(function (p) { return p.VoucherNumber == voucherNo && p.TransactionType == transactionType }).ToArray();
            var html = "<table class='boldR full-width'>";
            for (var i in records) {
                var detail = records[i];
                var items = $.inArray(transactionType, dcVoucherTypes) !== -1 ? detail.DCItems : detail.SaleItems;
                for (var i in items) {
                    var sale = items[i];
                    html += "<tr>";
                    html += "<td>" + sale.ItemName + ", W:" + sale.Width + ", MR:" + sale.TotalMeters + ", KG:" + sale.Quantity + "@" + sale.Rate + " Per " + sale.UnitType + "=" + detail.Comments + "</td>";
                    html += "<td class='align-right'>" + sale.NetAmount + "</td>";
                    html += "</tr>";
                }

            }
            html += "</table>";
            return html;
        },
        GetChallanVoucherDetail: function (voucherNo, bookNo, transactionType, data) {
            var recordDetails = new Array();
            var dcVoucherTypes = new Array(VoucherType.goodissue, VoucherType.goodreceive);

            if ($.inArray(transactionType, dcVoucherTypes) !== -1) {
                recordDetails = data.Challans;
            }
            else {
                recordDetails = data.Sales;
            }
            var records = Enumerable.From(recordDetails).Where(function (p) { return p.VoucherNumber == voucherNo && p.TransactionType == transactionType }).ToArray();
            var html = "<table class='boldr full-width'>";
            for (var i in records) {
                var detail = records[i];
                var items = $.inArray(transactionType, dcVoucherTypes) !== -1 ? detail.DCItems : detail.SaleItems;
                for (var i in items) {
                    var sale = items[i];
                    html += "<tr>";
                    if (transactionType == VoucherType.goodreceive) {
                        html += "<td>" + sale.ItemName + ", GSM:" + sale.Weight + ", Unit:" + sale.MainUnitQuantity + ", Quantity:" + sale.Quantity + "@" + sale.Rate + "=" + detail.Comments + " DC#:" + bookNo + "</td>";
                        html += "<td class='align-right'>" + sale.NetAmount + "</td>";
                    } else {
                        html += "<td>" + sale.ItemName + ", W:" + sale.Width + ", MR:" + sale.TotalMeters + ", KG:" + sale.Quantity + "@" + sale.Rate + " Per " + sale.UnitType + "=" + detail.Comments + " DC#:" + bookNo + "</td>";
                        html += "<td class='align-right'>" + sale.NetAmount + "</td>";
                    }
                    html += "</tr>";
                }

            }
            html += "</table>";
            return html;
        },
        LoadAccounts: function (accountheadid) {
            var accounts = Common.GetAllLeafAccounts(PageSetting.Products);
            Common.BindSelect(accounts, "#AccountId", false);
        },

        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },

    };
}();