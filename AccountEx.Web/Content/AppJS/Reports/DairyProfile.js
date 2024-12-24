
var Stock = function () {
    var apiController = "Stock";
    var DECIMAL_POINTS_FOR_TOTALS = 0;
    var DECIMAL_POINTS_FOR_STOCKTOTALS = 4;
    return {
        init: function () {
            var $this = this;
            $this.LoadStock();
            $("#btnShowReport").click($this.LoadStock);
            $("#AccountId").change($this.LoadStock);
            $("input[name='stock-type']").change($this.FilterByGroup);

            $('#txtSearch').keyup(function () {
                filterTable(document.getElementById("txtSearch"), document.getElementById("tbldetail"));
            });
            $('#Status, #GroupId').change(function () {
                const status = $('#Status').val();
                const group = $('#GroupId').val();

                $("#tbldetail tbody").find("[class^='group-']").addClass("hide");

                if (group && group != 0) {
                    $(`#tbldetail tbody .group-${group}.status-${status}`).removeClass("hide");
                } else {
                    $(`#tbldetail tbody .status-${status}`).removeClass("hide");
                }
            });

        },
        FilterByGroup: function () {

            var group = $("#GroupName option:selected").text();
            if (group == undefined || group == "" || group.trim() == "All Groups") {
                $("#tbldetail tbody tr").removeClass("hide");
            }
            else {
                var gr = group.replace(/\s+/g, "-").replace(/\\|\//g, "-").toLowerCase();
                $("#tbldetail tbody tr:not(.grouptr" + gr + ")").addClass("hide");
                $("#tbldetail tbody tr.grouptr" + gr).removeClass("hide");
            }
            if ($("input[value='stock']").is(":checked")) {
                $(".stock-value,.avg-sale-rate").removeClass("hide");
                $(".book-value,.unit-value,.avg-purchase-rate,.unit-price").addClass("hide");
            }
            else if ($("input[value='book']").is(":checked")) {
                $(".stock-value,.unit-value,.avg-sale-rate,.unit-price").addClass("hide");
                $(".book-value,.avg-purchase-rate").removeClass("hide");
            }
            else if ($("input[value='unit']").is(":checked")) {
                $(".stock-value,.book-value,.avg-sale-rate,.avg-purchase-rate").addClass("hide");
                $(".unit-value,.unit-price").removeClass("hide");
            }
        },
        LoadStock: function () {
            var $this = this;
            var fromdate = $("#FromDate").val();
            var todate = $("#ToDate").val();
            //$("#lblReportDate").html("Date: " + date);
            $("#lblReportDate").html("Date: " + fromdate + " to " + todate);
            var qs = "?key=GetDairyProfile";
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
                        var html = "";
                        var records = res.Data;
                        for (let i in records) {
                            const rec = records[i];
                            html += `<tr class="group-${rec.ItemGroupId} status-${rec.Status}"><td>${rec.ItemCode}</td><td>${rec.ItemName}</td>
                             <td>${rec.Milk}<td>${rec.ItemA}</td><td>${rec.ItemB}</td><td>${rec.ItemC}</td><td>${rec.ItemD}</td>
                             <td>${rec.Medicines}</td><td>${rec.PL}</td></tr>`
                        }
                        $("#tbldetail tbody").html(html);
                        $('#Status').trigger('change');
                        $('#GroupId').trigger('change');
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