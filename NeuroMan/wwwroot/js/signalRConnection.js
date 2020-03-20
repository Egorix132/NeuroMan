"use strict";

var roomNameText = document.getElementsByClassName("roomNameText")[0];

var inputWeights = [];
var outputWeights = [];

function WSConnect() {
    var readiness = false;

    var connection = new signalR.HubConnectionBuilder().withUrl("/ws").configureLogging(signalR.LogLevel.Information).withAutomaticReconnect().build();

    connection.serverTimeoutInMilliseconds = 1000 * 60 * 10;

    var sendButton = document.getElementById("sendButton");
    var reloginButton = document.getElementsByClassName("re-login")[0];

    //Disable send button until connection is established
    sendButton.disabled = true;

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
            span.textContent = participants[i].name;

            participant.appendChild(circle);
            participant.appendChild(span);

            participantsElem.appendChild(participant)
        }
    });

    connection.on("ShowAnswer", function (answer) {
        var answerElem = document.getElementsByClassName("answer")[0];
        answerElem.innerHTML = answer;

        sendButton.textContent = "Confirm"
        sendButton.style = "Background-color: green"
        readiness = false;
    });

    connection.on("SetCase", function (inputValues) {
        var neuroList = document.getElementsByClassName("neuro-list")[0];
        neuroList.innerHTML = "";

        for (let i = 0; i < inputValues.length; i++){
            var upWeight = document.createElement("div");
            upWeight.className = "inputUpWeight";

            var downWeight = document.createElement("div");
            downWeight.className = "inputDownWeight";

            var neuronValue = document.createElement("p");
            neuronValue.className = "input-neuron-value";
            neuronValue.textContent = inputValues[i];

            var circle = document.createElement("div");
            circle.className = "circle";
            circle.appendChild(upWeight);
            circle.appendChild(downWeight);
            circle.appendChild(neuronValue);    

            var inputField = document.createElement("input");
            inputField.className = "weight i";
            inputField.type = "number";
            inputField.name = `[${i}]`;
            inputField.min = "-1";
            inputField.max = "1";
            inputField.step = "0.1";
            inputField.value = inputWeights[i] ?? 0;

            var text = document.createElement("p");
            text.className = "weight-text";
            text.textContent = "weight: ";
            text.appendChild(inputField);

            var neuron = document.createElement("li");
            neuron.className = "col-1 neuro-field__list-neurons__input-neuron";
            neuron.appendChild(circle);
            neuron.appendChild(text);

            neuroList.appendChild(neuron); 
        }

        let counter = 0;

        var inputPicture = document.getElementsByClassName("input-picture")[0];
        inputPicture.innerHTML = "";
        let factors = findNearestsFactors(inputValues.length);

        for (let i = 0; i < factors[0]; i++) {
            var row = document.createElement("tr");
            for (let j = 0; j < factors[1] && counter < inputValues.length; j++) {
                var cell = document.createElement("td");
                cell.style = `background-color: rgba(0, 0, 0, ${inputValues[counter]})`;
                row.appendChild(cell);
                counter++;
            }
            inputPicture.appendChild(row);
        }

        UpdateInput();
    });

    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
        roomNameText.textContent = "Room: " + getCookie("room")
    }).catch(function (err) {
        return console.error(err.toString());
    });

    sendButton.addEventListener("click", sendWeights);

    reloginButton.addEventListener("click", function (event) {
        deleteCookie("room");
        document.location.reload(true);
    });

    document.addEventListener('keyup', function (event) {
        if (event.code == 'Enter') {
            sendWeights();
        }
    });


    function sendWeights(event) {

        if (readiness) {
            sendButton.textContent = "Confirm"
            sendButton.style = "Background-color: green"
            readiness = false;
        }
        else {
            checkWeights();

            sendButton.textContent = "Cancel"
            sendButton.style = "Background-color: red"
            readiness = true;
        }

        connection.invoke("SetWeights", readiness, inputWeights, outputWeights).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    }
    function checkWeights() {

        var inputWeightsElems = document.getElementsByClassName("i");
        for (let i = 0; i < inputWeightsElems.length; i++) {
            inputWeights[i] = parseFloat(inputWeightsElems[i].value);
        }

        var outputWeightsElems = document.getElementsByClassName("o");
        for (let i = 0; i < outputWeightsElems.length; i++) {
            outputWeights[i] = parseFloat(outputWeightsElems[i].value);
        }
    }

    function findNearestsFactors(num) {
        let root = Math.round(Math.sqrt(num));
        for (let i = 0; i < 4 && root < num; i++) {
            if (num % root == 0) {
                return [root, num / root];
            }
            root++;
        }

        return [root, Math.ceil(num / root)];
    }

    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }

    function deleteCookie(name) {
        setCookie(name, "", {
            'max-age': -1
        })
    }

    function setCookie(name, value, options = {}) {

        if (options.expires instanceof Date) {
            options.expires = options.expires.toUTCString();
        }

        let updatedCookie = encodeURIComponent(name) + "=" + encodeURIComponent(value);

        for (let optionKey in options) {
            updatedCookie += "; " + optionKey;
            let optionValue = options[optionKey];
            if (optionValue !== true) {
                updatedCookie += "=" + optionValue;
            }
        }

        document.cookie = updatedCookie;
    }
}
