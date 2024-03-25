
var MiscChargesChallans = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "MiscChargesChallan";
    var LIST_LOADED = false;
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;

            $("#RentAgreementId").change(function () {
                $this.LoadPreviousChallan();
            });
            $this.CustomClear();
            $this.LoadData();
        },


        New: function () {

            var $this = this;
            $this.CustomClear();
            //$this.LoadVoucher("nextvouchernumber");
        },
        Print: function (id) {
            var $this = this
            //var id = Challan.Id;
            //id = 13876;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?Id=" + id + "&printChallan=true",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "printing bill ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var d = res.Data.Order;
                        var challan = res.Data.Challan;
                        Common.MapDataWithPrefixFClasses(challan, ".div-report", "lbl", "html");
                        var dueDate = moment(challan.DueDate).format("MMMM DD, YYYY");
                        var monthString = moment(challan.DueDate).format("MMMM, YYYY");
                        $(".barcode").JsBarcode(challan.Id.toString(), options);
                        $(".lblDueDate").html(dueDate);
                        $(".lblMonth").html(monthString);
                        setTimeout(function () { window.print() }, 1500);
                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }
            });
        },

        CustomClear: function () {
            $this = this;
            $("#div-table").addClass("hide");
            $("#mainTable tbody").html('');
            Common.Clear();
        },



        Save: function () {
            var $this = this;
            var data = new Array();
            if (Common.Validate($("#form-info"))) {
                var challanItems = new Array();
                var record = Common.SetValue("#form-info");
                var agreement = Enumerable.From(AGREEMENTS).FirstOrDefault(null, function (p) { return p.Id == record.RentAgreementId });
                record = $.extend(true, record, agreement);
                delete record.Id;
                var item = $.extend(true, item, record);
                item.ChallanId = record.Id;
                item.TenantAccountName = item.TenantName;
                item.Amount = record.NetAmount;
                challanItems.push(item);
                record["ChallanItems"] = challanItems;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving Challan ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: "Challan Saved Successfully!" });
                            $this.CustomClear();
                            $this.Print(res.Data.Id);

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
        Delete: function (element, id) {
            var $this = this;
            var tr = $(element).closest("tr");
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    blockElement: "#div-table",
                    blockMessage: "Deleting ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            $(tr).remove();
                            $this.GetRentAgreementInfo();
                            Common.ShowMessage(true, { message: "Challan deleted successfully!" });
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

        LoadData: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "RentAgreement?key=GetRentAgreementsWithTenants&includeTransfeered=true",
                type: "Get",
                blockUI: true,
                blockElement: "#form-info",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        AGREEMENTS = data;
                        var html = "<option></option>";
                        for (var i in data) {
                            var record = data[i];
                            html += "<option data-shopid='" + record.ShopId + "' data-tenantid='" + record.TenantAccountId + "' value='" + record.Id + "'>" + record.TenantCode + "-" + record.TenantName + " (" + (record.Block != null ? "Block: " + record.Block + " ," : '') + " Shop No: " + record.ShopNo + ")" + "</option>";
                        }
                        $("#RentAgreementId").html(html);
                        $("#RentAgreementId").select2();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }

            });

        },
        LoadPreviousChallan: function () {
            var id = Common.GetInt($("#RentAgreementId").val())
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?rentAgreementId=" + id + "&key=GetRentAgreementInfo",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading info...please wait",
                success: function (res) {
                    if (res.Success) {
                        var challans = res.Data.AllChallan;
                        var html = "";
                        for (var i in challans) {
                            var challan = challans[i];
                            html += "<tr>"
                            html += "<td>" + challan.Id + "</td>";
                            html += "<td>" + moment(challan.DueDate).format("MMMM DD, YYYY") + "</td>";
                            html += "<td>" + Common.GetCeilInt(challan.TotalAmount).format() + "</td>";
                            html += "<td>" + (challan.IsReceived ? "Paid" : "Pending") + "</td>";
                            html += "<td><button type='button' class='btn yellow btn-xs' onclick='MiscChargesChallans.Print(" + challan.Id + ")'><i class='fa fa-print'></i>&nbsp;Print</button>";
                            if (!challan.IsReceived)
                                html += "<button type='button' class='btn red btn-xs' onclick='MiscChargesChallans.Delete(this," + challan.Id + ")'><i class='fa fa-trash-o'></i>&nbsp;Delete</button>";
                            html += "</td>";

                            html += "</tr>"
                        }
                        $("#mainTable tbody").html(html);
                        $("#div-table").removeClass("hide");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });

        }
    };
}();