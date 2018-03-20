$(function () {
    var loginLinkHover = $("#loginLink").hover(onLoginLinkHover);
    var loginCloseButto = $("#close-login").click(onCloseLogin);

    function onLoginLinkHover() {
        $("div[data-login-user-area]").addClass('open');
    }

    function onCloseLogin() {
        $("div[data-login-user-area]").removeClass('open');
    }
});