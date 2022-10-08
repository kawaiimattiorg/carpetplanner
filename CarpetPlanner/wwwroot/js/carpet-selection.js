$(document).ready(function () {
    $('#new-carpet').click(function () {
        $.ajax({
            url: '/carpet',
            method: 'POST',
            contentType: 'application/json',
            success: function (carpet) {
                window.location = '/carpet/' + carpet.id
            }
        });
    });
});
