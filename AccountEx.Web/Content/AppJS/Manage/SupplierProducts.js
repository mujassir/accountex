
var SupplierProducts = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "SupplierProduct";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;
            $this.ListView();
            $(document).on("change", "#SupplierId", function () {
                var supplierId = $("#SupplierId").val();
                $this.LoadProductsOfSupplier(supplierId);               
            });
            $(document).on("change", "#SupplierId,#GenericId,#BrandId", function () {
                $this.LoadProducts();
            });
            $(document).on("change", "#checkAll", function () {
                if ($("#checkAll").is(":checked")) {
                    $("#product-container tbody tr").each(function () {
                        $(this).find("input[type='checkbox']").prop("checked", true);
                    });
                    $.uniform.update();
                }
                else {
                    $("#product-container tbody tr").each(function () {
                        $(this).find("input[type='checkbox']").prop("checked", false);
                    });
                    $.uniform.update();
                }
            });
        },

        LoadProducts: function () {
            var supplierId = $("#SupplierId").val();
            var genericId = $("#GenericId").val();
            var brandId = $("#BrandId").val();
            Common.Validate($(".form-horizontal"))
            {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=LoadProducts&supplierId=" + supplierId + "&GenericId=" + genericId + "&BrandId=" + brandId,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            var products = res.Data;
                            $("#product-container tbody").html("");
                            var html = "";
                            for (var i in products) {
                                var product = products[i];
                                html += "<tr>";
                                html += "<td><input type='checkbox'/></td>";
                                html += "<td><input type='text' class='form-control hide ProductId' value='" + product.Id + "'>" + product.Name + "</td>";
                                html += "</tr>";
                            }
                            $("#product-container tbody").html(html);
                            $("input:checkbox").uniform();
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

        LoadProductsOfSupplier: function (supplierId) {
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            url += "?supplierId=" + supplierId
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
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
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
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
            $("#product-container tbody").html("");
            Common.Clear();

        },

        Save: function () {
            debugger;
            var $this = this;
            var supplierId = $("#SupplierId").val();
            var supplierProducts = new Array();
            $("#product-container tbody tr").each(function () {
                if ($(this).find("input[type='checkbox']").is(":checked")) {
                    supplierProducts.push(
                        {
                            ProductId: $(this).find("input.ProductId").val(),
                            SupplierId: supplierId
                        }
                        );
                }
            });
            var record = { '': supplierProducts };

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving Department...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.ListView();
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.CustomClear();
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                        $("#SupplierId").focus();
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
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading departments...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
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
                    blockMessage: "Deleting departments...please wait",
                    success: function (res) {
                        if (res.Success) {
                            var supplierId = $("#SupplierId").val();
                            $this.LoadProductsOfSupplier(supplierId);
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

    };
}();
