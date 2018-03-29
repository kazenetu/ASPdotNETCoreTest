(function () {
  'use strict';

  function LoginAPI(webApiService) {
    /**
     * ログイン処理
     * @param {*} params パラメータ
     * @param {*} callback 成功時に呼ばれるメソッド
     */
    function login(params, callback) {
      webApiService.post('api/account/login', params, callback);
    }

    /**
     * 初期化処理
     * @param {*} params パラメータ
     * @param {*} callback 成功時に呼ばれるメソッド
     */
    function init(params, callback) {
      webApiService.post('api/account/logout', params, callback);
    }

    return {
      init: init,
      login: login
    };
  }

  // ファクトリサービス定義
  angular.module('App').factory('loginAPI', ['webApiService', LoginAPI]);
}());
