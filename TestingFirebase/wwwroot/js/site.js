// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', '#delMainCategory', function (e) {

    e.preventDefault();

    //the below .data('whatever') are made up variables that go in the HTML tags
    //these allow to add any kind of info needed for the website they need
    //with Razor syntax variables and stuff
    const id = $(this).data('id');
    const numCat = $(this).data('num');
    const titleCat = $(this).data('title');
    var singularOrPlural;//subcategory or subcategories

    //alert("ID to delete: " + id + "\n" + "# of Subcategories: " + numCat);
    var pModalBody = $('#delP')[0].innerHTML
    //clear p html element in form body for any previous message
    $('#delP')[0].innerHTML = "";

    //below is for MainCategories since they have subcategories
    //if (numCat > 0) {
    //    singularOrPlural = numCat > 1 ? "subcategories" : "subcategory";

    //    $('#delP')[0].innerHTML =
    //        "<strong>" + titleCat + "</strong>" + " currently has " + "<strong>" + numCat + "</strong>" + " " +
    //        singularOrPlural + " .\n ALL the subcategories will be deleted as well!";
    //}
    //else {
    //    $('#delP')[0].innerHTML = "Are you sure you want to delete " + "<strong>" + titleCat + "</strong>" + " Category?" + "Hit delete to confirm";
    //}

    $('#delP')[0].innerHTML = "Are you sure you want to delete " + "<strong>" + titleCat + "</strong>" + " Category?" + "Hit delete to confirm";

    //load action to get form ready in case user proceeds with deletion
    //action is an HTML attibute that acts as a combination of asp-controller and asp-action
  /*  $('#deleteFormClient').attr('action', 'Sucategories/Delete/' + id);*/
})




$(document).on('click', '#delSubCategory', function (e) {

   // e.preventDefault();

    //the below .data('whatever') are made up variables that go in the HTML tags
    //these allow to add any kind of info needed for the website they need
    //with Razor syntax variables and stuff
    const id = $(this).data('id');
    const numCat = $(this).data('num');
    const titleCat = $(this).data('title');
    var singularOrPlural;//subcategory or subcategories

    //alert("ID to delete: " + id + "\n" + "# of Subcategories: " + numCat);
    var pModalBody = $('#delP')[0].innerHTML
    //clear p html element in form body for any previous message
    $('#delP')[0].innerHTML = "";


    $('#delP')[0].innerHTML = "Are you sure you want to delete " + "<strong>" + titleCat + "</strong>" + " Subcategory?" + " Hit delete to confirm";

    //load action to get form ready in case user proceeds with deletion
    //action is an HTML attibute that acts as a combination of asp-controller and asp-action
    //for below action. At this point, url would be at: https://localhost:5001/Subcategories/Index/c2
    //so the ../ below would update the url to point to https://localhost:5001/Subcategories/
    //then, since it's already in the right controller, only the IAction and ID need to be specified
      $('#deleteFormClient').attr('action', '../Delete/' + id);
})


/***************************** for drag n' drop zone ********************************************/

// Code By Webdevtrick ( https://webdevtrick.com )
function readFile(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var htmlPreview =
                '<img width="200" src="' + e.target.result + '" />' +
                '<p>' + input.files[0].name + '</p>';
            var wrapperZone = $(input).parent();
            var previewZone = $(input).parent().parent().find('.preview-zone');
            var boxZone = $(input).parent().parent().find('.preview-zone').find('.box').find('.box-body');

            wrapperZone.removeClass('dragover');
            previewZone.removeClass('hidden');
            boxZone.empty();
            boxZone.append(htmlPreview);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function reset(e) {
    e.wrap('<form>').closest('form').get(0).reset();
    e.unwrap();
}

$(".dropzone").change(function () {
    readFile(this);
});

$('.dropzone-wrapper').on('dragover', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).addClass('dragover');
});

$('.dropzone-wrapper').on('dragleave', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).removeClass('dragover');
});

$('.remove-preview').on('click', function () {
    var boxZone = $(this).parents('.preview-zone').find('.box-body');
    var previewZone = $(this).parents('.preview-zone');
    var dropzone = $(this).parents('.form-group').find('.dropzone');
    boxZone.empty();
    previewZone.addClass('hidden');
    reset(dropzone);
});


//resizing text areas


//$("textarea").each(function () {
//    this.setAttribute("style", "height:" + (this.scrollHeight) + "px;overflow-y:hidden;");
//}).on("input", function () {
//    this.style.height = "auto";
//    this.style.height = (this.scrollHeight) + "px";
//});

const tx = document.getElementsByTagName("textarea");
for (let i = 0; i < tx.length; i++) {
    tx[i].setAttribute("style", "height:" + (tx[i].scrollHeight) + "px;overflow-y:hidden;");
    tx[i].addEventListener("input", OnInput, false);
}

function OnInput() {
    this.style.height = "auto";
    this.style.height = (this.scrollHeight) + "px";
}