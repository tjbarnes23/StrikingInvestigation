window.SetFocusToElement = (element) =>
{
    element.focus();
}

window.PlayBellAudio = (audioid) =>
{
    var audio = document.getElementById(audioid);
    audio.play();
}
