/**
 * 検索画面のベース画面
 */
front.common.controller.SearchBase = function SearchBase(){
    front.common.utils.extendController(this,front.common.controller.PageBase);

    var ctrl = this;

    /**
     * 検索結果の初期化
     */
    ctrl.searchResult = null;

    /**
     * ページャーコントロールのインスタンスリスト
     */
    ctrl.childlen = [];

    /**
     * ソート対象のカラムキー
     */
    ctrl.sortKey = '';

    /**
     * ソート順
     */
    ctrl.sortType = '';

    /**
     * 結果一覧のカラムコントロールのインスタンスリスト
     */
    ctrl.columnChildlen = [];


    /**
     * ページコントロールから呼び出し元に自身のインスタンスを登録
     */
    ctrl.sendRoot = function(src) {
        if (ctrl.childlen.indexOf(src) == -1) {
            ctrl.childlen.push(src);
        }
    }

    /**
     * ページング処理
     */
    ctrl.paging = function(pageIndex, sender) {
        ctrl.getPage(pageIndex);

        ctrl.sendPageIndex(pageIndex, sender);
    }

    /**
     * 更新後のページ数をほかのページンコントロールに通知
     */
    ctrl.sendPageIndex = function(pageIndex, sender) {
        var index = 0;
        while (index < ctrl.childlen.length) {
            if (ctrl.childlen[index] !== sender) {
                ctrl.childlen[index].setPageIndex(pageIndex);
            }
            index++;
        }
    }

    /**
     * 検索結果カラムコントロールから呼び出し元に自身のインスタンスを登録
     */
    ctrl.sendColumn = function(src) {
        if (ctrl.columnChildlen.indexOf(src) == -1) {
            ctrl.columnChildlen.push(src);
        }
    }

    /**
     * 検索結果カラムコントロールクリックイベント
     */
    ctrl.clickColumn = function(sortKey,sortType){
        if(ctrl.validateInput !== undefined) {
            if(!ctrl.validateInput()) {
                return;
            }
        }
        ctrl.sortKey = sortKey;
        ctrl.sortType = sortType;
        ctrl.paging(0, null);
    }

}