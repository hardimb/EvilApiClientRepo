var hub;
var fileProccessResult;
$(document).ready(function ()
{
     // file upload result hub
    hub = $.connection.fileUpladResultHub;

    // notify result
    hub.client.NotifyResult = function (fileName, response) {
        $(".loader").toggle(false);
        
        var customer = response.customer ? response.customer : "";
        var value = response.value ? response.value : "";
        var added = response.added ? response.added : false;
        var hash = response.hash ? response.hash : "";
        var errors = response.errors ? response.errors : [];
        var html = "";
        html += "<div class='row " + (added ? "successrow" : "errorrow") + "'>";
        html += "<div class='col-md-4'> <b> File : </b>" + fileName + "</div>";
        html += "<div class='col-md-4'> <b> Customer : </b>" + customer + "<b>  Value: </b>" + value + "</div>";
        if (errors.length > 0) {
            html += "<div class='col-md-4'> <b> Errors : </b>" + errors.join() + "</div>";            
        }
        html += "</div>";

      

        $("#fileProcessResponse").append(html);
    };


    jQuery.validator.addMethod("extension", function (value, element, param) {
        param = typeof param === "string" ? param.replace(/,/g, '|') : "png|jpe?g|gif";
        return this.optional(element) || value.match(new RegExp(".(" + param + ")$", "i"));
    }, jQuery.format("Please select file with a valid extension."));

    jQuery.validator.addMethod("fileSize", function (val, element) {

        var size = element.files[0].size;
        console.log(size);

        if (size > 1048576)// checks the file more than 1 MB
        {            
            return false;
        } else {
            return true;
        }

    }, "File size should not be greater than 1 MB");

    $('#uploadFile-Form').validate({
        rules: {
            UserName: {
                minlength: 2,
                required: true
            },
            UploadFile: {
                required: true,                
                extension: "csv",
                fileSize : true

            }
        },
        highlight: function (element) {                        
            $(element).parent().addClass("has-error").removeClass("has-success");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parent().addClass("has-success").removeClass("has-error");
        },
        errorElement: 'span',
        errorClass: 'help-block'        
    });

    $("#submitupload").click(function (e) {
        if (e.preventDefault)
            e.preventDefault();
        else
            e.returnValue = false;
        //var formData = new FormData($('#uploadFile-Form'));
      
        if ($('#uploadFile-Form').valid()) {
            $(".loader").toggle(true);
            $("#loaderMessage").html("Uploading File");
            $("#uploadSuccess").show().delay(5000).fadeOut();
          
            var file_data = $("#UploadFile").prop("files")[0];   // Getting the properties of file from file field
            var formData = new FormData();                  // Creating object of FormData class
            formData.append("UploadFile", file_data)              // Appending parameter named file with properties of file_field to form_data
            formData.append("UserName", $("#UserName").val())
            $("#UserName").val("");
            $("#UploadFile").val("");
            $(".form-group").removeClass("has-success").removeClass("has-error");            
            $.ajax({

                url: $('#uploadFile-Form').prop("action"),
                type: $('#uploadFile-Form').prop("method"),
                data: formData,
                processData: false,                
                contentType: false,
                error: function (xhr, textStatus, errorThrown) {
                    $(".loader").toggle(false);
                    $("#uploadFailed").show().delay(5000).fadeOut();
                },
                success: function (result) {

                    $(".loader").toggle(false);
                    fileProccessResult = result;
                    //alert(result);
                    $.connection.hub.start().done(function () {

                        // Establish a connection
                        hub.server.establishConnection($.connection.hub.id, $.connection.hub.id);
                        $(".loader").toggle(true);
                        $("#loaderMessage").html("Processing File");
                        // Start processing customer file
                        $.ajax({
                            url: "Home/StartProcessingFile",
                            method: "GET",
                            data: { fileName: fileProccessResult.FileName, username: fileProccessResult.UserName, originalFileName: fileProccessResult.OriginalFileName },
                            success: function (message) {
                                if (message == 2) {
                                    $("#fileprocessMessage").html("Process is started");
                                } else {
                                    $("#fileprocessMessage").html("Process is already started before, but you will still get the notification if in progress!");
                                }
                            }
                        });
                    });
                },
                cache: false
            });
        }
        return false;
    });   
});