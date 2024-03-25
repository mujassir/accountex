

var StockOB = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "StockOB";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.MapJsonData();
            $this.LoadProducts();
            $("#Account").change(function () {
                $("#" + DATATABLE_ID).dataTable().fnDestroy();
                $this.LoadProducts();
            });
        },
        BindEvents: function () {
            $(".number2").autoNumeric("init", { aSep: "", dGroup: "2", vMin: "0", });
            $(".number4").autoNumeric("init", { aSep: "", dGroup: "4", vMin: "-99999999999999999", });
            $("input[type='text'].debit,input[type='text'].Quantity,input[type='text'].UnitPrice").change(function () {
                $(this).next().text($(this).val());
            });
        },
        RebindData: function () {
            oTable.fnDraw();
        },
        MapJsonData: function () {
            $("#jsondata :input").each(function () {
                try {
                    Setting[$(this).attr("Id")] = $.parseJSON($(this).val());
                } catch (e) {
                    Setting[$(this).attr("Id")] = $(this).val();

                }

            });
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
            $("select", "#mainform").val("").each(function () {
                $(this).chosen();
            });
            //$('#AccountId').chosen();
            GetNextVoucherNumber();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            var $this = this;
            //$("#saleitem tbody .chooseninner").chosen();
            Common.AllowNumerics();
            //$(".select2").select2();
            $("tbody select").each(function () {
                $(this).select2();
            });
            $this.BindEvents();
            //SetDropDown();
        },

        Save: function () {
            var $this = this;
            $("#divMessage").hide();
            var items = new Array();
            var totalItems = new Array();
            var allowedqty = 0;
            var previousqty = 0;
            var err = "";
            $("#" + DATATABLE_ID + " tbody tr").each(function () {


                var qty = Common.GetFloat($(this).children(":nth-child(3)").children("input[type='text']").val());
                var unitPrice = Common.GetFloat($(this).children(":nth-child(4)").children("input[type='text']").val());
                items.push({
                    Id: $(this).children(":nth-child(5)").children("input.Id").val(),
                    AccountId: $(this).children(":nth-child(5)").children("input.AccountId").val(),
                    Quantity: qty,
                    UnitPrice: unitPrice,
                });

            });
            var record = new Object();

            record[''] = items;
            //console.log(JSON.stringify(record));
            //return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                success: function (res) {
                    if (res.Success) {
                        //window.location = "OpeningBalance";
                        // LoadOpeningBalances();
                        // $this.LoadProducts();
                        Common.ShowMessage(true, { message: "<strong>Successs!</strong> Changes saved successfully" });
                        $("#btnEdit,span.Quantity,span.UnitPrice").removeClass("hide");
                        $("#btnSave,input[type='text'].Quantity,input[type='text'].UnitPrice").addClass("hide");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        },
        //LoadOpeningBalances: function () {
        //    var url = Setting.APIBaseUrl + API_CONTROLLER;
        //    DataTable.BindDatatable(DATATABLE_ID, url);
        //    LIST_LOADED = true;

        //},

        LoadProducts: function () {

            var $this = this;
            var html = "";
            var select = "";
            var amount = 0;
            var account = $("#Account").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var products = res.Data;
                        for (var i in products) {
                            var p = products[i];
                            var qty = p.Quantity;

                            if (qty == "0") qty = "";

                            html += "<tr><td>" + p.AccountCode + "</td>";
                            html += "<td>" + p.AccountName + "</td>";
                            html += "<td> <input type='text' class='number4 form-control input-xsmall Quantity hide' value='" + (qty != null ? qty : "") + "' /><span class='Quantity'>" + (qty != null ? qty : "") + "</span></td>";
                            html += "<td> <input type='text' class='number2 form-control input-xsmall UnitPrice hide' value='" + p.UnitPrice + "' /><span class='Quantity'>" + p.UnitPrice + "</span></td>";
                            html += "<td class=hide><input type=hidden class='Id' value=" + p.Id + " /><input type=hidden class='AccountId' value=" + p.AccountId + " /></td>"
                            html + "</tr>";
                        }

                        $("#mainTable tbody").html(html);
                        //if (LIST_LOADED) {
                        //    //   oTable.fnDraw();

                        //}
                        //else {
                        $this.BindStaticDatatable(DATATABLE_ID);
                        //DataTable.BindDatatable(DATATABLE_ID, url);
                        LIST_LOADED = true;
                        $this.BindEvents();
                        //}
                        //ReinializePlugin();
                        $("#btnEdit,span.debit,span.Quantity,span.UnitPrice").removeClass("hide");
                        $("#btnSave,input[type='text'].Quantity,input[type='text'].UnitPrice").addClass("hide");

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        },
        Edit: function () {
            $("#btnEdit,span.Quantity,span.UnitPrice").addClass("hide");
            $("#btnSave,input[type='text'].Quantity,input[type='text'].UnitPrice").removeClass("hide");

        },
        BindStaticDatatable: function (tableId) {

            var $this = this;
            if (!jQuery().dataTable)
                return;
            $("#" + tableId).dataTable().fnDestroy();
            oTable = $("#" + tableId).dataTable({
                "aLengthMenu": [
                    [10, 20, 50, 100, 500],
                    [10, 20, 50, 100, 500] // change per page values here
                ],
                // set the initial value
                "bRetrieve": true,
                "bAutoWidth": false,   // disable fixed width and enable fluid table
                "bDestroy": true,
                //"bServerSide": true, // server side processing
                //"bProcessing": true, // enable/disable display message box on record load
                //"sAjaxSource": ajaxSource, // ajax source

                "iDisplayLength": 500,
                "oLanguage": {
                    "sProcessing": "<img src=\"../Content/App.assets/admin/layout/img/loading-spinner-grey.gif\"/><span>&nbsp;&nbsp;" + Setting.DatatableLoadingMessage + "</span>",
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
                    App.initUniform();
                    $this.ReinializePlugin();
                }
            });

            //oTable.fnSort([[1, 'desc']]);

            jQuery("#" + tableId + "_wrapper .dataTables_filter input").addClass("form-control input-medium input-inline"); // modify table search input
            jQuery("#" + tableId + "_wrapper .dataTables_length select").addClass("form-control input-xsmall"); // modify table per page dropdown
            jQuery("#" + tableId + "_wrapper .dataTables_length select").select2(); // initialize select2 dropdown
        },
    };
}();


