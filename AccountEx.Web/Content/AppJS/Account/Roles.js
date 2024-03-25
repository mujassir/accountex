
var Roles = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Role";
    var UPLOAD_FOLDER = "Roles";
    var LIST_LOADED = false;
    var hash = "";
    return {
        init: function () {
            var $this = this;

            $this.ListView();
            $this.LoadActions();
            $this.MakeMenuTree();
            $(document).on("change", "#tblActions thead input[type='checkbox']", function () {
                var index = $(this).closest("td").index();
                var selectedcheckbox = $(this);
                $("#tblActions tbody tr").each(function () {
                    $("td:eq(" + index + ") input[type='checkbox']", $(this)).prop("checked", $(selectedcheckbox).is(":checked"));
                });
                $.uniform.update();
            });
            $(document).on("change", "#tblmenu thead input[type='checkbox']", function () {
                var index = $(this).closest("td").index();
                var selectedcheckbox = $(this);
                $("#tblmenu tbody tr").each(function () {
                    $("td:eq(" + index + ") input[type='checkbox']", $(this)).prop("checked", $(selectedcheckbox).is(":checked"));
                });
                $.uniform.update();
            });
            $(document).on("change", "#tblmenu tbody tr td input[type='checkbox']", function () {
                var index = $(this).closest("td").index();
                if (index == 1 && !$(this).is(":checked")) {
                    $(this).closest("td").nextAll().each(function () {
                        $("input[type='checkbox']", $(this)).prop("checked", false);
                    });
                    $.uniform.update();
                }
                if (index == 5 && $(this).is(":checked")) {
                    $(this).closest("td").prevAll().each(function () {
                        $("input[type='checkbox']", $(this)).prop("checked", true);
                    });
                    $.uniform.update();
                }
            });
            $(document).on("change", "#tblmenu tbody tr td input[type='checkbox']", function () {
                var index = $(this).closest("td").index();
                if (index == 1 && !$(this).is(":checked")) {
                    $(this).closest("td").nextAll().each(function () {
                        $("input[type='checkbox']", $(this)).prop("checked", false);
                    });
                    $.uniform.update();
                }
                if (index == 5 && $(this).is(":checked")) {
                    $(this).closest("td").prevAll().each(function () {
                        $("input[type='checkbox']", $(this)).prop("checked", true);
                    });
                    $.uniform.update();
                }
            });

        },

        SaveClose: function () {
            var $this = this;
            this.SaveRecord(function () {
                LIST_CHANGED = true;
                $this.ListView();
            });
        },
        LoadActions: function () {
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?action=default",
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        for (var i = 0; i < res.Data.Actions.length; i++) {
                            var action = res.Data.Actions[i];
                            html += "<tr id='action-" + action.Id + "' data-actionid='" + action.Id + "' data-id='0' >";
                            html += "<td>" + action.Description + "</td>";
                            html += "<td><input type='checkbox'></td>";
                            html += "</tr>";
                        }
                        $("#tblActions tbody").html(html);
                        $("input:checkbox").uniform();
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
            $this.CustomClear();
            $.uniform.update($("input:checkbox").prop("checked", false));
            Common.GoToTop();
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
            $("tr[data-id]").attr("data-id", 0);
            $.uniform.update($("input:checkbox").prop("checked", false));
        },
        Save: function () {
            var $this = this;
            this.SaveRecord(function () {
                $this.CustomClear();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                DataTable.RefreshDatatable(DATATABLE_ID);
                LIST_CHANGED = true;
                $this.ListView();


            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var role = Common.SetValue($("#form-info"));
                var roleAccess = new Array();
                var roleActions = new Array();
                $("#tblmenu tbody tr").each(function () {
                    roleAccess.push({
                        RoleId: Common.GetInt(role.Id),
                        MenuItemId: Common.GetInt($(this).attr("data-menuitemid")),
                        Id: Common.GetInt($(this).attr("data-id")),
                        CanView: $(this).find("td:nth-child(2) input.CanView").is(":checked"),
                        CanCreate: $(this).find("td:nth-child(3) input.CanCreate").is(":checked"),
                        CanUpdate: $(this).find("td:nth-child(4) input.CanUpdate").is(":checked"),
                        CanDelete: $(this).find("td:nth-child(5) input.CanDelete").is(":checked"),
                        CanAuthorize: $(this).find("td:nth-child(6) input.CanAuthorize").is(":checked"),
                    });
                });
                $("#tblActions tbody tr").each(function () {
                    roleActions.push({
                        RoleId: role.Id,
                        Id: Common.GetInt($(this).attr("data-id")),
                        ActionId: $(this).attr("data-actionid"),
                        Allowed: $(this).find("td:nth-child(2) input:checkbox").is(":checked"),
                    });
                });

                role["RoleAccess"] = roleAccess,
                role["RoleActions"] = roleActions
                record = role;

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving role...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $("#Name").val("");
                            callback();
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
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var role = res.Data.Role;
                        var accesses = res.Data.RoleAccess;
                        var actions = res.Data.RoleActions;
                        Common.MapEditData(role, "#form-info");
                        $.uniform.update($("input:checkbox").prop("checked", false));
                        for (var i in accesses) {
                            var access = accesses[i];
                            var tr = $("#node-" + access.MenuItemId);
                            $(tr).attr("data-id", access.Id);
                            for (var j in access) {
                                $.uniform.update($("input." + j, tr).prop("checked", access[j]));
                            }
                        }
                        for (var i in actions) {
                            var action = actions[i];
                            var tr = $("#action-" + action.ActionId);
                            $(tr).attr("data-id", action.Id);
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
        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                $.ajax({
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

        GetClients: function () {
            var $this = this;
            $.ajax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetEmployees: function () {
            var $this = this;
            $.ajax({
                url: Setting.APIBaseUrl + "COA/?key=GetEmployees",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindMultiSelect(res.Data, "#Employees", true);
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
                menuHtml += '<tr id="node-' + parent.Id + '" data-id="0" class="cursor-pointer" data-menuitemid="' + parent.Id + '" data-jstree={type:' + types[level] + '} data-haschild="true"><td>' + name + '</td>';

                menuHtml += '<td><input class="CanView" type="checkbox"></td>';
                menuHtml += '<td><input class="CanCreate" type="checkbox"></td>';
                menuHtml += '<td><input class="CanUpdate" type="checkbox"></td>';
                menuHtml += '<td><input class="CanDelete" type="checkbox"></td>';
                menuHtml += '<td><input class="CanAuthorize" type="checkbox"></td></tr>';
                //'</ul></li>';
            }
            else {
                level = 1;
                menuHtml += '<tr class="parent" data-id="0" id="node-' + parent.Id + '" data-menuitemid="' + parent.Id + '"  data-jstree={type:' + types[level] + '"}><td>' + name + '</td>';
                menuHtml += '<td><input class="CanView" type="checkbox"></td><td></td><td></td><td></td><td></td></tr>';
                for (var i in children) {
                    menuHtml += $this.RenderMenu(children[i]);
                }

            }
            return menuHtml;
        },
        RenderMenu1: function (parent) {
            var $this = this;
            var types = { 1: "default", 2: "head", 3: "control", 4: "account" };
            var menuHtml = "";
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var children = parent.SubMenues;
            var level = 1;

            var name = parent.Title;
            if (children == null || children.length == 0) {
                level = 2;
                menuHtml += " <li id=\"node-" + parent.Id + "\" class=\"cursor-pointer\" data-accountId=\"" + parent.Id + "\" data-level=\"" + parent.Level + "\" data-jstree='{\"type\":\"" + types[level] + "\"}' data-haschild=\"true\">" + name + "";
                level = 3;
                menuHtml += "<span class=\"cursor-pointer\" data-jstree='{\"type\":\"" + types[level] + "\"}'  data-haschild=\"true\">";
                menuHtml += "<div class=\"checkbox-list\">" +
                "<label class=\"checkbox-inline\"><input type=\"checkbox\">View </label>" +
                "<label class=\"checkbox-inline\"><input type=\"checkbox\">Add</label>" +
                "<label class=\"checkbox-inline\"><input type=\"checkbox\">Edit</label>" +
                "<label class=\"checkbox-inline\"><input type=\"checkbox\" >Delete</label>" +
                "<label class=\"checkbox-inline\"><input type=\"checkbox\" >Authorize</label></div></span></li>";
                //'</ul></li>';
            }
            else {

                menuHtml += "<li id=\"node-" + parent.Id + "\" data-level=\"" + parent.Level + "\"  data-jstree='{\"type\":\"" + types[level] + "\"}' >" +
                "<span class=\"cursor-pointer\"  data-haschild=\"true\">" + name + "</span><ul>";
                for (var i in children) {
                    menuHtml += $this.RenderMenu(children[i]);
                }
                menuHtml += "</ul></li>";
            }
            return menuHtml;
        },
        MakeTreeView: function () {
            $("#menuSearchTree").keyup(function () {
                var v = $("#menuSearchTree").val();
                $("#menuTree").jstree(true).search(v);
            });
            TreeView = $("#menuTree").jstree({
                "core": {
                    "themes": {
                        "responsive": false
                    }
                },
                persist: "cookie",
                cookieId: "treeview-black",
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-info icon-lg"
                    },
                    "head": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "control": {
                        "icon": "fa fa-folder icon-state-success icon-lg"
                    },
                    "account": {
                        "icon": "fa fa-file icon-state-danger icon-lg"
                    }
                },
                "plugins": ["types", "search"],
                "search": { "show_only_matches": true }
            });
            //$("#coaTree li[role='treeitem']").removeClass("jstree-closed").addClass("jstree-open");
        },
        ExpandTree: function () {
            TreeView.jstree("open_all");
            //$("#coaTree").jstree("open_node", "#j1_1");
        },
        CollapseTree: function () {
            TreeView.jstree("close_all");
            //$("#coaTree").jstree("close_node", "#j1_1");
            //$("#coaTree").jstree("toggle_node", "#j1_1");
        },
        ExpandSelectedNode: function () {
            var id = parseInt($(currentElement).attr("data-accountid"));
            this.ExpandTreeNode(id);
        },
        ExpandTreeNode: function (id) {
            var $this = this;
            $this.ExpandTreeNodeI(id);
            TreeView.jstree("select_node", "#node-" + id);
        },
        ExpandTreeNodeI: function (id) {
            var $this = this;
            var account = $.grep(Accounts, function (e) { return e.Id == id; })[0];
            if (typeof account != "undefined" && account.Level > 1) {
                var parent = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
                if (parent != null)
                    $this.ExpandTreeNodeI(parent.Id);
            }
            if (typeof account != "undefined" && account.Level < 4)
                TreeView.jstree("open_node", "#node-" + id);
        },

    };
}();
