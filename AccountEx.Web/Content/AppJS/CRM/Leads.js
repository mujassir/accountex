
var Leads = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Leads";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var focusElement = "#Customer";
    return {
        init: function () {
            var $this = this;
            //$this.LoadLeadNo();
            //$this.AddExpectedItems();
            //$this.AddQuotation();
            $this.ListView();
            $this.LoadPageSetting();
            $this.LoadSalesman();
            /////Starts/////
            $(document).on("keyup", "input.Rate", function () {
                var tr = $(this).closest("tr");
                var Quantity = Common.GetFloat($(tr).find("input.Quantity").val());
                var Rate = $(tr).find("input.Rate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.Amount").val(Amount.toFixed(2));

            });
            $(document).on("keyup", ".Quantity, .Rate ", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.Quantity").val());
                var rate = $(tr).find("input.Rate").val();
                var amount = qty * rate;
                $(tr).find("input.Amount").val(amount.toFixed(2));

                var code = $(tr).find("input.Code").val();
                //$this.GetQuantityTotal();
                if ($(tr).is(':last-child') && event.keyCode == 13 && amount != 0) {
                    $this.AddExpectedItems();
                }
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Quantity").focus();

                }
                $this.GetWholeTotal();

            });
            $(document).on("keyup", "input.QtRate", function () {
                var tr = $(this).closest("tr");
                var Quantity = Common.GetFloat($(tr).find("input.QtQuantity").val());
                var Rate = $(tr).find("input.QtRate").val();
                var Amount = Quantity * Rate;
                $(tr).find("input.QtAmount").val(Amount.toFixed(2));

            });
            $(document).on("keyup", ".QtQuantity, .QtRate,.DiscountPercent ", function (event) {

                var tr = $(this).closest("tr");
                var qty = Common.GetFloat($(tr).find("input.QtQuantity").val());
                var rate = $(tr).find("input.QtRate").val();
                $this.GetQuantityPriceTotal(tr);
                var amount = qty * rate;
                $(tr).find("input.QtAmount").val(amount.toFixed(2));

                var code = $(tr).find("input.QtCode").val();
                //$this.GetQuantityTotal();
                if ($(tr).is(':last-child') && event.keyCode == 13 && amount != 0) {
                    $this.AddQuotation();
                }
                else if (event.which == 13 && qty <= 0) {
                    var err = "Item " + code + " must have quantity greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.QtQuantity").focus();

                }
                $this.GetQtWholeTotal();

            });
            //$(document).on("keyup", ".QtCode", function (event) {

            //    var tr = $(this).closest("tr");
            //    var name = $(tr).find("input.QtName").val();
            //    if ($(tr).is(':last-child') && event.keyCode == 13 && name != "") {
            //        $this.AddQuotation();
            //    }
            //});
            /////Ends/////
        },
        LoadSalesman: function () {
            var salesmans = Common.GetLeafAccounts(PageSetting.Salesman);
            var html = "<option></option>";
            for (var i in salesmans) {
                var saleman = salesmans[i];
                html += "<option data-custom='" + saleman.Id + "' value='" + saleman.Name + "'>" + saleman.Name + "</option>";
            }
            $("#LeadOwner").html(html);
        },
        Add: function () {
            var $this = this;
            $("#tbl-Quotation tbody").html("");
            $("#tbl-expecteditems tbody").html("");
            $this.LoadLeadNo();
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
            $("#btnprint").prop("disabled", true);
            //$("#btnprint").addClass("hide");
            // Common.GetNextAccountCode(API_CONTROLLER);
        },
        DetailView: function () {
            var $this = this;
            $('#form-info').removeClass('hide');
            $('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();

            $this.AddExpectedItems();
            $this.AddQuotation();
        },
        ListView: function () {
            var $this = this;
            $('#form-info').addClass('hide');
            $('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $this.CustomClear();
        },
        Cancell: function () {

            var $this = this;
            focusElement = "#Customer";
            $this.CustomClear();
            Common.GetNextAccountCode(API_CONTROLLER);

        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
            //SetDropDown();
        },
        CustomClear: function () {
            //$("#tbl-Quotation").html("");
            //$("#tbl-expecteditems").html("");
            Common.Clear();
            //$("#btnprint").prop("disabled", true);
        },
        Print: function () {

            window.print();
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data;

            if (d == null)
                return;
            $("#report-saleitem tfoot tr ").find("td:nth-child(1)").text("Total ");

            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");

            $("#lblDate").html((d.Date ? moment(d.Date).format("dddd, DD-MM-YYYY") : ''));

            var html = "";
            var items = d.LeadConcernedItems;
            var index = 1;

            var nettotalamount = 0.0;

            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                nettotalamount += item.NetAmount;
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemCode + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td class='align-right'>" + item.Quantity + "</td>";
                html += "<td class='align-right'>" + item.Rate.format() + "</td>";
                html += "<td class='align-right'>" + item.NetAmount.format() + "</td>";
                //html += "<td>" + item.DiscountAmount.format() + "</td>";
                //html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";



            }

            $("#lblTotalAmount").html(nettotalamount.format());
            $("#lblVoucherNumber").html(d.LeadNo);
            $("#lblDate").html($("#Date").val());
            $("#lblAccountCode").html(d.LeadOwner);
            $("#lblAccountName").html(d.Customer);
            //$("#lblPartyAddress").html(d.Street?"null":" " + d.City?"null":" " + d.Country?"null":" ");
            var address = "";
            if (d.Street != null)
                address += d.Street + ", ";;
            if (d.City != null)
                address += d.City + ", ";
            if (d.Country != null)
                address += d.Country + ", ";
            if (d.State != null)
                address += d.State;
            $("#lblPartyAddress").html(address);
            if (d.Tel1 != null || d.Tel1 != 0) {
                $("#lblContactPerson").html(d.Tel1);
            }
            else { $("#lblContactPerson").html(d.Tel2); }
            $("#lblEmail").html(d.Email);

            var instruction = $("#Instructions").val().replace(/\r?\n/g, '<br />');
            $("#lblInstructions").html(instruction);

            $("#report-saleitem tbody").append(html);
            //var nettotal = $("#QuantityTotal").val();
            //var nettotal = $("#tbl-Quotation tfoot tr").find(":nth-child(2) input.QtQuantity").val(Quantity.toFixed(2));

        },

        Save: function () {
            var $this = this;
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($(".form"));
                ////starts////
                //var Salesman = $("#LeadOwner").val();
                //var Name = Salesman.split("-");
                //record["LeadOwner"] = Name[1];

                record["LeadOwnerId"] = $("#LeadOwnerId").val();
                record["Date"] = $("#Date").val();
                record["Instructions"] = $("#Instructions").val();
                ConcernedItems = Common.SaveItemData("#tbl-Quotation");
                ConcernedItems = Enumerable.From(ConcernedItems).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                for (var i in ConcernedItems) {
                    var item = ConcernedItems[i];
                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err = item.ItemCode + " is not valid code.";
                    }
                }
                if (ConcernedItems.length <= 0) {
                    err += "Please add atleast one item.";
                }
                record["LeadConcernedItems"] = ConcernedItems;

                ////////////////////////////////////////////////////////////////////////////////////////////////

                ExpectedItems = Common.SaveItemData("#tbl-expecteditems");
                ExpectedItems = Enumerable.From(ExpectedItems).Where("$.ItemCode.trim()!=''").ToArray();
                var err = "";
                for (var i in ExpectedItems) {
                    var item = ExpectedItems[i];
                    var product = Common.GetByCode(item.ItemCode);
                    if (typeof product == "undefined" || product == null) {
                        err = item.ItemCode + " is not valid code.";
                    }
                }
                if (ExpectedItems.length <= 0) {
                    err += "Please add atleast one item.";
                }
                record["LeadExpectedItems"] = ExpectedItems;


                ////ends////

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving leads information ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            $this.ListView();
                            $this.LoadLeadNo();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            //Common.GetNextAccountCode(API_CONTROLLER);
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
        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "Loading leads information ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#tbl-Quotation tbody").html("");
                        $("#tbl-expecteditems tbody").html("");
                        var j = res.Data;
                        $("#lblNextLeadNo").html(j.LeadNo);
                        Common.MapEditData(j, $("#form-info"));
                        Common.SetDate($("#Date"), j.Date);
                        $("#TempCode").val(res.Data.Code);
                        //////////starts///////////
                        if (j.Id > 0 && j.LeadExpectedItems != null && j.LeadExpectedItems.length > 0) {
                            Common.MapItemData(j.LeadExpectedItems, "#tbl-expecteditems", "#product-template-item-ExpectedSaleVolume");
                            $this.GetWholeTotal();
                        }
                        if (j.Id > 0 && j.LeadConcernedItems != null && j.LeadConcernedItems.length > 0) {
                            Common.MapItemData(j.LeadConcernedItems, "#tbl-Quotation");
                            $this.GetQtWholeTotal();
                        }
                        /////////ends/////////////
                        $this.DetailView();
                        $("#btnprint").prop("disabled", false);
                        $this.LoadReportData(res);

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
                    blockMessage: "Deleting leads information ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.LoadLeadNo();
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
        /////starts//////
        LoadLeadNo: function () {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=lead",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: " Loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#lblNextLeadNo").html(res.Data.LeadNo);
                        $("#LeadNo").val(res.Data.LeadNo);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        AutoCompleteInit: function (scope) {
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
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        //var product = Common.GetAccountDetailByAccountId(account.Id);
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.ItemName").val(account.Name);
                        //$(tr).find(":nth-child(4) input.Rate").val(Common.GetFloat(product.SalePrice));
                        $(".container-message").hide();
                    }
                }
            });

        },
        QtAutoCompleteInit: function (scope) {
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

            $(".QtCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof account != "undefined" && account != null) {
                        $(tr).find(":nth-child(1) input.ItemId").val(account.Id);
                        $(tr).find(":nth-child(2) input.QtName").val(account.Name);
                        $(".container-message").hide();
                    }
                }
            });

        },
        GetQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find("input.QtQuantity").val());
            Rate = Common.GetFloat($(tr).find("input.QtRate").val());
            var amount = Quantity * Rate;
            $(tr).find("input.QtAmount").val(amount.toFixed(2));
            var discount = Common.GetFloat($(tr).find("input.DiscountPercent").val());
            var discountAmount = Common.GetFloat(amount) * discount / 100;
            var netAmount = Common.GetFloat(amount - discountAmount);
            $(tr).find("input.DiscountAmount ").val(discountAmount.toFixed(2));
            $(tr).find("input.NetAmount").val(netAmount);

            $this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            $("#tbl-expecteditems tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find(":nth-child(3) input.Quantity").val());
                Price += Common.GetFloat($(this).find(":nth-child(5) input.Amount").val());
            });
            $("#tbl-expecteditems tfoot tr").find(":nth-child(2) input.Quantity").val(Quantity.toFixed(2));
            $("#tbl-expecteditems tfoot tr").find(":nth-child(3) input.TotalAmount").val(Price.toFixed(2));
            $("#ExpectedSaleVolume").val(Price.toFixed(2));

        },
        GetQtQuantityPriceTotal: function (tr) {
            $this = this;
            var Quantity = 0.0;
            var Rate = 0.0;
            Quantity = Common.GetFloat($(tr).find(":nth-child(3) input.QtQuantity").val());
            Rate = Common.GetFloat($(tr).find(":nth-child(4) input.QtRate").val());
            var amount = Quantity * Rate;
            $(tr).find(":nth-child(5) input.QtAmount").val(amount.toFixed(2));
            $this.GetQtWholeTotal();
        },
        GetQtWholeTotal: function () {
            var $this = this;
            var Quantity = 0.0;
            var Price = 0.0;
            var DiscountAmount = 0.0;
            var NetAmount = 0.0;
            $("#tbl-Quotation tbody tr").each(function () {
                Quantity += Common.GetFloat($(this).find(":nth-child(3) input.QtQuantity").val());
                Price += Common.GetFloat($(this).find(":nth-child(5) input.QtAmount").val());
                DiscountAmount += Common.GetFloat($(this).find(":nth-child(7) input.DiscountAmount").val());
                NetAmount += Common.GetFloat($(this).find(":nth-child(8) input.NetAmount").val());
            });
            $("#tbl-Quotation tfoot tr").find(":nth-child(2) input.QtQuantity").val(Quantity.toFixed(2));
            $("#tbl-Quotation tfoot tr").find(":nth-child(3) input.QtTotalAmount").val(Price.toFixed(2));
            $("#tbl-Quotation tfoot tr").find(":nth-child(4) input.QtTotalDisAmount").val(DiscountAmount.toFixed(2));
            $("#tbl-Quotation tfoot tr").find(":nth-child(5) input.QtTotalNetAmount").val(NetAmount.toFixed(2));

        },
        GetLeadId: function () {
            return Common.GetQueryStringValue("leadid").toLowerCase();
        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            $this.GetQtWholeTotal();


            if ($("#tbl-expecteditems tbody").children().length <= 0)
                $this.AddExpectedItems();
            if ($("#tbl-Quotation tbody").children().length <= 0)
                $this.AddQuotation();
        },
        AddExpectedItems: function () {

            var $this = this;

            var code = $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-expecteditems tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);


            }
            var html = $("#product-template-item-ExpectedSaleVolume").html();
            $("#tbl-expecteditems tbody").append(html);

            $this.AutoCompleteInit();
            Common.InitNumerics();
        },
        AddQuotation: function () {

            var $this = this;
            var code = $("#tbl-Quotation tbody tr:nth-last-child(1) td:nth-child(1) input.QtCode").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#tbl-Quotation tbody tr:nth-last-child(1) td:nth-child(1) input.QtCode").focus().select();
                }, 300);

                SetFocus = "code";
                return;
            }


            var html = $("#template-item").html();
            $("#tbl-Quotation tbody").append(html);

            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#tbl-Quotation tbody tr:nth-last-child(1) td:nth-child(1) input.QtCode").focus().select();
                }, 300);
                focusElement = "";
            }



            $this.QtAutoCompleteInit();
            Common.AllowNumerics();
            Common.InitNumerics();
        },
        MapExpectedItems: function (data) {
            var $this = this;
            Common.MapItemData(data.LeadExpectedItems, "#tbl-expecteditems", "#product-template-item-ExpectedSaleVolume");
            $this.GetQuantityPriceTotal();
        },
        MapConcernedItems: function (data) {
            var $this = this;
            Common.MapItemData(data.LeadConcernedItems, "#tbl-Quotation", "#template-item");
            $this.GetQuantityPriceTotal();
        },

        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },


        ///////ends///////

    };
}();
