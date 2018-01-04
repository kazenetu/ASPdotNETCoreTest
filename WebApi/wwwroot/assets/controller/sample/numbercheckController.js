front.controller.NumbercheckController =  function NumbercheckController($location, webApiService, userService,storageService) {
    front.common.utils.extendController(this, front.common.controller.PageBase);
    this.setTitle('数値入力チェック');

    var ctrl = this;
    ctrl.inputNumber = '';
    ctrl.intPartCount = 3;
    ctrl.decimalPartCount = 0;

    /**
     * 入力チェック
     */
    ctrl.inputCheck = function() {
        // 未入力
        if(ctrl.inputNumber === ''){
            ctrl.showError('未入力エラー');
            return;
        }

        // 数値入力
        if(!ctrl.isNumber(ctrl.inputNumber,
                ctrl.intPartCount, ctrl.decimalPartCount)){
            ctrl.showError('数値エラー');
            return;
        }

        ctrl.hideError();
    }
}

angular.module('App').controller('numbercheckController', front.controller.NumbercheckController);
