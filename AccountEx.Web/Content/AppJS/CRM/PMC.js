
var PMC = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "PMC";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var PageData = new Object();
    var PageSetting = new Array();
    var SELECTED_CUSTOMERID = false;
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
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem($(this));
            });
            $(document).on("change", "select.CurrencyId", function (event) {
                $this.GetExchangeRate($(this));
            });
            $(document).on("change", "select#CustomerId", function (event) {
                SELECTED_CUSTOMERID = Common.GetInt($(this).val());
                $("#VoucherNumber").val(0)
                $this.LoadVoucher("getbycutomerId");
            });
            $("#btnSearch").click(function () {
                $this.Filter();
            });
            $("#FilterFiscalId").change(function () {
                $this.Filter();
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", ".action-create-project", function (event) {
                $this.CreateProject($(this));
                event.preventDefault();
                return false;
            });
            $(document).on("click", ".action-grid-create-project", function (event) {
                $this.CreateProjectFromGrid($(this));
                event.preventDefault();
                return false;
            });
            $(document).on("keyup", ".ExcRate,.Price,.AnnualQty", function (event) {

                var tr = $(this).closest("tr");
                var annualPotential = Common.GetInt($(tr).find("input.AnnualPotential").val());
                var product = $(tr).find("input.ProductName").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && annualPotential > 0)
                    $this.AddItem();
                else if (event.which == 13 && annualPotential <= 0) {
                    var err = "Product " + product + " must have Annual Potential greater than zero(0).,";
                    Common.ShowError(err);
                }
            });
            $(document).on("change", "select.IsCounter", function (event) {
                var tr = $(this).closest("tr");
                $this.GetQuantityPriceTotal(tr);

            });
            $this.LoadPageSetting();
            $this.ListView();
            $this.LoadProducts();
            $this.GetCurrencies();
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                $this.Add();
                //} else {
                //    this.ListView();
                //}
            }
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

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                var fiscalId = $("#FilterFiscalId").val();
                url += "?fiscalId=" + fiscalId
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        Filter: function () {
            var $this = this;
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            var fiscalId = $("#FilterFiscalId").val();
            url += "?fiscalId=" + fiscalId
            url += Common.MakeQueryStringAll($("#filters-container"));
            DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
        },
        CustomClear: function () {
            Common.Clear();
        },
        AddItem: function () {
            var $this = this;
            //  if (Common.Validate($("#addrow"))) {
            var code = $("#item-container tbody tr:nth-last-child(1)  input.ProductName").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.ProductName").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.ProductName").focus().select();
                }, 300);
                focusElement = "";
            }
            $("#qty,#Rate,#Amount").val("");

            $this.UpdateSerialNo();
            Common.InitNumerics();
            Common.SetCheckChange();
            App.initUniform();
            $this.AutoCompleteInit();
            $this.AutoCompleteSalePersonInit();
            if (PageData.SalePersons && PageData.SalePersons.length == 1) {
                var salePerson = PageData.SalePersons[0];
                $("#item-container tbody tr:nth-last-child(1)").find("input.SalePerson").val(salePerson.Name);
                $("#item-container tbody tr:nth-last-child(1)").find("input.SalePersonId").val(salePerson.Id);
            }
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
        UpdateSerialNo: function () {
            var $this = this;
            var sn = 1
            $("#item-container tbody tr").each(function () {
                var $tr = $(this);
                $tr.find("td:first-child Input.SN").val(sn);
                sn++;
            });
        },
        GetSalePersons: function () {
            var $this = this;
            var customerId = $("#CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc/?key=GetSalesManByCustomerId&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        PageData.SalePersons = res.Data;
                        $this.AutoCompleteSalePersonInit();
                        if (PageData.SalePersons.length == 1) {
                            var salePerson = PageData.SalePersons[0];
                            $("#item-container tbody input.SalePerson").val(salePerson.Name);
                            $("#item-container tbody input.SalePersonId").val(salePerson.Id);
                        }
                        else {
                            $("#item-container tbody input.SalePerson").val('');
                            $("#item-container tbody input.SalePersonId").val(0);
                        }
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetExchangeRate: function ($elemnt) {
            var $this = this;
            var $tr = $elemnt.closest("tr");
            var currencyId = Common.GetInt($tr.find("select.CurrencyId").val());
            var CurrencyRate = Enumerable.From(PageData.Currencies).FirstOrDefault(null, "$.CurrencyId ==" + currencyId + "");
            var rate = 0;
            if (CurrencyRate != null)
                rate = CurrencyRate.Rate;

            $tr.find("input.ExcRate").val(rate);
            $this.GetQuantityPriceTotal($tr);

        },

        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var annualQty = Common.GetFloat($(tr).find("input.AnnualQty").val());
            var excRate = Common.GetFloat($(tr).find("input.ExcRate").val());
            var price = Common.GetFloat($(tr).find("input.Price").val());
            var total = annualQty * price * excRate;
            $(tr).find("input.AnnualPotential").val(total);
            var rplPotential = 0;
            var isCounter = Common.GetBool($("select.IsCounter", tr).val());
            if (isCounter)
                rplPotential = Common.GetInt($(tr).find("input.AnnualPotential").val());
            $(tr).find("input.AnnualRPLPotential").val(rplPotential);
            $this.CalculateSummary();
        },
        CalculateSummary: function (tr) {
            $this = this;
            var records = new Array();
            var html = "";
            var Items = Common.SaveItemData();
            var divisions = Enumerable.From(Items).Select("$.Division").Distinct().ToArray();
            var ownProductIds = Enumerable.From(PageData.Products).Where(function (x) { return x.IsOwnProduct }).Select("$.Id").Distinct().ToArray();
            for (var i in divisions) {
                var division = divisions[i];
                var potential = Enumerable.From(Items).Where(function (x) { return x.Division == division && x.IsCounter == 'true' }).Sum("Common.GetInt($.AnnualPotential)");
                var ownpotential = Enumerable.From(Items).Where(function (x) { return x.Division == division && $.inArray(Common.GetInt(x.ProductId), ownProductIds) !== -1 }).Sum("Common.GetInt($.AnnualPotential)");
                html += "<tr>"
                html += "<td>" + division + "</td>";
                html += "<td class='align-right'>" + potential.format() + "</td>";
                html += "<td class='align-right'>" + ownpotential.format() + "</td>";
                html += "<tr>"

            }

            var potential = Enumerable.From(Items).Where(function (x) { return x.IsCounter == 'true' }).Sum("Common.GetInt($.AnnualPotential)");
            var ownpotential = Enumerable.From(Items).Where(function (x) { return $.inArray(Common.GetInt(x.ProductId), ownProductIds) !== -1 }).Sum("Common.GetInt($.AnnualPotential)");

            html += "<tr class='bold subtotal'>"
            html += "<td class='align-right'>Total</td>";
            html += "<td class='align-right'>" + potential.format() + "</td>";
            html += "<td class='align-right'>" + ownpotential.format() + "</td>";
            html += "<tr>"
            $("#table-summary tbody").html(html);

        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                var voucherNo = Common.GetInt($("#VoucherNumber").val());
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                DataTable.RefreshDatatable(DATATABLE_ID);
                $this.GetNextVoucherNumber();
                //if (!$("#ChkGetNextVoucher").is(":checked")) {
                //    $("#VoucherNumber").val(voucherNo);
                //    $this.LoadVoucher("same");
                //}
                //else {
                //    $this.GetNextVoucherNumber();
                //}
            });
        },
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber");
        },
        SaveRecord: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (record.Id > 0) {
                var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                var newvoucherno = Common.GetInt(record.VoucherNumber);
                if (prevoucherno != newvoucherno) {
                    err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                    Common.ShowError(err);
                    return;
                }
            }

            if (Common.Validate($("#mainform"))) {
                $("#item-container tbody input.ExcRate").trigger("keyup");
                Items = Common.SaveItemData();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (Common.isNullOrWhiteSpace(item.ProductName)) {
                        err += "Item name is required for each row.,";
                    }
                    if (Common.isNullOrWhiteSpace(item.CurrencyId)) {
                        err += "Item " + item.ProductName + " must have currency selected.,";
                    }
                    if (item.AnnualQty <= 0) {
                        err += "Item " + item.ProductName + " must have quantity greater than zero(0).,";
                    }
                    if (item.Price <= 0) {
                        err += "Item " + item.ProductName + " must have Price greater than zero(0).,";
                    }
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                //if (Common.GetInt(record.NetTotal) <= 0) {
                //    err += "Transaction total amount should be graeter than zero(0).,";
                //}
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["PMCItems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },

        LoadVoucher: function (key, isView) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var customerId = $("#CustomerId").val();
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=0&key=" + key + "&voucher=" + voucherno + "&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.GetSalePersons();

                        $("#item-container tbody").html("");
                        $("#table-summary tbody").html('');
                        $this.CustomClear();
                        var d = res.Data.PMC;
                        var products = res.Data.Products;
                        var salePersons = res.Data.SalePersons;
                        var projectPmcItemIds = res.Data.ProjectPmcItemIds;
                        //var dcs = res.Data.DeliveryChallans;
                        Common.MapEditData(d, "#form-info", undefined, isView);
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            //var voucher = this.GetType();
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            //if (voucher != "sale") 
                            $("#DCNo").prop("disabled", "disabled");
                            //else
                            //    $("#DCNo").removeProp("disabled");
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (d.CashSale && d.Id > 0) {

                            }
                            else {
                                $.uniform.update();
                                if (d != null && d.Id == 0) {
                                    $this.AddItem();
                                }

                            }
                            if (d.Id > 0 && d.PMCItems != null && d.PMCItems.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                var items = d.PMCItems;


                                for (var key in items) {
                                    var item = items[key];
                                    var product = Enumerable.From(products).FirstOrDefault(null, "$.Id===" + item.ProductId + "");
                                    var salePerson = Enumerable.From(salePersons).FirstOrDefault(null, "$.Id===" + item.SalePersonId + "");
                                    if (product != null) {
                                        item["Vendor"] = product.Vendor;
                                        item["Division"] = product.Division;
                                        item["ProductName"] = product.Name;
                                    }
                                    if (salePerson != null) {
                                        item["SalePerson"] = salePerson.Name;
                                    }

                                }
                                //if (isView)

                                items = Enumerable.From(items).OrderByDescending("$.Division").ThenBy("$.ProductName").ToArray();

                                Common.MapItemData(items);
                                $this.CalculateSummary();
                                Common.InitNumerics();
                                Common.SetCheckChange();
                                App.initUniform();
                                Common.SetPageAccess(d, isView);
                                $this.AutoCompleteInit();
                                Common.goToByScroll();
                                $("#item-container tbody tr").each(function () {

                                    var id = Common.GetInt($(this).find("input.Id").val());
                                    var isCounter = Common.GetBool($(this).find("select.IsCounter").val());
                                    if (id > 0 && isCounter) {
                                        var $projectButton = $(this).find("a.action-create-project")
                                        $projectButton.removeClass("hide");
                                        if (Enumerable.From(projectPmcItemIds).Any("$===" + id + "")) {
                                            $projectButton.text("View");
                                        }
                                    }



                                });

                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductName").focus().select();
                                }, 500);
                            }
                        }
                        if (SELECTED_CUSTOMERID > 0) {
                            $("#CustomerId").select2("val", SELECTED_CUSTOMERID)
                        }
                        $this.GetSalePersons();
                        $this.DetailView();
                        $this.UpdateSerialNo();
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
                        $this.AutoCompleteInit();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
            $this.UpdateSerialNo();
        },
        CreateProject: function (elment) {

            var $this = this;
            var id = Common.GetInt($(elment).closest("tr").find("input.Id").val());
            if (id > 0)
                window.open('projects?pmcId=' + id);
        },
        CreateProjectFromGrid: function ($elment) {

            var $this = this;
            var id = Common.GetInt($elment.attr("data-id"));
            if (id > 0)
                window.open('projects?pmcId=' + id);
        },
        AutoCompleteInit: function (products) {
            var $this = this;
            var products = PageData.Products;
            var customerId = $("#CustomerId").val();
            if (PageSetting.IsSalePerson) {
                var customerCatgeories = PageSetting.CustomerCategories;
                var Ids = Enumerable.From(customerCatgeories).Where(function (x) { return x.CRMCustomerId == customerId }).Select("$.CategroyId").Distinct().ToArray();
                products = Enumerable.From(PageData.Products).Where(function (x) { return $.inArray(Common.GetInt(x.CategoryId), Ids) !== -1 }).ToArray();
            }
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.Name,
                        label: product.Name + " (" + product.Vendor + ")",
                        name: product.Name,
                        Product: product

                    }
                );
            }
            $(".ProductName").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ProductId").val(ui.item.id);
                    $(tr).find("input.Division").val(ui.item.Product.Division);
                    $(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

        },
        AutoCompleteSalePersonInit: function (products) {
            var $this = this;
            var salePersons = PageData.SalePersons;
            var suggestion = new Array();
            for (var i in salePersons) {
                var salePerson = salePersons[i];
                suggestion.push(
                    {
                        id: salePerson.Id,
                        value: salePerson.Name,
                        label: salePerson.Name,
                        name: salePerson.Name,
                        SalePerson: salePerson

                    }
                );
            }
            $(".SalePerson").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.SalePersonId").val(ui.item.id);
                }
            });

        },
        LoadPageSetting: function () {
            var $this = this;
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        Edit: function ($element, isView) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            $("#VoucherNumber").val(id);
            $this.LoadVoucher("same", isView);
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

    };
}();
