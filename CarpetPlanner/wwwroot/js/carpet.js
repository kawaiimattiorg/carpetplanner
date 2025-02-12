const carpetId = parseInt(document.getElementById("carpet-id").value);
const carpetElement = document.getElementById("carpet");

const defaultHeaders = new Headers();
defaultHeaders.append("Content-Type", "application/json");

// Utility methods
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

  carpetElement.append(container);

  updateStripeSizes();
  updateStripeHeight();
};

const applyStripeUpdates = (response) => {
  response.stripes.forEach((stripeId) => {
    const stripe = carpetElement.querySelector(
      `div[data-stripe-id="${stripeId}"]`
    );
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

const getSelectedStripes = () => {
  return Array.from(document.querySelectorAll("#carpet > .active")).map(
    (element) => element.dataset.stripeId
  );
};

const performStripePatch = (data) => {
  fetch(`/stripe/${carpetId}`, {
    method: "PATCH",
    headers: defaultHeaders,
    body: JSON.stringify(data),
  })
    .then((result) => result.json())
    .then((update) => applyStripeUpdates(update));
};

function updateStripeHeight() {
  let carpetHeight = 0;

  carpetElement.querySelectorAll("div[data-stripe-id]").forEach((stripe) => {
    carpetHeight += parseInt(stripe.dataset.stripeHeight);
  });

  document.getElementById("height-value").textContent = carpetHeight;
}

const updateStripeOrder = (moved, direction) => {
  moved.forEach((id) => {
    let stripe = carpetElement.querySelector(`div[data-stripe-id="${id}"]`);

    // move up
    if (direction === -1 && stripe.previousElementSibling) {
      stripe.parentNode.insertBefore(stripe, stripe.previousElementSibling);
    }

    // move down
    if (direction === 1 && stripe.nextElementSibling) {
      stripe.parentNode.insertBefore(stripe.nextElementSibling, stripe);
    }
  });
};

const updateStripeSizes = () => {
  const stripes = carpetElement.querySelectorAll("div[data-stripe-id]");

  if (stripes.length === 0) {
    return;
  }

  const selectionWidth =
    stripes[0].querySelector(".stripe-selection").clientWidth;
  const widthWidth = stripes[0].querySelector(".stripe-height").clientWidth;

  const uiHeightPx = carpetElement.clientHeight;
  const uiWidthPx = carpetElement.clientWidth - selectionWidth - widthWidth;
  const uiRatio = uiWidthPx / uiHeightPx;

  let carpetHeight = 0;
  stripes.forEach(
    (stripe) => (carpetHeight += parseInt(stripe.dataset.stripeHeight))
  );

  const carpetWidth = parseInt(
    document.getElementById("width-value").textContent
  );

  const uiHeightCm = carpetWidth / uiRatio;
  const tooHigh = carpetHeight > uiHeightCm;

  if (tooHigh) {
    const cmToPixel = uiHeightPx / carpetHeight;
    const widthPx = carpetWidth * cmToPixel;

    stripes.forEach((stripe) => {
      stripe.querySelector(".stripe-element").style.width = `${widthPx}px`;
      stripe.style.height = `${
        parseInt(stripe.dataset.stripeHeight) * cmToPixel
      }px`;
    });
  } else {
    const cmToPixel = uiWidthPx / carpetWidth;

    stripes.forEach((stripe) => {
      stripe.querySelector(".stripe-element").style.width = "calc(100% - 80px)";
      stripe.style.height = `${
        parseInt(stripe.dataset.stripeHeight) * cmToPixel
      }px`;
    });
  }
};

// Handler methods
const handleClickPostNewStripe = () => {
  fetch(`/carpet/${carpetId}`, {
    method: "POST",
  })
    .then((result) => result.json())
    .then((stripe) => addNewStripe(stripe));
};

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

const toggleSelected = (event) => {
  event.target.parentElement.classList.toggle("active");
};

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

updateStripeSizes();

// Register common handlers
window.addEventListener("resize", updateStripeSizes);

// Register handlers for carpets that allow edits
if (document.getElementById("edit-allowed").value === "True") {
  document
    .getElementById("change-name")
    .addEventListener("click", changeCarpetName);

  document
    .getElementById("change-width")
    .addEventListener("click", changeWidth);

  document
    .getElementById("move-up")
    .addEventListener("click", () => moveStripes(-1));

  document
    .getElementById("move-down")
    .addEventListener("click", () => moveStripes(1));

  document
    .getElementById("change-height")
    .addEventListener("click", changeHeight);

  document
    .getElementById("delete-stripes")
    .addEventListener("click", deleteStripes);

  document
    .getElementById("edit")
    .querySelectorAll("div[data-color-id]")
    .forEach((element) => {
      element.addEventListener("click", triggerColorChange);
    });

  document.querySelectorAll("#carpet > div").forEach((stripe) => {
    stripe.addEventListener("click", toggleSelected);
  });

  document
    .getElementById("new-stripe")
    .addEventListener("click", handleClickPostNewStripe);
}
