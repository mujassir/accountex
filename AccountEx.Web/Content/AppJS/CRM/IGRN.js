
var IGRN = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMIGRN";
    var focusElement = "#InvoiceNumber";
    var LIST_LOADED = false;
    var PageData = new Object();
    var ImportRecords = new Array();
    var HEADERS = new Array();
    var ALLOWED_HEADERS = new Array();
    var $tbl = null;
    return {
        init: function () {
            var $this = this;
            ALLOWED_HEADERS = new Array("DATE", "InvoiceNumber", "Product", "Price", "Quantity", "Total", "Tax", "Customer", "InvoiceType");
            $tbl = $("#tbl-data-import");
            $(document).on("click", "#btn-save", function (event) {
                $this.Save();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", "td.td-status", function (event) {
                var error = $(this).attr("data-error");
                if (!Common.isNullOrWhiteSpace(error)) {
                    Common.ShowError(error);
                }
                event.stopPropagation();

            });
            $(document).on("click", ".btn-view", function (event) {
                $this.Edit($(this), true);
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("change", "select.CurrencyId", function (event) {
                $this.GetExchangeRate($(this));
            });
            $(document).on("change", "#SaleType", function (event) {
                if ($(this).val() == CRMSaleType.NonGST) {
                    $("#InvoiceNumber").prop("disabled", true);
                    if ($("#Id").val() > 0)
                        return;
                    $this.GetNextInvoiceNo();
                }
                else {
                    $("#InvoiceNumber").prop("disabled", false);
                }
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem($(this));
            });
            $(document).on("change", "select#DeliveryType", function (event, salePersonId) {
                $this.SetCurrency();
            });
            $(document).on("click", ".action-create-project", function (event) {
                $this.CreateProject($(this));
                event.preventDefault();
                return false;
            });
            $("#Tax").keyup(function (e) {


                $this.GetNetTotal();
            });
            $("input#csv-file").change(function () {
                // $this.ParseCSVFile();
            });
            $(document).on("click", "#btnparse", function () {

                $this.ParseCSVFile();
            });
            $(document).on("keyup", ".Quantity,.Price,.ExcRate", function (event) {
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
            $(document).on("keyup", ".ItemName", function (event) {
                var tr = $(this).closest("tr");
                $this.LoadSalePerson($(this));
                $this.LoadProjects(tr);
            });
            $(document).on("change", "select.SaleType", function (event) {
                var type = $("option:selected", $(this)).html();
                if (type == "Project" || type == "Opportunity Sales") {
                    $this.LoadProjects($(this));
                }
                else {
                    var $tr = $(this).closest("tr");
                    $tr.find("input.ProjectId").val(0);
                    $tr.find("input.Project").val('');
                }

            });
            $("#btnSearch").click(function () {
                $this.Filter();
            });
            $("#FilterFiscalId").change(function () {
                $this.Filter();
            });

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
            $('#modal-import-invoices').on('hidden.bs.modal', function () {
                DataTable.RefreshDatatable(DATATABLE_ID);
                $("tbody", $tbl).html('');
            })
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
            // $this.GetNextVoucherNumber();
        },
        OpenImportModal: function () {
            var $this = this;
            $("tbody", $tbl).html('');
            $(".data-container").addClass("hide");
            $("#modal-import-invoices").modal("show");
            $("#btn-import-error span").text('Error').removeClass("hide");
        },
        ShowImportError: function () {
            var $this = this;
            $("#tbl-error-import thead").html('');
            $("#tbl-error-import tbody").html('');
            var theadClone = $('>thead tr', $tbl).clone();
            var tbodyClone = $('tr[data-parent="true"].error', $tbl).clone();
            $('#tbl-error-import thead').html(theadClone);
            $('#tbl-error-import tbody').html(tbodyClone);
            $("#modal-import-invoices-error").modal("show");
        },
        ParseCSVFile: function () {

            var $this = this;
            try {

                var file = $("#csv-file")[0].files[0]
                if (file == undefined) {
                    Common.ShowError("Please select csv file.");
                    return false;
                }
                else {
                    $("#btnparse").html("<span>processing...</span>&nbsp;<i class='fa fa-spinner fa-spin'></i>");
                    Papa.parse(file, {
                        header: true,
                        dynamicTyping: true,
                        complete: function (results) {
                            var data = results.data;
                            if (data != null && data.length > 0) {
                                ImportRecords = $.grep(data, function (p, i) { // just use arr
                                    return !Common.isNullOrWhiteSpace(["InvoiceNumber"]) && !Common.isNullOrWhiteSpace(p["Product"]) && !Common.isNullOrWhiteSpace(p["Customer"]);
                                });
                                $this.DrawTable();
                            }
                        }
                    });

                }
            }
            catch (e) {
            }
        },
        DrawTable: function () {
            var $this = this;
            $($tbl).find("thead,tbody").html('');
            $("#btn-start-import").removeClass("hide");

            HEADERS = new Array();
            $this.DrawTableHeader();
            $this.DrawTableBody();
            $("#btnparse").html("<span>Upload</span>&nbsp;<i class='fa fa-upload'></i>");
        },
        DrawTableHeader: function () {
            var $this = this;
            var headerHtml = "";
            var headerRow = ImportRecords[0];
            headerHtml = "<tr>";
            headerHtml += "<th>PI No</th><th>PI Date</th><th>Customer</th><th>Invoice No</th><th>Invoice Date</th>";
            headerHtml += "<th>BL No. / RR No.</th><th>BL / RR Date</th><th>Product</th><th class='align-right'>Quantity</th>";
            headerHtml += "<th>Currency</th><th class='align-right'>ExRate</th><th class='align-right'>LCRate</th><th class='align-right'>LCValue</th><th>Status</th>"
            headerHtml += "</tr>";
            $("thead", $tbl).html(headerHtml);
        },
        DrawTableBody: function (list) {
            var $this = this;
            var html = [];


            //        //// Overload:function(keySelector,elementSelector,resultSelector,compareSelector)
            //        var groupRecords = Enumerable.From(ImportRecords).GroupBy("$.InvoiceNumber", null,
            //           function (key, g) {
            //               var result = {
            //                   InvoiceNumber: key,
            //                   Invoice: g.FirstOrDefault(),
            //                   Records: g.ToArray()
            //               }
            //               return result;
            //           }).ToArray();

            //        groupRecords = Enumerable.From(ImportRecords)
            //.GroupBy(
            //    "{ InvoiceNumber: $.InvoiceNumber, Customer: $.Customer }",
            //    null,
            //    "{ InvoiceNumber: $.InvoiceNumber, Invoice: $$.FirstOrDefault(), Records: $$.ToArray() }",
            //    "String($.InvoiceNumber) + $.Customer"
            //)
            //.ToArray();

            $("#total-import-invoice-count").html("Total Records:" + ImportRecords.length);
            var rowcounter = 1;

            for (var j in ImportRecords) {
                var record = ImportRecords[j];
                //var invoice = groupRecord.Invoice;
                //var records = groupRecord.Records;
                //if (Common.GetInt(invoice.InvoiceNumber) == 0)
                //    continue;
                var childId = "";
                html.push("<tr class='group-tr accordion-toggle' data-record='" + JSON.stringify(record) + "' data-processing='pending' data-parent='true' data-toggle='collapse' data-target='#data-child_" + childId + "' data-invoicenumber='" + record.InvoiceNumber + "' data-customer='" + record.Customer + "' data-toggle='tooltip'>");
                html.push("<td>" + record.PINo + "</td>");
                html.push("<td>" + record.PIDate + "</td>");
                html.push("<td>" + record.Customer + "</td>");
                html.push("<td>" + record.InvoiceNumber + "</td>");
                html.push("<td>" + record.InvoiceDate + "</td>");
                html.push("<td>" + record.BLBRNo + "</td>");
                html.push("<td>" + record.BLRRDate + "</td>");
                html.push("<td>" + record.Product + "</td>");
                html.push("<td class='align-right'>" + Common.GetInt(record.Quantity).format() + "</td>");
                html.push("<td>" + (record.Currency != "£" ? record.Currency : "&pound;") + "</td>");
                html.push("<td class='align-right'>" + record.ExRate + "</td>");
                html.push("<td class='align-right'>" + record.LCRate + "</td>");
                html.push("<td class='align-right'>" + Common.GetInt(record.LCValue).format() + "</td>");
                html.push("<td class='td-status'></td>");
                html.push("</tr>");
                rowcounter++;
            }
            //$("tbody", $tbl).html(html);
            document.getElementById('importtbody').innerHTML = html.join('');
            $(".data-container").removeClass("hide");
        },
        StartImport: function (isValidate) {

            var $this = this;
            $this.DrawTable();
            $this.ProcessImport(isValidate);
            setTimeout(function () { $this.ProcessImport(isValidate) }, 500);
        },
        ProcessImport: function (isValidate) {
            try {


                var $this = this;
                //:first-child
                var totalTake = 40;
                var currentTake = totalTake;
                var totalInProcess = $tbl.find("tbody tr[data-parent='true'][data-processing='inprocess']").length;
                if (totalInProcess < totalTake) {
                    currentTake = 40 - totalInProcess;
                    if ($tbl.find("tbody tr[data-parent='true'][data-processing='pending']:lt(" + currentTake + ")").length) {
                        $tbl.find("tbody tr[data-parent='true'][data-processing='pending']:lt(" + currentTake + ")").each(function () {
                            var $tr = $(this);
                            $tr.attr("data-processing", "inprocess");
                            $this.PostToserverForValidation($tr, isValidate);
                        });
                        setTimeout(function () { $this.ProcessImport(isValidate) }, 500)
                    }
                }
                else {
                    setTimeout(function () { $this.ProcessImport(isValidate) }, 500)
                }
            } catch (e) {
                setTimeout(function () { $this.ProcessImport(isValidate) }, 500)
            }
            if ($("tr[data-parent='true'].error", $tbl).length) {
                $("#btn-import-error span").text("Error (" + $("tr[data-parent='true'].error", $tbl).length + ")");
                $("#btn-import-error").removeClass("hide");
            }
            else {
                $("#btn-import-error").addClass("hide");
            }
            $tbl.find("tbody tr[data-parent='true'][data-processing='inprocess']:first", $tbl).scrollView();

        },
        PostToserverForValidation: function ($tr, isValidate) {

            try {
                var $this = this;
                var invNo = $tr.attr("data-invoicenumber");
                var customer = $tr.attr("data-customer");
                var processingIcon = "<div class='icon-content'><i class='fa fa-spinner fa-spin' aria-hidden='true'></i></div>";
                $tr.find("td.td-status").html(processingIcon);
                $tr.attr("title", "Processing.Please wait").removeClass().addClass("processing");
                var data = null;
                try {

                    data = { IGRN: [$.parseJSON($tr.attr("data-record"))] };

                } catch (e) {
                    $this.SetRowStatus($tr, false, "unable to parse invoice data");
                }
                if (!$this.ValidateImport($tr))
                    return;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?import=true&isValidate=" + isValidate + "",
                    type: "POST",
                    data: data,
                    blockUI: false,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.SetRowStatus($tr, res.Success, "Successful");
                        } else {
                            $this.SetRowStatus($tr, res.Success, res.Error)
                        }
                    },
                    error: function (e) {
                        $this.SetRowStatus($tr, false, "Unspecific Errror!(" + e + ")")
                    }
                });
            } catch (e) {
                $this.SetRowStatus($tr, false, "Unspecific Errror!(Client side)(" + e + ")");
            }

        },
        SetRowStatus: function ($tr, success, message) {
            var $this = this;
            var tickedresponse = "<div class='icon-content'><i class='fa fa-check-circle colorgreen' aria-hidden='true'></i></div>";
            var crossresponse = "<div class='icon-content'><i class='fa fa-times-circle' aria-hidden='true'></i></div>";
            if (success) {
                $tr.find("td.td-status").html(tickedresponse).attr("title", message).removeAttr("data-error");
                $tr.removeClass().addClass("success");

            }
            else {
                $tr.find("td.td-status").html(crossresponse).attr("title", "click to show error detail").attr("data-error", message);
                $tr.removeClass().addClass("error text-danger");
            }
            $tr.attr("data-processing", "done");

        },
        IsHeaderAllowed: function (header) {
            var $this = this;
            return ALLOWED_HEADERS.indexOf(header) >= 0
        },
        ValidateImport: function ($tr) {
            var $this = this;
            data = $.parseJSON($tr.attr("data-record"));
            var invoiceNo = Common.GetInt(data["InvoiceNumber"]);
            var isSuccess = true;
            var error = "";
            //if (invoiceNo <= 0) {
            //    error = "Invoice no is required.";
            //    isSuccess = false;
            //}
            //if (Surname == null || Surname.trim() == "") {
            //    error = "Surname is required.";
            //    isSuccess = false;
            //}
            //if (roleId == null || roleId <= 0) {
            //    error = "Role Id is required.";
            //    isSuccess = false;
            //}
            if (!isSuccess)
                $this.SetRowStatus($tr, isSuccess, error)
            return isSuccess;
        },

        DetailView: function () {
            Common.GoToTop();
        },
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.LoadVoucher("nextvouchernumber");
        },

        ListView: function () {

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
            var code = $("#item-container tbody tr:nth-last-child(1)  input.ItemName").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.ItemName").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.ItemName").focus().select();
                }, 300);
                focusElement = "";
            }
            $("#qty,#Rate,#Amount").val("");

            $this.UpdateSerialNo()
            Common.InitNumerics();
            Common.SetCheckChange();
            App.initUniform();
            $this.AutoCompleteInit();
            $this.SetCurrency();
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
        SetCurrency: function (deliveryType) {
            var deliveryType = $("#DeliveryType").val()
            if (deliveryType == CRMSaleDeliveryType.ExStock) {
                var pkrValue = $("#item-container tbody tr:first-child select.CurrencyId option").filter(function () { return $(this).html().toLowerCase() == "rs"; }).val();
                $("#item-container tbody tr").find("select.CurrencyId").val(pkrValue);
            }
        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;

            var qty = Common.GetFloat($(tr).find("input.Quantity").val());
            var rate = Common.GetFloat($(tr).find("input.Price").val());
            var exRate = Common.GetFloat($(tr).find("input.ExcRate").val());
            var total = qty * rate * exRate;
            $(tr).find("input.NetTotal").val(total);
            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var quantity = 0.0;
            var amount = 0.0;
            var discount = 0.0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                quantity += Common.GetFloat($(this).find("input.Quantity").val());
                amount += Common.GetFloat($(this).find("input.NetTotal").val());

            });

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
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                DataTable.RefreshDatatable(DATATABLE_ID);
                //$this.CustomClear();
                //focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });

            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ItemName.trim()!=''").ToArray();
                var err = "";
                for (var i in Items) {
                    var item = Items[i];
                    if (Common.isNullOrWhiteSpace(item.ItemName)) {
                        err += "Item name is required for each row.,";
                    }
                    if (item.Quantity <= 0) {
                        err += "Item " + item.ItemName + " must have quantity greater than zero(0).,";
                    }
                    if (item.NetTotal <= 0) {
                        err += "Item " + item.ItemName + " must have total greater than zero(0).,";
                    }
                }

                //}
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                var record =
                    {
                        Invoices: Items
                    }
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
            var piNumber = $("#PINumber").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?isEdit=true&piNumber=" + piNumber,
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
                        var invoices = res.Data.Invoices;
                        var d = null;
                        if (invoices.length > 0) {
                            var d = invoices[0];
                            Common.MapEditData(d, "#form-info", undefined, isView);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                        }
                        var products = res.Data.Products;
                        var salePersons = res.Data.SalePersons;
                        var items = invoices;
                        for (var key in items) {
                            var item = items[key];
                            var product = Enumerable.From(products).FirstOrDefault(null, "$.Id===" + item.ProductId + "");
                            var salePerson = Enumerable.From(salePersons).FirstOrDefault(null, "$.Id===" + item.SalePersonId + "");
                            if (product != null) {
                                item["Vendor"] = product.Vendor;
                                item["Division"] = product.Division;
                                item["ItemName"] = product.Name;
                                item["CategoryId"] = product.CategoryId;
                            }
                            if (salePerson != null) {
                                item["SalePerson"] = salePerson.Name;
                            }

                        }

                        Common.MapItemData(items);
                        Common.InitNumerics();
                        Common.SetCheckChange();
                        App.initUniform();
                        $this.AutoCompleteInit();

                        $("#item-container tbody tr").each(function () {
                            var id = Common.GetInt($(this).find("input.Id").val());
                            $(this).find("input.ItemName").trigger("keyup");


                        });

                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ProductName").focus().select();
                        }, 500);
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
        GetNextInvoiceNo: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetNextInvoiceNo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#InvoiceNumber").val(res.Data);
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        LoadProjects: function ($element) {
            var $this = this;
            var $tr = $element.closest("tr");
            var customerId = Common.GetInt($("#CustomerId").val());
            var productId = Common.GetInt($tr.find("input.ProductId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetProjects&customerId=" + customerId + "&productId=" + productId,
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
                            $tr.find("input.ProjectId").val(data.Id);
                            $tr.find("input.Project").val(data.Name);
                            var value = $('select.SaleType option', $tr).filter(function () { return $(this).html().toLowerCase() == "project" || $(this).html().toLowerCase() == "project sales" || $(this).html().toLowerCase() == "opportunity sales" }).val();
                            $('select.SaleType', $tr).val(value);
                        }
                        else {
                            $tr.find("input.ProjectId").val('');
                            $tr.find("input.Project").val('');
                            var value = $('select.SaleType option', $tr).filter(function () { return $(this).html().toLowerCase() == "regular sales" }).val();
                            $('select.SaleType', $tr).val(value);
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
        LoadSalePerson: function ($element) {
            var $this = this;
            var $tr = $element.closest("tr");
            var customerId = Common.GetInt($("#CustomerId").val());
            var productId = Common.GetInt($tr.find("input.ProductId").val());
            var categoryId = Common.GetInt($tr.find("input.CategoryId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetSalePerson&customerId=" + customerId + "&categoryId=" + categoryId,
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
                            $tr.find("input.SalePersonId").val(data.Id);
                            $tr.find("input.SalePerson").val(data.Name);
                        }
                        else {
                            $tr.find("input.SalePersonId").val(0);
                            $tr.find("input.SalePerson").val('');
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
        DeleteRow: function (elment) {

            var $this = this;
            var $tr = $(elment).closest("tr");
            if (Common.GetInt($tr.find("Input.Id").val()) > 0) {
                $this.Delete(null, $tr);
            }
            else {
                $tr.remove();
                if ($("#item-container tbody").children().length <= 0)
                    $this.AddItem();
            }

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
                        value: product.Name,
                        label: product.Name,
                        name: product.Name,
                        Product: product

                    }
                );
            }
            $(".ItemName").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ProductId").val(ui.item.id);
                    $(tr).find("input.CategoryId").val(ui.item.Product.CategoryId);
                    $(tr).find("input.DivisionId").val(ui.item.Product.DivisionId);
                    $(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                    $this.LoadSalePerson(tr);
                    $this.LoadProjects(tr);
                }
            });

        },
        Edit: function ($element, isView) {
            var $this = this;
            var id = $element.attr("data-id");
            $("#PINumber").val(id);
            $this.LoadVoucher("same", isView);
        },
        Delete: function ($element, $tr) {
            var $this = this;
            var id = 0;
            if ($element != undefined && $element != null)
                id = Common.GetInt($element.attr("data-id"));
            if ($tr != undefined && $tr != null)
                id = Common.GetInt($tr.find("Input.Id").val());
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
                            Common.ShowMessage(true, { message: Messages.RecordDeleted });
                            $this.GetNextVoucherNumber();
                            if ($tr != undefined && $tr != null) {
                                $tr.remove();
                                if ($("#item-container tbody").children().length <= 0)
                                    $this.AddItem();
                            }
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
