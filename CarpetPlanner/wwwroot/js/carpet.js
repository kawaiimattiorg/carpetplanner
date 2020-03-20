let carpetId;
let username;

$(document).ready(function () {
    carpetId = parseInt($('#carpet-id').val());
    username = $('#username').val();

    initializeCarpetNameChange();
    initializeCarpetWidthChange();
    initializeMoveStripes();
    initializeStripeHeightChange();

    initializeSelectStripe();
    initializePostStripe();

    initializePatchStripe();
    initializeDeleteStripes();

    $(window).on('resize', function () {
        updateStripeSizes();
    });


    $('#print').click(function () {
        // TODO: CALL CONTROLLER OR OPEN ANOTHER PAGE THAT PRINTS??

        alert('print');
    });

    updateStripeSizes();
});

function initializeCarpetNameChange() {
    $('#change-name').click(function () {
        let name = prompt('Anna uusi nimi');

        if (name === null || name.length === 0) {
            return;
        }

        $.ajax({
            url: '/carpet/' + username + '/' + carpetId,
            method: 'PATCH',
            contentType: 'application/json',
            data: JSON.stringify({
                name: name
            }),
            success: function () {
                $('#name').text(name);
            }
        });
    });
}

function initializeCarpetWidthChange() {

    $('#change-width').click(function () {
        let width = parseInt(prompt('Anna uusi leveys senttimetreinä'));

        if (isNaN(width)) {
            return;
        }

        $.ajax({
            url: '/carpet/' + username + '/' + carpetId,
            method: 'PATCH',
            contentType: 'application/json',
            data: JSON.stringify({
                width: width
            }),
            success: function () {
                $('#width-value').text(width);
                updateStripeSizes();
            }
        });
    });
}

function initializeMoveStripes() {
    $('#move-up, #move-down').click(function () {
        let stripes = getSelectedStripes();

        if (stripes.length === 0) {
            return;
        }

        let data = {
            stripes: stripes,
            moveDirection: parseInt($(this).data('moveDirection'))
        };

        performStripePatch(data);
    });
}

function initializeStripeHeightChange() {
    $('#change-height').click(function () {
        let stripes = getSelectedStripes();

        if (stripes.length === 0) {
            return;
        }

        var height = parseInt(prompt('Anna uusi pituus senttimetreinä'));

        if (isNaN(height)) {
            return;
        }

        let data = {
            stripes: stripes,
            height: height
        };

        performStripePatch(data);
    });
}

function getSelectedStripes() {
    return $('#carpet')
        .children('.active')
        .map(function () {
            return $(this).data('stripeId');
        })
        .get();
}

function initializeDeleteStripes() {

    $('#delete-stripes').click(function () {

        let stripes = getSelectedStripes();

        if (stripes.length === 0) {
            return;
        }

        let data = {
            stripes: stripes,
            remove: true
        };

        performStripePatch(data);
    });

}

function initializePatchStripe() {
    $('#edit').on('click', 'div[data-color-id]', function () {
        let stripes = getSelectedStripes();

        if (stripes.length === 0) {
            return;
        }

        let data = {
            stripes: stripes,
            color: $(this).data('colorId')
        };

        performStripePatch(data);
    });
}

function initializeSelectStripe() {
    $('#carpet').on('click', 'div', function () {
        $(this).toggleClass('active');
    });
}

function initializePostStripe() {

    $('#new-stripe').click(function () {
        $.ajax({
            url: '/carpet/' + username + '/' + carpetId,
            method: 'POST',
            success: function (data) {
                $('#carpet').append($('<div>', {
                    attr: {
                        'data-stripe-id': data.id,
                        'data-stripe-height': data.height
                    },
                    html: [
                        $('<div>', {
                            'class': 'stripe-selection',
                            html: $('<p>')
                        }),
                        $('<div>', {
                            'class': 'stripe-element',
                            css: {
                                "backgroundColor": data.colorString
                            }
                        }),
                        $('<div>', {
                            'class': 'stripe-height',
                            html: $('<p>', {
                                text: data.height
                            })
                        })]
                }));

                updateStripeSizes();
                updateStripeHeight();
            }
        });
    });
}

function performStripePatch(data) {
    $.ajax({
        url: '/stripe/' + username + '/' + carpetId,
        method: 'PATCH',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            let $carpet = $('#carpet');

            response.stripes.forEach(function (stripeId) {
                let $stripe = $carpet.children('div[data-stripe-id="' + stripeId + '"]');

                if (response.height !== null) {
                    $stripe.data('stripeHeight', response.height);
                    $stripe.children('.stripe-height').children('p').text(response.height);
                }

                if (response.rgb !== null) {
                    $stripe.children('.stripe-element').css('backgroundColor', '#' + response.rgb);
                }

                if (response.remove === true) {
                    $stripe.remove();
                }
            });

            if (response.moveDirection !== undefined && response.moveDirection !== null) {
                updateStripeOrder(response.moved, response.moveDirection);
            }

            if (response.height !== null || response.remove === true) {
                updateStripeSizes();
                updateStripeHeight();
            }
        }
    });
}

function updateStripeOrder(moved, direction) {
    $.each(moved, function (index, id) {
        let stripe = $('#carpet')
            .children('div[data-stripe-id=' + id + ']')
            .first();

        // move up
        if (direction === -1) {
            stripe.insertBefore(stripe.prev());
        }

        // move down
        if (direction === 1) {
            stripe.insertAfter(stripe.next());
        }
    });
}

function updateStripeHeight() {
    let carpetHeight = 0;

    $('#carpet')
        .children('div[data-stripe-id]')
        .each(function () {
            carpetHeight += parseInt($(this).data('stripeHeight'));
        });

    $('#height-value').text(carpetHeight);
}

function updateStripeSizes() {
    let $carpet = $('#carpet');

    let $stripes = $carpet.children('div[data-stripe-id]');

    if ($stripes.length === 0) {
        return;
    }

    let selectionWidth = $stripes.first().children('.stripe-selection').width();
    let widthWidth = $stripes.first().children('.stripe-height').width();

    let uiHeightPx = $carpet.height();
    let uiWidthPx = $carpet.width() - selectionWidth - widthWidth;
    let uiRatio = uiWidthPx / uiHeightPx;

    let carpetHeight = 0;
    $stripes.each(function () {
        carpetHeight += parseInt($(this).data('stripeHeight'));
    });

    let carpetWidth = parseInt($('#width-value').text());
    let carpetRatio = carpetWidth / carpetHeight;

    let uiHeightCm = carpetWidth / uiRatio;

    let tooHigh = carpetHeight > uiHeightCm;

    if (tooHigh) {
        let cmToPixel = uiHeightPx / carpetHeight;
        let widthPx = carpetWidth * cmToPixel;

        $stripes.each(function () {
            let $this = $(this);
            $this.children('.stripe-element').css('width', widthPx + 'px');
            $this.css('height', parseInt($this.data('stripeHeight')) * cmToPixel + 'px');
        });
    } else {
        let cmToPixel = uiWidthPx / carpetWidth;

        $stripes.each(function () {
            let $this = $(this);
            $this.children('.stripe-element').css('width', 'calc(100% - 80px)');
            $this.css('height', parseInt($this.data('stripeHeight')) * cmToPixel + 'px');
        });
    }

    console.log('ui ratio:' + uiRatio);
    console.log('carpet ratio:' + carpetRatio);
    console.log('ui height cm:' + uiHeightCm);
}
