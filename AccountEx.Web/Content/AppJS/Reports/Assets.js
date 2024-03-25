
var Customers = function () {
    var max = 0;
    var datatableId = "mainTable";
    var apiController = "Customer";
    var listLoaded = false;
    return {
        init: function () {
           var $this = this;
            $this.ListView();
        },
        ListView: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!listLoaded) {
                var url = Setting.APIBaseUrl + apiController+"?type=report";
                listLoaded = true;
                DataTable.BindReportDatatable(datatableId, url);
            }
        }
    };
}();
