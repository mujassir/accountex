var FormFileUpload = function () {
    return {
        //main function to initiate the module
        init: function () {

            // Initialize the jQuery File Upload widget:
            $('#fileupload').fileupload({
                disableImageResize: false,
                autoUpload: true,
                replaceFileInput: false,
                disableImageResize: /Android(?!.*Chrome)|Opera/.test(window.navigator.userAgent),
                maxFileSize: 5000000,
                //acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
                // Uncomment the following to send cross-domain cookies:
                //xhrFields: {withCredentials: true},  
                //done: function () {
                //    //alert('kr');
                //    //App.initUniform();
                //    //$('.upl').remove();
                //    //$.each(data.files, function (index, file) {
                //    //    /**/
                //    //});
                //},
                //fail: function (e, data) {
                //    alert('Fail!');
                //}
            });
            $('#fileupload').bind('fileuploaddone', function (e, data) {

                setTimeout(function () { App.initUniform(); }, 300);
            });
            //$('#fileupload').bind('fileuploadstop', function (e, data) {
            //    App.initUniform();
            //});

            // Enable iframe cross-domain access via redirect option:
            $('#fileupload').fileupload(
                'option',
                'redirect',
                window.location.href.replace(
                    /\/[^\/]*$/,
                    '/cors/result.html?%s'
                )
            );

            //// Upload server status check for browsers with CORS support:
            //if ($.support.cors) {
            //    $.ajax({
            //        type: 'HEAD'
            //    }).fail(function () {
            //        $('<div class="alert alert-danger"/>')
            //            .text('Upload server currently unavailable - ' +
            //                    new Date())
            //            .appendTo('#fileupload');
            //    });
            //}

            // Load & display existing files:
            //$('#fileupload').addClass('fileupload-processing');
            //$.ajax({
            //    // Uncomment the following to send cross-domain cookies:
            //    //xhrFields: {withCredentials: true},
            //    url: $('#fileupload').attr("action"),
            //    dataType: 'json',
            //    context: $('#fileupload')[0]
            //}).always(function () {
            //    $(this).removeClass('fileupload-processing');
            //}).done(function (result) {
            //    $(this).fileupload('option', 'done')
            //    .call(this, $.Event('done'), {result: result});
            //});
        }

    };

}();

jQuery(document).ready(function () {
    FormFileUpload.init();
});