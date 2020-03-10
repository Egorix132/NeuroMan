"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ws").configureLogging(signalR.LogLevel.Information).withAutomaticReconnect().build();

connection.serverTimeoutInMilliseconds = 1000 * 60 * 10;

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("UpdateWeights", function (inputWeights, outputWeights) {
    var inputWeightElems = document.getElementsByClassName("i");
    var outputWeightElems = document.getElementsByClassName("o");

    for (let i = 0; i < inputWeightElems.length; i++) {
        inputWeightElems[i].textContent = inputWeights[i];
    }

    for (let i = 0; i < outputWeightElems.length; i++) {
        outputWeightElems[i].textContent = outputWeights[i];
    }
});

connection.on("UpdateUsersStates", function (participants) {
    var participantsElem = document.getElementsByClassName("participants")[0];
    participantsElem.innerHTML = "";

    for (let i = 0; i < participants.length; i++) {
        var participant = document.createElement("div")
        participant.className = "userState";

        var circle = document.createElement("div");
        circle.className = "circle";
        circle.style = `background-color: ${participants[i].isReady ? "green" : "gray"};`;

        var span = document.createElement("span");
        span.className = "userNames";
        span.textContent = participants[i].ip;

        participant.appendChild(circle);
        participant.appendChild(span);

        participantsElem.appendChild(participant)
    }
});

connection.on("ShowAnswer", function (answer) {
    var answerElem = document.getElementsByClassName("answer")[0];
    answerElem.innerHTML = answer;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    var inputWeightsElems = document.getElementsByClassName("i");
    let inputWeights = [];

    for (let i = 0; i < inputWeightsElems.length; i++) {
        inputWeights.push(parseFloat(inputWeightsElems[i].value));
    }

    var outputWeightsElems = document.getElementsByClassName("o");
    let outputWeights = [];

    for (let i = 0; i < outputWeightsElems.length; i++) {
        outputWeights.push(parseFloat(outputWeightsElems[i].value));
    }

    connection.invoke("SendWeights", inputWeights, outputWeights).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});