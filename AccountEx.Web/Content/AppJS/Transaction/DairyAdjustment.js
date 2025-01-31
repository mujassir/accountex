
var Adjustment = function () {
    var max = 0;
    var FORM_TYPE = "";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "DairyAdjustment";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function (type = "") {
            FORM_TYPE = type;
            var $this = this;
            Common.BindShortKeys($this);
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "#InvoiceNumber" : "#Date";
                    $this.LoadVoucher("same");
                    //setTimeout(function () {
                    //    $('#InvoiceNumber').focus();
                    //}, 1000);
                    //$('#InvoiceNumber').focus();
                }
            });
            $("#Date").keyup(function (e) {
                $("#AccountCode").focus();
            });
            $("#AccountCode").keypress(function (e) {
                if (e.which == 13) {
                    var party = Common.GetByCode($(this).val());
                    if (typeof party != "undefined" && party != null) {
                        $this.GetPreviousBalance(party.Id);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                        }, 500);
                    }

                    else {
                        if ($(this).val().trim() != "") {
                            var err = +$(this).val() + " is not valid adjusted to code.,";
                            $(this).focus();
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("blur", ".Code", function () {
                if (!PageSetting.BarCodeEnabled) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode($(this).val());
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = 0;
                        $(tr).find("input.ItemId").val(product.AccountId);
                        $(tr).find("input.Name").val(account.Name);
                        $(tr).find("input.Rate").val(Common.GetFloat(product.SalePrice));
                        $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                        $(".container-message").hide();
                    }
                    else {
                        if ($(this).val().trim() != "") {
                            var err = $(this).val() + " is not valid code.,";
                            Common.ShowError(err);
                        }
                    }
                }
            });
            $(document).on("change", "#ItemStatus,#GroupId", function () {
                const status = $("#ItemStatus").val();
                const groupId = $("#GroupId").val();
                const products = PageSetting.AccountDetails.filter(x => x.GroupId == groupId && x.UnitType == status);
                $("#item-container tbody").html("")
                const items = products.map(e => {
                    return {
                        "ItemId": e.AccountId,
                        "ItemCode": e.Code,
                        "ItemName": e.Name,
                        "Comment": null,
                        "DebitItem1": 1,
                        "DebitItem2": 1,
                        "DebitItem3": 1,
                        "DebitItem4": 1,
                        "DebitItem5": 1,
                        "CreditItem1": 1,
                        "CreditItem2": 1,
                        "CreditItem3": 1,
                        "CreditItem4": 1,
                        "CreditItem5": 1,
                        "CreditItem6": 1,
                        "CreditItem7": 1,
                        "CreditItem8": 1,
                        "CreditItem9": 1,
                        "CreditItem10": 1,
                        "CreditItem11": 1,
                        "CreditItem12": 1,
                        "CreditItem13": 1,
                        "CreditItem14": 1,
                        "CreditItem15": 1,
                        "CreditItem16": 1,
                        "CreditItem17": 1,
                        "CreditItem18": 1,
                        "CreditItem19": 1,
                        "CreditItem20": 1,
                        "CreditItem21": 1,
                        "CreditItem22": 1,
                        "CreditItem23": 1,
                        "CreditItem24": 1,
                        "CreditItem25": 1,
                    }
                })
                Common.MapItemData(items);
                $this.CalculateCalculations();
                $this.UpdateHeaderLabel();

            });
            $(document).on("blur", "#TotalDebitItem1, #TotalDebitItem2, #TotalDebitItem3, #TotalDebitItem4, #TotalDebitItem5, #TotalCreditItem1, #TotalCreditItem2, #TotalCreditItem3, #TotalCreditItem4, #TotalCreditItem5, #TotalCreditItem6, #TotalCreditItem7, #TotalCreditItem8, #TotalCreditItem9, #TotalCreditItem10, #TotalCreditItem11, #TotalCreditItem12, #TotalCreditItem13, #TotalCreditItem14, #TotalCreditItem15, #TotalCreditItem16, #TotalCreditItem17, #TotalCreditItem18, #TotalCreditItem19, #TotalCreditItem20, #TotalCreditItem21, #TotalCreditItem22, #TotalCreditItem23, #TotalCreditItem24, #TotalCreditItem25", function () {
                const rows = $("#item-container tbody").find("tr");
                const validRows = [];

                // Collect rows with non-empty `.Code` input values
                rows.each(function () {
                    const codeInput = $(this).find("td input.Code").val()?.trim();
                    if (codeInput) {
                        validRows.push(this);
                    }
                });

                // Avoid division by zero
                const rowCount = validRows.length || 1;

                // Calculate distributed counts
                const debitItem1Count = ($("#TotalDebitItem1").val()?.trim() || 0) / rowCount;
                const debitItem2Count = ($("#TotalDebitItem2").val()?.trim() || 0) / rowCount;
                const debitItem3Count = ($("#TotalDebitItem3").val()?.trim() || 0) / rowCount;
                const debitItem4Count = ($("#TotalDebitItem4").val()?.trim() || 0) / rowCount;
                const debitItem5Count = ($("#TotalDebitItem5").val()?.trim() || 0) / rowCount;
                const creditItem1Count = ($("#TotalCreditItem1").val()?.trim() || 0) / rowCount;
                const creditItem2Count = ($("#TotalCreditItem2").val()?.trim() || 0) / rowCount;
                const creditItem3Count = ($("#TotalCreditItem3").val()?.trim() || 0) / rowCount;
                const creditItem4Count = ($("#TotalCreditItem4").val()?.trim() || 0) / rowCount;
                const creditItem5Count = ($("#TotalCreditItem5").val()?.trim() || 0) / rowCount;
                const creditItem6Count = ($("#TotalCreditItem6").val()?.trim() || 0) / rowCount;
                const creditItem7Count = ($("#TotalCreditItem7").val()?.trim() || 0) / rowCount;
                const creditItem8Count = ($("#TotalCreditItem8").val()?.trim() || 0) / rowCount;
                const creditItem9Count = ($("#TotalCreditItem9").val()?.trim() || 0) / rowCount;
                const creditItem10Count = ($("#TotalCreditItem10").val()?.trim() || 0) / rowCount;
                const creditItem11Count = ($("#TotalCreditItem11").val()?.trim() || 0) / rowCount;
                const creditItem12Count = ($("#TotalCreditItem12").val()?.trim() || 0) / rowCount;
                const creditItem13Count = ($("#TotalCreditItem13").val()?.trim() || 0) / rowCount;
                const creditItem14Count = ($("#TotalCreditItem14").val()?.trim() || 0) / rowCount;
                const creditItem15Count = ($("#TotalCreditItem15").val()?.trim() || 0) / rowCount;
                const creditItem16Count = ($("#TotalCreditItem16").val()?.trim() || 0) / rowCount;
                const creditItem17Count = ($("#TotalCreditItem17").val()?.trim() || 0) / rowCount;
                const creditItem18Count = ($("#TotalCreditItem18").val()?.trim() || 0) / rowCount;
                const creditItem19Count = ($("#TotalCreditItem19").val()?.trim() || 0) / rowCount;
                const creditItem20Count = ($("#TotalCreditItem20").val()?.trim() || 0) / rowCount;
                const creditItem21Count = ($("#TotalCreditItem21").val()?.trim() || 0) / rowCount;
                const creditItem22Count = ($("#TotalCreditItem22").val()?.trim() || 0) / rowCount;
                const creditItem23Count = ($("#TotalCreditItem23").val()?.trim() || 0) / rowCount;
                const creditItem24Count = ($("#TotalCreditItem24").val()?.trim() || 0) / rowCount;
                const creditItem25Count = ($("#TotalCreditItem25").val()?.trim() || 0) / rowCount;

                // Update the values in each valid row
                $(validRows).each(function () {
                    $(this).find("td input.DebitItem1").val(debitItem1Count);
                    $(this).find("td input.DebitItem2").val(debitItem2Count);
                    $(this).find("td input.DebitItem3").val(debitItem3Count);
                    $(this).find("td input.DebitItem4").val(debitItem4Count);
                    $(this).find("td input.DebitItem5").val(debitItem5Count);
                    $(this).find("td input.CreditItem1").val(creditItem1Count);
                    $(this).find("td input.CreditItem2").val(creditItem2Count);
                    $(this).find("td input.CreditItem3").val(creditItem3Count);
                    $(this).find("td input.CreditItem4").val(creditItem4Count);
                    $(this).find("td input.CreditItem5").val(creditItem5Count);
                    $(this).find("td input.CreditItem6").val(creditItem6Count);
                    $(this).find("td input.CreditItem7").val(creditItem7Count);
                    $(this).find("td input.CreditItem8").val(creditItem8Count);
                    $(this).find("td input.CreditItem9").val(creditItem9Count);
                    $(this).find("td input.CreditItem10").val(creditItem10Count);
                    $(this).find("td input.CreditItem11").val(creditItem11Count);
                    $(this).find("td input.CreditItem12").val(creditItem12Count);
                    $(this).find("td input.CreditItem13").val(creditItem13Count);
                    $(this).find("td input.CreditItem14").val(creditItem14Count);
                    $(this).find("td input.CreditItem15").val(creditItem15Count);
                    $(this).find("td input.CreditItem16").val(creditItem16Count);
                    $(this).find("td input.CreditItem17").val(creditItem17Count);
                    $(this).find("td input.CreditItem18").val(creditItem18Count);
                    $(this).find("td input.CreditItem19").val(creditItem19Count);
                    $(this).find("td input.CreditItem20").val(creditItem20Count);
                    $(this).find("td input.CreditItem21").val(creditItem21Count);
                    $(this).find("td input.CreditItem22").val(creditItem22Count);
                    $(this).find("td input.CreditItem23").val(creditItem23Count);
                    $(this).find("td input.CreditItem24").val(creditItem24Count);
                    $(this).find("td input.CreditItem25").val(creditItem25Count);
                });
            });

            $(document).on("keyup", ".DebitItem1,.DebitItem2,.DebitItem3,.DebitItem4,.DebitItem5,.CreditItem1,.CreditItem2,.CreditItem2,.CreditItem3, .CreditItem4, .CreditItem5, .CreditItem6, .CreditItem7, .CreditItem8, .CreditItem9, .CreditItem10, .CreditItem11, .CreditItem12, .CreditItem13, .CreditItem14, .CreditItem15, .CreditItem16, .CreditItem17, .CreditItem18, .CreditItem19, .CreditItem20, .CreditItem21, .CreditItem22, .CreditItem23, .CreditItem24, .CreditItem25", function (event) {
                var tr = $(this).closest("tr");
                //var qty = Common.GetInt($(tr).find("input.Milk").val());
                //if (event.which == 13 && qty > 0)
                //    $this.AddItem();
                //else if (event.which == 13 && qty <= 0) {
                //    var err = "Item " + code + " must have quantity greater than zero(0).,";
                //    Common.ShowError(err);
                //}

                $this.CalculateCalculations();
            });
            $(document).on("change", "#GroupId", function (e) {
                const name = $(this).find(':selected').data('custom');
                $("#GroupName").val(name);
            }),
            $("#AccountCode").blur(function () {
                var party = Common.GetByCode($(this).val());
                if (typeof party != "undefined" && party != null) {
                    $("#AccountId").val(party.Id);
                    $(".container-message").hide();
                    var address = party.Address;
                    if (typeof address != "undefined" && address != "null")
                        $("#PartyAddress").val(address);
                }

                else {
                    if ($(this).val().trim() != "") {
                        var err = "" + $(this).val() + " is not valid adjusted to code.,";
                        Common.ShowError(err);
                    }
                }
            });
            $this.LoadPageSetting();
            var url = Setting.APIBaseUrl + API_CONTROLLER;

            AppData.AccountDetail = PageSetting.AccountDetails;
            if (typeof Url.voucherno != undefined && Url.voucherno != null) {
                $("#VoucherNumber").val(Url.voucherno);
                $this.LoadVoucher("same");
            }
            else {
                //if (Setting.PageLandingView == "DetailView") {
                this.Add();
                //} else {
                //    this.ListView();
                //}
            }

        },
        CalculateCalculations: function () {
            const rows = $("#item-container tbody").find("tr");
            let debitItem1 = debitItem2 = debitItem3 = debitItem4 = debitItem5 = 0;
            let creditItem1 = creditItem2 = creditItem3 = creditItem4 = creditItem5 = creditItem6 = creditItem7 = creditItem8 = creditItem9 = creditItem10 = creditItem11 = 0;
            let creditItem12 = creditItem13 = creditItem14 = creditItem15 = creditItem16 = creditItem17 = creditItem18 = creditItem19 = creditItem20 = creditItem21 = creditItem22 = creditItem23 = creditItem24 = creditItem25 = 0;
            rows.each(function () {
                debitItem1 += Number($(this).find("td input.DebitItem1")?.val()?.trim() || '0');
                debitItem2 += Number($(this).find("td input.DebitItem2")?.val()?.trim() || '0');
                debitItem3 += Number($(this).find("td input.DebitItem3")?.val()?.trim() || '0');
                debitItem4 += Number($(this).find("td input.DebitItem4")?.val()?.trim() || '0');
                debitItem5 += Number($(this).find("td input.DebitItem5")?.val()?.trim() || '0');
                creditItem1 += Number($(this).find("td input.CreditItem1")?.val()?.trim() || '0');
                creditItem2 += Number($(this).find("td input.CreditItem2")?.val()?.trim() || '0');
                creditItem3 += Number($(this).find("td input.CreditItem3")?.val()?.trim() || '0');
                creditItem4 += Number($(this).find("td input.CreditItem4")?.val()?.trim() || '0');
                creditItem5 += Number($(this).find("td input.CreditItem5")?.val()?.trim() || '0');
                creditItem6 += Number($(this).find("td input.CreditItem6")?.val()?.trim() || '0');
                creditItem7 += Number($(this).find("td input.CreditItem7")?.val()?.trim() || '0');
                creditItem8 += Number($(this).find("td input.CreditItem8")?.val()?.trim() || '0');
                creditItem9 += Number($(this).find("td input.CreditItem9")?.val()?.trim() || '0');
                creditItem10 += Number($(this).find("td input.CreditItem10")?.val()?.trim() || '0');
                creditItem11 += Number($(this).find("td input.CreditItem11")?.val()?.trim() || '0');
                creditItem12 += Number($(this).find("td input.CreditItem12")?.val()?.trim() || '0');
                creditItem13 += Number($(this).find("td input.CreditItem13")?.val()?.trim() || '0');
                creditItem14 += Number($(this).find("td input.CreditItem14")?.val()?.trim() || '0');
                creditItem15 += Number($(this).find("td input.CreditItem15")?.val()?.trim() || '0');
                creditItem16 += Number($(this).find("td input.CreditItem16")?.val()?.trim() || '0');
                creditItem17 += Number($(this).find("td input.CreditItem17")?.val()?.trim() || '0');
                creditItem18 += Number($(this).find("td input.CreditItem18")?.val()?.trim() || '0');
                creditItem19 += Number($(this).find("td input.CreditItem19")?.val()?.trim() || '0');
                creditItem20 += Number($(this).find("td input.CreditItem20")?.val()?.trim() || '0');
                creditItem21 += Number($(this).find("td input.CreditItem21")?.val()?.trim() || '0');
                creditItem22 += Number($(this).find("td input.CreditItem22")?.val()?.trim() || '0');
                creditItem23 += Number($(this).find("td input.CreditItem23")?.val()?.trim() || '0');
                creditItem24 += Number($(this).find("td input.CreditItem24")?.val()?.trim() || '0');
                creditItem25 += Number($(this).find("td input.CreditItem25")?.val()?.trim() || '0');
            });

            $("#TotalDebitItem1").val(debitItem1);
            $("#TotalDebitItem2").val(debitItem2);
            $("#TotalDebitItem3").val(debitItem3);
            $("#TotalDebitItem4").val(debitItem4);
            $("#TotalDebitItem5").val(debitItem5);
            $("#TotalCreditItem1").val(creditItem1);
            $("#TotalCreditItem2").val(creditItem2);
            $("#TotalCreditItem3").val(creditItem3);
            $("#TotalCreditItem4").val(creditItem4);
            $("#TotalCreditItem5").val(creditItem5);
            $("#TotalCreditItem6").val(creditItem6);
            $("#TotalCreditItem7").val(creditItem7);
            $("#TotalCreditItem8").val(creditItem8);
            $("#TotalCreditItem9").val(creditItem9);
            $("#TotalCreditItem10").val(creditItem10);
            $("#TotalCreditItem11").val(creditItem11);
            $("#TotalCreditItem12").val(creditItem12);
            $("#TotalCreditItem13").val(creditItem13);
            $("#TotalCreditItem14").val(creditItem14);
            $("#TotalCreditItem15").val(creditItem15);
            $("#TotalCreditItem16").val(creditItem16);
            $("#TotalCreditItem17").val(creditItem17);
            $("#TotalCreditItem18").val(creditItem18);
            $("#TotalCreditItem19").val(creditItem19);
            $("#TotalCreditItem20").val(creditItem20);
            $("#TotalCreditItem21").val(creditItem21);
            $("#TotalCreditItem22").val(creditItem22);
            $("#TotalCreditItem23").val(creditItem23);
            $("#TotalCreditItem24").val(creditItem24);
            $("#TotalCreditItem25").val(creditItem25);

        },
        New: function () {
            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        Add: function () {
            this.CustomClear();
            this.GetNextVoucherNumber();
            $(".container-message").hide();
        },
        AddItem: function () {
            return
            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.Code").focus().select();
                }, 300);
                focusElement = "";
            }
            $this.SetFormControl();
            $this.AutoCompleteInit();
            Common.InitNumerics();
        },
        SetFormControl: function () {
            if ($("#table-dc-detail tbody").children().length > 0) {
                $("#item-container tbody tr td:nth-last-child(1) span.action").remove();
                $("#item-container tbody tr:nth-last-child(1)").remove();
                $("#item-container tbody tr input:not(input.Rate)").prop("disabled", true);
            }
            var Id = Common.GetInt($("#Id").val());
            if (Id > 0)
                $(".btn-load,#dc-search").addClass("hide");
            else
                $(".btn-load,#dc-search").removeClass("hide");

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
            $this.CalculateCalculations();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });

            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            $(".container-message").hide();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            Items = Common.SaveItemData().filter(x => x.ItemId && x.ItemCode);
            const saveItems = [];
            Items.forEach(e => {
                e.Date = record.Date
                e.Shift = record.Shift
                e.Status = record.ItemStatus
                e.ItemGroupId = record.GroupId
                e.ItemGroupName = record.GroupName
                e.VoucherNumber = record.VoucherNumber

                if (!id) {
                    var items = PageSetting.AccountDetails;
                    for (let key in e) {
                        if ((key.startsWith("DebitItem") || key.startsWith("CreditItem") && !key.endsWith("Code"))) {
                            const item = items.find(x => x.AccountId == e[key + 'Code'])
                            e[key + 'Price'] = item?.PurchasePrice || 0;
                        }
                    }
                }
                saveItems.push(e)
            })
            if (Common.Validate($("#mainform"))) {
                var err = "";
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: { Items: saveItems },
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving ...please wait",
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
            
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadReportData: function (res) {
            var $this = this;
            var d = res.Data.Order;
            Common.MapDataWithPrefixF(d, "#div-report", "lbl", "html");
            $("#lblDate").html(moment(d.Date).format("dddd, DD-MM-YYYY"));
            $("#lblBiltyDate").html(moment(d.BiltyDate).format("DD/MM/YYYY"));
            $("#lblContactPerson").html(res.Data.ContactPerson);

            var html = "";
            var items = d.SaleItems;
            var index = 1;
            $("#report-saleitem tbody").html("");
            for (var i in items) {
                var item = items[i];
                html += "<tr>";
                html += "<td>" + (index++) + "</td>";
                html += "<td>" + item.ItemName + "</td>";
                html += "<td>" + item.Quantity + "</td>";
                //html += "<td>" + item.Unit + "</td>";
                html += "<td>" + item.Rate.format() + "</td>";
                html += "<td>" + item.Amount.format() + "</td>";
                html += "<td>" + item.DiscountAmount.format() + "</td>";
                html += "<td>" + item.NetAmount.format() + "</td>";
                html += "</tr>";
            }
            $("#report-saleitem tbody").append(html);

            html = "";
        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        var dcs = res.Data.DeliveryChallans;
                        Common.MapEditData(d, "#form-info");
                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $('.isNew').removeClass('hide')
                            $('.isUpdate').addClass('hide')
                        }
                        else {
                            if (d && d.length > 0) {
                                if (d[0].Id == 0) {
                                    $('.isNew').removeClass('hide')
                                    $('.isUpdate').addClass('hide')
                                } else {
                                    $('.isNew').addClass('hide')
                                    $('.isUpdate').removeClass('hide')
                                }
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $this.LoadReportData(res);
                                Common.MapItemData(d);
                                $("#VoucherNumber").val(d[0].VoucherNumber);
                                $("#Id").val(d[0].Id);
                                $("#GroupName").val(d[0].ItemGroupName);
                                $("#GroupId").select2("val", d[0].ItemGroupId);
                                $("#Shift").select2("val", d[0].Shift);
                                $("#ItemStatus").select2("val", d[0].Status);
                                $(".date-picker").each(function () {
                                    Common.SetDate($("#Date"), d[0].Date);
                                });
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                                $this.CalculateCalculations();
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
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        Delete: function () {
            var $this = this;

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?voucherNo=" + voucherno;
                //var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno;
                var id = Common.GetInt($("#Id").val());
                if (id <= 0) {
                    Common.ShowError("No Voucher found for deletion.");
                    return;
                }

                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                        } else {
                            Common.ShowError(res.Error);
                        }
                        $this.CalculateCalculations();

                    },
                    error: function (e) {
                    }
                });
            });
        },
        LoadAccounts: function () {
            var $this = this;
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var tokens = Common.GetAllLeafAccounts(exids);
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }
            $this.UpdateHeaderLabel();
            $("#AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var d = Common.GetByCode(ui.item.value);
                    if (typeof d != "undefined" && d != null) {

                        $("#Comments").val("Adjustmen To: " + d.AccountCode + "-" + d.Name);

                        $("#AccountName").val(d.Name);
                        $("#AccountId").val(d.Id);
                        $(".container-message").hide();
                    }
                }
            });



        },
        UpdateHeaderLabel: function () {
            const ele = document.querySelectorAll("[id^='Item-']");
            var exids = new Array();
            exids.push(Common.GetInt(PageSetting.Products));
            var items = PageSetting.AccountDetails;
            ele.forEach(row => {
                const key = row?.id?.split("-")?.[1];
                const item = items.find(x => x.AccountId == key)
                if (item?.Name) {
                    row.innerHTML = item?.Name
                }
            })

        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $("#account-type-conatiner").addClass("hide");
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
                        id: product.Id,
                        value: product.AccountCode,
                        label: product.AccountCode + "-" + product.DisplayName,
                        name: product.DisplayName

                    }
                );
            }
            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var account = Common.GetByCode(ui.item.value);
                    var tr = $(this).closest("tr");
                    if (typeof account != "undefined" && account != null) {
                        var product = Common.GetAccountDetailByAccountId(account.Id);
                        var discount = 0;
                        $(this).val(account.AccountCode);
                        $(tr).find("input.ItemId").val(account.Id);
                        $(tr).find("input.Name").val(account.Name);
                        //$(tr).find(":nth-child(4) input.Unit").val(product.UnitType);
                        $(tr).find("input.Rate").val(Common.GetFloat(product.SalePrice));
                        $(tr).find("input.DiscountPercent").val(Common.GetFloat(discount));
                        $(tr).find("input.Comment").focus().select();


                    }

                }
            });

        },
    };
}();