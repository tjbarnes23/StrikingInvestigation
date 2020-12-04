window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

window.SetFocusToElement = (element) =>
{
    element.focus();
}

window.PlayBellAudio = (audioid) =>
{
    var audio = document.getElementById(audioid);
    audio.play();
}
