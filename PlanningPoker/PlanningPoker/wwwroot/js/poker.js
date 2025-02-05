"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

const roomName = document.getElementById("roomName");
const userName = document.getElementById("userName");
const avatarPack = document.getElementById("avatarPack");
const avarageVoteContainer = document.getElementById("avarageVote");
const votesList = document.getElementById("paticipants");
const voteButtons = document.querySelectorAll(".vote");
const resetVotesButton = document.getElementById("resetVotes");
const revealVotesButton = document.getElementById("revealVotes");
const navUserName = document.getElementById("navUserName");
const reactionDiv = document.getElementById("reaction");


let currentRoom = "";
let currentUser = "";
let avarageVote = 0;
let totalUsers = 0;
let votes = [];
let userVoted = [];
const roomVotes = new Map();

connection.on("UserJoined", function (newUser, userList) {

    totalUsers = userList.length;
    
    const thumbnailDiv = createUserThumbnail(newUser.name, newUser.avatar);
    votesList.appendChild(thumbnailDiv);
    
    
});

connection.on("ReceiveUserList", (userList) => {
    votesList.innerHTML = "";

    userList.forEach(user => {
        const thumbnailDiv = createUserThumbnail(user.userName, user.avatar);
        votesList.appendChild(thumbnailDiv);
    });
});
connection.on("ReceiveVote", function (user, vote) {
    var badgeId = "userBadge" + getReplacedStr(user) + currentRoom;
    const badge = document.getElementById(badgeId);
    badge.innerHTML = '<i class="bi bi-check-circle fs-5"></i>';
    badge.className = "badge bg-success";
    badge.style.fontSize = "18px"

});
connection.on("ReceiveAvarageVote", function (average, fibonacciNumber, allUservotes, gifUrl) {

    var span = createAverageVoteBadge(average,"avarageVoteBadge");
    avarageVoteContainer.innerHTML = "";
    avarageVoteContainer.appendChild(span);

    voteButtons.forEach(button => {
        if (parseInt(button.getAttribute('data-value')) === fibonacciNumber) {
            button.classList.remove('vote');
            button.classList.add('voteHighlight');
        }
    });

    //displaying each users vote.
    const votes = allUservotes;

    votes.forEach((vote) => {
        
        var badgeId = "userBadge" + getReplacedStr(vote.userName) + currentRoom;
        const badge = document.querySelector(`#${badgeId}`);
        if (badge) {
            if (vote.vote == "coffee") {
                badge.innerHTML = `
                    <svg xmlns = "http://www.w3.org/2000/svg" width = "24" height = "24" fill = "currentColor" class="bi bi-cup-hot-fill" viewBox = "0 0 16 16" >
                        <path fill - rule="evenodd" d = "M.5 6a.5.5 0 0 0-.488.608l1.652 7.434A2.5 2.5 0 0 0 4.104 16h5.792a2.5 2.5 0 0 0 2.44-1.958l.131-.59a3 3 0 0 0 1.3-5.854l.221-.99A.5.5 0 0 0 13.5 6zM13 12.5a2 2 0 0 1-.316-.025l.867-3.898A2.001 2.001 0 0 1 13 12.5" />
                        <path d="m4.4.8-.003.004-.014.019a4 4 0 0 0-.204.31 2 2 0 0 0-.141.267c-.026.06-.034.092-.037.103v.004a.6.6 0 0 0 .091.248c.075.133.178.272.308.445l.01.012c.118.158.26.347.37.543.112.2.22.455.22.745 0 .188-.065.368-.119.494a3 3 0 0 1-.202.388 5 5 0 0 1-.253.382l-.018.025-.005.008-.002.002A.5.5 0 0 1 3.6 4.2l.003-.004.014-.019a4 4 0 0 0 .204-.31 2 2 0 0 0 .141-.267c.026-.06.034-.092.037-.103a.6.6 0 0 0-.09-.252A4 4 0 0 0 3.6 2.8l-.01-.012a5 5 0 0 1-.37-.543A1.53 1.53 0 0 1 3 1.5c0-.188.065-.368.119-.494.059-.138.134-.274.202-.388a6 6 0 0 1 .253-.382l.025-.035A.5.5 0 0 1 4.4.8m3 0-.003.004-.014.019a4 4 0 0 0-.204.31 2 2 0 0 0-.141.267c-.026.06-.034.092-.037.103v.004a.6.6 0 0 0 .091.248c.075.133.178.272.308.445l.01.012c.118.158.26.347.37.543.112.2.22.455.22.745 0 .188-.065.368-.119.494a3 3 0 0 1-.202.388 5 5 0 0 1-.253.382l-.018.025-.005.008-.002.002A.5.5 0 0 1 6.6 4.2l.003-.004.014-.019a4 4 0 0 0 .204-.31 2 2 0 0 0 .141-.267c.026-.06.034-.092.037-.103a.6.6 0 0 0-.09-.252A4 4 0 0 0 6.6 2.8l-.01-.012a5 5 0 0 1-.37-.543A1.53 1.53 0 0 1 6 1.5c0-.188.065-.368.119-.494.059-.138.134-.274.202-.388a6 6 0 0 1 .253-.382l.025-.035A.5.5 0 0 1 7.4.8m3 0-.003.004-.014.019a4 4 0 0 0-.204.31 2 2 0 0 0-.141.267c-.026.06-.034.092-.037.103v.004a.6.6 0 0 0 .091.248c.075.133.178.272.308.445l.01.012c.118.158.26.347.37.543.112.2.22.455.22.745 0 .188-.065.368-.119.494a3 3 0 0 1-.202.388 5 5 0 0 1-.252.382l-.019.025-.005.008-.002.002A.5.5 0 0 1 9.6 4.2l.003-.004.014-.019a4 4 0 0 0 .204-.31 2 2 0 0 0 .141-.267c.026-.06.034-.092.037-.103a.6.6 0 0 0-.09-.252A4 4 0 0 0 9.6 2.8l-.01-.012a5 5 0 0 1-.37-.543A1.53 1.53 0 0 1 9 1.5c0-.188.065-.368.119-.494.059-.138.134-.274.202-.388a6 6 0 0 1 .253-.382l.025-.035A.5.5 0 0 1 10.4.8" />
                    </svg > `;

                badge.className = "badge bg-warning";
                badge.style.fontSize = "18px";
            } else if (vote.vote == "question") {
                badge.innerHTML = '<i class="bi bi-patch-question fs-5"></i>';
                badge.className = "badge bg-warning";
                badge.style.fontSize = "18px"
            } else if (vote.vote == "infinity") {
                badge.innerHTML = `
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-infinity" viewBox="0 0 16 16">
                            <path d="M5.68 5.792 7.345 7.75 5.681 9.708a2.75 2.75 0 1 1 0-3.916ZM8 6.978 6.416 5.113l-.014-.015a3.75 3.75 0 1 0 0 5.304l.014-.015L8 8.522l1.584 1.865.014.015a3.75 3.75 0 1 0 0-5.304l-.014.015zm.656.772 1.663-1.958a2.75 2.75 0 1 1 0 3.916z" />
                        </svg>`;
                badge.className = "badge bg-warning";
                badge.style.fontSize = "18px";

            } else {
                badge.innerHTML = vote.vote;
                badge.className = "badge bg-success";
                badge.style.fontSize = "22px";
            }
        }
    });

    
    reactionDiv.innerHTML = `<img src="${gifUrl}" alt="reaction gif" style="max-width:300px; height:300px;" />`;
});
connection.on("ClearVotes", function (allUserVotes) {
    const votes = allUserVotes;
    const avarageVotebadge = document.getElementById(avarageVoteBadge);

    votes.forEach((vote) => {
        var badgeId = "userBadge" + getReplacedStr(vote.userName) + currentRoom;
        const badge = document.querySelector(`#${badgeId}`);

        if (badge) {
            badge.innerHTML = "";
            badge.textContent = "0";
            badge.className = "badge bg-light badge-soft";
            badge.style.fontSize = "25px";
        }
    });
    avarageVoteContainer.innerHTML = "";
    reactionDiv.innerHTML = "";

    const voteHighlightButtons = document.querySelectorAll(".voteHighlight");
    voteHighlightButtons.forEach(button => {

        button.classList.remove('voteHighlight');
        button.classList.add('vote');
    });
   

});
connection.start().then(function () {
    currentRoom = roomName.value;
    currentUser = userName.value;
    navUserName.textContent = currentUser;

    var badgeId = "userBadge" + getReplacedStr(currentUser) + currentRoom;
    connection.invoke("JoinRoom", currentRoom, currentUser, avatarPack.value, false).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
   return console.error(err.toString());
});

