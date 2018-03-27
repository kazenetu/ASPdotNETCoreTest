(function () {
    'use strict';

    front.controller.LoginController = function LoginController($location, webApiService, userService) {
        front.common.utils.extendController(this, front.common.controller.PageBase);
        this.setTitle('ログイン');

        var ctrl = this;

        ctrl.userId = "";
        ctrl.password = "";

        // 入力情報のエラークラス
        ctrl.errorUserId = "";
        ctrl.errorPassword = "";

        ctrl.login = function () {
            // 入力チェック
            if (!validateInput()) {
                return;
            }

            webApiService.post('api/account/login', {
                id: ctrl.userId,
                password: ctrl.password
            }, function (response) {
                if (response.result !== "OK") {
                    ctrl.showError(response.errorMessage);
                } else {
                    ctrl.hideError();
                    userService.setId(ctrl.userId);
                    userService.setName(response.responseData.name);
                    $location.path('/main');
                }
            });
        }

        /**
         * DB反映前の入力チェック
         */
        function validateInput() {
            // エラーなし状態に設定
            ctrl.hideError();
            ctrl.errorUserId = '';
            ctrl.errorPassword = '';

            if (ctrl.userId === '') {
                ctrl.showError('E0013', ['ユーザーID']);
                ctrl.errorUserId = 'has-error';
                return false;
            }

            if (ctrl.password === '') {
                ctrl.showError('E0013', ['パスワード']);
                ctrl.errorPassword = 'has-error';
                return false;
            }
            return true;
        }

        /**
         * ページ初期化処理
         */
        ctrl.init = function () {
            webApiService.post('api/account/logout', {
                id: ctrl.userId,
                password: ctrl.password
            }, function (response) {
            });
        }

    }

    // コントローラー定義
    angular.module('App').controller('loginController',['$location', 'webApiService', 'userService', front.controller.LoginController]);
}());
