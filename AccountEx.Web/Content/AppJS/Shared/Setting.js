
var Setting = {
    DatatablePageSize: 10,
    DatatableRecordCheckBox: true,
    DatatableLoadingMessage: "Loading Records...please wait",
    DateTimeDateFormat: "dd/mm/yyyy",
    DateTimeTimeFormat: "hh:mm tt",
    PageLandingView: "DetailView",
    //PageLandingView: 'ListView',
    APIBaseUrl: "../api/",
    COAAccountCodeManual: true,
    COAAccountUseCode: true,
    AppLevel: 4,
    AccountLevel: 4,
    SalesTax: 0.17,
    UploadRootFolder: "../Upload/",
    MinimumResultsForSearch: 10,
    GraphRecords: 10
};
//option for barcode print
var options = {
    width: 2,
    height: 30,
    quite: 10,
    format: "CODE128",
    displayValue: true,
    fontOptions: "",
    font: "monospace",
    textAlign: "center",
    fontSize: 14,
    backgroundColor: "",
    lineColor: "#000"
};
var Messages = {
    RecordSaved: "Record saved successfully!",
    RecordDeleted: "Record deleted successfully!",


};

AppData =
   {

       COA: new Object(),
       CustomerDiscount: new Object(),
       AccountDetail: new Object(),

   };

