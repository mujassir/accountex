var TestingResults = function () {
    var API_CONTROLLER = "CRMReport";
    var PageSetting = new Array();
    return {
        init: function () {
            var $this = this;
            $this.LoadProducts();
            $('#chkselect_all-select2-option-db').click(function () {
                var $Container = $(this).closest(".form-group");
                var $select2 = $Container.find("select.select2");
                if ($(this).is(':checked')) {
                    $select2.select2("enable", false);
                    Common.UpdateRequired($select2, false);
                }
                else {
                    $select2.select2("enable", false)
                    Common.UpdateRequired($select2, true);
                }


            });
            $("#btnShowReport").click(function () { $this.LoadData(); });

        },

        LoadProducts: function (key) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "CRMMisc" + "?key=GetProductsIdName",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.BindSelect(res.Data, $("#ProductIds"), true)

                    } else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                    Common.ShowError(e.responseText);
                }

            });
        },
        LoadData: function () {

            var $this = this;
            var productIds = $("#ProductIds").val();
            if (!Common.isNullOrWhiteSpace(productIds))
                productIds = productIds.join(",");


            var qs = "?key=TestingResults";
            if (Common.Validate($("#form-info"))) {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                    type: "Post",
                    data: { productIds: productIds },
                    success: function (res) {
                        if (res.Success) {
                            var html = "";
                            var records = res.Data.Table;
                            $("#tbl-summary tbody").html(html);
                            html = "";
                            for (var i in records) {
                                var record = records[i];
                                html += "<tr>";
                                html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                                html += "<td>" + record.VoucherNo + "</td>";
                                html += "<td>" + record.CreatedBy + "</td>";
                                html += "<td>" + record.Region + "</td>";
                                html += "<td>" + record.Customer + "</td>";
                                html += "<td>" + record.CaseType + "</td>";
                                html += "<td>" + record.Product + "</td>";
                                html += "<td>" + record.RPPLCounterProduct + "</td>";
                                html += "<td class='align-right'>" + record.Priority + "</td>";
                                html += "<td>" + record.TestRequired + "</td>";
                                html += "<td>" + record.TestingLab + "</td>";
                                html += "<td>" + Common.GetKeyFromEnum(record.StatusId, CRMComplaintStatus) + "</td>";
                                html += "<td>" + record.ResolvedBy + "</td>";
                                html += "</tr>";
                            }
                            if (records.length == 0)
                                html += "  <tr><td colspan='13' style='text-align: center'>No record(s) found</td></tr>";
                            $("#tbl-sale-detail tbody").html(html);


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


    };
}();