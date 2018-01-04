front.controller.NoPagerListController =  function NoPagerListController($location,$interval, webApiService, userService,storageService) {
    front.common.utils.extendController(this, front.common.controller.SearchBase);
    this.setTitle('ページャーなし検索サンプル');

    var ctrl = this;
    ctrl.totalPage = 0;
    ctrl.searchUserId = '';

    ctrl.columnId = '';
    ctrl.columnName = '';
    ctrl.columnDate = null;
    ctrl.columnTime = null;

    /**
     * ソート情報の取得
     */
    ctrl.order = function(){
        var result=ctrl.sortKey;
        if(ctrl.sortType !== ''){
            if(ctrl.sortType === 'DESC') {
                result = '-' +result;
            }
        }

        return result;
    }

    /**
     * フィルター設定
     */
    ctrl.filter = function(){
        return {
            id : ctrl.columnId,
            name : ctrl.columnName
        };
    }

    /**
     * セッションからフィルター情報の復元
     */
    function getFilter() {
        var values = storageService.getValue(storageService.keys.searchFilter);
        ctrl.columnId = values.id;
        ctrl.columnName = values.name

    }

    /**
     * セッションにフィルダー情報を設定
     */
    ctrl.changeFilter = function () {
        var values = ctrl.filter();
        storageService.setValue(storageService.keys.searchFilter,values);
    }

    /**
     * データ行のセルクリックでフィルターに設定値を設定
     */
    ctrl.clickCell = function(targetFilter,value) {
        ctrl[targetFilter] = value;
        ctrl.changeFilter();
    }

    /**
     * ページ設定
     */
    var settings = {
        totalPageApiUrl : 'api/user/totalpage',
        getPageApiUrl : 'api/user/pages',
        thisPage : '/sample/nopagerlist',
        editPage : '/master/useredit',
        getSearchParam : function() {
            return {
                searchUserId : ctrl.searchUserId
            };
        },
        setSearchControls : function(values) {
            ctrl.searchUserId = values.searchUserId;
        }
    };

    /**
     * 戻るボタンクリック
     */
    ctrl.back = function(){
        $location.path('/main');
    }

    /**
     * 新規作成ボタンクリック
     */
    ctrl.create = function(){
        var values={
                userId : null
        };
        storageService.setValue(storageService.keys.updateKeys,values);
        $location.path(settings.editPage);
    }

    /**
     * 一覧編集ボタンクリック
     */
    ctrl.edit = function(result){
        // 主キーの連想配列を変更キーStorageに設定
        var values={
                userId : result.id
        };

        storageService.setValue(storageService.keys.updateKeys,values);

        // 編集画面に遷移
        $location.path(settings.editPage);
    }

    /**
     * リクエストデータ取得
     */
    function getRequestData(pageIndex){
        var requestData =settings.getSearchParam();
        requestData['pageIndex'] = pageIndex;

        // ソート処理
        requestData['sortKey'] = ctrl.sortKey;
        requestData['sortType'] = ctrl.sortType;

        return requestData;
    }

    /**
     * 検索条件Storageの削除
     */
    function clearCondition(){
        storageService.clearValue(storageService.keys.condition);
    }

    /**
     * 検索条件Storageの設定
     */
    function setConditions(pageIndex){
        var values = getRequestData(pageIndex);
        storageService.setValue(storageService.keys.condition,values);
    }

    /**
     * 検索条件Storageから検索結果を再実行
     */
    function getConditions(){
        var values = storageService.getValue(storageService.keys.condition);

        // 検索条件Storageが設定されていれば検索を行う
        if('pageIndex' in values){
            // 検索条件
            settings.setSearchControls(values);

            // ソート処理
            ctrl.sortKey = values.sortKey;
            ctrl.sortType = values.sortType;


            // 検索(ページ指定)
            ctrl.search(values.pageIndex);
        }

        // フィルター情報を復元
        getFilter();
    }

    /**
     * ページ初期化処理
     */
    ctrl.init = function() {
        // 検索条件Storageから検索結果を再実行
        getConditions();

        // 一覧の高さ調整
        ctrl.setListHeight();
    }

    /**
     * 検索処理
     * ボタンクリック時はpageIndexは0固定
     */
    ctrl.search = function(pageIndex) {
        // 検索条件クリア
        clearCondition();

        // 総ページ数の取得
        webApiService.post(settings.totalPageApiUrl, {
            loginUserId : userService.getId(),
            requestData : getRequestData(pageIndex)
        }, function(response) {
            ctrl.totalPage = response.responseData;

            ctrl.hideError();
            if(ctrl.totalPage < 0){
                ctrl.showError(response.errorMessage);
                ctrl.totalPage = 0;
            }

            // 対象ページのレコードを取得
            ctrl.getPages(pageIndex);

            // 検索条件保存画面の指定(それ以外はヘッダーコントロールで削除)
            var values = [settings.thisPage, settings.editPage];
            storageService.setValue(storageService.keys.enableConditionPaths,values);
        });
    }

    /**
     * 検索ページ変更処理
     */
    ctrl.getPage = function(pageIndex) {
        // ソート情報を更新
        var values = storageService.getValue(storageService.keys.condition);
        values['sortKey'] = ctrl.sortKey;
        values['sortType'] = ctrl.sortType;
        storageService.setValue(storageService.keys.condition,values);
    }

    /**
     * 検索ページ取得処理
     */
    ctrl.getPages = function(pageIndex) {
        ctrl.searchResult = null;

        // 対象ページのレコードを取得
        webApiService.post(settings.getPageApiUrl, {
            loginUserId : userService.getId(),
            requestData : getRequestData(pageIndex)
        }, function(response) {
            //実行途中のインターバルタイマーを終了する
            if(ctrl.searchResultInterval !== undefined){
                $interval.cancel(ctrl.searchResultInterval);
            }

            // 遅延レンダリング用プロパティを初期化
            ctrl.searchResultInterval = undefined;
            ctrl.searchResultMaxCount = response.responseData.length;
            ctrl.searchResultIndex = 0;
            ctrl.searchResultTemp = [];

            // 表示位置を上に戻す
            $('#sc_target').scrollTop(0);

            // 一覧の高さ調整
            ctrl.setListHeight();

            // 検索条件Storageの設定
            setConditions(pageIndex)

            // ゼロ件の場合は終了
            if(ctrl.searchResultMaxCount <= 0){
                return;
            }

            // 検索結果のレコードを設定
            ctrl.searchResultTemp = response.responseData;

            ctrl.searchResult = [];

            // ファーストビュー
            ctrl.getLines(15);

            // 遅延レンダリングの監視を開始
            ctrl.searchResultInterval = $interval(function(){
                if(ctrl.isScroll) {
                    ctrl.getLines();
                }
                ctrl.isScroll = false;
            },500);
        });
    }

    // 遅延レンダリング用プロパティ
    ctrl.searchResultInterval = undefined;
    ctrl.searchResultIndex = 0;
    ctrl.searchResultMaxCount = 0;
    ctrl.searchResultTemp = [];
    ctrl.isScroll = false;

    /**
     * 検索結果一覧の遅延レンダリング
     */
    ctrl.getLines = function(maxCount){
        // すべての行をレンダリングし終えたら終了する
        if(ctrl.searchResultIndex >= ctrl.searchResultMaxCount){
            $interval.cancel(ctrl.searchResultInterval);
            ctrl.searchResultInterval = undefined;
            return false;
        }
        // 引数が指定されていない場合はデフォルト値を設定
        if(maxCount === undefined || maxCount < 0){
            maxCount = 100;
        }

        // レンダリング
        var count = 0;
        while(count<maxCount && ctrl.searchResultIndex < ctrl.searchResultMaxCount){
            ctrl.searchResult.push(ctrl.searchResultTemp[ctrl.searchResultIndex]);

            count++;
            ctrl.searchResultIndex++;
        }
        return true;
    }

    /**
     * 検索結果一覧遅延レンダリング用縦スクロールイベント
     */
    $('#sc_target').on('scroll',function(){
        var scrollPer = this.scrollTop / (this.scrollHeight - this.clientHeight);
        if(scrollPer > 0.75){
            ctrl.isScroll = true;
        }
    });

    /**
     * 検索結果一覧の高さを調整する
     */
    ctrl.setListHeight = function() {
        setTimeout(function(){
            var listHeight = $(window).height() - $('#list_main')[0].offsetTop-20;
            $('#list_main').css('height', listHeight+"px");
            $('#sc_target').css('max-height', listHeight - $('.clone').height()-10 +"px");
            $('#sc_target').css('margin-top','-8px');
        },0);
    }

    /**
     * ブラウザのリサイズイベント
     */
    $(window).on('resize',function(e){
        // 検索結果一覧の高さを調整する
        ctrl.setListHeight();
    });

    // ダウンロード処理用のID、名前を設定
    ctrl.userId = userService.getId();
    ctrl.userName = userService.getName();
}

angular.module('App').controller('noPagerListController', front.controller.NoPagerListController);
