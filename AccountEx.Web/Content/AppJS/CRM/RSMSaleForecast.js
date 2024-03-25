
var RSMSaleForecast = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMSaleForecastRSM";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var PageSetting = new Object();
    var PageData = new Object();
    var ForecastType = CRMSaleForecastType.RSM;
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
        GetHeaderHtml: function (product, divisionId) {
            var $this = this;
            var html = "";

            html = '<tr data-divisionid="' + divisionId + '" class="tr-month-name"><th>' + product + '</th>';
            html += '<th>Sale Person</th>';
            var months = $this.GetMonths()
            var k = 1;
            for (var i in months) {
                var month = months[i];
                var monthNo = month.Month;
                var year = month.Year;
                var customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"';
                if (k > 12)
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-next="true"';
                html += '<th style="width:5%" class="Month1 align-right" ' + customeraAttr + '>' + moment(month.Date).format('MMM-YY') + '</th>';
                k++;
            }
            html += '</tr>';
            return html;
        },
        GetSummaryHeaderHtml: function (product) {
            var $this = this;
            var html = "";

            html = '<tr class="tr-month-name"><th>' + product + '</th>';
            var months = $this.GetMonths()
            var k = 1;
            for (var i in months) {
                var month = months[i];
                var monthNo = month.Month;
                var year = month.Year;
                var customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"';
                if (k > 12)
                    customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-next-sp="true"';
                html += '<th style="width:5%" class="Month1 align-right" ' + customeraAttr + '>' + moment(month.Date).format('MMM-YY') + '</th>';
                k++;
            }
            k = 1;
            for (var i in months) {

                if (k <= 12) {
                    k++;
                    continue;
                }
                var month = months[i];
                var monthNo = month.Month;
                var year = month.Year;
                customeraAttr = 'data-month="' + monthNo + '" data-year="' + year + '"' + '" data-next="true"';
                html += '<th style="width:5%" class="Month1 align-right" ' + customeraAttr + '>' + moment(month.Date).format('MMM-YY') + '</th>';
                k++;
            }
            html += '</tr>';
            return html;
        },
        GetDataRowHtml: function (productId, divisionId, customerId, customer, sp, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' data-product='" + productId + "'>";
            html += "<td>" + customer + "</td>";
            html += "<td>" + sp + "</td>";
            var monthCounter = 1
            for (var m in Months) {
                var m = Months[m];
                var disable = "";
                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.CustomerId == customerId }).Sum("$.Quantity");
                html += "<td  class='align-right'>" + qty + "</td>";
                monthCounter++;
            }
            html += "</tr>";
            return html;
        },
        GetDataRowTotalHtml: function (productId, divisionId, records) {
            var $this = this;
            var html = "";
            var Months = $this.GetMonths();
            html += "<tr data-divisionid='" + divisionId + "' class='bold grand-total'>";
            html += "<td colspan='2' class='align-right' colspan='1'>Total</td>";
            var monthCounter = 1
            for (var m in Months) {
                var m = Months[m];
                var qty = 0;
                if (records != null)
                    qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year }).Sum("$.Quantity");
                html += "<td  class='align-right'>" + qty + "</td>";
                monthCounter++;
            }
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
                var $headers = $("#item-container-summary tr.tr-month-name:first").find("th[data-month][data-next]");
                $("#item-container-summary tbody tr[data-product]").each(function () {
                    var $tr = $(this);
                    var productId = Common.GetInt($tr.attr("data-product"));
                    var customerId = $tr.find("input.CustomerId").val();
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
                                   Type: CRMSaleForecastType.RSM
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
                            // $this.LoadEvents();
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
        LoadSaleByProductIds: function ($tr, productIds) {
            var $this = this;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            var salePersonId = Common.GetInt($("#SalePersonId").val());
            var productId = Common.GetInt($tr.attr("data-product"));
            var customerId = Common.GetInt($tr.find("input.CustomerId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/0?month=" + month + "&year=" + year + "&productId=" + productId + "&salePersonId=" + salePersonId + "&type=" + ForecastType + "&customerId=" + customerId,
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
                            var qty = Enumerable.From(data).Where(function (x) { return x.Month == month && x.Year == year }).Sum("$.Quantity");
                            $tr.find("td:eq(" + index + ") input[data-db-column^='Month']").val(qty);
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
        LoadSales: function () {
            var $this = this;
            var id = 0;
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());
            if (!(month > 0))
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/0?month=" + month + "&year=" + year,
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
            var $this = this;
            var saleForecastRecrods = res.Data;
            var html = "";
            var Months = $this.GetMonths();
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

            html = $this.GetSummaryHeaderHtml("Product");
            for (var i in groupByProducts) {
                var rate = 0;
                var saleForecastByProduct = groupByProducts[i];
                var records = saleForecastByProduct.Records;
                var productId = saleForecastByProduct.ProductId;
                html += "<tr data-divisionid='" + saleForecastByProduct.DivisionId + "' data-product=" + productId + ">";
                html += "<td>" + saleForecastByProduct.Product + "";
                html += "</td>";

                monthCounter = 1;
                for (var m in Months) {
                    var m = Months[m];

                    var qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.ProductId == saleForecastByProduct.ProductId && x.Type == 0 }).Sum("$.Quantity");
                    //if (monthCounter <= 12)
                    html += "<td  class='align-right'>" + qty + "</td>";
                    //else
                    //    html += "<td  class='align-right'><input type='text' class='form-control input-small num4 Month" + monthCounter + "' data-db-column='Month" + monthCounter + "' data-db-type='int'  value='" + qty + "'></td>";
                    monthCounter++;
                }

                monthCounter = 1;
                for (var m in Months) {
                    if (monthCounter <= 12) {
                        monthCounter++;
                        continue;
                    }
                    var m = Months[m];

                    var qty = Enumerable.From(records).Where(function (x) { return x.Month == m.Month && x.Year == m.Year && x.ProductId == saleForecastByProduct.ProductId && x.Type == 1 }).Sum("$.Quantity");
                    html += "<td  class='align-right'><input type='text' class='form-control input-small num4 Month" + monthCounter + "' data-db-column='Month" + monthCounter + "' data-db-type='int'  value='" + qty + "'></td>";
                    monthCounter++;
                }



                html += "<td class='td-delete'><span class='action'><i class='fa fa-trash-o action-delete' data-original-title='Delete Item'></i></span></td>";
                html += "</tr>";

            }
            return html;

        },
        DrawDetail: function (res) {
            var $this = this;
            var html = "";
            var saleForecastRecrods = res.Data;
            var groupByProducts = Enumerable.From(saleForecastRecrods).Where(function (x) { return x.Type == 0 }).GroupBy("$.ProductId", null,
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
                              SalePerson: g.FirstOrDefault().SalePerson,
                              Records: g.ToArray()
                          }
                          return result;
                      }).ToArray();

                for (var p in groupByCustomers) {
                    var groupByCustomer = groupByCustomers[p];
                    html += $this.GetDataRowHtml(product.ProductId, product.DivisionId, groupByCustomer.CustomerId, groupByCustomer.Customer, groupByCustomer.SalePerson, groupByCustomer.Records)
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
                    var html = $this.GetDataRowHtml(productId, 0, '', null);
                    html += $this.GetDataRowTotalHtml(productId, null);
                    $(tr).after($(html));
                    $this.AutoCompleteCustomersInit();
                    $this.LoadSaleByProductIds(tr);
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
                    $this.LoadSaleByProductIds($tr);
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
