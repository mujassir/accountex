var SalaryBankReconciliation = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "SalaryBankReconciliation";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "code";
    var recordtype = "Save";
    var month = 0;
    var year = 0;
    return {
        init: function () {
            var $this = this;
            $("#chkAll").change(function () {
                var checked = $("#chkAll").is(":checked");
                $("#approvesalary-container tbody input[type='checkbox']").each(function () {

                    $(this).prop("checked", checked);
                    $.uniform.update($(this));
                });
            });

            $(document).on("change", "#approvesalary-container tbody input[type='checkbox']", function () {
                $this.UpdateCheckbox();
            });

            $("#Month,#Year").change(function () {

                month = $("#Month").val();
                year = $("#Year").val()
                var date = $("#Date").val();
                $this.GetNextVoucherNumber();

            });

            $this.SetCurrentMonthYear();
            $this.GetNextVoucherNumber();
            $this.LoadPageSetting();
            AppData.AccountDetail = PageSetting.AccountDetails;
        },

        UpdateCheckbox: function () {
            if ($('#approvesalary-container tbody').children().length <= 0 || $('#approvesalary-container tbody :checkbox:not(:checked)').length > 0) {
                $.uniform.update($("#chkAll").prop("checked", false));
            }
            else {
                $.uniform.update($("#chkAll").prop("checked", true));
            }
        },

        SetCurrentMonthYear: function () {
            $this = this;
            var today = new Date();
            month = Common.GetInt(today.getMonth()) + 1;
            year = today.getFullYear();
            $("#Month").val(month);
            $("#Year").val(year);
            $("#Date").val(today);
        },
       
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
     
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
      
        Add: function () {
            $this = this;
            $this.CustomClear();
            $(".container-message").hide();
            $(".container-message").hide();
            $("#Name").prop("disabled", false);
            $("#Month").prop("disabled", false);
            $("#Year").prop("disabled", false);
        },
        ListView: function () {
            var $this = this;
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (LIST_LOADED) {
                if (LIST_CHANGED) {
                    this.RebindData();
                    LIST_CHANGED = false;
                }
            } else {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        AddItem: function () {

            var $this = this;
            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);
                SetFocus = "code";
                return;
            }

            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            $.uniform.update();
            if (SetFocus == "date") {
                setTimeout(function () {
                    $("#Date").focus();
                }, 300);
            }
            else {
                setTimeout(function () {
                    $("#EmployeeLeavesSetUp tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
                }, 300);
            }
            SetFocus = "code";
            $("#qty,#Rate,#Amount").val("");
            $("#lbldiscount").html("0 %");
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitDateMask();
            Common.InitNumerics();
        },
       
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#EmployeeLeavesSetUp tbody").children().length <= 0)
                $this.AddItem();
        },
      
        Post: function () {
            var $this = this;
            App.blockUI({
                target: $("#mailsalary-container"),
                message: "Processing and sending email please wait.",
                boxed: true,
                zIndex: 20050
                //iconOnly: true
            });
            setTimeout(function () {

              
                App.unblockUI($("#mailsalary-container"));
                Common.ShowMessage(true, { message: "Salaries are posted and emailed to bank." });
            }, 8000);


          
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                SetFocus = "date";
                $this.GetNextVoucherNumber();
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

            $this = this;
            var qs = "?key=Approve";
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {

                $("#approvesalary-container tbody tr").each(function () {

                    if ($(this).find("td:nth-child(1) input").is(":checked")) {

                        var salaryItemid = $(this).children(":nth-child(2)").find("input.Id").val();
                        var accountid = $(this).children(":nth-child(2)").find("input.AccountId").val();

                        Items.push({
                            Id: salaryItemid,
                            AccountId: accountid
                        });

                    }
                });

                var err = "";
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["ESalary"] = Items;
                record["SalaryItems"] = Items;
                record.RecordType = recordtype;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    // url: Setting.APIBaseUrl + API_CONTROLLER,
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  Approve Salary...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                            $this.CustomClear();
                            App.initUniform();

                            //   callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
      

        ReviewAgain: function (callback) {

            var $this = this;
            var qs = "?key=ReviewAgain";
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {

                $("#approvesalary-container tbody tr").each(function () {

                    if ($(this).find("td:nth-child(1) input").is(":checked")) {

                        var salaryItemid = $(this).children(":nth-child(2)").find("input.Id").val();

                        Items.push({
                            Id: salaryItemid,
                        });

                    }
                });

                var err = "";
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["ESalary"] = Items;
                record["SalaryItems"] = Items;
              //  record.RecordType = recordtype;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  Approve Salary...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                            $this.CustomClear();
                            App.initUniform();

                            //   callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },


        CustomClear: function () {
            $.uniform.update();
            $("#item-container tbody").html("");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
     
        LoadVoucher: function (key) {
            var $this = this;

            var qs = "?key=" + key;
            qs += "&month=" + month;
            qs += "&year=" + year;

           
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + qs + "&voucher=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  ...please wait",
                success: function (res) {
                    if (res.Success) {

                        $this.CustomClear();

                        $("#VoucherNumber").val(res.Data.VoucherNumber);

                        $("#approvesalary-container tbody").html("");
                        $("#mailsalary-container tbody").html("");
                        for (var i = 0; i < res.Data.MailSalaries.length; i++) {
                            var mailsalaries = res.Data.MailSalaries[i];
                            
                            var mailItems = Enumerable.From(mailsalaries.SalaryItems).Where(function (x) { return x.Status == 3 }).ToArray();//status =2 mean Emailtobank
                            if (mailItems.length > 0) {
                                for (var items in mailItems) {
                                    var item = mailItems[items]
                                    item["VoucherNumber"] = mailsalaries.VoucherNumber;
                                    item["PaymentDate"] = moment(mailsalaries.PaymentDate).format("DD/MM/YYYY");
                                    item["Status"] = "EmailToBank";
                                }
                                Common.MapItemData(mailItems, "#approvesalary-container", "#template-approvesalary", true);
                            }

                        
                         

                        }
                        for (var i = 0; i < res.Data.MailProcessed.length; i++) {
                            var mailprocessed = res.Data.MailProcessed[i];

                            var mailprocessed = Enumerable.From(mailprocessed.SalaryItems).Where(function (x) { return x.Status == 4 }).ToArray();//status =0 mean Mail Processed
                            for (var items in mailprocessed) {
                                var item = mailprocessed[items];
                                item["Status"] = "MailProcessed";
                            }
                            Common.MapItemData(mailprocessed, "#mailsalary-container", "#template-mailsalary", true);
                        }

                        App.initUniform();
                        Common.InitNumerics();
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
            var type = VoucherType[$this.GetType()];

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + type + "&voucher=" + voucherno;
                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting   ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.GetNextVoucherNumber();
                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                    }
                });
            });
        },
        Cancel: function () {
            var $this = this;
            // $this.LoadOpeningBalances();
            $("#btnEdit,span.notEdit,span.notEdit").removeClass("hide");
            $("#btnSave,#btnCancel,.canEdit").addClass("hide");
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
                        var employeesalarys = res.Data.EmployeeOverTimeHours;
                        var employee = res.Data.Employee;

                        $("#item-container tbody").html("");
                        for (var i in employeesalarys) {
                            var employeeIncome = employeesalarys[i];
                            name = employeeIncome.Name;
                            //var childaccounts = $.grep(employee, function (e) { return e.Id == employeeIncome.AccountId });   
                            var empInfo = Enumerable.From(employee).Where(function (x) { return x.Id == employeeIncome.AccountId }).FirstOrDefault();
                            employeeIncome["Code"] = empInfo.AccountCode;
                            var templateHtml = $("#template-item").html();
                            //var placeholder = $this.GetPlaceHolder(employeeIncome);
                            //templateHtml = templateHtml.allReplace(placeholder);
                            html = templateHtml;
                            $("#item-container tbody").append(html);
                            for (var key in employeeIncome) {
                                var element = $("#item-container tbody tr:last").find("." + key);
                                $(element).val(employeeIncome[key]);
                            }
                        }
                        $("#Name").val(name);
                        $("#Name").prop("disabled", true);
                        $("#Month").prop("disabled", true);
                        $("#Year").prop("disabled", true);
                        //$("#ToMonth").prop("disabled", true);
                        //$("#ToYear").prop("disabled", true);
                    //    $this.DetailView();
                        //  $this.LoadPageSetting();
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
            $this = this;
        //    var voucher = this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            //$this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function (el) {
            var tr = $(el).parent().parent();
            var code = $(tr).find("input.AccountCode").val();
            var accountId = $(tr).find("input#AccountId").val();
            var name = $(tr).find("input.Name").val();
            var designation = $(tr).find("td:nth-child(5)").text().trim();
            var department=$(tr).find("td:nth-child(4)").text().trim();
            var basicsalary = Common.GetFloat($(tr).find("input.BasicSalary").val());
            var absentdeduction = Common.GetInt($(tr).find("input.AbsentDeduction").val());
            var otamount = Common.GetFloat($(tr).find("input.OTAmount").val());
            var bonus = Common.GetFloat($(tr).find("input.Bonus").val());
          //  var Employee = Common.GetByCode(code);
            var EmployDetail = Common.GetAccountDetailByAccountId(accountId);

            var houseallowance = Common.GetFloat($(tr).find("input.HouseAllowance").val());
            var conveyanceallowance = Common.GetFloat($(tr).find("input.ConveyanceAllowance").val());
            var medicalallowance = Common.GetFloat($(tr).find("input.MedicalAllowance").val());

            if (houseallowance < 1) houseallowance = houseallowance * basicsalary;
            if (conveyanceallowance < 1) conveyanceallowance = conveyanceallowance * basicsalary;
            if (medicalallowance < 1) medicalallowance = medicalallowance * basicsalary;

            var tallow = houseallowance + conveyanceallowance + medicalallowance;

            //var pf = Common.GetInt($(el).parent().parent().find("td:nth-child(10) input.ProvidentFund").val());
            //var eobi = Common.GetInt($(el).parent().parent().find("td:nth-child(11) input.EOBI").val());
            //var sst = Common.GetInt($(el).parent().parent().find("td:nth-child(12) input.SST").val());
            //var tDeduction = pf + eobi + sst;

            var providentfund = Common.GetFloat($(tr).find("input.ProvidentFund").val());
            var socialsecurity = Common.GetFloat($(tr).find("input.SST").val());
            var eobi = Common.GetFloat($(tr).find("input.EOBI").val());

            if (providentfund < 1) providentfund = providentfund * basicsalary;
            if (socialsecurity < 1) socialsecurity = socialsecurity * basicsalary;
            if (eobi < 1) eobi = eobi * basicsalary;
            var tDeduction = providentfund + socialsecurity + eobi;

            var instal = Common.GetFloat($(tr).find("input.Installment").val());
            var incometax = Common.GetFloat($(tr).find("input.IncomeTax").val());
            var netsalary = Common.GetFloat($(tr).find("input.NetSalary").val());
          
            var tCredit = basicsalary + otamount + tallow + bonus;
            var tDebit = absentdeduction + tDeduction + incometax + instal;

            $("#lblCode").text(code);
            $("#lblName").text(name);

            if (EmployDetail.BankName == null)
                $("#lblBankName").text("N/A");
            else
                $("#lblBankName").text(EmployDetail.BankName);
            if (EmployDetail.AccountTitle == null)
                $("#lblAccountTitle").text("N/A");
            else
                $("#lblAccountTitle").text(EmployDetail.AccountTitle);
            if (EmployDetail.AccountNumber == null)
                $("#lblAccountNumber").text("N/A");
            else
                $("#lblAccountNumber").text(EmployDetail.AccountNumber);

            $("#lblDesignation").text(designation);
            $("#lblBonus").text(bonus.format());

            $("#lblSummaryBasicSalary").text(basicsalary.format());
            $("#lblAbsentsCost").text(absentdeduction.format());
            $("#lblTotalOTCost").text(otamount.format());
            $("#lblTotalAllowances").text(tallow.format());

            $("#lblSummaryDeductions").text(tDeduction.format());
            $("#lblIncomeTax").text(incometax.format());
            $("#lblInstallment").text(instal.format());

            $("#lblTotalCredit").text(tCredit.format());
            $("#lblTotalDebit").text(tDebit.format());
            $("#lblNetSalary").text(netsalary.format());

            window.print();
        },
    };
}();
