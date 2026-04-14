var ALERT_BUTTON_TEXT = "Ok";

if (document.getElementById) {
    window.alert = function (typ, txtmsg, color) {
        createCustomAlert(typ, txtmsg, color);
    }
}

function createCustomAlert(typ, txtmsg, color) {
    d = document;

    if (d.getElementById("modalContainer")) return;

    mObj = d.getElementsByTagName("body")[0].appendChild(d.createElement("div"));
    mObj.id = "modalContainer";
    mObj.style.height = d.documentElement.scrollHeight + "px";

    alertObj = mObj.appendChild(d.createElement("div"));
    alertObj.id = "alertBox";
    if (d.all && !window.opera) alertObj.style.top = document.documentElement.scrollTop + "px";
    alertObj.style.left = (d.documentElement.scrollWidth - alertObj.offsetWidth) / 2 + "px";
    alertObj.style.visiblity = "visible";
    if (color == "green") {
        alertObj.style.background = "radial-gradient(#7FFFD4, #98FB98)";
    }
    if (color == "red") {
        alertObj.style.background = "radial-gradient(#FFC0CB, #F08080)";
    }

    h1 = alertObj.appendChild(d.createElement("h1"));
    h1.style.background = color;

    h1.appendChild(d.createTextNode(typ));

    msg = alertObj.appendChild(d.createElement("p"));
    //msg.appendChild(d.createTextNode(txt));
    msg.innerHTML = txtmsg;

    btn = alertObj.appendChild(d.createElement("a"));
    btn.id = "closeBtn";
    btn.appendChild(d.createTextNode(ALERT_BUTTON_TEXT));
    btn.style.background = color;
    btn.href = "#";
    btn.focus();
    btn.onclick = function () {
        redirect(color);
        return false;
    }
    alertObj.style.display = "block";
}

function removeCustomAlert() {
    document.getElementsByTagName("body")[0].removeChild(document.getElementById("modalContainer"));
}

function sleep(millisecondsToWait) {
    var now = new Date().getTime();
    while (new Date().getTime() < now + millisecondsToWait) {
    }
}