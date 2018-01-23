using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;
using Domain.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using static Domain.Service.ServiceBase;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Web;
using WebApi.Utilities;

namespace WebApi.Controllers
{
  /// <summary>
  /// ユーザー系コントローラークラス
  /// </summary>
  [Route("api/user")]
  public class UsersController : ControllerBase
  {
    #region プライベートフィールド

    /// <summary>
    /// サービスインスタンス
    /// </summary>
    private readonly IUserService service;

    /// <summary>
    /// ログインスタンス
    /// </summary>
    private readonly ILogger logger;

    #endregion

    #region プライベート定数フィールド
    private static string ErrorLoginNG = "ログイン失敗";
    private static string ErrorPasswordNG = "パスワード失敗";
    private static string SearchResultZero = "検索結果ゼロ件";
    private static string ErrorNotFound = "データが見つかりません";
    private static string ErrorSave = "{0}が失敗しました";
    private static string ErrorVersion = "ほかの人が更新しています。再検索してください";
    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="service">サービスインスタンス</param>
    /// <param name="logger">ログインスタンス</param>
    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
      this.service = service;
      this.logger = logger;
    }

    #endregion

    #region パブリックメソッド

    #region ログイン・ログアウト

    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/login</remarks>
    [HttpPost("login")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Login([FromBody]Dictionary<string, object> param)
    {
      var paramNameUserId = "id";
      var paramNamePassword = "password";

      // 入力チェック
      if (!param.ContainsKey(paramNameUserId))
      {
        logger.LogError("Pram[{0}]が未設定", paramNameUserId);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNamePassword))
      {
        logger.LogError("Pram[{0}]が未設定", paramNamePassword);
        return BadRequest();
      }

      var userId = param[paramNameUserId].ToString();
      var password = param[paramNamePassword].ToString();

      var data = new Dictionary<string, object>();
      var serviceResult = false;
      try
      {
        var passwordHash = string.Empty;

        var model = service.Find(userId);
        if (model != null && model.EntryDate.HasValue)
        {
          // パスワードのハッシュ取得
          passwordHash = HashUtility.Create(model.UserID,password,model.EntryDate.Value);
        }

        // ログインチェック
        if (service.Login(userId, passwordHash) != null)
        {
          // セッション破棄
          refreshSession();

          // セッションキー設定
          session.SetString(SessionKeyUserID, userId);

          serviceResult = true;
          data.Add("name", model.UserName);
        }
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult)
      {
        result.Add("result", "OK");
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", ErrorLoginNG);
      }
      result.Add("responseData", data);

      return Json(result);
    }

    /// <summary>
    /// ログアウト
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/logout</remarks>
    [HttpPost("logout")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Logout([FromBody]Dictionary<string, object> param)
    {

      // セッション破棄
      HttpContext.Response.Cookies.Delete(ControllerBase.SessionCookieName);
      session.Clear();

      var result = new Dictionary<string, object>();
      result.Add("result", "OK");

      return Json(result);
    }
    #endregion

    #region 検索系

    /// <summary>
    /// 検索：ページ数取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/totalpage</remarks>
    [HttpPost("totalpage")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Totalpage([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if (param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName = string.Empty;

        // パラメータの設定
        paramName = "searchUserId";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SearchUserId = requestData[paramName].ToString();
        }
        paramName = "pageIndex";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.PageIndex = (int)requestData[paramName];
        }
        paramName = "sortKey";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SortKey = requestData[paramName].ToString();
        }
        paramName = "sortType";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SortType = requestData[paramName].ToString();
        }
      }

      var serviceResult = 0;
      try
      {
        serviceResult = service.GetPageCount(searchCondition);
      }
      catch (Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult >= 0)
      {
        result.Add("result", "OK");
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", SearchResultZero);
      }
      result.Add("responseData", serviceResult);

