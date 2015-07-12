

function toggleVisibility(elementId) {

    var element = document.getElementById(elementId);

    if (elementId != null) {

        if (element.style.display == "block") {
            element.style.display = "none";
        } else {
            element.style.display = "block";
        }
    }
}