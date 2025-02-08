let carpetId;

const defaultHeaders = new Headers();
defaultHeaders.append("Content-Type", "application/json");

document.addEventListener("DOMContentLoaded", function () {
  carpetId = parseInt(document.getElementById("carpet-id").value);

  if (document.getElementById("edit-allowed").value === "True") {
    initializeCarpetNameChange();
    initializeCarpetWidthChange();
    initializeMoveStripes();
    initializeStripeHeightChange();

    initializeSelectStripe();
    initializePostStripe();

    initializePatchStripe();
    initializeDeleteStripes();
  }

  window.addEventListener("resize", updateStripeSizes);
  updateStripeSizes();
});

const toggleSelected = (event) => {
  event.target.parentElement.classList.toggle("active");
};

function initializeCarpetNameChange() {
  const changeCarpetName = () => {
    const name = prompt("Anna uusi nimi");

    if (!name) {
      return;
    }

    fetch(`/carpet/${carpetId}`, {
      method: "PATCH",
      headers: defaultHeaders,
      body: JSON.stringify({ name }),
    }).then((_) => (document.getElementById("name").textContent = name));
  };
  document
    .getElementById("change-name")
    .addEventListener("click", changeCarpetName);
}

function initializeCarpetWidthChange() {
  const changeWidth = () => {
    const width = parseInt(prompt("Anna uusi leveys senttimetreinä"));

    if (isNaN(width)) {
      return;
    }

    fetch(`/carpet/${carpetId}`, {
      method: "PATCH",
      headers: defaultHeaders,
      body: JSON.stringify({ width }),
    }).then((_) => {
      document.getElementById("width-value").textContent = width;
      updateStripeSizes();
    });
  };

  document
    .getElementById("change-width")
    .addEventListener("click", changeWidth);
}

function initializeMoveStripes() {
  const moveStripes = (moveDirection) => {
    const stripes = getSelectedStripes();
    if (stripes.length === 0) {
      return;
    }

    const data = {
      stripes,
      moveDirection,
    };
    performStripePatch(data);
  };

  document
    .getElementById("move-up")
    .addEventListener("click", () => moveStripes(-1));
  document
    .getElementById("move-down")
    .addEventListener("click", () => moveStripes(1));
}

function initializeStripeHeightChange() {
  const changeHeight = () => {
    const stripes = getSelectedStripes();
    if (stripes.length === 0) {
      return;
    }

    const height = parseInt(prompt("Anna uusi pituus senttimetreinä"));

    if (isNaN(height)) {
      return;
    }

    const data = {
      stripes: stripes,
      height: height,
    };

    performStripePatch(data);
  };

  document
    .getElementById("change-height")
    .addEventListener("click", changeHeight);
}

function getSelectedStripes() {
  return Array.from(document.querySelectorAll("#carpet > .active")).map(
    (element) => element.dataset.stripeId
  );
}

function initializeDeleteStripes() {
  const deleteStripes = () => {
    let stripes = getSelectedStripes();

    if (stripes.length === 0) {
      return;
    }

    let data = {
      stripes: stripes,
      remove: true,
    };

    performStripePatch(data);
  };

  document
    .getElementById("delete-stripes")
    .addEventListener("click", deleteStripes);
}

function initializePatchStripe() {
  const triggerColorChange = (event) => {
    const stripes = getSelectedStripes();

    if (stripes.length === 0) {
      return;
    }

    const data = {
      stripes: stripes,
      color: event.target.dataset.colorId,
    };

    performStripePatch(data);
  };

  document
    .getElementById("edit")
    .querySelectorAll("div[data-color-id]")
    .forEach((element) => {
      element.addEventListener("click", triggerColorChange);
    });
}

function initializeSelectStripe() {
  document.querySelectorAll("#carpet > div").forEach((stripe) => {
    stripe.addEventListener("click", toggleSelected);
  });
}

