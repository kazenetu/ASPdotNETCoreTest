(function () {
    'use strict';

    front.controller.MainController = function MainController($location, webApiService, userService, storageService) {
        front.common.utils.extendController(this, front.common.controller.PageBase);
        this.setTitle('メニュー');

        var ctrl = this;

        ctrl.gotoPage = function (pageUrl) {
            $location.path(pageUrl);
        }

    }

    // コントローラー定義
    angular.module('App').controller('mainController', ['$location', 'webApiService', 'userService', front.controller.MainController]);
}());
