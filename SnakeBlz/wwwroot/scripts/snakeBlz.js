// Add a listener when the user presses key. Submit the key to the GameComponent.HandleKeyPress method.

    //document.addEventListener('keydown', function (event) {
    //    // First arg is the AssemblyName (solution name), second is the method, third is the parameter to pass in.
    //    DotNet.invokeMethodAsync("SnakeBlz", "HandleKeyPress", event.key);
    //});


function addKeyDownEventListener (gameBoardObjectReference) {
    document.addEventListener('keydown', function (event) {
        gameBoardObjectReference.invokeMethodAsync("HandleKeyPress", event.key);
    });
};