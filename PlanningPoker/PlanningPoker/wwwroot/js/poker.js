"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

const roomName = document.getElementById("roomName");
const userName = document.getElementById("userName");
const avarageVoteContainer = document.getElementById("avarageVote");
const votesList = document.getElementById("paticipants");
const voteButtons = document.querySelectorAll(".vote");
const resetVotesButton = document.getElementById("resetVotes");
const revealVotesButton = document.getElementById("revealVotes");
const navUserName = document.getElementById("navUserName");

let currentRoom = "";
let currentUser = "";
let avarageVote = 0;
let totalUsers = 0;
let votes = [];
let userVoted = [];
const roomVotes = new Map();

connection.on("UserJoined", function (newUser, userList) {
    //console.log(newUser);
    totalUsers = userList.length;
    votesList.innerHTML = "";
    userList.forEach(user => {
        const thumbnailDiv = createUserThumbnail(user.userName, user.avatar);
        votesList.appendChild(thumbnailDiv);
    });
});

connection.on("ReceiveUserList", (userList) => {
    votesList.innerHTML = "";
    userList.forEach(user => {
        const thumbnailDiv = createUserThumbnail(user.userName, user.avatar);
        votesList.appendChild(thumbnailDiv);
    });
});
connection.on("ReceiveVote", function (user, vote) {
    var badgeId = "userBadge" + user + currentRoom;
    const badge = document.getElementById(badgeId);
    badge.innerHTML = '<i class="bi bi-check-circle fs-5"></i>';
    badge.className = "badge bg-success";
    badge.style.fontSize = "18px"
    votes.push(vote);
    userVoted.push(user);
    addVote(currentRoom, user, vote);
});
connection.on("ReceiveAvarageVote", function (avarage) {
 
    const avarageVotebadge = document.createElement("span");
    avarageVotebadge.id = "avarageVoteBadge";
    avarageVotebadge.className = "badge bg-light text-center";
    avarageVotebadge.style.top = "10px";
    avarageVotebadge.style.left = "50%";
    avarageVotebadge.style.color = "black";
    avarageVotebadge.style.minWidth = "60px";
    avarageVotebadge.style.height = "60px";
    avarageVotebadge.style.lineHeight = "30px";
    avarageVotebadge.style.borderRadius = "20%";
    avarageVotebadge.style.fontSize = "35px";
    avarageVotebadge.textContent = avarage;

    avarageVoteContainer.innerHTML = "";
    avarageVoteContainer.appendChild(avarageVotebadge);

    const votes = getVotesForRoom(currentRoom);

    votes.forEach((vote, userName) => {
        var badgeId = "userBadge" + userName + currentRoom;
        const badge = document.querySelector(`#${badgeId}`);
        console.log(badgeId);
        if (badge) {
            badge.innerHTML = vote; // Display the vote
            badge.className = "badge bg-success";
            badge.style.fontSize = "22px";
        }
    });

});
connection.on("ClearVotes", () => {
    const votes = getVotesForRoom(currentRoom);
    const avarageVotebadge = document.getElementById(avarageVoteBadge);

    votes.forEach((vote, userName) => {
        var badgeId = "userBadge" + userName + currentRoom;
        const badge = document.querySelector(`#${badgeId}`);
        console.log(badgeId);
        if (badge) {
            badge.innerHTML = "";
            badge.textContent = "0";
            badge.className = "badge bg-light badge-soft";
            badge.style.fontSize = "25px";
        }
    });
    avarageVoteContainer.innerHTML = "";
});
connection.start().then(function () {
    currentRoom = roomName.value;
    currentUser = userName.value;
    navUserName.textContent = currentUser;
    var badgeId = "userBadge" + currentUser + currentRoom;
    connection.invoke("JoinRoom", currentRoom, currentUser).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
   return console.error(err.toString());
});

voteButtons.forEach(button => {
    button.addEventListener("click", async () => {
        const vote = button.getAttribute("data-value");
        //setting data attribute.
        var badgeId = "userBadge" + currentUser + currentRoom;
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
    await connection.invoke("RevealVotes", currentRoom, userVoted, votes).catch(function (err) {
        return console.error(err.toString());
    });
});

function createUserThumbnail(user, avatarUrl) {
    // Create a container div for the thumbnail
    const thumbnailDiv = document.createElement("div");
    thumbnailDiv.className = "thumbnail text-center"; 

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
    badge.id = "userBadge" + user + currentRoom;

    // Create an image element
    const img = document.createElement("img");
    img.src = avatarUrl; // Set the avatar URL
    img.className = "img-thumbnail";
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
function addVote(roomName, userName, vote) {
    if (!roomVotes.has(roomName)) {
        roomVotes.set(roomName, new Map());
    }
    const votes = roomVotes.get(roomName);
    votes.set(userName, vote);
}
function getVotesForRoom(roomName) {
    return roomVotes.get(roomName) || new Map();
}
function clearVotesForRoom(roomName) {
    if (roomVotes.has(roomName)) {
        roomVotes.get(roomName).clear();
    }
}