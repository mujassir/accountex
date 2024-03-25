
var Listing = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Customer";
    var LIST_LOADED = false;
    return {
        init: function (apiController, callback, extraKey) {
            API_CONTROLLER = apiController;
            var $this = this;
            $this.ListView(callback, extraKey);
        },
        ListView: function (callback, extraKey) {
            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER + "?type=report&" + extraKey;
                LIST_LOADED = true;
                DataTable.BindReportDatatable(DATATABLE_ID, url);
                callback();
            }
        }
    };
}();
