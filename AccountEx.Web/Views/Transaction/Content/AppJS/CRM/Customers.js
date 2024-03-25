
var Customer = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMCustomer";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var PageSetting = new Object();
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
            $(document).on("click", ".btn-view", function (event) {
                $this.Edit($(this), true);
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem();
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $this.LoadPageSetting();
            $this.CustomClear();
            $this.ListView();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
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

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            var $this = this;
            Common.Clear();
            $("#item-container tbody").html("");
            $this.AddItem();
            if (PageSetting.UserTypeId == CRMUserType.SalesExecutive || PageSetting.UserTypeId == CRMUserType.RSM)
            {
                $("#RegionId").select2("val", PageSetting.RegionId).select2("enable", false);
            }
        },

        Save: function () {
            var $this = this;
            var Items = new Array();
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where(function (p) { return p.CategroyId > 0 && p.UserId > 0 }).ToArray();
                record["CRMCustomerSalePersons"] = Items;
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

        Edit: function ($element, isView) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
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
                        $("#item-container tbody").html("");
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"), undefined, isView);
                        Common.MapItemData(j.CRMCustomerSalePersons);
                        $("#item-container tbody tr select.select2").each(function () {
                            $(this).prop('disabled', true);
                            Common.BindSelect2($(this));
                            
                        });
                        $("#item-container tbody tr").each(function () {
                            var $tr=$(this);
                            $("span.action", $tr).remove();
                        });
                        Common.SetCheckValue(j);
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
        AddItem: function () {
            var $this = this;
            //  if (Common.Validate($("#addrow"))) {
            var code = $("#item-container tbody tr:nth-last-child(1)  input.DivisionId").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.DivisionId").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.DivisionId").focus().select();
                }, 300);
                focusElement = "";
            }
            $("#item-container tbody tr:nth-last-child(1) select.select2").each(function () {
                var $element = $(this);
                Common.BindSelect2($element);
                if ($element.hasClass("UserId")) {
                    var childCount = $(this).find("option").length;
                    if (childCount == 2) {
                        var value = $element.find('option:nth-child(2)').attr("data-custom");
                        $($element).select2("val", value)
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
        },
        DeleteRow: function (element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            $tr.remove();

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
