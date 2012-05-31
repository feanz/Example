/*!
* Common page functionality
*/
function flashWarning(message) {
    $('#flash').html(message);
    $('#flash').toggleClass("notification-warning");
    $('#flash').delay(1000).fadeIn('slow');
    $('#flash').click(function () { $('#flash').fadeOut('slow'); });
}

function flashInfo(message) {
    $('#flash').html(message);
    $('#flash').toggleClass("info");
    $('#flash').delay(1000).fadeIn('slow');
    $('#flash').click(function () { $('#flash').fadeOut('slow'); });
}