
var RentAgreement = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "RentAgreement";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var PageSetting = new Object();
    var ordinalIndex = 1;
    var EDIT_STATE = new Object();
    return {
        init: function () {
            var $this = this;
            $("#AccountCode").focus();

            $(document).on("focus", ".date-picker", function () {
                $(".date-picker").datepicker({
                    format: 'dd/mm/yyyy',
                    autoclose: true,
                    todayHighlight: true,
                });
            });

            $(document).on("blur", "#AccountCode", function () {
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

            $(document).on("keyup", "#RentPerSqft", function () {
                var agreementPd = $("#AgreementPD").val();
                $this.DrawRentSchedule(agreementPd);
            });
            $(document).on("click", "#btn-refresh-coa", function () {
                if (confirm('Are you sure to continue..??'))
                    Common.LoadCOA(function () { location.reload(); }, true);

            });



            $(document).on("keyup blur", "#MonthlyRentArrears, #UCPercentArrears", function () {
                var monthlyArrears = Common.GetInt($("#MonthlyRentArrears").val());
                var UCPercentArrears = Common.GetInt($("#UCPercentArrears").val());
                $("#ArrearAmount").val(monthlyArrears + UCPercentArrears);
            });

            //$(document).on("keyup", ",input.Remarks", function (e) {
            //    var tr = $(this).closest("tr");
            //    var area = $(tr).find("input.Area").val();
            //    var rate = $(tr).find("input.Rate").val();
            //    var netRent = area * rate;
            //    $(tr).find("input.NetRent").val(Common.GetCeilInt(netRent));
            //    var ucpercent = Common.GetFloat($(tr).find("input.UCPercent").val())
            //    var ucAmount = (netRent * ucpercent) / 100;
            //    var total = ucAmount + netRent;
            //    $(tr).find("input.UCAmount").val(Common.GetCeilInt(ucAmount));
            //    $(tr).find("input.Total").val(Common.GetCeilInt(total));
            //    var preTr = $(tr).prev("tr");
            //    var incAmount = netRent - $(preTr).find("input.NetRent").val();
            //    $(tr).find("input.IncAmount").val(Common.GetCeilInt(incAmount));

            //    var totalAmount = $(tr).find("input.Total").val();
            //    if (e.which == 13 && totalAmount > 0) {
            //        $this.DrawRentSchedule();
            //    }
            //    $this.CalculateSecurityCharges();
            //    $this.CalculateSecurityBalance();
            //});
            $(document).on("keyup", "input.IncPercent,input.Rate,input.Area", function (e) {
                var tr = $(this).closest("tr");
                var incPercent = Common.GetFloat($(tr).find("input.IncPercent").val());
                var area = $(tr).find("input.Area").val();
                var rate = $(tr).find("input.Rate").val();

                var preTr = $(tr).prev("tr");
                var preNetRent;
                if (incPercent == 0)
                    preNetRent = area * rate;
                else
                    preNetRent = Common.GetFloat($(preTr).find("input.NetRent").val());
                //var preNetRent = area * rate;
                var newIncAmount = (incPercent * preNetRent) / 100;
                $(tr).find("input.IncAmount").val(Common.GetCeilInt(newIncAmount));
                var newNetRent = preNetRent + newIncAmount;
                $(tr).find("input.NetRent").val(Common.GetCeilInt(newNetRent));
                //var ucpercent = Common.GetFloat($(preTr).find("input.UCPercent").val());
                var ucpercent = Common.GetFloat($(tr).find("input.UCPercent").val());
                var ucAmount = (newNetRent * ucpercent) / 100;
                var total = ucAmount + newNetRent;
                $(tr).find("input.UCAmount").val(Common.GetCeilInt(ucAmount));
                $(tr).find("input.Total").val(Common.GetCeilInt(total));

                var totalAmount = $(tr).find("input.Total").val();
                if (e.which == 13 && totalAmount > 0) {
                    $this.DrawRentSchedule();
                }
                $this.CalculateSecurityCharges();
                $this.CalculateSecurityBalance();
            });
            $(document).on("keyup", "input.UCPercent", function (e) {
                var tr = $(this).closest("tr");
                var netRent = Common.GetCeilInt($(tr).find("input.NetRent").val());
                var ucpercent = Common.GetFloat($(tr).find("input.UCPercent").val())
                var ucAmount = (netRent * ucpercent) / 100;
                var total = ucAmount + netRent;
                $(tr).find("input.UCAmount").val(Common.GetCeilInt(ucAmount));
                $(tr).find("input.Total").val(Common.GetCeilInt(total));
                var preTr = $(tr).prev("tr");
                var incAmount = netRent - $(preTr).find("input.NetRent").val();
                var totalAmount = $(tr).find("input.Total").val();
                if (e.which == 13 && totalAmount > 0) {
                    $this.DrawRentSchedule();
                }
                $this.CalculateSecurityCharges();
                $this.CalculateSecurityBalance();
            });
            $(document).on("keyup", "#ExtraSecurityAmount", function () {

                $this.CalculateSecurityBalance();
            });
            $(document).on("change", "#ExtraSecurityType", function () {

                $this.CalculateSecurityBalance();
            });

            $(document).on("keyup", "#ExtraPossessionAmount", function () {

                $this.CalculatePossessionCharges();
            });
            $(document).on("change", "#ExtraPossessionType", function () {

                $this.CalculatePossessionCharges();
            });


            $(document).on("keyup", "#ReceivedSecurityAmount,#SecurityMoneyAmount", function () {
                $this.CalculateSecurityBalance();
            });

            $(document).on("keyup", "#AlreadyPaidPossessionAmount,#NotPaidPossessionAmount,#PossessionReceived", function () {
                $this.CalculatePossessionCharges();
            });
            $(document).on("keyup", "#PossessionInstallment", function () {
                var numberOfInstallment = $("#PossessionInstallment").val();
                var possessionBalance = $("#PossessionBalance").val();
                var possessionPerInstallment = possessionBalance / numberOfInstallment;
                $("#PossessionPerInstallment").val(Common.GetCeilInt(possessionPerInstallment));
            });
            $(document).on("keyup", "#SecurityInstallment", function () {
                var numberOfInstallment = $("#SecurityInstallment").val();
                var remainingBalance = $("#SecurityBalance").val();
                var securityPerInstallment = remainingBalance / numberOfInstallment;
                $("#SecurityPerInstallment").val(securityPerInstallment.toFixed(2));
                $("#SecurityRoundAmount").val(Common.GetCeilInt(securityPerInstallment));
            });
            $this.LoadPageSetting();
            $this.GetSettings();
            AppData.AccountDetail = PageSetting.AccountDetails;
            $this.AutoCompleteInit();
            $this.GetShops();
            $this.LoadTenants();

            $this.GetNextVoucherNumber();
            $("#ContactNo").inputmask("mask", { "mask": "9999-9999999" });

        },
        GetSettings: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetSettings",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",

                success: function (res) {
                    if (res.Success) {
                        var tokens = $.parseJSON(res.Data);
                        for (var i in tokens) {
                            var token = tokens[i];
                            PageSetting[token.Key] = token.Value;
                        }
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },
        CalculatePossessionInstallments: function () {
            if ($("#PossessionInstallment").val() == "") {
                $("#PossessionInstallment").val(PageSetting.SecurityMoneyMonths);
            }
            else {
                var existingPossessionInstallment = Common.GetInt($(".ExistingPossessionInstallment").val());
                var possessionInstallmentsReceived = Common.GetInt($("#PossessionInstallmentsReceived").html());
                var newInstallments = existingPossessionInstallment - possessionInstallmentsReceived;
                if (newInstallments < 1) newInstallments = 1;
                $("#PossessionInstallment").val(newInstallments);
            }
            var numberOfInstallment = $("#PossessionInstallment").val();
            var remainingBalance = $("#PossessionBalance").val();
            var possessionPerInstallment = remainingBalance / numberOfInstallment;
            $("#PossessionPerInstallment").val(Common.GetCeilInt(possessionPerInstallment));
        },
        CalculateSecurityCharges: function () {
            //var rent = Common.GetInt($("#item-container tbody").find("tr:nth-last-child(1) input.Total").val());
            //var total = Common.GetInt(rent) * PageSetting.SecurityMoneyMonths;
            //var extraSecurity = Common.GetCeilInt(total) - Common.GetCeilInt($(".SecurityMoneyAmount").val());
            //if ($("#IsRenew").val() == "1" && extraSecurity < 0) return;
            //$("#SecurityMoneyAmount").val(Common.GetCeilInt(total));
            //$("#ExtraSecurityAmount").val(extraSecurity);
        },
        CalculateSecurityBalance: function () {
            $this = this;
            //var existingSecurityMoneyAmount = Common.GetFloat($(".SecurityMoneyAmount").val());
            var existingSecurityMoneyAmount = Common.GetFloat($("#SecurityMoneyAmount").val());
            var extraSecurityAmount = Common.GetFloat($("#ExtraSecurityAmount").val());
            var type = Common.GetInt($("#ExtraSecurityType").val());
            var securityAmount = 0;
            if (type == AdjustmentType.Increase)
                securityAmount = existingSecurityMoneyAmount + extraSecurityAmount;
            else
                securityAmount = existingSecurityMoneyAmount - extraSecurityAmount;
            //$("#SecurityMoneyAmount").val(securityAmount);
            var securityMoneyAmount = $("#SecurityMoneyAmount").val();
            var receivedSecurityAmount = Common.GetCeilInt($("#ReceivedSecurityAmount").val());
            var remainingBalance = securityMoneyAmount - receivedSecurityAmount;
            if (EDIT_STATE != null)
                remainingBalance = remainingBalance - EDIT_STATE.SecurityAmountReceived;
            $("#SecurityBalance").val(remainingBalance);
            $this.CalculateSecurityInstallments();
        },
        CalculateSecurityInstallments: function () {
            if ($("#SecurityInstallment").val() == "") {
                $("#SecurityInstallment").val(PageSetting.SecurityMoneyMonths);
            }
            else {
                var existingSecurityInstallments = Common.GetInt($(".ExistingSecurityInstallment").val());
                var securityInstallmentsReceived = Common.GetInt($("#SecurityInstallmentsReceived").html());
                var newInstallments = existingSecurityInstallments - securityInstallmentsReceived;
                if (newInstallments < 1) newInstallments = 1;
                $("#SecurityInstallment").val(newInstallments);

            }
            var numberOfInstallment = $("#SecurityInstallment").val();
            var remainingBalance = $("#SecurityBalance").val();
            var securityPerInstallment = remainingBalance / numberOfInstallment;
            $("#SecurityPerInstallment").val(securityPerInstallment.toFixed(2));
            $("#SecurityRoundAmount").val(Common.GetCeilInt(securityPerInstallment));
        },
        DrawRentSchedule: function () {
            var $this = this;
            var html = "";
            var area = 0;
            var rentpersqft = 0;
            var tr = $("#item-container tr:nth-last-child(1)");
            area = $(tr).find("input.Area").val();
            var id = 0;
            area = (area == undefined ? 0 : area);
            rentpersqft = $(tr).find("input.Rate").val();
            rentpersqft = (rentpersqft == undefined ? 0 : rentpersqft);
            var rent = area * rentpersqft;
            var startDate = "";
            var endDate = "";
            var noOfRows = $("#item-container tbody tr").length;
            if (noOfRows == 0) {
                startDate = Common.GetTodayDate(2);
                endDate = new Date();
                endDate.setDate(endDate.getDate() + 364);
                endDate = moment(endDate).format("DD/MM/YYYY");
            }
            else {
                startDate = $("#item-container").find("tr").eq(-2).find("td:eq(2) input").val();
                if (startDate != undefined && startDate != "") {
                    startDate = startDate.split("/");
                    endDate = new Date(startDate[2], startDate[1] - 1, (Common.GetInt(startDate[0]) + 1)); //new Date();
                    endDate.setDate(endDate.getDate() + 364);
                    endDate = moment(endDate).format("DD/MM/YYYY");
                    startDate = (Common.GetInt(startDate[0]) + 1) + "/" + startDate[1] + "/" + startDate[2];

                }
                else {
                    startDate = Common.GetTodayDate(2);
                    endDate = new Date();
                    endDate.setDate(endDate.getDate() + 364);
                    endDate = moment(endDate).format("DD/MM/YYYY");
                }
            }
            html += "<tr>";
            html += "<td class='align-center'><input type='text' class='RentAgreementId hide'>" + ordinalIndex + "";
            html += "</td>";
            html += "<td><input type='text' class='FromDate date-picker form-control' value='" + startDate + "'><input type='text' class='Id hide' value='" + id + "'></td>";
            html += "<td><input type='text' class='ToDate date-picker form-control' value='" + endDate + "'></td>";
            html += "<td class='align-right'><input type='text' class='Area form-control' value='" + area + "'></td>";
            html += "<td class='align-right'><input type='text' class='Rate form-control' value='" + rentpersqft + "'></td>";
            html += "<td><input type='text' class='Remarks form-control'></td>";
            html += "<td class='align-right'><input type='text' class='IncPercent form-control'></td>";
            html += "<td class='align-right'><input type='text' readonly class='IncAmount form-control'></td>";
            html += "<td class='align-right'><input type='text' readonly class='NetRent form-control' value='" + Common.GetCeilInt(rent) + "'></td>";
            var ucpercent = Common.GetInt(PageSetting.UCPercent);
            var ucAmount = (rent * PageSetting.UCPercent) / 100;
            total = ucAmount + rent;
            if (noOfRows > 0) {
                var prevUCPercent = $("#item-container").find("tr").eq(-2).find("td:eq(9) input").val();
                var prevUCAmount = $("#item-container").find("tr").eq(-2).find("td:eq(10) input").val();
                if (prevUCPercent != ucpercent) {
                    html += "<td class='align-right'><input type='text' class='UCPercent form-control' value='" + prevUCPercent + "'></td>";
                    html += "<td class='align-right'><input type='text' readonly class='UCAmount form-control' value='" + prevUCAmount + "'></td>";
                    total = Common.GetFloat(prevUCAmount) + Common.GetFloat(rent); //prevUCPercent + rent;
                }
                else {
                    html += "<td class='align-right'><input type='text' class='UCPercent form-control' value='" + ucpercent + "'></td>";
                    html += "<td class='align-right'><input type='text' readonly class='UCAmount form-control' value='" + Common.GetCeilInt(ucAmount) + "'></td>";
                }
            }
            else {
                html += "<td class='align-right'><input type='text' class='UCPercent form-control' value='" + ucpercent + "'></td>";
                html += "<td class='align-right'><input type='text' readonly class='UCAmount form-control' value='" + Common.GetCeilInt(ucAmount) + "'></td>";
            }
            html += "<td class='align-right'><input type='text' class='Total form-control' value='" + Common.GetCeilInt(total) + "'></td>";
            html += "<td><span class='action'><i class='fa fa-trash-o' onclick='RentAgreement.DeleteRow(this);'></i></span></td>";
            html += "</tr>";
            ordinalIndex = ordinalIndex + 1;
            $("#item-container tbody").append(html);
            $this.ManageIndexing();
            if ($("#item-container tbody tr").length == 1) {
                $("#item-container tbody tr:nth-last-child(1) td:nth-child(7) input.IncPercent").attr("disabled", "disabled");
            }
            $this.CalculateSecurityCharges();
        },
        DeleteRow: function (tr) {
            var $this = this;
            $(tr).closest("tr").remove();
            if ($("#item-container tbody").children().length <= 0)
                $this.DrawRentSchedule();

            $this.CalculateSecurityCharges();
            $this.CalculateSecurityBalance();
            $this.ManageIndexing();
        },
        ManageIndexing: function () {
            $("#item-container tbody tr td:nth-child(1)").each(function (e) {
                $(this).text(++e);
            });
        },
        CalculatePossessionCharges: function () {
            var $this = this;
            var existingPossessionAmount = Common.GetFloat($(".TotalPossessionAmount").val());
            var extraPossessionAmount = Common.GetFloat($("#ExtraPossessionAmount").val());
            var possessionAmount = existingPossessionAmount + extraPossessionAmount;
            $("#TotalPossessionAmount").val(possessionAmount);
            var alreadyPaidAmount = Common.GetCeilInt($("#AlreadyPaidPossessionAmount").val());
            var notPaidPossessionAmount = Common.GetCeilInt($("#NotPaidPossessionAmount").val());
            var extraPossessionAmount = Common.GetCeilInt($("#ExtraPossessionAmount").val());
            var rcvdPossessionAmount = Common.GetCeilInt($("#PossessionAmountReceived").val());


            var type = Common.GetInt($("#ExtraPossessionType").val());
            var totalPossessionAmount = 0;
            if (type == AdjustmentType.Increase)
                totalPossessionAmount = alreadyPaidAmount + notPaidPossessionAmount + extraPossessionAmount;
            else
                totalPossessionAmount = alreadyPaidAmount + notPaidPossessionAmount - extraPossessionAmount;

            $("#TotalPossessionAmount").val(totalPossessionAmount);
            var possessionReceived = Common.GetCeilInt($("#PossessionReceived").val()) + rcvdPossessionAmount;
            var balance = totalPossessionAmount - possessionReceived;
            $("#PossessionBalance").val(balance);

            $this.CalculatePossessionInstallments();
        },



        OrdinalSuffixOf: function (i) {
            var j = i % 10,
                k = i % 100;
            if (j == 1 && k != 11) {
                return i + "st";
            }
            if (j == 2 && k != 12) {
                return i + "nd";
            }
            if (j == 3 && k != 13) {
                return i + "rd";
            }
            return i + "th";
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            $('#form-info').removeClass('hide');
            $('#div-table').addClass('hide');
            $(".portlet .container-message").addClass("hide");
            $("div.tools a.expand").click();
            Common.GoToTop();
        },
        ListView: function () {
            var $this = this;
            $('#form-info').addClass('hide');
            $('#div-table').removeClass('hide');
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $this.CustomClear();
        },
        New: function () {

            var $this = this;
            $this.CustomClear();
            $this.LoadVoucher("nextvouchernumber");
        },
        Renew: function () {

            var $this = this;
            $("#IsRenew").val("1");
            $("#item-container tbody").html("");
            $this.DrawRentSchedule();
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
        },
        CustomClear: function () {
            $this = this;
            $("#item-container tbody").html("");
            $("tfoot").addClass("hide");
            Common.Clear();
            $this.GetShops();
            $("#YearlyInc").val(10);
            $("#RentPerSqft").val(PageSetting.RentPerSqft);
            $("#btndelete,#btnprint").prop("disabled", true);
            $(".edit-disabled").prop("disabled", false).prop("readonly", false);
        },
        Print: function () {
            window.print();
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
        },
        LoadVoucher: function (key, voucherno) {
            var $this = this;
            if (!voucherno)
                var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {

                        EDIT_STATE = null;
                        $("#item-container tbody").html("");
                        $this.CustomClear();
                        var d = res.Data.Order;
                        var Vrecord = res.Data.VRecord;
                        var state = EDIT_STATE = res.Data.State;
                        Common.MapEditData(d, "#form-info");

                        if (d == null) {
                            $this.CustomClear();
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#savebutton").prop("disabled", false);
                            $(".edit-visible").addClass("hide");
                        }
                        else {

                            Common.MapEditData(Vrecord, "#form-info");
                            $("#Id").val(d.Id);
                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, d[$(this).attr("Id")]);
                            });
                            Common.SetCheckValue(Vrecord);
                            $(".edit-visible").removeClass("hide");
                            $(".edit-disabled").prop("disabled", true).prop("readonly", true);
                            $("#SecurityInstallmentsReceived").html(state.SecurityInstallmentsReceived);
                            $("#SecurityAmountReceived").html(state.SecurityAmountReceived);
                            $("#PossessionInstallmentsReceived").html(state.PossessionInstallmentsReceived);
                            $("#PossessionAmountReceived").val(state.PossessionAmountReceived);
                            $("#lblPossessionAmountReceived").html(state.PossessionAmountReceived);

                            $("#LastRentMonth").val(state.LastRentMonth);
                            $("#LastRentYear").val(state.LastRentYear);

                            $(".SecurityMoneyAmount").val(d.SecurityMoneyAmount);
                            $(".ExistingSecurityInstallment").val(d.SecurityInstallment);

                            $(".TotalPossessionAmount").val(d.TotalPossessionAmount);
                            $(".ExistingPossessionInstallment").val(d.PossessionInstallment);


                            var items = d.RentAgreementSchedules;
                            if (d.Id > 0 && items != null && items.length > 0) {
                                $("#btndelete,#btnprint").prop("disabled", false);
                                //$("#savebutton").prop("disabled", true);
                                Common.MapItemData(items);
                                var html = "";
                                for (var i in items) {
                                    var item = items[i];
                                    html += "<tr>";
                                    html += "<td class='align-center'><input type='text' class='RentAgreementId hide' value='" + d.Id + "'>" + (++i) + "";
                                    html += "</td>";
                                    html += "<td><input type='text' class='FromDate date-picker form-control' value='" + moment(item.FromDate).format('DD/MM/YYYY') + "'><span class='FromDate hide'>" + item.FromDate + "</span><input type='text' class='Id hide'  value='" + item.Id + "'></td>";
                                    html += "<td><input type='text' class='ToDate date-picker form-control' value='" + moment(item.ToDate).format('DD/MM/YYYY') + "'><span class='ToDate hide'>" + item.ToDate + "</span></td>";
                                    html += "<td><input type='text' class='Area form-control align-right' value='" + item.Area + "'></td>";
                                    html += "<td><input type='text' class='Rate form-control align-right' value='" + item.Rate + "'></td>";
                                    html += "<td><input type='text' class='Remarks form-control' value='" + (item.Remarks != null ? item.Remarks : '') + "'></td>";
                                    html += "<td><input type='text' class='IncPercent form-control align-right' value='" + item.IncPercent + "'></td>";
                                    html += "<td><input type='text' class='IncAmount form-control align-right' value='" + item.IncAmount + "'></td>";
                                    html += "<td><input type='text' class='NetRent form-control align-right' value='" + item.NetRent + "'></td>";
                                    html += "<td><input type='text' class='UCPercent align-right' value='" + item.UCPercent + "'></td>";
                                    html += "<td class='align-right'><input type='text' readonly class='UCAmount form-control' value='" + item.UCAmount + "'></td>";
                                    html += "<td><input type='text' readonly class='Total align-right' value='" + item.Total + "'></td>";
                                    if (state.LastRentMonth + state.LastRentYear == 0) {
                                        html += "<td><span class='action'><i class='fa fa-trash-o' onclick='RentAgreement.DeleteRow(this);'></i></span></td>";
                                    }
                                    else if ($this.IsExistInPeriod(state.LastRentMonth, state.LastRentYear, new Date(item.FromDate), new Date(item.ToDate)) > 0) {
                                        html += "<td><span class='action'><i class='fa fa-trash-o' onclick='RentAgreement.DeleteRow(this);'></i></span></td>";
                                    }
                                    else {
                                        html += "<td></td>";
                                    }
                                    html += "</tr>";
                                }
                                $("#item-container tbody").html(html);

                            }
                            //$this.CalculatePossessionCharges();
                            if ($("#AgreementPD").val()) {
                                //$("tfoot").removeClass("hide");
                            }
                            $("#SecurityRoundAmount").val(Common.GetCeilInt(d.SecurityPerInstallment));
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");

                        $this.DetailView();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        Save: function () {
            var $this = this;
            var data = new Array();
            var challanLiability = new Array();
            $(".portlet .container-message").addClass("hide");
            if (Common.Validate($("#form-info"))) {
                var RentAgreements = Common.SetValue($(".form"));
                RentAgreements["ArrearMonth"] = Common.GetInt($("#ArrearMonth").val());
                RentAgreements["ArrearYear"] = Common.GetInt($("#ArrearYear").val());
                RentAgreements["ArrearAmount"] = Common.GetFloat($("#ArrearAmount").val());

                var schedules = new Array();
                $("#item-container tbody tr").each(function () {

                    schedules.push(
                        {
                            Id: Common.GetInt($(this).find("input.Id").val()),
                            FromDate: Common.ChangeDateFormate($(this).find("input.FromDate").val()),
                            ToDate: Common.ChangeDateFormate($(this).find("input.ToDate").val()),
                            Area: $(this).find("input.Area").val(),
                            Rate: $(this).find("input.Rate").val(),
                            Remarks: $(this).find("input.Remarks").val(),
                            IncPercent: $(this).find("input.IncPercent").val(),
                            IncAmount: $(this).find("input.IncAmount").val(),
                            NetRent: $(this).find("input.NetRent").val(),
                            UCPercent: $(this).find("input.UCPercent").val(),
                            UCAmount: $(this).find("input.UCAmount").val(),
                            Total: $(this).find("input.Total").val(),
                        }
                        );

                });
                RentAgreements["RentAgreementSchedules"] = schedules;

                var Challans = new Object();
                Challans["Month"] = Common.GetInt($("#ArrearMonth").val());
                Challans["Year"] = Common.GetInt($("#ArrearYear").val());
                Challans["TransactionType"] = VoucherType["rentagreement"];
                Challans["EntryType"] = EntryType["Automatic"];
                var ChallanItems = new Array();
                ChallanItems.push(
                    {
                        MonthlyRent: $("#ArrearAmount").val(),
                        RentAgreementId: $("#RentAgreementId").val(),
                        InvoiceNumber: $("#InvoiceNumber").val(),
                        ShopId: $("#ShopId").val(),
                        ShopNo: $("#ShopNo").val(),
                        TenantAccountId: $("#TenantAccountId").val(),
                        TenantName: $("#TenantName").val(),
                        TenantCode: $("#AccountCode").val(),
                        TransactionType: VoucherType["rentagreement"],
                        EntryType: EntryType["Automatic"],

                    });
                Challans["ChallanItems"] = ChallanItems;

                challanLiability.push(Challans);

                var paidPosChallanItems = new Array();
                var paidPosChallan = new Object();
                paidPosChallan["TransactionType"] = VoucherType["possessioncharges"];
                paidPosChallan["EntryType"] = EntryType["Automatic"];
                //paidPosChallan["NumberOfInstallment"] = $("#PossessionInstallment").val();

                var paidPosChallanItems = new Array();
                paidPosChallanItems.push(
                    {

                        RentAgreementId: $("#RentAgreementId").val(),
                        InvoiceNumber: $("#InvoiceNumber").val(),
                        ShopId: $("#ShopId").val(),
                        ShopNo: $("#ShopNo").val(),
                        TenantAccountId: $("#TenantAccountId").val(),
                        TenantName: $("#TenantName").val(),
                        TenantCode: $("#AccountCode").val(),
                        TransactionType: VoucherType["possessioncharges"],
                        EntryType: EntryType["Automatic"],
                        Amount: $("#TotalPossessionAmount").val(),

                    });


                paidPosChallan["ChallanItems"] = paidPosChallanItems;
                challanLiability.push(paidPosChallan);



                var securityChallan = new Object();
                var securityChallanItems = new Array();
                securityChallan["TransactionType"] = VoucherType["securitymoney"];
                securityChallan["EntryType"] = EntryType["Automatic"];
                //  securityChallan["NumberOfInstallment"] = $("#SecurityInstallment").val();

                var securityChallanItems = new Array();
                securityChallanItems.push(
                    {

                        RentAgreementId: $("#RentAgreementId").val(),
                        InvoiceNumber: $("#InvoiceNumber").val(),
                        ShopId: $("#ShopId").val(),
                        ShopNo: $("#ShopNo").val(),
                        TenantAccountId: $("#TenantAccountId").val(),
                        TenantName: $("#TenantName").val(),
                        TenantCode: $("#AccountCode").val(),
                        TransactionType: VoucherType["securitymoney"],
                        EntryType: EntryType["Automatic"],
                        Amount: $("#SecurityMoneyAmount ").val(),

                    });


                securityChallan["ChallanItems"] = securityChallanItems;
                challanLiability.push(securityChallan);

                //for security receive challan
                if (RentAgreements.ReceivedSecurityAmount > 0) {
                    var receiveSecurityChallan = new Object();
                    var receiveSecurityChallanItems = new Array();
                    receiveSecurityChallan["TransactionType"] = VoucherType["securitymoney"];
                    receiveSecurityChallan["EntryType"] = EntryType["Automatic"];
                    receiveSecurityChallanItems.push(
                        {
                            RentAgreementId: $("#RentAgreementId").val(),
                            InvoiceNumber: $("#InvoiceNumber").val(),
                            ShopId: $("#ShopId").val(),
                            ShopNo: $("#ShopNo").val(),
                            TenantAccountId: $("#TenantAccountId").val(),
                            TenantName: $("#TenantName").val(),
                            TenantCode: $("#AccountCode").val(),
                            TransactionType: VoucherType["securitymoney"],
                            EntryType: EntryType["Automatic"],
                            Amount: $("#ReceivedSecurityAmount").val(),
                            IsReceived: true,
                        });
                    receiveSecurityChallan["ChallanItems"] = receiveSecurityChallanItems;
                    challanLiability.push(receiveSecurityChallan);
                }

                //for possession receive challan
                if (RentAgreements.PossessionReceived > 0) {
                    var receivePossessionChallan = new Object();
                    var receivePossessionChallanItems = new Array();
                    receivePossessionChallan["TransactionType"] = VoucherType["possessioncharges"];
                    receivePossessionChallan["EntryType"] = EntryType["Automatic"];
                    receivePossessionChallanItems.push(
                        {
                            RentAgreementId: $("#RentAgreementId").val(),
                            InvoiceNumber: $("#InvoiceNumber").val(),
                            ShopId: $("#ShopId").val(),
                            ShopNo: $("#ShopNo").val(),
                            TenantAccountId: $("#TenantAccountId").val(),
                            TenantName: $("#TenantName").val(),
                            TenantCode: $("#AccountCode").val(),
                            TransactionType: VoucherType["possessioncharges"],
                            EntryType: EntryType["Automatic"],
                            Amount: $("#PossessionReceived").val(),
                            IsReceived: true,
                        });
                    receivePossessionChallan["ChallanItems"] = receivePossessionChallanItems;
                    challanLiability.push(receivePossessionChallan);
                }

                var record = new Object();
                record["ChallanLiability"] = challanLiability;
                record["RentAgreements"] = RentAgreements;
                record["ExtraSecurityAmount"] = Common.GetCeilInt($("#ExtraSecurityAmount").val());
                record["ExtraSecurityType"] = Common.GetInt($("#ExtraSecurityType").val());
                record["ExtraPossessionAmount"] = Common.GetCeilInt($("#ExtraPossessionAmount").val());
                record["ExtraPossessionType"] = Common.GetInt($("#ExtraPossessionType").val());
                record["IsRenew"] = $("#IsRenew").val() == "1" ? true : false;

                //var visitResults = $.merge(ParameterResults, ValveResults);
                //var VisitResults = $.merge(visitResults, ChambersResults);
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Rent Agreement ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.ListView();
                            $this.CustomClear();
                            $this.GetShops();
                            $this.GetNextVoucherNumber();
                            Common.ShowMessage(true, { message: "Rent Agreement Saved Successfully!" });
                            $("#AccountCode").focus();
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
        Delete: function (id, voucherNumber) {
            var $this = this;
            if (!id)
                var id = $("#Id").val();
            if (!voucherNumber)
                var voucherNumber = $("#VoucherNumber").val();
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?VoucherNumber=" + voucherNumber,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            $this.GetNextVoucherNumber();
                            Common.ShowMessage(true, { message: "Rent Agreement Deleted Successfully!" });
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

        GetShops: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=GetAllShops",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        $this.ShopsAutoCompleteInit(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },

        AutoCompleteInit: function () {
            var tenants = Common.GetLeafAccounts(PageSetting.Tenants);
            var suggestions = new Array();
            for (var i in tenants) {
                var tenant = tenants[i];
                suggestions.push
                    (
                    {
                        id: tenant.AccountId,
                        value: tenant.AccountCode,
                        label: tenant.AccountCode + "(" + tenant.Name + ")",
                    }
                    );
            }

            $("#AccountCode").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Common.GetByCode(ui.item.value);
                    if (typeof account != "undefined" && account != null) {
                        var tenant = Common.GetAccountDetailByAccountId(account.Id);
                        $("#TenantAccountId").val(account.Id);
                        $("#TenantName").val(tenant.Name);
                        $("#Business").val(tenant.BrandName);
                        $("#Brand").val(tenant.Brand);
                        $("#ContactNo").val(tenant.ContactNumber);
                        $("#NumberOfPartners").val(tenant.NumberOfPartners);

                        if (tenant.NumberOfPartners > 0) {
                            $("#Single").val("No");
                            $("#Partnership").val("Yes");
                        }
                        else {
                            $("#Single").val("Yes");
                            $("#Partnership").val("No");
                        }


                    }
                }
            });
        },

        LoadTenants: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Tenants);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.tenants").html(html).select2();


        },
        RemoveUnpaidMonths: function () {
            var $this = this;
            var LastRentMonth = Common.GetInt($("#LastRentMonth").val());
            var LastRentYear = Common.GetInt($("#LastRentYear").val());
            if (LastRentMonth + LastRentYear == 0) {
                $("#item-container tbody tr:gt(0)").remove();
                $("#item-container tbody tr input:not(.date-picker,.Remarks)").val("0");
                $("#item-container tbody tr input.Remarks").val("");
            }
            else {
                $("#item-container tbody tr ").each(function (index, tr) {
                    var fromDate = new Date($(tr).find("span.FromDate").html());
                    var toDate = new Date($(tr).find("span.ToDate").html());
                    var result = $this.IsExistInPeriod(LastRentMonth, LastRentYear, fromDate, toDate);
                    if (result == 0) {
                        $(tr).find("input.ToDate").val(moment(new Date(LastRentYear, LastRentMonth - 1, toDate.getDate())).format("DD/MM/YYYY"));
                    }
                    else if (result == 1) {
                        $(tr).remove();
                    }
                });
            }
            $("#RemoveUnpaidMonths").addClass("hide");

        },
        IsExistInPeriod: function (month, year, startDate, endDate) {
            var inputDate = new Date(year, month, 0);
            var result;
            if (startDate <= inputDate && inputDate <= endDate) result = 0;
            else if (inputDate > endDate) result = -1;
            else
                result = 1;
            return result;
        },
        ShopsAutoCompleteInit: function (shops) {
            $this = this;
            var suggestions = new Array();
            for (var i in shops) {
                var shop = shops[i];
                suggestions.push
                    (
                    {
                        id: shop.Id,
                        value: shop.ShopNo,
                        label: shop.ShopNo + (shop.Block != null ? "(" + shop.Block + ")" : ''),
                    }
                    );
            }

            $("#ShopNo").autocomplete({
                source: suggestions,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var account = Enumerable.From(shops).Where("$.Id=='" + ui.item.id + "'").FirstOrDefault();
                    if (typeof account != "undefined" && account != null) {
                        $("#ShopId").val(account.Id);
                        $("#Area").val(account.TotalArea);
                        $("#Block").val(account.Block);
                        $("#North").val(account.North);
                        $("#South").val(account.South);
                        $("#East").val(account.East);
                        $("#West").val(account.West);
                        $("#item-container tbody").html("");
                        var agreementPd = $("#AgreementPD").val();
                        RentAgreement.DrawRentSchedule();
                        //$("tfoot").removeClass("hide");
                    }
                }
            });
        },
        GetTransferInfo: function (id) {

            var $this = this;
            var id = Common.GetInt($("#Id").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=GetTransferInfo&agreementId=" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#model-transfe",
                blockMessage: "Loading transfer Info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = ("#model-transfer");
                        var data = res.Data;
                        var agreement = data.Order;
                        var state = data.State;
                        $("#Id", scope).val(id);
                        if (agreement != null) {

                            agreement["SecurityAmountReceived"] = state.SecurityAmountReceived;
                            agreement["PossessionAmountReceived"] = state.PossessionAmountReceived;
                            agreement.SecurityBalance = agreement.SecurityBalance - agreement.SecurityAmountReceived;
                            agreement.PossessionBalance = agreement.PossessionBalance - agreement.PossessionAmountReceived;

                            //$("#SecurityInstallmentsReceived").html(state.SecurityInstallmentsReceived);
                            //$("#SecurityAmountReceived").html(state.SecurityAmountReceived);
                            //$("#PossessionInstallmentsReceived").html(state.PossessionInstallmentsReceived);
                            //$("#PossessionAmountReceived").html(state.PossessionAmountReceived);
                        }

                        var Vrecord = res.Data.VRecord;

                        Common.MapDataWithPrefixFClasses(agreement, scope, "", "html");
                        Common.MapDataWithPrefixFClasses(Vrecord, scope, "", "html");
                        $("#model-transfer").modal("show");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });


        },
        Transfeer: function () {
            var $this = this;
            var id = Common.GetInt($("#Id").val());
            if (Common.Validate($("#model-transfer"))) {
                var record = Common.SetValue("#model-transfer");
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?transfeer=true&id=" + id,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#model-transfer",
                    blockMessage: "Transfeering ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Transfeer Completed successfully." });
                            LIST_CHANGED = true;
                            $this.ListView();
                            $this.CustomClear();
                            $("#model-transfer").modal("hide");
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
    };
}();
