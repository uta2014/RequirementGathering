$(function() {
    $(document).on('click', '.image-wrapper #changeImage', function() {
        $('.image-wrapper').hide();
        $('.image-wrapper').next().removeClass('hidden');
        $('#fileRemoved').val('yes');
    });

    $(document).on('change', '#productVersionImage', function() {
        $("#upload-file-info").html($(this).val());
    });

    $(document).on('click', '.add.btn', function() {
        var parent = $(this).closest('.form-group');
        var count = parent.siblings().length;

        while (document.getElementById('Attributes_' + count + '__Name')) count++;

        var markup = '\
            <div class="form-group">\
                <label class="control-label col-md-2" for="Attributes_'+ count + '__Name"> </label>\
                <div class="col-md-3">\
                    <input class="form-control text-box single-line" id="Attributes_' + count + '__Name" name="Attributes[].Name" type="text" value="" />\
                    <span class="field-validation-valid text-danger" data-valmsg-for="Attributes[]" data-valmsg-replace="true"></span>\
                </div>\
                <div class="col-md-2">\
                    <button type="button" class="add btn btn-primary">\
                        <i class="glyphicon-plus glyphicon"></i>\
                    </button>\
                    <button type="button" class="remove btn btn-primary">\
                        <i class="glyphicon-minus glyphicon"></i>\
                    </button>\
                </div>\
            </div>';

        $(markup).insertAfter(parent);
    });

    $(document).on('click', '.remove.btn', function() {
        var parent = $(this).closest('.form-group');
        var count = parent.siblings().length;

        if (count > 0)
            parent.remove();
        else
            parent.fadeTo(500, 0.3, function() { parent.fadeTo(800, 1); });
    });

    $('.evaluation-form').submit(function() {
        $('[type="text"][name="Attributes[].Name"]').each(function(i) {
            $(this).attr('name', 'Attributes[' + i + '].Name');
            $(this).removeAttr('disabled');
        });
        $('[type="hidden"][name="Attributes[].Id"]').each(function(i) {
            $(this).attr('name', 'Attributes[' + i + '].Id');
        });
    });
});
