var EmployeeOTHour = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "EmployeeOTHour";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var SetFocus = "code";
    var recordtype = "Save";
    var otrate = 0;
    return {
        init: function () {
            var $this = this;

           
            $("#Month,#Year").change(function () { $this.SetSalaryConfigName(); });

            $("#Year").change(function () {
                $this.Add();
            });

            $(document).on("keyup", ".Date,.Hours,.Rate", function (event) {

                var tr = $(this).parent().parent();
                $this.CalculateAmount(tr);
              
                if (event.which==13)
                    $this.AddItem();
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
                        var err = "<li>" + $(this).val() + " is not valid code.</li>";
                        Common.ShowError(err);
                        $(this).focus();
                    }
                }

            });
          
            $this.LoadPageSetting();
            $this.LoadAccountDetail();
            $this.ListView();
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
         
            Common.Clear();
            $this.DetailView();
            $this.CustomClear();
            $this.SetSalaryConfigName();
            $("#item-container tbody").html("");
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
            //if (SetFocus == "date") {
            //    setTimeout(function () {
            //        $("#Date").focus();
            //    }, 300);
            //}
            //else {
            //    setTimeout(function () {
            //        $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.Code").focus().select();
            //    }, 300);
            //}
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
                Items = Enumerable.From(Items).Where("$.AccountId.trim()!=''").ToArray();
                var err = "";
                if (Items.length <= 0) {
                    err += "Please add atleast one item.";
                }
                for (var i in Items) {
                    var item = Items[i];
                    if (item.FromDate == "" || item.ToDate == "") {
                        //err += "<li>Employee " + item.Name + " must have From Date and End Date.";
                    }
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record["EmployeeOTHourList"] = Items;
                record.RecordType = recordtype;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving  " + $this.GetType() + " ...please wait",
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
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();
            Common.Clear();
        },
      
        LoadAccountDetail: function (id) {
            var $this = this;
            AppData.AccountDetail = PageSetting.AccountDetails;
        },
        Delete: function (month,year,name) {
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
        Edit: function (month, year,name) {
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
                      //  var data = res.Data.Records;
                        var html = "";
                        var name = "";
                        var month = 0;
                        var year = 0;
                        var employeeOThours = res.Data.EmployeeOverTimeHours;
                        var employee = res.Data.Employee;

                        $("#item-container tbody").html("");
                        for (var i in employeeOThours) {
                            var employeeOThour = employeeOThours[i];
                            name = employeeOThour.Name;
                            month = employeeOThour.Month;
                            year = employeeOThour.Year;
                            employeeOThour.FromDate = moment(employeeOThour.FromDate).format('DD/MM/YYYY');
                            employeeOThour.ToDate = moment(employeeOThour.ToDate).format('DD/MM/YYYY');
                            var empInfo = Enumerable.From(employee).Where(function (x) { return x.Id == employeeOThour.AccountId }).FirstOrDefault();
                            employeeOThour["Code"] = empInfo.AccountCode;
                            employeeOThour["EmployeeName"] = empInfo.DisplayName;

                            var templateHtml = $("#template-item").html();
                            //var placeholder = $this.GetPlaceHolder(employeeIncome);
                            //templateHtml = templateHtml.allReplace(placeholder);
                            html = templateHtml;
                            $("#item-container tbody").append(html);
                            for (var key in employeeOThour) {
                                var element = $("#item-container tbody tr:last").find("." + key);
                                $(element).val(employeeOThour[key]);
                            }
                        }
                        $("#Name").val(name);
                        $("#Month").val(month);
                        $("#Year").val(year);
                        //$("#Name").prop("disabled", true);
                        $("#Month").prop("disabled", true);
                        $("#Year").prop("disabled", true);
                   
                        $this.DetailView();
                        $this.AddItem();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        GetOTStandardRate: function (tr,accountId) {
            $this = this;
            // recordtype = "Edit";
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());

            var fromDate = moment(new Date(year, month - 1, 1)).format('YYYY-MM-DD');
            var toDate = moment(new Date(year + 1, month - 1, 0)).format('YYYY-MM-DD');

            var html = "";
            var qs = "?key=GetOTStandardRate";
            qs += "&fromdate=" + fromDate;
            qs += "&todate=" + toDate;
            qs += "&accountId=" + accountId;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var configItem = res.Data;
                        otrate = configItem.OverTimeRate;
                        $(tr).find("input.Rate").val(otrate);
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
            var id = PageSetting.Employees;
            //switch ($this.GetType().toLowerCase()) {
            //    case "sale":
            //        id = PageSetting.Customers;
            //        break;
            //    case "salereturn":
            //        id = PageSetting.Customers;
            //        break;
            //    case "purchase":
            //        id = PageSetting.Suppliers;
            //        break;
            //    case "purchasereturn":
            //        id = PageSetting.Suppliers;
            //        break;
            //}
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
                    otrate = $this.GetOTStandardRate(tr,employee.AccountId);
                   
                    if (typeof employee != "undefined" && employee != null) {
                        $(tr).find(":nth-child(1) input.ItemId").val(employee.AccountId);
                        $(tr).find(":nth-child(2) input.EmployeeName").val(employee.Name);
                     //   $(tr).find("input.Rate").val(otrate);
                        $(".container-message").hide();
                        setTimeout(function () {
                            $("#item-container tbody tr td:nth-child(4) input.FromDate").focus().select();
                        }, 10);
                    }
                }
            });

        },

      
    };
}();

