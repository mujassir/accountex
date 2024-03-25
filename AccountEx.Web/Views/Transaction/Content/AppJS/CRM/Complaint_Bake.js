
var Complaint = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "CRMComplaint";
    var LIST_LOADED = false;
    var focusElement = "#InvoiceNumber";
    var UPLOAD_FOLDER = "ComplaintsFiles";
    var PageData = new Object();
    var PageSetting = new Object();
    var $Current_TR = null;
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
            $(document).on("click", ".btn-upload-files", function (event) {
                $this.LoadFiles($(this));
            });
            $(document).on("change", "#RegionId", function (event) {
                $this.LoadComplaints();
            });
            $(document).on("change", "select.CustomerId", function (event) {
                $this.LoadProjects($(this));
            });
            $(document).on("change", "select.StatusId", function (event) {
                $this.ChangeStatus($(this));
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

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
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
            $Current_TR = null;
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
                        $this.MapComplaints([res.Data]);
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

        Save: function (callback) {

            var $this = this;
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();

            if (Common.Validate($("#form-info"))) {

                Items = Common.SaveItemData();
                //Items = Enumerable.From(Items).Where("$.ProductName.trim()!=''").ToArray();
                var err = "";
                //if (typeof party == "undefined" || party == null) {
                //    err += "" + $("#AccountCode").val() + " is not valid party code.,";
                //}
               
                for (var i in Items) {
                    var item = Items[i];
                    if (!Common.isNullOrWhiteSpace(item.TestTypeId))
                        item.TestTypeId = item.TestTypeId.join(",");
                }
                if (Items.length <= 0) {
                    err += "Please add atleast one item.,";
                }
                //if (Common.GetInt(record.NetTotal) <= 0) {
                //    err += "Transaction total amount should be graeter than zero(0).,";
                //}
                if (err.trim() != "") {
                    Common.ShowError(err);
                    return;
                }
                record =
                    {
                        '': Items
                    };
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },
        ChangeStatus: function ($elment) {

            var $this = this;
            var scope = $("#form-info");
            var Items = new Array();
            var $tr = $elment.closest("tr");
            var id = Common.GetInt($($tr).find("input.Id").val());
            if (id == 0)
                return;
            var statusId = Common.GetInt($elment.val());
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
                            if (statusId == CRMComplaintStatus.Resolved) {
                                $tr.find("ResolvedById").val(data.UserId);
                                $tr.find("ResolvedDate").val(data.Date);
                            }
                            else if (statusId == CRMComplaintStatus.Closed) {
                                $tr.remove();
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
        LoadFiles: function (element) {
            var $this = this;
            var $tr = $(element).closest("tr");
            var id = Common.GetInt($($tr).find("input.Id").val());
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
                        $Current_TR = $tr;
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
            var id = Common.GetInt($($Current_TR).find("input.Id").val());
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
        LoadProjects: function ($element) {
            var $this = this;
            var $tr = $element.closest("tr");
            var customerId = $tr.find("select.CustomerId").val();
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

                        if (res.Data.length == 1) {
                            var project = res.Data[0];
                            $($tr).find("input.ProjectId").val(project.Id);
                            $($tr).find("input.Project").val(project.Name);
                            $($tr).find("input.ProductId").val(project.ProductId);
                            $($tr).find("input.Product").val(project.Product);
                            $($tr).find("input.ActualProductId").val(project.ActualProductId);
                            $($tr).find("input.ActualProduct").val(project.ActualProduct);
                            $($tr).find("input.ActualProductDivision").val(project.ActualProductDivision);
                        }
                        else {
                            $this.AutoCompleteProjectsInit($tr);
                            $($tr).find("input.Project").val("");
                            $($tr).find("input.ProductId").val(0);
                            $($tr).find("input.Product").val("");
                            $($tr).find("input.ActualProductId").val('');
                            $($tr).find("input.ActualProduct").val('');
                        }
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
                        id: product.Id,
                        value: product.Name,
                        label: product.Name,
                        name: product.Name,
                        Product: product

                    }
                );
            }
            $(".ActualProduct").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ActualProductId").val(ui.item.id);
                    $(tr).find("input.ActualProductDivision").val(ui.item.Product.Division);
                    $(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                    //$(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

        },
        AutoCompleteProductInit: function () {
            var $this = this;
            var products = Enumerable.From(PageData.Products).Where(function (x) { return x.IsOwnProduct == true }).ToArray();
            var suggestion = new Array();
            for (var i in products) {
                var product = products[i];
                suggestion.push(
                    {
                        id: product.Id,
                        value: product.Name,
                        label: product.Name,
                        name: product.Name,
                        Product: product

                    }
                );
            }
            $(".Product").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {
                    var tr = $(this).closest("tr");
                    $(tr).find("input.ProductId").val(ui.item.id);
                    //$(tr).find("input.Division").val(ui.item.Product.Division);
                    //$(tr).find("input.Vendor").val(ui.item.Product.Vendor);
                }
            });

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
