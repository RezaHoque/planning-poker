"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

const roomName = document.getElementById("roomName");
const userName = document.getElementById("userName");
//const joinRoomButton = document.getElementById("joinRoom");
const votesList = document.getElementById("votes");
const voteButtons = document.querySelectorAll(".vote");
const resetVotesButton = document.getElementById("resetVotes");

let currentRoom = "";
let currentUser = "";

connection.on("UserJoined", function (userName) {
    console.log(userName);
    const item = document.createElement("li");
    item.textContent = `${userName} joined the room.`;
    votesList.appendChild(item);
});
connection.on("ReceiveUserList", (userList) => {
    votesList.innerHTML = "";
    userList.forEach(user => {
        const item = document.createElement("li");
        item.textContent = user;
        votesList.appendChild(item);
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