
var ChartOfAccount = function () {
    var currentElement = null;
    var currentForm = "MainAccount";
    var newOrEdit = "New";
    var treeView;
    var Accounts = new Array();
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            //Accounts = $.parseJSON($("#Accountjson").val());
            $this.LoadPageSetting();
            Accounts = AppData.COA;
            $this.LoadAccountTypes();
            $this.MapJsonData();
            $this.LoadDefaultDropdowns();
            $this.MakeAccountTree();


            $("#Name").keydown(function (e) {
                if (e.keyCode == 13) {
                    $this.Save();
                }
            });
            $("#LoginUserName,#LoginPassword").keyup(function (event) {
                if (event.which == 13)
                    $this.ContinueDelete();
            });

        },
        CacheAccounts: function () {
            for (var i = 0; i < data.length; i++) {
                var account = data[i];
                Accounts[account.Id] = account;
            }
        },
        LoadAccountTypes: function () {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetAccountTypes",
                type: "GET",
                success: function (res) {
                 
                    var html = "<option value=\"\">- None -</option>";
                    for (var i in res.Data) {
                        var account = res.Data[i];
                        html += "<option value=\"" + account.Id + "\">" + account.Name + "</option>";
                    }
                    $("#AccountType").html(html).select2();
                }
            });
           
        },
        GetNextAccountCode: function (element, value) {
            var $this = this;
            $(element).val(value);
            //if (Setting.COAAccountCodeManual) {
            //    $(element).prop("disabled", false);
            //}
            //else
            //    $(element).prop("disabled", true);
            if ((value == null)) {
                var headAccountId = $(currentElement).attr("data-level") == 3 ? Common.GetInt($(currentElement).attr("data-accountid")) : Common.GetInt($(currentElement).attr("data-parent"));
                //var HeadAccountId = Common.GetInt($(currentElement).attr("data-parent"));
                var d = "";
                $.ajax({
                    url: Setting.APIBaseUrl + "COA?key=GetNextAccountCode&HeadAccountId=" + headAccountId,
                    type: "Get",
                    data: d,
                    success: function (res) {

                        if (res.Success) {
                            $(element).val(res.Data);
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
        MapJsonData: function () {
            $("#jsondata :input").each(function () {
                try {
                    window.Account[$(this).attr("Id")] = $.parseJSON($(this).val());

                } catch (e) {

                }

            });
        },
        SetNewOrEdit: function (val) {
            $(".alert.alert-danger.container-message").hide();
            newOrEdit = val;
            if (newOrEdit === "Edit") {
                $("#account-edit-form").removeClass("blue");
                //$("#account-edit-form").addClass("purple");
                $("#btnSaveAccount").html("<i class='fa fa-save'></i> Update");
                $("#account-edit-form .icon").addClass("fa-edit");
                $("#account-edit-form .icon").removeClass("fa-plus");
            }
            else {
                $("#account-edit-form").removeClass("purple");
                //$("#account-edit-form").addClass("blue");
                $("#btnSaveAccount").html("<i class='fa fa-check'></i> Save");
                $("#account-edit-form .icon").removeClass("fa-edit");
                $("#account-edit-form .icon").addClass("fa-plus");
            }
        },
        Add: function () {
            var $this = this;
            $(".alert.alert-danger.container-message").hide();
            switch (currentForm) {
                case "MainAccount":
                    $this.AddMainAccount();
                    break;
                case "ControlAccount":
                    $this.AddControlAccount();
                    break;
                case "SubAccount":
                    $this.AddSubAccount();
                    break;
                case "Account":
                    $this.AddAccount();
                    break;
            }
        },
        AddMainAccount: function () {
            var $this = this;
            $("#Name").val("");
            $(".row-MainAccount,.row-ControlAccount,.row-SubAccount,.row-AccountCode,#AccountAttributes,#divDeleteAccount,#divDeleteAccount").addClass("hide");
            $(".row-AccountType").removeClass("hide");
            currentForm = "MainAccount";
            $this.SetNewOrEdit("New");
            Common.UpdateRequired($("#AccountCode"), false);
        },
        AddControlAccount: function () {
            var $this = this;
            $("#Name").val("");
            $(".row-ControlAccount,.row-SubAccount,.row-AccountCode,.row-AccountType,#AccountAttributes,#divDeleteAccount,#divDeleteAccount").addClass("hide");
            $(".row-MainAccount").removeClass("hide");
            currentForm = "ControlAccount";
            $this.SetNewOrEdit("New");
            Common.UpdateRequired($("#AccountCode"), false);
        },
        AddSubAccount: function () {
            var $this = this;
            $("#Name").val("");
            $(".row-SubAccount,.row-AccountCode,.row-AccountType,#AccountAttributes,#divDeleteAccount,#divDeleteAccount").addClass("hide");
            $(".row-MainAccount,.row-ControlAccount").removeClass("hide");
            currentForm = "SubAccount";
            $this.SetNewOrEdit("New");
            Common.UpdateRequired($("#AccountCode"), false);
        },
        OpenPopup: function () {
            var $this = this;
            $("#LoginUserName,#LoginPassword").val("");
            $("#btncredential").click();
        },




        AddAccount: function () {
            var $this = this;
            $("#Name").val("");
            $(".row-AccountType,#divDeleteAccount").addClass("hide");
            $(".row-MainAccount,.row-ControlAccount,.row-AccountCode,.row-SubAccount,#AccountAttributes").removeClass("hide");
            $this.GetNextAccountCode("#AccountCode");
            $this.RenderAttribute();
            currentForm = "Account";
            $this.SetNewOrEdit("New");
            Common.UpdateRequired($("#AccountCode"), true);
        },
        CustomClear: function (element) {
            var date = GetTodayDate(1);
            $(element).find("input[type=text],textarea,select,input[type=hidden],input[type=password]").val("");
            $("select", element).val("").each(function () {
                $(this).select2();
            });
        },
        RenderAttributeWithValue: function (accountId) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetAttributes&AccountId=" + accountId,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var currentAccount = $.grep(Accounts, function (e) { return e.Id == accountId; })[0];
                        var html = "";
                        var attributes = $.parseJSON($("#Attributejson").val());
                        var attributeTypes = $.parseJSON($("#AttributeTypejson").val());
                        var ids = new Array();
                        var attribute;
                        var attributetype;
                        var element;
                        for (var i in res.Data) {
                            var attr = res.Data[i];
                            attribute = $.grep(attributes, function (e) { return e.Id == attr.AttributeId; })[0];
                            ids.push(attribute.Id);
                            attributetype = $.grep(attributeTypes, function (e) { return e.Id == attribute.Type; })[0];
                            var vl = attr.Value + "";
                            if (vl == undefined || vl == null || vl == "null") vl = "";
                            if (attributetype.ControlType == "text") {
                                element = "<input type=\"text\" value=\"" + vl + "\" class=\"form-control " + attributetype.CssClass + "\" data-attributeId=\"" + attribute.Id + "\" data-attributeName=\"" + attribute.Name + "\">";
                            }
                            else if (attributetype.ControlType == "textarea") {
                                element = "<textarea class=\"form-control " + attribute.CssClass + "\" data-attributeId=\"" + attribute.Id + "\" data-attributeName=\"" + attribute.Name + "\">" + vl + "</textarea>";
                            }
                            html += "<div class=\"form-group\">" +
                                            "<label class=\"col-md-4 control-label\" for=\"prefix\">" + attributetype.Label + "</label>" +
                                            "<div class=\"col-md-8\">" + element + "</div></div>";
                        }
                        var accTypeId = $this.GetAccountTypeId(currentAccount);
                        attributes = $.grep(attributes, function (e) { return e.AccountTypeId == accTypeId; });
                        for (var i in attributes) {
                            attribute = attributes[i];
                            if (ids.indexOf(attribute.Id) == -1) {
                                attributetype = $.grep(attributeTypes, function (e) { return e.Id == attribute.Type; })[0];
                                if (attributetype.ControlType == "text") {
                                    element = "<input type=\"text\" class=\"form-control " + attributetype.CssClass + "\" data-attributeId=\"" + attribute.Id + "\" data-attributeName=\"" + attribute.Name + "\">";
                                }
                                else if (attributetype.ControlType == "textarea") {
                                    element = "<textarea class=\"form-control " + attributetype.CssClass + "\" data-attributeId=\"" + attribute.Id + "\" data-attributeName=\"" + attribute.Name + "\"></textarea>";
                                }
                                html += "<div class=\"form-group\">" +
                                                "<label class=\"col-md-4 control-label\" for=\"prefix\">" + attribute.Label + "</label>" +
                                                "<div class=\"col-md-8\">" + element + "</div></div>";
                            }
                        }
                        $("#AccountAttributes").html(html);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        RenderAttribute: function () {
            //var $this = this;
            //var html = "";
            //var element = "";
            //var AccTypeId = $(currentElement).attr("data-typeid");
            //var AccId = $(currentElement).attr("data-accountid");
            //if (AccTypeId == "" || AccTypeId == 'null') {
            //    //var Accounts = $.parseJSON($("#Accountjson").val());
            //    var CurrentAccount = $.grep(Accounts, function (e) { return e.Id == AccId; })[0];
            //    AccTypeId = $this.GetAccountTypeId(CurrentAccount);
            //    // alert("after-" + AccTypeId);
            //    // alert('id-' + MyACCId);
            //}
            //var Attributes = $.parseJSON($("#Attributejson").val());
            //var AttributeTypes = $.parseJSON($("#AttributeTypejson").val());
            //Attributes = $.grep(Attributes, function (e) { return e.AccountTypeId == AccTypeId; });
            //for (var i in Attributes) {
            //    var attribute = Attributes[i];
            //    var attributetype = $.grep(AttributeTypes, function (e) { return e.Id == attribute.Type; })[0];
            //    if (attributetype.ControlType == "text") {
            //        element = '<input type="text" class="form-control ' + attributetype.CssClass + '" data-attributeid="' + attribute.Id + '" data-attributename="' + attribute.Name + '">';
            //    }
            //    else if (attributetype.ControlType == "textarea") {
            //        element = '<textarea class="form-control ' + attributetype.CssClass + '" data-attributeid="' + attribute.Id + '" data-attributename="' + attribute.Name + '"></textarea>';
            //    }
            //    html += '<div class="form-group">' +
            //                    '<label class="col-md-4 control-label" for="prefix">' + attribute.Label + '</label>' +
            //                    '<div class="col-md-8">' + element + '</div></div>';
            //}
            //$("#AccountAttributes").html(html);
        },
        GetAccountTypeId: function (currentAccount) {
            var $this = this;
            if (currentAccount != null && currentAccount.ParentId != null) {
                var parentAccount = $.grep(Accounts, function (e) { return e.Id == currentAccount.ParentId; })[0];
                if (parentAccount.AccountTypeId == "" || parentAccount.AccountTypeId == undefined || parentAccount.AccountTypeId == null) {
                    return $this.GetAccountTypeId(parentAccount);
                }
                else {
                    return parentAccount.AccountTypeId;
                }
            }
            else if (currentAccount != null) {
                return currentAccount.AccountTypeId;
            }
        },
        BindSelect: function (elementId, accounts, value) {
            if (accounts.length == 0) {
                $("#" + elementId).html("");
            }
            var html = "";
            for (var i = 0; i < accounts.length; i++) {
                var account = accounts[i];
                html += "<option value='" + account.Id + "'>" + account.DisplayName + "</option>";
            }
            $("#" + elementId).html(html).select2();
            if (value != null && value != undefined)
                $("#" + elementId).select2("val", value);
        },
        LoadDefaultDropdowns: function () {
            var $this = this;
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var l1Accounts = $.grep(Accounts, function (e) { return e.Level == 1; });
            $this.BindSelect("MainAccount", l1Accounts);
            if (l1Accounts.length == 0) {
                $("#ControlAccount,#SubAccount").html("").select2();
                return;
            }
            var id = l1Accounts[0].Id;
            var l2Accounts = $.grep(Accounts, function (e) { return e.ParentId == id; });
            if (l2Accounts.length > 0) {
                var ac = l2Accounts[0];
                $this.BindSelect("ControlAccount", l2Accounts);
                var l3Accounts = $.grep(Accounts, function (e) { return e.ParentId == ac.Id; });
                $this.BindSelect("SubAccount", l3Accounts);
            }
            else
                $("#ControlAccount,#SubAccount").html("").select2();
        },
        MainAccount_Change: function (el) {
            var $this = this;
            var id = $(el).val();
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var l2Accounts = $.grep(Accounts, function (e) { return e.ParentId == id; });
            if (l2Accounts.length > 0) {
                var ac = l2Accounts[0];
                $this.BindSelect("ControlAccount", l2Accounts);
                var l3Accounts = $.grep(Accounts, function (e) { return e.ParentId == ac.Id; });
                $this.BindSelect("SubAccount", l3Accounts);
            }
            else
                $("#ControlAccount,#SubAccount").html("").select2();
        },
        ControlAccount_Change: function (el) {
            var $this = this;
            var id = $(el).val();
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var l3Accounts = $.grep(Accounts, function (e) { return e.ParentId == id; });
            $this.BindSelect("SubAccount", l3Accounts);
        },
        SubAccount_Change: function (el) {
            //var id = $(this).val();
        },
        LoadAccountDropdowns: function (accountId) {
            var $this = this;
            $(".row-MainAccount,.row-ControlAccount,.row-SubAccount,.row-AccountType").removeClass("hide");
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var account = $.grep(Accounts, function (e) { return e.Id == accountId; })[0];
            var parent = null, main = null;
            parent = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
            if (parent != null) main = $.grep(Accounts, function (e) { return e.Id == parent.ParentId; })[0];
            var l1Accounts = $.grep(Accounts, function (e) { return e.Level == 1; });
            if (account.Level == 1) {
                $this.BindSelect("MainAccount", l1Accounts, account.Id);
                $(".row-MainAccount,.row-ControlAccount,.row-SubAccount,.row-AccountCode,#AccountAttributes").addClass("hide");
                currentForm = "MainAccount";
            }
            else {
                var l2Accounts;
                if (account.Level == 2) {
                    $this.BindSelect("MainAccount", l1Accounts, account.ParentId);
                    l2Accounts = $.grep(Accounts, function (e) { return e.ParentId == account.ParentId; });
                    $this.BindSelect("ControlAccount", l2Accounts, account.Id);
                    $(".row-ControlAccount,.row-SubAccount,.row-AccountCode,#AccountAttributes,.row-AccountType").addClass("hide");
                    $(".row-MainAccount").removeClass("hide");
                    currentForm = "ControlAccount";
                }
                else {
                    var l3Accounts;
                    if (account.Level == 3) {
                        l2Accounts = $.grep(Accounts, function (e) { return e.ParentId == parent.ParentId; });
                        l3Accounts = $.grep(Accounts, function (e) { return e.ParentId == account.ParentId; });
                        $this.BindSelect("MainAccount", l1Accounts, parent.ParentId);
                        $this.BindSelect("ControlAccount", l2Accounts, account.ParentId);
                        $this.BindSelect("SubAccount", l3Accounts, account.Id);
                        $(".row-SubAccount,.row-AccountCode,#AccountAttributes,.row-AccountType").addClass("hide");
                        $(".row-ControlAccount,.row-MainAccount").removeClass("hide");
                        currentForm = "SubAccount";
                    }
                    else if (account.Level == 4) {
                        l2Accounts = $.grep(Accounts, function (e) { return e.ParentId == main.ParentId; });
                        l3Accounts = $.grep(Accounts, function (e) { return e.ParentId == parent.ParentId; });
                        $this.BindSelect("MainAccount", l1Accounts, main.ParentId);
                        $this.BindSelect("ControlAccount", l2Accounts, parent.ParentId);
                        $this.BindSelect("SubAccount", l3Accounts, account.ParentId);
                        $(".row-AccountType").addClass("hide");
                        $(".row-MainAccount,.row-ControlAccount,.row-SubAccount,.row-AccountCode,#AccountAttributes").removeClass("hide");
                        $this.GetNextAccountCode("#AccountCode", account.AccountCode);
                        currentForm = "Account";
                        // $this.RenderAttributeWithValue(account.Id);
                    }
                }
            }
            $("#Name").val(account.DisplayName);
            $("#AccountType").select2("val", account.AccountTypeId);

        },
        MakeTreeView: function () {
            $("#coaSearchTree").keyup(function () {
                var v = $("#coaSearchTree").val();
                $("#coaTree").jstree(true).search(v);
            });
            treeView = $("#coaTree").jstree({
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
                //"search": { "show_only_matches": true }
            });
            //$("#coaTree li[role='treeitem']").removeClass("jstree-closed").addClass("jstree-open");
        },
        ExpandTree: function () {
            treeView.jstree("open_all");
            //$("#coaTree").jstree("open_node", "#j1_1");
        },
        CollapseTree: function () {
            treeView.jstree("close_all");
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
            treeView.jstree("select_node", "#node-" + id);
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
                treeView.jstree("open_node", "#node-" + id);
        },
        MakeAccountTree: function () {
            var $this = this;
            var accountHtml = "";
            //var Accounts = $.parseJSON($("#Accountjson").val());
            //var children = Enumerable.From(Accounts).Where(function (e) { return e.ParentId != null && e.ParentId == parent.Id && !hiddenAccounts.Contains(e.Id) }).ToArray();

            var coaAccount = $.grep(Accounts, function (e) { return e.ParentId == null || e.ParentId == 0 && $.inArray(e.Id, PageSetting.HiddenAccounts) === -1 });
            for (var i in coaAccount) {

                var account = coaAccount[i];
                accountHtml += $this.RenderChildAccount(account);
            }
            $(".wrapper_coaTree").html("<div id='coaTree' class='tree-demo'><ul>" + accountHtml + "</ul></div>");
            if (Accounts == null || Accounts == undefined || Accounts.length == 0)
                $(".wrapper_coaTree").html("There are not accounts, to create account click above buttons.");
            else
                $this.MakeTreeView();
        },
        RenderChildAccount: function (parent) {
            var $this = this;
            var types = { 1: "default", 2: "head", 3: "control", 4: "account" };
            var accountHtml = "";
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var children = $.grep(Accounts, function (e) { return e.ParentId != null && e.ParentId == parent.Id && $.inArray(e.ParentId, PageSetting.HiddenAccounts) === -1; });

            var name = parent.DisplayName;
            if (parent.Level == 4 && parent.AccountCode + "" != "null")
                name = parent.AccountCode + "-" + name;
            if (children == null || children.length == 0) {
                accountHtml += " <li id=\"node-" + parent.Id + "\" class=\"cursor-pointer\" onclick=\"ChartOfAccount.ShowHideButton(this)\" data-accountId=\"" + parent.Id + "\" data-level=\"" + parent.Level + "\" data-jstree='{\"type\":\"" + types[parent.Level] + "\"}' data-haschild=\"true\" data-typeId=\"" + parent.AccountTypeId + "\" data-parent=\"" + parent.ParentId + "\">" + name + "</li>";
            }
            else {
                accountHtml += "<li id=\"node-" + parent.Id + "\" data-level=\"" + parent.Level + "\" data-parent=\"" + parent.ParentId + "\" data-haschild=\"true\"  data-jstree='{\"type\":\"" + types[parent.Level] + "\"}' >" +
                "<span class=\"cursor-pointer\" onclick=\"ChartOfAccount.ShowHideButton(this)\" data-accountId=\"" + parent.Id + "\" data-level=\"" + parent.Level + "\" data-haschild=\"true\" data-typeId=\"" + parent.AccountTypeId + "\" data-parent=\"" + parent.ParentId + "\">" + name + "</span><ul>";
                for (var i in children) {
                    accountHtml += $this.RenderChildAccount(children[i]);
                }
                accountHtml += "</ul></li>";
            }
            return accountHtml;
        },
        RedrawTree: function (account, isDelete) {
            var $this = this;
            try {
                if (isDelete) {
                    //var ac = $.grep(Accounts, function (e) { return e.Id == account; })[0];
                    //Accounts.pop(ac);
                    Accounts = Enumerable.From(Accounts).Where(function (p) { return p.Id != account }).ToArray();

                }
                else {
                    var bac = $.grep(Accounts, function (e) { return e.Id == account.Id; })[0];
                    if (bac == null)
                        Accounts.push(account);
                    else {
                        bac.DisplayName = account.Name;
                        bac.Name = account.Name;
                        bac.AccountCode = account.AccountCode;
                        bac.ParentId = account.ParentId;
                    }
                }
                $this.MakeAccountTree();
            } catch (e) {
            }
            //Common.SetData("COA", Accounts);
        },
        Save: function () {
            var $this = this;
            if (currentForm == "MainAccount") {
                $this.SaveMainAccount();
            }
            else if (currentForm == "ControlAccount") {
                $this.SaveControlAccount();
            }
            else if (currentForm == "SubAccount") {
                $this.SaveSubAccount();
            }
            else if (currentForm == "Account") {
                $this.SaveAccount();
            }

        },
        SaveMainAccount: function () {
            var $this = this;
            var name = $("#Name").val();
            $("#errordiv").fadeOut("slow");
            var scope = $("#account-edit-form");
            if (Common.Validate(scope)) {
                var input = {
                    AccountTypeId: $("#AccountType").val(),
                    Name: $("#Name").val(),
                    DisplayName: $("#Name").val(),
                    Level: 1,
                    ParentId: null,
                    IsLive: true,
                    IsSystemAccount: true
                };
                if (newOrEdit == "Edit") input.Id = parseInt($(currentElement).attr("data-accountid"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/RouteAccount/",
                    type: "POST",
                    data: input,
                    success: function (res) {
                        if (res.Success) {
                            input.Id = res.Data;
                            $this.RedrawTree(input);
                            $this.ExpandTreeNode(input.Id);
                            $this.Add();

                        }
                        else {
                            Common.ShowError(res.Error.split(","));
                        }

                    },
                    error: function (e) {
                    }
                });
            }
        },
        SaveControlAccount: function () {
            var $this = this;
            var name = $("#Name").val();
            $("#errordiv").fadeOut("slow");
            var scope = $("#account-edit-form");
            if (Common.Validate(scope)) {
                var accountType = $("#AccountType").val();
                var input =
                    {
                        //AccountTypeId: accountType == "" ? null : accountType,
                        Name: $("#Name").val(),
                        DisplayName: $("#Name").val(),
                        Level: 2,//parseInt($(currentElement).attr("data-level")) + 1,
                        ParentId: $("#MainAccount").val(),
                        IsLive: true,
                        IsSystemAccount: false

                    };
                if (newOrEdit == "Edit") input.Id = parseInt($(currentElement).attr("data-accountid"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/ParentAccount/",
                    type: "POST",
                    data: input,
                    //contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            input.Id = res.Data;
                            $this.RedrawTree(input);
                            $this.ExpandTreeNode(input.Id);
                            $this.Add();
                        }
                        else {
                            Common.ShowError(res.Error.split(","));
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        SaveSubAccount: function () {
            var $this = this;
            var name = $("#Name").val();
            $("#errordiv").fadeOut("slow");
            var scope = $("#account-edit-form");
            if (Common.Validate(scope)) {
                var accountType = $("#AccountType").val();
                var input =
                    {
                        //AccountTypeId: accountType == "" ? null : accountType,
                        Name: $("#Name").val(),
                        DisplayName: $("#Name").val(),
                        Level: 3,//parseInt($(currentElement).attr("data-level")) + 1,
                        ParentId: $("#ControlAccount").val(),
                        IsLive: true,
                        IsSystemAccount: false

                    };
                if (newOrEdit == "Edit") input.Id = parseInt($(currentElement).attr("data-accountid"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/ParentAccount/",
                    type: "POST",
                    data: input,
                    //contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (res) {
                        if (res.Success) {

                            input.Id = res.Data;
                            $this.RedrawTree(input);
                            $this.ExpandTreeNode(input.Id);
                            $this.Add();
                        }
                        else {
                            Common.ShowError(res.Error.split(","));
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },

        GetLeafAccounts: function (parentId) {
            var l4Accounts = new Array();

            var parentAccounts = $.grep(Accounts, function (e) { return e.ParentId == parentId; });
            for (var i in parentAccounts) {
                var account = parentAccounts[i];
                if (account.Level === 4) {
                    l4Accounts.push(account);
                } else
                    Array.prototype.push.apply(l4Accounts, GetLeafAccounts(account.Id));
            }
            return l4Accounts;


        },
        SaveAccount: function () {
            var $this = this;
            var name = $("#Name").val();
            $("#errordiv").fadeOut("slow");
            var scope = $("#account-edit-form");
            if (Common.Validate(scope)) {
                var accountAttributes = new Array();
                var accountType = $("#AccountType").val();
                $("#AccountAttributes").find("input[type=text],input[type=hidden],select,textarea").each(function () {
                    accountAttributes.push({
                        AttributeId: $(this).attr("data-attributeid"),
                        AttributeName: $(this).attr("data-attributename"),
                        Value: $(this).val()
                    });
                });
                var input =
                    {
                        //AccountTypeId: null,
                        Name: $("#Name").val(),
                        DisplayName: $("#Name").val(),
                        AccountCode: $("#AccountCode").val(),
                        Level: 4,//parseInt($(currentElement).attr("data-level")) + 1,
                        ParentId: $("#SubAccount").val(),
                        IsLive: true,
                        IsSystemAccount: false,
                        AccountAttributes: accountAttributes
                    };
                if (newOrEdit == "Edit") input.Id = parseInt($(currentElement).attr("data-accountid"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/LeafAccount/",
                    type: "POST",
                    data: input,
                    //contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            //input[AccountAttributes] = null;
                            //$this.LoadDefaultDropdowns();
                            input.Id = res.Data;
                            $this.RedrawTree(input);
                            $this.GetNextAccountCode("#AccountCode");
                            $this.ExpandTreeNode(input.Id);
                            $this.Add();
                        }
                        else {
                            Common.ShowError(res.Error.split(","));
                        }

                    },
                    error: function (e) {
                    }
                });
            }
        },

        DeleteAccount: function () {
            var $this = this;
            Common.ConfirmDelete(function () {
                var id = $(currentElement).attr("data-accountid");
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + "/LeafAccount/" + id,
                    type: "DELETE",
                    success: function (res) {
                        if (res.Success) {
                            $this.LoadDefaultDropdowns();
                            $this.RedrawTree($(currentElement).attr("data-accountid"), true);
                            $this.ExpandTreeNode($(currentElement).attr("data-parent"));
                            $this.Add();
                        }
                        else {
                            Common.ShowError(res.Error.split(","));
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        CloseRouteAccount: function () {
            this.ShowHide("#test", true);
        },
        CloseParentAccount: function () {
            this.ShowHide("#test", true);
        },
        CloseLeafAccount: function () {
            this.ShowHide("#test", true);
        },
        SetAccountFullName: function () {
            var $this = this;
            var id = $(currentElement).attr("data-accountid");
            var level = $(currentElement).attr("data-level");
            //var Accounts = $.parseJSON($("#Accountjson").val());
            var account = $.grep(Accounts, function (e) { return e.Id == id; })[0];
            var name = account.DisplayName;
            account = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
            if (account != null) {
                name = account.DisplayName + " > " + name;
                account = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
                if (account != null) {
                    name = account.DisplayName + " > " + name;
                    account = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
                    if (account != null) {
                        name = account.DisplayName + " > " + name;
                        account = $.grep(Accounts, function (e) { return e.Id == account.ParentId; })[0];
                    }
                }
            }
            $(".account-fullname").html(name);
        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                try {
                    PageSetting[token.Key] = $.parseJSON(token.Value);
                } catch (e) {
                    PageSetting[token.Key] = token.Value;
                }

            }
        },
        ShowHideButton: function (element) {
            var $this = this;
            $this.SetNewOrEdit("Edit");
            currentElement = element;
            $("#divDeleteAccount").removeClass("hide");
            $this.LoadAccountDropdowns($(currentElement).attr("data-accountid"));
            $("#divDeleteAccount").removeClass("hide");
            var currentlevel = parseInt($(currentElement).attr("data-level"));
            $this.SetAccountFullName();
        },
        ShowHide: function (element, showmain) {
            if (showmain) {
                $("#mainform").addClass("hide");
                $("#detailtree").removeClass("hide");
            }
            else {
                $("#mainform").removeClass("hide");
                $("#mainformcontainer > div").addClass("hide");
                $(element).removeClass("hide");
                $("#detailtree").addClass("hide");
            }
        }
    };
}();

