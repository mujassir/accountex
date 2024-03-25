var ProjectDetail = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.LoadCustomers();
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },

        LoadCustomers: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetCustomerIdNameByUserId",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#CustomerIds"), true)
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
            //var fromdate = $("#FromDate").val();
            //var toDate = $("#ToDate").val();
            //$("#lblReportDate").html("Date: " + fromdate + " to " + toDate);
            var year = $("#Year").val();
            var customerIds = $("#CustomerIds").val();
            if (!Common.isNullOrWhiteSpace(customerIds))
                customerIds = customerIds.join(",");

            var salePersonIds = $("#SalePersonIds").val();
            if (!Common.isNullOrWhiteSpace(salePersonIds))
                salePersonIds = salePersonIds.join(",");


            var regionIds = $("#RegionIds").val();
            if (!Common.isNullOrWhiteSpace(regionIds))
                regionIds = regionIds.join(",");

            var divisionIds = $("#DivisionIds").val();
            if (!Common.isNullOrWhiteSpace(divisionIds))
                divisionIds = divisionIds.join(",");
            var qs = "?key=ProjectDetail";
            qs += "&year=" + year;


            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: { customerIds: customerIds, salePersonIds: salePersonIds, regionIds: regionIds, divisionIds: divisionIds },
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var records = res.Data.Table;
                            var sn = 1;
                            html = "";
                            for (var i in records) {
                                var record = records[i];
                                html += "<tr>";
                                html += "<td>" + sn + "</td>";
                                html += "<td>" + record.Region + "</td>";
                                html += "<td>" + record.SalePerson + "</td>";
                                html += "<td>" + record.Organization + "</td>";
                                html += "<td>" + record.Product + "</td>";
                                html += "<td>" + record.ActualProduct + "</td>";
                                html += "<td>" + record.Company + "</td>";
                                html += "<td>" + record.Division + "</td>";
                                html += "<td>" + record.Currency + "</td>";
                                html += "<td  class='align-right'>" + record.Price.format() + "</td>";
                                html += "<td class='align-right'>" + record.Qty.format() + "</td>";
                                html += "<td  class='align-right'>" + record.Potential.format() + "</td>";
                                html += "<td  class='align-right'>" + record.TPrice.format() + "</td>";
                                html += "<td class='align-right'>" + record.TQty.format() + "</td>";
                                html += "<td  class='align-right'>" + record.TPotential.format() + "</td>";
                                html += "<td class='align-right'>" + record.AchQty.format() + "</td>";
                                html += "<td  class='align-right'>" + record.AchPotential.format() + "</td>";
                                html += "<td>" + record.Status + "</td>";
                                html += "<td>" + record.Remarks + "</td>";
                                html += "</tr>";
                                sn++;
                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='19' style='text-align: center'>No record(s) found</td></tr>";
                            else {
                                html += "<tr class='bold'>";
                                html += "<td class='align-right' colspan='10'>Grand Total</td>";
                                var Qty = Enumerable.From(records).Sum("$.Qty");
                                var Potential = Enumerable.From(records).Sum("$.Potential");
                                var TPotential = Enumerable.From(records).Sum("$.TPotential");
                                var AchPotential = Enumerable.From(records).Sum("$.AchPotential");
                                var TQty = Enumerable.From(records).Sum("$.TQty");
                                var AchQty = Enumerable.From(records).Sum("$.AchQty");
                                html += "<td class='align-right'>" + Qty.format() + "</td>";
                                html += "<td class='align-right'>" + Potential.format() + "</td>";
                                html += "<td ></td>";
                                html += "<td class='align-right'>" + TQty.format() + "</td>";
                                html += "<td class='align-right'>" + TPotential.format() + "</td>";
                                html += "<td class='align-right'>" + AchQty.format() + "</td>";
                                html += "<td class='align-right'>" + AchPotential.format() + "</td>";
                                html += "<td colspan='2'></td>";
                                html += "</tr>";
                            }
                            $("#tbl-sale-detail tbody").html(html);


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


    };
}();