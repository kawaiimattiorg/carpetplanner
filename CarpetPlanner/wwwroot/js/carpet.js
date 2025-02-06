let carpetId;

document.addEventListener("DOMContentLoaded", function() {
    carpetId = parseInt($('#carpet-id').val());

    if ($('#edit-allowed').val() === 'True')
    {
        initializeCarpetNameChange();
        initializeCarpetWidthChange();
        initializeMoveStripes();
        initializeStripeHeightChange();

        initializeSelectStripe();
        initializePostStripe();

        initializePatchStripe();
        initializeDeleteStripes();
    }

    window.addEventListener('resize', updateStripeSizes);
    updateStripeSizes();
});

function initializeCarpetNameChange() {
    $('#change-name').click(function () {
        let name = prompt('Anna uusi nimi');

        if (name === null || name.length === 0) {
            return;
        }

        $.ajax({
            url: '/carpet/' + carpetId,
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
            url: '/carpet/' + carpetId,
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
    const handleClickPostNewStripe = () => {
        fetch(`/carpet/${carpetId}`, {
            method: 'POST',
        })
            .then(result => result.json())
            .then(stripe => addNewStripe(stripe));
    };

    const addNewStripe = (stripe) => {
        const container = document.createElement('div');
        container.setAttribute('data-stripe-id', stripe.id);
        container.setAttribute('data-stripe-height', stripe.height);

        const stripeSelection = document.createElement('div');
        stripeSelection.classList.add('stripe-selection');
        stripeSelection.append(document.createElement('span'));
        container.append(stripeSelection);


        const stripeElement = document.createElement('div');
        stripeElement.classList.add('stripe-element');
        stripeElement.style.backgroundColor = stripe.colorString;
        container.append(stripeElement);

        const stripeHeight = document.createElement('div');
        stripeHeight.classList.add('stripe-height');
        stripeHeight.textContent = stripe.height;
        container.append(stripeHeight);

        document.getElementById('carpet').append(container);

        updateStripeSizes();
        updateStripeHeight();
    };

    document.getElementById('new-stripe').addEventListener('click', handleClickPostNewStripe);
}

function performStripePatch(data) {
    const applyStripeUpdates = (response) => {
        const carpet = document.getElementById('carpet');
        response.stripes.forEach(stripeId => {
            const stripe = carpet.querySelector(`div[data-stripe-id="${stripeId}"]`);
            if (response.height !== null){
                stripe.setAttribute('data-stripe-height', response.height);
                stripe.querySelector('.stripe-height').textContent = response.height;
            }

            if (response.rgb !== null) {
                stripe.querySelector('.stripe-element').style.backgroundColor = `#${response.rgb}`;
            }

            if (response.remove === true) {
                stripe.remove();
            }
        });

        if (response.moveDirection) {
            updateStripeOrder(response.moved, response.moveDirection);
        }

        if (response.height !== null || response.remove === true) {
            updateStripeSizes();
            updateStripeHeight();
        }
    };

    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    fetch(`/stripe/${carpetId}`, {
        method: 'PATCH',
        headers,
        body: JSON.stringify(data)
    }).then(result => result.json())
        .then(update => applyStripeUpdates(update));

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
    const carpet = document.getElementById('carpet');

    let stripes = carpet.querySelectorAll('div[data-stripe-id]');

    if (stripes.length === 0) {
        return;
    }

    let selectionWidth = stripes[0].querySelector('.stripe-selection').clientWidth;
    let widthWidth = stripes[0].querySelector('.stripe-height').clientWidth;

    let uiHeightPx = carpet.clientHeight;
    let uiWidthPx = carpet.clientWidth - selectionWidth - widthWidth;
    let uiRatio = uiWidthPx / uiHeightPx;

    let carpetHeight = 0;
    stripes.forEach( (stripe)=> carpetHeight += parseInt(stripe.getAttribute('data-stripe-height')));

    let carpetWidth = parseInt(document.getElementById('width-value').textContent);

    let uiHeightCm = carpetWidth / uiRatio;

    let tooHigh = carpetHeight > uiHeightCm;

    if (tooHigh) {
        let cmToPixel = uiHeightPx / carpetHeight;
        let widthPx = carpetWidth * cmToPixel;

        stripes.forEach((stripe)=> {
            stripe.querySelector('.stripe-element').style.width = `${widthPx}px`;
            stripe.style.height = `${parseInt(stripe.getAttribute('data-stripe-height')) * cmToPixel}px`;
        });
    } else {
        let cmToPixel = uiWidthPx / carpetWidth;

        stripes.forEach((stripe) => {
            stripe.querySelector('.stripe-element').style.width = 'calc(100% - 80px)';
            stripe.style.height  = `${parseInt(stripe.getAttribute('data-stripe-height')) * cmToPixel}px`
        });
    }
}
