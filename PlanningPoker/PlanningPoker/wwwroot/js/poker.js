"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

const roomName = document.getElementById("roomName");
const userName = document.getElementById("userName");
//const joinRoomButton = document.getElementById("joinRoom");
const votesList = document.getElementById("paticipants");
const voteButtons = document.querySelectorAll(".vote");
const resetVotesButton = document.getElementById("resetVotes");

let currentRoom = "";
let currentUser = "";

connection.on("UserJoined", function (newUser, userList) {
    console.log(newUser);

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
    const item = document.createElement("li");
    item.textContent = `${userName} voted: ${vote}`;
    votesList.appendChild(item);
});

connection.on("VotesReset", () => {
    votesList.innerHTML = "";
});
connection.start().then(function () {
    currentRoom = roomName.value;
    currentUser = userName.value;
    connection.invoke("JoinRoom", currentRoom, currentUser).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
   return console.error(err.toString());
});

voteButtons.forEach(button => {
    button.addEventListener("click", async () => {
        const vote = button.getAttribute("data-value");
        await connection.invoke("SubmitVote", currentRoom, currentUser, vote);
    });
});

resetVotesButton.addEventListener("click", async () => {
    await connection.invoke("ResetVotes", roomName);
});

function createUserThumbnail(user, avatarUrl) {
    // Create a container div for the thumbnail
    const thumbnailDiv = document.createElement("div");
    thumbnailDiv.className = "thumbnail text-center"; // Bootstrap class for styling

    // Create an image element
    const img = document.createElement("img");
    img.src = avatarUrl; // Set the avatar URL
   // img.alt = '${user}'s avatar''; // Alt text for the image
    img.className = "img-thumbnail"; // Optional: add Bootstrap's thumbnail class
    img.style.width = "100px"; // Adjust the size as needed
    img.style.height = "100px"; // Adjust the size as needed

    // Create a caption for the username
    const caption = document.createElement("p");
    caption.textContent = user;
    caption.className = "mt-2"; // Add some margin on top for spacing

    // Append the image and caption to the thumbnail div
    thumbnailDiv.appendChild(img);
    thumbnailDiv.appendChild(caption);

    return thumbnailDiv;
}