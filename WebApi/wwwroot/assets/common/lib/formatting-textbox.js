/**
 * @file 書式付テキストボックスクラスを記述するファイル
 *
 * @author kazenetu
 * @version 0.9.0
 * @license MIT license.
 */

(function($){
/**
* 書式付テキストボックス
* @classdesc テキストボックスに書式機能を追加する
* @constructor
* @param {string} format - 書式（例："____-____"）
* @param {Object} option - オプション（inputRegExp、delimiterRegExp）
*/
$.fn.FormattingTextbox = function(format,option){
  /**
  * コンストラクタ
  * @method
  * @name $.fn.FormattingTextbox#FormattingTextbox
  * @param {string} targetId - 対象のテキストボックスのId
  * @param {string} format - 書式（例："____-____"）
  * @param {Object} option - オプション（inputRegExp、delimiterRegExp）
  */
  var  FormattingTextbox= function(targetId,format,option){
  this.targetId = targetId;
  this.format = format;
  this.dataArray  = this.format.split("");

  var defaults = {
    inputRegExp:/\d/
    ,delimiterRegExp:/[-]/
  };
  var option = $.extend(defaults,option);
  this.inputRegExp = option.inputRegExp;
  this.delimiterRegExp = option.delimiterRegExp;

};

  var p=FormattingTextbox.prototype;

/**
 * 入力結果を反映
 * @method
 * @inner
 * @name $.fn.FormattingTextbox#displayText
 */
p.displayText = function(){
  $(this.targetId).val(this.dataArray.toString().replace(/,/g,""));
};

/**
 * 初期化処理
 * @method
 * @inner
 * @name $.fn.FormattingTextbox#init
 */
p.init = function(){
  var instance = this;
  instance.displayText();

  $(instance.targetId).on("keydown",function(e){
    if(e.keyCode === 35 || e.keyCode === 36 || e.keyCode === 9){
      return;
    }
    var isBS = (e.keyCode === 8);
    var isDel = (e.keyCode === 46);

    var inputKeyCode = e.keyCode;
    if(inputKeyCode>=96 && inputKeyCode<=105){
      //テンキー対応
      inputKeyCode = (inputKeyCode-96+48);
    }
    var inputValue = String.fromCharCode(inputKeyCode);
    if(isBS || isDel || instance.inputRegExp.test(inputValue)){
      var startPos = $(this)[0].selectionStart;
      if(isBS===false && startPos >= instance.format.length){
        return;
      }

      //置き換え位置の設定と値の設定
      if(isBS || isDel){
        if(isBS){
          var count = $(this)[0].selectionStart;
          var endPos = $(this)[0].selectionEnd;
          startPos = endPos;
          if(count === endPos) endPos++;
          while(count < endPos){
            startPos--;
            if(startPos<0){
              startPos=0;
            }

            //カーソルの移動
            if(instance.delimiterRegExp.test(instance.dataArray[startPos])){
              startPos--;
              count++;
            }
            //置き換え
            instance.dataArray[startPos] = "_";

            count++;
          }
        }
        if(isDel){
          var count = $(this)[0].selectionStart;
          var endPos = $(this)[0].selectionEnd;
          startPos = count;
          if(count === endPos) endPos++;
          while(count < endPos){
              var index = startPos;
              while(index < instance.format.length-1){
                  if(!instance.delimiterRegExp.test(instance.dataArray[index])){
                    if(instance.delimiterRegExp.test(instance.dataArray[index+1])){
                      instance.dataArray[index] = instance.dataArray[index+2];
                    }else{
                      instance.dataArray[index] = instance.dataArray[index+1];
                    }
                  }
                  index++;
               }
               instance.dataArray[index] = "_";
               count++;
          }
        }
      }else{

        //字送りを行う
        while(startPos < index){
          if(instance.delimiterRegExp.test(instance.dataArray[index])==false){
            if(instance.delimiterRegExp.test(instance.dataArray[index-1])){
              instance.dataArray[index] = instance.dataArray[index-2];
            }else{
              instance.dataArray[index] = instance.dataArray[index-1];
            }
          }

          index--;
        }

        //置き換え
        if(instance.delimiterRegExp.test(instance.dataArray[startPos])){
          startPos++;
        }
        instance.dataArray[startPos] = inputValue;

        //カーソルの移動
        if(startPos < instance.format.length){
          startPos++;
        }
      }

      //表示
      instance.displayText();

      //カーソル位置を設定
      $(this)[0].selectionStart = $(this)[0].selectionEnd = startPos;
    }
    if(e.keyCode!==37 && e.keyCode!==39){
      e.preventDefault();
    }
  });

  $(instance.targetId).on("keypress",function(e){
    e.preventDefault();
  });
  $(instance.targetId).on("keyup",function(e){
    e.preventDefault();
  });

  // データ変更用カスタムイベント
  $(instance.targetId).on("datachange",function(e, data){
    instance.dataArray = data.split('');
    instance.displayText();
  });
};
this.FormattingTextbox = new FormattingTextbox(this.selector,format,option);
this.FormattingTextbox.init();

return (this);
};
})(jQuery);
