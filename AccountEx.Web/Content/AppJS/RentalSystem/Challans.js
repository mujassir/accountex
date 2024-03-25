
var Challans = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Challan";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    var AGREEMENTS = new Array();
    return {
        init: function () {
            var $this = this;
            $("#TenantCode").focus();
            $("#div-table").addClass("hide");
            $(document).on("blur", "#TenantCode", function () {
                var account = Common.GetByCode($(this).val());
                if (typeof account != "undefined" && account != null) {
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }
            });
            $("#NumberOfInstallment").keyup(function () {
                $this.InstallmentCalculation();
            });
            $("#NetAmount").keyup(function () {
                $this.CalculateSummary();
            });
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    //$this.LoadVoucher("same");
                }
            });
            $("#RentAgreementId").change(function () {
                $this.GetRentAgreementInfo();
            });
            Common.Clear();
            $this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
            var dueDate = PageSetting.DueDate;
            Common.SetDate("#DueDate", dueDate, true);
            $this.LoadData();
        },

        InstallmentCalculation: function () {
            var $this = this;
            var noOfMonths = $("#NumberOfInstallment").val();
            var amountPerInstallment = $("#AmountPerInstallment").val();
            var amount = noOfMonths * amountPerInstallment;
            $("#NetAmount").val(Common.GetCeilInt(amount));
            $this.CalculateSummary();
        },
        CalculateSummary: function () {
           
            var amount =Common.GetInt( $("#NetAmount").val());
            $("#tbl-sumamry span.InstallmentAmount").text(amount.format());

            var balance = Common.GetIntHtml($("#tbl-sumamry span.BalanceAmount").text());
            var installmentBalance = balance - amount;
            // var installmentBalanceHtml = "<label class='label label-info col-md-6'>" + installmentBalance.toFixed("2") + "</label>";
            $("#tbl-sumamry span.InstallmentBalance").text(installmentBalance.format());


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
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        New: function () {

            var $this = this;
            $this.CustomClear();
            //$this.LoadVoucher("nextvouchernumber");
        },
        Print: function (id) {
            var $this = this
            //var id = Challan.Id;
            //id = 13876;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?Id=" + id + "&printChallan=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "printing bill ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data.Order;
                        var challan = res.Data.Challan;
                        Common.MapDataWithPrefixFClasses(challan, ".div-report", "lbl", "html");
                        var dueDate = moment(challan.DueDate).format("MMMM DD, YYYY");
                        var monthString = moment(challan.DueDate).format("MMMM, YYYY");
                        var challanItem = Enumerable.From(d.ChallanItems).FirstOrDefault();
                        res.Data.PaidAmount = challan.TotalReceived + res.Data.PaidAmount;
                        var agreement = res.Data.RentAgreement;
                        $(".lblHeld").html(Common.GetCeilInt(res.Data.PaidAmount));
                        $(".lblTotal").html(Common.GetCeilInt(res.Data.PaidAmount));
                      


                        var balance = Common.GetCeilInt(challan.TotalAmount - res.Data.PaidAmount);
                        $(".lblBalance").html(balance);
                        $(".lblArrearsTotal").html(Common.GetCeilInt(balance));
                        $(".CurrentInstallment").html("Current Installment ");
                        var previousInstallments = res.Data.PreviousInstallments;
                        $(".lblTotalInstallment").html(previousInstallments + "/" + challan.TotalInstallment);
                        var numberOfInstallment = challan.NumberOfInstallment;
                        var totalInstallments = previousInstallments + numberOfInstallment;
                        var remainingInstallments = 0;
                        if ((previousInstallments + 1) != totalInstallments)
                            $(".currinslbl").html((previousInstallments + 1) + " - " + totalInstallments);
                        else
                            $(".currinslbl").html((previousInstallments + 1));

                        $(".lblInstalPayable").html(Common.GetCeilInt(challan.NetAmount));
                        remainingInstallments = challan.TotalInstallment - totalInstallments;
                        $(".Arears").html("Remaining Amount " + remainingInstallments + "/" + challan.TotalInstallment);
                        if (VoucherType[$this.GetType()] == VoucherType.possessioncharges) {
                            $(".Charges").html("Possession Charges");
                        }
                        else if (VoucherType[$this.GetType()] == VoucherType.securitymoney) {
                            $(".Charges").html("Security Charges");
                        }
                        var options = {
                            width: 2.9,
                            height: 30,
                            //format: "CODE39",
                            displayValue: true
                        }
                        $(".barcode").JsBarcode(challan.Id.toString(), options);
                        $(".lblDueDate").html(dueDate);
                        $(".lblMonth").html(monthString);
                        setTimeout(function () { window.print() }, 1500);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
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
            $this = this;
            $("#item-container tbody").html("");
            $("#PaidIns").addClass("hide");
            $this.AmountLabels();
            Common.Clear();
            $("#YearlyInc").val(10);
            $("#RentPerSqft").val(PageSetting.RentPerSqft);
            $("#btndelete,#btnprint").prop("disabled", true);
        },

        GetNextVoucherNumber: function () {
            var $this = this;
            //$this.LoadVoucher("nextvouchernumber");
        },

        Save: function () {
            var $this = this;
            var data = new Array();
            var type = Common.GetQueryStringValue("type").toLowerCase();
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var challanItems = new Array();
                var record = Common.SetValue("#form-info");
                record["TransactionType"] = VoucherType[$this.GetType()];
                var agreement = Enumerable.From(AGREEMENTS).FirstOrDefault(null, function (p) { return p.Id == record.RentAgreementId });
                record = $.extend(true, record, agreement);
                delete record.Id;
                var item = $.extend(true, item, record);
                item.ChallanId = record.Id;
                item.TenantAccountName = item.TenantName;
                item.Amount = record.NetAmount;
                challanItems.push(item);
                record["ChallanItems"] = challanItems;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Challan ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Challan Saved Successfully!" });
                            $this.GetRentAgreementInfo();
                            $this.Print(res.Data.Id);

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
                blockMessage: "Loading salesman ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.MapEditData(j, $("#form-info"));
                        $(".date-picker").each(function () {
                            Common.SetDate(this, $(this).val());
                        });
                        $("#TempCode").val(res.Data.Code);
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
        Delete: function (element, id) {
            var $this = this;
            var tr = $(element).closest("tr");
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $(tr).remove();
                            $this.GetRentAgreementInfo();
                            Common.ShowMessage(true, { message: "Challan deleted successfully!" });
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
        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

        },

        LoadData: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "RentAgreement?key=GetRentAgreementsWithTenants",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        AGREEMENTS = data;
                        var html = "<option></option>";
                        for (var i in data) {
                            var record = data[i];
                            html += "<option data-shopid='" + record.ShopId + "' data-tenantid='" + record.TenantAccountId + "' value='" + record.Id + "'>" + record.TenantCode + "-" + record.TenantName + " (" + (record.Block != null ? "Block: " + record.Block + " ," : '') + " Shop No: " + record.ShopNo + ")" + "</option>";
                        }
                        $("#RentAgreementId").html(html);
                        $("#RentAgreementId").select2();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },

        AutoCompleteInit: function (data) {
            var $this = this;
            //var tenants = Common.GetLeafAccounts(PageSetting.Tenants);
            var suggestions = new Array();
            for (var i in data) {
                var rentAgreementWithTenant = data[i];
                suggestions.push
                    (
                    {
                        id: rentAgreementWithTenant.Id,
                        value: rentAgreementWithTenant.TenantCode,
                        label: rentAgreementWithTenant.TenantCode + "-" + rentAgreementWithTenant.TenantName + " (" + (rentAgreementWithTenant.Block != null ? "Block: " + rentAgreementWithTenant.Block + " ," : '') + " Shop No: " + rentAgreementWithTenant.ShopNo + ")",
                    }
                    );
            }

            $("#TenantCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        var tenant = Common.GetAccountDetailByAccountId(account.Id);
                        $("#RentAgreementId").val(account.Id);
                        $("#TenantAccountName").val(tenant.Name);
                        $("#Business").val(tenant.BrandName);
                        $("#Brand").val(tenant.Brand);
                        $this.GetRentAgreementInfo(ui.item.id);
                    }
                }
            });
        },
        GetRentAgreementInfo: function () {
            var id = Common.GetInt($("#RentAgreementId").val())
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?rentAgreementId=" + id + "&type=" + VoucherType[$this.GetType()] + "&key=GetRentAgreementInfo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var agreement = res.Data.Agreement;
                        var shopNo = res.Data.ShopInfo;
                        $("#PaidIns").removeClass("hide");
                        $("#NoOfPaidIstallment").text(res.Data.PaidIstallment);
                        $("#NoOfUnPaidIstallment").text(res.Data.TotalIstallment - res.Data.PaidIstallment);
                        $("#PaidAmount").val(res.Data.PaidAmount);
                        var balance = 0.0;
                        $("#AmountPerInstallment").val(res.Data.AmountPerInstallment);
                        $("#tbl-sumamry span.TotalAmount").text(res.Data.TotalAmount.format());
                        balance = res.Data.TotalAmount - res.Data.PaidAmount;

                        $("#tbl-sumamry span.PaidAmount").text(res.Data.PaidAmount.format());
                        $("#tbl-sumamry span.BalanceAmount").text(balance.format());

                        var challans = res.Data.AllChallan;
                        var html = "";
                        for (var i in challans) {
                            var challan = challans[i];
                            html += "<tr>"
                            html += "<td>" + challan.Id + "</td>";
                            html += "<td>" + moment(challan.DueDate).format("MMMM DD, YYYY") + "</td>";
                            html += "<td>" + Common.GetCeilInt(challan.TotalAmount).format() + "</td>";
                            html += "<td>" + (challan.IsReceived ? "Paid" : "Pending") + "</td>";
                            html += "<td>" + challan.RcvNo + "</td>";
                            html += "<td><button type='button' class='btn yellow btn-xs' onclick='Challans.Print(" + challan.Id + ")'><i class='fa fa-print'></i>&nbsp;Print</button>";
                            if (!challan.IsReceived)
                                html += "<button type='button' class='btn red btn-xs' onclick='Challans.Delete(this," + challan.Id + ")'><i class='fa fa-trash-o'></i>&nbsp;Delete</button>";
                            html += "</td>";

                            html += "</tr>"
                        }
                        $("#mainTable tbody").html(html);
                        $("#div-table").removeClass("hide");
                        $this.InstallmentCalculation();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        }
    };
}();
