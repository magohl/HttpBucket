"use strict";

var bucketId = window.location.pathname.split('/')[2];
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/bucketHub?bucketId="+bucketId)
    .build();

connection.on("ReceiveBucketMessage", function (data) {
    var body = data.body.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    var table = document.getElementById("messagesList");
    var row = table.insertRow(1);
    addCell(row, 0, data.id);
    addCell(row, 1, data.received);
    addCell(row, 2, data.method);
    addCell(row, 3, data.path);

    var cell = row.insertCell(4);    
    cell.innerHTML = "<i class='fas fa-list' title='" + data.headers + "'></i>";
    
    addCell(row, 5, data.statusCodeToReturn);
    addCell(row, 6, data.body);
});

function addCell(row, cellId, data) {
    var cell = row.insertCell(cellId);
    var cellText  = document.createTextNode(data)
    cell.appendChild(cellText);
}

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
