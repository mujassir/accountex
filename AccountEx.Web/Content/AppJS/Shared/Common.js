Number.prototype.format = function (noOfDecimalPoints, showZero, addBreaket) {
    if (this > -1)
        return commafy(this, noOfDecimalPoints, showZero);
    else {
        if (addBreaket != undefined) {
            decimalPoints = noOfDecimalPoints;
        }
        else
            addBreaket = true;

        if (addBreaket == undefined || addBreaket) {
            return "(" + commafy(this * -1, noOfDecimalPoints, showZero) + ")";
        }
        else
            return commafy(this, noOfDecimalPoints, showZero);
    }

};
String.prototype.format = function () {
    if (this > -1)
        return this.split(/(?=(?:\d{3})+(?:\.|$))/g).join(",");
    else return "(" + (this * -1).toString().split(/(?=(?:\d{3})+(?:\.|$))/g).join(",") + ")";
};
String.prototype.allReplace = function (obj) {
    var retStr = this;
    for (var x in obj) {
        retStr = retStr.replace(new RegExp(x.escapeRegExp(), 'g'), obj[x]);
    }
    return retStr;
};
String.prototype.escapeRegExp = function () {
    return this.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}
String.prototype.isNullOrWhitespace = function () {
    if (typeof this === "undefined" || this == null) return true;
    if (!this.replace)
        return;
    return this.replace(/\s/g, "").length < 1;
};
//Object.prototype.isNullOrWhitespace = function () {
//    if (typeof this === 'undefined' || this == null) return true;
//};
String.prototype.allReplace = function (obj) {
    var retStr = this;
    if (!retStr.replace)
        return;
    for (var x in obj) {
        retStr = retStr.replace(new RegExp(x.escapeRegExp(), 'g'), obj[x]);
    }
    return retStr;
};
String.prototype.escapeRegExp = function () {
    if (!this.replace)
        return;
    return this.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
};
$.fn.scrollView = function () {
    return this.each(function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top
        }, 1000);
    });
}
//Array.prototype.insert = function (index, item) {
//    this.splice(index, 0, item);
//};
function commafy(num, noOfDecimalPoints, showZero) {
    var forceDecimalPoints = false;
    var decimalPoints = 2;              // default value
    if (noOfDecimalPoints != undefined) {
        decimalPoints = noOfDecimalPoints;
    }
    var str = num.toString().split('.');
    if (str[0].length > 3) {

        // US number format (Million, Billion)
        str[0] = str[0].replace(/(\d)(?=(\d{3})+$)/g, '$1,');

        //Pakistani Number format (Lac, Cror)
        //str[0] = str[0].substr(0, str[0].length - 3).replace(/(\d)(?=(\d{2})+$)/g, '$1,') + "," + str[0].substr(str[0].length - 3, 3);
    }
    else if (str[0] == 0) {
        if (showZero)
            str[0] = '0';
        else
            str[0] = '';
    }

    if (str[1] && str[1].length >= decimalPoints) {
        str[1] = str[1].substr(0, decimalPoints);
    }
    if (noOfDecimalPoints != undefined && noOfDecimalPoints == 0)
        return str[0];

    return str.join('.');
}
var Common = function () {
    var confirmCallback;
    return {
        init: function () {
            var $this = this;
            $this.MarkRequired();
            $this.SetDropDown();
            $this.SetCheckChange();
            $this.AllowNumerics();
            $this.InitNumerics();


            //$this.HighlightMenu();
            //Get SiteBase url(global varaible) from Layout page with Razor
            Common["BaseUrl"] = SiteBaseUrl;
            Common["GlobalUploadFolder"] = globalUploadFolder;
            Common["StorageKey"] = StorageKey;
            Common["LocalStoragePrefix"] = "_Accountex_StorageKey_" + StorageKey;
            $("input").attr("autocomplete", "off");
            $("input").attr("autocomplete", "false");
            //jquery hot key
            jQuery.hotkeys.options.filterInputAcceptingElements = false;
            jQuery.hotkeys.options.filterContentEditable = false;
            jQuery.hotkeys.options.filterTextInputs = false;
            $this.InitDateMask();
            $this.InitDatePicker();
            $this.InitTooltip();
            AppData["COA"] = Common.GetData("COA" + Common.LocalStoragePrefix);
            AppData["CustomerDiscount"] = Common.GetData("CustomerDiscount");
            AppData["AccountDetail"] = Common.GetData("AccountDetail" + Common.LocalStoragePrefix);
            $this.SetPageAccess();

            //Common.BindShortKeys([{}])
            $("#btnsave").click(function () {
                // $this.Save();
            });
            $("#btnCancel").click(function () {
                try {
                    $("#errordiv").fadeOut("slow");
                    $this.CustomClear();

                } catch (e) {
                    $this.Clear();
                    $("#Id").val("0");
                }

            });
            $("#ReloginPassword").keyup(function (event) {
                if (event.which == 13)
                    $this.GlobalLogin();
            });
            $("#session-timeout-dialog-keepalive").on("click", function () {

                $this.GlobalLogin();

            });

            $("#session-timeout-dialog-logout").on("click", function () {
                window.location = "../account/logoff";
            });
            $('#session-timeout-dialog').on('shown.bs.modal', function () {
                $("#ReloginPassword").attr("type", "password").focus();
            });
            $('#session-timeout-dialog').on('hidden.bs.modal', function () {
                $("#ReloginPassword").attr("type", "text").focus();
            });


            $("#LoginPassword").keyup(function (event) {
                if (event.which == 13)
                    $this.ContinueDelete();
            });
            $('#credential').on('shown.bs.modal', function () {
                $("#LoginPassword").attr("type", "password").focus();
            });
            $('#credential').on('hidden.bs.modal', function () {
                $("#LoginPassword").attr("type", "text").focus();
            });


            $this.InitTab();

            $("select.Filterselect2").each(function () {
                $(this).select2(
                    {
                        allowClear: true,
                        placeholder: $this.GetSelect2PlaceHolder($(this)),
                        minimumResultsForSearch: Setting.MinimumResultsForSearch

                    });
            });
            $("select.select2").each(function () {
                $(this).select2(
                    {
                        allowClear: true,
                        placeholder: $this.GetSelect2PlaceHolder($(this)),
                        minimumResultsForSearch: Setting.MinimumResultsForSearch,
                    });
            });

            $("#switch-company").change(function () {
                window.location.href = Common.BaseUrl + 'company/SwitchCompany?compnayId=' + $(this).val();
            });
            $("#switch-fiscal").change(function () {
                window.location.href = Common.BaseUrl + 'company/SwitchFiscal?fiscalId=' + $(this).val();
            });
            $('#chkselect_all-select2-option,.chkselect_all-select2-option').click(function () {


                var $fontIcon = $('');



                var $Container = $(this).closest(".form-group");
                var $select2 = $Container.find("select.select2");
                $(this).closest("label").append('<i class="fa fa-spinner fa-spin" style="font-size:24px"></i>');
                if ($(this).is(':checked')) {
                    $select2.find("option").not("option:eq(0)").prop('selected', true);

                }
                else {
                    $select2.find("option").removeAttr('selected');
                }
                $select2.trigger("change");
                $(this).closest("label").find("i.fa-spinner").remove();

            });
            $('.chkselect_all-select2-option-db').click(function () {
                var $Container = $(this).closest(".form-group");
                var $select2 = $Container.find("select.select2");
                $select2.find("option").removeAttr('selected');
                if ($(this).is(':checked')) {
                    $select2.select2("enable", false);
                    if ($select2.attr("data-required"))
                        $this.UpdateRequired($select2, false);
                }
                else {
                    $select2.select2("enable", true)
                    if ($select2.attr("data-required"))
                        $this.UpdateRequired($select2, true);
                }
                $select2.trigger("change");


            });


            //=====================================END init=====================================
        },
        GetSelect2PlaceHolder: function (element) {
            var placeholder = $(element).attr('placeholder');
            var dataplaceholder = $(element).attr('data-placeholder');

            // For some browsers, `attr` is undefined; for others,
            // `attr` is false.  Check for both.
            if (typeof placeholder !== typeof undefined && placeholder !== false)
                return placeholder;
            else if (typeof dataplaceholder !== typeof undefined && dataplaceholder !== false)
                return dataplaceholder;
            else
                return "Select";
        },
        InitDateMask: function () {
            if (navigator.userAgent.indexOf("Android") === -1) {
                $(".date-picker").inputmask("d/m/y", {
                    "placeholder": Setting.DateTimeDateFormat

                }); //multi-char placeholder
            }
        },
        InitTooltip: function () {
            $("[data-toggle=\"tooltip\"]").tooltip();
            $("body").tooltip({
                selector: "[data-toggle=\"tooltip\"]"
            });
        },

        BindSelect2: function ($select) {
            var $this = this;
            $($select).select2(
                     {
                         allowClear: true,
                         placeholder: $this.GetSelect2PlaceHolder($(this)),
                         minimumResultsForSearch: Setting.MinimumResultsForSearch

                     });
        },

        InitNumerics: function () {
            $("input.num3").autoNumeric("init", { aSep: "", dGroup: "3", vMin: "0.000", mDec: 3 });
            $("input.num2").autoNumeric("init", { aSep: "", dGroup: "2", vMin: "0.00", vMax: "100.00" });
            $("input.num4").autoNumeric("init", { aSep: "", mDec: 0 });
            $("input.num5").autoNumeric("init", { aSep: "", mDec: 0, vMin: "0", vMax: "100" });
            $("input.num2dec2").autoNumeric("init", { mDec: 2, vMin: "0.000" });
        },
        InitTimePicker: function () {
            $('.time-picker').timepicker({
                template: false,
                showInputs: false,
                minuteStep: 5
            });
        },

        WordsConversion: function (total, scope) {
            var _this = this;
            var junkVal = total
            junkVal = Math.floor(junkVal);
            var obStr = new String(junkVal);
            numReversed = obStr.split("");
            actnumber = numReversed.reverse();

            if (Number(junkVal) >= 0) {
                //do nothing
            }

            if (Number(junkVal) == 0) {
                $("#lblAmountInWords", scope).html(' ');
                return 'Zero'
            }
            //if (actnumber.length > 9) {
            //    alert('Oops!!!! the Number is too big to covertes');
            //    return false;
            //}

            var iWords = ["Zero", " One", " Two", " Three", " Four", " Five", " Six", " Seven", " Eight", " Nine"];
            var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', ' Nineteen'];
            var tensPlace = ['dummy', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];

            var iWordsLength = numReversed.length;
            var totalWords = "";
            var inWords = new Array();
            var finalWord = "";
            j = 0;
            for (i = 0; i < iWordsLength; i++) {
                switch (i) {
                    case 0:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        inWords[j] = inWords[j] + '';
                        break;
                    case 1:
                        tens_complication();
                        break;
                    case 2:
                        if (actnumber[i] == 0) {
                            inWords[j] = '';
                        }
                        else if (actnumber[i - 1] != 0 && actnumber[i - 2] != 0) {
                            inWords[j] = iWords[actnumber[i]] + ' Hundred and';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]] + ' Hundred';
                        }
                        break;
                    case 3:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                            inWords[j] = inWords[j] + " Thousand";
                        }
                        break;
                    case 4:
                        tens_complication();
                        break;
                    case 5:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                            inWords[j] = inWords[j] + " Lakh";
                        }
                        break;
                    case 6:
                        tens_complication();
                        break;
                    case 7:
                        if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                            inWords[j] = '';
                        }
                        else {
                            inWords[j] = iWords[actnumber[i]];
                        }
                        inWords[j] = inWords[j] + " Crore";
                        break;
                    case 8:
                        tens_complication();
                        break;
                    default:
                        break;
                }
                j++;
            }

            function tens_complication() {
                if (actnumber[i] == 0) {
                    inWords[j] = '';
                }
                else if (actnumber[i] == 1) {
                    inWords[j] = ePlace[actnumber[i - 1]];
                }
                else {
                    inWords[j] = tensPlace[actnumber[i]];
                }
            }
            inWords.reverse();
            for (i = 0; i < inWords.length; i++) {
                finalWord += inWords[i];
            }
            return finalWord;
            //$("#lblAmountInWords", scope).html('Rupees' + finalWord);

        },
        WordsConversionInMillion: function (s) {
            var th = ['', 'thousand', 'million', 'billion', 'trillion'];

            var dg = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];

            var tn = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];

            var tw = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];
            s = s.toString();
            s = s.replace(/[\, ]/g, '');
            if (s != parseFloat(s)) return 'not a number';
            var x = s.indexOf('.');
            if (x == -1) x = s.length;
            if (x > 15) return 'too big';
            var n = s.split('');
            var str = '';
            var sk = 0;
            for (var i = 0; i < x; i++) {
                if ((x - i) % 3 == 2) {
                    if (n[i] == '1') {
                        str += tn[Number(n[i + 1])] + ' ';
                        i++;
                        sk = 1;
                    } else if (n[i] != 0) {
                        str += tw[n[i] - 2] + ' ';
                        sk = 1;
                    }
                } else if (n[i] != 0) {
                    str += dg[n[i]] + ' ';
                    if ((x - i) % 3 == 0) str += 'hundred ';
                    sk = 1;
                }
                if ((x - i) % 3 == 1) {
                    if (sk) str += th[(x - i - 1) / 3] + ' ';
                    sk = 0;
                }
            }
            if (x != s.length) {
                var y = s.length;
                str += 'point ';
                for (var i = x + 1; i < y; i++) str += dg[n[i]] + ' ';
            }
            return str.replace(/\s+/g, ' ') + "";

        },
        InitDatePicker: function () {
            var $this = this;
            $(".date-icon").click(function () {
                $(this).closest(".ac-date").find("input.date-picker").datepicker("show");
                $(this).closest("div").find("input.date-picker").datepicker("show");
            });
            $("input[data-hide-dropdown=\"true\"].date-picker").parent().find("span.date-icon").hide();
            var datepickerSelector = '.date-picker:not([data-fiscal-date="true"]),.date-picker[data-fiscal-date="true"]';
            $(datepickerSelector).on("show", function (e) {
                //console.debug('show', e.date, $(this).data('stickyDate'));

                if (e.date) {
                    $(this).data("stickyDate", e.date);
                }
                else {
                    $(this).data("stickyDate", null);
                }
                var hidedropdown = $this.GetBool($(this).attr("data-hide-dropdown"));
                if (hidedropdown != undefined && hidedropdown && navigator.userAgent.indexOf("Android") === -1) {
                    $(this).datepicker("hide");
                }

            });
            $(datepickerSelector).on("hide", function (e) {
                //console.debug('hide', e.date, $(this).data('stickyDate'));
                var stickyDate = $(this).data("stickyDate");

                if (!e.date && stickyDate) {
                    console.debug("restore stickyDate", stickyDate);
                    $(this).datepicker("setDate", stickyDate);
                    $(this).data("stickyDate", null);
                }
            });
            //  var data = new Date(Common.Fiscal.FromDate);
            $('.date-picker:not([data-fiscal-date="true"])').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true,
            });
            $('.date-picker[data-fiscal-date="true"],.date-mask[data-fiscal-date="true"]').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                todayHighlight: true,
                startDate: new Date(Common.Fiscal.FromDate),
                endDate: new Date(Common.Fiscal.ToDate)
            });
        },
        isNullOrWhiteSpace: function (input) {
            if (typeof input === "undefined" || input == null) return true;
            if (typeof input === "string")
                return input.replace(/\s/g, "").length < 1;
            else
                return false;
        },
        ConfirmDelete: function (callback, actionType, userMessage) {

            var $this = this;
            //$("#btnConfirm").click();
            if ($this.isNullOrWhiteSpace(actionType))
                actionType = ActionType.Delete;
            var message = ''
            if (actionType == ActionType.Delete) {
                if ($this.GetBool(Common.RequiredDeletePassword)) {
                    message = 'Are you sure to delete.Please provide your password?';
                }
                else {
                    message = 'Are you sure to delete...? Click <strong>Yes</strong> to delete.';
                }
            }
            else {
                message = 'You are about to perform a sensitive action.Are you sure to continue.?';

            }
            if (userMessage != null && userMessage !== undefined)
                message = userMessage;


            var $this = this;
            if ($this.GetBool(Common.RequiredDeletePassword)) {
                $("#LoginUserName,#LoginPassword").val("");
                $(".alert-danger").fadeOut("slow");
                $(".message-container", $("#credential")).html(message);
                $("#btncredential").click();
                $("#LoginPassword").focus();
                confirmCallback = callback;


            }
            else {
                $(".message-container", $("#modal-delete")).html(message);
                confirmCallback = callback;
                $("#modal-delete").modal("show");

            }
        },
        GlobalLogin: function (callback) {

            var $this = this;
            var url = window.location.pathname;
            var username = $("#lblusername").html();
            var password = $("#ReloginPassword").val();
            $("#session-timeout-dialog #modelerror").hide();
            //Common.SetData("HSCodes", Common.GetData("HSCodes"));
            var loadHsCodes = Common.GetData("HSCodes") == null ? true : false;
            loadHsCodes = false;
            if (password.trim() != "" && username.trim() != "") {
                var record = "password=" + password + "&username=" + username + "&loadhscodes=" + loadHsCodes + "&IsGlobalLogin=true&FiscalId=" + Common.Fiscal.Id + "&url=" + url;
                Common.WrapAjax({
                    url: "../Account/CheckLogin",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#session-timeout-dialog",
                    blockMessage: "Logging in...please wait",
                    success: function (response) {
                        var res = JSON.parse(response);
                        if (res.Success) {
                            //$.ajax(AJAX_OPTIONS);
                            $("#session-timeout-dialog").modal("hide");
                        }
                        else {
                            Common.ShowError(res.Error);
                            //$("#session-timeout-dialog #modelerror span").html(res.Error);
                            //$("#session-timeout-dialog #modelerror").fadeIn("slow");

                        }


                    },
                    error: function (e) {
                    }
                });
            } else {
                Common.ShowError("Password is required");
                //$("#session-timeout-dialog #modelerror span").html("");
                //$("#session-timeout-dialog #modelerror").fadeIn("slow");

            }
        },
        DoExport: function (button, type, tableSelector) {
            var $this = this;
            var $parent = $(button).closest("div[data-export='true']");
            if (!$parent.length)
                $parent = $(button).closest("#div-table");
            var $tables = $parent.find("table[data-export='true']");
            if (!$tables.length)
                $tables = $parent.find("table.report-table");
            var max = 1000;
            var min = 1;
            if (!$this.isNullOrWhiteSpace(tableSelector))
                $tables = [$(tableSelector)]
            $.each($tables, function (index, table) {
                var randomNumber = Math.floor(Math.random() * (max - min + 1)) + min;
                var options = {
                    fileName: $(document).find("title").text() + "_" + randomNumber,

                };
                var typeOption = new Object();
                switch (type) {
                    case "csv":
                        typeOption =
                            {
                                type: 'csv',
                                numbers: {
                                    html: {
                                        decimalMark: '.',
                                        thousandsSeparator: ','
                                    },
                                    output: {
                                        decimalMark: ',',
                                        thousandsSeparator: ''
                                    }
                                }
                            }
                        break;
                    case "excel":
                    case "xlsx":
                        typeOption =
                            {
                                type: type,
                                worksheetName: 'Sheet1',
                                ignoreCSS: ["input[type='hidden']"],
                                excelstyles: ['background-color', 'color',
                                             'border-bottom-color', 'border-bottom-style',
                                             'border-bottom-width',
                                             'border-top-color', 'border-top-style',
                                             'border-top-width',
                                             'border-left-color',
                                             'border-left-style',
                                             'border-left-width',
                                             'border-right-color',
                                             'border-right-style',
                                             'border-right-width',
                                             'font-family',
                                             'font-size',
                                             'font-weight'
                                ]
                            }
                        break;
                    case "pdf":
                        typeOption =
                             {
                                 type: 'pdf',
                                 jspdf: {
                                     format: 'bestfit',
                                     orientation: 'l',
                                     margins: { right: 10, left: 10, top: 10, bottom: 10 },
                                     //autotable: false,
                                     autotable: {
                                         styles: {
                                             fillColor: 'inherit',
                                             textColor: 'inherit',
                                             fillStyle: 'DF',
                                             lineColor: 200,
                                             lineWidth: 0.1
                                         },
                                         tableWidth: 'auto'
                                     }
                                 }
                             }
                        break;

                    default:
                        typeOption =
                            {
                                type: type,
                            }
                        break;

                }


                $.extend(true, options, typeOption);

                var $clonedTable = $(table).clone(true);
                $clonedTable.find("input[type='hidden']").remove();
                $(table).tableExport(options);

            });

        },
        ConvertToPDF: function (key) {
            var $this = this;
            var err = "";
            var reportName = document.title;

            switch (key) {
                case "ConvertPDF":
                    //  code block
                    var blockMessage = "saving pdf file...please wait";
                    break;
                case "ConvertExcel":
                    var blockMessage = "saving excel file...please wait";
                    break;
                default:
                    Common.ShowError("Pass ConvertPDF OR ConvertExcel key.");
            }

            Common.WrapAjax({
                url: Setting.APIBaseUrl + "Report/?key=" + key,
                type: "POST",
                data: { Data: document.documentElement.outerHTML, ReportName: reportName },
                blockUI: true,
                blockElement: "#form-info",
                blockMessage: blockMessage,
                success: function (res) {
                    if (res.Success) {
                        window.open(res.Data);

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
            var PageSetting = new Object();
            var formSetting = $.parseJSON($("#jsondata #data").html());
            $("#jsondata #data").remove();
            var tokens = formSetting;
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                PageSetting[token.Key] = token.Value;
            }
            return PageSetting;

        },
        InitTab: function () {
            $(".form-body").plusAsTab();
            JoelPurra.PlusAsTab.setOptions({
                // Use enter instead of plus
                // Number 13 found through demo at
                // http://api.jquery.com/event.which/
                key: 13
            });
        },
        ConfirmYes: function () {
            var $this = this;
            $this.ContinueDelete();
        },
        ConfirmDeleteWithYes: function () {
            var $this = this;
            $("#modal-delete").modal("hide");
            confirmCallback();
        },
        initDaterange: function () {
            $("#reportdate-range").daterangepicker({
                opens: (App.isRTL() ? "right" : "left"),
                startDate: moment().subtract("days", 6),
                endDate: moment(),
                minDate: moment().subtract("year", 1),
                maxDate: "12/31/2014",
                show: function () {

                },
                singleDatePicker: false,
                //dateLimit: {
                //    days: 60
                //},
                showDropdowns: false,
                showWeekNumbers: true,
                timePicker: false,
                timePickerIncrement: 1,
                timePicker12Hour: true,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract("days", 1), moment().subtract("days", 1)],
                    'Last 7 Days': [moment().subtract("days", 6), moment()],
                    'Last 30 Days': [moment().subtract("days", 29), moment()],
                    'This Month': [moment().startOf("month"), moment().endOf("month")],
                    'Last Month': [moment().subtract("month", 1).startOf("month"), moment().subtract("month", 1).endOf("month")]
                },
                buttonClasses: ["btn btn-sm"],
                applyClass: " blue",
                cancelClass: "default",
                format: "DD/MM/YYYY",
                separator: " to ",
                locale: {
                    applyLabel: "Apply",
                    fromLabel: "From",
                    toLabel: "To",
                    customRangeLabel: "Custom Range",
                    daysOfWeek: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
                    monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                    firstDay: 1
                }
            },
                function (start, end) {
                    $("#reportdate-range span").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));
                    //$("#FromDate").val(start.format('DD/MM/YYYY'));
                    $("#FromDate").datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", start.format("DD/MM/YYYY"));
                    $("#ToDate").datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", end.format("DD/MM/YYYY"));
                }

            );
            //$('#reportdate-range').on('datepicker-change.daterangepicker', function (ev, picker) {
            //    //do something, like clearing an input
            //    $('#daterange').val('');
            //});

            $(".range_inputs input").removeProp("disabled");
            $("#reportdate-range span").html(moment().subtract("days", 6).format("MMMM D, YYYY") + " - " + moment().format("MMMM D, YYYY"));
            $("#reportdate-range").show();
        },
        BindFileInputEvents: function (bindelement, setelement) {
            $("#" + bindelement).on("fileimageloaded", function (event, file, previewId, index, reader) {
                $(".kv-fileinput-upload").click();
            });
            $("#" + bindelement).on("filecleared", function (event) {

                $("#" + setelement).val("");
            });
            $("#" + bindelement).on("filedeleted", function (event) {


            });
            $("#" + bindelement).on("fileuploaded", function (event, data, previewId, index) {

                var form = data.form, files = data.files, extra = data.extra,
                    response = data.response, reader = data.reader;
                var files = response.Files;
                $("#" + setelement).val(files[0].Url);


            });
        },
        RefreshFileInput: function (bindelement, setelement, url, uplodfolder) {
            debugger;
            if (url != null && url != "") {
                $("#" + bindelement).fileinput("refresh",
                    {
                        overwriteInitial: true,
                        initialPreview: [

                             "<img src=\"" + Setting.UploadRootFolder + "/" + Common.GlobalUploadFolder + "/" + uplodfolder + "/" + url + "\" class=\"file-preview-image img-responsive\">"
                        ],
                        //initialPreviewConfig: [
                        //    { caption: "Desert.jpg", width: "120px", url: "alert('kr')", key: 1 }

                        //],
                    }
                );
            }
            else {
                //$(".fileinput-remove-button").click();
                $("#" + bindelement).fileinput('reset').fileinput('clear');

            }
            this.BindFileInputEvents(bindelement, setelement);
        },
        BindFileInput: function (bindelement, setelement, url, newoption) {


            var options = {
                uploadUrl: url,
                //allowedFileTypes: "['image']",
                //allowedFileExtensions: "['jpg', 'gif', 'png']",
                dropZoneEnabled: true,
                overwriteInitial: true,
                showCaption: false,
                browseLabel: "Pick File"
                //initialPreview: [
                //'<img src="../../Content/images/app-logo.png" class="file-preview-image">'

                //],
                //initialPreviewConfig: [
                //{ caption: "Desert.jpg", width: "120px", url: "/site/file-delete", key: 1 }

                //],
            };
            if (typeof newoption != "undefined")
                $.extend(true, options, newoption);
            $("#" + bindelement).fileinput(options);

            this.BindFileInputEvents(bindelement, setelement);
        },
        //HighlightMenu: function () {
        //   var $this = this;
        //    var url = document.location.pathname + window.location.search;
        //    $("ul.page-sidebar-menu li").removeClass("active").removeClass("open");
        //    $("ul.page-sidebar-menu li a").removeClass("open");
        //    var el = $("a[href*='" + window.location.pathname + window.location.search + "']")[0];
        //    $(el).closest("li").addClass("active").find("a").append('<span class="selected"></span>');
        //    $(el).parent().parent().closest("li").addClass("active open").find("a").append('<span class="selected"></span>');
        //    $(el).parent().parent().parent().parent().closest("li").addClass("active open").find("a").append('<span class="selected"></span>');
        //},
        BindDatePicker: function (selector) {
            $(selector).datepicker({
                orientation: "left",
                autoclose: true,
                format: Setting.DateTimeDateFormat,
            });
        },
        ContinueDelete: function () {

            var $this = this;
            var username = $("#LoginUserName").val();
            var password = $("#LoginPassword").val();

            if (password.trim() != "") {
                var record = "Password=" + password;
                Common.WrapAjax({
                    url: "../Account/ValidateLogin",
                    type: "POST",
                    data: record,
                    blockUI: true,
                    blockElement: "#credential .modal-content",
                    blockMessage: "Authenticating password...please wait",
                    success: function (res) {

                        res = $.parseJSON(res);
                        if (res.Success) {
                            $("#btnmodelcancel").click();
                            confirmCallback();

                        }
                        else {
                            Common.ShowError(res.Error);


                        }

                    },
                    error: function (e) {
                    }
                });




            } else {
                Common.ShowError("Enter delete password.");
            }
        },
        FormatDate: function (date, format) {
            if (!Common.isNullOrWhiteSpace(date))
                return moment(date).format(format);
            else return "";
        },
        SetValue: function (scope) {
            var $this = this;
            var record = new Object();
            $("div[data-save='save']", scope).find("input[type=text],textarea,select,input[type=hidden],input[type=checkbox],input[type=password]").not("[Data-Save='false'],[data-save='false'] input[type=text],[data-save='false'] input[type=hidden],[data-save='false'] select,[data-save='false'] textarea").each(function () {
                if ($(this).hasClass("ac-date") || $(this).hasClass("date-picker") || $(this).hasClass("date-mask")) {
                    record[$(this).attr("Id")] = $this.ChangeDateFormate($(this).val());
                }
                else {

                    record[$(this).attr("Id")] = $(this).val();
                }
            });
            return record;
        },
        SetValueByScope: function (scope) {
            var $this = this;
            var record = new Object();
            $(scope).find("input[type=text],textarea,select,input[type=hidden],input[type=checkbox],input[type=password]").each(function () {
                if ($(this).hasClass("ac-date") || $(this).hasClass("date-picker") || $(this).hasClass("date-mask")) {
                    record[$(this).attr("Id")] = $this.ChangeDateFormate($(this).val());
                }
                else {
                    record[$(this).attr("Id")] = $(this).val();
                }
            });
            return record;
        },

        MapEditData: function (data, scope, withClass, isView) {
            var $this = this;
            var selector = "#"
            if (typeof withClass != "undefined" || withClass != null)
                selector = ".";
            for (var key in data) {
                $(selector + key, scope).val(data[key]);
                if ($(selector + key, scope).hasClass("select2") || $(selector + key, scope).hasClass("Filterselect2"))
                    $(selector + key, scope).select2("val", data[key]);
                if ($(selector + key, scope).is("label"))
                    $(selector + key, scope).html(data[key]);
            }
            $this.SetPageAccess(data, isView);


        },


        MapDataWithPrefixF: function (data, scope, prefix, type) {
            var $this = this;
            for (var key in data) {
                var element = $("#" + prefix + key, scope);
                var dbtype = $(element).attr("data-db-type");
                var formating = $this.GetBool($(element).attr("data-formating"));
                var value = data[key];
                if (dbtype == "int")
                    value = Common.GetInt(value);
                else if (dbtype == "float")
                    value = Common.GetFloat(value);
                else if (dbtype == "date")
                    value = moment(value).format("DD/MM/YYYY")
                else if (dbtype == "datetime")
                    value = moment(value).format("DD/MM/YYYY HH:mm")
                if (formating)
                    value = value.format();
                $(element)[type](value);
            }
        },
        MapDataWithPrefixFClasses: function (data, scope, prefix, type) {
            var $this = this;
            for (var key in data) {

                var element = $("." + prefix + key, scope);
                var dbtype = $(element).attr("data-db-type");
                var formating = $this.GetBool($(element).attr("data-formating"));
                var value = data[key];
                if (dbtype == "int")
                    value = Common.GetInt(value);
                else if (dbtype == "float")
                    value = Common.GetFloat(value);
                else if (dbtype == "date")
                    value = moment(value).format("DD/MM/YYYY")
                else if (dbtype == "datetime")
                    value = moment(value).format("DD/MM/YYYY HH:mm")
                if (formating)
                    value = value.format();
                $(element)[type](value);


            }
        },
        TrimCharAtEnd: function (str, char) {
            var regexpStr = "/" + char + "\s*$/";
            var regExp = new RegExp(regexpStr.escapeRegExp(), "");
            return str.replace(regExp);
        },


        MapItemDataForReports: function (tokens, dataConatiener, templateId, replacePlaceHolder) {
            var $this = this;
            var html = "";
            if (typeof templateId == "undefined" || templateId == null)
                templateId = "#template-item";
            if (typeof dataConatiener == "undefined" || dataConatiener == null)
                dataConatiener = "#item-container";
            if (typeof replacePlaceHolder == "undefined" || replacePlaceHolder == null)
                replacePlaceHolder = false;
            for (var i in tokens) {
                var token = tokens[i];

                var templateHtml = $(templateId).html();
                if (replacePlaceHolder) {
                    var placeholder = $this.MakePlaceHolder(token);
                    templateHtml = templateHtml.allReplace(placeholder);
                }
                $(dataConatiener + " tbody").append(templateHtml);
                for (var key in token) {
                    var element = $(dataConatiener + " tbody tr:last").find("[data-db-column=" + key + "]");
                    var type = $(element).attr("data-db-type");
                    var value = token[key];
                    if (type == "int")
                        value = Common.GetInt(value);
                    else if (type == "float")
                        value = Common.GetFloat(value);
                    else if (type == "date")
                        value = moment(value).format("DD/MM/YYYY")
                    if ($(element).is("span"))
                        $(element).html(value);
                    else $(element).val(value);




                    data[key] != null ? record["{{" + key + "}}"] = data[key] : record["{{" + key + "}}"] = "";


                    var placeholder = $this.MakePlaceHolder(token);
                    templateHtml = templateHtml.allReplace(placeholder);

                }

            }
        },
        MapItemData: function (tokens, dataConatiener, templateId, replacePlaceHolder) {
            var $this = this;
            var html = "";
            if (typeof templateId == "undefined" || templateId == null)
                templateId = "#template-item";
            if (typeof dataConatiener == "undefined" || dataConatiener == null)
                dataConatiener = "#item-container";
            if (typeof replacePlaceHolder == "undefined" || replacePlaceHolder == null)
                replacePlaceHolder = false;
            for (var i in tokens) {
                var token = tokens[i];

                var templateHtml = $(templateId).html();
                if (replacePlaceHolder) {
                    var placeholder = $this.MakePlaceHolder(token);
                    templateHtml = templateHtml.allReplace(placeholder);
                }
                $(dataConatiener + " tbody").append(templateHtml);

                for (var key in token) {
                    var element = $(dataConatiener + " tbody tr:last").find("[data-db-column=" + key + "]");
                    var type = $(element).attr("data-db-type");
                    var formating = $this.GetBool($(element).attr("data-formating"));
                    var value = token[key];
                    if ($this.isNullOrWhiteSpace(value))
                        value = '';
                    if (type == "int")
                        value = Common.GetInt(value);
                    else if (type == "float")
                        value = Common.GetFloat(value);
                    else if (type == "date") {
                        if (!Common.isNullOrWhiteSpace(value))
                            value = moment(value).format("DD/MM/YYYY")
                    }
                    else if (type == "time") {
                        if (!Common.isNullOrWhiteSpace(value))
                            value = moment(value).format("hh:mm A")

                    }
                    else if (type == "bool" && $(element).attr("type") == "checkbox") {

                        if (typeof (value) !== "boolean") {
                            value = $this.GetBool(value);
                        }
                        $(element).prop("checked", value);
                        //$.uniform.update($(element).prop("checked", value));
                    }
                    else if (type == "bool" && $(element).attr("type") != "checkbox") {


                        value = value + "";

                    }
                    if (formating)
                        value = value.format();

                    if ($(element).is("span") || $(element).is("label"))
                        $(element).html(value);
                    else $(element).val(value);
                }

            }
        },
        MapItemDataBySelector: function (token, selector) {
            var $this = this;
            var html = "";

            for (var key in token) {
                var element = $(selector).find("[data-db-column=" + key + "]");
                var type = $(element).attr("data-db-type");
                var value = token[key];
                if (typeof type !== 'undefined' && type !== null) {
                    if (type == "int")
                        value = Common.GetInt(value);
                    else if (type == "float")
                        value = Common.GetFloat(value);
                    else if (type == "date" && moment(value, "YYYY-MM-DD").isValid()) {
                        Common.SetDate($(element), value);
                        value = moment(value).format("DD/MM/YYYY")


                    }
                }
                if ($(element).is("span"))
                    $(element).html(value);
                else $(element).val(value);
            }

        },
        SaveItemData: function (table) {
            var $this = this;
            var Items = new Array();
            var masterData = new Object();
            if (typeof table == "undefined" || table == null)
                table = "#item-container";
            $("[data-include-in-item]").each(function () {
                var dbId = $(this).attr("Id");
                if ($(this).attr("data-item-id"))
                    dbId = $(this).attr("data-item-id");
                if ($(this).hasClass("ac-date") || $(this).hasClass("date-picker") || $(this).hasClass("date-mask")) {

                    masterData[dbId] = $this.ChangeDateFormate($(this).val());
                }
                else {
                    masterData[dbId] = $(this).val();
                }
            });
            $(table + " tbody tr:not([data-exclude-row=true])").each(function () {
                var item = new Object();

                $(this).find("[data-db-column]").each(function () {
                    var type = $(this).attr("data-db-type");
                    var value = $(this).val();
                    if (type == "int")
                        value = Common.GetInt(value);
                    else if (type == "float" || type == "decimal")
                        value = Common.GetFloat(value);
                    else if (type == "date")
                        value = $this.ChangeDateFormate(value);
                    else if (type == "bool" && $(this).attr("type") == "checkbox")
                        value = $(this).is(":checked");

                    item[$(this).attr('data-db-column')] = value;
                });
                $.extend(true, item, masterData);
                Items.push(item);
            });

            return Items;
        },
        SaveItemDataBySelector: function (selector, isParent) {
            var $this = this;
            var Items = new Array();
            $(selector).each(function () {
                var item = new Object();

                $(this).find("[data-db-column]").each(function () {
                    var type = $(this).attr("data-db-type");
                    var value = $(this).val();
                    if (type == "int")
                        value = Common.GetInt(value);
                    else if (type == "float")
                        value = Common.GetFloat(value);
                    else if (type == "date")
                        value = $this.ChangeDateFormate(value);
                    else if (type == "bool" && $(this).attr("type") == "checkbox")
                        value = $(this).is(":checked");

                    item[$(this).attr('data-db-column')] = value;
                });
                Items.push(item);
            });
            if (isParent)
                return Items[0];
            else
                return Items;
        },
        MakePlaceHolder: function (data) {
            var record = new Object();
            for (var key in data) {
                data[key] != null ? record["{{" + key + "}}"] = data[key] : record["{{" + key + "}}"] = "";

            }
            return record;
        },
        MapDataWithPrefix: function (data, scope, prefix, type) {
            for (var key in data) {
                if (typeof data[key] == "number")
                    $("#" + prefix + key, scope)[type](data[key].format());
                else
                    $("#" + prefix + key, scope)[type](data[key]);
            }
        },
        MakeQueryString: function () {
            var $this = this;
            var querystring = "";
            $("[data-querystring=true]").each(function () {
                querystring += "&" + $(this).attr("Id") + "=" + $(this).val();
            });
            return querystring;
        },
        GetInt: function (number) {
            var $this = this;
            var replchars = { ',': "", '%': "", '(': "", ')': "", '_': "", ' ': "" };
            if (number == undefined)
                number = "0";
            if (typeof number != "string")
                number = number.toString().allReplace(replchars);
            if (typeof number == "string")
                number = number.allReplace(replchars);

            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return Math.round(parseFloat(number));
            }
        },
        GetFloorInt: function (number) {
            var $this = this;
            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return Math.floor(parseFloat(number));
            }
        },
        GetCeilInt: function (number) {
            var $this = this;
            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return Math.ceil(parseFloat(number));
            }
        },
        GetBool: function (number) {
            var $this = this;
            if (number == undefined || number == "") {
                return false;
            }

            if (typeof (number) === "boolean") {
                return number
            }
            else {
                number = number.toLowerCase();
                return number == 'true' ? true : false;
            }
        },
        GetFloat: function (number) {
            var $this = this;

            var replchars = { ',': "", '%': "", '(': "", ')': "", '_': "", ' ': "" };
            var $this = this;
            if (number == undefined)
                number = "0";
            if (typeof number != "string")
                number = number.toString().allReplace(replchars);
            if (typeof number == "string")
                number = number.allReplace(replchars);

            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return parseFloat(number);
            }
        },
        GetFloatHtml: function (number) {
            var replchars = { ',': "", '%': "", '(': "", ')': "" };
            var $this = this;
            if (number == undefined)
                number = "0";
            if (typeof number != "string")
                number = number.toString().allReplace(replchars);
            if (typeof number == "string")
                number = number.allReplace(replchars);
            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return parseFloat(number);
            }
        },
        GetIntHtml: function (number) {
            var replchars = { ',': "", '%': "" };
            var $this = this;
            if (number == undefined)
                number = "0";
            if (typeof number != "string")
                number = number.toString().allReplace(replchars);
            if (typeof number == "string")
                number = number.toString().allReplace(replchars);
            if (number == undefined || isNaN(number) || number == "") {
                return 0;
            }
            else {
                return parseInt(number);
            }
        },
        SetDropDown: function () {
            var $this = this;
            $("select[data-trackchange='true']").change(function () {
                var mytag = $("option:selected", this).attr("data-custom");
                if (isNaN(mytag))
                    mytag = 0;
                $(this).parent().find("input[type='hidden'],input[type='text']").not(".select2-input,.select2-offscreen").val(mytag);
            });
        },
        SetCheckChange: function () {
            var $this = this;
            $("[data-checktrack='true'],[data-checktrack='false']").change(function () {
                $(this).val($(this).prop("checked"));
            });
        },
        SetCheckValue: function (data) {
            var $this = this;
            if (data == null)
                return;
            $("[data-checktrack='true'],[data-checktrack='false']").each(function () {
                var value = data[$(this).attr("Id")];
                if (typeof (value) !== "boolean") {
                    value = $this.GetBool(value);
                }

                $.uniform.update($(this).prop("checked", value));
            });
        },
        GetQueryStringValue: function (name) {
            var $this = this;
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },
        SetDate: function (el, dt, isFormated) {

            var $this = this;
            var date = dt;
            if (typeof isFormated === undefined || !isFormated)
                date = moment(dt).format("DD/MM/YYYY");
            $(el).datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", date);
            //$(el).datepicker('update');
        },
        GetKeyFromEnum: function (value, object) {
            for (var prop in object) {
                if (object.hasOwnProperty(prop)) {
                    if (object[prop] === value) {
                        return prop;
                    }
                }
            }

        },
        Clear: function () {
            var $this = this;
            var date = Common.GetTodayDate(2);
            $("div[data-save='save']").find("input[type=text]:not([data-clear-type='date'],[data-clear='false']),textarea,input[type=hidden]:not([data-clear='false']),input[type=password]").val("");
            $("div[data-save='save']").find("select.select2:not([data-clear='false']),select.Filterselect2:not([data-clear='false'])").each(function () {

                $(this).select2("val", $("option:first-child", this).attr("value"));
            });
            $("div[data-save='save']").find("input[type=hidden]:not([data-clear='false'])").val("0");
            $("[data-clear-type='0']").val("0");
            $("[data-clear-type='date']").each(function () {
                $(this).datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", date);
            });

            //$(".date-picker").datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", date);
            //$("[data-clear-type='date']").val(date);
            //date = moment(date, 'DD-MM-YYYY').toDate();
            //$(".date-picker").datepicker("update", new Date());
            //$("[data-clear-type='date']").datepicker('setValue', '2017-11-11').datepicker('update');
            //$("[data-clear-type='date']").datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", date);
            $("[data-checktrack='false']").prop("checked", false);
            $("[data-checktrack='false']").val(false);
            $("[data-checktrack='true']").prop("checked", true);
            $("[data-checktrack='true']").val(true);
            $("#errordiv").fadeOut("slow");
            $(".portlet .container-message").addClass("hide");
            $(".fileinput-remove-button").click();
            $("[data-checktrack='true'],[data-checktrack='false']").each(function () {
                $.uniform.update($(this));
            });
            //$this.InitDatePicker();
        },
        ClearByScope: function (scope) {
            if (typeof scope == "string")
                scope = $(scope);
            var $this = this;
            var date = Common.GetTodayDate(2);
            $("div[data-save='save']", scope).find("input[type=text]:not([data-clear-type='date'],[data-clear='false']),textarea,input[type=hidden]:not([data-clear='false']),input[type=password]").val("");
            $("div[data-save='save']", scope).find("select.select2").each(function () {

                $(this).select2("val", $("option:first-child", this).attr("value"));
            });
            $("div[data-save='save']", scope).find("input[type=hidden]:not([data-clear='false'])").val("0");
            $("[data-clear-type='0']", scope).val("0");
            $("[data-clear-type='date']", scope).datepicker({ format: Setting.DateTimeDateFormat }).datepicker("setDate", date);

            $("[data-checktrack='false']", scope).prop("checked", false);
            $("[data-checktrack='false']", scope).val(false);
            $("[data-checktrack='true']", scope).prop("checked", true);
            $("[data-checktrack='true']", scope).val(true);
            $("#errordiv").fadeOut("slow");
            $(".portlet .container-message").addClass("hide");
            $(".fileinput-remove-button").click();
            // $('.choosen').val('').trigger("chosen:updated");
        },
        UpdateChosen: function () {
            var $this = this;
            $(".choosen").val("").trigger("chosen:updated");
        },
        goToByScroll: function (id) {
            // Remove "link" from the ID
            //    id = id.replace("link", "");
            //    // Scroll
            //    $('html,body').animate({
            //        scrollTop: $("#" + id).offset().top
            //    },
            //'slow');
        },
        Validate: function (scope) {
            var $this = this;
            var error = "";
            var isvalidated = true;
            $("[data-required='required']", scope).each(function () {
                if ($(this).val() == "" || $(this).val() == null) {
                    $(this).addClass("notValidated");
                    isvalidated = false;
                    error += $(this).attr("data-message") + ",";
                }
                else {
                    $(this).removeClass("notValidated");
                    var isFiscal = $(this).attr("data-fiscal-date");
                    var date = moment($(this).val(), 'DD/MM/YYYY');
                    date = date.toDate();
                    date.setHours(0, 0, 0, 0);
                    date = date.getTime();
                    if (isFiscal) {
                        var fromDate = new Date(Common.Fiscal.FromDate);
                        fromDate.setHours(0, 0, 0, 0);
                        var toDate = new Date(Common.Fiscal.ToDate);
                        toDate.setHours(0, 0, 0, 0)
                        if (!(fromDate.getTime() <= date && toDate.getTime() >= date)) {
                            error += "date should be within current fiscal year,";
                            $(this).addClass("notValidated");
                            isvalidated = false;
                        }
                        else
                            $(this).removeClass("notValidated");

                    }
                }
            });

            if (!isvalidated) {
                $this.ShowError(error);
            }
            else {
                $("#errordiv").fadeOut("slow");
            }
            return isvalidated;
        },
        MarkRequired: function () {

            var $this = this;
            $("[data-required='required']").each(function () {
                //var attr = $(this).parent().find("span.date-icon").length;
                if ($(this).hasClass("ac-date") || $(this).parent().find("span.date-icon").length > 0) {

                    var previoustd = $(this).parent().parent().prev();
                    //alert(previoustd.html());
                    $(previoustd).append("&nbsp;<span class=required>*</span>");
                } else {
                    previoustd = $(this).parent().prev();
                    $(previoustd).append("&nbsp;<span class=required>*</span>");
                }
            });
        },
        UpdateRequired: function (element, markRequired) {

            var $this = this;
            if (markRequired) {
                if ($(element).hasClass("ac-date") || $(element).parent().find("span.date-icon").length > 0) {

                    var previoustd = $(element).parent().parent().prev();
                    $(previoustd).find("span.required").remove();
                    $(previoustd).append("&nbsp;<span class=required>*</span>");
                } else {
                    previoustd = $(element).parent().prev();
                    $(previoustd).find("span.required").remove();
                    $(previoustd).append("&nbsp;<span class=required>*</span>");

                }
                $(element).attr("data-required", "required");
            }
            else {

                if ($(element).hasClass("ac-date") || $(element).parent().find("span.date-icon").length > 0) {

                    var previoustd = $(element).parent().parent().prev();
                    $(previoustd).find("span.required").remove();
                    $(element).attr("data-required", "false");
                } else {
                    previoustd = $(element).parent().prev();
                    $(previoustd).find("span.required").remove();
                    $(element).attr("data-required", "false");
                }
            }

        },
        SetDisplay: function (item, element) {
            var $this = this;
            if ($(element).attr("Isopen") == "false") {
                $(element).attr("Isopen", true);
                $("." + item).show();
                $(element).html("<img src='Images/toogle_minus.png' alt='Hide' />");
            }
            else {
                $(element).attr("Isopen", false);
                $("." + item).hide();
                $(element).html("<img src='Images/toogle_plus.png' alt='Show' />");
            }
        },
        CloseMessage: function (v) {
            $("#" + v).fadeOut("slow");
        },
        ShowMessage: function (isSuccess, options, toasterOptions) {
            var _this = this;
            toastr.clear();
            var errors = options.message;
            if (typeof errors == 'string') {

                errors = errors.replace(/(^,)|(,$)/g, "");
                errors = errors.split(',');
            }
            var html = "<ul>";
            if (typeof errors == 'object') {
                for (var i in errors) {
                    html += "<li>" + errors[i] + "</li>";
                }
            }
            else {
                html += errors;
            }
            html += "</ul>";


            toastr.options = {
                "closeButton": true,
                "debug": false,
                "positionClass": "toast-top-right",
                //"positionClass": "toast-top-full-width",
                "onclick": null,
                "preventDuplicates": true,
                "newestOnTop": true,
                "progressBar": true,
                "showDuration": "1000",
                "hideDuration": "1000",
                "timeOut": "15000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }

            if (typeof toasterOptions != undefined && toasterOptions != null)
                $.extend(true, toastr.options, toasterOptions);

            // $(el).find(".message-content").html(html);
            if (isSuccess) {
                $TOASTER = toastr["success"](html, options.title ? options.title : "Success!")

            }
            else {

                $TOASTER = toastr["error"](html, options.title ? options.title : "Error!")
            }

        },
        //ShowMessage: function (isSuccess, options) {
        //    var $this = this;
        //    var el = $(".portlet:visible .container-message")[0];
        //    if (el == undefined) el = $(".container-message")[0];
        //    $(el).removeClass("hide").show();
        //    var errors = options.message;
        //    if (typeof errors == "string") {
        //        errors = errors.replace(/,\s*$/, "");
        //        errors = errors.split(",");
        //    }
        //    if (errors.length > 1) {
        //        var errorhtml = "<ul>";
        //        if (typeof errors == "object") {
        //            for (var i in errors) {
        //                errorhtml += "<li>" + errors[i] + "</li>";
        //            }
        //        }
        //        else {
        //            errorhtml += errors;
        //        }
        //        errorhtml += "</ul>";
        //    }
        //    else
        //        errorhtml = options.message;

        //    $(el).find(".message-content").html(errorhtml);
        //    if (isSuccess) {
        //        $(el).removeClass("alert-danger").addClass("alert-success");
        //        $(el).find("strong").html(options.title ? options.title : "Success!");
        //        $(el).fadeOut(15000);
        //    }
        //    else {
        //        $(el).removeClass("alert-success").addClass("alert-danger");
        //        $(el).find("strong").html(options.title ? options.title : "Error!");
        //    }
        //    $this.GoToTop();
        //},
        GoToTop: function () {
            $(".icon-arrow-up").click();
        },

        GetTodayDate: function (type) {
            var $this = this;
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();

            if (dd < 10) {
                dd = "0" + dd;
            }
            if (mm < 10) {
                mm = "0" + mm;
            }
            if (type == 1)
                today = mm + "/" + dd + "/" + yyyy;
            else if (type == 2)
                today = dd + "/" + mm + "/" + yyyy;
            else if (type == 3)
                today = yyyy + "/" + mm + "/" + dd;
            return today;
        },
        GetMultiSelectValue: function (element) {
            var tokens = [];
            $("#" + element + " :selected").each(function (i, selected) {
                tokens[i] = $(selected).text();
            });
            return tokens.join("<br />");
        },
        FormateDate: function (currentdate, isDoubleDigit) {
            var $this = this;
            var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/"
                            + currentdate.getFullYear();
            if (isDoubleDigit == true)
                datetime = $this.DoubleDigit(currentdate.getDate()) + "/" + $this.DoubleDigit(currentdate.getMonth() + 1) + "/"
                            + $this.DoubleDigit(currentdate.getFullYear());
            return datetime;
        },
        DoubleDigit: function (number) {
            if (number > 9)
                return number + "";
            return "0" + number;
        },
        ChangeDateFormate: function (key) {
            var $this = this;
            if (key.trim() != "") {
                var data = key.split("/");
                var changedate = data[2] + "/" + data[1] + "/" + data[0];
                key = changedate;
            }
            return key;
        },
        GetCurrentTime: function () {
            var $this = this;
            var currentdate = new Date();
            var time = currentdate.getHours() + ":"
                            + currentdate.getMinutes() + ":"
                            + currentdate.getSeconds();
            return time;
        },
        ShowError: function (error, toasteroptions) {
            //$("#errordiv").removeClass("hide").show();
            //$("#errordiv .message").html(error);
            this.ShowMessage(false, { message: error }, toasteroptions);
            $(".scroll-to-top").click();
        },
        goToByScroll: function (element) {
            if (typeof element == "undefined" || element == null)
                element = "#form-info";
            $('html,body').animate({
                scrollTop: $(element).offset().top
            },
        'slow');
        },
        MakeQueryStringAll: function (scope) {
            var _this = this;
            var querystring = "";
            //$("[data-querystring=true]").each(function () {

            $("div[data-querystring=true]", scope).find("input[type=text],textarea,select,input[type=hidden],input[type=checkbox],input[type=password]").not("[Data-Save='false'],[data-save='false'] input[type=text],[data-save='false'] input[type=hidden],[data-save='false'] select,[data-save='false'] textarea").each(function () {


                if ($(this).hasClass("ac-date") || $(this).hasClass("date-picker") || $(this).hasClass("date-mask")) {

                    querystring += "&" + $(this).attr("Id") + "=" + _this.ChangeDateFormate($(this).val());

                }
                else {

                    querystring += "&" + $(this).attr("Id") + "=" + $(this).val();
                }
            });
            return querystring;
        },
        OpenInNewTab: function (page) {
            var $this = this;
            if (opener && !opener.closed) {
                opener.location.href = page;
            }
            else {
                opener = window.open(page, "main");
            }
        },
        WrapAjax: function (option) {
            var $this = this;
            var element = $("body");
            var message = "Loading...please wait...";
            if (option.type.toLowerCase() == "post")
                message = "Processing...please wait...";
            else if (option.type.toLowerCase() == "delete")
                message = "Deleting Record...please wait...";
            if (typeof option.blockUI == "undefined" || option.blockUI == null) {
                $(".page-loading").remove();
                var path = "../Content/themes/";
                $("body").append("<div class=\"page-loading hidden-print\"><img src=\"" + path + "loading-spinner-grey.gif\"/>&nbsp;&nbsp;<span>" + message + "</span></div>");
            }
            else if (option.blockUI) {

                if (typeof option.blockElement != "undefined" || option.blockElement != null)
                    element = $(option.blockElement);
                if (typeof option.blockMessage != "undefined" || option.blockMessage != null)
                    message = option.blockMessage;

                App.blockUI({
                    target: element,
                    message: message,
                    boxed: true,
                    zIndex: 20050
                    //iconOnly: true
                });
            }
            var ajaxoption = {
                url: option.url,
                type: option.type,
                async: option.async,
                data: option.data,
                contentType: option.contentType,
                dataType: option.dataType,
                success: function (res) {
                    App.unblockUI(element);
                    option.success(res);

                },
                error: function (e) {
                    App.unblockUI(element);
                    option.error(e);

                }
            };
            if (option.blockUI)
                $.ajax(ajaxoption);
            else {
                Pace.ignore(function () {
                    $.ajax(ajaxoption);
                });
            }

        },
        BlockUI: function (option) {
            var $this = this;
            var element = $("body");
            var message = "Loading...please wait...";
            if (typeof option.blockUI == "undefined" || option.blockUI == null) {
                $(".page-loading").remove();
                var path = "../Content/themes/";
                // $("body").append("<div class=\"page-loading hidden-print\"><img src=\"" + path + "loading-spinner-grey.gif\"/>&nbsp;&nbsp;<span>" + message + "</span></div>");
            }
            else if (option.blockUI) {

                if (typeof option.blockElement != "undefined" || option.blockElement != null)
                    element = $(option.blockElement);
                if (typeof option.blockMessage != "undefined" || option.blockMessage != null)
                    message = option.blockMessage;

                App.blockUI({
                    target: element,
                    message: message,
                    boxed: true,
                    zIndex: 20050
                    //iconOnly: true
                });
            }

        },
        InitCounterup: function () {
            if (!$().counterUp) {
                return;
            }

            $("[data-counter='counterup']").counterUp({
                delay: 10,
                time: 1000
            });
        },
        AllowNumerics: function () {
            var $this = this;
            //$('.num3').autoNumeric('init', { aSep: '', dGroup: '2', vMin: '0.00', });
            //$('.num4').autoNumeric('init', { aSep: ',', dGroup: '4', vMin: '0.0000', });
            //$('.num2').autoNumeric('init', { aSep: ',', dGroup: '2', vMin: '0.00', vMax: '100.00' });
            //$('.num').autoNumeric('init', { aSep: '', dGroup: '0', vMin: '0.00', vMax: '100.00' });
            //$('input[data-db-type="float"]').autoNumeric('init', { aSep: '', vMin: '0', vMax: '999999999.00', lZero: 'deny', aPad: true });
            //$('input[data-db-type="int"]').autoNumeric('init', { aSep: '', dGroup: '0', vMin: '0',vMax: '999999999', lZero: 'deny', aPad: true });
            //$('.page-container').autoNumeric('init', 'input[data-db-type="float"]', { aSep: ',', dGroup: '2', vMin: '0.00', });
            //$('body').autoNumeric('init', 'input[data-db-type="int"]', { aSep: ',', dGroup: '0', vMin: '0.00', });


            $(".number").keydown(function (event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                    // Allow: Ctrl+A
                    (event.keyCode == 65 && event.ctrlKey === true) ||
                    // Allow: home, end, left, right
                    (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            });
        },
        GetData: function (key) {

            var data = localStorage.getItem(key);
            if (data != undefined && data != null) {
                data = $.parseJSON(data);
                return data.Data;
            }
            else {
                return null;
            }

        },
        GetCompleteData: function (key) {
            var data = localStorage.getItem(key);
            if (data != undefined && data != null) {
                return $.parseJSON(data);
            } else {
                return null;
            }

        },
        GetDataDate: function (key) {
            var data = localStorage.getItem(key);
            if (data != undefined && data != null) {
                data = $.parseJSON(data);
                return data.Date;
            }
            else {
                return null;
            }


        },
        SetData: function (key, data) {
            var d =
                {
                    Date: Common.GetTodayDate(2),
                    key: key,
                    Data: data
                };
            AppData[key] = d;
            localStorage.setItem(key, JSON.stringify(d));
        },
        BindSelect: function (accounts, element, addBlankRow, justBind) {
            var $this = this;
            var html = "";
            if (addBlankRow)
                html += "<option></option>";
            for (var i = 0; i < accounts.length; i++) {
                var token = accounts[i];
                html += "<option value='" + token.Id + "'>" + token.Name + "</option>";
            }
            if (justBind)
                html = $(element).html();
            $(element).html(html).select2(
                 {
                     allowClear: true,
                     placeholder: $this.GetSelect2PlaceHolder($(this)),
                     minimumResultsForSearch: Setting.MinimumResultsForSearch
                 });

        },
        BindSelectWithAddress: function (accounts, element, addBlankRow) {
            var $this = this;
            var html = "";
            if (addBlankRow)
                html += "<option></option>";
            for (var i = 0; i < accounts.length; i++) {
                var token = accounts[i];
                html += "<option data-address='" + token.Address + "' value='" + token.Id + "'>" + token.Name + "</option>";
            }
            $(element).html(html).select2({
                allowClear: true,
                placeholder: $this.GetSelect2PlaceHolder($(element)),
                minimumResultsForSearch: Setting.MinimumResultsForSearch
            });
        },
        BindItemSelectWithPurchaseRate: function (accounts, element, addBlankRow) {
            var $this = this;
            var html = "";
            if (addBlankRow)
                html += "<option></option>";
            for (var i = 0; i < accounts.length; i++) {
                var token = accounts[i];
                html += "<option data-purchaseprice='" + token.PurchasePrice + "' value='" + token.AccountId + "'>" + token.Name + "</option>";
            }
            $(element).html(html).select2({
                allowClear: true,
                placeholder: $this.GetSelect2PlaceHolder($(element)),
                minimumResultsForSearch: Setting.MinimumResultsForSearch
            });
        },
        BindMultiSelect: function (accounts, element) {
            var $this = this;
            var html = "";
            for (var i = 0; i < accounts.length; i++) {
                var token = accounts[i];
                html += "<option value='" + token.Id + "'>" + token.Name + "</option>";
            }
            $(element).html(html).multiSelect();
        },
        GetChallanPeriod: function (fromMonth, fromYear, toMonth, toYear) {
            var duration = "";
            if (fromMonth == toMonth && fromYear == toYear)
                duration = fromMonth + " " + fromYear;
            else if (fromMonth != toMonth && fromYear == toYear)
                duration = fromMonth + " to " + toMonth + " " + fromYear;
            else
                duration = fromMonth + " " + fromYear + " to " + toMonth + " " + toYear;
            return duration;

        },
        GetLeafAccounts: function (parentId) {
            $this = this;
            var l4Accounts = new Array();

            var parentAccounts = $.grep(AppData.COA, function (e) { return e.ParentId == parentId; });
            for (var i in parentAccounts) {
                var account = parentAccounts[i];
                if (account.Level == 4) {
                    l4Accounts.push(account);
                } else
                    Array.prototype.push.apply(l4Accounts, $this.GetLeafAccounts(account.Id));
            }
            return l4Accounts;


        },
        GetMultipleLeafAccounts: function (parentIds) {
            $this = this;
            var l4Accounts = new Array();
            for (var id in parentIds) {
                var parentId = parentIds[id];
                var parentAccounts = $.grep(AppData.COA, function (e) { return e.ParentId == parentId; });
                for (var i in parentAccounts) {
                    var account = parentAccounts[i];
                    if (account.Level == 4) {
                        l4Accounts.push(account);
                    } else
                        Array.prototype.push.apply(l4Accounts, $this.GetLeafAccounts(account.Id));
                }
            }
            return l4Accounts;


        },
        GetById: function (id) {
            $this = this;
            return $.grep(AppData.COA, function (e) { return e.Id == id })[0];
        },

        GetById: function (id) {
            $this = this;
            return $.grep(AppData.COA, function (e) { return e.Id == id })[0];
        },
        UpdateParentId: function (accountId, parentAccountId) {
            $this = this;
            var account = $.grep(AppData.COA, function (e) { return e.Id == accountId; })[0];
            if (!$this.isNullOrWhiteSpace(account))
                account.ParentId = parentAccountId;
        },
        GetByName: function (name) {
            $this = this;
            // var accounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel });
            return $.grep(AppData.COA, function (e) { return !Common.isNullOrWhiteSpace(e.Name) && e.Name.toLowerCase() == name.toLowerCase() })[0];
        },
        GetByCode: function (code) {
            $this = this;
            // var accounts = $.grep(AppData.COA, function (e) { return e.Level == Setting.AccountLevel });
            return $.grep(AppData.COA, function (e) { return !Common.isNullOrWhiteSpace(e.AccountCode) && e.AccountCode.toLowerCase() == code.toLowerCase() })[0];
        },
        GetAccountDetailByCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return !Common.isNullOrWhiteSpace(e.Code) && e.Code.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetAccountDetailByAccountId: function (accountId) {

            var data = $.grep(AppData.AccountDetail, function (e) { return e.AccountId == accountId })[0];
            return data;
        },
        GetDiscountDetail: function (productid) {


            if (AppData.CustomerDiscount == null || AppData.CustomerDiscount == undefined)
                return 0;
            var customerid = $("#AccountId").val();
            var discount = $.grep(AppData.CustomerDiscount, function (e) { return e.CustomerId == customerid && e.COAProductId == productid; })[0];
            if (discount != null)
                return discount.Discount;
            else
                return 0;
        },
        GetByBarCode: function (code) {

            var data = $.grep(AppData.AccountDetail, function (e) { return !Common.isNullOrWhiteSpace(e.BarCode) && e.BarCode.toLowerCase() == code.toLowerCase(); })[0];
            return data;
        },
        GetLeafAccounts1: function (exids) {
            $this = this;
            var l4Accounts = new Array();
            var parentAccounts = $.grep(AppData.COA, function (e) {
                return $.inArray(e.ParentId, exids) === -1;
            });
            //var parentAccounts = $.grep(AppData.COA, function (e) {
            //    return e.ParentId != exids[0] && e.ParentId != exids[1];
            //});
            for (var i in parentAccounts) {
                var account = parentAccounts[i];
                if (account.Level == 4) {
                    l4Accounts.push(account);
                } else
                    Array.prototype.push.apply(l4Accounts, $this.GetLeafAccounts1(exids));
            }
            return l4Accounts;
        },
        GetAllLeafAccounts: function (exids) {
            $this = this;
            var parentAccounts = $.grep(AppData.COA, function (e) {
                return $.inArray(e.ParentId, exids) === -1 && e.Level == Setting.AccountLevel;
            });
            return parentAccounts;
        },
        filterarray: function (list, filter) {
            return $.grep(list, function (obj) {
                return $.inArray(obj.Rating, filter) !== -1;
            });
        },
        CheckIfLoadCOA: function (callback, blockUI) {
            var $this = this;

            var lastDate = Common.GetData("LastDate" + Common.LocalStoragePrefix);
            var storageKey = Common.GetData("StorageKey" + Common.LocalStoragePrefix);
            var COA = Common.GetData("COA" + Common.LocalStoragePrefix);
            var forceReload = false;
            if (storageKey == "" || COA == null || COA == 'undefined')
                forceReload = true;
            if (forceReload) {
                $this.LoadCOA(callback, blockUI);
                return;
            }

            var data =
                {
                    LastDate: lastDate,
                    key: 'CheckIfLoadCOA',
                    StorageKey: storageKey
                }
            return Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA",
                type: "GET",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: blockUI,
                blockElement: "body",
                blockMessage: "Validating chart of accounts...please wait",
                success: function (res) {
                    if (res.Success) {
                        if (res.Data.LoadCOA)
                            Common.LoadCOA(callback, blockUI);
                        else if (typeof callback != "undefined")
                            callback();
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        CheckIfFiscalChanged: function (callback, blockUI) {
            var $this = this;

            var lastDate = Common.GetData("LastDate" + Common.LocalStoragePrefix);
            var storageKey = Common.GetData("StorageKey" + Common.LocalStoragePrefix);
            var COA = Common.GetData("COA" + Common.LocalStoragePrefix);
            var forceReload = false;
            if (storageKey == "" || COA == null || COA == 'undefined')
                forceReload = true;
            if (forceReload) {
                $this.LoadCOA(callback, blockUI);
                return;
            }

            var data =
                {
                    key: 'CheckIfFiscalChanged',
                    FiscalId: Common.Fiscal.Id
                }
            return Common.WrapAjax({
                url: Setting.APIBaseUrl + "Misc",
                type: "GET",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: false,
                success: function (res) {
                    if (res.Success) {
                        $("#fiscal-year-change-dialog").modal("show");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        RefreshCOA: function (callback) {
            $.when(Common.LoadCOA()).done(function (res, a2, a3, a4) {
                if (res.Success && res.Data.LoadCOA) {
                    callback();
                }
            });
        },
        LoadCOA: function (callback, blockUI) {
            var $this = this;
            var data =
                {
                    key: 'LoadCOA',

                }
            Common.WrapAjax({
                url: Setting.APIBaseUrl + "COA",
                type: "GET",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                blockUI: blockUI,
                blockElement: "body",
                blockMessage: "Loading chart of accounts...please wait",
                success: function (res) {
                    if (res.Success) {

                        Common.SetData("COA" + Common.LocalStoragePrefix, res.Data.COA);
                        Common.SetData("LastDate" + Common.LocalStoragePrefix, res.Data.LastDate + "");
                        Common.SetData("StorageKey" + Common.LocalStoragePrefix, res.Data.StorageKey);
                        AppData["COA"] = Common.GetData("COA" + Common.LocalStoragePrefix);


                        if (typeof callback != "undefined")
                            callback();

                    }
                    else {
                        Common.ShowError(res.Error);
                    }
                },
                error: function (e) {
                }
            });
        },
        GetNextAccountCode: function (api_controller) {
            var $this = this;
            var accountId = 0;
            if ($("#ParentId").length > 0) {
                var accountId = Common.GetInt($("#ParentId").val());
                if (accountId <= 0)
                    return;
            }
            $.ajax({
                url: Setting.APIBaseUrl + api_controller + "/?key=GetNextAccountCode&AccountId=" + accountId,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#Code,#AccountCode").val(res.Data);
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        GetPartyAddress: function (accountid) {
            var $this = this;
            if (accountid <= 0)
                return;
            $.ajax({
                url: Setting.APIBaseUrl + "AccountDetail" + "/?PartyAddress=Key&&AccountId=" + accountid,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.Success) {
                        $("#PartyAddress").val(res.Data != null ? res.Data : "");
                    }
                    else {
                        Common.ShowError(res.Error);
                    }

                },
                error: function (e) {
                }
            });
        },
        ShowReloginWindow: function () {
            var $this = this;
            //var option
            //{
            //}
            // UIsessionTimeout.init(0);
            $("#ReloginPassword").val("");
            $("#session-timeout-dialog #modelerror").hide();
            $("#session-timeout-dialog").modal("show");

        },
        GetTransactionUrl: function (type, voucherno) {
            return "../Transaction/MapAction?type=" + type + "&voucherno=" + voucherno;

        },
        BindShortKeys: function (namespace) {

            $(document).on('keydown', null, 'alt+s', function () {

                namespace.Save();
                return false;
            });
            $(document).on('keydown', null, 'alt+n', function () {

                namespace.LoadVoucher("nextvouchernumber");
                return false;
            });
            $(document).on('keydown', null, 'alt+v', function () {

                $("#VoucherNumber").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+b', function () {

                $("#InvoiceNumber").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+d', function () {

                $("#Date").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+p', function () {

                $("#AccountCode").focus();
                return false;
            });
            $(document).on('keydown', null, 'alt+h', function () {

                $("#ShipViaName").focus();
                return false;
            });
            //$(document).on('keydown', null, 'alt+c', function () {

            //    $("#ShipViaCode").focus();
            //    return false;
            //});
            $(document).on('keydown', null, 'alt+i', function () {

                namespace.AddItem();
                return false;
            });
            $(document).on('keydown', null, 'alt+c', function () {

                $("input:radio[value='" + SaleType.cashsale + "']").prop("checked", true);
                $("input:radio[value='cash']").prop("checked", true).trigger("change");
                $.uniform.update();
            });
            $(document).on('keydown', null, 'alt+r', function () {

                $("input:radio[value='" + SaleType.creditsale + "']").prop("checked", true);
                $("input:radio[value='credit']").prop("checked", true).trigger("change");
                $.uniform.update();
            });
            $(document).on('keydown', null, 'alt+g', function () {

                $("input:radio[value='" + SaleType.sampling + "']").prop("checked", true);
                $.uniform.update();
            });
        },

        ApplyActions: function () {
            var $this = this;
            if (this.Access == null) {
                $("table.dataTable th:last-child,table.dataTable td:last-child").remove();
                return;
            }
            if (this.Access.CanUpdate == undefined || !this.Access.CanUpdate)
                $("table.dataTable span.action i.fa-edit").remove();
            if (this.Access.CanDelete == undefined || !this.Access.CanDelete)
                $("table.dataTable span.action i.fa-trash-o").remove();
            if ((this.Access.CanUpdate == undefined && this.Access.CanDelete == undefined && this.Access.CanView == undefined) || (!this.Access.CanUpdate && !this.Access.CanDelete && !this.Access.CanView))
                $("table.dataTable").not(".no-access-role").find("th:last-child:not('.no-action'),table.dataTable td:last-child").remove();
        },
        SetPageAccess: function (record, isView) {
            var $this = this;

            if ($("div.report-header").length)
                return;
            $(".form-actions button").removeClass("disabled");
            $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber,[data-permanent-disabled=\"true\"]'),select,[data-custum-disabled='true'],input.select2-input").prop("disabled", false);
            $(".form-actions div.btn-set.pull-right button.btn.btn-primary.green").removeClass("disabled").prop("disabled", false);
            $("#btn-add-item").removeClass("hide");
            if (this.Access == null) {
                $("table.dataTable th:last-child,table.dataTable td:last-child").remove();
                $(".form-actions").remove();

                return;
            }
            if ((this.Access.CanUpdate == undefined || !this.Access.CanUpdate) && (this.Access.CanView == undefined || !this.Access.CanView) && (this.Access.CanDelete == undefined || !this.Access.CanDelete)) {
                $(".form-actions div.btn-set.pull-left").remove();
            }
            if (this.Access.CanDelete == undefined || !this.Access.CanDelete) {
                $("#btndelete").remove();
                $("span.action").remove();
            }


            if (this.Access.CanCreate == undefined || !this.Access.CanCreate)
                $(".form-actions div.btn-set.pull-right button.btn.purple-plum").remove();
            if ((this.Access.CanUpdate == undefined || !this.Access.CanUpdate) && (this.Access.CanCreate == undefined || !this.Access.CanCreate)) {

                $("#btn-add-item").remove();
                $(".form-actions div.btn-set.pull-right button.btn.btn-primary.green").remove();
                $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber'),select,textarea").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);

            }



            if (typeof record != 'undefined' && record != 'undefined' && (record == null || record.Id == 0) && (this.Access.CanCreate == undefined || !this.Access.CanCreate)) {
                $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber'),select").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);
            }
            if (record != 'undefined' && (record != null && record.Id > 0) && (this.Access.CanUpdate == undefined || !this.Access.CanUpdate)) {
                $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber'),select,textarea").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);
                $(".form-actions div.btn-set.pull-right button.btn.btn-primary.green").addClass("disabled").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);
            }
            if ((record == 'undefined' || record == null || record.Id == 0 || this.Access.CanUpdate == undefined || !this.Access.CanUpdate) && (this.Access.CanCreate == undefined || !this.Access.CanCreate)) {
                $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber'),select").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);
                $(".form-actions div.btn-set.pull-right button.btn.btn-primary.green").addClass("disabled").prop("disabled", true);
                $(".form-actions #btn-save").remove();
            }
            $(".form-actions").find(".btn-primary, #btn-save").removeClass("hide");
            if (isView) {
                $("div[data-save='save']").find("input:not('#LoginPassword,#VoucherNumber'),select,textarea").not("[data-custum-disabled='false'],input.select2-input").prop("disabled", true);
                $(".form-actions").find(".btn-primary, #btn-save").addClass("hide");
                $("#btn-add-item").addClass("hide");


            }
            if (isView) {
                $("input[data-db-type]").each(function () {

                    var $element = $(this);
                    var dbtype = $element.attr("data-db-type");
                    var formating = $this.GetBool($(this).attr("data-formating"));
                    var value = $element.val();
                    if (dbtype == "int")
                        value = Common.GetInt(value);
                    else if (dbtype == "float" || dbtype == "decimal")
                        value = Common.GetFloat(value);
                    else if (dbtype == "date")
                        value = moment(value).format("DD/MM/YYYY")
                    else if (dbtype == "datetime")
                        value = moment(value).format("DD/MM/YYYY HH:mm")
                    if (formating)
                        value = value.format();
                    $element.val(value);

                });
            }

        },
        BindStickyTableHeaders: function (bindelement, newoption) {

            var offset = $('.navbar-nav').height();
            var options = {
                fixedOffset: offset
            };
            if (typeof newoption != "undefined")
                $.extend(true, options, newoption);
            if (typeof newoption != "undefined" || bindelement == null)
                bindelement = ".report-table";
            $(bindelement).stickyTableHeaders(options);

        },
        InitMultiSelectWithTable: function (container, table, newoption) {
            if (typeof container == "undefined" || container == null)
                container = "body"
            if (typeof table == "undefined" || table == null)
                table = "report-table"
            if (typeof table == "string")
                table = $("#" + table);
            if (typeof container == "string")
                container = $("#" + container);

            var $this = this;
            var html = "<select multiple='multiple' class='hide'>";
            $("thead th", table).each(function () {
                var head = $(this).text();
                var index = $(this).index();
                html += "<option value='" + index + "'>" + head + "</option>";
            });
            html += "</select>";
            $(container).html(html);
            $("select option", container).prop("selected", true);


            var options = {
                enableClickableOptGroups: true,
                enableCollapsibleOptGroups: true,
                // enableFiltering: true,
                includeSelectAllOption: true,
                disableIfEmpty: true,

                onChange: function (option, checked) {
                    //$("th,td", table).addClass("hide");
                    //$("select option:selected", container).each(function () {
                    //    var index = $(this).attr("value");
                    //    var childNo = Common.GetInt(index) + 1;
                    //    // $("th:eq(" + index + "),td:eq(" + index + ")", table).removeClass("hide");
                    //    $(table).find("tr th:nth-child(" + childNo + "),tr td:nth-child(" + childNo + ")").removeClass("hide");
                    //});
                },
                onSelectAll: function () {

                }
            };
            if (typeof newoption != "undefined")
                $.extend(true, options, newoption);

            $("select", container).multiselect(options);
            $("select", container).change(function () {
                $("th,td", table).addClass("hide");
                $("select option:selected", container).each(function () {
                    var index = $(this).attr("value");
                    var childNo = Common.GetInt(index) + 1;
                    // $("th:eq(" + index + "),td:eq(" + index + ")", table).removeClass("hide");
                    $(table).find("tr th:nth-child(" + childNo + "),tr td:nth-child(" + childNo + ")").removeClass("hide");
                });
            });
        },
    };
}();


//window.onerror = function (error, url, line) {
//    Common.ShowError(JSON.stringify({ acc: 'error', data: 'ERR:' + error + ' URL:' + url + ' L:' + line }));
//    $("body").removeClass("loading");
//};
var AJAX_OPTIONS = null;
$(document).ajaxStart(function () {

}).ajaxError(function (event, jqXhr, ajaxSettings, thrownError) {
    AJAX_OPTIONS = null;
    $("body").removeClass("loading");
    $(".page-loading").remove();
    if (jqXhr.status == 307) {
        AJAX_OPTIONS = ajaxSettings;
        Common.ShowReloginWindow();
    }
}).ajaxStop(function () {
    $("body").removeClass("loading");
    $(".page-loading").remove();

});
Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
};

