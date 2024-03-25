
var TrialBalance = function () {
    var apiController = "TrialBalance";
    return {
        init: function () {
           var $this = this;
            $("#btnShowReport").click(function () {
                var date = $("#txtdate").val();
                $this.LoadTrailBalance(date);
            });
        },
        LoadTrailBalance: function (date) {
            var isfilterzero;
            $("#lblReportDate").html("Date: " + date);
           
            if ($("#FilterZeroBalance").is(":checked")) {
                isfilterzero = true;
            }
            else {
                isfilterzero = false;
            }
            var splitdate = date.split("/");
            var prevyear = splitdate[2] - 1;
            var date1 = "30/06/"+prevyear;
           // var date2 = $("#txtdate").val();
            //var year = new Date(date).getFullYear();
Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "?date2=" + date + "&IsFilter=" + isfilterzero,
                type: "Get",
                data: "",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var data = res.Data.Records;
                        var j = 1;
                        for (var i in data) {
                            html += "<tr><td>" + data[i].Code + "</td><td><a href='../reports/general-ledger?accountId=" + data[i].AccountId + "&fromDate=" + date1 + "&toDate=" + date + "'>" + data[i].AccountTitle + "</a></td><td>" + data[i].Debit + "</td><td>" + data[i].Credit + "</td></tr>";
                        }
                        $("#tbldetail tbody").html(html);
                        html = "<tr><td colspan='2'>Total</td><td>" + res.Data.TotalDebit + "</td><td>" + res.Data.TotalCredit + "</td></tr>";
                        html += "<tr><td colspan='2'></td><td style='text-align: center;' colspan='2' >Difference = " + res.Data.Difference + "</td></tr>";
                        $("#tbldetail tfoot").html(html);
                        $("#tbldetail").dataTable({
                            'sDom': "t",
                            // set the initial value
                            "bAutoWidth": false,   // disable fixed width and enable fluid table
                            //"bServerSide": true, // server side processing
                            // "bProcessing": true, // enable/disable display message box on record load
                            //"sAjaxSource": ajaxSource, // ajax source
                            // "bProcessing": true, // enable/disable display message box on record load

                            "iDisplayLength": 1000,
                            "aoColumnDefs": [
                                { 'bSortable': false, "aTargets": ["no-sort"] },
                                { "bSearchable": false, "aTargets": ["sort"] }
                            ],
                        });
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