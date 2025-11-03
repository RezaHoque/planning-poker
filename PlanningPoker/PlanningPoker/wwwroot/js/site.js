
document.addEventListener("DOMContentLoaded", function () {

    const leaveButton = document.getElementById("btnLeave");
    const joinRoomBtn = document.getElementById("joinRoomBtn");
    const navUserName = document.getElementById("navUserName");
    const returnUrl = getQueryStringParameter("returnUrl");
    const userName = document.getElementById("userName");

    // Handle floating label for modern input
    if (userName) {
        // Check initial value
        if (userName.value && userName.value.trim() !== '') {
            userName.classList.add('has-value');
        }
        
        // Handle input events
        userName.addEventListener('input', function() {
            if (this.value && this.value.trim() !== '') {
                this.classList.add('has-value');
            } else {
                this.classList.remove('has-value');
            }
        });
        
        // Handle focus/blur
        userName.addEventListener('focus', function() {
            this.classList.add('has-value');
        });
    }

    let selectedPack = null;

    const icons = document.querySelectorAll('.icon');

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    icons.forEach(icon => {
        icon.addEventListener('click', () => {

            icons.forEach(i => i.classList.remove('selected'));
            icon.classList.add('selected');
            selectedPack = icon.getAttribute('data-pack');
        });
    });
   

    if (!getCookie("userName")) {
        leaveButton.style.display = "none";
    } else {
        navUserName.textContent = getCookie("userName");
        const cookieUserName = getCookie("userName");
        if (cookieUserName) {
            userName.value = cookieUserName;
            userName.classList.add('has-value');
        }
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
            const honeyPot = document.getElementById("trapField").value;

            if (honeyPot) {
                return;
            }
            if (!userName) {
                alert("Please enter your name!");
                return;
            }
           
            setCookie("userName", userName, 1);
            setCookie("avatarPack", selectedPack, 1);
            

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
        if (parts.length === 2) return decodeURIComponent(parts.pop().split(';').shift());
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
        const safeCookievalue = encodeURIComponent(value).replace(/%(23|24|26|2B|5E|60|7C)/g, decodeURIComponent);
        document.cookie = `${name}=${safeCookievalue}; ${expires}; path=/`;
    }
});
