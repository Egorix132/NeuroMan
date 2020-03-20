var inputValuesEl = document.getElementsByClassName("input-neuron-value")
var outputValuesEl = document.getElementsByClassName("output-neuron-value")
var inputValuesUp = document.getElementsByClassName("inputUpWeight")
var inputValuesDown = document.getElementsByClassName("inputDownWeight")
var outputValuesUp = document.getElementsByClassName("outputUpWeight")
var outputValuesDown = document.getElementsByClassName("outputDownWeight")
var inputWeightsEl = document.getElementsByClassName("i");
var outputWeightsEl = document.getElementsByClassName("o");

var currentInputIndex = 0;

for (let i = 0; i < outputValuesUp.length; i++) {

    outputValuesUp[i].addEventListener("click", function (event) {
        value = parseFloat(outputWeightsEl[i].value);
        value = value + 0.05 > 1 ? 1 : value + 0.05;
        outputWeightsEl[i].value = value;
        Calculate();
    });

    outputValuesDown[i].addEventListener("click", function (event) {
        value = parseFloat(outputWeightsEl[i].value);
        value = value - 0.05 < -1 ? -1 : value - 0.05;
        outputWeightsEl[i].value = value;
        Calculate();
    });
}

for (let i = 0; i < outputWeightsEl.length; i++) {
    outputWeightsEl[i].addEventListener("input", Calculate);
    outputWeightsEl[i].addEventListener('keyup', ToNextInput);
}

var MainValue = 0;

Calculate();

function ToNextInput(event) {
    if (event.code == 'ArrowLeft') {
        if (currentInputIndex == 0)
            currentInputIndex = outputWeightsEl.length + inputWeightsEl.length - 1;
        else
            currentInputIndex--;

        if (currentInputIndex >= inputWeightsEl.length) {
            outputWeightsEl[currentInputIndex - inputWeightsEl.length].focus();
        }
        else {
            inputWeightsEl[currentInputIndex].focus();
        }
    }

    if (event.code == 'ArrowRight') {
        if (currentInputIndex == outputWeightsEl.length + inputWeightsEl.length - 1)
            currentInputIndex = 0;
        else
            currentInputIndex++;

        if (currentInputIndex >= inputWeightsEl.length) {
            outputWeightsEl[currentInputIndex - inputWeightsEl.length].focus();
        }
        else {
            inputWeightsEl[currentInputIndex].focus();
        }
    }
}

function Calculate() {
    MainValue = 0;
    for (let i = 0; i < inputWeightsEl.length; i++) {
        MainValue += inputWeightsEl[i].value * (parseInt(inputValuesEl[i].textContent));
    };

    MainValue = Sigmoid(MainValue - 1);

    document.getElementsByClassName("main-value")[0].textContent = MainValue.toString().slice(0, 5);

    var outputSum = 0;

    for (let i = 0; i < outputWeightsEl.length; i++) {
        outputValuesEl[i].textContent = Sigmoid(outputWeightsEl[i].value * MainValue - 1);
        outputSum += parseFloat(outputValuesEl[i].textContent);
    };

    for (let i = 0; i < outputWeightsEl.length; i++) {
        outputValuesEl[i].textContent = (parseFloat(outputValuesEl[i].textContent) / outputSum).toString().slice(0, 5);
    };
}

function UpdateInput() {
    inputValuesEl = document.getElementsByClassName("input-neuron-value")
    inputValuesUp = document.getElementsByClassName("inputUpWeight")
    inputValuesDown = document.getElementsByClassName("inputDownWeight")
    inputWeightsEl = document.getElementsByClassName("i");

    for (let i = 0; i < inputWeightsEl.length; i++) {
        inputWeightsEl[i].addEventListener("input", Calculate);
        inputWeightsEl[i].addEventListener('keyup', ToNextInput);
    }

    for (let i = 0; i < inputValuesUp.length; i++) {

        inputValuesUp[i].addEventListener("click", function (event) {
            value = parseFloat(inputWeightsEl[i].value);
            value = value + 0.05 > 1 ? 1 : value + 0.05;
            inputWeightsEl[i].value = value;
            Calculate();
        });
        
        inputValuesDown[i].addEventListener("click", function (event) {
            value = parseFloat(inputWeightsEl[i].value);
            value = value - 0.05 < -1 ? -1 : value - 0.05;
            inputWeightsEl[i].value = value;
            Calculate();
        });
    }
}

function Sigmoid(x) {
    let k = Math.exp(x);
    return k / (1 + k);
}