      return Json(result);
    }

    /// <summary>
    /// 検索：対象ページのレコード取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/page</remarks>
    [HttpPost("page")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Page([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if (param.ContainsKey("requestData"))
      {
        // パラメータの設定
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
         searchCondition = getUserSearchCondition(requestData);
      }

      var serviceResult = new List<UserModel>();
      try
      {
        serviceResult.AddRange(service.GetUsers(searchCondition));
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult.Any())
      {
        result.Add("result", "OK");
        result.Add("responseData", serviceResult);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", SearchResultZero);
      }

      return Json(result);
    }

    /// <summary>
    /// 対象マスタを取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/find</remarks>
    [HttpPost("find")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Find([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      var userId = string.Empty;

      if (param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName = string.Empty;

        // パラメータの設定
        paramName = "id";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          userId = requestData[paramName].ToString();
        }
      }

      // 入力チェック
      if (string.IsNullOrEmpty(userId))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(userId));
        return BadRequest();
      }

      UserModel serviceResult = null;
      try
      {
        serviceResult = service.Find(userId);
      }
      catch (Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult != null)
      {
        result.Add("result", "OK");
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", ErrorNotFound);
      }
      result.Add("responseData", serviceResult);

      return Json(result);
    }

#endregion

    #region 更新系

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/passwordChange</remarks>
    [HttpPost("passwordChange")]
    [AutoValidateAntiforgeryToken]
    public IActionResult PasswordChange([FromBody]Dictionary<string, object> param)
    {
      var paramNameUserId = "id";
      var paramNamePassword = "password";
      var paramNameNewPassword = "newPassword";

      // 入力チェック
      if (!param.ContainsKey(paramNameUserId))
      {
        logger.LogError("Pram[{0}]が未設定", paramNameUserId);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNamePassword))
      {
        logger.LogError("Pram[{0}]が未設定", paramNamePassword);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNameNewPassword))
      {
        logger.LogError("Pram[{0}]が未設定", paramNameNewPassword);
        return BadRequest();
      }

      var userId = param[paramNameUserId].ToString();
      var password = param[paramNamePassword].ToString();
      var newPassword = param[paramNameNewPassword].ToString();

      var data = new Dictionary<string, object>();
      var serviceResult = UpdateResult.Error;
      try
      {
        var model = service.Find(userId);
        if (model != null && model.EntryDate.HasValue)
        {
          // パスワードのハッシュ取得
          var passwordHash = HashUtility.Create(model.UserID, password, model.EntryDate.Value);
          var newPasswordHash = HashUtility.Create(model.UserID, newPassword, model.EntryDate.Value);

          // パスワード変更
          serviceResult = service.ChangePassword(userId, passwordHash, newPasswordHash);
        }
        
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult == UpdateResult.OK)
      {
        result.Add("result", "OK");
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", ErrorPasswordNG);
      }
      result.Add("responseData", data);

      return Json(result);
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/insert</remarks>
    [HttpPost("insert")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Insert([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      // model取得
      var model = createModel(param);

      // 入力チェック
      if (string.IsNullOrEmpty(model.UserID))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(model.UserID));
        return BadRequest();
      }
      if (string.IsNullOrEmpty(model.Password))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(model.Password));
        return BadRequest();
      }

      var serviceResult = UpdateResult.Error;
      try
      {
        // 現在時刻を設定
        var entryDate = DateTime.Now;

        // パスワードのハッシュ取得
        var passwordHash = HashUtility.Create(model.UserID, model.Password, entryDate);
        
        // データ保存
        serviceResult = service.Save(model, getLoginUserId(param), passwordHash, entryDate);
      }
      catch (Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult == UpdateResult.OK)
      {
        result.Add("result", "OK");
        result.Add("responseData", string.Empty);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", string.Format(ErrorSave,"登録"));
      }

      return Json(result);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/update</remarks>
    [HttpPost("update")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Update([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      // model取得
      var model = createModel(param);

      // 入力チェック
      if (string.IsNullOrEmpty(model.UserID))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(model.UserID));
        return BadRequest();
      }

      var serviceResult = UpdateResult.Error;
      try
      {
        // 更新前のレコード取得
        var dbModel = service.Find(model.UserID);
        if (dbModel != null && dbModel.EntryDate.HasValue)
        {
          var passwordHash = string.Empty;

          // パスワードが設定されていない場合はDBの値を設定する
          if(string.IsNullOrEmpty(model.Password))
          {
            passwordHash  = dbModel.Password;
          }
          else
          {
            // パスワードのハッシュ取得
            passwordHash = HashUtility.Create(dbModel.UserID, model.Password, dbModel.EntryDate.Value);
          }

          // データ更新
          serviceResult = service.Save(model, getLoginUserId(param), passwordHash);
        }

      }
      catch (Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult == UpdateResult.OK)
      {
        result.Add("result", "OK");
        result.Add("responseData", null);
      }
      else
      {
        result.Add("result", "NG");
        if(serviceResult == UpdateResult.ErrorVaersion){
          result.Add("errorMessage", ErrorVersion);
        }
        else{
          result.Add("errorMessage", string.Format(ErrorSave,"更新"));
        }
      }

      return Json(result);
    }

