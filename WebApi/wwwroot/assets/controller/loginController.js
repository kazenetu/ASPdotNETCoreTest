(function () {
    'use strict';

    /**
     * ログイン ビジネスロジック
     * @param {Object} $q
     * @param {Object} $location
     * @param {LoginAPI} loginAPI 
     * @param {UserService} userService 
     */
    function LoginLogic($q, $location, loginAPI, userService) {

        /**
         * ログイン処理
         * @param {Array} params - 入力データ
         * @param {Object} result - 結果データ
         */
        function login(params, result) {
            var d = $q.defer();

            // 発火
            setTimeout(function () {
                d.resolve();
            }, 0);

            return d.promise
                .then(function (reject, resolve) {
                    var deferrred = $q.defer();

                    var result = {
                        result: false,
                        message: ''
                    };

                    loginAPI.login(params,
                        function (response) {
                            if (response.result !== "OK") {
                                result.message = response.errorMessage;

                                resolve = result;
                                reject = result;

                                return deferrred.resolve(result);
                            }
                            else {
                                userService.setId(params.id);
                                userService.setName(response.responseData.name);
                                result.result = true;

                                resolve = result;

                                return deferrred.resolve(result);
                            }
                        });

                    return deferrred.promise;
                });
        };

        return {
            login : login
        }
    }

    // ロジックサービス定義
    angular.module('App').factory('loginLogic', ['$q', '$location', 'loginAPI', 'userService', LoginLogic]);
}());

(function () {
    'use strict';

    front.controller.LoginController = function LoginController($q, $location, loginLogic, loginAPI, userService) {
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

            var params = {
                id: ctrl.userId,
                password: ctrl.password
            };
            var d = loginLogic.login(params);

            d.then(function (result) {
                if (result.result) {
                    ctrl.hideError();
                    $location.path('/main');
                }
                else {
                    ctrl.showError(result.message);
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
            loginAPI.init(
                {
                    id: ctrl.userId,
                    password: ctrl.password
                },
                function (response) {
                }
            );
        }

    }

    // コントローラー定義
    angular.module('App').controller('loginController', ['$q', '$location', 'loginLogic', 'loginAPI', 'userService', front.controller.LoginController]);
}());
