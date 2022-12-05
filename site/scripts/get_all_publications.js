function get_all_publications(popular = true) {
    var url = popular ? "popular" : "new";
    $.ajax({
        type: "GET",
        url: "/" + url,
        success: function (response) {
            if (response != "") {
                $('#container').replaceWith(response)
            }
        }
    });
}