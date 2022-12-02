function check_form() {
    var correctLogin = check_login()
    var correctName = check_name()
    var correctPasswords = match_password()
    return correctLogin && correctName && correctPasswords
}

function check_login(input) {
    var login = input.value;
    if (document.getElementById("taken-login-hint") != null)
        document.getElementById("taken-login-hint").style.display = "none"

    var systemWords = ["login", "register", "logout", "edit"]
    if (!systemWords.includes(login)) {
        document.getElementById("system-login-hint").style.display = "none"
    }
    else {
        document.getElementById("system-login-hint").style.display = "block"
        return false;
    }

    if (/^[a-zA-Z0-9_]*$/.test(login)) {
        document.getElementById("correct-login-hint").style.display = "none"
        return true
    }
    else {
        document.getElementById("correct-login-hint").style.display = "block"
        return false
    }
}

function check_name(input) {
    var name = input.value;
    if (/^[а-яА-ЯёЁ ]*$/.test(name)) {
        document.getElementById("correct-name-hint").style.display = "none"
        return true
    }
    else {
        document.getElementById("correct-name-hint").style.display = "block"
        return false
    }
}

function match_password(input1, input2, hint) {
    var pw1 = document.getElementById(input1).value;
    var pw2 = document.getElementById(input2).value;
    if (pw1 == pw2) {
        document.getElementById(hint).style.display = "none"
        return true
    }
    else {
        document.getElementById(hint).style.display = "block"
        return false
    }
}