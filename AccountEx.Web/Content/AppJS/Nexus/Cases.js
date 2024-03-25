
var Cases = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "NexusCase";
    var LIST_LOADED = false;
    var SetFocus = "";
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
            $(document).on("keyup", "input.Price", function (event) {
                $this.GetWholeTotal();
                if (event.which == 13 && $(this).val() > 0) {
                    $this.AddItem();
                }
            });
            $this.LoadTest();
            $this.ListView();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
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

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            $("#item-container,#item-nexus-container").find("tbody").html("");
            Common.Clear();
        },

        Save: function () {
            var $this = this;
            var Items = new Array();
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.TestName.trim()!=''").ToArray();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (item.Amount <= 0) {
                        err += item.TestName + " must have price greater than zero(0).";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one test.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["Nexus_PostedCasesItems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            $this.Close();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $("#Name").focus();
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

        Edit: function ($element) {
            var $this = this;
           
            var id = Common.GetInt($element.attr("data-id"));
            var $tr = $element.closest("tr");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container,#item-nexus-container").find("tbody").html("");
                        var j = res.Data;
                        var nexusCase = j.NexusCase;
                        Common.MapEditData(j.NexusCase, $(".nexus-info"), true);
                        Common.MapItemData(j.NexusCase.Nexus_CaseDetail, "#item-nexus-container", "#template-nexus-item");
                        Common.SetDate($(".RegistrationDate"), nexusCase.RegistrationDate);
                        Common.SetDate($("#Date"), nexusCase.RegistrationDate);
                        $("#CaseId").val(nexusCase.ID);
                        $(".PatientName").val($tr.find("td:nth-child(2)").html());
                        if (j.Case != null) {
                            Common.MapEditData(j.Case, $("#form-info"));
                            Common.SetDate($("#Date"), j.Case.Date);
                            var items = j.Case.Nexus_PostedCasesItems;
                            Common.MapItemData(items);
                        }
                        $this.AddItem();
                        $this.AutoCompleteInit();
                        $this.DetailView();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        LoadTest: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetTests",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Tests = res.Data;
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
        AutoCompleteInit: function (products) {
            var $this = this;
            var tokens = PageData.Tests;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.Name,
                        label: token.Name,
                        name: token.Name,
                        Token: token

                    }
                );
            }
            $(".TestName").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.Price").focus().select();
                    $(tr).find("input.TestId").val(ui.item.id);
                    //$(tr).find("input.Division").val(ui.item.Token.Division);
                    //$(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).closest("tr").remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        AddItem: function () {
            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.TestName").val();

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.TestName").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.TestName").focus().select();
                }, 300);
                SetFocus = "code";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (SetFocus == "date") {
                setTimeout(function () {
                    $("#Date").focus();
                }, 300);
            }
            else if (SetFocus == "voucher") {
                setTimeout(function () {
                    $("#VoucherNumber").focus();
                }, 300);
            }
            else {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.TestName").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $this.GetWholeTotal();
            $this.AutoCompleteInit();
            Common.InitNumerics();
        },
        GetWholeTotal: function () {

            var Quantity = 0;
            var amount = 0;
            var credit = 0;
            $("#item-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find("input.Price").val());


            });
            $("#TotalAmount,#NetAmount,#PaidAmount").val(amount);
            $("#Discount,#Less").val(0);
        },
        Delete: function ($element) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
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
                            DataTable.RefreshDatatable(DATATABLE_ID);
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
