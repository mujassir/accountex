
var RentMonthlyLiability = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "RentMonthlyLiability";
    var DATATABLE_ID = "mainTable";
    var PageSetting = new Object();
    var focusElement = "#VoucherNumber";
    return {
        init: function () {
            var $this = this;

            $this.InitShortKeys();
            // $this.SelectCurrentMonthYear();
            $("#VoucherNumber").keyup(function (e) {
                if (e.which == 13) {
                    $(this).val() == "0" ? focusElement = "date" : "code";
                    $this.LoadVoucher("same");
                }
            });
            jQuery('body').on('change', '#chk-select-all', function () {
                var set = jQuery('#item-container tbody tr:not([data-generated="generated"],[data-received="received"]):visible td:nth-child(1) :checkbox');
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    $(this).prop("checked", checked).trigger("change");
                });
                jQuery.uniform.update(set);
            });

            $('#txtSearch').keyup(function () {
                filterTable(document.getElementById("txtSearch"), document.getElementById("item-container"));
            });
            $(document).on("keyup", ".Amount", function (event) {
                $this.GetWholeTotal();
            });

            $this.GetNextVoucherNumber();

            $("#Month,#Year").change(function () {
                $this.LoadVoucher("byMonthYear");
            });
        },
        New: function () {
            var $this = this;
            focusElement = "date";
            $this.LoadVoucher("nextvouchernumber");
        },
        SelectCurrentMonthYear: function () {
            var $this = this;
            var date = new Date();
            m = date.getMonth() + 1;
            y = date.getFullYear();
            $("#Month").select2("val", m);
            $("#Year").select2("val", y);


        },

        InitShortKeys: function () {
            var $this = this;
            $(document).on('keydown', null, 'alt+s', function () {
                $this.Save();
                return false;
            });
            $(document).on('keydown', null, 'alt+n', function () {

                $this.LoadVoucher("nextvouchernumber");
                return false;
            });
            $(document).on('keydown', null, 'alt+p', function () {

                $this.LoadVoucher("previous");
                return false;
            });
            $(document).on('keydown', null, 'alt+f', function () {

                $this.LoadVoucher("next");
                return false;
            });

        },
        DeleteRow: function (element) {
            var $this = this;
            var tr = $(element).closest("tr");
            var challanItemId = $(tr).find("input.Id").val();
            var type = VoucherType[$this.GetType()];

            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + challanItemId + "?key=DeleteSingleRentLiability";

                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting  " + $this.GetType() + " ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.LoadVoucher('same');
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



            //$(elment).parent().parent().parent().remove();
            //this.GetWholeTotal();
        },
        GetWholeTotal: function () {
            var amount = 0;
            $("#item-container tbody tr").each(function () {
                amount += Common.GetFloat($(this).find("input.Amount").val());
            });
            $("#item-container tfoot tr td:nth-child(3) input").val(amount);
        },
        Add: function () {
            var $this = this;
            Common.Clear();
            $("#form-info").removeClass("hide");
            $("#div-table,#div-report").addClass("hide");
            $("select", "#form-info").each(function () {
                $(this).val("").select2();
            });
            $this.CustomClear();
            $this.GetNextVoucherNumber();
            $this.DetailView();
            $(".container-message").hide();
        },
        ListView: function () {
            var $this = this;
            $("#form-info,#div-report").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=" + $this.GetType();
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        DetailView: function () {
            $("#form-info").removeClass("hide");
            $("#div-table").addClass("hide");
        },
        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },
        ReinializePlugin: function () {
            //$("#saleitem tbody .chooseninner").chosen();
            AllowNumerics();
            //SetDropDown();
        },
        CloseItem: function () {
            Common.Clear();
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            $("#form-info-item").addClass("hide");
            $("#masterdetail").removeClass("hide");
            $("#div-table-item").addClass("hide");
        },
        Print: function () {
            window.print();
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {

                $this.CustomClear();
                Common.ShowMessage(true, { message: "Record saved successfully." });
                //focusElement = "date";
                $this.GetNextVoucherNumber();
                Common.ShowMessage(true, { message: Messages.RecordSaved });


            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                if ($("#PrintVoucher").is(":checked")) {
                    $this.Print();
                }
                else {
                    var scope = $("#form-info-item");
                    // $this.Close();
                    $this.ListView();
                }

                $this.CustomClear();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;
            $(".container-message").hide();


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
                Items = Enumerable.From(Items).Where("$.IsSelected=='true' && ($.IsReceived=='false' || $.IsReceived=='' ) && $.Id==0").ToArray();


                //if (id > 0) {
                //    var prevoucherno = Common.GetInt(record.PreVoucherNumber);
                //    var newvoucherno = Common.GetInt(record.VoucherNumber);
                //    if (prevoucherno != newvoucherno) {
                //        err = "You cannot change voucher no.Please save with previous  voucher no (" + prevoucherno + ").,";
                //        Common.ShowError(err);
                //        return;
                //    }
                //}
                var amounttotal = $("#item-container tfoot tr td:nth-child(2) input").val();
                var err = "";
                if (Items.length <= 0) {
                    err += "Please select atleast on tenant to generate liability.,";
                }
                //if (amounttotal == 0) {
                //    err += "Can't add empty voucher.";
                //}
                //for (var i in Items) {
                //    var item = Items[i];
                //    if (item.TenantAccountCode.trim() == "") {
                //        err += "Tenant is required.,";
                //    }
                //    else if (item.TenantAccountCode.trim() != "") {
                //        var party = Common.GetByCode(item.TenantAccountCode);
                //        if (typeof party == "undefined" || party == null) {
                //            err += item.TenantAccountCode + " is not valid code.,";
                //        }
                //    }
                //    if (item.ChargesAccountCode.trim() == "") {
                //        err += "Charge is required.,";
                //    }
                //    else if (item.ChargesAccountCode.trim() != "") {
                //        var party = Common.GetByCode(item.ChargesAccountCode);
                //        if (typeof party == "undefined" || party == null) {
                //            err += item.ChargesAccountCode + " is not valid code.,";
                //        }
                //    }
                //    if (item.Amount <= 0) {
                //        err += item.TenantAccountName + " must have charges amount greater than zero.,";
                //    }
                //}
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                record["RentDetailItems"] = Items;
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
        CustomClear: function () {
            Common.Clear();
            $("#btnprint").prop("disabled", true);
            $("#item-container tbody").html("");
        },
        GetNextVoucherNumber: function () {
            var $this = this;
            $this.LoadVoucher("nextvouchernumber");
        },
        LoadVoucher1: function (key) {
            var $this = this;
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?key=" + key,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  " + $this.GetType() + " ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var data = res.Data;
                        var challan = data.Challan;
                        Common.MapEditData(challan, "#form-info");
                        if (challan == null) {
                            $("#VoucherNumber").val(res.Data.VoucherNumber);
                            $("#Month").select2("val", "");
                            $("#Year").select2("val", "");
                        }
                        else {
                            $(".td-delete,.delete-row").removeClass("hide");
                            $("#VoucherNumber").val(challan.VoucherNumber);
                            $("#Id").val(challan.Id);
                            $("#ChallanId").val(challan.Id);
                            Common.MapItemData(challan.ChallanItems, "#item-container", "#template-item", true);
                            $("#btndelete").prop("disabled", false);
                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        LoadVoucher: function (key) {
            var $this = this;

            var voucherno = Common.GetInt($("#VoucherNumber").val());
            var month = Common.GetInt($("#Month").val());
            var year = Common.GetInt($("#Year").val());

            var qs = "?key=" + key;
            qs += "&voucherno=" + voucherno;
            qs += "&month=" + month;
            qs += "&year=" + year;

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  rent detail ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $("#item-container tbody").html("");
                        var data = res.Data;
                        var challan = data.Challan;
                        Common.MapEditData(challan, "#form-info");
                        if (challan != null && challan.RentDetailItems != null && challan.RentDetailItems.length > 0) {
                            var items = challan.RentDetailItems;
                            for (var i in items) {
                                var item = items[i];
                                var chExtra = Enumerable.From(data.ChallanExtra).Where(function (p) { return p.ShopId == item.ShopId && p.TenantAccountId == item.TenantAccountId }).FirstOrDefault();
                                if (chExtra != null) {
                                    //delete vehicle.Id;
                                    //vehicle["VehicleId"] = item.VehicleId;
                                    $.extend(true, chExtra, item);
                                }
                            }
                        }
                        var challanExtra = data.ChallanExtra
                        $("#VoucherNumber").val(data.VoucherNumber);


                        var blocks = Enumerable.From(challanExtra).GroupBy("$.Block", null,
                function (key, g) {
                    var result = {
                        blockName: key,
                        blockData: g.OrderBy(function (p) { return Common.GetInt(p.ShopNo); }).ToArray(),
                    }
                    return result;
                }).ToArray();

                        var html = "";
                        $("#item-container tbody").html('');


                        for (var i in blocks) {
                            var block = blocks[i];
                            var items = block.blockData;
                            html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'><td colspan='11' style='text-align:center;'>" + block.blockName + "</td></tr>";
                            //html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'>";
                            //html += "<td style='width:7%;'>Shop No</td>";
                            //html += "<td style='width: 30%;'>Tenant Name</td>";
                            //html += "<td>Previous Reading</td>";
                            //html += "<td>Current Reading</td>";
                            //html += "<td>Units</td>";
                            //html += "<td>Electriciy Unit Charges</td>";
                            //html += "<td>Amount</td>";
                            html += "</tr>";

                            for (var i in items) {
                                var item = items[i];

                                html += '<tr data-parent="' + block.blockName + '">';
                                html += '<td><label class="control-label"><input type="checkbox" class="IsSelected" data-db-column="IsSelected" data-checktrack="false" value="false" data-toggle="tooltip" title="select to add in Liability"></label></td>';
                                html += '<td>';
                                html += '<input type="hidden" class="Id" data-db-column="Id" value="' + Common.GetInt(item.Id) + '">';
                                html += '<input type="hidden" class="IsReceived" data-db-column="IsReceived"  value="' + Common.GetBool(item.IsReceived) + '">';
                                html += '<input type="hidden" data-db-column="RentDetailId" value="' + Common.GetInt(item.RentDetailId) + '">';
                                html += '<input type="hidden" class="ShopId" data-db-column="ShopId" value="' + Common.GetInt(item.ShopId) + '">';
                                html += '<input type="text" disabled="disabled" class="form-control typeahead input-small ui-autocomplete-input hide" data-db-column="ShopNo" value="' + item.ShopNo + '">' + item.ShopNo + '</td>';
                                html += '<td>' + item.TenantAccountName + '';
                                html += ' <input type="hidden" data-db-column="TenantCode" value="' + item.TenantCode + '">';
                                html += '<input type="hidden" data-db-column="TenantAccountId" value="' + item.TenantAccountId + '">';
                                html += '<input type="hidden" data-db-column="Business" value="' + item.Business + '">';
                                html += '<input type="text" disabled="disabled" class="TenantAccountName hide form-control typeahead input-small ui-autocomplete-input" data-db-column="TenantAccountName" value="' + item.TenantAccountName + '">';
                                html += '</td>';
                                html += '<td>';
                                html += '<input type="hidden" data-db-column="RentAgreementId" value="' + item.RentAgreementId + '">';
                                html += '<input type="text" disabled="disabled" class=" form-control input-medium align-right" data-db-column="MonthlyRent" data-db-type="float" value="' + item.MonthlyRent + '">';
                                html += '</td>';

                                html += '<td>';

                                html += '<input type="text" disabled="disabled" class="form-control input-medium align-right" data-db-column="UCPercent" data-db-type="float" value="' + item.UCPercent + '">';
                                html += '</td>';
                                html += '<td>';
                                html += ' <input type="hidden" data-db-column="ElectricityUnitId" value="' + item.ElectricityUnitId + '">';
                                html += '<input type="hidden" data-db-column="ElectricityUnitItemId" value="' + item.ElectricityUnitItemId + '">';
                                html += '<input type="text" disabled="disabled" class="form-control input-small align-right" data-db-column="ElectricityCharges" data-db-type="float" value="' + item.ElectricityCharges + '">';
                                html += '</td>';
                                html += '<td>';

                                html += ' <input type="text" disabled="disabled" class="form-control input-small align-right" data-db-column="RentArrears" data-db-type="float" value="' + item.RentArrears + '">';
                                html += '</td>';
                                html += '<td>';

                                html += '<input type="text" disabled="disabled" class="form-control input-small align-right" data-db-column="UCPercentArears" data-db-type="float" value="' + item.UCPercentArears + '">';
                                html += '</td>';
                                html += '<td>';
                                html += ' <input type="text" disabled="disabled" class="ElectricityArrears form-control input-small align-right" data-db-column="ElectricityArrears" value="' + item.ElectricityArrears + '" >';
                                html += '</td>';
                                html += '<td>';
                                html += ' <input type="text" disabled="disabled" class="SurCharge form-control input-small align-right" data-db-column="SurCharge" value="' + item.SurCharge + '" >';
                                html += '</td>';

                                html += '<td class="td-actions">';
                                html += ' <input type="hidden" class="ChallanId" data-db-column="ChallanId" value="' + item.ChallanId + '">';
                                html += ' <span class="label label-info label-status hide label-sm"></span>';
                                html += ' <span class="action hide"><i class="fa fa-trash-o" onclick="RentMonthlyLiability.DeleteRow(this)" data-original-title="Delete Item"></i></span>';
                                html += ' </td>';
                                html += '</tr>';

                            }
                        }

                        $("#item-container tbody").html(html);

                        //Common.MapItemData(challanExtra, "#item-container", "#template-item", true);
                        $(".btndelete").prop("disabled", true);
                        $(".td-delete,.delete-row").addClass("hide");
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        var set = jQuery('#item-container tbody tr td:nth-child(1) :checkbox');
                        $this.SetFormControl();
                        Common.SetCheckChange();
                        App.initUniform();



                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

        Delete: function (id) {
            var $this = this;
            var challanId = $("#ChallanId").val();
            Common.ConfirmDelete(function () {
                var voucherno = Common.GetInt($("#VoucherNumber").val());
                var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + challanId;
                if (challanId <= 0) {
                    Common.ShowError("No Voucher found for deletion.");
                    return;
                }

                Common.WrapAjax({
                    url: url,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Deleting ... please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Record deleted successfully." });
                            $this.LoadRentDetail();
                            $this.GetNextVoucherNumber();
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
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        SetFormControl: function (challan) {
            $(".btndelete,.btnprint").prop("disabled", true);
            if (challan != null && challan.Id > 0) {
                $("#btndelete,#btnprint").prop("disabled", false);
            }
            $("#item-container tbody tr").each(function () {
                var tr = $(this);
                var Id = Common.GetInt($(tr).find("input.Id").val());
                var isReceived = Common.GetBool($(tr).find("input.IsReceived").val())
                var tenant = $(tr).find("input.TenantAccountName").val();
                if (!isReceived) {
                    if (Id > 0) {
                        $(tr).attr(
                            {
                                "data-generated": "generated",
                                "title": "Liability Generated for " + tenant,
                                "data-toggle": "tooltip"
                            });
                        $(tr).find("td.td-actions span.action").removeClass("hide");
                        $(tr).find("*").prop("disabled", true);
                        $(tr).find("input.IsSelected").prop("checked", true);
                    }

                }
                else {
                    $(tr).attr("title", "Liability Received");
                    $(tr).attr(
                           {
                               "data-received": "received",
                               "title": "Liability Received  for " + tenant,
                               "data-toggle": "tooltip"
                           });
                    $(tr).find("input.IsSelected").prop("checked", true);
                    $(tr).find("*").prop("disabled", true);
                    $(tr).find("td.td-actions span.action").addClass("hide")
                    $(tr).find("td.td-actions span.label-status").html("Received").removeClass("hide")
                }
            });

        },

    };
}();

