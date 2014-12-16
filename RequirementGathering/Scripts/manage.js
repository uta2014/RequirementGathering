$(function() {
    var formats = { 'en-us': 'mm/dd/yy', 'fi-fi': 'dd.mm.yy', 'nl-nl': 'dd-mm-yy' };

    $('#User_DateOfBirth').removeAttr("data-val-date").removeAttr('data-val-required').datepicker({
        dateFormat: formats[(typeof culture === 'undefined') ? 'en-us' : culture.toLowerCase()]
    });

    $('div.changePassword > a').on('click', function() {
        $(this).hide();
        $('fieldset.changePassword input[type=hidden]').val(true);
        $('fieldset.changePassword').slideDown();
        return false;
    });
});
