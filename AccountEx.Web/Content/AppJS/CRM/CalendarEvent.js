
var CalendarEvent = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CalendarEvent";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var PageData = new Object();
    return {
        init: function () {
            var $this = this;
            $(document).on("click", "#btn-save", function (event) {
                $this.Save();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem();
            });
            $(document).on("click", "#btn-repeate-item", function (event) {
                $this.RepeateItem($(this));
            });
            $('#Date').datepicker().on('changeDate', function (e) {
                $this.LoadEvents();
            });
            $(document).on("change", "select.CustomerId", function (event) {
                $this.LoadProjects($(this));
            });

            $(document).on("click", "a#link-with-gc", function (event) {
                window.location = '../GoogleCalendarIntegration/Calendar';
                return false;
            });
            $(document).on("click", "a#sync-with-gc", function (event) {
                $this.SyncGoogleCalendarEvents();
            });

            



            $(document).on("keyup", "input.Project", function (event) {
                var tr = $(this).closest("tr");
                if ($(this).val().trim() == "") {
                    $(tr).find("input.ProjectId").val('');
                    $(tr).find("input.Product").prop("disabled", false);
                }
                else {
                    $(tr).find("input.Product").prop("disabled", true);
                }
            });
            $(document).on("keyup", "input.Product", function (event) {
                var tr = $(this).closest("tr");
                if ($(this).val().trim() != "") {

                    $(tr).find("input.ProjectId").val('');
                    $(tr).find("input.Project").val('').prop("disabled", true);
                }
                else {
                    $(tr).find("input.ProjectId").val('');
                    $(tr).find("input.Project").prop("disabled", false);
                }
            });
            $("#btnSearch").click(function () {
                $this.Filter();
            });
            $("#FilterFiscalId").change(function () {
                $this.Filter();
            });


            $this.CustomClear();
            $this.LoadProducts();
            $this.ListView();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
            $this.AddItem();
        },
        DetailView: function () {
            Common.GoToTop();
        },

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
        },
        Filter: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            var fiscalId = $("#FilterFiscalId").val();
            url += "?fiscalId=" + fiscalId
            url += Common.MakeQueryStringAll($("#filters-container"));
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            Common.Clear();
        },
        AddItem: function ($repeateRow) {

            var $this = this;

            var $tr = $("#item-container tbody tr:nth-last-child(1)");
            var voucherNo = 0;
            if ($tr.length > 0) {
                voucherNo = Common.GetInt($tr.find("input.VisitNo").val()) + 1;
                $this.AddItemHTML(voucherNo, $repeateRow);
            }
            else {

                var voucherno = Common.GetInt($("#VoucherNumber").val());
                //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetNextVoucherNumber",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Processing...please wait",
                    success: function (res) {

                        if (res.Success) {
                            voucherNo = res.Data;
                            $this.AddItemHTML(voucherNo, $repeateRow);

                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                        Common.ShowError(e.responseText);
                    }
                });
            }
        },
        AddItemHTML: function (voucherNo, $repeateRow) {
            var $this = this;
            //  if (Common.Validate($("#addrow"))) {
            var $lastTr = $("#item-container tbody tr:nth-last-child(1)");
            var code = $lastTr.find("input.VisitNo").val();
            var lastEndTime = $lastTr.find("input.EndTime").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.VisitNo").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.VisitNo").focus().select();
                }, 300);
                focusElement = "";
            }
            Common.InitNumerics();
            Common.SetCheckChange();
            $("#item-container tbody tr:nth-last-child(1) select.select2").each(function () {
                Common.BindSelect2($(this));
            });
            var $tr = $("#item-container tbody tr:nth-last-child(1)");
            $tr.find("input.VisitNo").val(voucherNo);
            if ($repeateRow != undefined && $repeateRow != null) {
                var startTime = $repeateRow.find("input.StartTime").val();
                var endTime = $repeateRow.find("input.EndTime").val();
                var customerId = $repeateRow.find("select.CustomerId").val();
                var modeId = $repeateRow.find("select.ModeId").val();
                $tr.find("input.StartTime").val(startTime);
                $tr.find("input.EndTime").val(endTime);
                $tr.find("select.CustomerId").select2("val", customerId).trigger("change");
                $tr.find("select.ModeId").select2("val", modeId);
            }
            else {
                $tr.find("input.StartTime").val(lastEndTime);
            }

            Common.InitTimePicker();
            App.initUniform();
            $this.AutoCompleteInit();
        },
        DeleteRow: function (element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            var id = Common.GetInt($($tr).find("input.Id").val());
            if (id > 0) {
                $this.Delete(id, $tr);

            }

            else {
                $tr.remove();
            }

        },
        RepeateItem: function ($element) {

            var $this = this;
            var $tr = $element.closest("tr");
            $this.AddItem($tr);
        },
        Save: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {

                Items = Common.SaveItemData();
                //Items = Enumerable.From(Items).Where("$.ProductName.trim()!=''").ToArray();
                var err = "";
                //if (typeof party == "undefined" || party == null) {
                //    err += "" + $("#AccountCode").val() + " is not valid party code.,";
                //}

                //for (var i in Items) {
                //    var item = Items[i];
                //    if (item.Quantity <= 0) {
                //        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).,";
                //    }

                //    var product = Common.GetByCode(item.ItemCode);
                //    if (typeof product == "undefined" || product == null) {
                //        err += "" + item.ItemCode + " is not valid code.,";
                //    }

                //}
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                //if (Common.GetInt(record.NetTotal) <= 0) {
                //    err += "Transaction total amount should be graeter than zero(0).,";
                //}
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record =
                    {
                        '': Items
                    };
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $("#item-container tbody").html("");
                            $this.CustomClear();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        SyncGoogleCalendarEvents: function () {

            var $this = this;
            var scope = $("#form-info");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?syncGoogleCalendarEvents=true",
                type: "POST",
                data: '',
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "syncing...please wait",
                success: function (res) {
                    if (res.Success) {
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.LoadEvents();
                        Common.ShowMessage(true, { message: "Syncing completed successfully." });
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },

        LoadEvents: function () {
            var $this = this;
            var id = 0;
            var date = $("#Date").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?date=" + date,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var events = res.Data;
                        Common.MapItemData(events);
                        Common.InitNumerics();
                        Common.SetCheckChange();
                        $("#item-container tbody tr select.select2").each(function () {
                            Common.BindSelect2($(this));
                        });
                        Common.InitTimePicker();
                        App.initUniform();
                        $this.AutoCompleteInit();
                        if (events.length == 0)
                            $this.AddItem();
                        Common.SetPageAccess();
                        $("input.Project,input.Product").trigger("keyup");
                        $("select.CustomerId").trigger("change");


                        //Common.MapEditData(j, $("#form-info"));
                        //$(".date-picker").each(function () {
                        //    Common.SetDate(this, $(this).val());
                        //});
                        //$this.DetailView();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadProducts: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetProducts",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Products = res.Data;
                        $this.AutoCompleteInit();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },

        LoadProjects: function ($element) {
            var $this = this;
            var $tr = $element.closest("tr");
            var customerId = $tr.find("select.CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetProjects&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Projects = res.Data;
                        $this.AutoCompleteProjectsInit(res.Data, $tr);
                        if (res.Data.length == 1) {
                            var project = res.Data[0];
                            $($tr).find("input.ProjectId").val(project.Id);
                            $($tr).find("input.Project").val(project.Name);
                            $($tr).find("input.ProductId").val(project.ProductId);
                            //$($tr).find("input.Product").val(project.Product);
                            $(tr).find("input.Product").prop("disabled", true);
                        }
                        else {
                            if (($tr).find("input.Id").val() == 0) {


                                $($tr).find("input.Project").val("");
                                $($tr).find("input.ProductId").val('');
                                $($tr).find("input.Product").val("");
                                $(tr).find("input.Product").prop("disabled", false);
                            }
                        }
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        AutoCompleteInit: function (products) {
            var $this = this;
            var products = PageData.Products;
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.Name,
                        label: product.Name,
                        name: product.Name,
                        Product: product

                    }
                );
            }
            $(".Product").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ProductId").val(ui.item.id);
                    $(tr).find("input.Project").val('').prop("disabled", true);
                    $(tr).find("input.ProjectId").val('');
                    //$(tr).find("input.Division").val(ui.item.Product.Division);
                    //$(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

        },

        AutoCompleteProjectsInit: function (projects, $tr) {
            var $this = this;
            var suggestion = new Array();
            for (var i in projects) {
                var project = projects[i];
                suggestion.push(
                    {
                        id: project.Id,
                        value: project.Name,
                        label: project.Name,
                        name: project.Name,
                        ProductId: project.ProductId,
                        Product: project.Product

                    }
                );
            }
            $(".Project", $tr).autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ProjectId").val(ui.item.id);
                    $(tr).find("input.ProductId").val(ui.item.ProductId);
                    //$(tr).find("input.Product").val(ui.item.Product);
                    $(tr).find("input.Product").prop("disabled", true);
                }
            });

        },

        Delete: function (id, $tr) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "deleting...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $($tr).remove();
                            Common.ShowMessage(true, { message: "Record deleted successfully." });
                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },

    };
}();
