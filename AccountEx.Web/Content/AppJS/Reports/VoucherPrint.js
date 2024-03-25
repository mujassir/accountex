
var VoucherPrint = function () {
    var apiController = "VoucherPrint";
    return {
        init: function () {
           var $this = this;
            $("#btnShowReport").click(function () {
                var vou = $("#VoucherNumber").val();
                $this.LoadReport(vou);
            });
            $("#btnPrintReport").click(function () {
                var vou = $("#VoucherNumber").val();
                $this.PrintReport(vou);
            });
        },
        LoadReport: function (date) {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "/" + date,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  dabit voucher...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                       
                        $("#lblDate").html(j.Date);
                        $("#lblVoucherNumber").html(j.VoucherNumber);
                        $("#lblAccountTitle").html(j.AccountTitle);
                        $("#lblComments").html(j.Comments);
                        $(".label-amount").html(j.Amount);
                        $("#div-report").removeClass("hide");
                    }
                    else {
                        alert("Voucher number not found! please enter correct voucher number");

                         Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        PrintReport: function (date) {
            Common.WrapAjax({
                url: Setting.APIBaseUrl + apiController + "/" + date,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  dabit voucher...please wait",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                     
                        $("#lblDate").html(j.Date);
                        $("#lblVoucherNumber").html(j.VoucherNumber);
                        $("#lblAccountTitle").html(j.AccountTitle);
                        $("#lblComments").html(j.Comments);
                        $(".label-amount").html(j.Amount);
                        $("#div-report").removeClass("hide");
                        setTimeout(function(){
                            window.print();
                        },1000);
                    
                    }
                    else {
                        alert("Voucher number not found! please enter correct voucher number");
                         Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        }
};
}();