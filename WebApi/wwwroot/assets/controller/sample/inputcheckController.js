front.controller.InputcheckController =  function InputcheckController($location, webApiService, userService,storageService) {
    front.common.utils.extendController(this, front.common.controller.PageBase);
    this.setTitle('入力チェック');

    var ctrl = this;
    ctrl.inputTest = '';
    ctrl.inputAlpha = '';
    ctrl.inputKana = '';
    ctrl.inputNumber = 0;
    ctrl.inputDate = null;
    ctrl.inputTime = null;
    ctrl.inputMonth =null;

    /**
     * 入力リセット
     */
    ctrl.reset = function() {
        ctrl.inputTest = '';
        ctrl.inputAlpha = '';
        ctrl.inputKana = '';
        ctrl.inputNumber = 0;
        ctrl.inputDate = null;
        ctrl.inputTime = null;
        ctrl.inputMonth =null;
        ctrl.inputSingle = '';
    }

    /**
     * 入力チェック
     */
    ctrl.inputCheck = function() {
        // 未入力
        if(ctrl.inputTest === ''){
            ctrl.showError('未入力エラー');
            return;
        }

        // 最大桁数超過
        if(ctrl.isOverMaxLength(ctrl.inputTest,10)){
            ctrl.showError('最大桁数オーバー');
            return;
        }

        // 英数字チェック
        if(!ctrl.isNumAlpha(ctrl.inputTest)){
            ctrl.showError('英数字エラー');
            return;
        }

        // 英字チェック
        if(ctrl.inputAlpha !== ''){
            if(!ctrl.isAlpha(ctrl.inputAlpha)){
                ctrl.showError('英字エラー');
                return;
            }
        }

        // カタカナチェック
        if(ctrl.inputKana !== ''){
            if(!ctrl.isKana(ctrl.inputKana)){
                ctrl.showError('カタカナエラー');
                return;
            }
        }

        // 日付チェック
        if(ctrl.inputDate !== null) {
            if(!ctrl.isValidDate(ctrl.inputDate)) {
                ctrl.showError('日付エラー');
                return;
            }
        }

        // 時刻チェック
        if(ctrl.inputTime !== null) {
            if(!ctrl.isValidTime(ctrl.inputTime)) {
                ctrl.showError('時刻エラー');
                return;
            }
        }

        // 年月チェック
        if(ctrl.inputMonth !== null) {
            if(!ctrl.isValidDate(ctrl.inputMonth)) {
                ctrl.showError('年月エラー');
                return;
            }
        }

        ctrl.hideError();
    }

    ctrl.clearValue = function(targetName){
        ctrl[targetName] = null;
    }

    ctrl.clearDate = function(id){
        $('#'+id).val(null);
    }

    ctrl.blur = function(name){
        console.log(name+' blur');
    }

    ctrl.inputSingle = '';

    ctrl.singleByteChangeEvent = function(fieldName) {
        if(!this[fieldName]){
            return;
        }

        var debug = '['+ctrl.inputSingle;

        this[fieldName] = this[fieldName].replace(/[^\s\w]/g,'').replace(/　/g,'');

        debug += ']->[' + ctrl.inputSingle+']';
        console.log(debug);
    }
}

angular.module('App').controller('inputcheckController', front.controller.InputcheckController);
