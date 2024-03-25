
var Project = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMProject";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var PageData = new Object();
    var DirectPmcMapping = false;
    var PROJECCT = null
    return {
        init: function () {
            var $this = this;
            $(document).on("click", "#btn-save", function (event) {
                $this.Save();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", ".btn-view", function (event) {
                $this.Edit($(this), true);
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("change", "select#CurrencyId", function (event) {
                $this.GetExchangeRate($(this));
            });
            $(document).on("change", "select#CustomerId", function (event, salePersonId) {
                $this.GetSalePersons(salePersonId);
                $this.BindProducts();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.GetNextVoucherNumber();
            });
            $(document).on("change", "#FromDate", function (event) {

                var toDate = moment($(this).val(), 'DD/MM/YYYY').add(1, 'year').add(-1, 'days').toDate();
                Common.SetDate($("#Date"), toDate);

            });
            $(document).on("change", "select#ModelCustomerId", function (event) {
                $this.GetProjectProduct();
            });

            $("#btnSearch").click(function () {
                $this.Filter();
            });
            $("#FilterFiscalId").change(function () {
                $this.Filter();
            });
            $this.LoadPageSetting();
            $this.GetCurrencies();
            $this.LoadProducts();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {


                if (typeof Url.pmcId != undefined && Url.pmcId != null) {
                    var pmcItemId = Common.GetInt(Url.pmcId);
                    if (pmcItemId > 0) {

                        if (PageSetting.VoucherNo > 0) {
                            $("#VoucherNumber").val(PageSetting.VoucherNo);
                            $this.LoadVoucher("same");
                        }
                        else {
                            DirectPmcMapping = true;
                            $this.Add();

                        }

                    }

                }
                else {
                    $this.Add();
                }
            }
            $(document).on("keyup", "#ExcRate,#Price,#Quantity", function (event) {

                $this.GetQuantityPriceTotal();

            });
            $(document).on("keyup", "#Potential", function (event) {

                $this.GetTargetPotentialPercent();

            });


            $this.ListView();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
            $this.GetNextVoucherNumber();


        },
        DetailView: function () {
            Common.GoToTop();
        },
        OpenSaleTypeChangeModal: function () {
            var $this = this;
            $("#modal-saletype-change").modal("show");
        },
        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                var fiscalId = $("#FilterFiscalId").val();
                url += "?fiscalId=" + fiscalId
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            $this = this;
            Common.Clear();
            var fromDate = new Date(Common.Fiscal.FromDate);
            var toDate = new Date(Common.Fiscal.ToDate);
            Common.SetDate($("#FromDate"), fromDate);
            Common.SetDate($("#Date"), toDate);

        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;

            var annualQty = Common.GetFloat($("#Quantity").val());
            var excRate = Common.GetFloat($("#ExcRate").val());
            var price = Common.GetFloat($("#Price").val());
            var total = annualQty * price * excRate;
            var totalPercent = 0;
            $("#Potential").val(total);
            //$("#Potential").val(total);
            $this.GetTargetPotentialPercent();
            //$this.GetWholeTotal();
        },
        GetTargetPotentialPercent: function (tr) {
            $this = this;
            var potentail = Common.GetFloat($("#Potential").val());
            var actualPotential = Common.GetFloat($("#AnnualPotential").val());
            var totalPercent = potentail / actualPotential * 100;
            if (actualPotential <= 0)
                totalPercent = 0;
            $("#PotentialPercent").val(totalPercent.toFixed(2));
            //$("#Potential").val(total);

            //$this.GetWholeTotal();
        },
        Filter: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            var fiscalId = $("#FilterFiscalId").val();
            url += "?fiscalId=" + fiscalId
            url += Common.MakeQueryStringAll($("#filters-container"));
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
        },
        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            $this.GetNextVoucherNumber();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });

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
        UpdateSaleType: function () {
            var $this = this;
            if (Common.Validate($("#modal-saletype-change"))) {
                var record = Common.SetValue($("#modal-saletype-change"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?changeSaleType=true&customerId=" + record.ModelCustomerId + "&productId=" + Common.GetInt(record.ModelProductId),
                    type: "POST",
                    blockUI: true,
                    blockElement: "#modal-saletype-change",
                    blockMessage: "processing...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Sale type successfully updated." });

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
        GetSalePersons: function (salePersonId) {
            var $this = this;
            var customerId = $("#CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc/?key=GetSalesManByCustomerId&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        Common.BindSelect(data, "#SalePersonId", true);

                        if (data != null && data.length == 1) {
                            $("#SalePersonId").select2("val", data[0].Id);
                        }
                        if (salePersonId > 0) {
                            $("#SalePersonId").select2("val", salePersonId);
                        }

                        $("#SalePersonId").select2("enable", false)
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetProjectProduct: function (salePersonId) {
            var $this = this;
            var customerId = $("#ModelCustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc/?key=GetProjectLinkedProduct&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#modal-saletype-change-container",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        Common.BindSelect(data, "#ModelProductId", true);

                        if (data != null && data.length == 1) {
                            $("#ModelProductId").select2("val", data[0].Id);
                        }
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        LoadVoucher: function (key, isView) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=0&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Project;
                        PROJECCT = d;
                        var pmc = res.Data.PMC;
                        Common.MapEditData(PROJECCT, "#form-info", undefined, isView);
                        
                        if (!DirectPmcMapping && pmc.Id > 0)
                            $this.DoPmcMapping(pmc);
                        $this.BindProducts();
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            //var voucher = this.GetType();
                            $this.GetSalePersons(d.SalePersonId);
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            //if (voucher != "sale") 
                            $("#DCNo").prop("disabled", "disabled");
                            //else
                            //    $("#DCNo").removeProp("disabled");
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (d.CashSale && d.Id > 0) {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }
                            if (d.Id > 0 && d.PMCItems != null && d.PMCItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                var items = d.PMCItems;

                                for (var key in items) {
                                    var item = items[key];
                                    var product = Enumerable.From(products).FirstOrDefault(null, "$.Id===" + item.ProductId + "");
                                    if (product != null) {
                                        item["Vendor"] = product.Vendor;
                                        item["Division"] = product.Division;
                                        item["ProductName"] = product.Name;
                                    }


                                }

                                Common.MapItemData(d.PMCItems);
                                Common.InitNumerics();
                                Common.SetCheckChange();
                                App.initUniform();

                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductName").focus().select();
                                }, 500);
                            }
                        }
                        if (DirectPmcMapping) {
                            DirectPmcMapping = false;
                            PageSetting.PMCItem.PMCItemId = Url.pmcId;
                            $this.DoPmcMapping(PageSetting.PMCItem)
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        Common.goToByScroll();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        GetCurrencies: function (customerid) {
            var $this = this;
            var date = $("#Date").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetCurrencies&date=" + date,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        PageData.Currencies = res.Data;
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetExchangeRate: function () {
            var $this = this;
            var currencyId = Common.GetInt($("select#CurrencyId").val());
            var CurrencyRate = Enumerable.From(PageData.Currencies).FirstOrDefault(null, "$.CurrencyId ==" + currencyId + "");
            var rate = 0;
            if (CurrencyRate != null)
                rate = CurrencyRate.Rate;
            $("#ExcRate").val(rate);
            $this.GetQuantityPriceTotal();

        },

        Edit: function ($element, isView) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            $("#VoucherNumber").val(id);
            $this.LoadVoucher("same", isView);
        },
        DoPmcMapping: function (product) {
            var $this = this;
            if (product != null) {
                product["PMCExcRate"] = product.ExcRate
                product["PMCPrice"] = product.Price

                delete product.VoucherNumber;
                delete product.Id;
                delete product.ProductId;
                delete product.ExcRate;
                delete product.Price;
                delete product.Price;
                var currencyId = product.CurrencyId;
                if (PROJECCT != null) {
                    currencyId = PROJECCT.CurrencyId;
                    delete product.CurrencyId;
                }
                Common.MapEditData(product, "#form-info");
                if (PROJECCT == null) {
                    $("#CurrencyId").select2("val", product.CurrencyId);
                    $("#ExcRate").val(product.PMCExcRate);
                    $("#CustomerId").select2("val", product.CustomerId);
                    $("#CustomerId").trigger("change", [product.SalePersonId]);
                }


            }
        },



        Delete: function ($element) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "deleting...please wait",
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
        BindProducts: function () {
            debugger;
            var $this = this;
            var products = PageData.Products;
            var customerId = Common.GetInt($("#CustomerId").val());
            if (PageSetting.IsSalePerson && customerId>0) {
                var customerCatgeories = PageSetting.CustomerCategories;
                var Ids = Enumerable.From(customerCatgeories).Where(function (x) { return x.CRMCustomerId == customerId }).Select("$.CategroyId").Distinct().ToArray();
                products = Enumerable.From(PageData.Products).Where(function (x) { return $.inArray(Common.GetInt(x.CategoryId), Ids) !== -1 && x.IsOwnProduct == 1 }).ToArray();
            }
            products = Enumerable.From(products).Where(function (x) { return x.IsOwnProduct == 1 })
                    .Select("x => { Id: x['Id'], Name: x['Name'] }").ToArray();
            Common.BindSelect(products, "#ProductId", true);
            if (PROJECCT != null && PROJECCT.ProductId > 0) {
                $("#ProductId").select2("val", PROJECCT.ProductId);
            }
        },
        LoadProducts: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetProducts",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Products = res.Data;
                        $this.BindProducts();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },

    };
}();
