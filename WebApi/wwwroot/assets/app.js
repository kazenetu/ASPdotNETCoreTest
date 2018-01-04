var myApp = angular.module('App', [ 'ngResource', 'ngRoute', 'ngSanitize', 'fixed.table.header', 'webApiService','storageService','userService' ]);

myApp.config([ '$routeProvider', function($routeProvider) {
    $routeProvider.when('/', {
        templateUrl : 'view/login.html'
    }).when('/passwordChange', {
        templateUrl : 'view/passwordChange.html',
    }).when('/main', {
        templateUrl : 'view/main.html',
    }).when('/master/userlist', {
        templateUrl : 'view/master/userList.html',
    }).when('/master/useredit', {
        templateUrl : 'view/master/userEdit.html',

    }).when('/sample/upload', {
        templateUrl : 'view/sample/upload.html',
    }).when('/sample/nopagerlist', {
        templateUrl : 'view/sample/noPagerList.html',
    }).when('/sample/inputcheck', {
        templateUrl : 'view/sample/inputcheck.html',
    }).when('/sample/numbercheck', {
        templateUrl : 'view/sample/numbercheck.html',
    }).when('/sample/multi', {
        templateUrl : 'view/sample/multi.html',
    }).when('/sample/fileupload', {
        templateUrl : 'view/sample/fileupload.html',

    }).otherwise({
        redirectTo : '/'
    });

    // ルートURL退避
    sessionStorage.setItem("rootURL",location.protocol + '//' + location.host + '/web/');
}
]);

// 名前空間を作成
var front = {
    common:{
        component:{},
        controller:{},
        service:{},
        utils:{},
        messages:{}
    },
    controller:{},
};