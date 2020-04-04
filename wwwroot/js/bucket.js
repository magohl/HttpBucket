"use strict";

// Popovers Initialization
$(function () {
    $('[data-toggle="popover"]').popover()
});

var bucketId = window.location.pathname.split('/')[2];
console.log('BucketID:' + bucketId);
var connection = new signalR.HubConnectionBuilder().withUrl("/bucketHub?bucketId="+bucketId).build();

connection.on("ReceiveBucketMessage", function (data) {
    var body = data.body.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    console.log(body);

    var table = document.getElementById("messagesList");
    var row = table.insertRow(1);
    addCell(row, 0, data.id);
    addCell(row, 1, data.received);
    addCell(row, 2, data.method);
    addCell(row, 3, data.path);
    addCell(row, 4, data.statusCodeToReturn);
    addCell(row, 5, data.body);
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
