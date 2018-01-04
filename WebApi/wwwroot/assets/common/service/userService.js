/**
 * ユーザー情報サービス
 */
front.common.service.UserService = function UserService(){
    this.userId = "";
    this.name = "";

    /**
     * ユーザー情報のクリア
     */
    this.clear = function(){
        this.userId = "";
        this.name = "";
    };

    /**
     * ログインユーザーID設定
     */
    this.setId = function(id){
      this.id = id;
      sessionStorage.setItem("userId",this.id);
    };

    /**
     * ログインユーザーID取得
     */
    this.getId = function(){
      if(isValueNone(this.id)){
        this.id = sessionStorage.getItem("userId");
      }
      return this.id;
    };

    /**
     * ログインユーザー名設定
     */
    this.setName = function(name){
      this.name = name;
      sessionStorage.setItem("userName",this.name);
    };

    /**
     * ログインユーザー名取得
     */
    this.getName = function(){
      if(isValueNone(this.name)){
        this.name = sessionStorage.getItem("userName");
      }
      return this.name;
    };

    /**
     * 値が存在するかチェック
     */
    function isValueNone(value){
        return value===null || value === undefined || value === "";
    }
}

angular.module('userService',[]).service('userService', front.common.service.UserService);
