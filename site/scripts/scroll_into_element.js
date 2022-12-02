function scroll_into_element(element_id) {
    var element = document.getElementById(element_id)
    if (element != null)
        element.scrollIntoView({
            behavior: 'auto',
            block: 'center',
            inline: 'center'
        });
}