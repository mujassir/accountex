var SalaryConfig = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "SalaryConfig";
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
            //$(document).on("keyup", ".BasicSalary,.HouseAllowance,.ConveyanceAllowance,.MedicalAllowance", function (event) {
            //    var tr = $(this).parent().parent();
            //    $this.CalculateGrossSalary(tr);
            //});
            $('#Month').on('select2-selecting', function (e) {
                console.log('here');
            });

            $("#FromMonth,#FromYear").change(function () { $this.SetSalaryConfigName(); });


            $("#SelectedLeaveMonth").change(function () {

                var fromyear = Common.GetInt($("#FromYear").val());
                var selectedleavemonth = Common.GetInt($("#SelectedLeaveMonth").val());

                var lastDate = $this.AddOneYear(selectedleavemonth, fromyear);
                var firstDate = $this.GetFirstDate(selectedleavemonth, fromyear);

                $("#LeaveMonthStart").val(firstDate);
                $("#LeaveBankEnd").val(lastDate);

            });

       //     $this.DetailView();
            $this.LoadPageSetting();
            $this.LoadConfig('same');
        },
        SetSalaryConfigName: function () {
            var $this = this;
            var frommonth = Common.GetInt($("#FromMonth").val());
            var fromyear = Common.GetInt($("#FromYear").val());

            var firstDate = $this.GetFirstDate(frommonth, fromyear);
            var lastDate = $this.AddOneYear(frommonth, fromyear);

            $("#FromDate").val(firstDate);
            $("#ToDate").val(lastDate);
            $("#Name").val($("#FromMonth option:selected").text() + " " + $("#FromYear").val() + " to " + lastDate);
        },
        AddOneYear: function (month, year) {
            var leavemonthend = new Date(year + 1, month - 1, 0);
            leavemonthend = moment(leavemonthend).format('MMMM YYYY');
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

            basicSalary = Common.GetInt($(tr).find(":input.BasicSalary").val());

            houseAllowance = Common.GetFloat($(tr).find(":input.HouseAllowance").val());
            conveyanceAllowance = Common.GetFloat($(tr).find(":input.ConveyanceAllowance").val());
            medicalAllowance = Common.GetFloat($(tr).find(":input.MedicalAllowance").val());

            medicalAllowance = Common.GetFloat($(tr).find(":input.MedicalAllowance").val());

            providentFund = Common.GetFloat($(tr).find(":input.ProvidentFund").val());
            eOBI = Common.GetFloat($(tr).find(":input.EOBI").val());
            sSt = Common.GetFloat($(tr).find(":input.SST").val());

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

            var totalductions = providentFund + eOBI + sSt;
            var totalallow = houseAllowance + conveyanceAllowance + medicalAllowance;

            $(tr).find(":input.TotalAllowances ").val(totalallow);
            $(tr).find(":input.TotalDeductions ").val(totalductions);

            //var grossSalary = (basicSalary + totalallow) - totalductions;
            var grossSalary = (basicSalary + totalallow);
            if (grossSalary >= 0)
                $(tr).find(":input.GrossSalary ").val(grossSalary);
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },

        LoadConfig: function (key) {
            var $this = this;
            var configId = Common.GetInt($("#Id").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + configId + "?key=" + key,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading   ...please wait",
                success: function (res) {
                    if (res.Success) {

                        if (res.Data.SalaryConfig != null) {
                            Common.MapEditData(res.Data.SalaryConfig, "#mainform");
                            $this.AddItem(res.Data.SalaryConfig.SalaryConfigItems);
                            $("#item-container tbody tr td select").each(function (index, el) { $(el).val($(el).attr("data-value")); });
                            $("#ToDate").val(moment(res.Data.SalaryConfig.ToDate).format("MMMM YYYY"));
                            $("#FromMonth").val(moment(res.Data.SalaryConfig.FromDate).format("M"));
                            $("#FromYear").val(moment(res.Data.SalaryConfig.FromDate).format("YYYY"));

                            $("#btnClone").removeAttr("disabled");
                            $("#FromMonth,#FromYear,#Name").attr("disabled", "disabled");
                        }
                        else {
                            $this.AddItem();
                            $this.SetSalaryConfigName();

                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");



                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
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

        AddItem: function (configItems) {

            var $this = this;
            var headid = PageSetting.Employees;
            var tokens = Common.GetLeafAccounts(headid);
            var html = "";
            for (var i in tokens) {
                var employee = tokens[i];
                employee["AccountId"] = employee.Id;
                var templateHtml = $("#template-item-add").html();

                if (configItems != undefined && configItems != null && configItems.length > 0) {
                    var configItem = Enumerable.From(configItems).Where(function (x) { return x.AccountId == employee.Id }).FirstOrDefault();
                    if (configItem != null)
                    {
                        templateHtml = $("#template-item-edit").html();
                        templateHtml = templateHtml.allReplace($this.GetPlaceHolder(configItem));
                    }
                }
                templateHtml = templateHtml.allReplace($this.GetPlaceHolder(employee));
                html += templateHtml;
            }
            $("#item-container tbody tr").remove();
            $("#item-container tbody").append(html);

            $("#Name").prop("disabled", false);
            $("#FromMonth").prop("disabled", false);
            $("#FromYear").prop("disabled", false);
            $("#ToMonth").prop("disabled", false);
            $("#ToYear").prop("disabled", false);
        },

        GetPlaceHolder: function (data) {
            var record = new Object();
            for (var key in data) {
                record["{{" + key + "}}"] = data[key];
            }
            return record;
        },
        DeleteRow: function (elment) {
            var $this = this;
            $(elment).parent().parent().parent().remove();
         
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
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                var scope = $("#form-info-item");
                $this.ListView();
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;

            if (Common.Validate($("#mainform"))) {

                var frommonth = Common.GetInt($("#FromMonth").val());
                var fromyear = Common.GetInt($("#FromYear").val());

                var fromDate = moment(new Date(fromyear, frommonth - 1, 1)).format('YYYY-MM-DD');
                var toDate = moment(new Date(fromyear + 1, frommonth - 1, 0)).format('YYYY-MM-DD');

                var Items = Common.SaveItemData();

                var record = {
                    Id: $("#Id").val(),
                    Name: $("#Name").val(),
                    FromDate: fromDate,
                    ToDate: toDate,
                    SalaryConfigItems: Items,
                };

                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  Salary Configration ...please wait",
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
       
        Clone: function () {
            var $this = this;
            $("#Id").val("0");
            $("#FromMonth,#FromYear,#Name").removeAttr("disabled");
            $("#btnClone").attr("disabled", "disabled");
        },
        New: function () {
            var $this = this;
            $this.CustomClear();
            $("#Id").val("0");
            $this.LoadConfig("same");
            $("#btnClone").attr("disabled", "disabled");
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            $("#FromMonth,#FromYear,#Name,#disabled").removeAttr("disabled");
            $("#btnClone").attr("disabled", "disabled");

            Common.Clear();
        },
     
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
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

                      //  $this.DetailView();
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

       

        LoadPageSetting: function () {
            var $this = this;
          
            var formSetting = $.parseJSON($("#jsondata #data").html());
          
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
          
        },
       
    };
}();

