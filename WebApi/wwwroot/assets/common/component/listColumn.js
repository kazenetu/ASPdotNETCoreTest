/**
 * 検索結果ソート可能カラム コンポーネント
 */
front.common.component.ListColumnController = function ListColumnController() {
    var ctrl = this;

    /**
     * ロード完了イベント
     */
    ctrl.$onInit = function() {
        // 呼び出し元にインスタンスを登録
        ctrl.onSendRoot({src:ctrl});
    };

    /**
     * ソート変更イベント
     */
    ctrl.changeSort = function(){

        var tempType = ctrl.sortType;
        if(ctrl.key !== ctrl.activeKey){
            tempType = 'ASC';
        }else{
            if(ctrl.sortType === 'ASC'){
                tempType = 'DESC';
            }
            else{
                tempType = 'ASC';
            }
        }

        ctrl.onClick({sortKey:ctrl.key,sortType:tempType});
    }
}

/**
 * コンポーネントを登録
 */
myApp.component("listcolumn", {
    bindings : {
        onSendRoot:'&',
        title : '@',
        key : '@',
        activeKey : '@',
        sortType : '@',
        existRecord : '@',
        onClick : '&'
    },
    templateUrl:'assets/common/component/listColumn.html',
    controller : front.common.component.ListColumnController
});
