front.controller.UserListController = function UserListController($q, $location, webApiService, userService, storageService) {
    front.common.utils.extendController(this, front.common.controller.SearchBase);
    this.setTitle('ユーザー検索');

    var ctrl = this;
    ctrl.totalPage = 0;
    ctrl.searchUserId = '';
    ctrl.errorSearchUserId = '';

    /**
     * ページ設定
     */
    var settings = {
        totalPageApiUrl: 'api/user/totalpage',
        getPageApiUrl: 'api/user/page',
        thisPage: '/master/userlist',
        editPage: '/master/useredit',
        getSearchParam: function () {
            return {
                searchUserId: ctrl.searchUserId
            };
        },
        setSearchControls: function (values) {
            ctrl.searchUserId = values.searchUserId;
        }
    };

    /**
     * 戻るボタンクリック
     */
    ctrl.back = function () {
        $location.path('/main');
    }

    /**
     * 新規作成ボタンクリック
     */
    ctrl.create = function () {
        var values = {
            userId: null
        };
        storageService.setValue(storageService.keys.updateKeys, values);
        $location.path(settings.editPage);
    }

    ctrl.json = '';
    ctrl.csvAction = '';
    ctrl.downloadCsv = function (webApiUri) {
        // 入力チェック
        if (!ctrl.validateInput()) {
            return false;
        }

        var params = {
            loginUserId: userService.getId(),
            requestData: getRequestData(0)
        };

        var d = $q.defer();
        d.promise
            .then(function () {
                var deferrred = $q.defer();

                // TODO CSVレコード数の取得
                webApiService.post(settings.totalPageApiUrl, params,
                    function (response) {
                        ctrl.hideError();
                        if (response.responseData < 0) {
                            ctrl.showError(response.errorMessage);
                        }
                        if (response.result === 'NG') {
                            return;
                        }

                        deferrred.resolve();
                    });

                return deferrred.promise;
            })
            .then(function () {
                ctrl.csvAction = webApiUri;
                ctrl.json = JSON.stringify(params);
                setTimeout(function () {
                    $('#csv').trigger('click');
                }, 0);
            });
        // 発火
        d.resolve();
    }

    ctrl.downloadCsvJS = function (webApiUri) {
        // 入力チェック
        if (!ctrl.validateInput()) {
            return false;
        }

        var params = {
            loginUserId: userService.getId(),
            requestData: getRequestData(0)
        };

        var d = $q.defer();
        d.promise
            .then(function () {
                var deferrred = $q.defer();

                // CSVデータ取得
                webApiService.post(webApiUri, params,
                    function (response) {
                        ctrl.hideError();
                        if (response.result === 'NG') {
                            ctrl.showError(response.errorMessage);
                            return;
                        }

                        // CSVファイル作成
                        var bom = new Uint8Array([0xEF, 0xBB, 0xBF]);
                        var content = response.responseData.csv;
                        var fileName = decodeURIComponent(response.responseData.filename);
                        var blob = new Blob([bom, content], { "type": "text/csv" });

                        if (window.navigator.msSaveBlob) {
                            window.navigator.msSaveBlob(blob, fileName);

                            // msSaveOrOpenBlobの場合はファイルを保存せずに開ける
                            window.navigator.msSaveOrOpenBlob(blob, fileName);
                        } else {
                            $('#downloadHref').attr('href', window.URL.createObjectURL(blob));
                            $('#downloadHref').attr('download', fileName);
                            setTimeout(function () {
                                $('#downloadHref')[0].click();
                            }, 0);
                        }
                    });
            });
        // 発火
        d.resolve();
    }

    /**
     * 一覧編集ボタンクリック
     */
    ctrl.edit = function (result) {
        // 主キーの連想配列を変更キーStorageに設定
        var values = {
            userId: result.userID
        };

        storageService.setValue(storageService.keys.updateKeys, values);

        // 編集画面に遷移
        $location.path(settings.editPage);
    }

    /**
     * リクエストデータ取得
     */
    function getRequestData(pageIndex) {
        var requestData = settings.getSearchParam();
        requestData['pageIndex'] = pageIndex;

        // ソート処理
        requestData['sortKey'] = ctrl.sortKey;
        requestData['sortType'] = ctrl.sortType;

        return requestData;
    }

    /**
     * 検索条件Storageの削除
     */
    function clearCondition() {
        storageService.clearValue(storageService.keys.condition);
    }

    /**
     * 検索条件Storageの設定
     */
    function setConditions(pageIndex) {
        var values = getRequestData(pageIndex);
        storageService.setValue(storageService.keys.condition, values);
    }

    /**
     * 検索条件Storageから検索結果を再実行
     */
    function getConditions() {
        var values = storageService.getValue(storageService.keys.condition);

        // 検索条件Storageが設定されていれば検索を行う
        if ('pageIndex' in values) {
            // 検索条件
            settings.setSearchControls(values);

            // ソート処理
            ctrl.sortKey = values.sortKey;
            ctrl.sortType = values.sortType;


            // 検索(ページ指定)
            ctrl.search(values.pageIndex);
        }
    }

    /**
     * ページ初期化処理
     */
    ctrl.init = function () {
        setTimeout(function () {
            var headerTop = $(".fixed-header.clone").position().top;
            var recordTop = $(".scroll-area>.fixed-header").position().top;

            var marginTop = parseInt($(".scroll-area>.fixed-header").css("margin-top"));
            marginTop = (headerTop - recordTop);
            $(".scroll-area>.fixed-header").css("margin-top", marginTop);

            // 検索条件Storageから検索結果を再実行
            getConditions();
        }, 0);
    }

    /**
     * 入力チェック
     */
    ctrl.validateInput = function () {
        // エラークリア
        ctrl.errorSearchUserId = '';

        // 検索条件のチェック
        if (ctrl.searchUserId !== '') {
            if (!ctrl.isNumAlpha(ctrl.searchUserId)) {
                ctrl.showError('E0001', ['ユーザーID', '半角英数字']);
                ctrl.errorSearchUserId = 'has-error'
                return false;
            }
        }
        return true;
    }

    /**
     * 検索処理
     * ボタンクリック時はpageIndexは0固定
     */
    ctrl.search = function (pageIndex) {
        // 検索条件クリア
        clearCondition();

        // 入力チェック
        if (!ctrl.validateInput()) {
            return;
        }

        // 検索条件Storageの設定
        setConditions(pageIndex)

        // 総ページ数の取得
        webApiService.post(settings.totalPageApiUrl, {
            loginUserId: userService.getId(),
            requestData: getRequestData(pageIndex)
        }, function (response) {
            ctrl.totalPage = response.responseData;

            ctrl.hideError();
            if (ctrl.totalPage < 0) {
                ctrl.showError(response.errorMessage);
                ctrl.totalPage = 0;
            }
            if (response.result === 'NG') {
                ctrl.searchResult = null;
                ctrl.totalPage = 0;
                return;
            }

            // 対象ページのレコードを取得
            ctrl.paging(pageIndex, null);

            // 検索条件保存画面の指定(それ以外はヘッダーコントロールで削除)
            var values = [settings.thisPage, settings.editPage];
            storageService.setValue(storageService.keys.enableConditionPaths, values);
        });
    }

    /**
     * 検索ページ変更処理
     */
    ctrl.getPage = function (pageIndex) {
        // 対象ページのレコードを取得
        webApiService.post(settings.getPageApiUrl, {
            loginUserId: userService.getId(),
            requestData: getRequestData(pageIndex)
        }, function (response) {
            // 検索結果のレコードを設定
            ctrl.searchResult = response.responseData;

            // 検索条件Storageの設定
            setConditions(pageIndex)
        });
    }
}

angular.module('App').controller('userListController', front.controller.UserListController);
