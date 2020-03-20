"use strict";

var joinButton = document.getElementById("joinButton");
var inputName = document.getElementById("inputName");
var inputRoom = document.getElementById("inputRoom");
var overlay = document.getElementsByClassName("overlay")[0];

joinButton.addEventListener("click", async function (event) {

    let user = {
        name : inputName.value,
        room : inputRoom.value
    };

    fetch('/Auth', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(user)
    })
        .then(response => response.text())
        .then(result => {
            switch (result) {
                case "ok":
                    console.log(result);
                    overlay.remove();
                    WSConnect();
                    break;
                case "This name is already taken":
                    alert(result);
                    break;
                case "Room is full":
                    alert(result);
                    break;
            }
        }); 
});