
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
   

    const copyUrlButton = document.getElementById("btnCopyUrl");
    
    // Handle Copy URL button
    if (copyUrlButton) {
        // Only show copy button if we're in a room (URL contains /room/)
        if (window.location.pathname.includes('/room/')) {
            copyUrlButton.addEventListener("click", function () {
                const currentUrl = window.location.href;
                copyToClipboard(currentUrl);
                showToast("URL copied to clipboard!");
            });
        } else {
            copyUrlButton.style.display = "none";
        }
    }

    if (!getCookie("userName")) {
        if (leaveButton) leaveButton.style.display = "none";
        if (copyUrlButton) copyUrlButton.style.display = "none";
    } else {
        navUserName.textContent = getCookie("userName");
        const cookieUserName = getCookie("userName");
        if (cookieUserName) {
            if (userName) {
                userName.value = cookieUserName;
                userName.classList.add('has-value');
            }
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
    
    if (leaveButton) {
        leaveButton.addEventListener("click", function () {
            document.cookie = "userName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            window.location.href = "/";
        });
    }
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

    // Copy to clipboard function
    function copyToClipboard(text) {
        if (navigator.clipboard && window.isSecureContext) {
            // Use the modern Clipboard API
            navigator.clipboard.writeText(text).then(function() {
                // Success
            }).catch(function(err) {
                // Fallback for older browsers
                fallbackCopyToClipboard(text);
            });
        } else {
            // Fallback for older browsers
            fallbackCopyToClipboard(text);
        }
    }

    // Fallback copy function for older browsers
    function fallbackCopyToClipboard(text) {
        const textArea = document.createElement("textarea");
        textArea.value = text;
        textArea.style.position = "fixed";
        textArea.style.left = "-999999px";
        textArea.style.top = "-999999px";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        try {
            document.execCommand('copy');
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }
        document.body.removeChild(textArea);
    }

    // Show toast notification
    function showToast(message) {
        // Remove existing toast if any
        const existingToast = document.getElementById('copyToast');
        if (existingToast) {
            existingToast.remove();
        }

        // Create toast element
        const toast = document.createElement('div');
        toast.id = 'copyToast';
        toast.className = 'copy-toast';
        toast.textContent = message;
        document.body.appendChild(toast);

        // Trigger animation
        setTimeout(() => {
            toast.classList.add('show');
        }, 10);

        // Remove toast after animation
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 300);
        }, 2000);
    }
});
