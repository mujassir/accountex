

var OpeningBalance = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "OpeningBalance";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.MapJsonData();
            $this.LoadOpeningBalances();
            $("#Account").change(function () {
                $("#mainTable").dataTable().fnDestroy();
                $this.LoadOpeningBalances();
            });
        },
        BindEvents: function () {
            //$("input[type='text'].debit,input[type='text'].credit").change(function () {
            //    var amount = $(this).val();
            //    $(this).next().text(amount != "" ? Common.GetFloat(amount).format() : "");
            //});
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
            var Items = new Array();
            var TotalItems = new Array();
            var allowedqty = 0;
            var previousqty = 0;
            var err = "";
            $("#" + DATATABLE_ID + " tbody tr").each(function () {

                var debit = Common.GetFloat($(this).children(":nth-child(3)").children("input[type='text']").val());
                var credit = Common.GetFloat($(this).children(":nth-child(4)").children("input[type='text']").val());
                var amount = debit - credit;
                //if (amount != 0) {

                var type = $(this).children(":nth-child(5)").children("select").val();
                Items.push({
                    InvoiceNumber: 0,
                    VoucherNumber: 0,
                    Quantity: 0,
                    Price: 0,
                    Id: $(this).children(":nth-child(5)").children("input[type='hidden']").val(),
                    TransactionType: VoucherType[Setting.Voucher],
                    AccountId: $(this).children(":nth-child(1)").children("input[type='hidden']").val(),
                    AccountTitle: $(this).children(":nth-child(1)").text(),
                    Credit: amount < 0 ? amount * -1 : 0,
                    Debit: amount > 0 ? amount : 0,

                });



                //}
            });

            record = new Object();

            record["Items"] = Items;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                success: function (res) {
                    if (res.Success) {
                        //window.location = "OpeningBalance";
                        $this.LoadOpeningBalances();
                        Common.ShowMessage(true, { message: "Changes Saved Succsessfully." });

                        //$("#btnEdit,span.debit,span.credit").removeClass("hide");
                        //$("#btnSave,#btnCancel,input[type='text'].debit,input[type='text'].credit").addClass("hide");
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
        LoadOpeningBalances: function () {

            var $this = this;
            var html = "";
            var select = "";
            var amount = 0;
            var account = $("#Account").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?account=" + account,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var OpeningBalances = res.Data;
                        for (var i in OpeningBalances) {
                            var debit = OpeningBalances[i].Debit;
                            var credit = OpeningBalances[i].Credit;
                            if (debit == "0") debit = "";
                            if (credit == "0") credit = "";
                            var tooltip = "";
                            if (OpeningBalances[i].TransactionType == 29)
                                tooltip = "data-toggle='tooltip' title='Automatic opening balance from last fiscal year (read only)'";
                            html += "<tr data-transaction-type='" + OpeningBalances[i].TransactionType + "' " + tooltip + "><td><input type=hidden value=" + OpeningBalances[i].AccountId + " />" + OpeningBalances[i].AccountCode + "</td><td><a href='../reports/general-ledger?accountId=" + OpeningBalances[i].AccountId + "'>" + OpeningBalances[i].AccountTitle + "</a></td><td class='align-right'> <input type='text' class='number form-control input-xsmall debit hide' value='"
                                + debit + "' /><span class='debit'>" + (debit != "" ? debit.format(3) : debit) + "</span></td><td class='align-right'><input type=text class='number form-control input-xsmall credit hide' value='"
                                + credit + "' /><span class='credit'>" + (credit != "" ? credit.format(3) : credit) + "</span></td><td class=hide><input type=hidden value=" + OpeningBalances[i].Id + " /></td></tr>";
                        }
                        //  var debittotal = Enumerable.From(OpeningBalances).Sum("$.Debit");
                        //var debittotal = Enumerable.From(OpeningBalances).Sum("$.Credit");

                        $("#" + DATATABLE_ID + " tbody tr").each(function () {

                            var debit = Common.GetFloat($(this).children(":nth-child(3)").children("input[type='text']").val());
                            var credit = Common.GetFloat($(this).children(":nth-child(4)").children("input[type='text']").val());
                        });
                        $("#mainTable tbody").html(html);
                        var debittotal = $("#" + DATATABLE_ID + " tbody tr :nth-child(3) input[type='text']").toEnumerable().Select("Common.GetInt($.val())").Sum();
                        var credittotal = $("#" + DATATABLE_ID + " tbody tr :nth-child(4) input[type='text']").toEnumerable().Select("Common.GetInt($.val())").Sum();
                        html = "<tr><td colspan='2' class='align-right'><b>Total</b></td><td><span>" + debittotal.format() + "</span></td><td><span>" + credittotal.format() + "</span></td></tr>";
                        $("#mainTable tfoot").html(html);


                        //DataTable.BindDatatable(DATATABLE_ID, url);
                        LIST_LOADED = true;
                        $this.BindEvents();

                        var debittotal = $("#" + DATATABLE_ID + " tbody tr :nth-child(3) input[type='text']").toEnumerable().Select("Common.GetInt($.val())").Sum();
                        var credittotal = $("#" + DATATABLE_ID + " tbody tr :nth-child(4) input[type='text']").toEnumerable().Select("Common.GetInt($.val())").Sum();
                        html = "<tr  class='bold'><td colspan='2' class='align-right'><b>Total</b></td><td class='align-right'><span>" + debittotal.format() + "</span></td><td class='align-right'><span>" + credittotal.format() + "</span></td></tr>";
                        $("#mainTable tfoot").html(html);
                        $("#mainTable").dataTable().fnDestroy();
                        //$this.BindStaticDatatable(DATATABLE_ID, false);
                        $("#btnEdit,span.debit,span.credit").removeClass("hide");
                        $("#btnSave,input[type='text'].debit,input[type='text'].credit,#btnCancel").addClass("hide"); $("table#mainTable tr[data-transaction-type='29']").find("input[type='text'].debit,input[type='text'].credit").prop("disabled", true);
                      
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        },
        Cancel: function () {
            var $this = this;
            $this.LoadOpeningBalances();
            $("#btnEdit,span.debit,span.credit").removeClass("hide");
            $("#btnSave,#btnCancel,input[type='text'].debit,input[type='text'].credit").addClass("hide");
        },
        Edit: function () {
            $("#btnEdit,span.debit,span.credit").addClass("hide");
            $("#btnSave,#btnCancel,input[type='text'].debit,input[type='text'].credit").removeClass("hide");
            $("table#mainTable tr[data-transaction-type='29']").find("input[type='text'].debit,input[type='text'].credit").prop("disabled", true);
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
                //"bRetrieve": true,
                "bAutoWidth": false,
                //"bDestroy": true,
                "paging": false,
                // disable fixed width and enable fluid table
                //"bServerSide": true, // server side processing
                //"bProcessing": true, // enable/disable display message box on record load
                //"sAjaxSource": ajaxSource, // ajax source

                "iDisplayLength": 100,
                "oLanguage": {
                    "sProcessing": "<img src=\"../Content/assets/layout/img/loading-spinner-grey.gif\"/><span>&nbsp;&nbsp;" + Setting.DatatableLoadingMessage + "</span>",
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


