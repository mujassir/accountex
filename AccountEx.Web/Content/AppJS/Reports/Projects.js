
var Project = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Project";
    var UPLOAD_FOLDER = "ProjectFiles";
    var LIST_LOADED = false;
    return {
        init: function () {
           var $this = this;
            $this.ListView();
        },

        ListView: function () {
            var url = Setting.APIBaseUrl + API_CONTROLLER+"?type=report";
            if (!LIST_LOADED) {
                LIST_LOADED = true;
                DataTable.BindReportDatatable(DATATABLE_ID, url);
            } else {
                DataTable.RefreshDatatable(DATATABLE_ID, url);

            }
        },
       
     

       

    };
}();
