
var Customers = function () {
    var max = 0;
    var datatableId = "mainTable";
    var API_CONTROLLER = "Customer";
    var LIST_LOADED = false;
    return {
        init: function () {
           var $this = this;
            $this.ListView();
        },
        ListView: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER+"?type=report";
                LIST_LOADED = true;
                DataTable.BindReportDatatable(datatableId, url);
            }
        }
    };
}();
