/**
 * ページベース
 */
front.common.controller.PageBase = function PageBase(){
    var ctrl = this;

    ctrl.title = '';
    ctrl.header = null;

    /**
     * ヘッダーコントロールから呼び出し元に自身のインスタンスを登録
     */
    ctrl.sendRootHeader = function(src){
        ctrl.header = src;

        // タイトル設定
        ctrl.header.setTitle(ctrl.title);
    }

    /**
     * エラーメッセージの表示
     */
    ctrl.showError = function(message, params){
        if(ctrl.header === null) return;

        // メッセージ取得
        var argParam = params;
        if(!params){
            argParam = [];
        }
        var formattedMessage = getMessage(message, argParam);
        ctrl.header.showError(formattedMessage);
    }

    /**
     * エラーメッセージの非表示
     */
    ctrl.hideError = function(){
        if(ctrl.header === null) return;

        ctrl.header.hideError();
    }

    /**
     * タイトル設定
     */
    ctrl.setTitle = function(title){
        ctrl.title = title;
        document.title = title;
    }

    /**
     * 確認メッセージの表示
     */
    ctrl.showConfirm = function($q,title,message,buttonText,params){
        if(ctrl.header === null) return;

        return function(){
            var deferrred = $q.defer();

            // メッセージ取得
            var argParam = params;
            if(!params){
                argParam = [];
            }
            var formattedMessage = getMessage(message, argParam);

            // タイトル取得
            var formattedTitle = getMessage(title, []);

            ctrl.header.showConfirm(formattedTitle,formattedMessage,buttonText,function(){
                deferrred.resolve();
            });

            return deferrred.promise;
        }
    }

    /**
     * メッセージダイアログの表示
     */
    ctrl.showMsgDialog = function($q,title,message,buttonText,params){
        if(ctrl.header === null) return;

        return function(){
            var deferrred = $q.defer();

            // メッセージ取得
            var argParam = params;
            if(!params){
                argParam = [];
            }
            var formattedMessage = getMessage(message, argParam);

            // タイトル取得
            var formattedTitle = getMessage(title, []);

            ctrl.header.showMsgDialog(formattedTitle,formattedMessage,buttonText,function(){
                deferrred.resolve();
            });

            return deferrred.promise;
        }
    }

    /**
     * メッセージ取得
     */
    function getMessage(messageId,params){
        var message = messageId;
        if(messageId in front.common.messages){
            var message = front.common.messages[messageId];

            var index=0;
            while(index < params.length){
                message = message.replace('{'+index+'}', params[index]);
                index++;
            }
        }
        return message;
    }

    /**
     * 最大桁数超過チェック
     */
    ctrl.isOverMaxLength = function(src, maxLength) {
        if(!src){
            return false;
        }
        return src.toString().length > maxLength;
    }

    /**
     * 英数字チェック
     */
    ctrl.isNumAlpha = function(src) {
        if(!src){
            return false;
        }
        return /^[0-9A-Za-z]+$/.test(src);
    }

    /**
     * 英字チェック
     */
    ctrl.isAlpha = function(src) {
        if(!src){
            return false;
        }
        return /^[A-Za-z]+$/.test(src);
    }

    /**
     * カタカナチェック
     */
    ctrl.isKana = function(src) {
        if(!src){
            return false;
        }
        return /^[ｱ-ﾝ|ﾞｰ|ァ-ンヴー| |　]+$/.test(src);
    }

    /**
     * 数値チェック
     *  @param {string} {src} - 入力値
     *  @param {number} {intPartCount} - 整数部の桁数
     *  @param {number} [decimalPartCount=0] - 小数部の桁数(省略可能)
     */
    ctrl.isNumber = function(src, intPartCount, decimalPartCount) {
        if(src === null || src === undefined) {
            return false;
        }

        if(decimalPartCount === null || decimalPartCount === undefined) {
            decimalPartCount = 0;
        }
        var srcArray = src.toString().split('.');
        var srcIntPart = '0';
        var srcDecimalPart = '';
        if(/^[0-9]+[.][0-9]+$/.test(src)) {
            srcIntPart = srcArray[0];
            srcDecimalPart = srcArray[1];
        }
        else {
            if(/[.]$/.test(src)) {
                srcDecimalPart = srcArray[0];
            }
            else{
                srcIntPart = srcArray[0];
            }
        }
        // 整数部はいったん数値に変換してゼロを除去
        srcIntPart = parseInt(srcIntPart,10).toString();

        var regString = '';

        // 整数部
        if(intPartCount > 1){
            regString = '[1-9]?';
            intPartCount -= 1;
        }
        regString += '[0-9]{1,' + intPartCount + '}';
        if(!(new RegExp('^'+ regString + '$').test(srcIntPart))) {
            return false;
        }

        // 小数部
        if(srcDecimalPart.length > 0){
            if(decimalPartCount > 0){
                regString = '[0-9]{0,' + decimalPartCount + '}';
                if(!(new RegExp('^'+ regString + '$').test(srcDecimalPart))){
                    return false;
                }
            }
            else {
                return false;
            }
        }

        return true;
    }

    /**
     * 日付が有効か否か
     */
    ctrl.isValidDate = function(date) {
        if(date === null || date === undefined) {
            return false;
        }
        return true;
    }

    /**
     * Dateから文字列(yyyyMMdd)に変換
     */
    ctrl.dateToString = function(src) {
        var type = Object.prototype.toString.call(src).slice(8, -1).toLowerCase();
        if(type === 'date'){
            return ('0'+String(src.getFullYear())).slice(-4) +
                    ('0'+(src.getMonth()+1)).slice(-2) +
                    ('0'+src.getDate()).slice(-2);
        }

        return '';
    }

    /**
     * 文字列(yyyyMMdd)からDateに変換
     */
    ctrl.stringToDate = function(src) {
        var type = Object.prototype.toString.call(src).slice(8, -1).toLowerCase();
        if(type === 'string' && src !== ''){
            var year  = parseInt(src.substr(0,4),10);
            var month = parseInt(src.substr(4,2),10) - 1;
            var day   = parseInt(src.substr(6,2),10);
            return new Date(year, month, day);
        }

        return null;
    }

    /**
     * 時刻が有効か否か
     */
    ctrl.isValidTime = function(time) {
        if(time === null || time === undefined) {
            return false;
        }
        return true;
    }

    /**
     * Date(Time)から文字列(hhmmss)に変換
     */
    ctrl.timeToString = function(src) {
        var type = Object.prototype.toString.call(src).slice(8, -1).toLowerCase();
        if(type === 'date'){
            return ('0'+(src.getHours())).slice(-2) + ('0'+src.getMinutes()).slice(-2) + ('0'+src.getSeconds()).slice(-2);
        }

        return '';
    }

    /**
     * 文字列(hhmmss)からDate(Time)に変換
     */
    ctrl.stringToTime = function(src) {
        var type = Object.prototype.toString.call(src).slice(8, -1).toLowerCase();
        if(type === 'string' && src !== ''){
            var hour  = parseInt(src.substr(0,2),10);
            var minutes = parseInt(src.substr(2,2),10);
            var second = parseInt(src.substr(4,2),10);
            return new Date(1970,1,1,hour,minutes,second);
        }

        return null;
    }

}