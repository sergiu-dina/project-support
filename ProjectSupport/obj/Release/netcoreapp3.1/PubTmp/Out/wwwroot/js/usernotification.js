"use strict";

var connection = null;

function connectToNotificationHub(userId) {
  connection = new signalR.HubConnectionBuilder().withUrl("/NotificationsHub?userId=" + userId).build();


  connection.on("sendToUser", (description, timestamp) => {
    console.log(description + " " + timestamp);
    let numberOfNotifications = parseInt(document.getElementById("notificationCounter").innerHTML) || 0;
    document.getElementById("notificationCounter").innerHTML = numberOfNotifications + 1;
  });

  connection.start().catch(function (err) {
    return console.error(err.toString());
  }).then(function () {
    connection.invoke('GetConnectionId').then(function (connectionId) {
      document.getElementById('signalRConnectionId').innerHTML = connectionId;
    })
  });
}

