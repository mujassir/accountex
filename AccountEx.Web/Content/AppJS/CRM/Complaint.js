
var Complaint = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMComplaint";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var UPLOAD_FOLDER = "ComplaintsFiles";
    var PageData = new Object();
    var PageSetting = new Object();
    var Current_ID = 0;
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
            $(document).on("click", ".btn-view", function (event) {
                $this.Edit($(this), true);
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $(document).on("click", ".action-delete", function (event) {
                $this.DeleteRow($(this));
            });
            $(document).on("click", "#btn-add-item", function (event) {
                $this.AddItem($(this));
            });
            //$(document).on("click", ".btn-upload-files", function (event) {
            //    $this.LoadFiles($(this));
            //});
            $(document).on("change", "#RegionId", function (event) {
                $this.LoadComplaints();
            });
            $(document).on("change", "#CaseTypeId", function (event) {
                $this.SetFormControl();
            });
            $(document).on("change", "select#CustomerId", function (event) {
                $this.LoadProjects();
            });
            $(document).on("change", "select#ProjectId", function (event) {
                $this.SelectActualProduct();
            });
            $(document).on("change", "select#StatusId", function (event) {
                $this.ChangeStatus();
            });
            $(document).on("click", "#btn-save-files", function (event) {
                $this.SaveDocuments($(this));
            });

            $(document).on("click", ".model-upload-documents table :button.delete", function (event) {
                $this.DeleteFile($(this));
            });
            $(".model-upload-documents .fileupload-buttonbar").on("click", ":button.delete-all", function () {
                $this.DeleteAllFile($(this));
            });
            $(".model-upload-documents .fileupload-buttonbar").on("change", "#select-all-files", function () {
                $this.SelectAllFile($(this).is(":checked"));
            });

            $this.LoadPageSetting();
            $this.AddItem($(this));
            $this.CustomClear();
            $this.LoadProducts();
            $this.ListView();
            $this.New();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            Common.GoToTop();
        },
        New: function () {
            var $this = this;
            SetFocus = "date";
            $this.GetNextVoucherNumber();
        },

        SetFormControl: function () {
            var $this = this;
            var caseTypeId = $("#CaseTypeId").val();
            if (caseTypeId == CRMLabCaseType.ProjectTesting) {
                $(".project-container,.own-product-container,.comp-product-container").removeClass("hide");
                $("#ActualProductId,#ProductId").select2("enable", false);
                $("#ProjectId").select2("enable", true);
                $(".counter-product-label").html("Product");
            }
            else if (caseTypeId == CRMLabCaseType.OwnProductTesting) {
                $(".project-container,.comp-product-container").addClass("hide");
                $(".own-product-container").removeClass("hide");
                $("#ProductId").select2("enable", true);
                $("#ActualProductId,#ProjectId").select2("enable", false);
                $(".counter-product-label").html("Product");
            }
            else if (caseTypeId == CRMLabCaseType.CompProductTesting) {
                $(".project-container,.own-product-container").addClass("hide");
                $(".comp-product-container,.product-price-container,.counter-multiple-product-container").removeClass("hide");
                $("#ActualProductId").select2("enable", true);
                $("#ProductId,#ProjectId").select2("enable", false);
            }
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNo").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetNextVoucherNumber",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Processing...please wait",
                success: function (res) {

                    if (res.Success) {
                        $("#VoucherNo").val(res.Data)
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
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

        CustomClear: function () {
            var $this = this;
            Common.Clear();
            $Current_TR = 0;
            $("#item-container tbody").html('');
            $this.AddItem();
            App.initUniform();
        },
        AddItem: function () {

            var $this = this;

            var $tr = $("#item-container tbody tr:nth-last-child(1)");
            var voucherNo = 0;
            if ($tr.length > 0) {
                voucherNo = Common.GetInt($tr.find("input.VoucherNo").val()) + 1;
                $this.AddItemHTML(voucherNo);
            }
            else {

                var voucherno = Common.GetInt($("#VoucherNumber").val());
                //url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?type=" + VoucherType[$this.GetType()] + "?&key=" + key,
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetNextVoucherNumber",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Processing...please wait",
                    success: function (res) {

                        if (res.Success) {
                            voucherNo = res.Data;
                            $this.AddItemHTML(voucherNo);

                        }
                        else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                        Common.ShowError(e.responseText);
                    }
                });
            }
        },
        AddItemHTML: function (voucherNo) {
            var $this = this;
            var customerId = $("#item-container tbody tr:nth-last-child(1)  select.CustomerId").val();
            if (typeof customerId != "undefined" && customerId.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) select.CustomerId").focus().select();
                }, 300);

                focusElement = "";
                return;
            }
            var html = $("#template-item").html();
            $("#item-container tbody").append(html);
            if (focusElement != "") {
                setTimeout(function () {
                    $(focusElement).focus();
                    focusElement = "";
                }, 300);

            }
            else {

                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) input.VoucherNo ").focus().select();
                }, 300);
                focusElement = "";
            }
            Common.InitNumerics();
            Common.SetCheckChange();
            $("#item-container tbody tr:nth-last-child(1) select.select2").each(function () {
                Common.BindSelect2($(this));
            });
            Common.InitTimePicker();
            App.initUniform();
            var $tr = $("#item-container tbody tr:nth-last-child(1)");
            $tr.find("input.VoucherNo").val(voucherNo);
            $this.AutoCompleteActualProductInit($tr);
            $this.AutoCompleteProductInit($tr);
        },
        Edit: function ($element, isView) {
            var $this = this;
            var id = Common.GetInt($element.attr("data-id"));
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
                        var data = res.Data;
                        if (res.Data != null) {
                            if (!Common.isNullOrWhiteSpace(data.TestTypeId))
                                data["TestTypeId"] = data.TestTypeId.split(",");
                            if (!Common.isNullOrWhiteSpace(data.CounterProductIds))
                                data["CounterProductIds"] = data.CounterProductIds.split(",");
                        }
                        Common.MapEditData(data, "#form-info", undefined, isView);
                        $this.LoadProjects(data.ProjectId);
                        $this.SetFormControl([res.Data]);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        DeleteRow: function (element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            var id = Common.GetInt($($tr).find("input.Id").val());
            if (id > 0) {
                $this.Delete(id, $tr);

            }

            else {
                $tr.remove();
            }

        },
        DeleteFile: function (element) {
            $(element).closest("tr").remove();


        },
        DeleteAllFile: function (element) {
            var $this = this;
            $(".model-upload-documents tbody.files tr td input:checked").each(function () {
                $this.DeleteFile($(this));
            });

        },
        SelectAllFile: function (checked) {
            var $this = this;
            $(".model-upload-documents tbody.files tr td input[type='checkbox']").each(function () {
                $(this).prop("checked", checked);
            });
            $.uniform.update();

        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                var voucherNo = Common.GetInt($("#VoucherNumber").val());
                DataTable.RefreshDatatable(DATATABLE_ID);
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.GetNextVoucherNumber();

            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            if (!Common.isNullOrWhiteSpace(record.TestTypeId)) {
                var tests = Enumerable.From(record.TestTypeId).Where(function (x) { return !Common.isNullOrWhiteSpace(x) }).ToArray();
                record.TestTypeId = tests.join(",");
            }
            if (!Common.isNullOrWhiteSpace(record.CounterProductIds)) {
                var counterProductIds = Enumerable.From(record.CounterProductIds).Where(function (x) { return !Common.isNullOrWhiteSpace(x) }).ToArray();
                record.CounterProductIds = counterProductIds.join(",");
            }
            var Items = new Array();
            if (Common.Validate($("#mainform"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
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
        ChangeStatus: function () {

            var $this = this;
            var scope = $("#form-info");
            var Items = new Array();
            var id = Common.GetInt($("#Id").val());
            var statusId = Common.GetInt($("#StatusId").val());
            if (id == 0)
                return;
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=changeStatus&statusId=" + statusId + "&id=" + id,
                    type: "POST",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            var data = res.Data;
                            Common.ShowMessage(true, { message: "status has been changed successfully." });
                            if (statusId == CRMComplaintStatus.Closed) {
                                $this.New();
                            }
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        LoadComplaints: function () {
            var $this = this;
            var id = 0;
            var regionId = $("#RegionId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?regionId=" + regionId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-table",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var complaints = res.Data;
                        $this.MapComplaints(complaints);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        MapComplaints: function (complaints) {

            $("#item-container tbody").html("");
            for (var i in complaints) {
                var item = complaints[i];
                if (!Common.isNullOrWhiteSpace(item.TestTypeId))
                    item.TestTypeId = item.TestTypeId.split(",");
            }
            Common.MapItemData(complaints);
            Common.InitNumerics();
            Common.SetCheckChange();
            $("#item-container tbody tr select.select2").each(function () {
                Common.BindSelect2($(this));
            });

            Common.InitTimePicker();
            App.initUniform();
            $this.AutoCompleteActualProductInit();
            if (PageSetting.UserType == CRMUserType.LabUser) {
                $("#item-container tbody a.btn-upload-files").html('Upload&nbsp;<i class="fa fa-upload"></i>').removeClass("hide");

            }
            else {
                $("#item-container tbody a.btn-upload-files").html('View File&nbsp;<i class="fa fa-eye"></i>').removeClass("hide");
            }
            $("#item-container tbody a.btn-upload-files").removeClass("hide");
            $this.AutoCompleteProductInit();
            if (complaints.length == 0)
                $this.AddItem();
        },
        LoadProducts: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetProducts",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Products = res.Data;
                        $this.AutoCompleteActualProductInit();
                        $this.AutoCompleteProductInit();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadFiles: function (id, element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            if (id == 0) {
                return;
            }
            $("#tbl-document-uploads tbody").html('');
            $("tbody.files").html('');
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?dataKey=GetComplaintFiles&complaintId=" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: ".model-upload-documents",
                blockMessage: "Loading ... please wait",
                success: function (res) {
                    if (res.Success) {
                        Current_ID = id;
                        var documents = res.Data;
                        var $form = $('#fileupload');
                        $form.fileupload('option', 'done').call($form, $.Event('done'), { result: { files: documents } });

                        //var html = "";
                        //for (var i in documents) {
                        //    html += "<tr>";
                        //    var document = documents[i];
                        //    html += "<td>" + document.Name + "</td>";
                        //    html += "<td><span class='preview'>";
                        //    html += "<a target='_blank' href='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "' title='View Large Image'><img style='max-height: 80px;' src='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "'></a>";
                        //    html += "</span></td>";
                        //    html += "<td><button class='btn btn-danger delete btn-sm'><i class='glyphicon glyphicon-trash'></i><span>Delete</span></button><input type='checkbox' class='checker'></td>";
                        //    html += "</tr>";
                        //}
                        //$("#tbl-document-uploads tbody").html(html);
                        App.initUniform();
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
            var id = Current_ID;
            if (Common.Validate($(".model-upload-documents"))) {
                var scope = $(".model-upload-documents");
                var record = new Object();
                var files = new Array();
                $("tbody.files tr td:last-child", scope).each(function () {
                    var file = $(this).children("input.data").attr("data-file");
                    if (typeof file != 'undefined' && file != null) {
                        file = $.parseJSON(file);
                        file["CRMComplaintId"] = id;
                        files.push(file);
                    }

                });
                record = { '': files }

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?savedocuments=savedocuments&complaintId=" + id,
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
        LoadProjects: function (projectId) {
            var $this = this;
            var customerId = $("select#CustomerId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetProjectsWithPMCDetail&customerId=" + customerId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData.Projects = res.Data;
                        Common.BindSelect(PageData.Projects, $("#ProjectId"), true);
                        if (projectId > 0) {
                            $("#ProjectId").select2("val", projectId)
                        }
                        $this.SelectActualProduct();
                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        AutoCompleteActualProductInit: function () {
            var $this = this;
            var products = Enumerable.From(PageData.Products).Where(function (x) { return x.IsOwnProduct == false }).ToArray();
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        Id: product.Id,
                        Name: product.Name + "(Division:" + product.Division + " Vendor:" + product.Vendor + ")",

                    }
                );
            }

            Common.BindSelect(suggestion, $("#ActualProductId"), true);

            //$(".ActualProduct").autocomplete({
            //    source: suggestion,
            //    selectFirst: true,
            //    autoFocus: true,
            //    select: function (event, ui) {
            //        var tr = $(this).closest("tr");
            //        $(tr).find("input.ActualProductId").val(ui.item.id);
            //        $(tr).find("input.ActualProductDivision").val(ui.item.Product.Division);
            //        $(tr).find("input.Vendor").val(ui.item.Product.Vendor);
            //        //$(tr).find("input.Vendor").val(ui.item.Product.Vendor);
            //    }
            //});

        },
        AutoCompleteProductInit: function () {
            var $this = this;
            var products = Enumerable.From(PageData.Products).Where(function (x) { return x.IsOwnProduct == true }).ToArray();
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        Id: product.Id,
                        Name: product.Name + "(Division:" + product.Division + " Vendor:" + product.Vendor + ")",


                    }
                );
            }
            Common.BindSelect(suggestion, $("#ProductId"), true);
            Common.BindSelect(suggestion, $("#CounterProductIds"), true);



        },

        AutoCompleteProjectsInit: function ($tr) {
            var $this = this;
            var projects = PageData.Projects;
            var suggestion = new Array();
            for (var i in projects) {
                var project = projects[i];
                suggestion.push(
                    {
                        id: project.Id,
                        value: project.Name,
                        label: project.Name,
                        name: project.Name,
                        ProductId: project.ProductId,
                        Product: project.Product,
                        Project: project

                    }
                );
            }
            $(".Project", $tr).autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $($tr).find("input.ProjectId").val(ui.item.Project.Id);
                    $($tr).find("input.Project").val(ui.item.Project.Name);
                    $($tr).find("input.ProductId").val(ui.item.Project.ProductId);
                    $($tr).find("input.Product").val(ui.item.Project.Product);
                    $($tr).find("input.ActualProductId").val(ui.item.Project.ActualProductId);
                    $($tr).find("input.ActualProduct").val(ui.item.Project.ActualProduct);
                    $($tr).find("input.ActualProductDivision").val(ui.item.Project.ActualProductDivision);


                }
            });

        },
        SelectActualProduct: function () {

            var projectId = Common.GetInt($("select#ProjectId").val());
            if (projectId > 0) {
                var project = Enumerable.From(PageData.Projects).Where(function (x) { return x.Id == projectId }).FirstOrDefault();
                if (project != null) {
                    $("#ActualProductId").select2("val", project.ActualProductId).select2("enable", false);
                    $("#ProductId").select2("val", project.ProductId).select2("enable", false);


                }
            }

        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },

        Delete: function (id, $tr) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "item-container",
                    blockMessage: "deleting...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $($tr).remove();
                            Common.ShowMessage(true, { message: "Record deleted successfully." });
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

    };
}();
