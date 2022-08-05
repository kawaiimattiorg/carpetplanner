$(document).ready(function () {
    $('#new-carpet').click(function () {
        let username = $('#username').val();
        
        $.ajax({
            url: '/carpet/' + username,
            method: 'POST',
            contentType: 'application/json',
            success: function (carpet) {
                window.location = '/user/' + carpet.username + '/' + carpet.id
            }
        });
    });
});