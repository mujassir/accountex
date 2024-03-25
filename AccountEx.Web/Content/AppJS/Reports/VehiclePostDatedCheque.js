
var VehiclePostDatedCheque = function () {
    var API_CONTROLLER = "Report";
    var PageSetting = new Object();
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });

            $(document).on("change", "#Type", function () {
                var type = $("#Type").val();
                if (type == "Summary") {
                    $("#div-summary").removeClass("hide");
                    $("#div-table").addClass("hide");
                }
                else {
                    $("#div-summary").addClass("hide");
                    $("#div-table").removeClass("hide");
                }
            });
            $this.LoadPageSetting();
            $this.LoadAgreements();
            $this.LoadBank();
        },
        LoadData: function () {
            var $this = this;
                 
            var vehicleSaleId = $("#VehicleSaleId").val();
            var status = $("#Status").val();
            var chequeNo = $("#ChequeNo").val();
            var bankId = $("#BankId").val();
            var qs = "?key=GetVehiclePostDatedCheque";
            qs += "&vehicleSaleId=" + vehicleSaleId;
         
            qs += "&status=" + status;
            qs += "&chequeNo=" + chequeNo;
            qs += "&bankId=" + bankId;
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading ...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var records = res.Data;
                        var sn = 1;
                        for (var i in records) {
                            var record = records[i];
                           
                            html += "<tr>";
                            html += "<td>" + sn + "</td>";
                            html += "<td>" + record.ChequeNo + "</td>"; 
                            html += "<td>" + Common.FormatDate(record.ChequeDate, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Customer + "</td>";
                            html += "<td>" + record.ChassisNo + "</td>"
                            html += "<td>" + record.RegNo + "</td>";
                            html += "<td class='align-right'>" + record.Amount + "</td>";
                            html += "<td>" + Common.FormatDate(record.Date, "DD-MM-YYYY") + "</td>";
                            html += "<td>" + record.Banks + "</td>";
                            html += "<td>" +record.Status + "</td>";
                            html += "<td>" + record.Remarks + "</td>";                                                     
                            html += "</tr>";
                            sn++;
                        }
                        if (records.length == 0)
                            html += "  <tr><td colspan='10' style='text-align: center'>No record(s) found</td></tr>";
                        var TotalAmount = Common.GetInt(Enumerable.From(records).Sum("$.Amount"));
                        html += "<tr class='bold grand-total'>";
                        html += "<td colspan = '3' class='align-right'>Total</td>";
                        html += "<td></td>";
                        html += "<td></td>";
                        html += "<td></td>";
                        html += " <td class='align-right'>" + TotalAmount.format() + "</td>";
                        html += "<td colspan = '5' class='align-right'></td>";
                        html += "</tr>";
                        $(".report-table tbody").html(html);

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        LoadPageSetting: function () {

            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            this.LoadAccounts();
            $(".voucher-title").html(PageSetting.FormTitle);
        },
        LoadAgreements: function () {

            var controller = "VehiclePostDatedCheque";
            var $this = this;
            $("#item-container tbody").html("");
            var voucherno = Common.GetInt($("#VoucherNumber").val());
            Common.WrapAjax({
                url: Setting.APIBaseUrl + controller + "/?key=loadAgreements",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading agreements....please wait",
                success: function (res) {
                    if (res.Success) {

                        Common.BindSelect(res.Data, "#VehicleSaleId", true)

                    } else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });

            var $this = this;



        },
        LoadBank: function () {
            var $this = this;
            var accounts = Common.GetLeafAccounts(PageSetting.Banks);
            var html = "<option value=''></option>";
            for (var i in accounts) {
                var account = accounts[i];
                html += "<option value=\"" + account.Id + "\">" + account.AccountCode + "-" + account.Name + "</option>";
            }
            $("select.banks").html(html).select2();


        },
        LoadAccounts: function () {
            var accounts = AppData.COA;
            var filteraccount = new Array();
            var exids = new Array();
            var exids = new Array();
            var $this = this;
            var headAccuntId = 0;
            filteraccounts = Common.GetLeafAccounts(PageSetting.Suppliers);
            var tokens = $.grep(filteraccounts, function (e) { return e.Level == Setting.AccountLevel; });
            var suggestion = new Array();
            for (var i in tokens) {
                var token = tokens[i];
                suggestion.push(
                    {
                        id: token.Id,
                        value: token.AccountCode,
                        label: token.AccountCode + "-" + token.DisplayName

                    }
                );
            }

            $("table tr .AccountCode").autocomplete({
                source: suggestion,
                selectFirst: true,
                autoFocus: true,
                select: function (event, ui) {

                    var voucher = Common.GetQueryStringValue("type").toLowerCase();
                    var ac = Common.GetByCode(ui.item.value);
                    var tr = $(this).parent().parent();
                    if (typeof ac != "undefined" && ac != null) {
                        $(tr).find(":nth-child(1) input.AccountId").val(ac.Id);
                        $(tr).find(":nth-child(2) input.AccountName").val(ac.DisplayName);
                        $(tr).find(":nth-child(2) input.AccountName").focus();
                        $this.MapDescription(tr);
                        $this.LoadBL(tr);
                    }
                }
            });



        },
    };
}();