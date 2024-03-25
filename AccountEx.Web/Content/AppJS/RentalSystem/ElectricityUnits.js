
var ElectricityUnits = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "ElectricityUnit";
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var focusElement = "#VoucherNo";
    var PageSetting = new Object();
    var month = 0;
    var year = 0;
    return {
        init: function () {
            var $this = this;

            $this.LoadVoucher("nextvouchernumber");
            //$this.SelectCurrentMonthYear();

            $("#Remarks").keyup(function (e) {
                if (e.which == 13) {
                    $(".ShopNo").focus();
                }
            });
            $('#txtSearch').keyup(function () {
                filterTable(document.getElementById("txtSearch"), document.getElementById("item-container"));
            });
            $(document).on("change", "#Year, #Month", function (event) {
                month = $("#Month").val();
                year = $("#Year").val();
                voucherno = $("#VoucherNo").val();
                $this.LoadVoucher("byMonthYear");
            });

            $(document).on("keyup change", ".PreviousReading, .CurrentReading", function (event) {

                var tr = $(this).closest("tr");
                var shopno = $(tr).find("input.ShopNo").val();
                var previousReading = $(tr).find("input.PreviousReading").val();
                var currentReading = $(tr).find("input.CurrentReading").val();
                $(tr).find("input.Unit").val(currentReading - previousReading);
                var unit = $(tr).find("input.Unit").val();
                if ($(tr).is(':last-child') && event.keyCode == 13 && unit != 0) {
                    $this.AddItem();
                }
                else if (event.which == 13 && unit <= 0) {
                    var err = "Unit must be greater than zero(0).";
                    Common.ShowError(err);
                    $(tr).find("input.Unit").focus();
                }
                var electriciyUnitCharges = $(tr).find("input.ElectriciyUnitCharges").val();
                var amount = Common.GetCeilInt(Common.GetCeilInt(unit) * Common.GetFloat(electriciyUnitCharges));
                $(tr).find("input.Amount").val(amount);

            });
            $(document).on("keyup change", ".ElectriciyUnitCharges", function (event) {
                var tr = $(this).closest("tr");
                var unit = $(tr).find("input.Unit").val();
                var electriciyUnitCharges = $(tr).find("input.ElectriciyUnitCharges").val();
                var amount = Common.GetCeilInt(unit) * Common.GetFloat(electriciyUnitCharges);
                $(tr).find("input.Amount").val(Common.GetCeilInt(amount));
            });
            $this.LoadPageSetting();
            //$("#VoucherNo").keyup(function (e) {
            //    if (e.which == 13) {
            //        $(this).val() == "0" ? focusElement = "#VoucherNo" : "#Date";
            //        $this.LoadVoucher("same");
            //    }
            //});

        },
        SelectCurrentMonthYear: function () {
            var $this = this;
            var date = new Date();
            m = date.getMonth() + 1;
            y = date.getFullYear();
            $("#Month").select2("val", m);
            $("#Year").select2("val", y);


        },
        SelectNextMonthYear: function () {
            var $this = this;
            var date = new Date();
            var m = month + 1;

            $("#Month").select2("val", month);
            $("#Year").select2("val", year);


        },
        LoadPageSetting: function () {
            var tokens = $.parseJSON($("#FormSetting").val());
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
        },
        AddItem: function () {
            var $this = this;

            var code = $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ShopNo").val();
            if (typeof code != "undefined" && code.trim() == "") {
                setTimeout(function () {
                    $("#item-container tbody tr:nth-last-child(1) td:nth-child(1) input.ShopNo").focus().select();
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
                    $("#item-container tbody tr:nth-last-child(1) input.ShopNo").focus().select();
                }, 300);
                focusElement = "";
            }
        },

        Close: function () {
        },
        CustomClear: function () {
            var $this = this;
            $("#item-container tbody").html("");
            $("div[data-save='save']").find("input[type=text]:not([data-clear-type='date'],[data-clear='false']),textarea,input[type=hidden]:not([data-clear='false']),input[type=password]").val("");
        },

        SaveRecord: function () {
            var $this = this;
            $(".container-message").hide();
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            if (Common.Validate($("#form-info"))) {

                var Items = new Array();
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.ShopNo.trim()!=''").ToArray();
                record["ElectricityUnitItems"] = Items,

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/Post",
                    data: record,
                    type: "POST",
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving voucher...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            var data = res.Data;
                            var voucherNo = data.VoucherNo;

                            $("div[data-save='save']").find("select.select2").each(function () {

                                $(this).select2("val", $("option:first-child", this).attr("value"));
                            });
                            $this.CustomClear();

                            $("#VoucherNo").val(voucherNo);
                            $this.LoadVoucher("nextvouchernumber");
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                        }
                        else {

                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (data, status, jqXHR) {
                        alert(jqXHR)
                    }
                });
            }
        },

        LoadVoucher: function (key) {
            var $this = this;

            var $this = this;
            var qs = "?key=" + key;
            qs += "&month=" + month;
            qs += "&year=" + year;
            qs += "&blockId=" + Common.GetInt($("#BlockId").val());

            var voucherno = Common.GetInt($("#VoucherNo").val());

            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + qs + "&voucherno=" + voucherno,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  Voucher ...please wait",
                success: function (res) {
                    if (res.Success) {
                        $this.CustomClear();
                        var data = res.Data;

                        //Map Print Master Data start
                        if (data.ElectricityUnit != null) {
                            var month = data.ElectricityUnit.Month;
                            var year = data.ElectricityUnit.Year;
                            var voucherNo = data.ElectricityUnit.VoucherNo;
                            var remarks = data.ElectricityUnit.Remarks;

                            var formattedMonth = moment(data.ElectricityUnit.Month, 'MM').format('MMMM'); // September
                            var billMonth = "" + formattedMonth + "-" + year;
                        }


                        if (remarks == typeof (undefined) || remarks == null)
                            remarks = "";
                        if (voucherNo == typeof (undefined) || voucherNo == null)
                            voucherNo = "";
                        if (billMonth == typeof (undefined) || billMonth == null)
                            billMonth = "";
                        $('#lblMonth').html(billMonth);
                        $('#lblVoucherNo').html(voucherNo);
                        $('#Remarks').html(remarks);
                        //Map Print Master Data start END

                        var electricityUnit = data.ElectricityUnit;
                        var previousReading = data.PreviousReading;

                        Common.MapEditData(electricityUnit, "#form-info");
                        var agreements = data.TenantsAgreemnet;
                        for (var i in agreements) {
                            var agreement = agreements[i];
                            if (electricityUnit != null) {
                                var EUnit = Enumerable.From(electricityUnit.ElectricityUnitItems).Where(function (p) { return p.RentAgreementId == agreement.Id }).FirstOrDefault();
                                if (typeof EUnit == 'undefined')
                                    agreement.Id = 0;
                                $.extend(agreement, EUnit);
                            }
                            if (electricityUnit == null || Enumerable.From(electricityUnit.ElectricityUnitItems).Where(null, function (p) { return p.RentAgreementId == agreement.Id }) == null) {
                                agreement.PreviousReading = 0;
                                if (previousReading != null) {
                                    var previosEUnit = Enumerable.From(previousReading.ElectricityUnitItems).Where(function (p) { return p.RentAgreementId == agreement.Id }).FirstOrDefault();
                                    $.extend(agreement, previosEUnit);
                                    if (previosEUnit != null) {
                                        agreement.PreviousReading = previosEUnit.CurrentReading;
                                    }


                                }
                                agreement.CurrentReading = 0;
                                agreement.Unit = 0;
                                agreement.ElectricityUnitId = 0;
                                agreement.Id = 0;
                            }

                        }
                        if (electricityUnit == null) {
                            $("#VoucherNo").val(data.VoucherNo);
                        }

                        //  Common.MapItemData(agreements);

                        var blocks = Enumerable.From(agreements).GroupBy("$.BlockName", null,
                     function (key, g) {
                         var result = {
                             blockName: key,
                             blockData: g.OrderBy(function (p) { return Common.GetInt(p.ShopNo) }).ToArray(),
                         }
                         return result;
                     }).ToArray();

                        var html = "";
                        $(".item-container tbody").html('');

                        for (var i in blocks) {
                            var block = blocks[i];
                            var items = block.blockData;
                            html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'><td colspan='7' style='text-align:center;'>" + block.blockName + "</td></tr>";
                            html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'>";
                            html += "<td style='width:7%;'>Shop No</td>";
                            html += "<td style='width: 30%;'>Tenant Name</td>";
                            html += "<td>Previous Reading</td>";
                            html += "<td>Current Reading</td>";
                            html += "<td>Units</td>";
                            html += "<td>Electriciy Unit Charges</td>";
                            html += "<td>Amount</td>";
                            html += "</tr>";

                            for (var i in items) {
                                var item = items[i];

                                if (typeof item.PreviousReading == 'undefined')
                                    item.PreviousReading = 0;
                                if (typeof item.CurrentReading == 'undefined')
                                    item.CurrentReading = 0;
                                if (typeof item.Unit == 'undefined')
                                    item.Unit = 0;
                                if (typeof item.Amount == 'undefined' || item.Amount == 'NULL' || item.Amount == null)
                                    item.Amount = 0;
                                if (typeof item.ElectriciyUnitCharges == 'undefined' || item.ElectriciyUnitCharges == 'NULL' || item.ElectriciyUnitCharges == null)
                                    item.ElectriciyUnitCharges = PageSetting.ElectriciyUnitCharges;
                                html += "<tr data-parent='" + block.blockName + "'>";
                                html += "<td>";
                                html += "<input type='hidden' class='Id' data-db-column='Id' value='" + item.Id + "'>";
                                html += "<input type='hidden' class='Id' data-db-column='Id' id='ItemId' value='" + item.Id + "'>";
                                html += "<input type='hidden' class='ShopId' id='ShopId' data-db-column='ShopId'  value='" + item.ShopId + "'>";
                                html += "<input type='text' disabled='disabled' class='ShopNo form-control typeahead input-small hide' id='ShopNo' data-db-column='ShopNo'  value='" + item.ShopNo + "'>";
                                html += "" + item.ShopNo + "</td>";

                                html += "<td>";
                                html += "<input type='hidden' class='RentAgreementId' id='RentAgreementId' data-db-column='RentAgreementId' value='" + item.RentAgreementId + "'>";
                                html += "<input type='hidden' class='TenantAccountId' id='TenantAccountId' data-db-column='TenantAccountId' value='" + item.TenantAccountId + "'>"
                                html += "<input type='text' disabled='disabled' class='TenantName form-control input-medium hide' id='TenantName' data-db-column='TenantAccountName' value='" + item.TenantAccountName + "'>";
                                html += "" + item.TenantAccountName + "</td>";

                                html += "<td>";
                                html += "<input type='text' class='PreviousReading form-control input-small' id='PreviousReading' data-db-column='PreviousReading' data-db-type='int' value='" + item.PreviousReading + "'>";
                                html += "</td>";

                                html += "<td>";
                                html += "<input type='text' class='CurrentReading form-control input-small' id='CurrentReading' data-db-column='CurrentReading' data-db-type='int' value='" + item.CurrentReading + "'>";
                                html += "</td>";

                                html += "<td>";
                                html += "<input type='text' class='Unit form-control input-small'id='Unit' data-db-column='Unit' data-db-type='int' disabled='disabled' value='" + item.Unit + "'>";
                                html += "</td>";

                                html += "<td>";
                                html += "<input type='text' class='ElectriciyUnitCharges form-control input-small' data-db-column='ElectriciyUnitCharges' value='" + item.ElectriciyUnitCharges + "'>";
                                html += "</td>";

                                html += "<td>";
                                html += "<input type='text' class='Amount form-control input-small' data-db-column='Amount' value='" + item.Amount + "'>";
                                html += "</td>";

                                html += "</tr>";

                            }

                            $(".item-container tbody").html(html);

                        }
                        if (res.Data.Next)
                            $(".form-actions .next,.form-actions .last").removeClass("disabled");
                        else
                            $(".form-actions .next,.form-actions .last").addClass("disabled");
                        if (res.Data.Previous)
                            $(".form-actions .first,.form-actions .previous").removeClass("disabled");
                        else
                            $(".form-actions .first,.form-actions .previous").addClass("disabled");
                        //   Common.MapItemData(blocks);



                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },

        LoadReportData: function (blocks, items) {

            var html = "";


            $(".item-container tbody").html('');
            $("#electrity-unitPrint tbody").html('');
            for (var i in blocks) {
                var block = blocks[i];
                var items = block.blockData;

                html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'><td colspan='7' style='text-align:center;'>" + block.blockName + "</td></tr>";
                html += "<tr class='bold' data-parent='" + block.blockName + "' data-header-row='true' data-exclude-row='true'>";
                html += "<td style='width:7%;'>Shop No</td>";
                html += "<td style='width: 30%;'>Tenant Name</td>";
                html += "<td>Previous Reading</td>";
                html += "<td>Current Reading</td>";
                html += "<td>Units</td>";
                html += "<td>Electriciy Unit Charges</td>";
                html += "<td>Amount</td>";
                html += "</tr>";

                for (var i in items) {
                    var item = items[i];

                    if (typeof item.PreviousReading == 'undefined')
                        item.PreviousReading = 0;
                    if (typeof item.CurrentReading == 'undefined')
                        item.CurrentReading = 0;
                    if (typeof item.Unit == 'undefined')
                        item.Unit = 0;
                    if (typeof item.Amount == 'undefined' || item.Amount == 'NULL' || item.Amount == null)
                        item.Amount = 0;
                    if (typeof item.ElectriciyUnitCharges == 'undefined' || item.ElectriciyUnitCharges == 'NULL' || item.ElectriciyUnitCharges == null)
                        item.ElectriciyUnitCharges = PageSetting.ElectriciyUnitCharges;
                    html += "<tr data-parent='" + block.blockName + "'>";

                    html += "<td>" + item.ShopNo + "</td>";

                    html += "<td>";
                    html += "" + item.TenantAccountName + "</td>";



                    html += "<td>";
                    html += item.PreviousReading;
                    html += "</td>";

                    html += "<td>";
                    html += item.CurrentReading;
                    html += "</td>";

                    html += "<td>";
                    html += item.Unit;
                    html += "</td>";

                    html += "<td>";
                    html += item.ElectriciyUnitCharges;
                    html += "</td>";

                    html += "<td>";
                    html += item.Amount;
                    html += "</td>";

                    html += "</tr>";
                }
                $("#electrity-unitPrint tbody").html(html);
            }
        },

        //Delete: function () {
        //    var $this = this;
        //    Common.ConfirmDelete(function () {
        //        var voucherno = Common.GetInt($("#VoucherNo").val());
        //        var url = Setting.APIBaseUrl + API_CONTROLLER + "/" + voucherno + "?voucher=" + voucherno;
        //        var id = Common.GetInt($("#Id").val());
        //        if (id <= 0) {
        //            Common.ShowError("No Voucher found for deletion.");
        //            return;
        //        }

        //        Common.WrapAjax({
        //            url: url,
        //            type: "DELETE",
        //            contentType: "application/json; charset=utf-8",
        //            dataType: "json",
        //            blockUI: true,
        //            blockElement: "#form-info",
        //            blockMessage: "Deleting voucher ...please wait",
        //            success: function (res) {
        //                if (res.Success) {
        //                    focusElement = "#VoucherNo";
        //                    $this.LoadVoucher();
        //                } else {
        //                    Common.ShowError(res.Error);
        //                }

        //            },
        //            error: function (e) {
        //            }
        //        });
        //    });
        //},

        //LoadData: function () {

        //    var $this = this;
        //    var qs = "?key=getrecordbyyear";
        //    qs += "&month=" + month;
        //    qs += "&year=" + year;
        //    //qs += "&voucherno=" + voucherno;

        //    if (Common.Validate($("#form-info"))) {
        //        Common.WrapAjax({
        //            url: Setting.APIBaseUrl + API_CONTROLLER + "/" + qs,
        //            type: "GET",
        //            contentType: "application/json; charset=utf-8",
        //            dataType: "json",
        //            blockUI: true,
        //            blockElement: "#form-info",
        //            blockMessage: "Loading Information ...please wait",
        //            success: function (res) {
        //                if (res.Success) {
        //                    $("#item-container tbody").html("");
        //                    var electricityUnits = res.Data.TenantsData;
        //                    if (electricityUnits.length > 0) {
        //                        for (var i = 0; i < electricityUnits.length; i++) {
        //                            electricityUnit = electricityUnits[i];
        //                            Common.MapEditData(electricityUnit, "#form-info");
        //                            var electricityUnitItems = electricityUnit.ElectricityUnitItems;
        //                            Common.MapItemData(electricityUnitItems, "#item-container", "#template-item", true);
        //                        }
        //                    }
        //                    else {
        //                        $this.LoadVoucher();
        //                    }
        //                    //if (d.length > 0) {
        //                    //    //Common.MapEditData(d, "#form-info");
        //                    //    var ElectricityUnitItems = d.ElectricityUnitItems;
        //                    //    $("#item-container tbody").html("");
        //                    //    Common.MapItemData(ElectricityUnitItems, "#item-container", "#template-item", true);
        //                    //    $.uniform.update();
        //                    //}

        //                }
        //                else {
        //                    Common.ShowError(res.Error);
        //                }
        //                App.initUniform();
        //            },
        //            error: function (e) {
        //                Common.ShowError(e.responseText);
        //            }
        //        });
        //    }
        //},
    }
}();




