var EmployeeLeave = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "EmployeeLeave";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "booknumber";
    var recordtype = "Save";
    return {
        init: function () {
            var $this = this;

            $("#Month,#Year").change(function () { $this.SetSalaryConfigName(); });


            $(document).on("keyup", ".Remarks", function (event) {

                if (event.which == 13)
                    $this.AddItem();
            });

            $(document).on("blur", ".NumberOfLeaves", function () {
                var daysInmonth = $this.daysInMonth();
                if($(this).val() > daysInmonth)
                    Common.ShowError("Number of leaves should be less than "+daysInmonth);
            });

            $(document).on('keyup', '.Code', function (event) {

                if (event.which == 13) {
                    if ($(this).val() == "") {
                        $(".btn.btn-primary.green").focus();
                    }
                    else {
                        $(this).parent().parent().find("td:nth-child(3) input.LeaveType").focus();
                    }
                }
            });
            $(document).on("blur", ".Code", function () {
                var voucher = Common.GetQueryStringValue("type").toLowerCase();
                var employee = $this.GetByCode($(this).val());
                console.log(employee);
                var tr = $(this).parent().parent();
                if (typeof employee != "undefined" && employee != null) {
                    $(tr).find(":nth-child(1) input.AccountId").val(employee.AccountId);
                    $(tr).find(":nth-child(2) input.Name").val(employee.Name);
                    $(".container-message").hide();
                }
                else {
                    if ($(this).val().trim() != "") {
                        var err = $(this).val() + " is not valid code.";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }

            });
        
            $this.LoadPageSetting();
            $this.LoadAccountDetail();
            $this.ListView();
           
        },


        daysInMonth: function () {
            var month = $("#Month").val();
            var year = $("#Year").val();
            return new Date(year, month, 0).getDate();
        },

        SetSalaryConfigName: function () {
            var $this = this;
            var frommonth = Common.GetInt($("#Month").val());
            var fromyear = Common.GetInt($("#Year").val());

            $("#Name").val($("#Month option:selected").text() + " " + $("#Year").val());
        },

        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        CalculateRemainingLeaves: function (tr)
        {
            $this = this;
            var total = 0;
            var availed = 0;
            var numberofleaves = 0;
            total = Common.GetInt($(tr).find(":nth-child(4) input.Total").val());
            availed = Common.GetInt($(tr).find(":nth-child(5) input.Availed").val());
            var balance = total - availed;
            $(tr).find(":nth-child(6) input.Balance ").val(balance);
            numberofleaves = Common.GetInt($(tr).find(":nth-child(7) input.NumberOfLeaves ").val());
            var remainingleaves = Common.GetInt(balance - numberofleaves);
            $(tr).find(":nth-child(8) input.Remaining").val(remainingleaves);
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
    
        GetByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },

        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
      
        Add: function () {
            var $this = this;
            recordtype = "Save";
            $this.DetailView();
            $this.CustomClear();
            $this.SetSalaryConfigName();
            $this.AddItem();
            $(".container-message").hide();
            //$("#Name").prop("disabled", false);
            $("#Month").prop("disabled", false);
            $("#Year").prop("disabled", false);
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

            //if (Common.Validate($("#addrow"))) {
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
            setTimeout(function () {
                $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
            }, 300);
            SetFocus = "code";
            if (!PageSetting.BarCodeEnabled) {
                $this.AutoCompleteInit();
            }
            Common.InitDateMask();


        },
    
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            $this.GetWholeTotal();
            if ($("#item-container tbody").children().length <= 0)
                $this.AddItem();
        },

        CheckMonthDay: function () {
            var $this = this;

            //$("#item-container tbody tr td:nth-child(4) input.NumberOfLeaves").val()
            $("#item-container tbody tr").each(function () {
              //  var x = $(this).val();

            });
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
                var scope = $("#form-info-item");
                $this.ListView();
             //   $this.RebindData();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            var err = "";

            //validate Number of leaves not greater then selected month
            $("#item-container tbody tr").each(function () {
                //$("#item-container tbody tr td:nth-child(4) input.NumberOfLeaves").val()
                //   if ($(this).find("td:nth-child(1) input.AccountId").val() > 0)
                var noOfLeaves= $(this).find("td:nth-child(4) input.NumberOfLeaves").val();
                var daysInmonth = $this.daysInMonth();
                if (noOfLeaves > daysInmonth) {
                    err += "Number of leaves should be less than " + daysInmonth+"</li>";
                    Common.ShowError(err);
                } 
            });

            //var firstOfMonth = new Date($("#Year").val(), $("#Month").val(), 1);


            $(".container-message").hide();
            var mode = "add";
            var voucher = Common.GetQueryStringValue("type").toLowerCase();
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
               
              //  $this.CheckMonthDay();

                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.AccountId.trim()!=''").ToArray();
              
                if (Items.length <= 0) {
                    err += "<li>Please add atleast one item.</li>";
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["EmployeeLeaveList"] = Items;
                record.RecordType = recordtype;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  leaves ...please wait",
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
            $("input:radio[value='credit']").prop("checked", true);
            $("#item-container tbody").html("");
            Common.Clear();
        },
      
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
      
        Delete: function (month, year, name) {
            $this = this;
            var qs = "?key=Edit";
            qs += "&name=" + name;
            qs += "&month=" + month;
            qs += "&year=" + year;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/1" + qs,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockMessage: "Delete Appointment...please wait",
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
        Cancel: function () {
            var $this = this;
            $("#btnEdit,span.notEdit,span.notEdit").removeClass("hide");
            $("#btnSave,#btnCancel,.canEdit").addClass("hide");
        },
        Edit: function (month, year, name) {
            $this = this;
            var html = "";

            recordtype = "Edit";
            qs += "&name=" + name;
            var qs = "?key=Edit";
            qs += "&month=" + month;
            qs += "&year=" + year;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data.Records;
                        var html = "";
                        var name = "";
                        var year = 0;
                        var month = 0;
                        var employeeleave = res.Data.EmployeeLeave;
                        var employee = res.Data.Employee;

                        $("#item-container tbody").html("");
                        for (var i in employeeleave) {
                            var employeeleaves = employeeleave[i];
                            name = employeeleaves.Name;
                            month = employeeleaves.Month;
                            year = employeeleaves.Year;
                            employeeleaves.FromDate = moment(employeeleaves.FromDate).format('DD/MM/YYYY');
                            employeeleaves.ToDate = moment(employeeleaves.ToDate).format('DD/MM/YYYY');
                            //var childaccounts = $.grep(employee, function (e) { return e.Id == employeeIncome.AccountId });   
                            var empInfo = Enumerable.From(employee).Where(function (x) { return x.Id == employeeleaves.AccountId }).FirstOrDefault();
                            employeeleaves["Code"] = empInfo.AccountCode;
                            employeeleaves["EmployeeName"] = empInfo.DisplayName;
                            var templateHtml = $("#template-item").html();
                            //var placeholder = $this.GetPlaceHolder(employeeIncome);
                            //templateHtml = templateHtml.allReplace(placeholder);
                            html = templateHtml;
                            $("#item-container tbody").append(html);
                            for (var key in employeeleaves) {
                                var element = $("#item-container tbody tr:last").find("." + key);
                                $(element).val(employeeleaves[key]);
                            }
                        }
                        $("#Name").val(name);
                        $("#Month").val(month);
                        $("#Year").val(year);
                        $("#Name").prop("disabled", true);
                        $("#Month").prop("disabled", true);
                        $("#Year").prop("disabled", true);
                        $this.DetailView();
                        $this.AddItem();
                        //$this.LoadPageSetting();
                      
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
      
        //LoadAccounts: function () {
        //    var $this = this;
        //    var id = 0;
        //    switch ($this.GetType().toLowerCase()) {
        //        case "sale":
        //            id = PageSetting.Customers;
        //            break;
        //        case "salereturn":
        //            id = PageSetting.Customers;
        //            break;
        //        case "purchase":
        //            id = PageSetting.Suppliers;

        //            break;
        //        case "purchasereturn":
        //            id = PageSetting.Suppliers;
        //            break;
        //    }
        //    var tokens = Common.GetLeafAccounts(id);
        //    var suggestion = new Array();
        //    for (var i in tokens) {
        //        var token = tokens[i];
        //        suggestion.push(
        //            {
        //                id: token.AccountId,
        //                value: token.AccountCode,
        //                label: token.AccountCode + "-" + token.DisplayName
        //            }
        //        );
        //    }

        //    $("#AccountCode").autocomplete({
        //        source: suggestion,
        //        selectFirst: true,
        //        autoFocus: true,
        //        select: function (event, ui) {

        //            var d = $this.GetByCode(ui.item.value);
        //            var type = $this.GetType();
        //            if (typeof d != "undefined" && d != null) {
        //                if (type == "sale") {
        //                    $("#Comments").val("Sold To: " + d.Code + "-" + d.Name);
        //                }
        //                else if (type == "salereturn") {
        //                    $("#Comments").val("Sale Return From: " + d.Code + "-" + d.Name);
        //                }
        //                else if (type == "purchase") {
        //                    $("#Comments").val("Purchase From: " + d.Code + "-" + d.Name);
        //                }
        //                else if (type == "purchasereturn") {
        //                    $("#Comments").val("Purchase Return To: " + d.Code + "-" + d.Name);
        //                }
        //                $("#AccountName").val(d.Name);

        //                $("#AccountId").val(d.AccountId);
        //                var address = d.Address;
        //                if (typeof address != "undefined" && address != "null")
        //                    $("#PartyAddress").val(address);
        //                $(".container-message").hide();
        //            }
        //            $this.MapComments();
        //        }
        //    });
        //},
        LoadPageSetting: function () {
            $this = this;
           // var voucher = this.GetType();
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
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        AutoCompleteInit: function (partyid) {
            var $this = this;
            var employees = Common.GetLeafAccounts(PageSetting.Employees);
            var suggestion = new Array();
            for (var i in employees) {
                var employee = employees[i];
                suggestion.push(
                    {
                        id: employee.AccountId,
                        value: employee.AccountCode,
                        label: employee.AccountCode + "-" + employee.DisplayName
                    }
                );
            }

            $(".Code").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var employee = $this.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof employee != "undefined" && employee != null) {
                        $(tr).find(":nth-child(1) input.AccountId").val(employee.AccountId);
                        $(tr).find(":nth-child(2) input.EmployeeName").val(employee.Name);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr td:nth-child(4) input.NumberOfLeaves").focus().select();
                        }, 10);
                    }
                }
            });

        },
        LeaveTypeAutoCompleteInit: function () {
            var $this = this;
            //   var tokens = LeaveTypes; //LeaveTypes is Enum
            var tokens = PageSetting.LeaveTypes;
            var suggestion = new Array();
            for (var key in tokens) {
                var token = tokens[key];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.Name,
                        label: token.Name,
                        //Name: token.Name
                    }
                );
            }

            $(".LeaveType").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).parent().parent();
                    var employeid = $(tr).find("input.AccountId").val();
                    var employecode = $(tr).find("input.Code").val();
                    if (typeof employeid != "undefined" && employeid != null) {
                        var employeeincomdata = $this.GetEmployeeIncomData(employeid);
                        var availedleaves = $this.GetAvailedLeaves(employeid);

                        if (typeof employeeincomdata != "undefined" && employeeincomdata != null) {
                            var leavetype = ui.item.id
                            $(tr).find(":nth-child(4) input.Total").val((leavetype == LeaveTypes.Annual ?employeeincomdata.AnnualLeaves:
                                leavetype == LeaveTypes.Sick?employeeincomdata.SickLeaves:
                                leavetype == LeaveTypes.Casual ? employeeincomdata.CasualLeaves : ""));
                            $(tr).find(":nth-child(5) input.Availed ").val(availedleaves);
                            $(tr).find(":nth-child(3) input#LeaveTypeId ").val(ui.item.id);
                            //$(tr).find(":nth-child(3) input.Quantity").focus();
                            $(".container-message").hide();
                            $this.CalculateRemainingLeaves(tr)
                        }
                    }
                }
            });

        },
        //GetEmployeeIncomData: function (employeeid) {

        //    var data= Enumerable.From(PageSetting.EmployeeIncomeConfigs).FirstOrDefault(null, "$.AccountId == '" + employeeid + "'");
        //    return data;
        //},
        //GetAvailedLeaves: function (employeeid) {

        //    var availedleaves = Enumerable.From(PageSetting.EmployeeLeaves).Where(function (x) { return x.AccountId == employeeid })
        //    .Sum(function (x) { return x.NumberOfLeaves });
        //    return availedleaves;
        //},
    };
}();

