front.controller.PasswordChangeController = function PasswordChangeController($location, webApiService, userService) {
    front.common.utils.extendController(this, front.common.controller.PageBase);
    this.setTitle('パスワード変更');

    var ctrl = this;

    ctrl.passwordChange = function () {
        webApiService.post('api/account/passwordChange', {
            id: userService.getId(),
            password: ctrl.password,
            newPassword: ctrl.newPassword
        }, function (response) {
            if (response.result !== 'OK') {
                ctrl.showError(response.errorMessage);
            } else {
                ctrl.header.hideError();
                $location.path('/main');
            }
        });
    }

    ctrl.cancel = function () {
        $location.path('/main');
    }

}

angular.module('App').controller('passwordChangeController', front.controller.PasswordChangeController);
