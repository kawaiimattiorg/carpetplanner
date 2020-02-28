let carpetId;
let username;

$(document).ready(function () {
    carpetId = parseInt($('#carpet-id').val());
    username = $('#username').val();

    initializeCarpetNameChange();
    initializeCarpetWidthChange();

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
    let $name = $('#name');
    let $newName = $('#new-name');

    $name.click(function () {
        $newName.val($name.text());

        $newName.one('focusout', function () {
            let newName = $newName.val();
            if (newName.length === 0) {
                $newName.hide();
                $name.show();
                return;
            }

            $.ajax({
                url: '/carpet/' + username + '/' + carpetId,
                method: 'PATCH',
                contentType: 'application/json',
                data: JSON.stringify({
                    name: newName
                }),
                success: function () {
                    $name.text(newName);
                },
                complete: function () {
                    $newName.hide();
                    $name.show();
                }
            })
        });

        $name.hide();
        $newName.show();
        $newName.focus();
    });
}

function initializeCarpetWidthChange() {
    let $widthValue = $('#width-value');
    let $widthInput = $('#width-input');

    $widthValue.click(function () {
        $widthInput.val(parseInt($widthValue.text()));

        $widthInput.one('focusout', function () {
            let newWidth = $widthInput.val();
            if (newWidth <= 0) {
                $widthInput.hide();
                $widthValue.show();
                return;
            }

            $.ajax({
                url: '/carpet/' + username + '/' + carpetId,
                method: 'PATCH',
                contentType: 'application/json',
                data: JSON.stringify({
                    width: newWidth
                }),
                success: function () {
                    $widthValue.text(newWidth);
                    updateStripeSizes();
                },
                complete: function () {
                    $widthInput.hide();
                    $widthValue.show();
                }
            })
        });

        $widthInput.width($('#width').width() - $('#width-text').width() - 10);
        $widthInput.height($widthValue.height());

        $widthValue.hide();
        $widthInput.show();
        $widthInput.focus();
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
                            'class': 'stripe-width'
                        })]
                }));

                updateStripeSizes();
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
                }

                if (response.rgb !== null) {
                    $stripe.children('.stripe-element').css('backgroundColor', '#' + response.rgb);
                }

                if (response.remove === true) {
                    $stripe.remove();
                }
            });

            if (response.height !== null || response.remove === true) {
                updateStripeSizes();
            }
        }
    });
}

function updateStripeSizes() {
    let $carpet = $('#carpet');

    let $stripes = $carpet.children('div[data-stripe-id]');

    if ($stripes.length === 0) {
        return;
    }

    let selectionWidth = $stripes.first().children('.stripe-selection').width();
    let widthWidth = $stripes.first().children('.stripe-width').width();

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
            $this.children('.stripe-element').css('width', 'calc(100% - 64px)');
            $this.css('height', parseInt($this.data('stripeHeight')) * cmToPixel + 'px');
        });
    }

    console.log('ui ratio:' + uiRatio);
    console.log('carpet ratio:' + carpetRatio);
    console.log('ui height cm:' + uiHeightCm);
}