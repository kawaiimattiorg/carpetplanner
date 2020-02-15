$(document).ready(function () {
    $('#confirm').click(function () {
        let username = $('#username').val();

        if (username.length === 0) {
            return;
            // TODO: OPEN ERROR NOTIFICATION
        }

        $.ajax({
            url: 'carpet/' + username,
            method: 'POST',
            contentType: 'application/json',
            success: function (carpet) {
                window.location = 'user/' + carpet.username + '/' + carpet.id
            }            
        });
    });
});