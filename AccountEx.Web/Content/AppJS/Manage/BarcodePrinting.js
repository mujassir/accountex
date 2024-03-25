
var BarcodePrinting = function () {
    var API_CONTROLLER = "Product";
    var title = "";
    return {
        init: function () {
            var $this = this;

            jQuery('body').on('change', '#chk-select-all', function () {
                var set = jQuery('#item-container tbody tr:visible td:nth-child(1) :checkbox');
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    $(this).prop("checked", checked).trigger("change");
                });
                jQuery.uniform.update(set);
            });
            $this.LoadData();

        },
        LoadData: function () {

            var qs = "?loadBarcodeData=true";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading products...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var products = res.Data;
                        var users = res.Data.Users;
                        var vtype = Common.GetQueryStringValue("vocuhertype");
                        var vclass = "show";
                        if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                            vclass = "hide";
                        }
                        for (var i in products) {
                            var item = products[i];
                            html += "<tr>";
                            html += "<td><input type='hidden' class='Id' value='" + item.Id + "' data-db-column='Id'><input type='checkbox' class='IsSelected' data-db-column='IsSelected' data-checktrack='false' value='false' data-toggle='tooltip' title='select to print Barcode'></td>";
                            html += "<td>" + item.Code + "</td>";
                            html += "<td>" + item.Name + "</td>";
                            html += "<td>" + item.ArticleNo + "</td>";
                            html += "<td>" + Common.GetFloat(item.PurchasePrice).format() + "</td>";
                            html += "<td>" + Common.GetFloat(item.SalePrice).format() + "</td>";
                            html += "<td>" + item.Manufacturer + "</td>";
                            html += "</tr>";
                        }
                        if (products.length == 0) {

                            html += "  <tr><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                            $(".data-container").addClass("hide");
                        }
                        else
                            $(".data-container").removeClass("hide");

                        $("#item-container tbody").html(html);
                        Common.SetCheckChange();
                        App.initUniform();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                Common.ShowMessage(true, { message: "Barcode printed successfully." });

            });
        },
        SaveRecord: function (callback) {
            var $t
            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (Common.Validate(scope)) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.IsSelected=='true'").ToArray();
                var err = "";
                if (Items.length <= 0) {
                    err += "Please select atleast on product to print barcode.,";
                }
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                record[""] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?printBarcode=true",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
                            //window.location = res.Data;
                            window.open(res.Data);
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },

    };
}();