function show_password_confirm(input, div_id) {
    if (input.value != "")
    {
        document.getElementById(div_id).style.display = "block"
    }
    else {
        document.getElementById(div_id).style.display = "none"
    }
}