function initializePostStripe() {
  const handleClickPostNewStripe = () => {
    fetch(`/carpet/${carpetId}`, {
      method: "POST",
    })
      .then((result) => result.json())
      .then((stripe) => addNewStripe(stripe));
  };

  const addNewStripe = (stripe) => {
    const container = document.createElement("div");
    container.dataset.stripeId = stripe.id;
    container.dataset.stripeHeight = stripe.height;
    container.addEventListener("click", toggleSelected);

    const stripeSelection = document.createElement("div");
    stripeSelection.classList.add("stripe-selection");
    stripeSelection.append(document.createElement("span"));
    container.append(stripeSelection);

    const stripeElement = document.createElement("div");
    stripeElement.classList.add("stripe-element");
    stripeElement.style.backgroundColor = stripe.colorString;
    container.append(stripeElement);

    const stripeHeight = document.createElement("div");
    stripeHeight.classList.add("stripe-height");
    stripeHeight.textContent = stripe.height;
    container.append(stripeHeight);

    document.getElementById("carpet").append(container);

    updateStripeSizes();
    updateStripeHeight();
  };

  document
    .getElementById("new-stripe")
    .addEventListener("click", handleClickPostNewStripe);
}

function performStripePatch(data) {
  const applyStripeUpdates = (response) => {
    const carpet = document.getElementById("carpet");
    response.stripes.forEach((stripeId) => {
      const stripe = carpet.querySelector(`div[data-stripe-id="${stripeId}"]`);
      if (response.height !== null) {
        stripe.dataset.stripeHeight = response.height;
        stripe.querySelector(".stripe-height").textContent = response.height;
      }

      if (response.rgb !== null) {
        stripe.querySelector(
          ".stripe-element"
        ).style.backgroundColor = `#${response.rgb}`;
      }

      if (response.remove === true) {
        stripe.remove();
      }
    });

    if (response.moveDirection && response.moved) {
      updateStripeOrder(response.moved, response.moveDirection);
    }

    if (response.height !== null || response.remove === true) {
      updateStripeSizes();
      updateStripeHeight();
    }
  };

  fetch(`/stripe/${carpetId}`, {
    method: "PATCH",
    headers: defaultHeaders,
    body: JSON.stringify(data),
  })
    .then((result) => result.json())
    .then((update) => applyStripeUpdates(update));
}

function updateStripeOrder(moved, direction) {
  const carpet = document.getElementById("carpet");
  moved.forEach((id) => {
    let stripe = carpet.querySelector(`div[data-stripe-id="${id}"]`);

    // move up
    if (direction === -1 && stripe.previousElementSibling) {
      stripe.parentNode.insertBefore(stripe, stripe.previousElementSibling);
    }

    // move down
    if (direction === 1 && stripe.nextElementSibling) {
      stripe.parentNode.insertBefore(stripe.nextElementSibling, stripe);
    }
  });
}

function updateStripeHeight() {
  let carpetHeight = 0;

  document
    .getElementById("carpet")
    .querySelectorAll("div[data-stripe-id]")
    .forEach((stripe) => {
      carpetHeight += parseInt(stripe.dataset.stripeHeight);
    });

  document.getElementById("height-value").textContent = carpetHeight;
}

function updateStripeSizes() {
  const carpet = document.getElementById("carpet");

  let stripes = carpet.querySelectorAll("div[data-stripe-id]");

  if (stripes.length === 0) {
    return;
  }

  let selectionWidth =
    stripes[0].querySelector(".stripe-selection").clientWidth;
  let widthWidth = stripes[0].querySelector(".stripe-height").clientWidth;

  let uiHeightPx = carpet.clientHeight;
  let uiWidthPx = carpet.clientWidth - selectionWidth - widthWidth;
  let uiRatio = uiWidthPx / uiHeightPx;

  let carpetHeight = 0;
  stripes.forEach(
    (stripe) => (carpetHeight += parseInt(stripe.dataset.stripeHeight))
  );

  let carpetWidth = parseInt(
    document.getElementById("width-value").textContent
  );

  let uiHeightCm = carpetWidth / uiRatio;

  let tooHigh = carpetHeight > uiHeightCm;

  if (tooHigh) {
    let cmToPixel = uiHeightPx / carpetHeight;
    let widthPx = carpetWidth * cmToPixel;

    stripes.forEach((stripe) => {
      stripe.querySelector(".stripe-element").style.width = `${widthPx}px`;
      stripe.style.height = `${
        parseInt(stripe.dataset.stripeHeight) * cmToPixel
      }px`;
    });
  } else {
    let cmToPixel = uiWidthPx / carpetWidth;

    stripes.forEach((stripe) => {
      stripe.querySelector(".stripe-element").style.width = "calc(100% - 80px)";
      stripe.style.height = `${
        parseInt(stripe.dataset.stripeHeight) * cmToPixel
      }px`;
    });
  }
}
