
var VoucherPosting = function () {
    var API_CONTROLLER = "VoucherPosting";
    var title = "";
    return {
        init: function () {
            var $this = this;
            $("#btnShowReport").click(function () {
                $this.LoadData();
            });
            jQuery('body').on('change', '#chk-select-all', function () {
                var set = jQuery('#item-container tbody tr:not([data-generated="generated"],[data-received="received"]):visible td:nth-child(1) :checkbox');
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    $(this).prop("checked", checked).trigger("change");
                });
                jQuery.uniform.update(set);
            });
            $this.SetPageData();

        },
        LoadData: function () {
            var date1 = $("input[name='FromDate']").val();
            var date2 = $("input[name='ToDate']").val();
            $("#lblReportDate").html("Date: " + date1 + " to " + date2);
            var selectedvoucher = $("#vouchertype-container input:checkbox:checked").map(function () {
                return this.value;
            }).get();

            var qs = "?key=GetVoucherList";
            qs += "&date1=" + date1;
            qs += "&date2=" + date2;
            qs += "&vouchertype=" + selectedvoucher;
            qs += "&accountId=" + $("select[name='AccountId']").val();
            Common.WrapAjax({
                url: Setting.APIBaseUrl + API_CONTROLLER + qs,
                type: "Get",
                data: "",
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: "Loading  voucher...please wait",
                success: function (res) {
                    if (res.Success) {
                        var html = "";
                        var select = "";
                        var amount = 0;
                        var vouchers = res.Data.Vouchers;
                        var users = res.Data.Users;
                        var vtype = Common.GetQueryStringValue("vocuhertype");
                        var vclass = "show";
                        if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                            vclass = "hide";
                        }
                        for (var i in vouchers) {
                            var voucher = vouchers[i];
                            var voucherItems = voucher.VoucherItems;
                            var url = Common.GetTransactionUrl(voucher.TransactionType, voucher.VoucherNumber);
                            var vType = VoucherTypes[voucher.TransactionType];
                            var user = Enumerable.From(users).FirstOrDefault(null, function (p) { return p.Id == voucher.CreatedBy });
                            var createdBy = "";
                            if (user != null)
                                createdBy = user.Name;
                            html += "<tr class='head'>";
                            html += "<td><input type='hidden' class='Id' value='" + voucher.Id + "' data-db-column='Id'><input type='checkbox' class='IsSelected' data-db-column='IsSelected' data-checktrack='false' value='false' data-toggle='tooltip' title='select to post voucher'></td>";
                            html += "<td>" + moment(voucher.Date).format("DD/MM/YYYY") + "</td>";
                            html += "<td>" + ("<a href='" + url + "' title='" + vType.Description + "'>" + vType.Code + "-" + voucher.VoucherNumber + "</a>") + "</td>";
                            html += "<td>" + createdBy + "</td>";
                            html += "<td>" + Common.GetInt(voucher.NetTotal).format() + "</td>";
                            html += "<td colspan='4'></td>";
                            html += "</tr>";
                            //html += "<tr class='' data-parent='" + vouchers.DetailedVoucherNo + "' data-header-row='true' data-exclude-row='true'>";
                            //html += "<td>Account</td>";
                            //html += "<td colspan='2'>Description</td>";
                            //html += "<td>Debit</td>";
                            //html += "<td>Credit</td>";
                            //html += "</tr>";
                            for (var j in voucherItems) {
                                var item = voucherItems[j];
                                html += "<tr>";
                                html += "<td colspan='5'></td>";
                                html += "<td>" + item.AccountName + "</td>";
                                html += "<td>" + item.Description + "</td>";
                                html += "<td>" + Common.GetInt(item.Debit).format() + "</td>";
                                html += "<td>" + Common.GetInt(item.Credit).format() + "</td>";
                                html += "</tr>";
                            }
                        }
                        if (vouchers.length == 0) {

                            html += "  <tr><td colspan='9' style='text-align: center'>No record(s) found</td></tr>";
                            $(".data-container").addClass("hide");
                        }
                        else
                            $(".data-container").removeClass("hide");

                        $("#item-container tbody").html(html);
                        Common.SetCheckChange();
                        App.initUniform();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },

        Save: function () {
            var $this = this;
            $this.SaveRecord(function () {
                Common.ShowMessage(true, { message: "Record saved successfully." });
                $this.LoadData();
            });
        },
        SaveClose: function () {
            var $this = this;
            $this.SaveRecord(function () {
                if ($("#PrintVoucher").is(":checked")) {
                    $this.Print();
                }
                else {
                    var scope = $("#form-info-item");
                    // $this.Close();
                    $this.ListView();
                }

                $this.CustomClear();
            });
        },
        SaveRecord: function (callback) {
            var $t
            var $this = this;
            $(".container-message").hide();
            var mode = "add";
            var scope = $("#form-info");
            var record = Common.SetValue(scope);
            var Items = new Array();
            if (Common.Validate(scope)) {
                Items = Common.SaveItemData();
                Items = Enumerable.From(Items).Where("$.IsSelected=='true'").ToArray();
                var err = "";
                if (Items.length <= 0) {
                    err += "Please select atleast on voucher to post.,";
                }
                if (err != "") {
                    Common.ShowError(err);
                    return;
                }
                record[""] = Items;
                LIST_CHANGED = true;
                Common.WrapAjax({
                    url: Setting.APIBaseUrl + API_CONTROLLER,
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#form-info",
                    blockMessage: "Saving ...please wait",
                    success: function (res) {
                        if (res.Success) {
                            callback();
                        } else {
                            Common.ShowError(res.Error);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
        },

        SetPageData: function () {

            var vtype = Common.GetQueryStringValue("vocuhertype");
            if (vtype != typeof "undefined" && vtype != null && vtype != "") {
                $("#vouchertype-container").addClass("hide");
                $.uniform.update($("input:checkbox[value='" + vtype + "']").prop("checked", true));

                $("th.allth").addClass("hide");
                // var title = "";
                if (vtype == "5") {
                    title = "Cash Receipts Voucher";
                }
                else if (vtype == "6") {
                    title = "Cash Payment Voucher";

                }
                else if (vtype == "7") {
                    title = "Bank Receipts Voucher";

                }
                else if (vtype == "8") {
                    title = "Bank Payment Voucher";

                }
                else if (vtype == "12") {
                    title = "Transfer Voucher";

                }


            }
            else {
                $("#vouchertype-container").removeClass("hide");
                title = "All Voucher List";
                $("th.allth").removeClass("hide");
            }
            $(".page-title .main-title").html(title);
            $("#pageurl").html("<a href='" + window.location.href + "'>" + title + "</a>");
        },

    };
}();