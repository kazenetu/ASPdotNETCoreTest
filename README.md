# ASPdotNETCoreTest
[https://github.com/kazenetu/dotNETCoreTest](https://github.com/kazenetu/dotNETCoreTest) をベースとしたASP.NET Coreのテスト

# 発行方法
下記コマンドを発行する  
* CentOS7  
`dotnet publish --configuration Release  -r centos.7-x64 -o publish`
* Windows  
`dotnet publish --configuration Release  -r win-x64 -o publish`  
`dotnet publish --configuration Release  -r win-x86 -o publish`

# TODO
- [X] パスワードのハッシュ化
- [X] CSV出力対象カラムの設定機能
- [X] CSVダウンロード
- [ ] ファイルアップロード  
  - [ ] 容量制限の確認  
    [kazenetu/dotNETCoreTest：ファイルアップロードの例](https://github.com/kazenetu/dotNETCoreTest/blob/master/WebApp/WebApiSample/Controllers/UsersController.cs#L198-L256)
- [ ] PDFダウンロード  
  - [ ] パッケージ決定  
    - [DinkToPdf https://www.nuget.org/packages/DinkToPdf/](https://www.nuget.org/packages/DinkToPdf/)
    - [Windowsで始める仮想サーバー その5 「ASP.NET CoreでPDF出力」](https://github.com/kazenetu/blog-reports/tree/master/reports/20-dotnetTestCentOS5)
 
 ## 参考URL
 1.  パスワードのハッシュ化
     1. [パスワードのハッシュ:Microsoft](https://docs.microsoft.com/ja-jp/aspnet/core/security/data-protection/consumer-apis/password-hashing)
