
var SalaryList = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Salary";
    var LIST_LOADED = false;
    return {
        init: function () {
            $("select").select2();
           var $this = this;
            //if (Setting.PageLandingView == "DetailView") {
            //    $this.Add();
            //}
            //else {
            //$this.ListView();
            //}
            $this.GetNextVoucherNumber();
            $("#MedicalSecurity,#EOBI,#Insurance,#ProvidentFund").keyup(function () {
                $this.CalculateBD();
            });
            $("#OTBasicSalary,#SummaryBasicSalary,#BasicSalary").keyup(function () {
                $("#OTBasicSalary,#SummaryBasicSalary,#BasicSalary").val($(this).val());
                $this.CalculateAllowances();
                $this.CalculateBD();
                $this.CalculateNetCost();
                $this.CalculateNetOT();
                $this.CalculateSummary();
                $this.CalculateTax();
            });
            $("#HouseRent,#Medical,#Etc").keyup(function () {
                $this.CalculateAllowances();
            });
            $("#AdvanceBalance,#Installment").keyup(function () {
                var advance = Common.GetFloat($("#AdvanceBalance").val());
                var installment = Common.GetFloat($("#Installment").val());
                var balance = advance - installment;
                if (balance < 0) bal = 0;
                $("#NetBalance").val(balance);
                $this.CalculateTotalDeductions();
                $this.CalculateSummary();
            });
            $("#OTBasicSalary,#OTTotalWorkingDays").keyup(function () {
                var otBasicSalary = Common.GetFloat($("#OTBasicSalary").val());
                var otTotalWorkingDays = Common.GetFloat($("#OTTotalWorkingDays").val());
                if (otTotalWorkingDays <= 0) otTotalWorkingDays = 30;
                var perHourCost = parseFloat(otBasicSalary / otTotalWorkingDays / 8).toFixed(0);
                $("#OTPerHourCost,#OTHourlyRate").val(perHourCost);
                $this.CalculateNetOT();
            });
            $("#OTHours,#OTHourlyRate").keyup(function () {
                var hours = Common.GetFloat($("#OTHours").val());
                var rate = Common.GetFloat($("#OTHourlyRate").val());
                var cost = hours * rate;
                $("#OTCost").val(cost);
                $this.CalculateNetOT();
            });
            $("#GHHours,#GHHourlyRate").keyup(function () {
                var hours = Common.GetFloat($("#GHHours").val());
                var rate = Common.GetFloat($("#GHHourlyRate").val());
                var cost = hours * rate;
                $("#GHCost").val(cost);
                $this.CalculateNetOT();
            });
            $("#SOTHours,#SOTHourlyRate").keyup(function () {
                var hours = Common.GetFloat($("#SOTHours").val());
                var rate = Common.GetFloat($("#SOTHourlyRate").val());
                var cost = hours * rate;
                $("#SOTCost").val(cost);
                $this.CalculateNetOT();
            });
            $("#Absents").keyup(function () {
                $this.CaculateAbsentCost();
            });
            $(".leaves input").keyup(function () {
                $this.CalculateBalanceLeaves();
            });

            //$this.GetClients();
            $this.GetEmployees();
            $("#AccountId").change(function () {
                $this.LoadEmployeeInfo($(this).val());
            });
            $("#Filter_Month,#Filter_Year").change(function () {
                $this.Filter();
            });
            $this.Filter();
        },
        Filter: function () {
            $("#div-table").removeClass("hide");
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            url += "?month=" + $("#Filter_Month").val();
            url += "&year=" + $("#Filter_Year").val();
            url += "&isProcessed=true";
            if (!LIST_LOADED)
                DataTable.BindDatatable(DATATABLE_ID, url);
            else
                DataTable.RefreshDatatableUrl(DATATABLE_ID, url);
            LIST_LOADED = true;
        },
        CalculateBalanceLeaves: function () {
           var $this = this;
            var totalAllowed = 0;
            var totalAvailed = 0;
            var totalBalance = 0;
            $(".leaves tbody tr:not(tr.total)").each(function () {
                var allowed = Common.GetInt($(this).find(".allowed").val());
                var availed = Common.GetInt($(this).find(".availed").val());
                var balance = allowed - availed;
                totalAllowed += allowed;
                totalAvailed += availed;
                totalBalance += balance;
                $(this).find(".balance").val(balance);
            });
            $(".leaves tbody tr.total .allowed").val(totalAllowed);
            $(".leaves tbody tr.total .availed").val(totalAvailed);
            $(".leaves tbody tr.total .balance").val(totalBalance);
        },
        CaculateAbsentCost: function () {
            this.CalculateTotalDeductions();
            var hours = Common.GetFloat($("#Absents").val());
            var rate = Common.GetFloat($("#OTPerHourCost").val());
            var deductions = Common.GetFloat($("#SummaryDeductions").val());
            var cost = hours * rate * 8;
            $("#AbsentsCost").val(cost);
            deductions = deductions + cost;
            $("#SummaryDeductions").val(deductions);
            this.CalculateSummary();
        },
        CalculateSummary: function () {
           var $this = this;
            var salary = 0.0;
            $("#SummaryBasicSalary,#SummaryAllowances,#SummaryOT").each(function () {
                salary += Common.GetFloat($(this).val());
            });
            debugger;
            var summaryOt = Common.GetFloat($("#SummaryOT").val());
            var summaryDeductions = Common.GetFloat($("#SummaryDeductions").val());
            var grossSalary = salary - summaryDeductions;
            var incomeTax = $this.CalculateIncomeTax(grossSalary - summaryOt, summaryOt);
            var netSalary = grossSalary - incomeTax;
            $("#SummarySalary").val(grossSalary);
            $("#IncomeTax").val(incomeTax);
            $("#NetSalary").val(netSalary);
        },
        CalculateIncomeTax: function (monthlySalary, overTime) {
            var yearlySalary = monthlySalary * 12 + overTime;
            var tax = 0.0, taxableAmount;
            if (yearlySalary > 750000) {
                taxableAmount = yearlySalary - 750000;
                tax = 17500 + taxableAmount * 0.10;
            }
            else if (yearlySalary > 400000) {
                taxableAmount = yearlySalary - 400000;
                tax = taxableAmount * 0.05;
            }
            var monthlyTax = (tax / 12).toFixed(0);

            return monthlyTax;
        },
        CalculateNetOT: function () {
            var sum = 0.0;
            $("#OTCost,#GHCost,#SOTCost").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#TotalOTCost,#SummaryOT").val(sum);
            this.CalculateSummary();
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
        },
        ListView: function () {
            //$('#form-info').addClass('hide');
            //$('#div-table').removeClass('hide');
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        GetNextVoucherNumber: function () {
           var $this = this;
            var d = "type=Salary";
            Common.WrapAjax({
                url: "../Transaction/GetNextVoucherNumber",
                type: "POST",
                data: d,
                success: function (res) {
                    var q = JSON.parse(res);
                    if (q.Success) {
                        $("#VoucherNumber").val(q.Data);
                        $("#lblVoucherNumber").html(q.Data);
                    }
                    else {
                        ShowError(q.Data);
                    }
                },
                error: function (e) {
                }
            });
        },
        CalculateTotalDeductions: function () {
            var sum = 0.0;
            $("#SocialSecuirty,#EOBI,#Insurance,#ProvidentFund,#Installment").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#SummaryDeductions").val(sum);
        },
        CalculateBD: function () {
            var sum = 0.0;
            $("#MedicalSecurity,#EOBI,#Insurance,#ProvidentFund").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#TotalBD").val(sum);
            this.CalculateTotalDeductions();
            this.CalculateSummary();
        },
        CalculateAllowances: function () {
            var sum = 0.0;
            $("#HouseRent,#Medical,#Etc").each(function () {
                sum += Common.GetFloat($(this).val());
            });
            $("#TotalAllowances,#SummaryAllowances").val(sum);
            this.CalculateSummary();
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
        },

        Save: function () {
           var $this = this;
            var record = Common.SetValue("#form-info");
            record.Id = 0;
            record.Name = $("#AccountId option:selected").text();

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Saving salary List ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.ListView();
                        Common.ShowMessage(true, { message: Messages.RecordSaved });
                        DataTable.RefreshDatatable(DATATABLE_ID);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        LoadEmployeeInfo: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Employee" + "/" + id + "?type=account",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        Common.MapEditData(res.Data, "#form-info");
                        // Common.MapDataWithPrefix(res.Data, "#form-info","","html");
                        $("#OTBasicSalary,#SummaryBasicSalary").val($("#BasicSalary").val());
                        $this.CalculateSummary();
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
        Process: function (id) {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + $("#Filter_Month").val(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Salaries processed successfully!" });
                        $this.Filter();
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
                    blockElement: "#div-form",
                    blockMessage: "Deleting salarylist ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
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

        GetClients: function () {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetEmployees: function () {
           var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetEmployees",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
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
