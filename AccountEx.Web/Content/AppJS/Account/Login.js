
var Login = function () {
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "Manage";
    var dashBoardUrl = "";
    var LIST_LOADED = false;
    var attributeTypes;
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            //$this.LoadPageSetting();
            $(".alert-danger").addClass("hide");
            var username = $("#UserName").val();
            var password = $("#Password").val();
            $("#IsRemember").keypress(function (event) {
                if (event.which == 13) {
                    $this.CheckLogin();
                }
            });
            $("#UserName,#Password").keyup(function (event) {
                if (event.which == 13) {
                    $this.CheckLogin();
                }
            });
            $(".btn-login").click(function (event) {

                $this.CheckLogin();

            });

            $("body").tooltip({
                selector: "[data-toggle='tooltip']"
            });
        },
        CheckLogin: function () {
            $(".container-message").hide();
            var $this = this;
            var username = $("#UserName").val();
            var password = $("#Password").val();
            var IsRemember = $("#IsRemember").is(":checked");
            var record =
                {
                    Password: password,
                    UserName: username,
                    IsRemember: IsRemember,

                };
            if (password.trim() != "" || username.trim() != "") {
                Common.WrapAjax({
                    url: "../Account/CheckLogin",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: ".content",
                    blockMessage: "Logging in...please wait",
                    success: function (response) {
                        var res = JSON.parse(response);
                        if (res.Success) {
                            Common.LocalStoragePrefix = "_Accountex_StorageKey_" + res.Data.StorageKey;
                            if (res.Data.Fiscals == null) {

                                Common.CheckIfLoadCOA(function () {
                                    Login.RedirectToUrl(res);
                                }, true);

                            }
                            else {
                                var fiscals = res.Data.Fiscals;
                                var html = "";
                                for (var i = 0; i < fiscals.length; i++) {
                                    var fiscal = fiscals[i];
                                    html += "<option value='" + fiscal.Id + "'>" + fiscal.Name + "</option>";
                                }

                                $("#FiscalId").html(html);

                                $("#model-continue").modal("show");
                            }



                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            } else {
                Common.ShowError("Username & password is incorrect");
            }
        },

        Continue: function () {

            var _this = this;
            var fiscalId = $("#FiscalId").val();
            var username = $("#UserName").val();

            if (fiscalId.trim() != "") {


                var record = "FiscalId=" + fiscalId
                Common.WrapAjax({
                    url: "../Account/continue",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#model-continue",
                    blockMessage: "Processing...please wait",
                    success: function (res) {

                        var res = $.parseJSON(res);
                        if (res.Success) {
                            Common.CheckIfLoadCOA(function () {
                                Login.RedirectToUrl(res);
                            }, true);

                        }
                        else {
                            Common.ShowError(res.Error);


                        }




                    },
                    error: function (e) {
                    }
                });




            } else {
                Common.ShowError("Please select branch to continue.");
                $(".alert-danger").removeClass('hide');
            }
        },
        RedirectToUrl: function (res) {

            var url = res.Data.DashBoardUrl;
            var returnurl = Common.GetQueryStringValue("ReturnURL");
            if (typeof returnurl != "undefined" && returnurl.trim() != "")
                window.location = returnurl;
            else {
                if (typeof url == "undefined" || url == null || url.trim() == "")
                    url = "../home/xdashboard?name=AdminDashboard";
                window.location = url;
            }

        },

    };
}();