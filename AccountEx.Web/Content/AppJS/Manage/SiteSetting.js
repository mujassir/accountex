var SiteSetting = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "SiteSetting";
    var UPLOAD_FOLDER = "ApplicationLogo";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            var $this = this;

            $("select").each(function () {
                $(this).select2();
            });
            $("#LogoUrl").fileinput({
                uploadUrl: "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER,
                //allowedFileTypes: "['image']",
                //allowedFileExtensions: "['jpg', 'gif', 'png']",
                dropZoneEnabled: false,
                overwriteInitial: true,
                showCaption: false,
                browseLabel: "Pick Logo URL"
                //initialPreview: [
                //'<img src="../../Content/images/app-logo.png" class="file-preview-image">'

                //],
                //initialPreviewConfig: [
                //{ caption: "Desert.jpg", width: "120px", url: "/site/file-delete", key: 1 }

                //],
            });
            //$(document).on("input[data-coa='coa']").keypress(function (e) {
            //    $this.AutoCompleteInit();
            //});
            $("#LoginLogoUrl").fileinput({
                uploadUrl: "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER,
                //allowedFileTypes: "['image']",
                //allowedFileExtensions: "['jpg', 'gif', 'png']",
                dropZoneEnabled: false,
                overwriteInitial: true,
                showCaption: false,
                browseLabel: "Pick Login Logo"
                //initialPreview: [
                //'<img src="../../Content/images/app-logo.png" class="file-preview-image">'

                //],
                //initialPreviewConfig: [
                //{ caption: "Desert.jpg", width: "120px", url: "/site/file-delete", key: 1 }

                //],
            });
            $("#BillLogoUrl").fileinput({
                uploadUrl: "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER,
                //allowedFileTypes: "['image']",
                //allowedFileExtensions: "['jpg', 'gif', 'png']",
                dropZoneEnabled: false,
                overwriteInitial: true,
                showCaption: false,
                browseLabel: "Pick Bill Logo"
                //initialPreview: [
                //'<img src="../../Content/images/app-logo.png" class="file-preview-image">'

                //],
                //initialPreviewConfig: [
                //{ caption: "Desert.jpg", width: "120px", url: "/site/file-delete", key: 1 }

                //],
            });
            $this.AutoCompleteInit();
            Common.BindFileInputEvents("LogoUrl", "PictureUrl");
            Common.BindFileInputEvents("LoginLogoUrl", "LoginPictureUrl");
            Common.BindFileInputEvents("BillLogoUrl", "BillPictureUrl");

            $this.LoadSetting();
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $this.CustomClear();
            $this.DetailView();
        },
        DetailView: function () {
            $("#div-form").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        ListView: function () {
            debugger;
            var $this = this;
            $("#div-form").addClass("hide");
            $("#div-table").removeClass("hide");
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        LoadSetting: function () {
            //var url = ".../api/" + API_CONTROLLER + "/";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                //url: url,
                type: "GET",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;

                        for (var i = 0; i < res.Data.length; i++) {
                            var setting = res.Data[i];
                            var element = $("[name='" + setting.Key + "']")[0];
                            if (typeof element != "undefined" && element != null) {
                                if (element.nodeName.toLowerCase() == "input") {
                                    if ($(element).is(":text"))
                                        $(element).val(setting.Value);
                                    if ($(element).is(":radio")) {
                                        $("input[name='" + setting.Key + "'][value=" + setting.Value + "]").prop("checked", true);
                                    }
                                }
                                else if (element.nodeName.toLowerCase() == "select") {
                                    //$(element).select2('val', setting.AccountId);
                                    $(element).select2("val", setting.Value);
                                }
                                else if (element.nodeName.toLowerCase() == "textarea") {

                                    $(element).val(setting.Value);
                                }
                            }
                            if (setting.Key == "Report.Header")
                                CKEDITOR.instances.ReportHeader.setData(setting.Value);
                            $(element).attr("data-id", setting.Id);
                        }
                        $.uniform.update();
                        try {
                            Common.RefreshFileInput("LogoUrl", "PictureUrl", $("#PictureUrl").val(), UPLOAD_FOLDER);
                            Common.RefreshFileInput("LoginLogoUrl", "LoginPictureUrl", $("#LoginPictureUrl").val(), UPLOAD_FOLDER);
                            Common.RefreshFileInput("BillLogoUrl", "BillPictureUrl", $("#BillPictureUrl").val(), UPLOAD_FOLDER);
                        } catch (e) {

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
        Close: function () {
            $("#div-form").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            var $this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
        },
        AutoCompleteInit: function () {
            var $this = this;
            //var products = Common.GetLeafAccounts(PageSetting.Products);
            Accounts = AppData.COA;
            var suggestion = new Array();
            var mainaccounts = $.grep(Accounts, function (e) { return e.Level == 1 });
            for (var i in mainaccounts) {
                var account = mainaccounts[i];
                suggestion.push({
                    id: account.Id,
                    value: account.DisplayName,
                    label: account.DisplayName

                });
                var controlaccounts = $.grep(Accounts, function (e) { return e.ParentId == account.Id });
                for (var j in controlaccounts) {
                    var controlaccount = controlaccounts[j];
                    suggestion.push({
                        id: controlaccount.Id,
                        value: controlaccount.DisplayName,
                        label: "-" + controlaccount.DisplayName

                    });
                    var subaccounts = $.grep(Accounts, function (e) { return e.ParentId == controlaccount.Id });
                    for (var k in subaccounts) {
                        var subaccount = subaccounts[k];
                        suggestion.push({
                            id: subaccount.Id,
                            value: subaccount.DisplayName,
                            label: "--" + subaccount.DisplayName

                        });
                        var childaccounts = $.grep(Accounts, function (e) { return e.ParentId == subaccount.Id });
                        for (var r in childaccounts) {
                            var childaccount = childaccounts[r];
                            suggestion.push({
                                id: childaccount.Id,
                                value: childaccount.DisplayName,
                                label: "---" + childaccount.DisplayName

                            });
                        }


                        $("input[data-coa='coa']").autocomplete({
                            source: suggestion,
                            selectFirst: true,
                            autoFocus: true,
                            select: function (event, ui) {

                            }

                        });

                    }
                }
            }
        },
        Save: function (callback) {
            var $this = this;
            var settings = new Array();
            $(".form-horizontal input[type='text'],.form-horizontal select,.form-horizontal textarea").each(function () {
                if ($(this).hasClass("ckeditor")) {
                    settings.push({
                        Key: $(this).attr("name"),
                        Id: Common.GetInt($(this).attr("data-id")),
                        Value: CKEDITOR.instances[$(this).attr("id")].getData(),
                    });
                }
                else if ($(this).attr("name") != undefined && $(this).attr("name") != null)
                    settings.push({
                        Id: Common.GetInt($(this).attr("data-id")),
                        Key: $(this).attr("name"),
                        Value: $(this).val()
                    });
            });
            $(".form-horizontal input[type='radio']:checked").each(function () {
                settings.push({
                    Key: $(this).attr("name"),
                    Value: $(this).val()
                });
            });
            var st = $.grep(settings, function (e) { return e.Key == "Report.Header" })[0];
            st.Value = CKEDITOR.instances.ReportHeader.getData();
            var record = {
                Settings: settings,
            };
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#div-form",
                blockMessage: "Saving settings ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.LoadSetting();
                        Common.ShowMessage(true, { message: "Setting saved successfully" });
                        Common.GoToTop();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        BindFileInputEvents: function (bindelement, setelement) {
            $("#" + bindelement).on("fileimageloaded", function (event, file, previewId, index, reader) {
                $(".kv-fileinput-upload").click();
            });
            $("#" + bindelement).on("filecleared", function (event) {
                console.log("filecleared");
                $("#" + setelement).val("");
            });
            $("#" + bindelement).on("filedeleted", function (event) {
                console.log("filedeleted");

            });
            $("#" + bindelement).on("fileuploaded", function (event, data, previewId, index) {

                var form = data.form, files = data.files, extra = data.extra,
                    response = data.response, reader = data.reader;
                var files = response.Files;
                $("#" + setelement).val(files[0].Url);
                console.log("File uploaded triggered");

                //console.log(files);
                //console.log(response);
                console.log(data);
            });
        },
        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-form",
                blockMessage: "Loading settings ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(res.Data, "#div-form");
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




