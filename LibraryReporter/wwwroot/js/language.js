﻿$(document).ready(function () {
    $(".unauth-lang").click(function () {
        const lang = $(this).attr("data-lang");
        eraseCookie("lang");
        setCookie("lang", lang);
        location.reload();
    });

    function setCookie(name, value) {
        document.cookie = name + "=" + (value || "") + "; path=/";
    }

    function getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(";");
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == " ") c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    function eraseCookie(name) {
        document.cookie = name + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
    }
});
