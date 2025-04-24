function readImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#uploadedImage').attr('src', e.target.result);
            $('#uploadedImage').css('display', 'block');
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function copySearchString() {
    document.getElementById("searchStringForm2").value = document.getElementById("searchStringForm1").value;
}

function changeToDay() {
    document.getElementById("startTime").type = "date";
    document.getElementById("endTime").type = "date";
}
function changeToMonth() {
    document.getElementById("startTime").type = "month";
    document.getElementById("endTime").type = "month";
}
function changeToYear() {
    document.getElementById("startTime").type = "number";
    document.getElementById("endTime").type = "number";
}