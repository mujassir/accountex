
//var Setting = new Object();
var LIST_LOADED = false;
var API_CONTROLLER = "FormSetting";
var DATATABLE_ID = "mainTable";
var PageSetting = new Object();
var PageData = new Object();
function RebindData() {
    DataTable.RefreshDatatable(DATATABLE_ID);
}
function SetFormLabel() {
    var FormTitle = $.grep(Setting.FormSetting, function (e) { return e.KeyName == "FormTitle" })[0].Value;
    $(".formtitle").html(FormTitle);
}
function MapJsonData() {
    $("#jsondata :input").each(function () {
        try {
            Setting[$(this).attr("Id")] = $.parseJSON($(this).val());
        } catch (e) {
            Setting[$(this).attr("Id")] = $(this).val();
        }
    });
}
function DetailView() {
    $("#form-info").removeClass("hide");
    $("#div-table").addClass("hide");
}
function Add() {
    DetailView();
    CustomClear();
   //Common. Clear();
    $(".row-Account").addClass("hide");
}

function ListView() {
    $("#form-info").addClass("hide");
    $("#div-table").removeClass("hide");
    if (!LIST_LOADED) {
        var url = Setting.APIBaseUrl + API_CONTROLLER;
        LIST_LOADED = true;
        DataTable.BindDatatable(DATATABLE_ID, url);
    }
}

var max = 0;
function AddItem() {
    if (Common.Validate($("#addrow"))) {
        var qty = $("#qty").val();
        var rate = $("#Rate").val();
        var amount = $("#Amount").val();
        var item = $("#Item").val();
        var itemtext = $("#Item option:Selected").text();
        var html = "<tr>";
        html += "<td><input type='hidden' id='ItemId' value='" + item + "'>";
        html += "<input type='hidden' id='Id' value=''>";
        html += "" + itemtext + "</td>";
        html += "<td>" + qty + "</td>";
        html += "<td>" + rate + "</td>";
        html += "<td>" + amount + "</td>";
        html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
        html += "</tr>";
        $("#saleitem tbody").append(html);
        $("#qty,#Rate,#Amount").val("");
        GetWholeTotal();
        $("#Item").focus();
    }
}
function ReinializePlugin() {
    AllowNumerics();
}
function DeleteRow(elment) {
    $(elment).parent().parent().parent().remove();
    GetWholeTotal();
}
function CloseItem() {
    Common.Clear();
    $("#form-info").addClass("hide");
    $("#div-table").removeClass("hide");
    $("#form-info-item").addClass("hide");
    $("#masterdetail").removeClass("hide");
    $("#div-table-item").addClass("hide");
}
function Save() {
    if (Common.Validate($("#mainform"))) {
        var input = {
            Id: Common.GetInt($("#Id").val()),
            VoucherType: $("#VoucherType").val(),
            KeyName: $("#KeyName").val(),
            Value: $("#Value").val(),
            UseCOA: $("#UseCOA").is(":checked"),
        };
        Common.WrapAjax({
            url: Setting.APIBaseUrl + API_CONTROLLER,
            type: "POST",
            data: input,
            success: function (res) {
                if (res.Success) {
                    ListView();
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
}
function GetQuantityPriceTotal() {
    var Quantity = 0;
    var Rate = 0;
    Quantity = GetInt($("#qty").val());
    Rate = GetInt($("#Rate").val());
    $("#Amount").val(Quantity * Rate);

}
function GetWholeTotal() {
    var Quantity = 0;
    var Price = 0;
    $("#saleitem tbody tr").each(function () {
        Quantity += GetInt($(this).children(":nth-child(2)").text());
        Price += GetInt($(this).children(":nth-child(4)").text());
    });
    $("#qtytotal").val(Quantity);
    $("#amounttotal").val(Price);
    GetNetTotal();
}
function GetNetTotal() {
    var total = GetInt($("#amounttotal").val());
    var discount = GetInt($("#Discount").val());
    var nettotal = GetInt(total - discount);
    $("#Nettotal").val(nettotal);
}
function CustomClear() {
    $("#saleitem tbody").html("");
    $("#Id").val("");
}
function Edit(id) {
    Common.WrapAjax({
        url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            if (res.Success) {
                DetailView();
                var j = res.Data;
                $("#Id").val(j.Id);
                $("#VoucherType").val(j.VoucherType);
                $("#KeyName").val(j.KeyName);
                $("#Value").val(j.Value);
                $("#UseCOA").prop("checked", j.UseCOA);
                $.uniform.update("#UseCOA");
                if (j.UseCOA) {
                    $(".row-Account").removeClass("hide");
                    $("#AccountId").select2("val", j.Value);
                }
                else
                    $(".row-Account").addClass("hide");

            }
            else {
                 Common.ShowError(res.Error);
            }

        },
        error: function (e) {
        }
    });
}

function DeleteMultiple(id) {
    ConfirmDelete(function () {
        Common.WrapAjax({
            url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
            type: "DELETE",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                if (res.Success) {
                    RebindData();
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
function Delete(id) {
    ConfirmDelete(function () {
        Common.WrapAjax({
            url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
            type: "DELETE",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                if (res.Success) {
                    RebindData();
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


function BindSelect(accounts, element, addBlankRow) {
    var html = "";
    if (addBlankRow)
        html += "<option></option>";
    for (var i = 0; i < accounts.length; i++) {
        var token = accounts[i];
        html += "<option value='" + token.Id + "'>" + token.Name + "</option>";
    }
    $(element).html(html).select2();
}
function LoadAccounts() {
    var qs = "key=GetBothLeafAccounts";
    qs += "&MasterAccountId=" + PageSetting.MasterAccountId;
    qs += "&ItemAccountId=" + PageSetting.ItemAccountId;
    Common.WrapAjax({
        url: Setting.APIBaseUrl + "COA/" + "?" + qs,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            debugger;
            if (res.Success) {
                PageData.MasterAccounts = res.Data.MasterAccounts;
                PageData.ItemAccounts = res.Data.ItemAccounts;
                BindSelect(res.Data.MasterAccounts, "#AccountId", true);
                BindSelect(res.Data.ItemAccounts, "#Item", false);
            }
            else {
                 Common.ShowError(res.Error);
            }
        },
        error: function (e) {
        }
    });
}
function LoadPageSetting() {
    debugger;
    var voucher = GetQueryStringValue("type");
    var formSetting = $.parseJSON($("#FormSetting").val());
    var tokens = $.grep(formSetting, function (e) { return e.VoucherType == voucher });
    for (var i = 0; i < tokens.length; i++) {
        var token = tokens[i];
        PageSetting[token.KeyName] = token.Value;
    }
    LoadAccounts();
    $(".page-title").html(PageSetting.FormTitle + " <small>Add/Edit/Delete " + voucher + "</small>");
}
$(document).ready(function () {
    //$('#AccountId,#Item').select2();
    MapJsonData();
    //SetFormLabel();
    $("#UseCOA").change(function (e) {
        if ($("#UseCOA").is(":checked"))
            $(".row-Account").removeClass("hide");
        else
            $(".row-Account").addClass("hide");
    });
    var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + Setting.Voucher;
    //if (Setting.PageLandingView == "DetailView") {
    //    Add();
    //}
    //else {
    ListView();
    $("#AccountId").select2();

    //}
    //LoadPageSetting();

});