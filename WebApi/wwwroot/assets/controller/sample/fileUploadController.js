front.controller.FileUploadController =  function FileUploadController($location,$q, webApiService, userService,storageService,$http) {
    front.common.utils.extendController(this, front.common.controller.PageBase);
    this.setTitle('ファイルアップロード');

    var ctrl = this;
    ctrl.fileBase64 = '';

    var converBase64 = function(file,callback) {
        var fileData = new FormData();
        fileData.append('file',file);

        //post
        $http.post('./api/fileupload/convert/base64',fileData,{
            transformRequest: null,
            headers: {'Content-type':undefined}
        })
        .then(function(res){
            ctrl.fileBase64 = res.data;

            callback();
        });
    }

    ctrl.submit = function(){
        var fileTag = document.getElementById("file");
        if(file.files.length > 0){
            converBase64(file.files[0],
                function(){
                    var d = $q.defer();
                    d.promise
                    .then(function(){
                        var deferrred = $q.defer();

                        // データ更新
                        webApiService.post('./api/fileupload/upload', {
                            loginUserId: userService.getId(),
                            requestData : {
                                fileName:file.files[0].name,
                                fileData:ctrl.fileBase64
                            }
                        }, function(response) {
                            if (response.result !== 'OK') {
                                ctrl.showError(response.errorMessage);
                            } else {
                                ctrl.hideError();

                                deferrred.resolve();
                            }
                        });

                        return deferrred.promise;
                    });

                    // 発火
                    d.resolve();
                }
            );
        }

        return false;
    }
}

angular.module('App').controller('fileUploadController', front.controller.FileUploadController);
