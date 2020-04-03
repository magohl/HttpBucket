"use strict";

var bucketId = window.location.pathname.split('/')[2];
console.log('BucketID:' + bucketId);
var connection = new signalR.HubConnectionBuilder().withUrl("/bucketHub?bucketId="+bucketId).build();

connection.on("ReceiveBucketMessage", function (date, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var fullMsg = date + ' | ' + message;
    var li = document.createElement("li");
    li.classList.add("list-group-item");
    li.textContent = fullMsg;
    document.getElementById("messagesList").prepend(li);
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
