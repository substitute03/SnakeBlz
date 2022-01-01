function addKeyDownEventListener (gameBoardObjectReference) {
    document.addEventListener('keydown', function (event) {
        gameBoardObjectReference.invokeMethodAsync("HandleKeyPress", event.key);
    });
};

function playAudio(elementName) {
    document.getElementById(elementName).play();
}

function showHighScoreModal() {
    var modal = new bootstrap.Modal(document.getElementById('highScoreModal'), {});
    modal.show();
}

function hideHighScoreModal() {
    var modal = new bootstrap.Modal(document.getElementById('highScoreModal'), {});
    modal.hide();
}