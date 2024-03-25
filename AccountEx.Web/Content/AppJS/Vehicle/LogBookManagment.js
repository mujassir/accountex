
var LogBookManagment = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "VehicleLogBook";
    var UPLOAD_FOLDER = "LogBookScanned";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    var CURRENT_ROW = null;
    var STATUS = 0;
    return {
        init: function () {
            var $this = this;
            $(document).on("change", "select.StatusId", function (event) {
                var tr = $(this).closest("tr");
                var isFinal = $(tr).find("select.StatusId option:selected").attr("data-isfinal");
                if (isFinal == "True") {
                    $(tr).find("select.AssignedBranchId,input.PurchasePrice,input.SalePrice").prop("disabled", false);
                }
                else {
                    $(tr).find("select.AssignedBranchId,input.PurchasePrice,input.SalePrice").prop("disabled", true);
                }

            });
            $(document).on("change", "#PaymentMode", function () {
                var paymentOptions = $(this).val();
                var scope = $(this).closest("div[data-save='save']")
                if (paymentOptions != "Bank") {
                    $(".banks-options", scope).addClass("hide");
                }
                else {
                    $(".banks-options", scope).removeClass("hide");
                }
            });
            $("tbody.files").on("click", ":button.delete", function () {
                $this.DeleteFile($(this));
            });
            $(".fileupload-buttonbar").on("click", ":button.delete", function () {
                $this.DeleteAllFile($(this));
            });
            $("#BranchId").change(function () {
                $this.LoadLogBookDetail();
            })
            $("#btnShowReport").click(function () {
                $this.LoadLogBookDetail();
            });
            $this.LoadLogBookDetail();
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $("#followup-container").modal("hide");

            });
        },
        DeleteFile: function (element) {
            $(element).parent().parent().remove();


        },
        DeleteAllFile: function (element) {
            var $this = this;
            $("tbody.files tr.template-download td:nth-child(4) input:checked").each(function () {
                $this.DeleteFile($(this));
            });

        },
        CustomClear: function () {
            $("input:radio[value='credit']").prop("checked", true);
            $.uniform.update();

            $("#item-container tbody,#table-dc-detail tbody").html("");
            $("#lblcurrentbalance").html("00");
            $("#lblpreviousbalance").html("00");
            $("#AccountCode").removeAttr("disabled");
            $("#btndelete,#btnprint").prop("disabled", true);
            Common.Clear();
        },
        LoadLogBookDetail: function () {
            var $this = this;
            var branchId = $("#BranchId").val();
            var soldStatus = $("#SoldStatus").val();
            var transfeerStatus = $("#TransfeerStatus").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?&branchId=" + branchId + "&soldStatus=" + soldStatus + "&transfeerStatus=" + transfeerStatus,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Logbook detail ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var html = "";
                        var records = res.Data;
                        for (var i in records) {

                            var record = records[i];
                            html += "<tr>";

                            html += "<td>" + record.SupplierName + "</td>";
                            html += "<input type='hidden' value='" + record.Id + "' class='Id' data-db-column='Id'>";
                            html += "<input type='hidden' value='" + record.Status + "' class='Status' data-db-column='Status'>"
                            html += "</td>";
                            html += "<td>" + Common.FormatDate(record.LogBookAppliedDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Type + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>";
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td>" + Common.FormatDate(record.LogBookReceivedDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.ReceivedBy + "</td>";
                            html += "<td><a onclick='LogBookManagment.ViewScan(this," + LogBookStatus.Apply + ")'>View</a></td>";
                            html += "<td>" + record.SoldStatus + "</td>";
                            html += "<td>" + record.ClearedBy + "</td>";
                            html += "<td>" + record.TransfeeredToWhom + "</td>";
                            html += "<td><a onclick='LogBookManagment.ViewScan(this," + LogBookStatus.Transferred + ")'>View</a></td>";
                            html += "<td class='td-actions'>";
                            html += "<button type='button' class='btn btn-success btn-xs btn-logbook-apply hide' onclick='LogBookManagment.Process(this," + LogBookStatus.Apply + ")'>Apply</button>";
                            html += "<button type='button' class='btn btn-info btn-xs btn-logbook-received hide' onclick='LogBookManagment.Process(this," + LogBookStatus.Received + ")'>Receive</button>";
                            html += "<button type='button' class='btn btn-danger btn-xs btn-logbook-transferred hide' onclick='LogBookManagment.Process(this," + LogBookStatus.Transferred + ")'>Transfeer to Customer</button>";
                            html += "</td>";
                            html += "</tr>";

                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";

                        $("#item-container tbody").html(html);
                        PageData = res.Data;
                        $this.CheckFinalStatus();

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },
        Process: function (element, status) {
            var $this = this;
            $("tbody.files").html("");
            var scope = $("#modal-logbook");
            Common.ClearByScope(scope);
            var tr = $(element).closest("tr");
            STATUS = status;
            CURRENT_ROW = tr;
            var vehicleId = $(tr).find("input.Id").val();
            $("#Id").val(vehicleId);
            var vehicle = Enumerable.From(PageData).Where(function (p) { return p.Id == vehicleId }).FirstOrDefault();
            Common.MapDataWithPrefixF(vehicle, scope, "lbl", "html");
            var title = "Apply for Logbook";
            if (status == LogBookStatus.Received) {
                title = "Receive Logbook";
            }
            else if (status == LogBookStatus.Transferred) {
                title = "Transfeer Logbook to Customer";
            }
            $("#modal-logbook .modal-title span").html(title);
            $("#modal-logbook").modal("show");



        },

        ViewScan: function (element, status) {
            var $this = this;
            var tr = $(element).closest("tr");
            STATUS = status;
            CURRENT_ROW = tr;
            var vehicleId = $(tr).find("input.Id").val();
            var branchId = $("#BranchId").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?&branchId=" + branchId + "&vehicleId=" + vehicleId + "&viewScan=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Logbook detail ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var scope = $("#modal-logbook-view-scan");
                        var records = res.Data;
                        Common.ClearByScope(scope);
                        $("#Id").val(vehicleId);
                        var vehicle = Enumerable.From(PageData).Where(function (p) { return p.Id == vehicleId }).FirstOrDefault();
                        Common.MapDataWithPrefixF(vehicle, scope, "lbl", "html");
                        var title = "View Scan";
                        if (status == LogBookStatus.Transferred) {
                            title = "View Transeered Scan";
                            records = Enumerable.From(records).Where(function (p) { return p.Type == LogBookStatus.Transferred }).ToArray();
                        }
                        else {
                            records = Enumerable.From(records).Where(function (p) { return p.Type == LogBookStatus.Apply || p.Type == LogBookStatus.Received }).ToArray();
                        }
                        $("#modal-logbook-view-scan .modal-title span").html(title);
                        $("#modal-logbook-view-scan").modal("show");

                        var html = "";
                        var groupData = Enumerable.From(records).GroupBy("$.Type", null,
              function (key, g) {
                  var result = {
                      Type: key,
                      Records: g.ToArray(),
                  }
                  return result;
              }).ToArray();


                        for (var i in groupData) {
                            var data = groupData[i];
                            var TypeData = data.Records;
                            var subType = "Apply Scan";
                            if (data.Type == LogBookStatus.Received)
                                subType = "Recieved Scan";
                            else if (data.Type == LogBookStatus.Transferred)
                                subType = "Transferred Scan";
                            html += '<h3 class="font-green-sharp bold hidden-print">' + subType + '</h3>';
                            html += '<ul>';
                            var rowcount = 1;
                            for (var j in TypeData) {
                                var file = TypeData[j];

                                html += '<li>';


                                html += '<div class="mix mix-category"><input class="data hide" data-file=\'' + JSON.stringify(file) + '\' />' +
                        '<div class="mix-inner">' +
                           '<img class="img-responsive" style="max-height:200px;"  src="../upload/' + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + file.Url + '" alt="" data-rel="fancybox-button"></div><div class="mix-details">';
                                html += '<a class="mix-preview fancybox-button" target="_blank" href="../upload/' + Common.GlobalUploadFolder + "/" + UPLOAD_FOLDER + "/" + file.Url + '" title="View Large Image" data-rel="fancybox-button"><i class="fa fa-search"></i>  </a></div></div>';
                                html += '</li>';

                            }


                            html += '</ul>';

                        }






                        $(".mix-grid ul").html(html);
                        //$(".mix-grid").mixitup();
                        //Metronic.initFancybox();
                        //$('.scroller').slimScroll({
                        //    height: '250px'
                        //});





                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });




        },

        SaveRecord: function () {
            var $this = this;
            if (Common.Validate($("#modal-logbook"))) {
                var record = Common.SetValue($("#modal-logbook"));
                record.Type = STATUS;
                var files = new Array();
                $("tbody.files tr td:last-child").each(function () {
                    var file = $.parseJSON($(this).children("input.data").attr("data-file"));
                    if (typeof file != 'undefined' && file != null) {
                        file["VehicleId"] = record.Id;
                        files.push(file);
                    }

                });
                record["VehicleLogBookScanes"] = files;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $("#modal-logbook").modal("hide");
                            $this.LoadLogBookDetail();

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
        CheckFinalStatus: function () {
            $("#item-container tbody tr").each(function () {
                var tr = $(this);
                $(tr).find("td.td-actions").find(":button").addClass("hide").prop("disabled", true);
                var vehicleId = $(tr).find("input.Id").val();
                var vehicle = Enumerable.From(PageData).Where(function (p) { return p.Id == vehicleId }).FirstOrDefault();
                if (vehicle != null) {
                    var status = vehicle.Status;
                    if (status == LogBookStatus.Default) {
                        $(tr).find("td.td-actions").find(".btn-logbook-apply").removeClass("hide").prop("disabled", false);
                    }
                    else if (status == LogBookStatus.Apply) {
                        $(tr).find("td.td-actions").find(".btn-logbook-received").removeClass("hide").prop("disabled", false);
                    }
                    else if (status == LogBookStatus.Received && vehicle.SoldStatus != "Not Sold") {
                        $(tr).find("td.td-actions").find(".btn-logbook-transferred").removeClass("hide").prop("disabled", false);
                    }
                    else if (status == LogBookStatus.Transferred) {
                        $(tr).find("td.td-actions").find(":button").addClass("hide").prop("disabled", true);
                    }
                }

            })
        },
        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#jsondata #data").html());
            for (var i in tokens) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
    }

}();