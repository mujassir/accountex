
var UserBranches = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "VehicleUserBranches";
    var LIST_LOADED = false;
    var hash = "";
    return {
        init: function () {
            var $this = this;


            $("#UserId").change(function () {
                $this.LoadBranches($(this).val());
            });
        },



        CustomClear: function () {
            Common.Clear();
        },

        Save: function () {
            var $this = this;
            if (Common.Validate($("#form-info"))) {
                var record = Common.SetValue($("#form-info"));
                var selectedbranches = record.BranchIds;
                var branches = new Array();
                for (var i in selectedbranches) {
                    branches.push(
                        {
                            UserId: Common.GetInt($("#UserId").val()),
                            BranchId: Common.GetInt(selectedbranches[i])
                        });
                }

              
                var record =
                   {
                       '': branches,
                   };

                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving...please wait",

                    success: function (res) {
                        if (res.Success) {
                            $this.CustomClear();
                            Common.ShowMessage(true, { message: Messages.RecordSaved });

                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },

        LoadBranches: function (id) {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "/" + id,
                type: "GET",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading user branches...please wait",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        var j = res.Data;
                        $("#BranchIds").select2("val", j);


                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },

    };
}();
