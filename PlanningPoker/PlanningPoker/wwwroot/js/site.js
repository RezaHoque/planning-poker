﻿
document.addEventListener("DOMContentLoaded", function () {

    const leaveButton = document.getElementById("btnLeave");
    const joinRoomBtn = document.getElementById("joinRoomBtn");
    const navUserName = document.getElementById("navUserName");
    const returnUrl = getQueryStringParameter("returnUrl");


    if (!getCookie("userName")) {
        leaveButton.style.display = "none";
    } else {
        navUserName.textContent = getCookie("userName");
    }
   
    if (joinRoomBtn) {
        if (returnUrl) {
            joinRoomBtn.textContent = "Join room";
        } else {
            joinRoomBtn.textContent = "Start a room";
        }
        joinRoomBtn.addEventListener("click", function () {
            const userName = document.getElementById("userName").value;
            const roomId = document.getElementById("roomId").value;

            if (!userName) {
                alert("Please enter your name!");
                return;
            }
            setCookie("userName", userName, 1);

            window.location.href = returnUrl ? returnUrl : `/room/${roomId}`;
        });
    }
    
    leaveButton.addEventListener("click", function () {
        document.cookie = "userName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        window.location.href = "/";
    });
    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return null;
    } 

    function getQueryStringParameter(name) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(name);
    }
    function setCookie(name, value, days) {
        const date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000); // Days to milliseconds
        const expires = "expires=" + date.toUTCString();
        document.cookie = `${name}=${value}; ${expires}; path=/`;
    }
});
