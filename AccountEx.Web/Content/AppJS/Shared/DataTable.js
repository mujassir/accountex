

var DataTable = function () {
    var DataTables = new Array();
    var IS_LOADED = false;
    var sEcho = 1;
    return {
        init: function () {
            var $this = this;
        },
        RefreshDatatable: function (tableId) {
            DataTables[tableId].fnDraw();
        },
        RefreshDatatableUrl: function (tableId, url) {
            DataTables[tableId].fnReloadAjax(url);
        },
        AdjustColumnSizing: function (tableId) {
            DataTables[tableId].fnAdjustColumnSizing(false);
        },
        BindDatatable: function (tableId, ajaxSource, newoption, callback, callback1) {

            var $this = this;
            if (!jQuery().dataTable)
                return;
            //if (localStorage.getItem("datatable")) {
            //    var dt = JSON.parse(localStorage.getItem("datatable"));
            //    $('#' + tableId).dataTable({data:dt.aaData});
            //    return;
            //}

            var options = {
                "searchDelay": "1000",
                "aLengthMenu": [
                    [10, 20, 50, 100, 30000],
                    [10, 20, 50, 100, "All"] // change per page values here
                ],
                // set the initial value
                "bAutoWidth": false,   // disable fixed width and enable fluid table

                "bServerSide": true, // server side processing
                "bProcessing": false, // enable/disable display message box on record load
                "sAjaxSource": ajaxSource, // ajax source
                //dom: 'Blfrtip',
                //buttons: [
                //{ extend: 'csv', text: '<button type="button" class="btn green btn-sm"><i class="fa fa fa-file-o" aria-hidden="true"></i>&nbsp;CSV Export</button>' }
                //],
                "iDisplayLength": Setting.DatatablePageSize,
                "oLanguage": {
                    "sProcessing": "<img src=\"../Content/themes/loading-spinner-grey.gif\"/><span>&nbsp;&nbsp;" + Setting.DatatableLoadingMessage + "</span>",
                    "sLengthMenu": "_MENU_ records",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    },
                    //"sProcessing": "Loading Records...please wait"
                },
                "aaSorting": [[1, "desc"]],
                "aoColumnDefs": [
                     { 'bSortable': false, "aTargets": ["no-sort"] },
                     { "bSearchable": false, "aTargets": [0] }
                ],
                "fnDrawCallback": function () {
                    App.initUniform();
                    if (typeof options.checkAccess == 'undefined' || options.checkAccess)
                        Common.ApplyActions();
                    if (callback != undefined) callback();


                },
                "fnInitComplete": function () {
                    console.log("Compelete");
                    //$("input").attr("autocomplete", "off");
                    //$("input[type='search']").val('');
                    //alert( $("input[type='search']").val());
                    if (callback1 != undefined) callback1();
                    //var $buttons = $('.dt-buttons').hide();
                    //$('ul.data-table-export-buttons a').on('click', function () {
                    //    var btnClass = ".buttons-" + $(this).attr("data-export-type");
                    //    if (btnClass) $buttons.find(btnClass).click();
                    //})
                },
                //"fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                //    oSettings.jqXHR = $.ajax({
                //        "dataType": 'json',
                //        "type": "GET",
                //        "url": sSource,
                //        "success": function (result) {
                //            //alert(result.sEcho);
                //            fnCallback;
                //        },
                //        "error": function (result) {
                //            alert("errro");
                //        },
                //        "failure": function (result) {
                //            alert("failure");
                //        }
                //    });
                //},
                "fnPreDrawCallback": function (oSettings) {
                    //alert("json: " + localStorage.getItem("datatable"));
                    //if (localStorage.getItem("datatable")) {

                    //    o JSON.parse(localStorage.getItem("datatable"));
                    //    return false;
                    //}
                    ////return false;
                },
                //"fnInitComplete": function (oSettings, json) {
                //    alert("d");
                //    localStorage.setItem("datatable_" + document.location.pathname, JSON.stringify(json));
                //},
                //"fnDrawCallback": function (oSettings, json) {
                //    alert( 'DataTables has redrawn the table' );
                //    debugger
                //},
                //"bStateSave": true,
                //"fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                //    var key = "datatable_" + document.location.href;
                //    var json = JSON.parse(localStorage.getItem(key));
                //    if (json != null && json != undefined && json != "") {
                //        json.sEcho = $this.sEcho++;
                //        fnCallback(json);
                //    }
                //    else {
                //        var url = oSettings.ajax;
                //        if (url == null || url == undefined || url == "") url = sSource;
                //        oSettings.jqXHR = Common.WrapAjax({
                //            "type": "GET",
                //            "url": url,
                //            "data": aoData,
                //            "success": function (json) {
                //                json.sEcho = $this.sEcho++;
                //                localStorage.setItem(key, JSON.stringify(json));
                //                fnCallback(json);
                //            }
                //        });
                //    }
                //},
                //"fnStateLoad": function (oSettings) {
                //    alert("wait");
                //    return JSON.stringify(json);
                //}

            };
            if (typeof newoption != "undefined" && newoption != undefined && newoption != "")
                $.extend(true, options, newoption);

            var oTable = $("#" + tableId).dataTable(options);

            //oTable.fnSort([[1, 'desc']]);
            DataTables[tableId] = oTable;
            jQuery("#" + tableId + " .group-checkable").change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery("#" + tableId + "_wrapper .dataTables_filter input").addClass("form-control input-medium input-inline"); // modify table search input
            jQuery("#" + tableId + "_wrapper .dataTables_length select").addClass("form-control input-xsmall"); // modify table per page dropdown
            jQuery("#" + tableId + "_wrapper .dataTables_length select").select2(); // initialize select2 dropdown
        },
        BindReportDatatable: function (tableId, ajaxSource) {
            var $this = this;
            if (!jQuery().dataTable)
                return;
            var oTable = $("#" + tableId).dataTable({
                'sDom': "t",
                // set the initial value
                "bAutoWidth": false,   // disable fixed width and enable fluid table
                "bServerSide": true, // server side processing
                // "bProcessing": true, // enable/disable display message box on record load
                "sAjaxSource": ajaxSource, // ajax source
                //"bProcessing": true, // enable/disable display message box on record load

                "iDisplayLength": 60000,
                "aoColumnDefs": [
                    { 'bSortable': false, "aTargets": ["no-sort"] },
                    { "bSearchable": false, "aTargets": ["sort"] }
                ],

            });

            //oTable.fnSort([[1, 'desc']]);
            DataTables[tableId] = oTable;
            jQuery("#" + tableId + " .group-checkable").change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery("#" + tableId + "_wrapper .dataTables_filter input").addClass("form-control input-medium input-inline"); // modify table search input
            jQuery("#" + tableId + "_wrapper .dataTables_length select").addClass("form-control input-xsmall"); // modify table per page dropdown
            // jQuery("#" + tableId + "_wrapper .dataTables_length select").select2(); // initialize select2 dropdown

        },
        DestroyDatatable: function (tableId) {
            if (DataTables[tableId])
                DataTables[tableId].fnDestroy();
        },
        BindReportWithFooterDatatable: function (tableId, ajaxSource) {
            var $this = this;
            if (!jQuery().dataTable)
                return;
            var oTable = $("#" + tableId).dataTable({
                'sDom': "t",
                // set the initial value
                "bAutoWidth": false,   // disable fixed width and enable fluid table
                "bServerSide": true, // server side processing
                // "bProcessing": true, // enable/disable display message box on record load
                "sAjaxSource": ajaxSource, // ajax source
                //"bProcessing": true, // enable/disable display message box on record load

                "iDisplayLength": 60000,
                "aoColumnDefs": [
                    { 'bSortable': false, "aTargets": ["no-sort"] },
                    { "bSearchable": false, "aTargets": ["sort"] }
                ],
                "footerCallback": function (row, data, start, end, display) {
                    var api = this.api(), data;

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    // Total over this page
                    col1total = api
                        .column(5, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    col2total = api
                        .column(6, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    col3total = api
                        .column(7, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    // Update footer
                    $(api.column(5).footer()).html(col1total.format());
                    $(api.column(6).footer()).html(col2total.format());
                    $(api.column(7).footer()).html(col3total.format());
                }
            });

            //oTable.fnSort([[1, 'desc']]);
            DataTables[tableId] = oTable;
            jQuery("#" + tableId + " .group-checkable").change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery("#" + tableId + "_wrapper .dataTables_filter input").addClass("form-control input-medium input-inline"); // modify table search input
            jQuery("#" + tableId + "_wrapper .dataTables_length select").addClass("form-control input-xsmall"); // modify table per page dropdown
            jQuery("#" + tableId + "_wrapper .dataTables_length select").select2(); // initialize select2 dropdown

        },
        RowGroup: function (tableId, index) {
            DataTables[tableId].rowGrouping({
                iGroupingColumnIndex: index,
                bExpandableGrouping: true,
                sGroupingColumnSortDirection: "asc",
                iGroupingOrderByColumnIndex: 0
            });
        },
        RowGroup2: function (tableId, index, index2) {
            DataTables[tableId].rowGrouping({
                iGroupingColumnIndex: index,
                iGroupingColumnIndex2: index2,
                bExpandableGrouping: true,
                sGroupingColumnSortDirection: "asc",
                iGroupingOrderByColumnIndex: 0
            });
        },
    };
}();





/*                               Override functions                                                */

$.fn.dataTableExt.sErrMode = "throw";
jQuery.fn.dataTableExt.oApi.fnReloadAjax = function (oSettings, sNewSource, fnCallback, bStandingRedraw) {
    // DataTables 1.10 compatibility - if 1.10 then `versionCheck` exists.
    // 1.10's API has ajax reloading built in, so we use those abilities
    // directly.
    if (jQuery.fn.dataTable.versionCheck) {
        var api = new jQuery.fn.dataTable.Api(oSettings);

        if (sNewSource) {
            api.ajax.url(sNewSource).load(fnCallback, !bStandingRedraw);
        }
        else {
            api.ajax.reload(fnCallback, !bStandingRedraw);
        }
        return;
    }

    if (sNewSource !== undefined && sNewSource !== null) {
        oSettings.sAjaxSource = sNewSource;
    }

    // Server-side processing should just call fnDraw
    if (oSettings.oFeatures.bServerSide) {
        this.fnDraw();
        return;
    }

    this.oApi._fnProcessingDisplay(oSettings, true);
    var that = this;
    var iStart = oSettings._iDisplayStart;
    var aData = [];

    this.oApi._fnServerParams(oSettings, aData);

    oSettings.fnServerData.call(oSettings.oInstance, oSettings.sAjaxSource, aData, function (json) {
        /* Clear the old information from the table */
        that.oApi._fnClearTable(oSettings);

        /* Got the data - add it to the table */
        var aData = (oSettings.sAjaxDataProp !== "") ?
            that.oApi._fnGetObjectDataFn(oSettings.sAjaxDataProp)(json) : json;

        for (var i = 0 ; i < aData.length ; i++) {
            that.oApi._fnAddData(oSettings, aData[i]);
        }

        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();

        that.fnDraw();

        if (bStandingRedraw === true) {
            oSettings._iDisplayStart = iStart;
            that.oApi._fnCalculateEnd(oSettings);
            that.fnDraw(false);
        }

        that.oApi._fnProcessingDisplay(oSettings, false);

        /* Callback user function - for event handlers etc */
        if (typeof fnCallback == "function" && fnCallback !== null) {
            fnCallback(oSettings);
        }
    }, oSettings);
};
$.fn.DataTable.ext.oApi._fnLog = function (settings, level, msg, tn) {
    msg = "Error:" + msg + ".An Error Occured while getting data from server.";
    if (settings.jqXHR.status != 307) {
        Common.ShowError(msg);
    }
};