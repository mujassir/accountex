
var OrderListing = function () {
    var API_CONTROLLER = "Report";
    var DATATABLE_ID = "mainTable";
    return {
        init: function () {
           var $this = this;

            $("#btnShowReport").click(function () {

                $this.LoadData();
            });
            $this.LoadData();

        },
        LoadData: function () {

           var $this = this;
            var qs = "?key=GetOrderListing";
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
                        html += "";
                        for (var i in data) {
                            var od = data[i];
                            var bgcss = "#FFFFFF !important";
                            if (od.Type == "Order") {

                                html += "<tr  class='order'><td>" + data[i].OrderNo + "</td><td>" + data[i].OrderDate
                              + "</td><td></td><td>"
                              + "</td><td></td><td></td>"
                                + "<td>" + data[i].GPNo + "</td><td>" + data[i].Customer + "</td>"
                                 + "<td>" + data[i].NetTotal.format() + "</td><td>" + data[i].OrderStatus + "</td></tr>";

                            }
                            else if (od.Type == "DC") {
                                html += "<tr  class='dc'><td>" + data[i].OrderNo + "</td><td>" + data[i].OrderDate
                              + "</td><td>" + data[i].DCNo + "</td><td>" + data[i].DCDate
                              + "</td><td></td><td></td>"
                                + "<td>" + data[i].GPNo + "</td><td>" + data[i].Customer + "</td>"
                                 + "<td>" + data[i].NetTotal.format() + "</td><td>" + data[i].OrderStatus + "</td></tr>";
                            }
                            else if (od.Type == "Sale") {
                                html += "<tr  class='invoice'><td>" + data[i].OrderNo + "</td><td>" + data[i].OrderDate
                              + "</td><td>" + data[i].DCNo + "</td><td>" + data[i].DCDate
                              + "</td><td>" + data[i].SaleNo + "</td><td>" + data[i].SaleDate + "</td>"
                                + "<td>" + data[i].GPNo + "</td><td>" + data[i].Customer + "</td>"
                                 + "<td>" + data[i].NetTotal.format() + "</td><td>" + data[i].OrderStatus + "</td></tr>";
                            }


                        }
                        //if (res.Data.length == 0)
                        //    html += "  <tr><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";

                        $(".report-table tbody").html(html);
                        //$this.BindStaticDatatable(DATATABLE_ID);
                        //$(".report-table tfoot").html(html);
                        //$(".opening-balance").html(res.Data.OpeningBalance);
                        //$(".total-debit").html(res.Data.TotalDebit);
                        //$(".total-credit").html(res.Data.TotalCredit);
                        //$(".total-balance").html(res.Data.TotalBalance);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
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