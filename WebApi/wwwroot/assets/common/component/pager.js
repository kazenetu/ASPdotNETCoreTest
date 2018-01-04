/**
 * ページャーコンポーネント
 */
front.common.component.PagerController = function PagerController() {
    var ctrl = this;

    /**
     * 現在ページ
     */
    ctrl.pageIndex = 0;


    ctrl.getPages = function() {
        var lastPageIndex = ctrl.totalPage-1;
        var pages= [];

        var index = ctrl.pageIndex-2;
        if(index < 0){
            index = 0;
        }
        else{
            if(ctrl.pageIndex >= lastPageIndex-2) {
                index = lastPageIndex-4;
                if(index < 0){
                    index = 0;
                }
            }
        }

        for(var i=0;i<5;i++){
            pages.push(index+i);
            if(index+i >= lastPageIndex){
                i = 5;
            }
        }

        return pages;
    }


    /**
     * 表示用現在ページ
     */
    ctrl.indexlPage = function() {
        return parseInt(ctrl.pageIndex,10)+1;
    }

    /**
     * 前ページボタンクリック
     */
    ctrl.prev = function() {
        if (ctrl.isEnabledPrev()) {
            ctrl.pageIndex --;
            ctrl.onPaging({pageIndex:ctrl.pageIndex,sender:ctrl });
        }
    }

    /**
     * ページリンク クリック
     */
    ctrl.selectPage = function(pageIndex){
        ctrl.pageIndex= pageIndex;
        ctrl.onPaging({pageIndex:ctrl.pageIndex,sender:ctrl });
    }

    /**
     * 次ページボタンクリック
     */
    ctrl.next = function() {
        if (ctrl.isEnabledNext()) {
            ctrl.pageIndex ++;
            ctrl.onPaging({pageIndex:ctrl.pageIndex,sender:ctrl });
        }
    }

    /**
     * 前ページボタン有効状態取得
     */
    ctrl.isEnabledPrev = function(){
        return ctrl.pageIndex > 0;
    }

    /**
     * 次ページボタン有効状態取得
     */
    ctrl.isEnabledNext = function(){
        return ctrl.pageIndex  < ctrl.totalPage - 1;
    }

    /**
     * ロード完了イベント
     */
    ctrl.$onInit = function() {
        // 呼び出し元にインスタンスを登録
        ctrl.onSendRoot({src:ctrl});
    };

    /**
     * 現在ページ変更イベント
     * （呼び出しもとから呼び出される）
     */
    ctrl.setPageIndex = function(index){
        ctrl.pageIndex = index;
    }

}

/**
 * コンポーネントを登録
 */
myApp.component("pager", {
    bindings : {
        totalPage : '@',
        onPaging: '&',
        onSendRoot:'&'
    },
    templateUrl:'assets/common/component/pager.html',
    controller : front.common.component.PagerController
});
