var LeadActivities = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "LeadActivities";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            var leadid = $this.GetLeadId();
            $this.LoadPageSetting();
            $this.ListView();
            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val().trim() != "") {
                        var tbl = $(this).closest("table").attr("id");
                        if (tbl == "tbl-concerneditems")
                            $this.AddItem();
                        else {
                            $("#" + tbl + " tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();
                        }
                    }

                }
            });

            //$("#FollowUpStatus").change(function () {

            //    //var text = $(this).find("option:selected").text();
            //    var text = $(this).val();
            //    if (text == "Yes") {
            //        $(".DueDate").removeClass('hide');
            //    } else {
            //        $(".DueDate").addClass('hide');
            //    }
                
            //});

            $(document).on("keyup", ".Quantity,.Rate,.Amount", function (event) {

                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && qty > 0)
                    $this.AddExpectedItem();
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }


            });
            $(document).on("blur", ".Quantity", function (event) {

                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                var code = $(tr).find("input.Code").val();

                if (qty <= 0) {
                    //var err = "Item " + code + " must have quantity greater than zero(0).";
                    //Common.ShowError(err);
                    $(tr).find(":nth-child(3) input.Quantity").val("1");
                    //$(tr).find("input.Quantity").focus();

                }
                $this.GetQuantityPriceTotal(tr);

            });
            AppData.AccountDetail = PageSetting.AccountDetails;
            if (Common.GetInt(leadid) > 0) {

                $this.LoadRecords(leadid)
            }
        },
        LoadRecords: function (leadid) {
            var $this = this;
            $this.EditConcernedItems(leadid, true)
            $this.EditExpectedItems(leadid, true)
            $this.LoadOpenActivities(leadid)

        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
            Common.GetNextAccountCode(API_CONTROLLER);
        },
        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            var $this = this;
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }

        },
        Cancell: function () {
            var $this = this;
            $this.CustomClear();
            Common.GetNextAccountCode(API_CONTROLLER);

        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },
        CustomClear: function () {
            Common.Clear();
        },
        SaveLeadActivity: function () {
            var $this = this;
            if (Common.Validate($("#Meeting-Container"))) {
                var record = Common.SetValue($("#Meeting-Container"));
                record.LeadId = $this.GetLeadId();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/?Type=1",
                    type: "Post",
                    data: record,
                    blockUI: true,
                    blockElement: "#Meeting-Container",
                    blockMessage: "Saving record ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ClearByScope("#Meeting-Container");
                            $this.SetLeadActivityTableData(res.Data);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
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
        SetLeadActivityTableData: function (recordlist) {
            var $this = this;
            if (recordlist.length > 0) {
                for (var items in recordlist) {
                    var item = recordlist[items];
                    item.RecordObject = JSON.stringify(item);
                    item.ActivityTypeName = Common.GetKeyFromEnum(item.ActivityType, ActivityType);
                    if (item.FromDate != null)
                        item.FromDate = moment(item.FromDate).format("DD/MM/YYYY");
                    else
                        item.FromDate = "";
                    if (item.ToDate != null)
                        item.ToDate = moment(item.ToDate).format("DD/MM/YYYY");
                    else
                        item.ToDate = "";
                    if (item.DueDate != null)
                        item.DueDate = moment(item.DueDate).format("DD/MM/YYYY");
                    else
                        item.DueDate = "";
                }

                $("#tblhtml-meetings tbody").html("");
                Common.MapItemData(recordlist, "#tblhtml-meetings", "#meeting-template", true)
                if ($("#tblhtml-meetings tbody tr").length > 0) {
                    $("#tblhtml-meetings").removeClass('hide');
                    $("#tbl-expecteditems tbody").html("");

                    $(".btn-close-model").click();
                }
            }
        },

        SaveConcernedItems: function () {
            var $this = this;
            var record = new Object();
            $(".portlet .container-message").addClass("hide");
            Items = Common.SaveItemData("#tbl-concerneditems");
            Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
            var err = "";
            for (var i in Items) {
                var item = Items[i];
                var product = Common.GetByCode(item.ItemCode);
                if (typeof product == "undefined" || product == null) {
                    err = item.ItemCode + " is not valid code.";
                }
            }
            if (Items.length <= 0) {
                err += "Please add atleast one item.";
            }
            record["LeadConcernedItems"] = Items;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "Post",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving items ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $this.SetConcernedTableData(res)
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        SetConcernedTableData: function (res) {
            var $this = this;
            $("#tblhtml-concerneditems tbody").html("");
            Common.MapItemData(res.Data, "#tblhtml-concerneditems", "#template-itemhtml", true)
            if ($("#tblhtml-concerneditems tbody tr").length > 0) {
                $("#tblhtml-concerneditems").removeClass('hide');
                $("#tbl-concerneditems tbody").html("");
                $("#btn-concerned").addClass('hide');
                $("#btn-concern-edit").removeClass('hide');
                $(".btn-close-model").click();
            }
            else {
                $("#btn-concerned").removeClass('hide');
                $("#btn-concern-edit").addClass('hide');

            }
        },

        SaveExpectedItems: function () {
            var $this = this;
            var record = new Object();
            $(".portlet .container-message").addClass("hide");
            Items = Common.SaveItemData("#tbl-expecteditems");
            Items = Enumerable.From(Items).Where("$.ItemCode.trim()!=''").ToArray();
            var err = "";
            for (var i in Items) {
                var item = Items[i];
                var product = Common.GetByCode(item.ItemCode);
                if (typeof product == "undefined" || product == null) {
                    err = item.ItemCode + " is not valid code.";
                }
            }
            if (Items.length <= 0) {
                err += "Please add atleast one item.";
            }
            record["LeadExpectedItems"] = Items;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?Key=Expected",
                type: "Post",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving items ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.SetExpectedTableData(res)
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        SetExpectedTableData: function (res) {
            var $this = this;
            $("#tblhtml-expecteditems tbody").html("");
            Common.MapItemData(res.Data, "#tblhtml-expecteditems", "#expected-template-itemhtml", true)
            if ($("#tblhtml-expecteditems tbody tr").length > 0) {
                $("#tblhtml-expecteditems").removeClass('hide');
                $("#tbl-expecteditems tbody").html("");
                $("#btn-expected").addClass('hide');
                $("#btn-expc-edit").removeClass('hide');
                $(".btn-close-model").click();
            }
            else {
                $("#btn-expected").removeClass('hide');
                $("#btn-expc-edit").addClass('hide');

            }
        },

        EditConcernedItems: function (id, ispageload) {
            var $this = this;
            if (id == 0)
                id = $this.GetLeadId();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?Key=ConcernedItems",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading product ...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (!ispageload) {
                            var j = res.Data;
                            $("#tbl-concerneditems tbody").html("");
                            Common.MapItemData(j, "#tbl-concerneditems", null);
                            $("#btn-concerned").click();
                        }
                        else {
                            $this.SetConcernedTableData(res)
                        }
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        EditExpectedItems: function (id, ispageload) {
            var $this = this;
            if (id == 0)
                id = $this.GetLeadId();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading product ...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (!ispageload) {
                            var j = res.Data;
                            $("#tbl-expecteditems tbody").html("");
                            Common.MapItemData(j, "#tbl-expecteditems", "#expected-template-item");
                            $this.GetWholeTotal();
                            $("#btn-expected").click();
                        }
                        else {
                            $this.SetExpectedTableData(res)
                        }
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        LoadOpenActivities: function (id) {
            var $this = this;
            if (id == 0)
                id = $this.GetLeadId();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=1",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading product ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.SetLeadActivityTableData(res.Data)

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        EditLeadActivities: function (el) {
            var $this = this;
            var object = $.parseJSON($(el).closest('td').find("input#row-record").val());
            Common.MapEditData(object, $("#Meeting-Container"));
            $(".date-picker,.ac-date", $("#Meeting-Container")).each(function () {

                Common.SetDate(this, object[$(this).attr("Id")]);
            });
          
            var type = Common.GetKeyFromEnum(object.ActivityType, ActivityType)
            $("#model-title").html(type);
            $this.SetModelLayOut(object.ActivityType);
            $("#Meeting-Container").modal("show");

                //var text = $(this).find("option:selected").text();
                //var text = $("#FollowUpStatus").val();
                //if (text == "Yes") {
                //    $(".DueDate").removeClass('hide');
                //} else {
                //    $(".DueDate").addClass('hide');
                //}

        },

        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting product ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.GetNextAccountCode(API_CONTROLLER);
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
        AddConcernedItems: function () {
            var $this = this;
            $("#ConcernedItem-Container").modal("show");
            $this.AddItem();
        },
        AddExpectedVolume: function () {
            var $this = this;
            $("#ExpectedVolume-Container").modal("show");
            $this.AddExpectedItem();
            Common.InitNumerics();
        },
        AddLeadActivity: function (type) {
            var $this = this;
            //$("#model-title").html(type);
            $("#model-title").html("Meeting");
            Common.ClearByScope("#Meeting-Container");
            var activitytype = ActivityType[type];
            $("#ActivityType").val(activitytype);
            $this.SetModelLayOut(activitytype)
            $("#Meeting-Container").modal("show");
        },
        SetModelLayOut: function (activitytype) {
            var $this = this;
            var scope = $("#Meeting-Container");
            if (activitytype == ActivityType.Task) {
                $(".event-row", scope).addClass('hide');
                $(".call-row", scope).addClass('hide');

                $(".task-row", scope).removeClass('hide');
                //$(".DueDate").addClass('hide');

            }
            else if (activitytype == ActivityType.Event) {
                $(".task-row", scope).addClass('hide');
                $(".call-row", scope).addClass('hide');

                $(".event-row", scope).removeClass('hide');

            }
            else if (activitytype == ActivityType.Call) {
                $(".task-row", scope).addClass('hide');
                $(".event-row", scope).addClass('hide');

                $(".call-row", scope).removeClass('hide');

            }
        },
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#tbl-concerneditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-concerneditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 100);

                focusElement = "";
                return;
            }

            var html = $("#template-item").html();

            $("#tbl-concerneditems tbody").append(html);

            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 100);

            }
            else {

                setTimeout(function () {
                    $("#tbl-concerneditems tbody tr:nth-last-child(1) input.Code").focus().select();
                }, 100);
                focusElement = "";
            }

            $this.AutoCompleteInit();
            Common.AllowNumerics();
            Common.InitNumerics();

        },
        AddExpectedItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 100);

                focusElement = "";
                return;
            }
            var html = $("#expected-template-item").html();
            $("#tbl-expecteditems tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                    focusElement = "";
                }, 100);

            }
            else {

                setTimeout(function () {
                    $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 100);
                focusElement = "";
            }
            var scope = $("#tbl-expecteditems tbody")
            $this.AutoCompleteInit();
            ////Common.AllowNumerics();
            Common.InitNumerics();

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        AutoCompleteInit: function (scope) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName

                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.Name").val(account.Name);
                        $(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.SalePrice));
                        $(".container-message").hide();
                    }
                }
            });

        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $("#account-type-conatiner").addClass("hide");
            //this.LoadAccounts();
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find(":nth-child(3) input.Quantity").val());
            Rate = Common.GetFloat($(tr).find(":nth-child(4) input.Rate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(5) input.Amount").val(amount.toFixed(2));
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            $("#tbl-expecteditems tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find(":nth-child(3) input.Quantity").val());
                Price += Common.GetFloat($(this).find(":nth-child(5) input.Amount").val());
            });
            $("#tbl-expecteditems tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity.toFixed(2));
            $("#tbl-expecteditems tfoot tr").find(":nth-child(3) input.Amount").val(Price.toFixed(2));
            // $("#NetTotal").val(netamount + "");
        },
        GetLeadId: function () {
            return Common.GetQueryStringValue("leadid").toLowerCase();
        },

    };
}();
