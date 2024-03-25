
var Manage = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Manage";
    var LIST_LOADED = false;
    var attributeTypes;
    return {
        init: function () {
            var $this = this;
            $this.GetAttributes();
            $this.LoadAttributeTypes();
            if (Setting.PageLandingView == "DetailView") {
                $this.Add();
            }
            else {
                $this.ListView();
            }
        },
        LoadAttributeTypes: function () {
            attributeTypes = JSON.parse($("#txtAttributeTypeJson").val());
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        GetAttributes: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER + "/1?key=GetAttributes";
            url += "&type=" + $this.GetType();
            Common.WrapAjax({
                url: url,
                type: "GET",
                success: function (res) {
                    if (res.Success) {

                        $("#form-info .form-body").html("");
                        //$(el).Clear();
                        var html = "";
                        for (var i = 0; i < res.Data.Attributes.length; i++) {
                            var el = res.Data.Attributes[i];
                            var atType = $.grep(attributeTypes, function (e) { return e.Id == el.TypeId })[0];
                            var controlHtml = "";
                            if (atType.ControlType == "text")
                                controlHtml += "<input id='" + el.Name + "' class='form-control " + atType.SizeId + " " + atType.CssClass + "' data-id='" + atType.Id + "'/>";
                            else if (atType.ControlType == "textarea")
                                controlHtml += "<textarea id='" + el.Name + "' class='form-control " + atType.SizeId + " " + atType.CssClass + "' data-id='" + atType.Id + "'></textarea>";
                            html = "<div class=\"form-group\"><label class=\"col-md-2 control-label\" >" + el.Label + "</label>\
                                   <div class=\"col-md-6\">"+ controlHtml + "</div></div>";
                            $("#form-info .form-body").append(html);
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
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            var $this = this;
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        ListView: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },

        Close: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            var $this = this;
            //$("#saleitem tbody .chooseninner").chosen();
            Common.AllowNumerics();
            //$(".select2").select2();
            $("select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },
        CustomClear: function () {
        },
        SetValue: function (scope) {
            var $this = this;
            var record = new Object();
            $("div[data-save='save']", scope).find("input[type=text],textarea,select,input[type=hidden],input[type=checkbox],input[type=password]").not("[Data-Save='false'],[data-save='false'] input[type=text],[data-save='false'] input[type=hidden],[data-save='false'] select,[data-save='false'] textarea").each(function () {
                if ($(this).hasClass("ac-date") || $(this).hasClass("date-picker")) {
                    record[$(this).attr("Id")] = $this.ChangeDateFormate($(this).val());
                }
                else {

                    record[$(this).attr("Id")] = $(this).val();
                }
            });
            return record;
        },
        Save: function () {
            var $this = this;
            var record = Common.SetValue("#form-info");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                success: function (res) {
                    if (res.Success) {
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.ListView();
                    }
                    else {
                       Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        Common.MapEditData(res.Data, "#form-info");
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
        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
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
        }

    };
}();

