
var CashPostedCases = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "NexusCashPostedCase";
    var LIST_LOADED = false;
    var SetFocus = "";
    var PageData = new Object();
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $(document).on("click", "#btn-save", function (event) {
                $this.Save();
            });
            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("keyup", ".Price", function (event) {
                var $tr = $(this).closest("tr");
                var $nextTr = $tr.next("tr");
                var price = Common.GetInt($(this).val());
                if (event.which == 13 && price > 0) {
                    $nextTr.find("input.EmployeeId").focus();
                    var pCaseNo = $tr.find("input.CaseNumber").val();
                    var nCaseNo = $nextTr.find("input.CaseNumber").val();
                    if (pCaseNo == nCaseNo) {
                        $nextTr.find("input.EmployeeId").val($tr.find("input.EmployeeId").val());
                        $nextTr.find("input.EmployeeName").val($tr.find("input.EmployeeName").val());
                        $nextTr.find("input.Relationship").val($tr.find("input.Relationship").val());
                        $nextTr.find("input.DepartmentId").val($tr.find("input.DepartmentId").val());
                        $nextTr.find("input.Department").val($tr.find("input.Department").val());
                        var departmentId = Common.GetInt($tr.find('input.DepartmentId').val());
                        var tests = Enumerable.From(PageData.Tests).Where(function (p) { return p.DepartmentAccountId == departmentId; }).ToArray();
                        $this.AutoCompleteInit(tests);
                        $nextTr.find("input.TestName").focus();
                    }


                }

            });
            $("#btnShowReport").click(function () {

                $this.LoadCases();
            });
            $this.LoadPageSetting();
            Common.BindStickyTableHeaders("#tbl-cases");

        },
        LoadData: function (blockUi, reloadPage) {
            var $this = this;
            if (confirm('This will relaod all test and price.Any unsaved data enter in posted cases will be lost.Are you sure to continue?')) {
                $.when($this.LoadDepartments(), $this.LoadTest()).then(function () {
                    window.location.reload();
                });
            }
        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
            Common.GoToTop();
        },

        ListView: function () {

            //if (!LIST_LOADED) {
            //    var url = Setting.APIBaseUrl + API_CONTROLLER;
            //    LIST_LOADED = true;
            //    DataTable.BindDatatable(DATATABLE_ID, url);
            //}
            //$("#Name").focus();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            $("#item-container,#item-nexus-container").find("tbody").html("");
            Common.Clear();
        },

        Save: function () {
            var $this = this;
            var Items = new Array();
            if (Common.Validate($("#form-info"))) {

                Items = Common.SaveItemData("#tbl-cases");
                Items = Enumerable.From(Items).Where("$.IsSelected=='true'").ToArray();
                var err = "";
                if (Items.length <= 0) {
                    err += "Please add atleast one case to process.";
                }
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                var record = { '': Items }
                record["Nexus_PostedCasesItems"] = Items;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            $this.LoadCases();
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

        Edit: function ($element) {
            var $this = this;

            var id = Common.GetInt($element.attr("data-id"));
            var $tr = $element.closest("tr");
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container,#item-nexus-container").find("tbody").html("");
                        var j = res.Data;
                        var nexusCase = j.NexusCase;
                        Common.MapEditData(j.NexusCase, $(".nexus-info"), true);
                        Common.MapItemData(j.NexusCase.Nexus_CaseDetail, "#item-nexus-container", "#template-nexus-item");
                        Common.SetDate($(".RegistrationDate"), nexusCase.RegistrationDate);
                        Common.SetDate($("#Date"), nexusCase.RegistrationDate);
                        $("#CaseId").val(nexusCase.ID);
                        $(".PatientName").val($tr.find("td:nth-child(2)").html());
                        if (j.Case != null) {
                            Common.MapEditData(j.Case, $("#form-info"));
                            Common.SetDate($("#Date"), j.Case.Date);
                            var items = j.Case.Nexus_PostedCasesItems;
                            Common.MapItemData(items);
                        }
                        $this.AddItem();
                        $this.AutoCompleteInit();
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
        LoadTest: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetTests",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading test...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Tests = res.Data;
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        LoadCases: function (key) {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var fromDate = $("#FromDate").val();
                var toDate = $("#ToDate").val();
                var qs = "?dataKey=GetCases";
                qs += "&fromDate=" + fromDate;
                qs += "&toDate=" + toDate;

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "" + qs,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "loading...please wait",
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            PageData.Cases = res.Data;
                            var cases = res.Data;
                            for (var i in cases) {
                                var postCase = cases[i];

                                html += "<tr>";
                                html += "<td>";

                                html += "<input type='hidden' class='CaseId' data-db-column='CaseId' value=" + postCase.CaseId + ">";
                                html += "<input type='hidden' class='CaseDetailId' data-db-column='CaseDetailId' value=" + postCase.CaseDetailId + ">";
                                html += "<input type='hidden' class='TestId' data-db-column='TestId'>";
                                html += "<input type='hidden' class='DepartmentId' data-db-column='DepartmentId' value=" + PageSetting.CashAccount + ">";
                                html += "<input type='hidden' class='RegistrationDate' data-db-column='RegistrationDate' value=" + postCase.RegistrationDate + ">";
                                html += "<input type='hidden' class='CaseNumber' data-db-column='CaseNumber' value=" + postCase.CaseNumber + ">";
                                html += "<input type='hidden' class='PatientName' data-db-column='PatientName' value='" + postCase.PatientName + "'>";
                                html += "<input type='hidden' class='Price' data-db-column='Price' value='" + postCase.Rate + "'>";

                                html += "<label class='control-label'><input type='checkbox' class='IsSelected' data-db-column='IsSelected' data-checktrack='false' value='false'></label>";
                                html += "</td>";
                                html += " <td>" + postCase.CaseNumber + "</td>";
                                html += " <td>" + postCase.PatientName + "</td>";
                                html += " <td>" + Common.FormatDate(postCase.RegistrationDate, "DD-MM-YYYY") + "</td>";
                                html += " <td>" + postCase.ReferenceName + "</td>";
                                html += " <td>" + postCase.ConsultantName + "</td>";
                                html += " <td>" + postCase.TestName + "</td>";
                                html += " <td>" + postCase.Rate + "</td>";
                                html += "</tr>";
                            }
                            if (cases.length == 0)
                                html += "  <tr><td colspan='18' style='text-align: center'>No record(s) found</td></tr>";
                            else {
                                $("#tbl-cases tbody").html(html);
                                $("input[type='checkbox']").uniform();
                                Common.SetCheckChange();


                            }



                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                        Common.ShowError(e.responseText);
                    }
                });
            }
        },

        LoadPageSetting: function () {
            var $this = this;
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

            //$(".caption").html(" <i class='fa fa-edit'></i>" + PageSetting.FormTitle);
        },

        GetWholeTotal: function () {

            var Quantity = 0;
            var amount = 0;
            var credit = 0;
            $("#item-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find("input.Price").val());


            });
            $("#TotalAmount,#NetAmount,#PaidAmount").val(amount);
            $("#Discount,#Less").val(0);
        },


    };
}();
