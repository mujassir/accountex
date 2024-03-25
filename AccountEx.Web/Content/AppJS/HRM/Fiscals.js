var Fiscals = function () {
    var max = 0;
    var LIST_LOADED = false;
    var LIST_CHANGED = false;
    var API_CONTROLLER = "Fiscal";
    var DATATABLE_ID = "mainTable";
    return {
        init: function () {
            var $this = this;
            $('#Month').on('select2-selecting', function (e) {
                console.log('here');
            });

            $("#FromMonth,#FromYear").change(function () { $this.SetFiscalName(); });
            $this.ListView();
        },
        SetFiscalName: function () {
            var $this = this;
            var frommonth = Common.GetInt($("#FromMonth").val());
            var fromyear = Common.GetInt($("#FromYear").val());

            var firstDate = $this.GetFirstDate(frommonth, fromyear);
            var lastDate = $this.AddOneYear(frommonth, fromyear);

            $("#FromDate").val(firstDate);
            $("#ToDate").val(lastDate);
            $("#Name").val($("#FromMonth option:selected").text() + " " + $("#FromYear").val() + " to " + lastDate);
            var fromYear = $("#FromYear").val();
            var toYear = moment(new Date(fromyear + 1, frommonth - 1, 0)).format('YYYY');
            var shortName = '';
            if (fromyear == toYear)
                shortName = fromyear;
            else
                shortName = fromyear + "-" + toYear;
            $("#Name").val($("#FromMonth option:selected").text() + " " + $("#FromYear").val() + " to " + lastDate);
            $("#ShortName").val(shortName);
        },
        AddOneYear: function (month, year) {
            var leavemonthend = new Date(year + 1, month - 1, 0);
            leavemonthend = moment(leavemonthend).format('MMMM YYYY');
            return leavemonthend;
        },
        GetFirstDate: function (month, year) {
            var firstDate = "01/" + month + "/" + year;
            return firstDate;
        },
        GetType: function () {
            return Common.GetQueryStringValue("type").toLowerCase();
        },
        DetailView: function () {
            //$("#form-info").removeClass("hide");
            //$("#div-table").addClass("hide");
        },
        ListView: function () {
            if (LIST_LOADED) {
                if (LIST_CHANGED) DataTable.RefreshDatatable(DATATABLE_ID);
            }
            else {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
        },
        GetPlaceHolder: function (data) {
            var record = new Object();
            for (var key in data) {
                record["{{" + key + "}}"] = data[key];
            }
            return record;
        },
        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                $this.CustomClear();
                Common.ShowMessage(true, { message: Messages.RecordSaved });
                $this.ListView();
                //$this.RebindData();
            });
        },
        SaveRecord: function (callback) {
            var $this = this;

            if (Common.Validate($("#mainform"))) {

                var frommonth = Common.GetInt($("#FromMonth").val());
                var fromyear = Common.GetInt($("#FromYear").val());

                var fromDate = moment(new Date(fromyear, frommonth - 1, 1)).format('YYYY-MM-DD');
                var toDate = moment(new Date(fromyear + 1, frommonth - 1, 0)).format('YYYY-MM-DD');
                var isdefault = $("#IsDefault").is(":checked");
                var record = {
                    Id: $("#Id").val(),
                    Name: $("#Name").val(),
                    ShortName: $("#ShortName").val(),
                    FromDate: fromDate,
                    ToDate: toDate,
                    IsDefault: isdefault,
                };

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
        Clone: function () {
            var $this = this;
            $("#Id").val("0");
            $("#FromMonth,#FromYear,#Name").removeAttr("disabled");
            $("#btnClone").attr("disabled", "disabled");
        },
        CustomClear: function () {
            $.uniform.update();
            $("#FromMonth,#FromYear,#Name,#disabled").removeAttr("disabled");
            $("#btnClone").attr("disabled", "disabled");
            Common.Clear();
        },
        Delete: function (id) {
            $this = this;
            Common.ConfirmDelete(function () {
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                    type: "DELETE",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    blockUI: true,
                    //blockElement: "#form-info",
                    blockMessage: "Delete Appointment...please wait",
                    success: function (res) {

                        if (res.Success) {
                            DataTable.RefreshDatatable(DATATABLE_ID);
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
        Edit: function (id) {
            $this = this;
            var html = "";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var data = res.Data;
                        var html = "";
                        var name = "";
                        var todate = "";
                        var fromdate = "";
                        $("#Id").val(data.Id);
                        $("#Name").val(data.Name);
                        $("#ShortName").val(data.ShortName);
                        $("#ToDate").val(moment(data.ToDate).format('DD/MM/YYYY'));
                        //$("#Name").prop("disabled", true);
                        $("#FromMonth").prop("disabled", true);
                        $("#FromYear").prop("disabled", true);
                        var check = moment(data.FromDate, 'YYYY/MM/DD');
                        var month = check.format('M');
                        var day = check.format('D');
                        var year = check.format('YYYY');
                        $("#FromMonth").val(month);
                        $("#FromYear").val(year);
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
        Print: function () {
            window.print();
        },
    };
}();
