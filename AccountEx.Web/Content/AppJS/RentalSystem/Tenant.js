
var Tenant = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Tenant";
    var LIST_LOADED = false;
    var Counter = 0;
    return {
        init: function () {
            var $this = this;
            $("#Code").keyup(function (e) {
                if (e.which == 13)

                    $this.GetAccountByCode();
            });
            $("#ParentId").change(function () {
                Common.GetNextAccountCode(API_CONTROLLER);
            });
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });
            $this.InitCNICMask();
            $this.GetCollectionAccounts();
            $this.ListView();
            Common.GetNextAccountCode(API_CONTROLLER);
            $('[data-db-column="ContactNumber"]').inputmask("mask", { "mask": "9999-9999999" });
        },
        InitCNICMask: function () {
            $(".cnic").inputmask("99999-9999999-9")
        },
        GetCollectionAccounts: function () {
            var collectionAccounts = Common.GetAllLeafAccounts();
            var html = "<option></option>";
            for (var i in collectionAccounts) {
                var account = collectionAccounts[i];
                html += "<option value='" + account.Id + "'>" + account.Name + "</option>";
            }
            $("#CollectionAccountId").html(html);

        },
        AddPartner: function () {
            var $this = this;
            Counter++
            var record = new Object();
            var templateHtml = $("#template-item").html();
            record["{{PartnerNo}}"] = 2 + Counter;
            templateHtml = templateHtml.allReplace(record);
            if (Counter % 2 != 0) {
                var html = "";
                html += "<div class='row added-partner'>"
                html += templateHtml;
                html += "</div>";
                $(".partners-container").append(html);
            }
            else {

                $(".partners-container").find("div.row:nth-last-child(1)").append(templateHtml)
            }
            $this.InitCNICMask();
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
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
            $this.CustomClear();
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            Common.GetNextAccountCode(API_CONTROLLER);
            $("#Name").focus();
        },
        GetAccountByCode: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
            var code = $("#Code").val();
            if (code.trim() == "")
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetAccountDetailByCode&code=" + code + "&formid=" + $("#AccountDetailFormId").val(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data != null) {
                            Common.MapEditData(res.Data, "#form-info");
                            $("#TempCode").val(res.Data.Code);
                        }
                        else {
                            $this.CustomClear();
                            var msg = "You can create a new account with this code";
                            Common.ShowMessage(true, { message: msg });
                            Common.GetNextAccountCode(API_CONTROLLER);
                        }


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        ReinializePlugin: function () {
            AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
            Common.Clear();
            $(".added-partner").remove();
            $("tbody.files").html("");
            Counter = 0;
        },
        Save: function () {
            var $this = this;
            var record = new Object()
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var data = Common.SetValue($(".form"));
                if (Common.GetInt(record.Id) > 0) {
                    if (record.Code != record.TempCode) {
                        err = "<li>You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ") </li>";
                        Common.ShowError(err);
                        return;
                    }
                }
                var Items = $this.SavePartnersData();
                Items = Enumerable.From(Items).Where("$.Name.trim()!=''").ToArray();
                data["NumberOfPartners"] = Items.length;
                var files = new Array();
                $("tbody.files tr td:last-child").each(function () {
                    var file = $.parseJSON($(this).children("input.data").attr("data-file"));
                    if (typeof file != 'undefined' && file != null) {
                        file["ApplicationId"] = record.Id;
                        files.push(file);
                    }

                });

                record["AccountDetail"] = data;
                record["TenantPartners"] = Items;
                record["TenantDocuments"] = files;
              
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=tenant",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving tenant ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
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
        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading salesman ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var tenant = res.Data.Tenant;
                        var partners = res.Data.Partners;
                        var $form = $('#fileupload');
                        var files = res.Data.TenantDocument;
                        Common.MapEditData(tenant, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        $("#TempCode").val(res.Data.Code);

                        $form.fileupload('option', 'done').call($form, $.Event('done'), { result: { files: files } });
                        Common.goToByScroll("form_wizard_1");

                        if (partners.length > 0) {
                            var i = 1;
                            for (var item in partners) {
                                var container = "";
                                var item = partners[item];
                                if (i == 1)
                                    container = $(".partner-container:first-child");
                                else if (i == 2)
                                    container = $(".partner-container:last-child");
                                else {
                                    $this.AddPartner();
                                    container = $(".partners-container").find("div.row:nth-last-child(1) .partner-container:nth-last-child(1) ");
                                }
                                //if(container.trim() != "")
                                $this.MapPartnerData(item, container);
                                i++
                            }

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
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting salesman ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
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
        DeleteFile: function (element) {
            $(element).parent().parent().remove();


        },
        DeleteAllFile: function (element) {
            var _this = this;
            $("tbody.files tr.template-download td:nth-child(4) input:checked").each(function () {
                _this.DeleteFile($(this));
            });

        },
        GetNextAccountCode: function () {
            var $this = this;
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=GetNextAccountCode",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#Code").val(res.Data);
                        $("#Name").focus();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        SavePartnersData: function (table) {
            var $this = this;
            var Items = new Array();
            var masterData = new Object();
            if (typeof table == "undefined" || table == null)
                table = "#item-container";
            $(".partner-container").each(function () {
                var item = new Object();
                $(this).find("[data-db-column]").each(function () {
                    var value = $(this).val();
                    item[$(this).attr('data-db-column')] = value;
                });
                Items.push(item);
            });

            return Items;
        },
        MapPartnerData: function (token, dataConatiener) {
            var $this = this;
            for (var key in token) {
                var element = $(dataConatiener).find("[data-db-column=" + key + "]");
                var value = token[key];
                $(element).val(value);
            }
        },
    };
}();
