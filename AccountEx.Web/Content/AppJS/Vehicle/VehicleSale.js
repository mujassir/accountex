
var VehicleSale = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "VehicleSale";
    var UPLOAD_FOLDER = "AgreementFiles";
    var PageSetting = new Object();
    var LIST_CHANGED = false;
    var LIST_LOADED = false;
    var BRANCHID = 0;
    var ID = 0;
    var DATA = null;
    return {
        init: function () {
            var $this = this;
            BRANCHID = $("#BranchId").val();
            $("input").attr("autocomplete", "off");
            $this.CustomClear();
            $("#InsuranceAccountId,#ReturnAccountId").html($("#TrackerAccountId").html()).select2();
            $("#BranchId").change(function () {
                LIST_LOADED = false;
                DataTable.DestroyDatatable(DATATABLE_ID);
                $this.init();
            });


            if (VoucherType[$this.GetType()] == VoucherType.vehicleinstallmentsale) {
                $(".NoOfInstallments").removeClass("hide");
                //$(".PaymentOptions").addClass("hide");
                //$("#PaymentOptions").removeAttr("data-required");
            }
            else {
                $(".NoOfInstallments").addClass("hide");
                //$(".PaymentOptions").removeClass("hide");
                //$("#PaymentOptions").prop("data-required", true);
            }
            $(document).on("keyup", "#SalePrice,#Received,#LogBookFee", function () {
                $this.CalculateBalance();
            });

            $(document).on("keyup change", "#DepreciationAmount,#IsTradeUnitReturned", function () {
                $this.CalculateReturnAmount();
            });


            $(document).on("keyup", "#InsurenceValue,#InsurencePercent,#LevyPercent,#StampDutyAmount", function () {
                $this.CalculateInsurancValue();
            });

            $(document).on("keyup", "#TrackerSalePrice,#TrackerReceivedAmount", function () {
                $this.CalculateTrackerBalance();
            });
            $(document).on("keyup", "#InsurenceSalePrice,#InsurenceReceivedAmount", function () {
                $this.CalculateInsuranceBalance();
            });
            $(document).on("keyup change", "#Discount,#Amount", function () {
                $this.CalculateDiscount();
            });
            $(document).on("keyup", "#VoucherNo", function (e) {
                if (e.which == 13) {
                    $this.LoadInstallmentDetails();
                }
            });

            $(document).on("change", "#Date", function () {

                var date = moment($("#Date").val(), "DD/MM/YYYY");
                date = date.toDate();
                date = date.setMonth(date.getMonth() + 1);
                Common.SetDate($("#InstalmentStartDate"), date);
            });

            $(document).on("change", "#IsTradeIn", function () {
                if ($(this).is(":checked")) {
                    $(".tradein-container").removeClass("hide");
                    Common.UpdateRequired("#TradeInVehicleId", true);
                    Common.UpdateRequired("#TradeInPrice", true);
                }
                else {
                    $("#TradeInPrice").val(0);
                    $("#TradeInVehicleId").select2("val", '');
                    Common.UpdateRequired("#TradeInVehicleId", false);
                    Common.UpdateRequired("#TradeInPrice", false);
                    $(".tradein-container").addClass("hide");
                }
                $this.CalculateBalance();
            });
            $(document).on("change", "#TradeInVehicleId", function () {
                var price = Common.GetInt($("option:selected", this).attr("data-price"));
                $("#TradeInPrice").val(price);
                $this.CalculateBalance();
            });
            $(document).on("change", "#PaymentMode", function () {
                var paymentOptions = $(this).val();
                var scope = $(this).closest("div[data-save='save']")
                if (paymentOptions != "Bank") {
                    $(".banks-options", scope).addClass("hide");
                }
                else {
                    $(".banks-options", scope).removeClass("hide");
                }
            });
            $(document).on("change", "#IsTrackerAdded", function () {
                var markRequired = false;
                if ($(this).is(":checked")) {
                    $(".tracker-container").removeClass("hide");
                    markRequired = true;
                }
                else {
                    $(".tracker-container").addClass("hide");
                    markRequired = false;
                }
                $(".tracker-container").find("[data-required]").each(function () {
                    Common.UpdateRequired("#" + $(this).attr("id"), markRequired);

                });
            });
            $(document).on("change", "#IsInsurenceAdded", function () {
                var markRequired = false;
                if ($(this).is(":checked")) {
                    $(".insurance-container").removeClass("hide");
                    markRequired = true;
                }
                else {
                    $(".insurance-container").addClass("hide");
                    markRequired = false;
                }
                $(".insurance-container").find("[data-required]").each(function () {
                    Common.UpdateRequired("#" + $(this).attr("id"), markRequired);

                });
            });
            $(document).on("change", "#InstalmentNo", function () {
                var amount = $("option:selected", $(this)).attr("data-amount");
                $("#Amount").val(amount).trigger("change");
            });
            $(document).on("keyup", "#NoOfInstallments,#SalePrice,#Received,#InstalmentStartDate", function () {
                var noOfInstallments = $("#NoOfInstallments").val();
                $this.CalculateInstallments(noOfInstallments);
            });
            $(document).on("keyup", "#item-container input.Amount", function () {
                $this.CalculateInstallmentTotal();

            });

            $(document).on("click", "#btnprint", function () {
                $this.PrintAgreement();
            });
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });
            $(document).on("change", "#item-sales-deposits-container .AccountId", function () {
                var tr = $(this).closest("tr");
                var accountId = Common.GetInt($(tr).find("select.AccountId").val());
                var amount = Common.GetInt($(tr).find("input.Amount").val());
                if (accountId == 0) {
                    Common.ShowError("Account is required");
                    return;
                }
                if (amount <= 0) {
                    Common.ShowError("Account should be greate than zero(0).");
                    return;
                }
                else
                    $this.AddItem();
            });
            $(document).on("keyup", "#item-sales-deposits-container .Amount", function (event) {
                if (event.which == 13) {
                    var tr = $(this).closest("tr");
                    var accountId = Common.GetInt($(tr).find("input.AccountId").val());
                    var amount = Common.GetInt($(tr).find("input.Amount").val());
                    if (accountId == 0) {
                        Common.ShowError("Account is required");
                        return;
                    }
                    if (amount <= 0) {
                        Common.ShowError("Account should be greate than zero(0).");
                        return;
                    }
                    else
                        $this.AddItem();


                }
                $this.GetWholeReceieved();
            });
            //var datePicker = $("#InstalmentStartDate.date-picker").on('change', function (ev) {
            //    var noOfInstallments = $("#NoOfInstallments").val();
            //    $this.CalculateInstallments(noOfInstallments);
            //});
            $this.LoadPageSetting();
            $this.AutoCompleteInit();
            $this.LoadVehicles();

            $this.LoadSupplier();


            $this.LoadBanks();
            var options =
              {
                  dropZoneTitle: "Drag & drop agreemnt scan here …",
                  browseLabel: "Pick Agreemnt Scan",
                  showPreview: true,
                  allowedFileExtensions: ["jpg", "png"],
                  elErrorContainer: "#errorBlock43",

              };
            Common.BindFileInput("ProfilePic", "FileUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER, options);
            this.ListView();
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


        New: function () {

            var $this = this;
            focusElement = "#InvoiceNumber";
            $this.LoadVoucher("nextvouchernumber");
        },
        DeleteFile: function (element) {
            $(element).parent().parent().remove();


        },
        DeleteAllFile: function (element) {
            var $this = this;
            $("tbody.files tr.template-download td:nth-child(4) input:checked").each(function () {
                $this.DeleteFile($(this));
            });

        },

        LoadInstallmentDetails: function () {
            Common.ClearByScope("#model-pay-installments")
            var installmentNo = Common.GetInt($("#VoucherNo").val());
            var vehicleSaleId = Common.GetInt($("#Id").val());
            var vehicleId = Common.GetInt($("#VehicleId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?installmentNo=" + installmentNo + "&vehicleSaleId=" + vehicleSaleId + "&vehicleId=" + vehicleId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ... please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data;

                        Common.MapEditData(d.OtherDetail, ".installment-form");
                        Common.SetDate("#RecievedDate", new Date());
                        var html = "";
                        html += "<option></option>";
                        var installments = d.Installments;
                        for (var i = 0; i < installments.length; i++) {
                            var token = installments[i];
                            var text = token.InstalmentNo + " (" + token.Amount.format() + "-" + token.RecievedAmount.format() + "=" + token.Pending.format() + ")";
                            if (token.RecievedAmount <= 0)
                                text = token.InstalmentNo + " (" + token.Pending.format() + ")";
                            html += "<option value='" + token.InstalmentNo + "' data-amount='" + token.Pending + "' data-instalment-no='" + token.InstalmentNo + "' >" + text + "</option>";
                        }
                        $("#InstalmentNo").html(html).select2(
                             {
                                 placeholder: Common.GetSelect2PlaceHolder($("#InstalmentNo")),
                                 minimumResultsForSearch: Setting.MinimumResultsForSearch
                             });
                        var payments = d.PaidInstallments;
                        var html = "";
                        for (var i in payments) {
                            html += "<tr>";
                            var payment = payments[i];
                            var account = Common.GetById(payment.RcvAccountId);
                            html += "<td>" + Common.FormatDate(payment.RecievedDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + payment.Amount.format() + "</td>";
                            html += "<td>" + payment.Discount.format() + "</td>";
                            html += "<td>" + payment.PaymentMode + "</td>";
                            html += "<td>" + account.Name + "</td>";
                            html += "<td><button type='button' class='btn yellow btn-xs' onclick='VehicleSale.PrintReceipt(" + payment.Id + ")'><i class='fa fa-print'></i>&nbsp;Print</button>&nbsp;";
                            html += "<button type='button' class='btn red btn-xs' onclick='VehicleSale.DeleteInstallment(" + payment.Id + ",this)'><i class='fa fa-trash'></i>&nbsp;Delete</button></td>";
                            html += "</tr>";
                        }
                        var vehicle = d.Vehicle;
                        $("#Remarks", $(".installment-form")).val("Installment payment " + vehicle.Name);
                        $("#tbl-paid-installments tbody").html(html);
                        $("#model-pay-installments").modal("show");
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });

        },
        LoadDocuments: function (id) {
            var $this = this;
            ID = id;
            $("#tbl-document-uploads tbody").html('');
            $("tbody.files").html('');
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?laodDatakey=loadDocuments" + "&id=" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: ".model-upload-documents",
                blockMessage: "Loading ... please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data;
                        var documents = d.Documents;
                        var html = "";
                        for (var i in documents) {
                            html += "<tr>";
                            var document = documents[i];
                            html += "<td>" + document.Name + "</td>";
                            html += "<td><span class='preview'>";
                            html += "<a target='_blank' href='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "' title='View Large Image'><img src='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "'></a>";
                            html += "</span></td>";

                            //html += "<td><a class='mix-preview fancybox-button' target='_blank' href='../upload/ApplicationProcess/" + document.Url + "' title='View Large Image' data-rel='fancybox-button'><img class='img-responsive' style='max-height:200px;'  src='../upload/ApplicationProcess/" + document.Url + "' alt='' data-rel='fancybox-button'></a></td>";
                            //html += "<td><a class='mix-preview fancybox-button' target='_blank' href='../upload/VehicleAgreements/'" + documents.Url + "' title='View Large Image' data-rel='fancybox-button'>View Large Image</a></td>";
                            //html += "<td><button type='button' class='btn yellow btn-xs' onclick='VehicleSale.PrintReceipt(" + document.Id + ")'><i class='fa fa-print'></i>&nbsp;Delete</button>";
                            html += "</tr>";
                        }
                        $("#tbl-document-uploads tbody").html(html);
                        $("#model-upload-documents").modal("show");
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });

        },
        SaveDocuments: function () {
            var $this = this;
            if (Common.Validate($(".model-upload-documents"))) {
                var scope = $(".model-upload-documents");
                var record = new Object();
                var files = new Array();
                $("tbody.files tr td:last-child", scope).each(function () {
                    var file = $(this).children("input.data").attr("data-file");
                    if (typeof file != 'undefined' && file != null) {
                        file = $.parseJSON(file);
                        file["SaleId"] = ID;
                        files.push(file);
                    }

                });
                record = { '': files }

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?savedocuments=savedocuments",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: ".model-upload-documents",
                    blockMessage: "saving document..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Document saved successfully." });
                            $("#model-upload-documents").modal("hide");
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


        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();
            $this.GetWholeTotal();
            if ($("#item-sales-deposits-container tbody").children().length <= 0)
                $this.AddItem();
        },
        GetWholeReceieved: function () {
            var $this = this;

            var amount = 0.0;
            $("#item-sales-deposits-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find("input.Amount").val());

            });
            $("#item-sales-deposits-container tfoot tr").find("input.Amount").val(amount.format());
            $("#Received").val(amount);
            $this.CalculateBalance();

        },
        LoadExpensesDetails: function () {
            var $this = this;
            var vehicleId = Common.GetInt($("#VehicleId").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?loadExpense=true&vehicleId=" + vehicleId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading expenses detail... please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data;
                        var purchasePrice = Common.GetInt($("#PurchasePrice").val());
                        var expenses = res.Data;
                        var total = purchasePrice + expenses;
                        $("#lblpurchaseprice").html(Common.GetInt(purchasePrice).format());
                        $("#lblexpenses").html(Common.GetInt(expenses).format());
                        $("#lbltotal").html(Common.GetInt(total).format());
                        $this.CalculateBalance();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });

        },
        LoadTradeInVehicle: function (data) {
            var $this = this;
            var customerId = Common.GetInt($("#AccountId").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?loadTradeVehicle=true&tradeVehicle=tradeVehicle&customerId=" + customerId + "&branchId=" + BRANCHID,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading trade in vehicles... please wait",
                success: function (res) {
                    if (res.Success) {
                        var $this = this;
                        var vehicles = res.Data;
                        var html = "<option value=''></option>";
                        for (var i in vehicles) {
                            var vehicle = vehicles[i];
                            var label = "Chessis No:" + vehicle.ChassisNo + " RegNo No:" + vehicle.RegNo + " Manufacturer:" + vehicle.Manufacturer;
                            html += "<option value=\"" + vehicle.Id + "\" data-price=\"" + vehicle.PurchasePrice + "\">" + label + "</option>";
                        }
                        $("#TradeInVehicleId").html(html).select2();
                        if (data != null && data != undefined) {
                            $("#TradeInVehicleId").select2("val", data.TradeInVehicleId);
                            $("#TradeInVehicleId").trigger("change");
                            $("#IsTradeIn").trigger("change");
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
        AddInstallments: function () {
            var $this = this;
            $("#model-pay-installments").modal("show");
        },
        LoadBanks: function () {
            var banks = Common.GetLeafAccounts(PageSetting.Banks);
            var html = "";
            html += "<select class='form-control Banks'>";
            for (var i in banks) {
                var bank = banks[i];
                html += "<option value=" + bank.Id + ">" + bank.AccountCode + "</option>";
            }
            html += "</select>";
            $(".Banks").html(html);
        },
        SaveInstallments: function () {
            var $this = this;
            if (Common.Validate($(".installment-form"))) {
                var record = Common.SetValue($(".installment-form"));
                record.VehicleSaleId = $("#Id").val();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=payinstallments",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Installment saved successfully." });
                            $("#model-pay-installments").modal("hide");
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

        PrintAgreement: function () {
            var $this = this;
            var type = $this.GetType();
            var element = "#CashDealAgreement-print-container";
            if (type != 'vehiclecashsale')
                element = "#VehicleSaleAgreement-print-container";
            $this.Print(element);
        },
        Print: function (element) {
            var $this = this;
            $(".main-print-conatiner >div").addClass("hide").removeClass("visible-print");
            $(".main-print-conatiner").find(element).removeClass("hide").addClass("visible-print");
            window.print();
        },
        Add: function () {
            var $this = this;
            $this.CustomClear();
            $this.GetNextVoucherNumber();

            $(".container-message").hide();
        },
        toWords: function (s) {
            var th = ['', 'thousand', 'million', 'billion', 'trillion'];

            var dg = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];

            var tn = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];

            var tw = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];
            s = s.toString();
            s = s.replace(/[\, ]/g, '');
            if (s != parseFloat(s)) return 'not a number';
            var x = s.indexOf('.');
            if (x == -1) x = s.length;
            if (x > 15) return 'too big';
            var n = s.split('');
            var str = '';
            var sk = 0;
            for (var i = 0; i < x; i++) {
                if ((x - i) % 3 == 2) {
                    if (n[i] == '1') {
                        str += tn[Number(n[i + 1])] + ' ';
                        i++;
                        sk = 1;
                    } else if (n[i] != 0) {
                        str += tw[n[i] - 2] + ' ';
                        sk = 1;
                    }
                } else if (n[i] != 0) {
                    str += dg[n[i]] + ' ';
                    if ((x - i) % 3 == 0) str += 'hundred ';
                    sk = 1;
                }
                if ((x - i) % 3 == 1) {
                    if (sk) str += th[(x - i - 1) / 3] + ' ';
                    sk = 0;
                }
            }
            if (x != s.length) {
                var y = s.length;
                str += 'point ';
                for (var i = x + 1; i < y; i++) str += dg[n[i]] + ' ';
            }
            return str.replace(/\s+/g, ' ') + " Shilling Only";

        },

        CalculateInstallmentTotal: function () {
            var sum = $("#item-container tbody input.Amount").toEnumerable().Select("Common.GetInt($.val())").Sum(); // 55
            $("#item-container tfoot label.lbl-installment-total").html(sum.format());
        },

        CalculateInstallments: function (noOfInstallments) {
            var $this = this;
            var html = "";
            $(".instalment-status").addClass("hide");
            var balance = $("#Balance").val();
            instalmentStartDate = moment($("#InstalmentStartDate").val(), "DD/MM/YYYY");
            instalmentStartDate = instalmentStartDate.toDate();
            var records = new Array();
            var x = 1; //or whatever offset
            var perInstallmentAmount = balance / noOfInstallments;
            $("#item-container tbody").html("");

            for (var i = 1; i <= noOfInstallments; i++) {
                var date = moment(($("#InstalmentStartDate").val(), moment(instalmentStartDate).format("DD/MM/YYYY")), "DD/MM/YYYY");
                if (i == noOfInstallments) {
                    var addedAmount = (i - 1) * Common.GetInt(perInstallmentAmount);
                    perInstallmentAmount = balance - addedAmount;
                }
                var item = new Object({
                    Id: 0,
                    InstalmentNo: i,
                    InstallmentDate: date,
                    Amount: Common.GetInt(perInstallmentAmount)


                });
                records.push(item);
                date = instalmentStartDate.setMonth(instalmentStartDate.getMonth() + x);
                //html += "<tr>";
                //html += "<td clas='align-center'>" + (i + 1) + "";
                //html += "<input type='text' class='InstalmentNo hide' value='" + (i + 1) + "' data-db-column='InstalmentNo' data-db-type='int' />";
                //html += "<input type='text' class='Id hide' value='0' data-db-column='Id' data-db-type='int' />";
                //html += "</td>";
                //html += "<td><input type='text' class='InstallmentDate date-picker' value='" + moment(instalmentStartDate).format("DD/MM/YYYY") + "' data-db-column='InstallmentDate' data-db-type='date' /></td>";
                //html += "<td><input type='text' class='Amount num3' value='" + Math.ceil(perInstallmentAmount) + "' data-db-column='Amount' data-db-type='decimal'/></td>";

                //html += "</tr>";

            }
            Common.MapItemData(records, null, null, true);
            Common.InitDateMask();
            Common.InitNumerics();
            $this.CalculateInstallmentTotal();
        },
        CalculateBalance: function () {
            var salePrice = Common.GetInt($("#SalePrice").val());
            var TrackerSalePrice = Common.GetInt($("#TrackerSalePrice").val());
            var insuranceSalePrice = Common.GetInt($("#InsurenceSalePrice").val());
            var received = Common.GetInt($("#Received").val());
            var advance = Common.GetInt($("#Advance").val());
            var trackerReceived = Common.GetInt($("#TrackerReceivedAmount").val());
            var insuranceReceived = Common.GetInt($("#InsurenceReceivedAmount").val());
            var advance = Common.GetInt($("#Advance").val());
            var logbookFee = Common.GetInt($("#LogBookFee").val());
            var tradeinPrice = Common.GetInt($("#TradeInPrice").val());


            salePrice = salePrice + TrackerSalePrice + insuranceSalePrice + logbookFee;
            received = received + trackerReceived + insuranceReceived + tradeinPrice;

            var balance = salePrice - received - advance;
            $("#Balance").val(balance);
        },

        CalculateInsurancValue: function () {
            var $this = this;
            var value = Common.GetInt($("#InsurenceValue").val());
            var percent = Common.GetFloat($("#InsurencePercent").val());
            var insurenceAmount = Common.GetInt(Common.GetFloat(percent / 100) * value);
            var levyPercent = Common.GetFloat($("#LevyPercent").val());
            var levyAmount = Common.GetInt(Common.GetFloat(levyPercent / 100) * insurenceAmount);
            var stampDutyAmount = Common.GetFloat($("#StampDutyAmount").val());
            var salePrice = insurenceAmount + levyAmount + stampDutyAmount;
            $("#InsurenceAmount").val(insurenceAmount);
            $("#LevyAmount").val(levyAmount);
            $("#InsurenceSellingPrice").val(salePrice);
            $this.CalculateInsuranceBalance();
        },
        CalculateTrackerBalance: function () {
            var $this = this;
            var salePrice = $("#TrackerSalePrice").val();
            var received = $("#TrackerReceivedAmount").val();
            var balance = Common.GetInt(salePrice) - Common.GetInt(received);
            $("#TrackerBalanceAmount").val(balance);
            $this.CalculateBalance();
        },
        CalculateInsuranceBalance: function () {
            var $this = this;
            var salePrice = $("#InsurenceSalePrice").val();
            var received = $("#InsurenceReceivedAmount").val();
            var balance = Common.GetInt(salePrice) - Common.GetInt(received);
            $("#InsurenceBalanceAmount").val(balance);
            $this.CalculateBalance();
        },
        DetailView: function () {
            //$('#form-info').removeClass('hide');
            //$('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            var $this = this;
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + VoucherType[$this.GetType()] + "&branchId=" + BRANCHID;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url, undefined, function () {
                    $(".table-scrollable").removeClass("table-scrollable");
                });
            }

            $("#Name").focus();
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
            Common.Clear();
            $("#PaymentMode,#Date").trigger("change");
            $(".instalment-status").addClass("hide");
            $("#lblpurchaseprice").html(Common.GetInt(0).format());
            $("#lblexpenses").html(Common.GetInt(0).format());
            $("#lbltotal").html(Common.GetInt(0).format());
            $("#lbladvance").html("0.0");
            $("tbody.files").html("");
            $("#PaymentMode,#IsTrackerAdded,#IsInsurenceAdded").trigger("change");
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        MarkFinal: function (id, isFinal) {
            $this = this;
            var mf = function () {
                var record = new Object();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=markFinal&isFinal=" + isFinal,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "please wait",
                    success: function (res) {
                        if (res.Success) {
                            if (isFinal) {
                                Common.ShowMessage(true, { message: "Sale has been finalized successfully." });
                            }
                            else {
                                Common.ShowMessage(true, { message: "Sale has been  successfully mark as Unfinsihed." });
                            }
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.ListView();
                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                })
            };
            if (!isFinal)
                Common.ConfirmDelete(mf, ActionType.Senstitive);
            else
                mf();

        },
        MarkDelivered: function (id) {
            if (!confirm('Are you sure to mark this vehicle as delivered...??'))
                return;
            $this = this;
            var record = new Object();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=markDelivery",
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "marking delivery..please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Delivery process completed successfully." });
                        DataTable.RefreshDatatable(DATATABLE_ID);
                        $this.ListView();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },
        MarkVoid: function (id, isVoid) {
            $this = this;
            Common.ConfirmDelete(function () {
                var record = new Object();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=markVoid&isVoid=" + isVoid,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "please wait",
                    success: function (res) {
                        if (res.Success) {
                            if (isVoid) {
                                Common.ShowMessage(true, { message: "Sale has been mark void successfully." });
                            }
                            else {
                                Common.ShowMessage(true, { message: "Sale has been successfully mark as valid." });
                            }
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.ListView();
                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                })
            }, ActionType.Senstitive);

        },
        PrintDeliveryNote: function (id) {

            $this = this;
            var record = new Object();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?doPrinting=true&id=" + id + "&printKey=deliveryNotePrint",
                type: "Get",
                data: record,
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "printing delivery note..please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = $("#deliverynote-print-container");
                        var d = res.Data;
                        Common.MapDataWithPrefixFClasses(d, scope, "lbldn", "html");
                        $this.Print("#deliverynote-print-container");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },
        LoadVehicles: function (id) {

            $this = this;
            var record = new Object();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?saleAvailable=true" + "&branchId=" + BRANCHID,
                type: "Get",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading vehicls..please wait",
                success: function (res) {
                    if (res.Success) {
                        VehicleSale.VehicleAutoCompleteInit(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },
        PrintReceipt: function (id) {

            $this = this;
            var record = new Object();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?doPrinting=true&id=" + id + "&printKey=CRPrint",
                type: "Get",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "printing receipt..please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = $("#cashreceipt-print-container");
                        var vp = res.Data.Payment;
                        var sale = res.Data.Sale;
                        Common.MapDataWithPrefixFClasses(sale, scope, "lblcrvs", "html");
                        Common.MapDataWithPrefixFClasses(vp, scope, "lblcrvp", "html");
                        $("span.lbldcrvpamountinword").html($this.toWords(vp.Amount));
                        $this.Print("#cashreceipt-print-container");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },



        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                record.BankId = $(".Banks option:selected").val();
                record["TransactionType"] = VoucherType["sale"];
                record["SaleType"] = VoucherType[$this.GetType()];
                record["BranchId"] = BRANCHID;
                if (VoucherType[$this.GetType()] == VoucherType.vehicleinstallmentsale) {
                    var saleDetails = new Array();
                    var vehiclesaledeposits = new Array();
                    Items = Common.SaveItemData();

                    record["VehicleSaleDetails"] = Items;



                }
                VehicleSaleDeposits = Common.SaveItemData("#item-sales-deposits-container");

                var files = new Array();
                $("tbody.files tr td:last-child").each(function () {
                    var file = $.parseJSON($(this).children("input.data").attr("data-file"));
                    if (typeof file != 'undefined' && file != null) {
                        file["SaleId"] = record.Id;
                        files.push(file);
                    }

                });
                if (record.SaleType == VoucherType.vehicleinstallmentsale) {
                    var installmentAmount = Enumerable.From(record.VehicleSaleDetails).Sum(function (x) { return Common.GetInt(x.Amount) });
                    if (installmentAmount != record.Balance) {
                        Common.ShowError("Installment total amount is not equal to total balance.");
                        return;
                    }
                }
                var err = "";
                for (var i in VehicleSaleDeposits) {
                    var item = VehicleSaleDeposits[i];
                    if (item.Amount > 0 && item.AccountId == 0) {
                        err += "Please select deposit account where amount is greater than zero(0).,";
                    }
                }
                VehicleSaleDeposits = Enumerable.From(VehicleSaleDeposits).Where("$.AccountId>0").ToArray();
                record["VehicleSaleDeposits"] = VehicleSaleDeposits;
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["SaleDocuments"] = files;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Sale...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.GetNextVoucherNumber();
                            $("#Name").focus();
                            $(".pay-installment").addClass("hide");
                            PageSetting.Vehicles = Enumerable.From(PageSetting.Vehicles).Where(function (p) { return p.Id != record.VehicleId }).ToArray();
                            $this.LoadVehicles();
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

        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");

        },
        LoadVoucher: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "&key=" + key + "&voucher=" + voucherno + "&byvoucher=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var vehicle = "";
                        $("#item-container tbody").html("");
                        $("#item-sales-deposits-container tbody").html("");
                        $(".pay-installment").addClass("hide");
                        $this.CustomClear();
                        DATA = res.Data;
                        var d = res.Data.Voucher;
                        if (d != null) {
                            var vehicle = res.Data.Vehicle;
                            if (vehicle != null) {
                                delete vehicle.Id;
                                delete vehicle.IsTradeIn;
                                delete vehicle.SalePrice;
                                $.extend(true, d, res.Data.Customer);
                            }
                            $.extend(true, d, vehicle);
                        }

                        Common.MapEditData(d, "#form-info");
                        $("#lbladvance").html("0.0");

                        if (d == null) {
                            $("#DCNo").removeProp("disabled");
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#InvoiceNumber").val(res.Data.InvoiceNumber);
                        }
                        else {
                            //var voucher = this.GetType();
                            Common.SetCheckValue(d);
                            $("#PreVoucherNumber").val(d.VoucherNumber);
                            //if (voucher != "sale") 
                            $("#DCNo").prop("disabled", "disabled");
                            //else
                            //    $("#DCNo").removeProp("disabled");
                            $(".date-picker,.ac-date").each(function () {

                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            if (d.Advance > 0) {
                                $("#lbladvance").html(Common.GetFloat(d.Advance).format());
                            }

                            if (d.Id > 0 && d.VehicleSaleDetails != null && d.VehicleSaleDetails.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                $("#qtytotal1").val(d.QuantityTotal);
                                var saleDetail = saleDetails = Enumerable.From(d.VehicleSaleDetails).OrderBy("$.InstalmentNo").ToArray();;

                                if (d.IsFinal) {
                                    for (var i in saleDetail) {
                                        var item = saleDetail[i];
                                        item["RecievedAmount"] = item.RecievedAmount.format();
                                        var status = "<span class='label label-sm label-info'>Pending</span>"
                                        if (item.IsRecieved) {
                                            status = "<span class='label label-sm label-success'>Paid</span>"
                                        }

                                        else {
                                            var today = new Date();
                                            today.setHours(0, 0, 0, 0);

                                            var instalmentDate = moment(item.InstallmentDate);
                                            instalmentDate = instalmentDate.toDate();
                                            instalmentDate.setHours(0, 0, 0, 0);

                                            if (instalmentDate < today) {
                                                status = "<span class='label label-sm label-danger'>Overdue</span>"
                                            }
                                            else if (!item.IsRecieved && item.RecievedAmount > 0) {
                                                status = "<span class='label label-sm label-warning'>Partialy Paid</span>"
                                            }


                                        }
                                        //if (Common.isNullOrWhiteSpace(item.BankName))
                                        //    item["BankName"] = "";
                                        //if (Common.isNullOrWhiteSpace(item.ChequeNo))
                                        //    item["ChequeNo"] = "";
                                        item["Status"] = status;
                                    }
                                }
                                Common.MapItemData(saleDetail, null, null, true);

                                if (d.IsFinal)
                                    $(".pay-installment").removeClass("hide");
                                setTimeout(function () {
                                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                                }, 500);
                            }
                            if (d.Id > 0 && d.VehicleSaleDeposits != null && d.VehicleSaleDeposits.length > 0) {

                                var saledeposits = d.VehicleSaleDeposits;
                                Common.MapItemData(saledeposits, "#item-sales-deposits-container", "#template-sales-deposits", false);
                            }
                            $this.LoadTradeInVehicle(d);
                            var files = d.SaleDocuments;
                            var $form = $('#fileupload');
                            $form.fileupload('option', 'done').call($form, $.Event('done'), { result: { files: files } });


                        }
                        if (d != null) {
                            if (d.IsFinal) {
                                $("#btnsave,#btnfinal,#btndelete").addClass("hide");
                                $("#btnunfinal").removeClass("hide");
                                $(".instalment-status").removeClass("hide");
                                $(".pay-installment").removeClass("hide");


                            }
                            else {
                                $(".pay-installment").addClass("hide");
                                $(".instalment-status").addClass("hide");
                                $("#btnsave,#btnfinal,#btndelete").removeClass("hide");
                                $("#btnunfinal").addClass("hide");
                            }
                        }
                        $this.LoadExpensesDetails();
                        $("#btndelete,#btnprint").prop("disabled", false);
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        if (d != null && d.IsFinal) {
                            if (d.RecoveryStatus == RecoveryStatus.SaleReturn) {
                                $(".form-actions .canceldeal").addClass("hide");
                                Common.ShowMessage(true, { message: "This deal has been cancelled.No action can been done on this deal." });
                            }
                            else {
                                $(".form-actions .canceldeal").removeClass("hide");
                            }
                        }
                        else {
                            $(".form-actions .canceldeal").addClass("hide");
                        }
                        $this.AddItem();
                        Common.InitDateMask();
                        Common.InitNumerics();
                        $this.CalculateInsurancValue();
                        $this.CalculateInstallmentTotal();
                        $("#PaymentMode,#IsTrackerAdded,#IsInsurenceAdded").trigger("change");
                        if (d != null && vehicle != null) {
                            $this.LoadPrint(d, vehicle, res.Data.TradeVehicle);
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
        LoadPrint: function (d, vehicle, tradeVehicle) {
            var $this = this;
            var scope = $(".print-container");
            Common.MapDataWithPrefixFClasses(d, scope, "", "html");
            $('span.SalePrice').text(Common.GetInt(d.SalePrice).format());
            $('span.SalePriceInWords').text($this.toWords(d.SalePrice));
            var rcvd = d.Received + d.Advance + d.TradeInPrice;
            $('span.Deposited').text(Common.GetInt(rcvd).format());

            $('span.lbltradePurchasePrice').text(Common.GetInt(rcvd).format());
            $('span.Received').text(Common.GetInt(rcvd).format());
            $('span.Advance').text(Common.GetInt(d.Advance).format());
            var depositHtml = "2.";
            var trackerInsuranceHtml = "";
            if (d.IsTradeIn) {
                depositHtml += "DEPOSIT: TRADE IN WITH <span>" + tradeVehicle.Manufacturer + "</span>&nbsp;<span>" + tradeVehicle.Model + "</span>&nbsp;";
                depositHtml += "<span>" + tradeVehicle.RegNo + "</span> at <span>" + Common.GetInt(tradeVehicle.PurchasePrice).format() + "</span> AND";
            }
            if ((d.IsInsurenceAdded && d.InsurenceBalanceAmount > 0) || (d.IsTrackerAdded && d.TrackerBalanceAmount > 0)) {
                trackerInsuranceHtml += "(Inclusive ";
                if (d.IsTrackerAdded && d.TrackerBalanceAmount > 0) {
                    trackerInsuranceHtml += "Tracker:<span class='TrackerBalanceAmount'>" + d.TrackerBalanceAmount.format() + "</span>";
                    if (d.IsInsurenceAdded && d.InsurenceBalanceAmount > 0) {
                        trackerInsuranceHtml += ",";
                    }
                }
                if (d.IsInsurenceAdded && d.InsurenceBalanceAmount > 0) {
                    trackerInsuranceHtml += "Insurance:<span class='InsurenceBalanceAmount'>" + d.InsurenceBalanceAmount.format() + "</span>";
                }
                trackerInsuranceHtml += ")";
            }

            $('span.ins-trac-lines').html(trackerInsuranceHtml);

            var deposits = d.VehicleSaleDeposits;
            for (var i in deposits) {
                var deposit = deposits[i];
                if (i == 0)
                    depositHtml += " PAID ";
                else
                    depositHtml += " AND ";

                depositHtml += deposit.PaymentMode + " OF <span>" + Common.GetInt(deposit.Amount).format() + "</span>&nbsp;ON <span>" + Common.FormatDate(deposit.Date, "DD-MM-YYYY") + "</span>&nbsp;";
            }


            if (d.Advance > 0) {
                depositHtml += "<span>AND ADVANCE PAYMENT OF <span>" + Common.GetInt(d.Advance).format() + "</span></span>";
            }
            depositHtml += "(<span class='text-uppercase'>" + $this.toWords(rcvd) + "</span>).";
            $('p.deposit-container').html(depositHtml);
            $('span.LogBookFee').text(Common.GetInt(d.LogBookFee).format());
            $('span.ReceivedInWords').text($this.toWords(rcvd));
            var balance = d.Balance;
            if (d.SaleType != VoucherType.vehiclecashsale) {
                balance = d.Balance - d.LogBookFee;
            }
            else {
                balance = d.Balance;
            }


            $('span.Balance').text(Common.GetInt(balance).format());
            $('span.TrackerBalanceAmount').text(Common.GetInt(d.TrackerBalanceAmount).format());
            $('span.InsurenceBalanceAmount').text(Common.GetInt(d.InsurenceBalanceAmount).format());
            $('span.BalanceInWords').text($this.toWords(balance));
            $('span.TotalBalance').text(Common.GetInt(d.Balance).format());
            $('span.TotalBalanceInWords').text($this.toWords(d.Balance));
            $('span.Date').text(Common.FormatDate(d.Date, "DD MMMM, YYYY"));
            var html = "";
            var saledetails = d.VehicleSaleDetails;
            for (var i in saledetails) {
                var installment = saledetails[i];
                html += "<tr>";
                html += "<td>" + installment.InstalmentNo + "</td>";
                html += "<td>" + Common.FormatDate(installment.InstallmentDate, "DD/MM/YYYY") + "</td>";
                html += "<td>" + Common.GetInt(installment.Amount).format() + "</td>";
                html += "</tr>";
            }
            $('span.TotalInstallment').text(saledetails.length);
            $("#item-container-print tbody").html(html);
            if (Common.isNullOrWhiteSpace(d.Comments)) {
                $("p.comment-container").addClass("hide");
            }
            else {
                $("p.comment-container").removeClass("hide");
            }
            if (Common.isNullOrWhiteSpace(d.Notes)) {
                $("p.notes-container").addClass("hide");
            }
            else {
                $("p.notes-container").removeClass("hide");
            }




        },
        WordsConversion: function (total, scope) {
            var _this = this;
            var junkVal = total
            junkVal = Math.floor(junkVal);
            var obStr = new String(junkVal);
            numReversed = obStr.split("");
            actnumber = numReversed.reverse();

            if (Number(junkVal) >= 0) {
                //do nothing
            }

            if (Number(junkVal) == 0) {
                $("#lblAmountInWords", scope).html(' ');
                return 'Zero Rupees Only'
            }
            //if (actnumber.length > 9) {
            //    alert('Oops!!!! the Number is too big to covertes');
            //    return false;
            //}

            var iWords = ["Zero", " One", " Two", " Three", " Four", " Five", " Six", " Seven", " Eight", " Nine"];
            var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', ' Nineteen'];
            var tensPlace = ['dummy', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];

            var iWordsLength = numReversed.length;
            var totalWords = "";
            var inWords = new Array();
            var finalWord = "";
            j = 0;
            for (i = 0; i < iWordsLength; i++) {
                switch (i) {
                    case 0:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        inWords[j] = inWords[j] + ' Shilling Only';
                        break;
                    case 1:
                        tens_complication();
                        break;
                    case 2:
                        if (actnumber[i] == 0) {
                            inWords[j] = '';
                        }
                        else if (actnumber[i - 1] != 0 && actnumber[i - 2] != 0) {
                            inWords[j] = iWords[actnumber[i]] + ' Hundred and';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]] + ' Hundred';
                        }
                        break;
                    case 3:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                            inWords[j] = inWords[j] + " Thousand";
                        }
                        break;
                    case 4:
                        tens_complication();
                        break;
                    case 5:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                            inWords[j] = inWords[j] + " Lakh";
                        }
                        break;
                    case 6:
                        tens_complication();
                        break;
                    case 7:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        inWords[j] = inWords[j] + " Crore";
                        break;
                    case 8:
                        tens_complication();
                        break;
                    default:
                        break;
                }
                j++;
            }

            function tens_complication() {
                if (actnumber[i] == 0) {
                    inWords[j] = '';
                }
                else if (actnumber[i] == 1) {
                    inWords[j] = ePlace[actnumber[i - 1]];
                }
                else {
                    inWords[j] = tensPlace[actnumber[i]];
                }
            }
            inWords.reverse();
            for (i = 0; i < inWords.length; i++) {
                finalWord += inWords[i];
            }
            return finalWord;
            //$("#lblAmountInWords", scope).html('Rupees' + finalWord);

        },
        Edit: function (id, voucherNo) {
            var $this = this;
            $("#VoucherNumber").val(voucherNo);
            $this.LoadVoucher("same");
        },
        CalculateReturnAmount: function () {
            var $this = this;
            var storeData = DATA;
            var d = storeData.Voucher;
            if (d != null) {
                var vehicle = storeData.Vehicle;
                if (vehicle != null) {
                    delete vehicle.Id;
                    delete vehicle.IsTradeIn;
                    delete vehicle.SalePrice;
                    $.extend(true, d, storeData.Customer);
                }
                $.extend(true, d, vehicle);
            }
            var scope = ("#model-deal-cancellation");
            var tradeVehicle = storeData.TradeVehicle;

            var saleDeposit = d.TrackerReceivedAmount + d.InsurenceReceivedAmount + d.Received;
            $("span.SaleDeposit").html(saleDeposit.format());
            var depAmount = Common.GetInt($("#DepreciationAmount").val());
            var netDeposit = saleDeposit + Common.GetIntHtml($("span.InstallmentDeposit").html());
            if (tradeVehicle != null) {
                if (!tradeVehicle.IsSale) {
                    if (!$("#IsTradeUnitReturned").is(":checked")) {
                        netDeposit = netDeposit + d.TradeInPrice;
                    }

                }
                else {
                    netDeposit = netDeposit + d.TradeInPrice;
                }
            }
            var netAmount = netDeposit - depAmount;
            $("span.lblNetDeposit").html(netDeposit.format());
            $("span.lblNetAmount").html(netAmount.format());
            $("#ReturnAmount").val(netAmount);
        },
        GetCancellitionInfo: function () {
            var $this = this;
            var voucherno = Common.GetInt($("#Id").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?loadCancellationInfo=true&strloadCancellationInfo=strloadCancellationInfo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Cancellation Info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var vehicle = "";


                        var storeData = DATA;
                        var d = storeData.Voucher;
                        if (d != null) {
                            var vehicle = storeData.Vehicle;
                            if (vehicle != null) {
                                delete vehicle.Id;
                                delete vehicle.IsTradeIn;
                                delete vehicle.SalePrice;
                                $.extend(true, d, storeData.Customer);
                            }
                            $.extend(true, d, vehicle);
                        }
                        $("#DepreciationAmount").val(0);
                        var scope = ("#model-deal-cancellation");
                        var tradeVehicle = storeData.TradeVehicle;
                        Common.MapDataWithPrefixFClasses(d, scope, "", "html");
                        Common.MapDataWithPrefixFClasses(tradeVehicle, scope, "trade", "html");
                        var saleDeposit = d.TrackerReceivedAmount + d.InsurenceReceivedAmount + d.Received + d.LogBookFee;
                        $("span.SaleDeposit").html(saleDeposit.format());
                        $("span.InstallmentDeposit").html(res.Data.PaidAmount.format());
                        var netDeposit = saleDeposit + res.Data.PaidAmount;

                        if (d.SaleType == VoucherType.vehiclecashsale) {
                            $(".tr-installemt-info").addClass("hide");
                        }
                        else {
                            $(".tr-installemt-info").removeClass("hide");
                        }

                        if (d.IsTradeIn) {
                            $(".tr-trade-info").removeClass("hide");
                        }
                        else {
                            $(".tr-trade-info").addClass("hide");
                        }
                        if (tradeVehicle != null) {
                            if (!tradeVehicle.IsSale) {

                                $("span.trade-status").addClass("label-warning").removeClass("label-danger").html("Un-Sold");
                                $(".tr-IsTradeUnitReturned").removeClass("hide");
                                $.uniform.update($("#IsTradeUnitReturned").val("true").prop("checked", true));
                            }
                            else {
                                $(".tr-IsTradeUnitReturned").addClass("hide");
                                $("span.trade-status").removeClass("label-success").addClass("label-danger").html("Sold");
                                $.uniform.update($("#IsTradeUnitReturned").val("false").prop("checked", false));
                            }
                        }
                        else {
                            $("tr-IsTradeUnitReturned").addClass("hide");
                        }
                        $this.CalculateReturnAmount();
                        $("#model-deal-cancellation").modal("show");
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        CancellDeal: function () {
            var $this = this;
            var scope = ("#model-deal-cancellation");
            if (Common.Validate($("#model-deal-cancellation"))) {
                var record = Common.SetValue(scope);
                var storeData = DATA;
                $.extend(true, storeData.Voucher, record);


                var err = "";

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?Cancellationdeal=true",
                    type: "POST",
                    data: storeData.Voucher,
                    blockUI: true,
                    blockElement: "#model-deal-cancellation",
                    blockMessage: "cancelling deal...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Deal is cancelled successfully." });
                            $("#VoucherNumber").val(storeData.VoucherNumber);
                            $this.LoadVoucher("same");
                            $("#model-deal-cancellation").modal("hide");
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

        CalculateDiscount: function () {
            var $this = this;
            var totalAmount = Common.GetInt($("#Amount").val());
            var discount = Common.GetInt($("#Discount").val());
            $("#TotalAmount").val(Common.GetInt(totalAmount - discount));
        },
        Delete: function (id, voucherNo) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=deleteSale",
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting Sale...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $this.LoadVehicles();
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

        DeleteInstallment: function (id, element) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?key=deleteInstallment",
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting installment...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Installment payment deleted successfully." });
                            $(element).closest("tr").remove();
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
            var tokens = $.parseJSON($("#jsondata #data").html());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },

        AutoCompleteInit: function () {
            var $this = this;
            var customers = Common.GetLeafAccounts(PageSetting.Customers);
            var suggestions = new Array();
            for (var i in customers) {
                var customer = customers[i];
                suggestions.push(
                    {
                        id: customer.Id,
                        value: customer.AccountCode,
                        label: customer.AccountCode + "-" + customer.DisplayName
                    }
                    );
            }

            $("#AccountCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    $("#AccountId").val(account.Id);
                    $("#AccountName").val(account.Name);
                    $this.GetBalance();
                    $this.LoadTradeInVehicle();
                }
            });
        },
        LoadSupplier: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.suppliers").html(html).select2();


        },
        VehicleAutoCompleteInit: function (vehicles) {
            var $this = this;

            var suggestions = new Array();
            for (var i in vehicles) {
                var token = vehicles[i];
                var label = "Chessis No:" + token.ChassisNo + " RegNo No:" + token.RegNo + " Manufacturer:" + token.Manufacturer;
                label += " Color:" + token.Color + " CC:" + token.EnginePower + " Model:" + token.Model + " Car TYPE:" + token.CarType;
                suggestions.push(
                    {
                        id: token.Id,
                        value: token.ChassisNo,
                        label: label,
                        Vehicle: token
                    }
                );
            }

            $("#ChassisNo").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var vehicle = Enumerable.From(vehicles).Where(function (p) { return p.Id == ui.item.id }).FirstOrDefault();
                    $("#VehicleId").val(vehicle.Id);
                    $("#VehicleName").val(vehicle.BrandName);
                    $("#Description").val(vehicle.DoM);
                    $("#RegNo").val(vehicle.RegNo);
                    $("#ChassisNo").val(vehicle.ChassisNo);
                    $("#CC").val(vehicle.EnginePower);
                    $("#EngineNo").val(vehicle.EngineNo);
                    $("#Model").val(vehicle.Model);
                    $("#Fuel").val(vehicle.Fuel);
                    $("#Colour").val(vehicle.Color);
                    $("#PurchasePrice").val(vehicle.PurchasePrice);
                    $("#SalePrice").val(vehicle.SalePrice);
                    $("#Manufacturer").val(vehicle.Manufacturer);

                    $this.GetBalance();
                    $this.LoadExpensesDetails();
                    $this.CalculateBalance();
                }
            });
        },
        GetBalance: function () {
            var $this = this;
            var type = $this.GetType();
            var customerid = Common.GetInt($("#AccountId").val());
            var vehicleId = Common.GetInt($("#VehicleId").val());
            if (vehicleId == 0 || customerid == 0)
                return;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc/?key=GetPreviousBalanceWithExludingTradeIn&accountId=" + customerid + "&vehicleId=" + vehicleId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var id = Common.GetInt($("#Id").val());
                        var balance = Math.abs(res.Data);
                        $("#lbladvance").html("0.0");
                        if (res.Data < 0) {
                            $("#lbladvance").html(Common.GetFloat(balance).format());
                            $("#Advance").val(balance);
                        }
                        $this.CalculateBalance();

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        AddItem: function () {
            var $this = this;
            //  if (Common.Validate($("#addrow"))) {
            var code = $("#item-sales-deposits-container tbody tr:nth-last-child(1) td:nth-child(1) input.Date").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-sales-deposits-container tbody tr:nth-last-child(1) td:nth-child(1) input.Date").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-sales-deposits").html();
            $("#item-sales-deposits-container tbody").append(html);
            setTimeout(function () {
                $("#item-sales-deposits-container tbody tr:nth-last-child(1) input.Date").focus().select();
            }, 300);
            Common.InitDateMask();
            Common.InitNumerics();
            $this.GetWholeReceieved();
        },
    };
}();
