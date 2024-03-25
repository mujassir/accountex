
var ChangePassword = function () {
    var API_CONTROLLER = "User";
    var hash = "";
    return {
        init: function () {
           var $this = this;
        },
        CustomClear: function () {
        },
        Save: function () {
           var $this = this;
            if (Common.Validate($("#form-info"))) {
                var oldPassword = $("#OldPassword").val();
                var newPassword = $("#NewPassword").val();
                var confirmPassword = $("#ConfirmPassword").val();
                if (newPassword != confirmPassword) {
                    Common.ShowMessage(false, { message: "New password & Confirm password must be same" });
                }
                else {
                    Common.WrapAjax({
                        url: Setting.APIBaseUrl + API_CONTROLLER + "?oldPassword=" + oldPassword + "&newPassword=" + newPassword,
                        type: "GET",
                        success: function (res) {
                            if (res.Success) {
                                Common.ShowMessage(true, { message: "Password changed successfully!" });
                            }
                            else {
                                Common.ShowError(res.Error);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
            }
        }

    };
}();
