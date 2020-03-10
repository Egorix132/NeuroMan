var inputValues = document.getElementsByClassName("input-neuron-value")
var outputValues = document.getElementsByClassName("output-neuron-value")
var inputWeights = document.getElementsByClassName("i");
var outputWeights = document.getElementsByClassName("o");

var MainValue = 0;

UpdateValues();

for (let i = 0; i < inputWeights.length; i++) {
    inputWeights[i].addEventListener("input", function (event) {
        UpdateValues();
    })
};

for (let i = 0; i < outputWeights.length; i++) {
    outputWeights[i].addEventListener("input", function (event) {
        UpdateValues();
    })
};

function UpdateValues() {
    MainValue = 0;
    for (let i = 0; i < inputWeights.length; i++) {
        MainValue += inputWeights[i].value * (parseInt(inputValues[i].textContent));
    };

    document.getElementsByClassName("main-value")[0].textContent = MainValue.toString().slice(0,5);

    for (let i = 0; i < outputWeights.length; i++) {
        outputValues[i].textContent = (outputWeights[i].value * MainValue).toString().slice(0,5);
    };
}