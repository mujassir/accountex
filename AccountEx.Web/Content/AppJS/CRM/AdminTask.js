
var AdminTask = function () {
    var max = 0;
    var DATATABLE_ID = "mainTable";
    var API_CONTROLLER = "AdminTask";
    var LIST_LOADED = false;
    var PageData = new Object();
    return {
        init: function () {
            var $this = this;
            $(document).on("click", "#btn-transfer-project-pmc", function (event) {
                $this.TransferProjectPMC();
            });
            $(document).on("click", "#btn-lock-project", function (event) {
                $this.LockProject();
            });
            $(document).on("click", "#btn-lock-pmc", function (event) {
                $this.LockPmc();
            });
            $(document).on("click", "#btn-missing-pmc-create", function (event) {
                $this.CreateMissingPMC();
            });

            $(document).on("click", "#btnCancel", function (event) {
                $this.ListView();
            });
            $(document).on("click", ".btn-edit", function (event) {
                $this.Edit($(this));
            });
            $(document).on("click", ".btn-delete", function (event) {
                $this.Delete($(this));
            });
            $this.LoadInfo();

        },

        Add: function () {
            var $this = this;
            $this.DetailView();
            Common.Clear();
            $this.CustomClear();
        },
        DetailView: function () {
            Common.GoToTop();
        },

        ListView: function () {

            if (!LIST_LOADED) {
                var url = Setting.APIBaseUrl + API_CONTROLLER;
                LIST_LOADED = true;
                DataTable.BindDatatable(DATATABLE_ID, url);
            }
            $("#Name").focus();
        },

        Close: function () {
            $("#form-info").addClass("hide");
            $("#div-table").removeClass("hide");
        },

        CustomClear: function () {
            Common.Clear();
        },

        TransferProjectPMC: function () {
            var $this = this;
            var fun = function () {
                var record = Common.SetValue($("#form-info"));
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER + "?key=transferProjectPmc",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "transferring...please wait",
                    success: function (res) {
                        if (res.Success) {
                            Common.ShowMessage(true, { message: Messages.RecordSaved });
                            PageData.IsProjectPmcTransferred = res.Data;
                            $this.SetFormControl();
                        }
                        else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
            Common.ConfirmDelete(fun, ActionType.Senstitive, "This will transfer all project and pmc record to next year.Are you sure to continue?");


        },
        LockProject: function () {
            var $this = this;
            var record = Common.SetValue($("#form-info"));
            var message = PageData.IsProjectLocked ? "Unlock" : "Locking";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=lockProject",
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "" + message + "...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Project " + message + " done successfully." });
                        PageData.IsProjectLocked = res.Data;
                        $this.SetFormControl();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },
        LockPmc: function () {
            var $this = this;
            var record = Common.SetValue($("#form-info"));
            var message = PageData.IsPmcLocked ? "Unlock" : "Locking";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=lockPmc",
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "" + message + "...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "PMC " + message + " done successfully." });
                        PageData.IsPmcLocked = res.Data;
                        $this.SetFormControl();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },
        CreateMissingPMC: function () {
            var $this = this;
            var record = Common.SetValue($("#form-info"));
            var message = PageData.IsPmcLocked ? "Unlock" : "Locking";
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=createPmc",
                type: "POST",
                data: record,
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "creating pmc...please wait",
                success: function (res) {
                    if (res.Success) {
                        Common.ShowMessage(true, { message: "Missing PMC creation done successfully." });
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

        },

        LoadInfo: function () {
            var $this = this;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + "?key=info",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "loading...please wait",
                success: function (res) {
                    if (res.Success) {
                        PageData = res.Data;
                        $this.SetFormControl();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        SetFormControl: function () {
            if (PageData.IsProjectPmcTransferred) {
                $("#btn-transfer-project-pmc").addClass("hide");
                $("#btn-transfer-project-pmc").removeClass("hide");
            }
            else {
                $("#btn-transfer-project-pmc").removeClass("hide");
            }
            if (PageData.IsProjectLocked) {
                $("#btn-lock-project").text("Unlock Project Module");
            }
            else {
                $("#btn-lock-project").text("Lock Project Module");
            }
            if (PageData.IsPmcLocked) {
                $("#btn-lock-pmc").text("Unlock PMC Module");
            }
            else {
                $("#btn-lock-pmc").text("Lock PMC Module");
            }
            $("td.action-container div.action-conatiner").removeClass("hide");
        },

    };
}();
