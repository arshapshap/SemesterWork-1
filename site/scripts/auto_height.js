function auto_height(textarea_id) {
    textarea = document.getElementById(textarea_id)
    textarea.style.height = "1px";
    textarea.style.height = (textarea.scrollHeight - 3) + "px";
}

function on_key_down(event) {
    if (event.keyCode == 13)
    {
        event.preventDefault();
    }
}