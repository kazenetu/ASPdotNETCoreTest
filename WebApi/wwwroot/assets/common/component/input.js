/**
 * inputタグ拡張コンポーネント
 */
front.common.component.InputExController = function InputExController() {
    var ctrl = this;

    /**
     * 半角文字のみ許可するか否か
     */
    ctrl.singleByteMode = false;

    /**
     * 最大桁数
     */
    ctrl.maxlengthValue = 1000;

    /**
     * 初期化
     */
    ctrl.$onInit = function(){
        // ime-disabledがclassに設定されていれば、半角入力制限をする
        if(!!ctrl.class && ctrl.class.indexOf('ime-disabled') >= 0){
            ctrl.singleByteMode = true;
        }

        // 最大文字列が設定されていれば設定する
        if(!!ctrl.maxlength){
            ctrl.maxlengthValue = ctrl.maxlength;
        }
    }

    /**
     * 値変更チェック
     */
    ctrl.changeEvent = function() {
        // 入力なし または text以外の場合は処理終了
        if(!ctrl.ngModel || ctrl.type !== 'text'){
            return;
        }

        var result = ctrl.ngModel;

        // 半角文字以外を削除
        if(ctrl.singleByteMode){
            result = result.replace(/[^\s\w]/g,'').replace(/　/g,'');
        }

        // 結果を反映
        ctrl.ngModel = result;
    }

    /**
     * フォーカスロスト
     */
    ctrl.onBlur = function() {
        // 入力あり かつ textの場合は処理終了
        if(!!ctrl.ngModel && ctrl.type === 'text'){
            // 後ろのスペースを除去
            ctrl.ngModel = ctrl.ngModel.replace(/[\s]+$/g,'')
        }

        // フォーカスロストイベント実行
        ctrl.ngBlur();
    }
}

/**
 * コンポーネントを登録
 */
myApp.component("inputex", {
    bindings : {
        type:'@',
        ngModel:'=',
        maxlength:'@',
        class:'@',
        id:'@',
        placeholder:'@',
        ngBlur:'&'
    },
    templateUrl:'assets/common/component/input.html',
    controller : front.common.component.InputExController
});
