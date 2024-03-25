
var Test = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "Test";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var MAINCATEGORY = "";

    //var SetFocus = "booknumber";
    return {
        init: function () {
            var $this = this;
            $(document).on('change', '#MainCategoryId', function (event) {

                var category = MAINCATEGORY = $("option:selected", $(this)).text();
                if (category != null)
                    MAINCATEGORY = category = category.toUpperCase();
                if (category == "RADIOLOGY") {
                    $("#parameter-container").addClass("hide");
                }
                else {
                    $("#parameter-container").removeClass("hide");
                }
            });
            $("#Amount").keyup(function (e) {

                if (e.which == 13) {
                    var ischeque = $('#ChequeNo').hasClass('hide');
                    if (ischeque == true) {
                        $("#Investitem tbody tr:nth-last-child(1) td:nth-child(1) input.Doctor").focus().select();
                    }
                    else {
                        $('#ChequeNo').focus();
                    }
                }
            });

            $(document).on("keyup", "#Investitem input.Price", function (event) {
                if (event.which == 13 && $(this).val().trim() != "") {
                    $this.AddItem();
                }
            });
            $(document).on('keyup', '.PaymentAmount', function (event) {

                var tr = $(this).parent().parent();
                // $this.DoItemCalculations(tr, event);
            });

            var url = Setting.APIBaseUrl + API_CONTROLLER;
            if (Setting.PageLandingView == "DetailView") {
                this.Add();
            } else {
                this.ListView();
            }
            $this.LoadPageSetting();
            $this.ListView();

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
        },
        RebindData: function () {
            DataTable.RefreshDatatable(DATATABLE_ID);
        },


        DetailView: function () {
            var $this = this;
            $('#form-info').removeClass('hide');
            $('#div-table').addClass('hide');
        },
        Add: function () {
            var $this = this;
            $this.DetailView();
            $this.CustomClear();

        },
        ListView: function () {
            $('#form-info').addClass('hide');
            $('#div-table').removeClass('hide');
            var $this = this;
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

        AddItem: function () {

            var $this = this;

            var code = $("#Investitem tbody tr:nth-last-child(1) td:nth-child(1) input.ParameterName").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#Investitem tbody tr:nth-last-child(1) td:nth-child(1) input.ParameterName").focus().select();
                }, 300);

                // SetFocus = "doctor";
                return;

            }
            var html = "<tr>";
            html += "<td><input type='hidden' class='Id'/>";
            html += "<input type='hidden' class='ParameterId'/>";
            html += "<input type='text' class='ParameterName form-control typeahead'/></td>";

            html += "<td class='align-right'><input type='text' class='Price form-control input-xsmall num3' /></td>";
            html += "<td class='td-width'></td>";
            html += "<td class='td-width'></td>";
            html += '<td class="td-width"></td>';
            html += '<td style="width: 8px;"><span class="action"><i class="fa fa-trash-o" onclick="Test.DeleteRow(this)" data-original-title="Delete Item"></i></span></td>';
            html += "</tr>";
            $("#Investitem tbody").append(html);
            $("#Investitem tbody tr:nth-last-child(1) input.ParameterName").focus().select();
            $this.AutoCompleteInit();



        },
        ReinializePlugin: function () {
            Common.AllowNumerics();

        },
        DrawDepartment: function () {
            var $this = this;
            var html = "";
            var departments = Common.GetLeafAccounts(PageSetting.Customers);
            for (var i in departments) {
                var department = departments[i];
                html += "<tr>";
                html += "<td>";
                html += "<input type='hidden' class='DepartmentAccountId' data-db-column='DepartmentAccountId' value=" + department.Id + ">";
                html += "<label class='control-label'><input type='checkbox' class='IsSelected' data-db-column='IsSelected' data-checktrack='false' value='false'></label>";
                html += "</td>";
                html += " <td><input type='text' class='DepartmentName form-control' data-db-column='DepartmentName' value='" + department.DisplayName + "'  disabled='disabled' readonly='readonly'></td>";
                html += " <td><input type='text' class='Price form-control input-small num3 align-right' data-db-column='Price'></td>";
                html += "</tr>";
            }
            var $tbody = $("#tbl-department tbody");
            $tbody.html(html);
            $("input[type='checkbox']", $tbody).uniform();
            Common.SetCheckChange();

        },
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).parent().parent().parent().remove();
            // $this.GetWholeTotal();
            if ($("#Investitem tbody").children().length <= 0)
                $this.AddItem();
        },
        CloseItem: function () {
            Common.Clear();
            $('#form-info').addClass('hide');
            $('#div-table').removeClass('hide');
            $('#form-info-item').addClass('hide');
            $('#masterdetail').removeClass('hide');
            $('#div-table-item').addClass('hide');
        },
        //Save: function () {
        //    var $this = this;
        //    $this.SaveRecord(function () {
        //       // $this.ListView();
        //        $this.CustomClear();
        //        SetFocus = "date";


        //    });
        //},
        Save: function () {
            var $this = this;
            this.SaveRecord(function () {
                DataTable.RefreshDatatable(DATATABLE_ID);
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                LIST_CHANGED = true;
                $this.Add();
            });
        },
        SaveClose: function () {
            var $this = this;
            this.SaveRecord(function () {
                DataTable.RefreshDatatable(DATATABLE_ID);

                $this.ListView();
            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            $(".container-message").hide();
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {

                $("#Investitem tbody tr").each(function () {
                    var parameterid = Common.GetInt($(this).find("input.ParameterId").val());
                    if (parameterid > 0) {
                        Items.push({
                            TestId: record.Id,
                            ParameterId: parameterid,
                            Price: Common.GetFloat($(this).find("input.Price").val()),
                            Id: $(this).find("input.Id").val(),
                        });
                    }
                });
                record["TestParameters"] = Items;
                var err = "";
                var testDepartments = Common.SaveItemData("#tbl-department");
                testDepartments = Enumerable.From(testDepartments).Where("$.IsSelected=='true'").ToArray();
                var err = "";

                for (var i in testDepartments) {
                    var testDepartment = testDepartments[i];
                    if (testDepartment.Price <= 0) {
                        err += "Department  '<b>" + testDepartment.DepartmentName + "</b>' must have test price greater than zero(0).,";
                    }

                }
                if (MAINCATEGORY != "RADIOLOGY" && Items.length <= 0) {
                    err += "Atleast one Parameter must be added in test."
                }

                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                var testExtraRecord = {
                    Test: record,
                    TestDepartments: testDepartments
                }



                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?withRateList=true",
                    type: "POST",
                    data: testExtraRecord,
                    success: function (res) {
                        if (res.Success) {
                            callback();
                            $("#TestGroupId").focus();
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
            var $this = this;
            Common.Clear();
            $("#Investitem tbody").html('');
            $this.AddItem();
            $('#TestGroupId').focus();
            $this.DrawDepartment();

        },

        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
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

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var test = res.Data.Test;

                        var testparameters = test.TestParameters;

                        Common.MapEditData(test, "#form-info");
                        $("#Investitem tbody").html('');
                        if (testparameters.length > 0) {
                            var html = "";
                            for (var i in testparameters) {
                                var testparameter = testparameters[i];
                                var parameter = $.grep(res.Data.Parameters, function (element, index) { return element.Id == testparameter.ParameterId })[0];
                                html = "<tr>";
                                html += "<td><input type='hidden' id='Id' value='" + testparameter.Id + "' />";
                                html += "<input type='hidden' class='ParameterId' id='ParameterId' value='" + testparameter.ParameterId + "'/>";
                                html += "<input type='text' id='ParameterName' class='ParameterName form-control typeahead' value='" + parameter.Name + "'/></td>";
                                html += "<td class='align-right'><input type='text' class='Price form-control input-xsmall num3'  value='" + testparameter.Price + "' /></td>";
                                html += "<td class='td-width'>" + parameter.Unit + "</td>";
                                html += "<td class='td-width'>" + parameter.MinValue + "</td>";
                                html += "<td class='td-width'>" + parameter.MaxValue + "</td>";
                                html += '<td style="width: 8px;"><span class="action"><i class="fa fa-trash-o" onclick="Test.DeleteRow(this)" data-original-title="Delete Item"></i></span></td>';
                                html += "</tr>";
                                $("#Investitem tbody").append(html);
                            }
                        }
                        $this.DrawDepartment()
                        $("#tbl-department tbody tr").each(function () {
                            var $tr = $(this);
                            var departmentId = Common.GetInt($tr.find("input.DepartmentAccountId").val());
                            if (Enumerable.From(res.Data.TestDepartments).Any("$.DepartmentAccountId == " + departmentId + "")) {
                                $.uniform.update($tr.find("input[type='checkbox']").prop("checked", true).val("true"));
                                var departmentPrice = Enumerable.From(res.Data.Prices).FirstOrDefault(null, "$.DepartmentAccountId == " + departmentId + "");
                                if (departmentPrice != null)
                                    $tr.find("input.Price").val(departmentPrice.Price)
                            }
                        });
                        $this.AddItem();
                        $this.DetailView();
                        $("#MainCategoryId").trigger("change");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        Print: function () {
            window.print();
        },
        LoadDepartment: function (products) {
            var $this = this;
            var customers = Common.GetLeafAccounts(PageSetting.Customers);
            Common.BindSelect(customers, "#DepartmentAccountId", true)


        },
        AutoCompleteInit: function () {
            var $this = this;
            var suggestion = new Array();
            for (var i in AppData.ParameterNames) {
                var item = AppData.ParameterNames[i];
                suggestion.push({
                    id: item.Id,
                    value: item.Name,
                    label: item.Name,
                    parameter: item,
                });
            }
            $(".ParameterName").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    $(".ParameterId", $(this).parent().parent()).val(ui.item.id);
                    var tr = $(this).parent().parent();
                    $(tr).find('td:nth-child(3)').html(ui.item.parameter.Unit)
                    $(tr).find('td:nth-child(4)').html(ui.item.parameter.MinValue)
                    $(tr).find('td:nth-child(5)').html(ui.item.parameter.MaxValue)
                    $(tr).find("input.Price").focus();

                }
            });

        },

    }
}();

window.onpopstate = function (e) {
    if (e.state) {
        var type = Common.GetQueryStringValue("type").toLowerCase();
        Transaction.ChangeType(type);
    }
};