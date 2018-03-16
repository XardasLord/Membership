$(function () {
    var registerUserCheckBox = $('#AcceptUserAgreement').click(onToogleRegisterUserDisableClick);

    function onToogleRegisterUserDisableClick() {
        $('.register-user-panel button').toggleClass('disabled');
    }
});