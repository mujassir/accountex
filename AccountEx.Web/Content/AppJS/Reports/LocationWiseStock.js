
var LocationWiseStock = function () {
    var API_CONTROLLER = "Report";
    var datatableId = "mainTable";
    var listLoaded = false;
    return {
        init: function () {
            var $this = this;
            PageSetting = Common.LoadPageSetting();
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            $("#ReportType").change(function () {
                var type = $(this).val();
                if (type.toLocaleLowerCase() == "summary") {
                    $("tr[data-row='summary']").removeClass("hide");
                    $("tr[data-row='detail']").addClass("hide");
                }
                else {
                    $("tr[data-row='summary']").addClass("hide");
                    $("tr[data-row='detail']").removeClass("hide");
                }
                // Common.InitMultiSelectWithTable("column-hide-show-container", "mainTable");
            });
            $this.LoadData();
            // Common.InitMultiSelectWithTable("column-hide-show-container", "mainTable");
        },
        LoadData: function () {
            var $this = this;
            $("#lblReportDate").html("Date: " + $("#FromDate").val() + " to " + $("#ToDate").val());
            var qs = "?key=LocationWiseStock";
            const locationIds = $('#LocationIds').val()?.join(',') || '';
            if (locationIds == '') {
                Common.ShowError('Please select Location');
                return
            }
            qs += `&locationIds=${locationIds}`;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  party wise" + " " + Common.GetQueryStringValue("type").toLowerCase() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var data = res.Data;
                        const products = [...new Map(data.map(item => [item.ItemId, item])).values()];
                        const locations = [...new Map(data.map(item => [item.LocationId, item])).values()];
                        const warehouses = [...new Map(data.map(item => [item.WarehouseId, item])).values()];

                        let header = `<tr><th></th>`;
                        header += locations.map(e => {
                            const count = warehouses.filter(x => x.LocationId == e.LocationId).length;
                            return `<th colspan="${count}" class="text-center">${e.LocationName}</th>`
                        })
                        header += `</tr>`;

                        header += `<tr><th>Product</th>`;
                        header += locations.map(e => warehouses.filter(x => x.LocationId == e.LocationId).map(a => `<th class="text-center">${a.WarehouseName}</th>`))
                        header += `</tr>`;
                        $(".report-table thead").html(header);

                        for (var i in products) {
                            const product = products[i];
                            html += "<tr>";
                            html += `<td>${product.ItemCode} ${product.ItemName}</td>`;
                            html += locations.map(l =>
                                warehouses.filter(w => w.LocationId == l.LocationId)
                                    .map(w => `<td class="text-center">${data.find(p =>
                                        p.LocationId === l.LocationId && p.WarehouseId === w.WarehouseId && p.ItemId === product.ItemId)?.Quantity 
                                        || 0}</td>`)
                            ) 
                            html += "</tr>";
                        }
                        $(".report-table tbody").html(html);
                        $("#ReportType").trigger("change");
                        $('#column-hide-show-container select').trigger('change');

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