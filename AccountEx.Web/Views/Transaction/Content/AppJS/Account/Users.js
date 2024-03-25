
var Users = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "User";
    var LIST_LOADED = false;
    var hash = "";
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $this.LoadActions();
            $this.MakeMenuTree();
            $.uniform.update($("input:checkbox", $("#role-access")).prop("disabled", "disabled"));
            $("#RoleIds").change(function () {
                var roleid = $(this).val();
                if (roleid == "" || roleid == null) {
                    $.uniform.update($("input:checkbox", $("#role-access")).prop("checked", false));
                }
                else {
                    $this.LoadAccess(roleid);
                }
            });
        },

        Add: function () {
            var $this = this;
            $this.DetailView();

            $this.CustomClear();
            $("#Hash").attr("data-required", "required");
            $("#Username").prop("disabled", false);
            Common.BindSelect("", $("#RoleIds"), "", true);
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        ListView: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        CalculateTax: function () {
            var tax = Common.GetFloat($("#GrossCost").val()) * Setting.SalesTax;
            $("#SalesTax").val(tax);
            CalculateNetCost();
        },
        CalculateNetCost: function () {
            var sum = 0;
            $("#GrossCost,#SalesTax,#Miscellaneous").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#NetCost").val(sum);

        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            //$("#saleitem tbody .chooseninner").chosen();
            AllowNumerics();
            //$(".select2").select2();
            $("select").each(function () {
                $(this).select2();
            });

            //SetDropDown();
        },
        CustomClear: function () {
            Common.Clear();
            $.uniform.update($("input:checkbox").prop("checked", false));
        },

        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                if (record.RoleIds == null || record.RoleIds.length <= 0) {
                    var err = "Role is required."
                    Common.ShowError(err);
                    return;
                }
                var roles = new Array();
                var items = new Array();
                var newArray = new Array();
                $("#RoleIds").select2().find(":selected").each(function () {

                    roles.push($(this).attr("data-custom"));
                });
                roles = $.grep(roles, function (n) { return (n) });
                record.RoleIds = $.grep(record.RoleIds, function (n) { return (n) });
                $.each(record.RoleIds, function (index, item) {
                    items.push({
                        UserId: record.Id,
                        RoleId: item
                    });
                });
                record.Role = roles.join();
                record.RoleIds = record.RoleIds.join();
                record["UserRoles"] = items;
                 Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?hash=" + record.Hash,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving user...please wait",

                    success: function (res) {
                        if (res.Success) {
                            Common.Clear();
                            $.uniform.update($("input:checkbox").prop("checked", false));
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
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

        Edit: function (id) {
            var $this = this;
             Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        hash = j.Hash;
                        Common.SetCheckValue(j);
                        $("#Hash").attr("data-required", "not-required");
                        $("#Username").prop("disabled", true);
                        $("#Hash").val("");
                        if (j.RoleIds != null)
                            $("#RoleIds").select2("val", j.RoleIds.split(","));
                        $("#RoleIds").trigger("change");
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
        },
      
        LoadAccess: function (id) {
            var $this = this;
             Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?roleids=" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var accesses = res.Data.RoleAccess;
                        var actions = res.Data.RoleActions;
                        var scope = $("#role-access");
                        //$("#RoleId").val(id);
                        $.uniform.update($("input:checkbox", scope).prop("checked", false));
                        for (var i in accesses) {
                            var access = accesses[i];
                            var tr = $("#node-" + access.MenuItemId);
                            for (var j in access) {
                                $.uniform.update($("input." + j, tr).prop("checked", access[j]));
                            }
                        }
                        for (var i in actions) {
                            var action = actions[i];
                            var tr = $("#action-" + action.ActionId);
                            $.uniform.update($("input:checkbox", tr).prop("checked", action.Allowed));
                        }
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
        LoadActions: function () {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?action=default&actionid=1",
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        for (var i = 0; i < res.Data.Actions.length; i++) {
                            var action = res.Data.Actions[i];
                            html += "<tr id='action-" + action.Id + "' data-actionid='" + action.Id + "'>";
                            html += "<td>" + action.Description + "</td>";
                            html += "<td><input type='checkbox'></td>";
                            html += "</tr>";
                        }
                        $("#tblActions tbody").html(html);
                        $("input:checkbox").uniform();
                        $.uniform.update($("input:checkbox", $("#tblActions")).prop("disabled", "disabled"));
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        MakeMenuTree: function () {

            var $this = this;
            var menuHtml = "";
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var menulist = $.parseJSON($("#Menu").val());
            for (var i in menulist) {
                var menu = menulist[i];
                menuHtml += $this.RenderMenu(menu);
            }
            //$(".wrapper_menuTree").html("<div id='menuTree' class='tree-demo'><ul>" + MenuHtml + "</ul></div>");
            //if (menulist == null || menulist == undefined || menulist.length == 0)
            //    $(".wrapper_menuTree").html("There are no menu, to create access,first create the menu.");
            //else
            //    $this.MakeTreeView();
            $("#tblmenu tbody").html(menuHtml);
        },
        RenderMenu: function (parent) {
            var $this = this;
            var types = { 1: "default", 2: "head", 3: "control", 4: "account" };
            var menuHtml = "";
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var children = parent.SubMenues;
            var level = 1;

            var name = parent.Title;
            if (children == null || children.length == 0) {
                level = 1;
                menuHtml += " <tr id=\"node-" + parent.Id + "\" class=\"cursor-pointer\" data-menuitemid=\"" + parent.Id + "\" data-jstree='{\"type\":\"" + types[level] + "\"}' data-haschild=\"true\"><td>" + name + "</td>";

                menuHtml += "<td><input class=\"CanView\" type=\"checkbox\"></td>" +
                "<td><input class=\"CanCreate\" type=\"checkbox\"></td>" +
                "<td><input class=\"CanUpdate\" type=\"checkbox\"></td>" +
                "<td><input class=\"CanDelete\" type=\"checkbox\"></td>" +
                "<td><input class=\"CanAuthorize\" type=\"checkbox\"></td></tr>";
                //'</ul></li>';
            }
            else {
                level = 1;
                menuHtml += "<tr class=\"parent\" id=\"node-" + parent.Id + "\" data-menuitemid=\"" + parent.Id + "\"  data-jstree='{\"type\":\"" + types[level] + "\"}' ><td>" + name + "</td><td><input class=\"CanView\" type=\"checkbox\"></td><td></td><td></td><td></td><td></td></tr>";
                for (var i in children) {
                    menuHtml += $this.RenderMenu(children[i]);
                }

            }
            return menuHtml;
        },
    };
}();
