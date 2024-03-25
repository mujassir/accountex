
var Project = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Project";
    var UPLOAD_FOLDER = "ProjectFiles";
    var LIST_LOADED = false;
    return {
        init: function () {
            var $this = this;

            //if (Setting.PageLandingView == "DetailView") {
            //    $this.Add();
            //}
            //else {
            $this.ListView();
            //}
            $("#GrossCost,#GSTPercent,#WHTPercent").keyup(function () {
                $this.CalculateTax();
            });
            $("#GST,#Miscellaneous").keyup(function () {
                $this.CalculateNetCost();
            });
            $("input.Planned").keyup(function () {
                $this.CalculatePlanCost();
            });
            $("input.Actual").keyup(function () {
                $this.CalculateActualCost();
            });
            $this.GetClients();
            $this.GetEmployees();
            $("#PoCopy").fileinput({
                uploadUrl: "../Handlers/FileUpload.ashx?directory=" + UPLOAD_FOLDER,
                showUpload: true,
                dropZoneEnabled: false,
                overwriteInitial: true,
                showCaption: false,
                browseLabel: "Select Scan PO Copy"
            });
            Common.BindFileInputEvents("PoCopy", "PictureUrl");
        },

        Add: function () {
            var $this = this;
            $this.DetailView();

            $this.CustomClear();
            $("#div-ProjectReceipts").addClass("hide");
            $this.GetNextProjectNumber();

        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        ListView: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            var url = Setting.APIBaseUrl + API_CONTROLLER;
            if (!LIST_LOADED) {
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            } else {
                DataTable.RefreshDatatable(DATATABLE_ID, url);

            }
        },
        CalculateTax: function () {
            var gstpercent = Common.GetFloat($("#GSTPercent").val());
            var cost = Common.GetFloat($("#GrossCost").val());
            if (gstpercent > 0 && cost > 0) {
                var saletax = cost - ((cost * 100) / (100 + gstpercent));
                $("#GST").val(saletax.toFixed(2));
            }
            else
                $("#GST").val("0.0");

            var whtpercent = Common.GetFloat($("#WHTPercent").val());
            if (whtpercent > 0 && cost > 0) {
                var whttax = cost * whtpercent / 100;
                $("#WHT").val(whttax.toFixed(2));
            } else
                $("#WHT").val("0.0");

            this.CalculateNetCost();
        },
        CalculateNetCost: function () {
            var grossCost = Common.GetFloat($("#GrossCost").val());
            $("#GST,#WHT").each(function () {
                grossCost -= Common.GetFloat($(this).val());
            });
            $("#NetCost").val(grossCost);

        },
        CalculatePlanCost: function () {
            var total = 0.0;
            $("input.Planned").each(function () {
                total += Common.GetFloat($(this).val());
            });
            $("#lblPlannedtotal").html(total.format());


        },
        CalculateActualCost: function () {
            var total = 0.0;
            $("input.Actual").each(function () {
                total += Common.GetFloat($(this).val());
            });
            $("#lblActualtotal").html(total.format());

        },


        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {

            AllowNumerics();
            //$(".select2").select2();
            $("select").each(function () {
                $(this).select2();
            });


        },
        CustomClear: function () {
            Common.Clear();
            $("#GSTPercent").val("16");
            $("#lblPlannedtotal,#lblActualtotal").html("0.0");
            $("#WHTPercent").val("8");
            $("#Employeest option:selected").removeAttr("selected");
            // $('#PoCopy').fileinput('clear');
            $(".fileinput-remove-button").click();
            $("#Employees").multiSelect("deselect_all");

        },

        Save: function () {
            var $this = this;
            if (Common.Validate("#form-info")) {
                var record = Common.SetValue($("#form-info"));
                if ($("#Id").val() != null && $("#Id").val() != "")
                    record.Id = $("#Id").val();
                record["Employees"] = $this.GetMultiSelectValue("Employees");
                record.AccountTitle = $("#AccountId option:selected").text();
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving projects ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $("#Id").val(res.Data);
                            ProjectReceipts.ProjectId = res.Data;
                            var url = Setting.APIBaseUrl + API_CONTROLLER;
                            if (!LIST_LOADED) {
                                LIST_LOADED = true;
                                DataTable.BindDatatable(DATATABLE_ID, url);
                            } else {
                                DataTable.RefreshDatatable(DATATABLE_ID, url);
                            }
                            //Common.ShowMessage(true, { message: "Changes saved successfully!" });
                            //bootbox.confirm("Project saved successfully! Do you want to add payments", function (result) {
                            //    if (result) {
                            //        $("#div-ProjectReceipts").removeClass("hide");
                            //        //$this.ListView();

                            //    }
                            //    else {
                            //        $this.ListView();

                            //    }
                            //});
                            $this.CustomClear();
                            bootbox.confirm({
                                //title: 'danger - danger - danger',
                                message: "Project saved successfully! Do you want to add payments",
                                buttons: {
                                    'confirm': {
                                        label: "Yes! Add Payments",
                                        className: "btn-success"
                                    },
                                    'cancel': {
                                        label: "No! Back to List",
                                        className: "btn-default"
                                    }

                                },
                                callback: function (result) {
                                    if (result)
                                        $("#div-ProjectReceipts").removeClass("hide");
                                    else
                                        $this.ListView();
                                }
                            });

                        } else {
                            Common.ShowError(res.Error);
                        }

                    },
                    error: function (e) {
                        Common.ShowError(e);
                    }
                });
            }
        },
        GetMultiSelectValue: function (element) {
            var tokens = [];
            $("#" + element + " :selected").each(function (i, selected) {
                tokens[i] = $(selected).val();
            });
            return tokens.join(",");
        },

        Edit: function (id, obj, isprint) {

            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id + "?type=" + $this.GetType(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#div-form",
                blockMessage: "Loading project ...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.Clear();

                        if (typeof isprint != "undefined" && isprint) {
                            ////////////////////////////////////
                            $("#PR_Print_mainTable tbody").html("");
                            var html = "";
                            var data = res.Data.Vouchers;
                            for (var i in data) {
                                var item = data[i];
                                var voucherItems = item.VoucherItems;
                                var item = Enumerable.From(voucherItems).FirstOrDefault(null, function (p) { return p.Debit > 0 });
                                if (item != null) {
                                    date = moment(item.Date).format("DD/MM/YYYY");
                                    html += "<tr><td>" + item.VoucherNumber + "</td>";
                                    html += "<td>" + date + "</td>";
                                    html += "<td>" + item.AccountName + "</td>";
                                    html += "<td>" + item.Description + "</td>";
                                    html += "<td>" + item.Debit.format() + "</td></tr>";
                                }


                            }
                            $("#PR_Print_mainTable tbody").html(html);

                            ////////////////////////////////////

                            var scope = $("#div-report");
                            if (res.Data.data == "StartDate" || res.Data.data == "EndDate") {
                                Common.ChangeDateFormate(res.Data.data.val());
                            }
                            Common.MapDataWithPrefix(res.Data.data, scope, "lbl", "html");
                            $("#lblDate").text(moment(res.Data.data.POIssueDate).format("DD/MM/YYYY"));

                            $("#lblStartDate").text(moment(res.Data.data.StartDate).format("DD/MM/YYYY"));
                            $("#lblEndDate").text(moment(res.Data.data.EndDate).format("DD/MM/YYYY"));
                            var total = res.Data.data.GrossCost + res.Data.data.GST + res.Data.data.WHT + res.Data.data.Miscellaneous;
                            $("#lblTotal").text(Common.GetInt(total).format());

                            var receipt = $(obj).closest("tr").find(":nth-child(9)").html();
                            receipt = parseFloat(receipt.trim().replace(/[^0-9\.]+/g, ""));
                            $("#lblReceipts").text(Common.GetInt(receipt).format());
                            var balance = parseFloat(total) - parseFloat(receipt);
                            $("#lblBalancePrint").text(Common.GetInt(balance).format());

                            Common.MapEditData(res.Data.data, scope);
                            ProjectReceipts.ListView();
                            $this.CalculateActualCost();
                            $this.CalculatePlanCost();
                            $this.Print();
                        }
                        else {

                            var scope = $("#form-info");
                            Common.MapEditData(res.Data.data, scope);
                            $("#div-ProjectReceipts").removeClass("hide");
                            ProjectReceipts.ProjectId = id;
                            Common.MapEditData(res.Data.data, scope);

                            $(".date-picker,.ac-date").each(function () {
                                Common.SetDate(this, res.Data.data[$(this).attr("Id")]);
                            });
                            if (typeof res.Data.data.Employees != undefined && res.Data.data.Employees != null) {
                                var emp = res.Data.data.Employees.split(",");
                                $("#Employees").val(emp);
                            }
                            var url = res.Data.data.PictureUrl;
                            if (url != null && url != "") {
                                $("#PoCopy").fileinput("refresh",
                                    {
                                        overwriteInitial: true,
                                        initialPreview: [

                                              "<img src=\"" + Setting.UploadRootFolder + UPLOAD_FOLDER + "/" + url + "\" class=\"file-preview-image\">"
                                        ],
                                        //initialPreviewConfig: [
                                        //    { caption: "Desert.jpg", width: "120px", url: "alert('kr')", key: 1 }

                                        //],
                                    }
                                );
                            }
                            else {
                                $(".fileinput-remove-button").click();

                            }
                            Common.BindFileInputEvents("PoCopy", "PictureUrl");
                            $("#Employees").multiSelect("refresh");
                            $this.DetailView();
                            $this.CalculateActualCost();
                            $this.CalculatePlanCost();
                            ProjectReceipts.ListView();
                        }
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        //Edit function before Print
        //Edit: function (id) {
        //   var $this = this;
        //    Common.WrapAjax({
        //        url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
        //        type: "GET",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (res) {
        //            if (res.Success) {
        //                Common.Clear();
        //                var scope = $("#form-info");
        //                $("#div-ProjectReceipts").removeClass("hide");
        //                ProjectReceipts.ProjectId = id;
        //                Common.MapEditData(res.Data, scope);

        //                $(".date-picker").each(function () {
        //                    Common.SetDate(this, $(this).val());
        //                });
        //                if (typeof res.Data.Employees != undefined && res.Data.Employees != null) {
        //                    var emp = res.Data.Employees.split(',');
        //                    $("#Employees").val(emp);
        //                }
        //                var url = res.Data.PictureUrl;
        //                if (url != null && url != "") {
        //                    $('#PoCopy').fileinput('refresh',
        //                        {
        //                            overwriteInitial: true,
        //                            initialPreview: [

        //                                  '<img src="' + Setting.UploadRootFolder + UPLOAD_FOLDER + '/' + url + '" class="file-preview-image">'
        //                            ],
        //                            //initialPreviewConfig: [
        //                            //    { caption: "Desert.jpg", width: "120px", url: "alert('kr')", key: 1 }

        //                            //],
        //                        }
        //                    );
        //                }
        //                else {
        //                    $(".fileinput-remove-button").click();

        //                }
        //                Common.BindFileInputEvents("PoCopy", "PictureUrl");
        //                $("#Employees").multiSelect("refresh");
        //                $this.DetailView();
        //                $this.CalculateActualCost();
        //                $this.CalculatePlanCost();
        //                ProjectReceipts.ListView();
        //            }
        //            else {
        //                Common.ShowError(res.Error);
        //            }

        //        },
        //        error: function (e) {
        //        }
        //    });
        //},

        Delete: function (id) {
            var $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-form",
                    blockMessage: "Deleting project ...please wait",
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

        GetClients: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetClients",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindSelect(res.Data, "#AccountId", true);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetEmployees: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA/?key=GetEmployees",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        Common.BindMultiSelect(res.Data, "#Employees", true);
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
            var $this = this;
            //var type = $this.GetType().toLowerCase();
            //$("#form-info").addClass("hide");
            //$("#div-report").removeClass("hide");
            //setTimeout(function () {
            //    window.print();
            //    $("#div-report").addClass("hide");
            //}, 500);
            window.print();
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        GetNextProjectNumber: function () {
            var $this = this;

            Common.WrapAjax({
                url: "../Project/GetNextProjectNumber",
                type: "POST",
                data: "",
                success: function (res) {
                    var q = JSON.parse(res);
                    if (q.Success) {
                        $("#Number").val(q.Data);
                        //$("#lblVoucherNumber").html(q.Data);
                    }
                    else {
                        Common.ShowError(q.Data);
                    }
                },
                error: function (e) {
                }
            });
        },

    };
}();
