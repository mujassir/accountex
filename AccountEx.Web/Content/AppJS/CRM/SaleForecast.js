
var SaleForecast = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMSaleForecast";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var PageSetting = new Object();
    var PageData = new Object();
    var ForecastType = CRMSaleForecastType.SalePerson;
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
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem($(this));
            });
            $(document).on("click", "#btn-add-customer", function (event) {
                $this.AddCustomer($(this));
            });
            $('#Date').datepicker().on('changeDate', function (e) {
                $this.LoadEvents();
            });
            $(document).on("change", "select.CustomerId", function (event) {
                $this.LoadProjects($(this));
            });
            $(document).on("change", "select#Month", function (event) {
                $this.LoadSales();
            });
            $(document).on("change", "select#DivisionId", function (event) {
                $this.FilterByDivision();
            });

            if (Common.isNullOrWhiteSpace(Url.type)) {
                ForecastType = CRMSaleForecastType.SalePerson;
            }
            else if (Url.type.toLowerCase() == "rsm") {
                ForecastType = CRMSaleForecastType.RSM;
            }
            else if (Url.type.toLowerCase() == "summary") {
                ForecastType = CRMSaleForecastType.Summary;
            }

            $this.LoadPageSetting();
            $this.CustomClear();
            $this.AutoCompleteSalePerson();
            $this.AddItem();
            $this.SetFormControl();
            $this.LoadCustomers();


        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            Common.GoToTop();
        },
        SetFormControl: function () {
            var $this = this;
            if (PageSetting.IsSalePerson) {
                var salePerson = PageSetting.SalePersons[0];
                $("#SalePerson").val(salePerson.Name).prop("disabled", true).prop("readonly", true);
                $("#SalePersonId").val(salePerson.Id);
            }

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
        CustomClear: function () {
            Common.Clear();
        },
        AddItem: function () {
            var $this = this;
            var html = $this.GetHeaderHtml();
            $("#item-container tbody").append(html);
            Common.InitNumerics();
            Common.SetCheckChange();
            $("#item-container tbody tr:nth-last-child(1) select.select2").each(function () {
                Common.BindSelect2($(this));
            });
            Common.InitNumerics();
            $this.AutoCompleteInit();
        },
        AddCustomer: function ($button) {
            var $this = this;
            var $tr = $button.closest("tr");
            var productId = Common.GetInt($tr.attr("data-product"));
            var html = $this.GetDataRowHtml(productId, 0, '', '', null);
            //html += $this.GetDataRowTotalHtml(productId, null);
            $(html).insertBefore($tr);
            $this.AutoCompleteCustomersInit();
            //$this.LoadSaleByProductIds($tr);
        },



        GetHeaderHtml: function (product, divisionId) {
            var $this = this;
            var html = "";
            if (Common.isNullOrWhiteSpace(product))
                html = '<tr class="tr-month-name"><th><input type="text" class="Product form-control ui-autocomplete-input" data-db-column="Product" autocomplete="off"></th>';
            else
                html = '<tr data-divisionid="' + divisionId + '"  class="tr-month-name"><th><input  type="text" disabled="disabled" readonly="readonly" class="Product form-control ui-autocomplete-input" data-db-column="Product" value="' + product + '" autocomplete="off"></th>';
            var months = $this.GetMonths()
            var k = 1;
            for (var i in months) {
                var month = months[i];
                var monthNo = month.Month;
                var year = month.Year;
                var customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"';
                if (k > 12)
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-next="true"';
                else
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-prev="true"';

                html += '<th style="width:3%" class="Month1 align-right" ' + customeraAttr + '>' + moment(month.Date).format('MMM-YY') + '</th>';
                k++;
            }
            html += '<th style="width:15%">Remarks</th>';
            html += '<th style="width: 2%">Action</th>';

            html += '</tr>';
            return html;
        },
        GetDataRowHtml: function (productId, divisionId, customerId, customer, remarks, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' data-product='" + productId + "' data-body='true'>";
            html += "<td>";
            html += "<input type='hidden' class='ProductId' data-db-column='ProductId' value='" + productId + "'>"
            html += "<input type='hidden' class='CustomerId' data-db-column='CustomerId' value='" + customerId + "'>"
            html += "<input type='text' class='Customer form-control' data-db-column='Customer' value='" + customer + "' >"
            html += "</td>";
            var monthCounter = 1;
            var d = new Date();
            var currentMonth = d.getMonth() + 1;
            var currentYear = d.getFullYear();
            for (var m in Months) {
                var m = Months[m];
                var disable = "";

                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.CustomerId == customerId }).Sum("$.Quantity");
                if (monthCounter <= 12)
                    html += "<td  class='align-right'>" + qty + "</td>";
                else {
                    if (m.Month == currentMonth && m.Year == currentYear)
                        disable = "disabled=disabled";
                    html += "<td  class='align-right'><input type='text' " + disable + " class='form-control input-small num4 Month" + monthCounter + "' data-db-column='Month" + monthCounter + "' data-db-type='int'  value='" + qty + "'></td>";
                }

                monthCounter++;
            }
            if (remarks == null)
                remarks = "";
            html += "<td><input type='text' class='Remarks form-control input-medium' data-db-column='Remarks' value='" + remarks + "' ></td>"
            html += "<td class='td-delete'><span class='action'><i class='fa fa-trash-o action-delete' data-original-title='Delete Item'></i></span></td>";
            html += "</tr>";
            return html;
        },
        GetDataRowTotalHtml: function (productId, divisionId, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' class='bold grand-total' data-product='" + productId + "'>";
            html += "<td class='align-right' colspan='1'> <a id='btn-add-customer' class='btn btn-xs green pull-left' href='javascript:;'>Add Customer</a>&nbspTotal</td>";
            var monthCounter = 1
            for (var m in Months) {
                var m = Months[m];
                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year }).Sum("$.Quantity");
                html += "<td  class='align-right'>" + qty + "</td>";
                monthCounter++;
            }
            html += "<td colspan='2'>";
            html += "</tr>";
            return html;
        },
        GetMonths: function () {
            var $this = this;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            var d = new Date($("#Year").val(), month - 1, 28);
            var startDate = moment(d).subtract(12, 'months').toDate();
            var endDate = moment(d).add(3, 'M').toDate();
            var k = 1
            var startMonth = month - 3
            var Months = new Array();
            while (startDate <= endDate) {
                var monthNo = startDate.getMonth() + 1;
                var year = startDate.getFullYear();
                Months.push({ Month: monthNo, Year: year, Date: startDate })
                startDate = moment(startDate).add(1, 'M').toDate();
                k++;
            }
            return Months;
        },
        DeleteRow: function (element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            var id = Common.GetInt($($tr).find("input.Id").val());
            if (id > 0) {
                $this.Delete(id, $tr);

            }

            else {
                $tr.remove();
            }

        },

        Save: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {

                var err = "";
                var recrods = new Array();

                var salePersonId = Common.GetInt($("#SalePersonId").val());
                $("#item-container tbody tr[data-product][data-body]").each(function () {
                    var $tr = $(this);
                    var productId = Common.GetInt($tr.find("input.ProductId").val());
                    var customerId = $tr.find("input.CustomerId").val();
                    var remarks = $tr.find("input.Remarks").val();
                    var $headers = $("#item-container tr.tr-month-name:first").find("th[data-month][data-next]");

                    $headers.each(function () {
                        var index = $(this).index();
                        var month = $(this).attr("data-month");
                        var year = $(this).attr("data-year");
                        var qty = $tr.find("td:eq(" + index + ") input[data-db-column^='Month']").val();
                        if (qty > 0) {
                            recrods.push(
                               {
                                   SalePersonId: salePersonId,
                                   ForecaseMonth: Common.GetInt($("#Month").val()),
                                   ForecastYear: Common.GetInt($("#Year").val()),
                                   CustomerId: customerId,
                                   ProductId: productId,
                                   Month: month,
                                   Year: year,
                                   Quantity: qty,
                                   Remarks: remarks,
                                   Type: CRMSaleForecastType.SalePerson
                               })
                        }
                    });
                });

                if (recrods.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record =
                    {
                        '': recrods
                    };
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.LoadSales();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        LoadSaleByCustomerId: function ($tr) {
            var $this = this;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var salePersonId = Common.GetInt($("#SalePersonId").val());
            var productId = Common.GetInt($tr.find("input.ProductId").val());
            var customerId = Common.GetInt($tr.find("input.CustomerId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/0?month=" + month + "&year=" + year + "&productId=" + productId + "&salePersonId=" + salePersonId + "&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        $(".tr-month-name").find("th[data-month]").each(function () {
                            var index = $(this).index();
                            var month = $(this).attr("data-month");
                            var year = $(this).attr("data-year");
                            var qty = Common.GetInt(Enumerable.From(data).Where(function (x) { return x.Month == month && x.Year == year }).Sum("$.Quantity"));
                            if (qty > 0) {
                                if ($tr.find("td:eq(" + index + ") input").length)
                                    $tr.find("td:eq(" + index + ") input[data-db-column^='Month']").val(qty);
                                else
                                    $tr.find("td:eq(" + index + ")").html(qty);
                            }

                        });


                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        FilterByDivision: function () {
            var divisionId = Common.GetInt($("select#DivisionId").val());
            if (divisionId > 0) {
                $("tr[data-divisionid]").addClass("hide");
                $("tr[data-divisionid='" + divisionId + "']").removeClass("hide");
            }
            else {
                $("tr[data-divisionid]").removeClass("hide");
            }
        },
        LoadSales: function () {
            var $this = this;
            var id = 0;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var salePersonId = Common.GetInt($("#SalePersonId").val());
            if (!(month > 0))
                return;
            if (salePersonId == 0 && ForecastType == CRMSaleForecastType.SalePerson)
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/0?month=" + month + "&year=" + year + "&salePersonId=" + salePersonId + "&type=" + ForecastType,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        var saleForecastRecrods = res.Data;
                        $("#item-container tbody").html("");
                        $("#saleforecast-recrod-container").removeClass("hide");
                        var html = $this.DrawSummary(res);
                        $("#item-container-summary tbody").html(html);
                        html = $this.DrawDetail(res);
                        $("#item-container tbody").html(html);
                        Common.InitNumerics();
                        $this.AutoCompleteInit();
                        if (saleForecastRecrods.length == 0 && ForecastType != CRMSaleForecastType.RSM)
                            $this.AddItem();
                        $this.FilterByDivision();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        DrawSummary: function (res) {
            var html = "";
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var saleForecastRecrods = res.Data;
            var groupByProducts = Enumerable.From(saleForecastRecrods).GroupBy("$.ProductId", null,
                          function (key, g) {
                              var result = {
                                  ProductId: key,
                                  Product: g.FirstOrDefault().Product,
                                  DivisionId: g.FirstOrDefault().DivisionId,
                                  Records: g.ToArray()
                              }
                              return result;
                          }).ToArray();
            var headerHtml = '<tr class="tr-month-name"><th>Product</th>';
            var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];


            var d = new Date($("#Year").val(), month - 1, 28);
            var startDate = moment(d).subtract(12, 'months').toDate();
            var endDate = moment(d).add(3, 'M').toDate();
            var k = 1
            var startMonth = month - 3
            var Months = new Array();
            while (startDate <= endDate) {
                var monthNo = startDate.getMonth() + 1;
                var year = startDate.getFullYear();
                headerHtml += '<th style="width:5%" class="Month1 align-right" data-month="' + monthNo + '" data-year="' + year + '">' + moment(startDate).format('MMM-YY') + '</th>';
                startDate = moment(startDate).add(1, 'M').toDate();
                Months.push({ Month: monthNo, Year: year })
                k++;
            }
            headerHtml += '</tr>';
            html += headerHtml;
            for (var i in groupByProducts) {
                var rate = 0;
                var saleForecastByProduct = groupByProducts[i];
                var records = saleForecastByProduct.Records;

                html += "<tr data-divisionid='" + saleForecastByProduct.DivisionId + "'>";
                html += "<td>" + saleForecastByProduct.Product + "</td>"
                for (var m in Months) {
                    var m = Months[m];
                    var qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.ProductId == saleForecastByProduct.ProductId }).Sum("$.Quantity");
                    html += "<td class='align-right'>" + qty + "</td>"
                }
                html += "</tr>";

            }
            return html;

        },
        DrawDetail: function (res) {
            var $this = this;
            var html = "";
            var saleForecastRecrods = res.Data;
            var groupByProducts = Enumerable.From(saleForecastRecrods).GroupBy("$.ProductId", null,
                       function (key, g) {
                           var result = {
                               ProductId: key,
                               Product: g.FirstOrDefault().Product,
                               DivisionId: g.FirstOrDefault().DivisionId,
                               Records: g.ToArray()
                           }
                           return result;
                       })
                //.Take(2)
                .ToArray();

            for (var i in groupByProducts) {
                var rate = 0;
                var product = groupByProducts[i];
                html += $this.GetHeaderHtml(product.Product, product.DivisionId);
                var records = product.Records;
                var groupByCustomers = Enumerable.From(records).GroupBy("$.CustomerId", null,
                      function (key, g) {
                          var result = {
                              CustomerId: key,
                              Customer: g.FirstOrDefault().Customer,
                              Remarks: g.FirstOrDefault('', "$.Remarks != ''").Remarks,
                              Records: g.ToArray()
                          }
                          return result;
                      }).ToArray();

                for (var p in groupByCustomers) {
                    var groupByCustomer = groupByCustomers[p];
                    html += $this.GetDataRowHtml(product.ProductId, product.DivisionId, groupByCustomer.CustomerId, groupByCustomer.Customer, groupByCustomer.Remarks, groupByCustomer.Records)
                }
                html += $this.GetDataRowTotalHtml(product.ProductId, product.DivisionId, records)
            }
            return html;
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
        LoadCustomers: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetCustomerIdNameByUserId",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {

                        PageData.Customers = res.Data;
                        $this.AutoCompleteCustomersInit();

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
            var customerId = $tr.find("select.CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetProjects&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Products = res.Data;
                        $this.AutoCompleteProjectsInit(res.Data, $tr);
                        if (res.Data.length == 1) {
                            var project = res.Data[0];
                            $($tr).find("input.ProjectId").val(project.Id);
                            $($tr).find("input.Project").val(project.Name);
                            $($tr).find("input.ProductId").val(project.ProductId);
                            $($tr).find("input.Product").val(project.Product);
                        }
                        else {
                            $($tr).find("input.Project").val("");
                            $($tr).find("input.ProductId").val(0);
                            $($tr).find("input.Product").val("");
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
        AutoCompleteInit: function () {
            var $this = this;
            var products = PageSetting.Products;
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
            $(".Product").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    var productId = ui.item.id;
                    var html = $this.GetDataRowHtml(productId, 0, '', '', null);
                    html += $this.GetDataRowTotalHtml(productId, null);
                    $(tr).after($(html));
                    $this.AutoCompleteCustomersInit();
                    // $this.LoadSaleByProductIds(tr);
                }
            });

        },
        AutoCompleteCustomersInit: function () {
            var $this = this;
            var customers = PageData.Customers;
            var suggestion = new Array();
            for (var i in customers) {
                var customer = customers[i];
                suggestion.push(
                    {
                        id: customer.Id,
                        value: customer.Name,
                        label: customer.Name,
                        name: customer.Name,
                        Customer: customer

                    }
                );
            }
            $(".Customer").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var $tr = $(this).closest("tr");
                    $tr.find("input.CustomerId").val(ui.item.id);
                    $this.LoadSaleByCustomerId($tr);
                }
            });

        },

        AutoCompleteSalePerson: function () {
            var $this = this;

            var salePersons = PageSetting.SalePersons;
            var suggestion = new Array();
            for (var i in salePersons) {
                var salePerson = salePersons[i];
                suggestion.push(
                    {
                        id: salePerson.Id,
                        value: salePerson.Name,
                        label: salePerson.Name,
                        name: salePerson.Name,


                    }
                );
            }
            $("#SalePerson").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    $("#SalePersonId").val(ui.item.id);
                    $this.LoadSales();

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
        Delete: function (id, $tr) {
            var $this = this;
            var productId = Common.GetInt($tr.find("input.ProductId").val());
            var customerId = Common.GetInt($tr.find("input.CustomerId").val());
            var year = Common.GetInt($("#Month option:selected").attr("data-custom"));
            var month = Common.GetInt($("#Month").val());
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
                            $($tr).remove();
                            Common.ShowMessage(true, { message: "Record deleted successfully." });
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