connection.on("Leaveroom", (userName) => {
    var badgeId = "userThumbnail_" + getReplacedStr(userName) + currentRoom;
    const badge = document.getElementById(badgeId);
    badge.remove();
});

voteButtons.forEach(button => {
    button.addEventListener("click", async () => {
        const vote = button.getAttribute("data-value");
        var badgeId = "userBadge" + getReplacedStr(currentUser) + currentRoom;
        const badge = document.getElementById(badgeId);
      
        badge.dataset.vote = vote;
        await connection.invoke("SubmitVote", currentRoom, currentUser, vote);
    });
});

resetVotesButton.addEventListener("click", async () => {
    await connection.invoke("ResetVotes", currentRoom).catch(function (err) {
        return console.error(err.toString());
    });
});

revealVotesButton.addEventListener("click", async () => {
    await connection.invoke("RevealVotes", currentRoom).catch(function (err) {
        return console.error(err.toString());
    });
});

function createUserThumbnail(user, avatarUrl) {
    // Create a container div for the thumbnail
    const userStr = getReplacedStr(user);
    const thumbnailDiv = document.createElement("div");
    thumbnailDiv.className = "thumbnail text-center";
    thumbnailDiv.id = "userThumbnail_" + userStr + currentRoom;

    // Create the badge
    const badge = document.createElement("span");
    badge.className = "badge bg-light badge-soft";
    badge.style.top = "10px";
    badge.style.left = "50%";

    badge.style.minWidth = "50px"; 
    badge.style.height = "50px"; 
    badge.style.lineHeight = "30px";
    badge.style.borderRadius = "30%"; 
    badge.textContent = "0"; 
    badge.style.fontSize = "25px";
    badge.id = "userBadge" + userStr + currentRoom;

    // Create an image element
    const img = document.createElement("img");
    if (avatarUrl.includes("multiavatar")) {
        var svg = multiavatar(user);
        const encodedSVG = encodeURIComponent(svg);
        img.src = `data:image/svg+xml;charset=utf-8,${encodedSVG}`;
    } else {
        img.src = avatarUrl;
    }

    img.className = "img-thumbnail shadow";
    img.style.width = "100px"; 
    img.style.height = "100px";

    // Create a caption for the username
    const caption = document.createElement("p");
    caption.textContent = user;
    caption.className = "mt-2";

    
    thumbnailDiv.appendChild(img);
    thumbnailDiv.appendChild(caption);
    thumbnailDiv.appendChild(badge);

    return thumbnailDiv;
}
function getVotesForRoom(roomName) {
        connection.invoke("GetRoomVotesWithUsers", roomName)
            .then(response => {
                console.log(response);
                return response;
        })
        .catch(err => {
            console.error(err);
        });
}
function clearVotesForRoom(roomName) {
    if (roomVotes.has(roomName)) {
        roomVotes.get(roomName).clear();
    }
    votes = [];
}

function getReplacedStr(inputStr) {
    return inputStr.replace(/[^a-zA-Z0-9]/g, '');
}

window.addEventListener("beforeunload", async () => {
    try {
        await connection.stop();
        console.log("Disconnected from SignalR hub due to browser/tab close");
    } catch (err) {
        console.error("Error during SignalR disconnection:", err);
    }
});

function createAverageVoteBadge(average,id) {
    const avarageVotebadge = document.createElement("span");
    avarageVotebadge.id = id;
    avarageVotebadge.className = "badge bg-light text-center";
    avarageVotebadge.style.top = "10px";
    avarageVotebadge.style.left = "50%";
    avarageVotebadge.style.color = average <= 8 ? "#198754" : "#dc3545";
    avarageVotebadge.style.minWidth = "70px";
    avarageVotebadge.style.height = "70px";
    avarageVotebadge.style.lineHeight = "35px";
    avarageVotebadge.style.borderRadius = "20%";
    avarageVotebadge.style.fontSize = "40px";
    avarageVotebadge.style.border = "1px solid #e9ecef";
    avarageVotebadge.textContent = average;

    return avarageVotebadge;
}
