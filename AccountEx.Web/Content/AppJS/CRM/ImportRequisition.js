
var ImportRequisition = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMImportRequisition";
    var focusElement = "#InvoiceNumber";
    var LIST_LOADED = false;
    var PageData = new Object();
    var RequisitionType = CRMImportRequisitionType.Default;
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
            $(document).on("change", "#CustomerId", function (event) {
                $this.LoadCustomer();
            });
            $(document).on("change", "select.CurrencyId", function (event) {
                $this.GetExchangeRate($(this));
            });
            $(document).on("change", "#SupplierId", function (event) {
                $this.LoadSupplierAddress();
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", ".action-create-project", function (event) {
                $this.CreateProject($(this));
                event.preventDefault();
                return false;
            });
            $("#Tax").keyup(function (e) {
                $this.GetNetTotal();
            });
            $(document).on("keyup", ".Quantity,.Rate", function (event) {
                var tr = $(this).closest("tr");
                var quantity = Common.GetInt($(tr).find("input.Quantity").val());
                $this.GetQuantityPriceTotal(tr);
                var product = $(tr).find("input.ItemName").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && quantity > 0)
                    $this.AddItem();
                else if (event.which == 13 && quantity <= 0) {
                    var err = "Product " + product + " must have Quantity greater than zero(0).,";
                    Common.ShowError(err);
                }
            });
            $(document).on("keyup", ".Size,.Nos", function (event) {
                var tr = $(this).closest("tr");
                var amount = Common.GetInt($(tr).find("input.Amount").val());
                $this.GetQuantityPriceTotal(tr);
                var product = $(tr).find("input.ItemName").val();
                $this.GetQuantityPriceTotal(tr);
                if (event.which == 13 && amount > 0)
                    $this.AddItem();
                else if (event.which == 13 && amount <= 0) {
                    var err = "Product " + product + " must have amount greater than zero(0).,";
                    Common.ShowError(err);
                }
            });
            $(document).on("change", "select.SaleType", function (event) {
                var type = $("option:selected", $(this)).html();
                if (type == "Project") {
                    $this.LoadProjects($(this));
                }
                else {
                    var $tr = $(this).closest("tr");
                    $tr.find("input.ProjectId").val(0);
                    $tr.find("input.Project").val('');
                }

            });
            if (Common.isNullOrWhiteSpace(Url.type)) {
                RequisitionType = CRMImportRequisitionType.Default;
            }
            else if (Url.type.toLowerCase() == "dh") {
                RequisitionType = CRMImportRequisitionType.DH;
            }
            else if (Url.type.toLowerCase() == "rsm") {
                RequisitionType = CRMImportRequisitionType.RSM;
            }



            $this.ListView();
            $this.LoadProducts();
            $this.GetCurrencyRates()
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                $this.Add();
                //if (RequisitionType == CRMImportRequisitionType.Default) {
                //    $this.Add();
                //}
                //else if (RequisitionType == CRMImportRequisitionType.DH) {
                //    $this.LoadVoucher("last");
                //}
                //else if (RequisitionType == CRMImportRequisitionType.RSM) {
                //    $this.LoadVoucher("last");
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
        GetCurrencyRates: function () {
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
        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + RequisitionType;
                LIST_LOADED = true;
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
        CustomClear: function () {
            Common.Clear();
            $("#item-container tbody").html("");
            $(".revision-history-container").addClass("hide");

        },
        AddItem: function () {
            var $this = this;
            //  if (Common.Validate($("#addrow"))) {
            var code = $("#item-container tbody tr:nth-last-child(1)  input.HSCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.HSCode").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.HSCode").focus().select();
                }, 300);
                focusElement = "";
            }
            $("#qty,#Rate,#Amount").val("");

            $this.UpdateSerialNo()
            Common.InitNumerics();
            Common.SetCheckChange();
            App.initUniform();
            $this.AutoCompleteInit();
        },
        UpdateSerialNo: function () {
            var $this = this;
            var sn = 1
            $("#item-container tbody tr").each(function () {
                var $tr = $(this);
                $tr.find("td:first-child Input.SRNo").val(sn);
                sn++;
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
            var qty = Common.GetFloat($(tr).find("input.Quantity").val());
            var rate = Common.GetFloat($(tr).find("input.Rate").val());
            //var excRate = Common.GetFloat($(tr).find("input.ExcRate").val());
            var total = qty * (rate);
            $(tr).find("input.Amount").val(total);
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var size = 0.0;
            var nos = 0.0;
            var amount = 0.0;
            var discount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find("input.Quantity").val());
                amount += Common.GetFloat($(this).find("input.Amount").val());
                nos += Common.GetFloat($(this).find("input.Nos").val());
                size += Common.GetFloat($(this).find("input.Size").val());

            });
            $("#item-container tfoot tr").find("input.Quantity").val(quantity);
            $("#item-container tfoot tr").find("input.Size").val(size);
            $("#item-container tfoot tr").find("input.Nos").val(nos);
            $("#item-container tfoot tr").find("input.Amount").val(amount.format())
            $("#GrossTotal").val(Common.GetInt(amount));
            $this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetFloat($("#GrossTotal").val());
            var percent = Common.GetFloat($("#Tax").val());
            var tax = Common.GetInt(total * (percent / 100));
            var nettotal = Common.GetInt(Math.ceil(total - tax));
            $("#NetTotal").val(nettotal);
        },
        Save: function (status) {
            var $this = this;
            $("#Status").val(status);
            $this.SaveRecord(function () {
                var voucherNo = Common.GetInt($("#VoucherNumber").val());
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                DataTable.RefreshDatatableUrl(DATATABLE_ID);
                if (RequisitionType == CRMImportRequisitionType.Default) {
                    $this.GetNextVoucherNumber();
                }
                else if (RequisitionType == CRMImportRequisitionType.DH) {
                    $this.LoadVoucher("last");
                }
                else if (RequisitionType == CRMImportRequisitionType.RSM) {
                    $this.LoadVoucher("last");
                }



            }, status);
        },
        SaveRecord: function (callback, type) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemName.trim()!=''").ToArray();
                var err = "";
                //if (typeof party == "undefined" || party == null) {
                //    err += "" + $("#AccountCode").val() + " is not valid party code.,";
                //}

                //for (var i in Items) {
                //    var item = Items[i];
                //    if (item.Quantity <= 0) {
                //        err += "Item " + item.ItemCode + "(" + item.ItemName + ") must have quantity greater than zero(0).,";
                //    }

                //    var product = Common.GetByCode(item.ItemCode);
                //    if (typeof product == "undefined" || product == null) {
                //        err += "" + item.ItemCode + " is not valid code.,";
                //    }

                //}
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
                record["CRMImportRequisitionItems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?type=" + RequisitionType,
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
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + RequisitionType + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        var d = res.Data.Requisition;
                        var customer = res.Data.Customer;
                        var histories = res.Data.History;
                        if (d != null && customer != null) {
                            d["ImportAddress"] = customer.ImportAddress;
                            d["NTN"] = customer.NTN;
                            d["GSTRN"] = customer.GSTRN;
                            d["SupplierAddress"] = res.Data.Address;
                        }
                        //var dcs = res.Data.DeliveryChallans;
                        Common.MapEditData(d, "#form-info", undefined, isView);
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#PINumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                            $("div.status-container").html('').addClass("hide");
                            $(".btn-approve,.btn-review,.btn-revised").addClass("hide");
                        }
                        else {
                            var piNumber = d.VoucherNumber;
                            if (d.RevisionNo > 0) {
                                piNumber = piNumber + "-" + String.fromCharCode(64 + d.RevisionNo);
                            }
                            if (histories.length > 0) {
                                var html = "";
                                for (var i in histories) {
                                    var rh = histories[i];
                                    html += "<tr>";
                                    html += "<td>" + rh.RevisionNo + "</td>";
                                    html += "<td>" + (rh.RevisionNo > 0 ? rh.VoucherNumber + "-" + String.fromCharCode(64 + rh.RevisionNo) : rh.VoucherNumber + "-0") + "</td>";
                                    html += "<td>" + Common.FormatDate(rh.ReviseDate, "DD-MM-YYYY") + "</td>";
                                    html += "</tr>";

                                }
                                $("#tbl-history tbody").html(html);
                                $(".revision-history-container").removeClass("hide");
                            }


                            $("#PINumber").val(piNumber);
                            $(".btn-approve,.btn-review,.btn-revised").removeClass("hide");
                            $this.SetStatus(d.Status);
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
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }
                            var items = d.CRMImportRequisitionItems;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                Common.MapItemData(items);
                                Common.InitNumerics();
                                Common.SetCheckChange();
                                App.initUniform();

                                $("#item-container tbody tr").each(function () {

                                    var id = Common.GetInt($(this).find("input.Id").val());
                                    if (id > 0)
                                        $(this).find("a.action-create-project").removeClass("hide");

                                });

                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductName").focus().select();
                                }, 500);
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

                        $this.AddItem();
                        $this.GetWholeTotal();
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
        LoadCustomer: function () {
            var $this = this;
            var customerId = Common.GetInt($("#CustomerId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetCustomer&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        if (data != null) {
                            $("#ImportAddress").val(data.ImportAddress);
                            $("#NTN").val(data.NTN);
                            $("#GSTRN").val(data.GSTRN);

                        }
                        else {
                            $("#ImportAddress,#NTN,#GSTRN").val('');
                        }

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadSupplierAddress: function () {
            var $this = this;
            var supplierId = Common.GetInt($("#SupplierId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetSupplierAddress&supplierId=" + supplierId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#SupplierAddress").val(res.Data);




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
        },
        SetStatus: function (status) {

            var $this = this;
            var statusLabel = "";
            if (status == CRMImportRequisitionStatus.Pending) {
                statusLabel = "<span class='label label-danger'>Pending</span>";
            }
            else if (status == CRMImportRequisitionStatus.Review) {
                statusLabel = "<span class='label label-warning'>in Review</span>";
            }
            else if (status == CRMImportRequisitionStatus.Approved) {
                statusLabel = "<span class='label label-success'>Approved</span>";
            }
            else if (status == CRMImportRequisitionStatus.Revised) {
                statusLabel = "<span class='label label-warning'>Revised</span>";
            }
            $("div.status-container").html(statusLabel).removeClass("hide");
        },
        CreateProject: function (elment) {

            var $this = this;
            var id = Common.GetInt($(elment).closest("tr").find("input.Id").val());
            if (id > 0)
                window.open('projects?pmcId=' + id);
        },
        AutoCompleteInit: function (products) {
            var $this = this;
            var products = PageData.Products;
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.HSCode,
                        label: product.HSCode + " (" + product.Name + ")",
                        Product: product

                    }
                );
            }
            $(".HSCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ItemId").val(ui.item.id);
                    $(tr).find("input.ItemName").val(ui.item.Product.Name);
                    $(tr).find("input.DivisionId").val(ui.item.Product.DivisionId);
                    $(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

        },
        Edit: function ($element, isView) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
            $("#VoucherNumber").val(id);
            $this.LoadVoucher("same", isView);
        },
        Delete: function ($element) {
            var $this = this;
            var id = Common.GetInt($("#Id").val());
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
                            Common.ShowMessage(true, { message: Messages.RecordDeleted });
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

    };
}();
