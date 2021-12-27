function addKeyDownEventListener (gameBoardObjectReference) {
    document.addEventListener('keydown', function (event) {
        gameBoardObjectReference.invokeMethodAsync("HandleKeyPress", event.key);
    });
};

function playAudio(elementName) {
    document.getElementById(elementName).play();
}