front.controller.MultiController = function MultiController($scope,$q, $location, webApiService, userService,storageService) {
    front.common.utils.extendController(this, front.common.controller.PageBase);
    this.setTitle('複数モデル登録テスト');

    var ctrl = this;
    ctrl.id = "";
    ctrl.Name = "";

    /**
     * 初期化イベント
     */
    ctrl.init = function(){
    }

    /**
     * テーブル登録
     */
    ctrl.insert = function(){

        var d = $q.defer();
        d.promise
        .then(ctrl.showConfirm($q,'確認',
                '登録しますか','登録する'))
        .then(function(){
            var deferrred = $q.defer();

            // データ更新
            webApiService.post('api/multi/insert', {
                loginUserId: userService.getId(),
                requestData : {
                    id : ctrl.id,
                    name : ctrl.name                }
            }, function(response) {
                if (response.result !== 'OK') {
                    ctrl.showError(response.errorMessage);
                } else {
                    ctrl.hideError();

                    deferrred.resolve();
                }
            })

            return deferrred.promise;
        })
        .then(ctrl.showMsgDialog($q,'登録の報告','登録しました', '確認'))
        .then(function(){
            //ctrl.getList();
        });

        // 発火
        d.resolve();
    }
}

angular.module('App').controller('multiController', front.controller.MultiController);