#endregion

    #region  CSVダウンロード系

    /// <summary>
    /// CSVダウンロード
    /// </summary>
    /// <param name="json">入力情報のJSOｎデータ</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/download</remarks>
    [HttpPost("download")]
    public ActionResult Download(string json)
    {
      var csvData = new System.Text.StringBuilder();

      try
      {
        if (!string.IsNullOrEmpty(json))
        {
          var param = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

          // ログインチェック
          if (!isLogin(param))
          {
            return LocalRedirect("/");
          }

          // 検索条件の存在確認
          if (param.ContainsKey("requestData"))
          {
            var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSearchCondition>(param["requestData"].ToString());

            // データ取得とCSV文字列取得
            var models = service.GetAllUsers(requestData);
            foreach (var model in models)
            {
              csvData.AppendLine(model.GetCSV());
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return LocalRedirect("/");
      }

      // サンプルのファイル名
      string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
      fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);

      // ファイル名を設定
      Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

      return Content(csvData.ToString(), "text/csv");
    }

    /// <summary>
    /// ヘッダー付CSVダウンロード
    /// </summary>
    /// <param name="json">入力情報のJSOｎデータ</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/download</remarks>
    [HttpPost("downloadHeaderCSV")]
    public ActionResult DownloadHeaderCSV(string json)
    {
      var csvData = new System.Text.StringBuilder();

      try
      {
        if (!string.IsNullOrEmpty(json))
        {
          var param = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

          // ログインチェック
          if (!isLogin(param))
          {
            return LocalRedirect("/");
          }

          // 検索条件の存在確認
          if (param.ContainsKey("requestData"))
          {
            var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSearchCondition>(param["requestData"].ToString());

            // データ取得とCSV文字列取得
            var isFirstRecord = true;
            var models = service.GetAllUsers(requestData);
            foreach (var model in models)
            {
              csvData.AppendLine(model.GetCSV(isFirstRecord));
              isFirstRecord = false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return LocalRedirect("/");
      }

      // サンプルのファイル名
      string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
      fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);

      // ファイル名を設定
      Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

      return Content(csvData.ToString(), "text/csv");
    }

    /// <summary>
    /// CSVダウンロード：JSで作成
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/download</remarks>
    [HttpPost("downloadJS")]
    [AutoValidateAntiforgeryToken]
    public IActionResult DownloadJS([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if (param.ContainsKey("requestData"))
      {
        // パラメータの設定
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
         searchCondition = getUserSearchCondition(requestData);
      }

      var csvData = new System.Text.StringBuilder();

      try
      {
        // データ取得とCSV文字列取得
        var models = service.GetAllUsers(searchCondition);
        foreach (var model in models)
        {
          csvData.AppendLine(model.GetCSV());
        }
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (csvData.Length > 0)
      {
        result.Add("result", "OK");

        var data = new Dictionary<string,string>();
        data.Add("csv",csvData.ToString());

        // サンプルのファイル名
        string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
        fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
        data.Add("filename",fileName);

        result.Add("responseData", data);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", SearchResultZero);
      }

      return Json(result);
    }

    /// <summary>
    /// ヘッダー付CSVダウンロード：JSで作成
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/downloadHeaderCSVJS</remarks>
    [HttpPost("downloadHeaderCSVJS")]
    [AutoValidateAntiforgeryToken]
    public IActionResult DownloadHeaderCSVJS([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if (!isLogin(param))
      {
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if (param.ContainsKey("requestData"))
      {
        // パラメータの設定
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
         searchCondition = getUserSearchCondition(requestData);
      }

      var csvData = new System.Text.StringBuilder();

      try
      {
        // データ取得とCSV文字列取得
        var isFirstRecord = true;
        var models = service.GetAllUsers(searchCondition);
        foreach (var model in models)
        {
          csvData.AppendLine(model.GetCSV(isFirstRecord));
          isFirstRecord = false;
        }
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (csvData.Length > 0)
      {
        result.Add("result", "OK");

        var data = new Dictionary<string,string>();
        data.Add("csv",csvData.ToString());

        // サンプルのファイル名
        string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
        fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
        data.Add("filename",fileName);

        result.Add("responseData", data);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", SearchResultZero);
      }

      return Json(result);
    }
    #endregion

    #endregion

    #region プライベートメソッド

    /// <summary>
    /// リクエストから条件入力インスタンスを取得
    /// </summary>
    /// <param name="src">リクエスト</param>
    /// <returns>条件入力インスタンス</returns>
    private UserSearchCondition getUserSearchCondition( Newtonsoft.Json.Linq.JObject src){
      var searchCondition = new UserSearchCondition();

        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName = string.Empty;

        // パラメータの設定
        paramName = "searchUserId";
        if (src.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SearchUserId = src[paramName].ToString();
        }
        paramName = "pageIndex";
        if (src.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.PageIndex = (int)src[paramName];
        }
        paramName = "sortKey";
        if (src.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SortKey = src[paramName].ToString();
        }
        paramName = "sortType";
        if (src.TryGetValue(paramName, out jsonToken))
        {
          searchCondition.SortType = src[paramName].ToString();
        }

      return searchCondition;
    }

    /// <summary>
    /// Model生成
    /// </summary>
    /// <param name="param">入力情報</param>
    private UserModel createModel(Dictionary<string, object> param)
    {
      var userId = string.Empty;
      var userName = string.Empty;
      var password = string.Empty;
      var isDelete = false;
      var version = 0;

      if (param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName = string.Empty;

        // パラメータの設定
        paramName = "id";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          userId = requestData[paramName].ToString();
        }
        paramName = "name";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          userName = requestData[paramName].ToString();
        }
        paramName = "password";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          password = requestData[paramName].ToString();
        }
        paramName = "isDelete";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          bool.TryParse(requestData[paramName].ToString(), out isDelete);
        }
        paramName = "version";
        if (requestData.TryGetValue(paramName, out jsonToken))
        {
          int.TryParse(requestData[paramName].ToString(), out version);
        }
      }

      return new UserModel(userId, userName, password, isDelete,
                           string.Empty, null, string.Empty, null, version);

    }
    #endregion

  }
}