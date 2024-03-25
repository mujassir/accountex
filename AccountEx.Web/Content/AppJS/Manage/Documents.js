
var Documents = function () {
    var max = 0;
    var DATATABLE_ID = "tbl-document-uploads";
    var API_CONTROLLER = "Document";
    var UPLOAD_FOLDER = "AgreementFiles";
    var PageSetting = new Object();
    var LIST_CHANGED = false;
    var LIST_LOADED = false;
    var BRANCHID = 0;
    var ID = 0;
    var DATA = null;
    return {
        init: function () {
            var $this = this;
         
            $this.LoadLeafAccount();
            $this.ListView();
        },
        SaveDocuments: function () {
            var $this = this;
            var accountId = $("#AccountId").val();
            if (Common.Validate($(".model-upload-documents"))) {
                var scope = $(".model-upload-documents");
                var record = new Object();
                var files = new Array();
                $("tbody.files tr td:last-child", scope).each(function () {
                    var file = $(this).children("input.data").attr("data-file");
                    if (typeof file != 'undefined' && file != null) {
                        file = $.parseJSON(file);
                        file["AccountId"] = accountId;
                        files.push(file);
                        
                    }

                });
                record = { '': files }

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?savedocuments=savedocuments",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: ".model-upload-documents",
                    blockMessage: "saving document..please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Document saved successfully." });
                            DataTable.RefreshDatatable(DATATABLE_ID);
                            $('#documents tbody').html("");
                            Common.Clear();
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
        LoadDocuments: function (id) {
            var $this = this;
            ID = id;
            $("#tbl-document-uploads tbody").html('');
            $("tbody.files").html('');
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?laodDatakey=loadDocuments" + "&id=" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: ".model-upload-documents",
                blockMessage: "Loading ... please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data;
                        var documents = d.Documents;
                        var html = "";
                        for (var i in documents) {
                            html += "<tr>";
                            var document = documents[i];
                            html += "<td>" + document.Name + "</td>";
                            html += "<td><span class='preview'>";
                            html += "<a target='_blank' href='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "' title='View Large Image'><img src='../upload/" + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + document.Url + "'></a>";
                            html += "</span></td>";

                            //html += "<td><a class='mix-preview fancybox-button' target='_blank' href='../upload/ApplicationProcess/" + document.Url + "' title='View Large Image' data-rel='fancybox-button'><img class='img-responsive' style='max-height:200px;'  src='../upload/ApplicationProcess/" + document.Url + "' alt='' data-rel='fancybox-button'></a></td>";
                            //html += "<td><a class='mix-preview fancybox-button' target='_blank' href='../upload/VehicleAgreements/'" + documents.Url + "' title='View Large Image' data-rel='fancybox-button'>View Large Image</a></td>";
                            //html += "<td><button type='button' class='btn yellow btn-xs' onclick='VehicleSale.PrintReceipt(" + document.Id + ")'><i class='fa fa-print'></i>&nbsp;Delete</button>";
                            html += "</tr>";
                        }
                        $("#tbl-document-uploads tbody").html(html);
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
        DeleteRow: function (elment) {

            var $this = this;
            $(elment).closest("tr").remove();
          
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
        Delete: function (id) {
          
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting Document...please wait",
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
        LoadLeafAccount: function () {
            var leafAccount = Common.GetAllLeafAccounts();
            Common.BindSelect(leafAccount, "#AccountId", true);
        },
        ListView: function () {
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
    };
}();
