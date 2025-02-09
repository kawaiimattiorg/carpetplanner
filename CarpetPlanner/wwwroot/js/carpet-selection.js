const postNewCarpet = () => {
  fetch("/carpet", {
    method: "POST",
  })
    .then((result) => result.json())
    .then((carpet) => {
      window.location = "/carpet/" + carpet.id;
    });
};

document.getElementById("new-carpet").addEventListener("click", postNewCarpet);
