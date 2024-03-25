var GenerateSalary = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "GenerateSalary";
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
                $("#item-container tbody input[type='checkbox']").each(function () {

                    $(this).prop("checked", checked);
                    $.uniform.update($(this));
                });
            });

            $(document).on("change", "#item-container tbody input[type='checkbox']", function () {
                $this.UpdateCheckbox();
            });

            $("#Month,#Year").change(function () {

                month = $("#Month").val();
                year = $("#Year").val()
                var date = $("#Date").val();
                $this.GetNextVoucherNumber();

            });

            var today = new Date();
            month = Common.GetInt(today.getMonth()) + 1;
            year = today.getFullYear();
            $("#Month").val(month);
            $("#Year").val(year);
            $("#Date").val(today);

          
            $(document).on('keyup', '.Code', function (event) {
                if (event.which == 13) {
                    $(this).parent().parent().find("td:nth-child(3) input.DepartmentId").focus();
                }
            });

           
            //$(document).on("keyup", ".ProvidentFund,.SST,.EOBI", function (event) {
            //    var tr = $(this).parent().parent();
            //    $this.CalculateGrossSalary(tr);
            //    $this.CalculateNetSalary(tr);
            //});

            //$(".Bonus").keyup(function (e) {
            //    var tr = $(this).parent().parent();
            //    $this.CalculateNetSalary(tr);
            //});
            //$(".Installment").keyup(function (e) {
            //    var tr = $(this).parent().parent();
            //    $this.CalculateNetSalary(tr);
            //});
            //$(".Installment").keyup(function (e) {
            //    var tr = $(this).parent().parent();
            //    $this.CalculateNetSalary(tr);
            //});
            $(document).on("keyup", ".Bonus,.Installment,.IncomeTax", function (event) {
                var tr = $(this).parent().parent();
                $this.CalculateNetSalary(tr);
            });
           
            $this.Add();
            $this.GetNextVoucherNumber();
        },
        CalculateGrossSalary: function (tr) {
            var salary = Common.GetFloat($(tr).find("input.Salary").val());
            var providentfund = Common.GetFloat($(tr).find("input.ProvidentFund").val());
            var socialsecurity = Common.GetFloat($(tr).find("input.SST").val());
            var eobi = Common.GetFloat($(tr).find("input.EOBI").val());
            if (providentfund < 1) providentfund = providentfund * salary;
            if (socialsecurity < 1) socialsecurity = socialsecurity * salary;
            if (eobi < 1) eobi = eobi * salary;
            var grossSalary = salary + providentfund + socialsecurity + eobi;
            $(tr).find("input.GrossSalary").val(grossSalary);
        },
        CalculateNetSalary: function (tr) {
            var grossSalary = Common.GetFloat($(tr).find("input.GrossSalary").val());
            var Bonus = Common.GetFloat($(tr).find("input.Bonus").val());
            var OTAmount = Common.GetFloat($(tr).find("input.OTAmount").val());
            var AbsentDeduction = Common.GetFloat($(tr).find("input.AbsentDeduction").val());
            var installment = Common.GetFloat($(tr).find("input.Installment").val());
            var IncomeTax = Common.GetFloat($(tr).find("input.IncomeTax").val());
            var TotalDeductions = Common.GetFloat($(tr).find("input.TotalDeductions").val());
            var netsalary = grossSalary + Bonus + OTAmount - AbsentDeduction - installment - IncomeTax - TotalDeductions;
            $(tr).find("input.NetSalary").val(netsalary);
        },
        UpdateCheckbox: function () {
            if ($('#item-container tbody').children().length <= 0 || $('#item-container tbody :checkbox:not(:checked)').length > 0) {
                $.uniform.update($("#chkAll").prop("checked", false));
            }
            else {
                $.uniform.update($("#chkAll").prop("checked", true));
            }
        },

        CalculateAmount: function (tr) {
            $this = this;
            var hours = 0;
            var rate = 0;
            var numberofleaves = 0;
            hours = Common.GetFloat($(tr).find(":nth-child(6) input.Hours").val());
            rate = Common.GetFloat($(tr).find(":nth-child(7) input.Rate").val());
            var amount = hours * rate;
            $(tr).find(":nth-child(8) input.Amount").val(amount);
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
        
                $this.RebindData();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#mainform"))) {
                
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where(function (x) { return x.IsSelected == true }).ToArray();
                debugger;
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
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  ...please wait",
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

        CustomClear: function () {
            $.uniform.update();
            $("#item-container tbody").html("");
            Common.Clear();
        },
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
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
                        var d = res.Data.SalaryConfigItems;



                        if (d != null) {
                            Common.MapItemData(d, null, null, true);
                            App.initUniform();
                            if (res.Data.VoucherNumber != 0) {

                                $("#VoucherNumber").val(res.Data.VoucherNumber);
                                //    $("#Date").val(moment(d[0].Date).format("dddd, DD-MM-YYYY"));
                                $("#processedsalary-container tbody").html("");
                                for (var i = 0; i < res.Data.Salaries.length; i++) {
                                    var salary = res.Data.Salaries[i];
                                    Enumerable.From(salary.SalaryItems).ForEach(function (x) {
                                        x.VoucherNumber = salary.VoucherNumber;
                                        x.PaymentDate = moment(salary.PaymentDate).format("DD/MM/YYYY");
                                    });

                                    var unApproveditems = Enumerable.From(salary.SalaryItems).Where(function (x) { return x.Status == 0 }).ToArray();//status =0 mean UnApproved
                                    for (var items in unApproveditems) {
                                        var item = unApproveditems[items];
                                        item["Status"] = "Pending";
                                    }
                                  
                                    Common.MapItemData(unApproveditems, "#processedsalary-container", "#template-processedsalary", true);


                                    var approvedItems = Enumerable.From(salary.SalaryItems).Where(function (x) { return x.Status == 1 }).ToArray();//status =1 mean Approved
                                    if (approvedItems.length > 0) {
                                        for (var items in approvedItems) {
                                            var item = approvedItems[items]
                                            item["VoucherNumber"] = salary.VoucherNumber;
                                            item["PaymentDate"] = moment(salary.PaymentDate).format("DD/MM/YYYY");
                                            item["Status"] = "Proved";
                                        }
                                       
                                        Common.MapItemData(approvedItems, "#processedsalary-container", "#template-processedsalary", true);
                                    }


                                    var mailItems = Enumerable.From(salary.SalaryItems).Where(function (x) { return x.Status == 3 }).ToArray();//status =2 mean Emailtobank
                                    if (mailItems.length > 0) {
                                        for (var items in mailItems) {
                                            var item = mailItems[items]
                                            item["VoucherNumber"] = salary.VoucherNumber;
                                            item["PaymentDate"] = moment(salary.PaymentDate).format("DD/MM/YYYY");
                                            item["Status"] = "EmailToBank";
                                        }
                                        Common.MapItemData(mailItems, "#processedsalary-container", "#template-processedsalary", true);
                                    }

                                  //  Common.MapItemData(salary.SalaryItems, "#processedsalary-container", "#template-processedsalary", true);
                                }


                            }
                        }
                        $("#btndelete,#btnprint").prop("disabled", false);
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        //  $this.AddItem();
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
                        var generatesalarys = res.Data.EmployeeOverTimeHours;
                        var employee = res.Data.Employee;

                        $("#item-container tbody").html("");
                        for (var i in generatesalarys) {
                            var employeeIncome = generatesalarys[i];
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
                        $this.DetailView();
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
            var $this = this;
            var voucher = $this.GetType();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            $this.LoadAccounts();
            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },
        Print: function () {
            window.print();
        },
    };
}();

