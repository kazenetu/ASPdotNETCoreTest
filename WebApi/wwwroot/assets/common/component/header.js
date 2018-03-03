/**
 * ヘッダーコンポーネント
 */
front.common.component.HeaderController = function HeaderController($location, userService, webApiService, storageService) {
    var ctrl = this;

    /**
     * すべて表示かタイトルだけ表示か
     */
    ctrl.isAllDisplay = true;

    ctrl.title = '';
    ctrl.isError = false;
    ctrl.errorMsg = '';

    // ダイアログ系
    ctrl.isConfirmMode = true;
    ctrl.dialogTitle = '';
    ctrl.dialogMessage = '';
    ctrl.dialogCommitText = '';

    /**
     * 確定ボタンクリックで呼ばれるコールバック関数
     */
    var dialogCallback = null;

    /**
     * メッセージダイアログの表示
     */
    ctrl.showMsgDialog = function(title,message,buttonText,callFunction){
        ctrl.isConfirmMode = false;
        ctrl.dialogTitle = title;
        ctrl.dialogMessage = message;
        ctrl.dialogCommitText = buttonText;
        $('#confirm').modal('show');
        dialogCallback = callFunction;
    }

    ctrl.userName = function() {
        return userService.getName();
    };

    /**
     * ロード完了イベント
     */
    ctrl.$onInit = function() {
        // 呼び出し元にインスタンスを登録
        ctrl.onSendRoot({src:ctrl});

        if(ctrl.titleOnly === 'true'){
            ctrl.isAllDisplay = false;
        }

        // 検索結果表示許可画面以外の場合は検索条件をクリア
        var paths = storageService.getValue(storageService.keys.enableConditionPaths);
        var currentPath = $location.path();
        if(Array.isArray(paths) && paths.indexOf(currentPath) < 0){
            storageService.clearValue(storageService.keys.condition);
            storageService.clearValue(storageService.keys.searchFilter);
        }

        var buttons = $(".buttons-row:first").children(".btn:first");
        if(buttons.length > 0){
            buttons.after('<a href="index.html#!/main" class="btn btn-primary" style="margin-left:1em;">メニュー</a>');
        }

        // メッセージjsonを取得する
        if(JSON.stringify(front.common.messages).length <= 2){
            webApiService.get('message.json', {} , function(response) {
                front.common.messages = response;
            });
        }

        $('.modal-backdrop').remove();
    };

    /**
     * ログアウト
     */
    ctrl.logout = function() {
        // ユーザーデータ取得
        webApiService.post('api/account/logout', {
        }, function(response) {
            userService.clear();
            storageService.clearAllValues();
            $location.path('/');
        });
    };

    /**
     * エラーメッセージ表示
     */
    ctrl.showError = function(message) {
        ctrl.isError = true;
        ctrl.errorMsg = message;
    }

    /**
     * エラーメッセージ非表示
     */
    ctrl.hideError = function() {
        ctrl.isError = false;
        ctrl.errorMsg = '';
    }

    ctrl.setTitle = function(title) {
        ctrl.title = title;
    }

    /**
     * 決定ボタンクリックイベント
     */
    ctrl.dialogCommitClick = function(){
        if(dialogCallback !== null){
            $('#confirm').on('hidden.bs.modal',function(){
                $('#confirm').off('hidden.bs.modal');
                dialogCallback();
            });
            $('#confirm').modal('hide');
        }
    }

    /**
     * 確認メッセージの表示
     */
    ctrl.showConfirm = function(title,message,buttonText,callFunction){
        ctrl.isConfirmMode = true;
        ctrl.dialogTitle = title;
        ctrl.dialogMessage = message;
        ctrl.dialogCommitText = buttonText;
        $('#confirm').modal({backdrop:'static',keybord:false,show:true});
        dialogCallback = callFunction;
    }
}

/**
 * コンポーネントを登録
 */
myApp.component("header", {
    bindings : {
        onSendRoot:'&',
        titleOnly:'@'
    },
    templateUrl:'assets/common/component/header.html',
    controller : front.common.component.HeaderController
});
