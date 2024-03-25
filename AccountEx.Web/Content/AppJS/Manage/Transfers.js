
var Transfers = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Transfer";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $("#TransferByAccountCode").focus();
            $("#AgreementPD").keyup(function () {
                $this.CalculateTransferCharges();
            });
            $(document).on("blur", "#TransferByAccountCode,#TransferToAccountCode", function () {
                var account = Common.GetByCode($(this).val());
                if (typeof account != "undefined" && account != null) {
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });

            $this.LoadPageSetting();
            $this.GetSettings();
            AppData.AccountDetail = PageSetting.AccountDetails;
            //$this.TransferByAutoCompleteInit();
            $this.TransferToAutoCompleteInit();
            $this.GetShops();
            $this.GetNextVoucherNumber();
        },
        GetSettings: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetSettings",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",

                success: function (res) {
                    if (res.Success) {
                        var tokens = $.parseJSON(res.Data);
                        for (var i in tokens) {
                            var token = tokens[i];
                            PageSetting[token.Key] = token.Value;
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

        CalculateTransferCharges: function () {
            var html = "";
            var area = $("#TotalArea").val();
            var transferAmount = area * PageSetting.TransferCharges;
            $("#TransferChargesAmount").val(transferAmount);
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
            //if (!LIST_LOADED) {
            //    var url = Setting.APIBaseUrl + API_CONTROLLER;
            //    LIST_LOADED = true;
            //    DataTable.BindDatatable(DATATABLE_ID, url);
            //}
            //Common.GetNextAccountCode(API_CONTROLLER);
            $("#Name").focus();
        },
        New: function () {

            var $this = this;
            $this.CustomClear();
            $this.LoadVoucher("nextvouchernumber");
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
            $("#YearlyInc").val(10);
            $("#btndelete,#btnprint").prop("disabled", true);
        },

        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        var shopInfo = res.Data.ShopInfo;
                        var transferByInfo = res.Data.TransferBy;
                        var transferToInfo = res.Data.TransferTo;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            Common.MapEditData(transferByInfo, "#form-info");
                            Common.MapEditData(transferToInfo, "#form-info");
                            Common.MapEditData(shopInfo, "#form-info");
                            $("#Id").val(d.Id);
                            $("#btndelete,#btnprint").prop("disabled", false);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            //var html = "";
                            //html += "<tr>";
                            //html += "<td><input type='text' readonly class='TransferChargesName' value='" + d.TransferChargesName + "'></td>";
                            //html += "<td><input type='text' class='TransferChargesAmount' value='" + d.TransferChargesAmount + "'></td>";
                            //html += "</tr>";
                            //$("#TransferCharges tbody").html(html);
                            //html = "";
                            //html += "<tr>";
                            //html += "<td><input type='text' readonly class='ProcessingFee' value='Processing Fee for Seller'></td>";
                            //html += "<td><input type='text' class='SellerFee' value='" + d.SellerFee + "'></td>";
                            //html += "</tr>";
                            //html += "<tr>";
                            //html += "<td><input type='text' readonly class='ProcessingFee' value='Processing Fee for Buyer'></td>";
                            //html += "<td><input type='text' class='BuyerFee' value='" + d.BuyerFee + "'></td>";
                            //html += "</tr>";
                            //$("#ProcessingFee tbody").html(html);TransferToNumberOfPartners

                            if (transferByInfo.TransferByNumberOfPartners > 0) {
                                $("#TransferBySingle").val("No");
                                $("#TransferByPartnership").val("Yes");
                            }
                            else {
                                $("#TransferBySingle").val("Yes");
                                $("#TransferByPartnership").val("No");
                            }
                            if (transferToInfo.TransferToNumberOfPartners > 0) {
                                $("#TransferToSingle").val("No");
                                $("#TransferToPartnership").val("Yes");
                            }
                            else {
                                $("#TransferToSingle").val("Yes");
                                $("#TransferToPartnership").val("No");
                            }
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        Save: function () {
            var $this = this;
            var data = new Array();
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($(".form"));

                //record.TransferChargesName = $(".TransferChargesName").val();
                //record.TransferChargesAmount = $(".TransferChargesAmount").val();
                //record.SellerFee = $(".SellerFee").val();
                //record.BuyerFee = $(".BuyerFee").val();

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Transfers ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            $this.CustomClear();
                            $this.GetNextVoucherNumber();
                            Common.ShowMessage(true, { message: "Transfers Saved Successfully!" });
                            $("#AccountCode").focus();
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
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        $("#TempCode").val(res.Data.Code);
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
        Delete: function () {
            var $this = this;
            var id = $("#Id").val();
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            $this.GetNextVoucherNumber();
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

        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

        },

        GetShops: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetAllShops",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                       //$this.ShopsAutoCompleteInit(res.Data);
                        AppData.shops = res.Data.Shops;
                        AppData.agreedTenants = res.Data.Tenants;
                        $this.TransferByAutoCompleteInit();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },

        TransferByAutoCompleteInit: function () {
            var tenants = Common.GetLeafAccounts(PageSetting.Tenants);
            var agreedTenants = AppData.agreedTenants;
            var filteredTenants = Enumerable.From(tenants).Join(agreedTenants,
                                "tn=>tn.Id",
                                "tnAg=>tnAg.TenantAccountId",
                                "(tn,tnAg)=>{Id:tn.Id,AccountCode:tn.AccountCode,DisplayName:tn.DisplayName,ShopId:tnAg.ShopId}"
                                ).ToArray();
            var suggestions = new Array();
            for (var i in filteredTenants) {
                var tenant = filteredTenants[i];
                suggestions.push
                    (
                    {
                        id: tenant.AccountId,
                        value: tenant.AccountCode,
                        label: tenant.AccountCode + "(" + tenant.DisplayName + ")",
                        shopid: tenant.ShopId
                    }
                    );
            }

            $("#TransferByAccountCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        var tenant = Common.GetAccountDetailByAccountId(account.Id);
                        var shop = Enumerable.From(AppData.shops).Where(function (e) { return e.Id == ui.item.shopid }).FirstOrDefault();
                        $("#TransferByTenantAccountId").val(account.Id);
                        $("#TransferByTenantName").val(tenant.Name);
                        $("#TransferByCNIC").val(tenant.CNIC);
                        $("#TransferByBusiness").val(tenant.BrandName);
                        $("#TransferByContactNo").val(tenant.ContactNumber);
                        $("#TransferByNumberOfPartners").val(tenant.NumberOfPartners);

                        $("#ShopId").val(shop.Id); 
                        $("#ShopNo").val(shop.ShopNo);
                        $("#TotalArea").val(shop.TotalArea);
                        $("#Block").val(shop.Block);
                        $("#North").val(shop.North);
                        $("#South").val(shop.South);
                        $("#East").val(shop.East);
                        $("#West").val(shop.West);

                        if (tenant.NumberOfPartners > 0) {
                            $("#TransferBySingle").val("No");
                            $("#TransferByPartnership").val("Yes");
                        }
                        else {
                            $("#TransferBySingle").val("Yes");
                            $("#TransferByPartnership").val("No");
                        }
                    }
                }
            });
        },
        TransferToAutoCompleteInit: function () {
            var tenants = Common.GetLeafAccounts(PageSetting.Tenants);
            var suggestions = new Array();
            for (var i in tenants) {
                var tenant = tenants[i];
                suggestions.push
                    (
                    {
                        id: tenant.AccountId,
                        value: tenant.AccountCode,
                        label: tenant.AccountCode,
                    }
                    );
            }

            $("#TransferToAccountCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        var tenant = Common.GetAccountDetailByAccountId(account.Id);
                        $("#TransferToTenantAccountId").val(account.Id);
                        $("#TransferToTenantName").val(tenant.Name);
                        $("#TransferToCNIC").val(tenant.CNIC);
                        $("#TransferToBusiness").val(tenant.BrandName);
                        $("#TransferToContactNo").val(tenant.ContactNumber);
                        $("#TransferToNumberOfPartners").val(tenant.NumberOfPartners);

                        if (tenant.NumberOfPartners > 0) {
                            $("#TransferToSingle").val("No");
                            $("#TransferToPartnership").val("Yes");
                        }
                        else {
                            $("#TransferToSingle").val("Yes");
                            $("#TransferToPartnership").val("No");
                        }
                    }
                }
            });
        },
        ShopsAutoCompleteInit: function (shops) {
            $this = this;
            var suggestions = new Array();
            for (var i in shops) {
                var shop = shops[i];
                suggestions.push
                    (
                    {
                        id: shop.Id,
                        value: shop.ShopNo,
                        label: shop.ShopNo,
                    }
                    );
            }

            $("#ShopNo").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    //var account = $.grep(shops, function (e) { return e.ShopNo == ui.item.value });
                    var account = Enumerable.From(shops).Where("$.ShopNo=='" + ui.item.value + "'").FirstOrDefault();
                    if (typeof account != "undefined" && account != null) {
                        $("#ShopId").val(account.Id);
                        $("#TotalArea").val(account.TotalArea);
                        $("#Block").val(account.Block);
                        $("#North").val(account.North);
                        $("#South").val(account.South);
                        $("#East").val(account.East);
                        $("#West").val(account.West);
                    }
                }
            });
        }
    };
}();
