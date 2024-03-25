

var ShopSetup = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "ShopSetup";
    var UPLOAD_FOLDER = "ShopDocuments";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    return {
        init: function () {
            var $this = this;
            this.ListView();
            $this.GetNextShopCode();
            Common.InitNumerics();
            //Common.GetNextAccountCode(API_CONTROLLER);

            //$('input#Length').blur(function () {
            //    var num = Common.GetFloat($(this).val());
            //    var cleanNum = num.toFixed(2);
            //    $(this).val(cleanNum);
            //    if (num / cleanNum < 1) {
            //        Common.ShowError('Please enter only 2 decimal places...we have truncated extra points');
            //    }
            //});
            //$('input#Width').blur(function () {
            //    var num = Common.GetFloat($(this).val());
            //    var cleanNum = num.toFixed(2);
            //    $(this).val(cleanNum);
            //    if (num / cleanNum < 1) {
            //        Common.ShowError('Please enter only 2 decimal places...we have truncated extra points');
            //    }
            //});
          
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });

            //$(document).on("keyup blur", "#Length, #Width", function (event) {
            //    var length = Common.GetFloat($("#Length").val());
            //    var width = Common.GetFloat($("#Width").val());
            //    var total = length * width;
            //    $("#TotalArea").val(total.toFixed(2));
            //});
            $(document).on("keyup blur", "#GroundFloor, #InterFlooring, #Basement, #FirstFloor", function (event) {
                var groundFloor = Common.GetFloat($("#GroundFloor").val());
                var interFlooring = Common.GetFloat($("#InterFlooring").val());
                var basement = Common.GetFloat($("#Basement").val());
                var firstFloor = Common.GetFloat($("#FirstFloor").val());
                var total = groundFloor + interFlooring + basement + firstFloor;
                $("#TotalArea").val(total.toFixed(2));
            });
            //Common.BindFileInput("Documents", "FileUrl", "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER);
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $this.CustomClear();
            $this.DetailView();
            $this.GetNextShopCode();
        },
        DetailView: function () {
            $('#form-info').removeClass('hide');
            $('#div-table').addClass('hide');
        },
        ListView: function () {

            var $this = this;
            $('#form-info').addClass('hide');
            $('#div-table').removeClass('hide');
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $this.CustomClear();
            //Common.GetNextAccountCode(API_CONTROLLER);
        },

        Close: function () {
            $('#div-form').addClass('hide');
            $('#div-table').removeClass('hide');
        },
        ReinializePlugin: function () {
            var $this = this;
            Common.AllowNumerics();
            $("select").each(function () {
                $(this).select2();
            });
        },
        CustomClear: function () {
            Common.Clear();
            $("#Name").focus();
            $("tbody.files").html("");
        },
        GetNextShopCode: function () {
            var $this = this;
            $.ajax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/?key=GetNextShopCode",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#ShopCode").val(res.Data);
                        $("#ShopNo").focus();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        Save: function () {

            var $this = this;
            $this.SaveRecord(function () {
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                LIST_CHANGED = true;
                $this.ListView();
                $this.CustomClear();
            });
        },
        SaveClose: function () {
            var $this = this;
            this.SaveRecord(function () {
                LIST_CHANGED = true;
                //Common.GetNextAccountCode(API_CONTROLLER);
                $this.ListView();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue("#form-info");
                
                record["Block"] = $("#BlockId").find(':selected').attr('data-blockName');
                var files = new Array();
                $("tbody.files tr td:last-child").each(function () {
                    var file = $.parseJSON($(this).children("input.data").attr("data-file"));
                    if (typeof file != 'undefined' && file != null) {
                        file["ShopId"] = record.Id;
                        files.push(file);
                    }
                });
                record["ShopDocuments"] = files;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    success: function (res) {
                        if (res.Success) {

                            callback();
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

        Edit: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        Common.MapEditData(res.Data, "#form-info");
                        Common.SetCheckValue(data);
                        //Common.SetDate("#FromDate", data.FromDate);
                        //Common.SetDate("#ToDate", data.ToDate);
                        //Common.RefreshFileInput("Documents", "FileUrl", data.FileUrl, UPLOAD_FOLDER);
                        $(".files").html("");
                        var files = data.ShopDocuments;
                        var $form = $('#fileupload');
                        $form.fileupload('option', 'done').call($form, $.Event('done'), { result: { files: files } });
                        $this.DetailView();
                        $("#ShopNo").focus();
                        Common.SetCheckValue(data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
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
                            $this.CustomClear();
                            //Common.GetNextAccountCode(API_CONTROLLER);
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
        DeleteFile: function (element) {
            var $this = this;
            $(element).parent().parent().remove();


        },
        DeleteAllFile: function (element) {
            var $this = this;
            $("tbody.files tr.template-download td:nth-child(4) input:checked").each(function () {
                _this.DeleteFile($(this));
            });

        },
    }
}();




