
var OrderByStatus = function () {
    var API_CONTROLLER = "Report";
    var DATATABLE_ID = "mainTable";
    var VOUCHER_TYPE = 0;
    return {
        init: function () {
            var $this = this;

            $("#btnShowReport").click(function () {
                $(".nav-tabs li a[href='#portlet_tab1']").click();
                $this.LoadData();
            });
            $("#VoucherType").change(function () {
                var vouchertype = $(this).val();
                if (vouchertype == VoucherType.saleorder) {
                    $(".tab-pane .table th:nth-child(1)").html("Customer");
                }
                else {
                    $(".tab-pane .table th:nth-child(1)").html("Supplier");
                }
                $("#btnShowReport").trigger('click');
            });
            //

        },
        LoadData: function () {

            var $this = this;
            if (Common.Validate("#form-info")) {
                var date1 = $("#FromDate").val();
                var date2 = $("#ToDate").val();
                VOUCHER_TYPE = $("#VoucherType").val();
                $("#lblReportDate").html("Date: " + date1 + " to " + date2);
                var qs = "?key=GetOrderByStatus";
                qs += "&date1=" + date1;
                qs += "&date2=" + date2;
                qs += "&voucherType=" + VOUCHER_TYPE;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Get",
                    data: "",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Loading orders...please wait",
                    success: function (res) {
                        if (res.Success) {
                            var date = moment().format("YYYY-MM-DD");

                            var delivered = Enumerable.From(res.Data).Where(function (x) {
                                return x.Status == TransactionStatus.Delivered
                            }).ToArray();
                            $this.MapTableData(delivered, "#tbl-delivered");

                            var partiallydelivered = Enumerable.From(res.Data).Where(function (x) {
                                return x.Status == TransactionStatus.PartialyDelivered
                                    && moment(x.DeliveryDate).format("YYYY-MM-DD") >= date
                            }).ToArray();
                            $this.MapTableData(partiallydelivered, "#tbl-partiallydelivered");

                            var inprogressorders = Enumerable.From(res.Data).Where(function (x) {
                                return (x.Status == TransactionStatus.PendingOrder || x.Status == TransactionStatus.PendingProduction)
                                    && moment(x.DeliveryDate).format("YYYY-MM-DD") >= date
                            }).ToArray();
                            $this.MapTableData(inprogressorders, "#tbl-inprogress");

                            var lateorders = Enumerable.From(res.Data).Where(function (x) {
                                return x.Status != TransactionStatus.Delivered
                                    && moment(x.DeliveryDate).format("YYYY-MM-DD") < date
                            }).ToArray();
                            $this.MapTableData(lateorders, "#tbl-late");

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
        MapTableData: function (data, tableid) {
            var $this = this;
            $(tableid + " tbody").html("");
            var html = "";
            var orders = Enumerable.From(data).GroupBy("$.OrderId", null,
                function (key, g) {
                    var result = {
                        OrderId: key,
                        Order: g.FirstOrDefault(),
                        DeliveryChallans: g.ToArray(),
                    }

                    return result;
                }).ToArray();
            var netTotal = Enumerable.From(data).Sum("$.NetTotal");
            var type = VOUCHER_TYPE == VoucherType.saleorder ? "SaleOrder" : "PurchaseOrder";
            for (var j in orders) {
                var order = orders[j];
                var account = Common.GetById(order.Order.AccountId);
                var deliverychallans = order.DeliveryChallans;

                var remainingBalance = 0;
                html += "<tr style='background-color:skyblue' class='bold grand-total'>";
                html += "<td>" + (account != null ? account.Name : '') + "</td>";
                html += "<td><a target='_blank' href='" + Common.BaseUrl + "transaction/OrderBooking?type=" + type + "&voucherno=" + order.Order.VoucherNumber + "'>" + order.Order.VoucherNumber + "</a></td>";
                html += "<td >" + (!Common.isNullOrWhiteSpace(order.Order.PartyPONumber) ? order.Order.PartyPONumber : "") + "</td>";
                html += "<td>" + moment(order.Order.DeliveryDate).format("DD/MM/YYYY") + "</td>";
                html += "<td class='align-right' >" + Common.GetInt(order.Order.OrderTotalQuantity).format() + "</td>";
                html += "<td colspan='5'></td>";
                html += "</tr>";
                var tOrderQty = Common.GetInt(order.Order.OrderTotalQuantity);
                for (var k in deliverychallans) {
                    var item = deliverychallans[k];
                    var delivered = Common.GetInt(item.DCQuantity);

                    if (k == 0)
                        remainingBalance = tOrderQty - delivered;
                    else
                        remainingBalance = remainingBalance - delivered

                    if (item.DcDate != null && item.DcDate.trim() != "")
                        item.DcDate = moment(item.DcDate).format("DD/MM/YYYY")
                    else {
                        item.DcDate = "";
                    }
                    html += "<tr>";
                    html += "<td colspan='5'></td>";
                    html += "<td>" + (item.DcNo != null ? item.DcNo : "") + "</td>";
                    html += "<td>" + item.DcDate + "</td>";

                    html += "<td class='align-right'>" + Common.GetInt(item.DCQuantity).format() + "</td>";
                    html += "<td class='align-right'>" + remainingBalance.format() + "</td><td></td></tr>";
                }
                var totaldelivered = Common.GetInt(Enumerable.From(deliverychallans).Sum("$.DCQuantity"));
                html += "<tr class='bold subtotal'><td colspan=6></td>";
                html += "<td class='align-right'>Order Total</td><td class='align-right'>" + totaldelivered.format() + "</td><td></td><td class='align-right'>" + order.Order.NetTotal.format() + "</td></tr>";
            }
            html += "<tr class='bold grand-total'><td colspan=8></td>";
            html += "<td class='align-right'>Grand Total</td><td class='align-right'>" + netTotal.format() + "</td></tr>";
            $(tableid + " tbody").html(html);
        },
        BindStaticDatatable: function (tableId) {

            var $this = this;
            if (!jQuery().dataTable)
                return;
            $("#" + tableId).dataTable().fnDestroy();
            oTable = $("#" + tableId).dataTable({
                "aLengthMenu": [
                    [10, 20, 50, 100],
                    [10, 20, 50, 100] // change per page values here
                ],
                // set the initial value
                "bRetrieve": true,
                "bAutoWidth": false,
                "paging": false,
                // disable fixed width and enable fluid table
                //"bServerSide": true, // server side processing
                //"bProcessing": true, // enable/disable display message box on record load
                //"sAjaxSource": ajaxSource, // ajax source

                "iDisplayLength": 100,
                "oLanguage": {
                    "sProcessing": "<img src=\"../Content/metronic/assets/admin/layout/img/loading-spinner-grey.gif\"/><span>&nbsp;&nbsp;" + Setting.DatatableLoadingMessage + "</span>",
                    "sLengthMenu": "_MENU_ records",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aaSorting": [[0, "asc"]],
                "aoColumnDefs": [
                     { 'bSortable': false, "aTargets": ["no-sort"] },
                     { "bSearchable": true, "aTargets": [0] }
                ],
                "fnDrawCallback": function () {

                }
            });

            //oTable.fnSort([[1, 'desc']]);

            jQuery("#" + tableId + "_wrapper .dataTables_filter input").addClass("form-control input-medium input-inline"); // modify table search input
            jQuery("#" + tableId + "_wrapper .dataTables_length select").addClass("form-control input-xsmall"); // modify table per page dropdown
            jQuery("#" + tableId + "_wrapper .dataTables_length select").select2(); // initialize select2 dropdown
        },
    };
}();