
var Services = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Services";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "code";
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
            $(document).on("keyup", ".Code", function (event) {
                if (event.which == 13) {
                    if ($(this).val() == "") {
                            $(".btn.btn-primary.green").focus();
                    }
                    else {
                        //$("#saleitem tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").removeAttr('disabled');

                        $("#saleitem tbody tr:nth-last-child(1) td:nth-child(3) input.Quantity").focus().select();

                    }

                }
            });
            $(document).on("keyup", ".Quantity", function (event) {
                var tr = $(this).parent().parent();
                var qty = Common.GetInt($(tr).find("input.Quantity").val());
                if (event.which == 13) {
                    $this.AddItem();
                }
            });
            this.LoadPageSetting();
            $this.LoadAccountDetail();
            $this.ListView();
            Common.GetNextAccountCode(API_CONTROLLER);
            
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
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
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            Common.GetNextAccountCode(API_CONTROLLER);
            $("#Name").focus();
            $this.CustomClear();
            $this.AddItem();
        },
        CalculateTax: function () {
            var tax = Common.GetFloat($("#GrossCost").val()) * Setting.SalesTax;
            $("#SalesTax").val(tax);
            CalculateNetCost();
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
            $("#saleitem tbody").html("");
        },

        Save: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
            var Items = new Array();
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($(".form"));
                if (Common.GetInt(record.Id) > 0) {
                    if (record.Code != record.TempCode) {
                        err = "<li>You cannot change account code.Please save with previous  Account Code (" + record.TempCode + ") </li>";
                        Common.ShowError(err);
                        return;
                    }
                }
                $("#saleitem tbody tr").each(function () {

                    var itemcode = $(this).children(":nth-child(1)").find("input.Code").val();
                    if (typeof itemcode != "undefined" && itemcode.trim() != "") {
                        var itemname = $(this).children(":nth-child(2)").find("input.Name").val();
                        var itemid = Common.GetInt($(this).children(":nth-child(1)").find("input.ItemId").val());
                        var qty = Common.GetFloat($(this).children(":nth-child(3)").find("input.Quantity").val());
                        Items.push({
                            ServiceId: record.AccountId,
                            Quantity: qty,
                            Id: $(this).children(":nth-child(1)").children("#Id").val(),
                            ItemCode: itemcode,
                            ItemName: itemname,
                            ItemId: itemid,
                        });
                    }
                });
                var data =
                    {
                        Service: record,
                        ServiceItems: Items
                    };

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?Key=Key",
                    type: "POST",
                    data: data,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving service...please wait",
                    success: function (res) {
                        if (res.Success) {
                            
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            Common.GetNextAccountCode(API_CONTROLLER);
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

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading service...please wait",
                success: function (res) {

                    if (res.Success) {
                        $("#saleitem tbody").html("");
                        var j = res.Data.Service;
                        Common.MapEditData(j, $("#form-info"));
                        $("#TempCode").val(res.Data.Service.Code);
                        var services = res.Data.ServiceItems;
                        var html = "";
                        for (var i in services) {
                            var item = services[i];
                            html += "<tr>";
                            html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                            html += "<input type='hidden'  id='Id' value='" + item.Id + "'>";
                            html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                            html += "<td><input type='text' disabled='disabled' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                            html += "<td class='align-right'><input type='text' class='Quantity form-control input-small num3' value='" + item.Quantity + "'></td>";
                            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"Services.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td></tr>";
                        }
                        $("#saleitem tbody").append(html);
                        Common.InitNumerics();
                        $this.DetailView();
                        $this.AddItem();
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
                    blockMessage: "Deleting service...please wait",
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
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetClients: function () {
            var $this = this;
            Common.WrapAjax({
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
            Common.WrapAjax({
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
        AddItem: function () {

            var $this = this;
            //if (Common.Validate($("#addrow"))) {

            var code = $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();


            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }
            var html = "<tr>";
            html += "<td><input type='hidden' class='ItemId' id='ItemId'>";
            html += "<input type='hidden' id='Id' value=''><input class='PriceType' type='hidden' id='PriceType' value=''>";
            html += "<input type='text' class='Code form-control typeahead input-small'></td>";
            html += "<td><input type='text' disabled='disabled' class='Name form-control input-medium'></td>";
            html += "<td class='align-right'><input type='text' class='Quantity form-control input-small num3'></td>";
            html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"Services.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
            html += "</tr>";
            $("#saleitem tbody").append(html);
            Common.InitNumerics();
                setTimeout(function () {
                    $("#saleitem tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);
            SetFocus = "code";
           
         


            //}
            $this.AutoCompleteInit();


        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            //this.LoadAccounts();
        },
        AutoCompleteInit: function () {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
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
                    var product = $this.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof product != "undefined" && product != null) {
                        $(tr).find(":nth-child(1) input.ItemId").val(product.AccountId);
                        $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(tr).find(":nth-child(3) input.Quantity").val();
                        //$(tr).find(":nth-child(3) input.Quantity").focus();
                        $(".container-message").hide();
                    }
                }
            });

        },
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
        GetByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetByCodeFromCOA: function (code) {
            var accounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel; });
            var data = $.grep(accounts, function (e) { return e.AccountCode.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        DeleteRow: function (elment) {

            var $this = this;
            var parent = $(elment).parent().parent().parent();
            var id = Common.GetInt($(parent).find("td:first input#Id").val());
            if (id == 0) {
                $(parent).remove();
                if ($("#saleitem tbody").children().length <= 0)
                    $this.AddItem();
            }
            else {
                Common.ConfirmDelete(function () {

                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=deleteItem",
                        type: "DELETE",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        blockUI: true,
                        blockElement: "#saleitem",
                        blockMessage: "Deleting...please wait",
                        success: function (res) {

                            if (res.Success) {
                                $(parent).remove();
            if ($("#saleitem tbody").children().length <= 0)
                $this.AddItem();
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

        },
    };
}();
