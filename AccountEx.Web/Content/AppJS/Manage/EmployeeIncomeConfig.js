var EmployeeIncomeConfig = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "EmployeeIncomeConfig";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var grosssalary = 0;
    var recordtype = "Save";
    return {
        init: function () {
            var $this = this;
            $(document).on("keyup", ".BasicSalary,.EOBI,.HouseAllowance,.MedicalAllowance,.ConveyanceAllowance,.ProvidentFund,.EOBI,.SST", function (event) {
                var tr = $(this).parent().parent();
                $this.CalculateGrossSalary(tr);
            });
            $('#Month').on('select2-selecting', function (e) {
                console.log('here');
            });

            $("#FromMonth").change(function () {
                var frommonth = Common.GetInt($("#FromMonth").val());
                var fromyear = Common.GetInt($("#FromYear").val());

                var firstDate = $this.GetFirstDate(frommonth, fromyear);
                var lastDate = $this.AddOneYear(frommonth, fromyear);

                $("#FromDate").val(firstDate);
                $("#ToDate").val(lastDate);

            });
            $("#FromYear").change(function () {
                var frommonth = Common.GetInt($("#FromMonth").val());
                var fromyear = Common.GetInt($("#FromYear").val());

                var firstDate = $this.GetFirstDate(frommonth, fromyear);
                var lastDate = $this.AddOneYear(frommonth, fromyear);

                $("#FromDate").val(firstDate);
                $("#ToDate").val(lastDate);
            });

            $("#SelectedLeaveMonth").change(function () {
               
                var fromyear = Common.GetInt($("#FromYear").val());
                var selectedleavemonth = Common.GetInt($("#SelectedLeaveMonth").val());

                var lastDate = $this.AddOneYear(selectedleavemonth, fromyear);
                var firstDate = $this.GetFirstDate(selectedleavemonth, fromyear);

                $("#LeaveMonthStart").val(firstDate);
                $("#LeaveBankEnd").val(lastDate);
              
            });

            $this.ListView();
        },

        AddOneYear: function (month,year) {
            var leavemonthend = new Date(year + 1, month - 1, 0);
            leavemonthend = moment(leavemonthend).format('DD/MM/YYYY');
            return leavemonthend;
        },
        GetFirstDate: function (month, year) {
            var firstDate = "01/" + month + "/" + year;
            return firstDate;
        },

        CalculateGrossSalary: function (tr) {
            $this = this;
            var basicSalary = 0;
            var houseAllowance = 0.0;
            var medicalAllowance = 0;
            var conveyanceAllowance = 0;

            var providentFund = 0.0;
            var eOBI = 0.0;
            var sSt = 0.0;

            basicSalary = Common.GetInt($(tr).find(":nth-child(5) input.BasicSalary").val());

            houseAllowance = Common.GetFloat($(tr).find(":nth-child(6) input.HouseAllowance").val());
            conveyanceAllowance = Common.GetFloat($(tr).find(":nth-child(7) input.ConveyanceAllowance").val());
            medicalAllowance = Common.GetFloat($(tr).find(":nth-child(8) input.MedicalAllowance").val());

            providentFund = Common.GetFloat($(tr).find(":nth-child(10) input.ProvidentFund").val());
            eOBI = Common.GetFloat($(tr).find(":nth-child(11) input.EOBI").val());
            sSt = Common.GetFloat($(tr).find(":nth-child(12) input.SST").val());

            if (houseAllowance < 1 && houseAllowance > 0)
                houseAllowance = basicSalary * houseAllowance;
            if (conveyanceAllowance < 1 && conveyanceAllowance > 0)
                conveyanceAllowance = basicSalary * conveyanceAllowance;
            if (medicalAllowance < 1 && medicalAllowance > 0)
                medicalAllowance = basicSalary * medicalAllowance;

            if (providentFund < 1 && providentFund > 0)
                providentFund = basicSalary * providentFund;
            if (eOBI < 1 && eOBI > 0)
                eOBI = basicSalary * eOBI;
            if (sSt < 1 && sSt > 0)
                sSt = basicSalary * sSt;

            var grossSalary = (basicSalary + houseAllowance + medicalAllowance + conveyanceAllowance) - (providentFund + eOBI + sSt);
            if (grossSalary >= 0)
                $(tr).find(":nth-child(9) input.GrossSalary ").val(grossSalary);
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        MapComments: function () {
            var $this = this;
            var type = $this.GetType();
            if (type == "sale") {

                var html = "Book No:" + $("#InvoiceNumber").val() + ", Dc No:" + $("#DCNo").val() + ", Order No:" + $("#OrderNo").val();
                $("#Comments").val(html);
            }
        },

        New: function () {

            var $this = this;
            SetFocus = "booknumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        ChangeType: function (type) {
            var $this = this;
            window.history.pushState(type, document.title + " | " + type, "index?type=" + type);
            //document.title = document.title + " | " + type;
            $this.LoadPageSetting();
            if ($("#div-table").is(":visible")) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            } else if ($("#form-info").is(":visible")) {
                $this.Add();
            }
            Common.HighlightMenu();
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },

        GetDiscountDetail: function (productid) {

            var customerid = $("#AccountId").val();
            var discount = $.grep(AppData.CustomerDiscount, function (e) { return e.CustomerId == customerid && e.COAProductId == productid; })[0];
            if (discount != null)
                return discount.Discount;
            else
                return 0;
        },
        GetByCode: function (code) {
            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetByBarCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return (e.BarCode != null ? e.BarCode.toLowerCase() : "") == code.toLowerCase() || e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetByCodeFromCOA: function (code) {
            var accounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel; });
            var data = $.grep(accounts, function (e) { return e.AccountCode.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        GetProductDetail: function () {

            var product = $.parseJSON($("#Item option:selected").attr("data-detail"));
            $("#Rate").val(product.SalePrice > 0 ? product.SalePrice : "");
            $("#lbldiscount").html(product.Discount + " %");
            $("#DiscountPercent").val(product.Discount);

        },
        Add: function () {
            var $this = this;
            recordtype = "Save";
            Common.Clear();
            $this.DetailView();
            $this.CustomClear();
            $this.LoadPageSetting();
            $this.AddItem();
          

            //this.GetNextVoucherNumber();
            //$(".container-message").hide();
        },
        ListView: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        AddItem: function () {

            var $this = this;
            $("#item-container tbody tr").remove();
            var headid = PageSetting.Employees;
            var tokens = Common.GetLeafAccounts(headid);
            Common.MapItemData(tokens, null, null, true);

            //var html = "";
            //for (var i in tokens) {
            //    var employee = tokens[i];
            //    var templateHtml = $("#template-item").html();
            //    employee["AccountId"] = employee.Id;
            //    var placeholder = $this.GetPlaceHolder(employee);
            //    templateHtml = templateHtml.allReplace(placeholder);
            //    html += templateHtml;
            //}
          
            //$("#item-container tbody").append(html);

            $("#Name").prop("disabled", false);
            $("#FromMonth").prop("disabled", false);
            $("#FromYear").prop("disabled", false);
            $("#ToMonth").prop("disabled", false);
            $("#ToYear").prop("disabled", false);
        },

        GetPlaceHolder: function (data) {
            var record = new Object();
            for (var key in data) {
                record["[" + key + "]"] = data[key];
            }
            return record;
        },
        ReinializePlugin: function () {
            Common.AllowNumerics();
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },
        CloseItem: function () {
            Common.Clear();
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            $("#form-info-item").addClass("hide");
            $("#masterdetail").removeClass("hide");
            $("#div-table-item").addClass("hide");
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.ListView();
                $this.RebindData();
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                //$this.GetNextVoucherNumber();
                var scope = $("#form-info-item");
                //$this.CustomClear();
                $this.ListView();
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            //var fromDate = "01/" + $("#FromMonth").val() + "/" + $("#FromYear").val();
            //var toDate = "30/" + $("#ToMonth").val() + "/" + $("#ToYear").val();
            //fromDate = Common.ChangeDateFormate(fromDate);
            //toDate = Common.ChangeDateFormate(toDate);
            //var name = $("#Name").val();

            var Items = new Array();
            var scope = $("#form-info");
            var record = Common.SetValue(scope);

            if (Common.Validate($("#mainform"))) {

                //$("#item-container tbody tr").each(function () {
                //    var EmployeeConfig = new Object();
                //    EmployeeConfig["FromDate"] = fromDate;
                //    EmployeeConfig["ToDate"] = toDate;
                //    EmployeeConfig["Name"] = name;
                //    $(this).find("input,select").each(function () {
                //        var DbId = $(this).attr('class').split(' ')[0];
                //        EmployeeConfig[DbId] = $(this).val();
                //    });
                //    Items.push(EmployeeConfig);
                //});

                Items = Common.SaveItemData();

                record["EmployeeIncomeList"] = Items;
                record.RecordType = recordtype;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
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
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0;
            var Price = 0;
            var discount = 0;
            var netamount = 0;
            $("#item-container tbody tr").each(function () {
                Quantity += Common.GetInt($(this).find(":nth-child(3) input.Quantity").val());
                Price += Common.GetInt($(this).find(":nth-child(5) input.Amount").val());
                discount += Common.GetInt($(this).find(":nth-child(7) input.DiscountAmount").val());
                netamount += Common.GetInt($(this).find(":nth-child(8) input.NetAmount").val());
            });
            $("#item-container tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity);
            $("#item-container tfoot tr").find(":nth-child(3) input.Amount").val(Price);
            $("#item-container tfoot tr").find(":nth-child(4) input.DiscountAmount").val(discount);
            $("#item-container tfoot tr").find(":nth-child(5) input.NetAmount").val(netamount);
            $("#qtytotal1").val(Quantity);
            $("#QuantityTotal").val(Quantity);
            if (Price === 0)
                $("#GrossTotal").val("");
            else
                $("#GrossTotal").val(Price);
            var incAmount = Price - discount;
            $("#Discount").val(discount);
            $("#NetTotal").val(incAmount + "");
            $this.CalculatePreviousBalance();
            //this.GetNetTotal();
        },
        GetNetTotal: function () {
            var total = Common.GetInt($("#amounttotal").val());
            var discount = Common.GetInt($("#Discount").val());
            var nettotal = Common.GetInt(total - discount);

            $("#Nettotal").val(nettotal);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            //$("#item-container tbody").html("");
            //$("#lblcurrentbalance").html("00");
            //$("#lblpreviousbalance").html("00");
            //$("#AccountCode").removeAttr("disabled");
            //$("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadDeliveryChallan: function (key) {

            var $this = this;
            var orderno = Common.GetInt($("#DCNo").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "DeliveryChallan/" + orderno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + orderno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {

                        var voucherno = $("#VoucherNumber").val();
                        var invoiceno = $("#InvoiceNumber").val();
                        var d = res.Data.Order;
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else if (d.Status == 5) {
                            Common.ShowError("Challan has already processed.");
                            SetFocus = "dcnumber";
                        }
                        else {
                            Common.MapEditData(d, "#form-info");
                            $("#VoucherNumber").val(voucherno);
                            $("#InvoiceNumber").val(invoiceno);
                            $("#qtytotal1").val(d.QuantityTotal);
                            $("#AccountCode").trigger("blur");
                            $("#Id").val(0);
                            $("#item-container tbody").html("");
                            $("#OBNo").val(d.InvoiceNumber);
                            $("#OrderDate").val(d.Date);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            var items = d.DCItems;
                            if (d.Id > 0 && items != null && items.length > 0) {

                                var html = "";
                                for (var i in items) {
                                    var item = items[i];
                                    var product = $this.GetByCode(item.ItemCode);
                                    var discountpercent = 0;
                                    var price = 0;
                                    var discount = 0;
                                    var qty = item.Quantity;
                                    if (typeof product != "undefined" && product != null) {
                                        discountpercent = Common.GetFloat($this.GetDiscountDetail(product.AccountId));
                                        price = Common.GetFloat(product.SalePrice);
                                    }
                                    var amount = qty * price;
                                    var discountAmount = Common.GetInt(amount) * discountpercent / 100;
                                    var netAmount = Common.GetInt(amount) - discountAmount;
                                    html += "<tr>";
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden' id='Id' value='" + item.Id + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Rate form-control input-small' value='" + price + "' ></td>";
                                    html += "<td><input type='text' value='" + amount + "' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td><input type='text' value='" + discountpercent + "' class='DiscountPercent form-control input-small'></td>";
                                    html += "<td><input type='text' value='" + discountAmount + "'  class='DiscountAmount  form-control input-small' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td><input type='text' value='" + netAmount + "' class='NetAmount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"Trans.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#item-container tbody").append(html);
                                $this.MapComments();
                                //setTimeout(function () {
                                //    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                //}, 500);
                            }
                            $this.GetWholeTotal();
                        }

                        $this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetTransporters: function () {
            var $this = this;
            $this.TransporterAutoCompleteInit(PageSetting.Transporters);
            //Common.WrapAjax({
            //    url: Setting.APIBaseUrl + "Misc/?key=GetTransporters",
            //    type: "GET",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (res) {
            //        if (res.Success) {

            //            $this.TransporterAutoCompleteInit(res.Data);
            //        }

            //    },
            //    error: function (e) {
            //    }
            //});
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);
            var type = $this.GetType();
            if (type == "sale") {
                $("#lblReportTitle").html("Sales Estimate");
                $("#lblFooterNotes").html(" * In case of any error in invoice please inform us within 7 working days");
            }
            else if (type == "purchase") {
                $(".row-sale").addClass("hide");
                $("#lblReportTitle").html("Purchase Voucher");
                $("#lblFooterNotes").html(" * All imported items are not returnable");
            }
            var html = "";
            var items = d.salarysetups;
            var index = 1;
            $("#report-item-container tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "<td>" + item.DiscountAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-item-container tbody").append(html);
            html = "";
            $("#tblAgingItems tbody").html("");
            for (var i in res.Data.AgingItems) {
                var item = res.Data.AgingItems[i];
                html += "<tr>";
                html += "<td></td>";
                html += "<td>" + item.VoucherNumber + "</td>";
                html += "<td></td>";
                html += "<td>" + moment(item.Date).format("DD/MM/YYYY") + "</td>";
                html += "<td></td>";
                html += "<td>" + item.DueAmount.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Balance.format() + "</td>";
                html += "<td></td>";
                html += "<td>" + item.Age + "</td>";
                html += "</tr>";
            }
            html += "<tr style='font-size:1.2em;'><th colspan='6' class='align-right' style='padding-top:10px;'>Current Balance :</th><th colspan='4' class='align-center' style='padding-top:10px;'>" + res.Data.Balance.format() + "</th></tr>";
            $("#tblAgingItems tbody").append(html);
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber,#InvoiceNumber").val(res.Data.VoucherNumber);
                        }
                        else {
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            $("#DCNo").prop("disabled", "disabled");
                            //alert($("#Date").val());
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (!d.CashSale && d.Id > 0) {
                                $("input:radio[value='credit']").prop("checked", true);
                                $.uniform.update();
                            }
                            else {
                                $("input:radio[value='cash']").prop("checked", true);
                                $.uniform.update();
                                if (d.Id == 0)
                                    $("input:radio[value='credit']").trigger("change");
                            }
                            if (d.Id > 0 && d.item - containers != null && d.item - containers.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                $this.LoadReportData(res);
                                $this.GetPreviousBalance(d.AccountId);
                                var html = "";
                                var items = d.item - containers;
                                for (var i in items) {
                                    var item = items[i];
                                    html += "<tr>";
                                    html += "<td><input type='hidden' class='ItemId' id='ItemId' value='" + item.ItemId + "'>";
                                    html += "<input type='hidden' id='Id' value='" + item.Id + "'>";
                                    html += "<input type='text' class='Code form-control typeahead input-small' value='" + item.ItemCode + "'></td>";
                                    html += "<td><input type='text' disabled='disabled' class='Name form-control input-medium' value='" + item.ItemName + "'></td>";
                                    html += "<td><input type='text' class='Quantity form-control input-small' value='" + item.Quantity + "'></td>";
                                    html += "<td><input type='text' class='Rate form-control input-small'value='" + item.Rate + "' ></td>";
                                    html += "<td><input type='text' value='" + item.Amount + "' class='Amount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td><input type='text' value='" + item.DiscountPercent + "' class='DiscountPercent form-control input-small'></td>";
                                    html += "<td><input type='text' value='" + item.DiscountAmount + "'  class='DiscountAmount  form-control input-small' disabled='disabled' readonly='readonly' ></td>";
                                    html += "<td><input type='text' value='" + item.NetAmount + "' class='NetAmount form-control input-small' disabled='disabled' readonly='readonly'></td>";
                                    html += "<td style=\"width: 8px;\"><span class=\"action\"><i class=\"fa fa-trash-o\" onclick=\"Trans.DeleteRow(this)\" data-original-title=\"Delete Item\"></i></span></td>";
                                    html += "</tr>";
                                }
                                $("#item-container tbody").append(html);
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.GetWholeTotal();
                            }
                        }
                        //if (res.Data.Next)
                        //    $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        //else
                        //    $(".form-actions .next,.form-actions .last").addClass("disabled");
                        //if (res.Data.Previous)
                        //    $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        //else
                        //    $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        //$this.AddItem();
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        GetPreviousBalance: function (customerid) {
            var $this = this;
            var type = $this.GetType();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetPreviousBalance&accountid=" + customerid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var id = Common.GetInt($("#Id").val());
                        if (id == 0) {
                            $("#lblpreviousbalance").html(Common.GetFloat(res.Data).format());
                        }
                        else {
                            var currentbalance = Common.GetFloat(res.Data);
                            var invoicetotal = Common.GetFloat($("#NetTotal").val());
                            $("#lblpreviousbalance").html((currentbalance - invoicetotal).format());
                        }
                        $this.CalculatePreviousBalance();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        CalculatePreviousBalance: function () {

            var $this = this;
            var type = $this.GetType().toLowerCase();
            var currentbalance = 0;
            var invoicetotal = Common.GetFloat($("#NetTotal").val());
            var previousbalance = Common.GetFloat($("#lblpreviousbalance").html());
            var currentbalance = 0;
            if (type == "sale" || type == "purchasereturn") {
                currentbalance = previousbalance + invoicetotal;
            }
            else {
                currentbalance = previousbalance - invoicetotal;
            }
            $("#lblcurrentbalance").html((currentbalance).format());

        },
        GetCustomerProducts: function (id) {
            var $this = this;
            AppData.CustomerDiscount = PageSetting.Discounts;
            //Common.WrapAjax({
            //    url: Setting.APIBaseUrl + "Misc/?&key=GetCustomerProducts",
            //    type: "GET",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (res) {
            //        if (res.Success) {

            //            AppData.CustomerDiscount = res.Data;

            //            //var html = "";
            //            //var products = res.Data;
            //            //html += "<option></option>";
            //            //for (var i = 0; i < products.length; i++) {
            //            //    var product = products[i];
            //            //    html += "<option data-detail='" + JSON.stringify(product) + "' value='" + product.COAProductId + "'>" + product.ProductCode + "-" + product.ProductTitle + "</option>";
            //            //}
            //            //$("#Item").html(html).select2();

            //        } else {
            //            Common.ShowError(res.Error);
            //        }

            //    },
            //    error: function (e) {
            //    }
            //});
        },
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
            //Common.WrapAjax({
            //    url: Setting.APIBaseUrl + "COA/?&key=LoadAccountDetail",
            //    type: "GET",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (res) {
            //        if (res.Success) {

            //            AppData.AccountDetail = res.Data;

            //            //var html = "";
            //            //var products = res.Data;
            //            //html += "<option></option>";
            //            //for (var i = 0; i < products.length; i++) {
            //            //    var product = products[i];
            //            //    html += "<option data-detail='" + JSON.stringify(product) + "' value='" + product.COAProductId + "'>" + product.ProductCode + "-" + product.ProductTitle + "</option>";
            //            //}
            //            //$("#Item").html(html).select2();

            //        } else {
            //            Common.ShowError(res.Error);
            //        }

            //    },
            //    error: function (e) {
            //    }
            //});
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
            //var d = "type=" + Common.GetQueryStringValue("type").toLowerCase();
            //Common.WrapAjax({
            //    url: "../Transaction/GetNextVoucherNumber",
            //    type: "POST",
            //    data: d,
            //    success: function (res) {
            //        var q = JSON.parse(res);
            //        if (q.Success) {
            //            $("#VoucherNumber,#InvoiceNumber").val(q.Data);
            //            $("#lblVoucherNumber").html(q.Data);
            //        } else {
            //            Common.ShowError(q.Data);
            //        }
            //    },
            //    error: function (e) {
            //    }
            //});
        },
        DeleteMultiple: function (id) {
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.Success) {
                            RebindData();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },

        Delete: function (fromDate, toDate) {
            $this = this;
            var qs = "?key=key";
            qs += "&fromdate=" + fromDate;
            qs += "&todate=" + toDate;

            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/1" + qs,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    //blockElement: "#form-info",
                    blockMessage: "Delete Appointment...please wait",
                    success: function (res) {

                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            //Common.GetNextAccountCode(API_CONTROLLER);
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

        Edit: function (fromDate, toDate) {
            $this = this;
            recordtype = "Edit";
            var html = "";
            var qs = "?key=Edit";
            qs += "&fromdate=" + fromDate;
            qs += "&todate=" + toDate;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data.Records;
                        var html = "";
                        var name = "";
                        var amount = 0;
                        var employeeIncomeConfigs = res.Data.EmployeeIncome;
                        var employee = res.Data.Employee;
                        var leavebankend = "";
                        var todate = "";
                        var fromdate = "";
                        $("#item-container tbody").html("");
                        for (var i in employeeIncomeConfigs) {
                            var employeeIncome = employeeIncomeConfigs[i];
                            name = employeeIncome.Name;
                            leavebankend = employeeIncome.LeaveBankEnd;
                            todate = employeeIncome.ToDate;
                            fromdate = employeeIncome.FromDate;
                            //var childaccounts = $.grep(employee, function (e) { return e.Id == employeeIncome.AccountId });   
                            var empInfo = Enumerable.From(employee).Where(function (x) { return x.Id == employeeIncome.AccountId }).FirstOrDefault();
                            employeeIncome["AccountCode"] = empInfo.AccountCode;
                            var templateHtml = $("#template-item").html();
                            var placeholder = $this.GetPlaceHolder(employeeIncome);
                            templateHtml = templateHtml.allReplace(placeholder);
                            html = templateHtml;
                            $("#item-container tbody").append(html);
                            for (var key in employeeIncome) {
                                var element = $("#item-container tbody tr:last").find("." + key);
                                $(element).val(employeeIncome[key]);
                            }
                        }
                        $("#Name").val(name);
                        $("#ToDate").val(moment(todate).format('DD/MM/YYYY'));
                        $("#LeaveBankEnd").val(moment(leavebankend).format('DD/MM/YYYY'));
                        $("#Name").prop("disabled", true);
                        $("#FromMonth").prop("disabled", true);
                        $("#FromYear").prop("disabled", true);
                        $("#ToMonth").prop("disabled", true);
                        $("#ToYear").prop("disabled", true);
                        $("#SelectedLeaveMonth").prop("disabled", true);

                        var check = moment(fromdate, 'YYYY/MM/DD');
                        var month = check.format('M');
                        var day = check.format('D');
                        var year = check.format('YYYY');

                        $("#FromMonth").val(month);
                        $("#FromYear").val(year);

                        $this.DetailView();
                        $this.LoadPageSetting();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        ShowAttachments: function (el) {
            // MEDIA_ELEMENT = el;
            $("#dialogAttachments").addClass("in");
        },

        LoadAccounts: function () {
            var $this = this;
            var id = 0;
            switch ($this.GetType().toLowerCase()) {
                case "sale":
                    id = PageSetting.Customers;
                    break;
                case "salereturn":
                    id = PageSetting.Customers;
                    break;
                case "purchase":
                    id = PageSetting.Suppliers;

                    break;
                case "purchasereturn":
                    id = PageSetting.Suppliers;
                    break;
            }
            var tokens = Common.GetLeafAccounts(id);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.AccountId,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var d = $this.GetByCode(ui.item.value);
                    var type = $this.GetType();
                    if (typeof d != "undefined" && d != null) {
                        if (type == "sale") {
                            $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "salereturn") {
                            $("#Comments").val("Sale Return From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchase") {
                            $("#Comments").val("Purchase From: " + d.Code + "-" + d.Name);
                        }
                        else if (type == "purchasereturn") {
                            $("#Comments").val("Purchase Return To: " + d.Code + "-" + d.Name);
                        }
                        $("#AccountName").val(d.Name);

                        $("#AccountId").val(d.AccountId);
                        var address = d.Address;

                        if (typeof address != "undefined" && address != "null")
                            $("#PartyAddress").val(address);
                        $(".container-message").hide();
                    }
                    $this.MapComments();
                }
            });
        },

        LoadPageSetting: function () {
            var voucher = this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            //  $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function () {
            window.print();
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var products = Common.GetLeafAccounts(PageSetting.Products);
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.AccountId,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName
                    }
                );
            }
            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var product = $this.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof product != "undefined" && product != null) {
                        var discount = $this.GetDiscountDetail(product.AccountId);
                        $(tr).find(":nth-child(1) input.AccountId").val(product.AccountId);
                        $(tr).find(":nth-child(2) input.Name").val(product.Name);
                        $(".container-message").hide();
                    }
                    //$(tr).find("input.Quantity").focus();
                    //alert("event:" + JSON.stringify(event) + ", ui:" + JSON.stringify(ui));
                }
            });
        },
        TransporterAutoCompleteInit: function (data) {
            var $this = this;
            var tokens = data;
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.Code,
                        label: token.Code + "-" + token.Name,
                        Name: token.Name
                    }
                );
            }

            $("#ShipViaCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    $("#ShipViaName").val(ui.item.Name);
                    $("#ShipViaId").val(ui.item.id);
                    $(".btn.btn-primary.green").focus();
                }
            });
        },
    };
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};