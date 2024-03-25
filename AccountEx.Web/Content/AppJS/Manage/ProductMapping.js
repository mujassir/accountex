

var ProductMapping = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "ProductMapping";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.MapJsonData();
            //$this.LoadProductMappings();
            $("#Account").change(function () {
                $("#" + DATATABLE_ID).dataTable().fnDestroy();
                var account = $("#Account").val();
                if (account)
                    $this.LoadProductMappings(account)
                else
                    $("#" + DATATABLE_ID + " tbody").html('')
            });
        },
        BindEvents: function () {
            $("input[type='text'].debit,input[type='text'].ManualCode").change(function () {
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
            //$("#saleitem tbody .chooseninner").chosen();
            Common.AllowNumerics();
            //$(".select2").select2();
            $("tbody select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },

        Save: function () {
            var $this = this;
            $("#divMessage").hide();
            var items = new Array();
            var totalItems = new Array();
            var allowedqty = 0;
            var previousqty = 0;
            var customerId = Common.GetInt($("#Account").val());
            if (customerId == 0 && !confirm('This will update product mappings for all customers.Are you sure to continue?'))
                return;
            var err = "";
            $("#" + DATATABLE_ID + " tbody tr").each(function () {


                var manualcode = $(this).children(":nth-child(3)").children("input[type='text']").val();

                var type = $(this).children(":nth-child(3)").children("select").val();
                items.push({
                    Id: $(this).children(":nth-child(4)").children("input.Id").val(),
                    ProductId: $(this).children(":nth-child(1)").children("input.ProductId").val(),
                    ProductTitle: $(this).children(":nth-child(1)").text(),
                    ProductCode: $(this).children(":nth-child(2)").text(),
                    CustomerId: $("#Account").val(),
                    CustomerTitle: $("#Account option:selected").text(),
                    COAProductId: $(this).children(":nth-child(4)").children("input.COAProductId").val(),
                    ManualCode: manualcode,


                });

            });
            record = new Object();

            record["ProductMappings"] = items;
            //console.log(JSON.stringify(record));
            //return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Saving customer Product Mappings...please wait",
                success: function (res) {
                    if (res.Success) {
                        //window.location = "OpeningBalance";
                        //LoadOpeningBalances();
                        $("#divMessage").show();
                        $("#divMessage").fadeOut(5000);
                        // Common.ShowMessage(true, { message: "Record saved successfully." });
                        $("#btnEdit,span.ManualCode").removeClass("hide");
                        $("#btnSave,input[type='text'].ManualCode").addClass("hide");
                        $("#Account").trigger("change");
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
        LoadProductMappings: function (account) {

            var $this = this;
            var html = "";
            var select = "";
            var amount = 0;
            //var account = $("#Account").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?account=" + account,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var productMappings = res.Data;
                        for (var i in productMappings) {
                            var productMapping = productMappings[i];
                            var manualCode = productMapping.ManualCode;

                            if (manualCode == null) manualCode = "";

                            html += "<tr><td><input type=hidden class='ProductId' value=" + productMapping.ProductId + " />" + productMapping.ProductTitle + "</td><td>" + productMapping.ProductCode + "</td><td> <input type='text' class=' form-control input-xsmall ManualCode hide' value='"
                                + manualCode + "' /><span class='ManualCode'>" + manualCode + "</span></td><td class=hide><input type=hidden class='Id' value=" + productMapping.Id + " /><input type=hidden class='CustomerId' value=" + productMapping.CustomerId + " /><input type=hidden class='CustomerTitle' value=" + productMapping.CustomerTitle + " /><input type=hidden class='COAProductId' value=" + productMapping.COAProductId + " /></td></tr>";
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
                        $("#btnEdit,span.debit,span.credit").removeClass("hide");
                        $("#btnSave,input[type='text'].debit,input[type='text'].credit").addClass("hide");

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
            $("#btnEdit,span.ManualCode").addClass("hide");
            $("#btnSave,input[type='text'].ManualCode").removeClass("hide");

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
                "bAutoWidth": false,   // disable fixed width and enable fluid table
                "paging": false,
                //"bServerSide": true, // server side processing
                //"bProcessing": true, // enable/disable display message box on record load
                //"sAjaxSource": ajaxSource, // ajax source

                "iDisplayLength": 100,
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


