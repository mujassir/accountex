
var BLStatuses = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "BLStatus";
    var DATATABLE_ID = "mainTable";
    var DC_DATATABLE_ID = "DCTable";
    var PageSetting = new Object();
    var PageData = new Object();
    var focusElement = "#InvoiceNumber";
    return {
        init: function () {
            var $this = this;
            $(document).on("change", "select.StatusId", function (event) {
                var tr = $(this).closest("tr");
                var id = $(tr).attr("data-id");
                var isFinal = $(tr).find("select.StatusId option:selected").attr("data-isfinal");
                if (isFinal == "True") {
                    $(tr).find("select,input").not($(this)).prop("disabled", false);
                    $("tr[data-parent='" + id + "']").find("select,input").prop("disabled", false);
                }
                else {
                    $(tr).find("select,input").not($(this)).prop("disabled", true);
                    $("tr[data-parent='" + id + "']").find("select,input").prop("disabled", true);
                }

            });
            $this.LoadBLStatuses();
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                focusElement = "#Date";
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.LoadBLStatuses();

            });
        },
        SaveRecord: function (callback) {

            var $this = this;
            var mode = "add";
            var id = Common.GetInt($("#Id").val());
            var scope = $("#form-info");
            var Bls = new Array();
            $("#item-container tbody tr[main-parent-row='true']").each(function () {
                var tr = $(this);
                var id = $(tr).attr("data-id");
                var record = Common.SaveItemDataBySelector(tr, true);
                var Items = new Array();
                var selector = $("tr[data-parent='" + id + "']").not(tr).not("[data-exclude-row='true']");
                Items = Common.SaveItemDataBySelector(selector);
                record["Vehicles"] = Items;
                Bls.push(record);
            });
            var err = "";
            if (Bls.length <= 0) {
                err += "Please add atleast one BL.,";
            }
            if (err.trim() != "") {
                Common.ShowError(err);
                return;
            }
            var record = { "": Bls };
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Updating Statuses...please wait",
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
        LoadBLStatuses: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading Statuses ...please wait",
                success: function (res) {
                    if (res.Success) {

                        var BLs = Enumerable.From(res.Data).GroupBy("$.BLNo", null,
                    function (key, g) {
                        var result = {
                            BLNo: key,
                            BL: g.FirstOrDefault(),
                            Vehicles: g.ToArray(),
                        }
                        return result;
                    }).ToArray();

                        var html = "";
                        $("#item-container tbody").html('');

                        for (var i in BLs) {
                            var BL = BLs[i];
                            var vehicles = BL.Vehicles;
                            Common.MapItemData([BL.BL], null, "#template-item-header-row", true);
                            html = "";
                            html += "<tr class='align-center' data-parent='" + BL.BLNo + "' data-header-row='true' data-exclude-row='true'>";
                            html += "<td colspan='2'>Car</td>";
                            html += "<td>Branch</td>";
                            html += "<td><span class='pull-lef'>Purchase Price</span></td>";
                            html += "<td>Sale Price</td>";
                            html += "</tr>";
                            $("#item-container tbody").append(html);
                            Common.MapItemData(vehicles, null, null, true);


                        }
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


        CheckFinalStatus: function () {
            $("#item-container tbody tr").each(function () {
                var tr = $(this);
                var id = $(tr).attr("data-id");
                var isFinal = $(tr).find("select.StatusId option:selected").attr("data-isfinal");
                if (isFinal == "True") {
                    $(tr).find("select,input").not($(this)).prop("disabled", false);
                    $("tr[data-parent='" + id + "']").find("select,input").prop("disabled", false);
                }
                else {
                    $(tr).find("select,input").not($(this)).prop("disabled", true);
                    $("tr[data-parent='" + id + "']").find("select,input").prop("disabled", true);
                }
            })
        },
        LoadPageSetting: function () {
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }

            //this.LoadAccounts();
        },
    }
}();