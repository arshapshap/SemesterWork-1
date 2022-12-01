function checkForm() {
    var correctLogin = checkLogin()
    var correctName = checkName()
    var correctPasswords = matchPassword()
    return correctLogin && correctName && correctPasswords
}

function checkLogin(input) {
    var login = input.value;
    if (document.getElementById("taken-login-hint") is not null)
        document.getElementById("taken-login-hint").style.display = "none"

    var systemWords = ["login", "register", "logout"]
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

function checkName(input) {
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

function matchPassword() {
    var pw1 = document.getElementById("password").value;
    var pw2 = document.getElementById("password-confirm").value;
    if (pw1 == pw2) {
        document.getElementById("password-confirm-hint").style.display = "none"
        return true
    }
    else {
        document.getElementById("password-confirm-hint").style.display = "block"
        return false
    }
}