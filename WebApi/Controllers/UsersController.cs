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
using WebApi.DTO;

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

    #region 検索系

    /// <summary>
    /// 検索：ページ数取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/totalpage</remarks>
    [HttpPost("totalpage")]
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

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;

      if (serviceResult < 0)
      {
        status = ResponseDTO.Results.NG;
        message = SearchResultZero;
      }

      return Json(new ResponseDTO(status, message, serviceResult));
    }

    /// <summary>
    /// 検索：対象ページのレコード取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/page</remarks>
    [HttpPost("page")]
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

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;
      if (!serviceResult.Any())
      {
        status = ResponseDTO.Results.NG;
        message = SearchResultZero;
      }

      return Json(new ResponseDTO(status, message, serviceResult));
    }

    /// <summary>
    /// 対象マスタを取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/find</remarks>
    [HttpPost("find")]
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

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;

      if (serviceResult == null)
      {
        status = ResponseDTO.Results.NG;
        message = ErrorNotFound;
      }

      return Json(new ResponseDTO(status, message, serviceResult));
    }

    #endregion

    #region 更新系

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/insert</remarks>
    [HttpPost("insert")]
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

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;

      if (serviceResult != UpdateResult.OK)
      {
        status = ResponseDTO.Results.NG;
        message = string.Format(ErrorSave, "登録");
      }

      return Json(new ResponseDTO(status, message, serviceResult));
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/update</remarks>
    [HttpPost("update")]
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
          if (string.IsNullOrEmpty(model.Password))
          {
            passwordHash = dbModel.Password;
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

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;

      if (serviceResult != UpdateResult.OK)
      {
        status = ResponseDTO.Results.NG;

        if (serviceResult == UpdateResult.ErrorVaersion)
        {
          message = ErrorVersion;
        }
        else
        {
          message = string.Format(ErrorSave, "更新");
        }
      }

      return Json(new ResponseDTO(status, message, null));
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
    [IgnoreAntiforgeryToken]
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
            var generator = new CSVGenerator<UserModel>();
            foreach (var model in models)
            {
              generator.Add(model);
            }
            csvData.AppendLine(generator.GetCSV(getDownloadColumnUserModel()));
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
    [IgnoreAntiforgeryToken]
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
            var models = service.GetAllUsers(requestData);
            var generator = new CSVGenerator<UserModel>();
            foreach (var model in models)
            {
              generator.Add(model);
            }
            csvData.AppendLine(generator.GetCSV(true, getDownloadColumnUserModel()));
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
        var generator = new CSVGenerator<UserModel>();
        foreach (var model in models)
        {
          generator.Add(model);
        }
        csvData.AppendLine(generator.GetCSV(getDownloadColumnUserModel()));
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;
      var data = new Dictionary<string, string>();

      if (csvData.Length > 0)
      {
        data.Add("csv", csvData.ToString());

        // サンプルのファイル名
        string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
        fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
        data.Add("filename", fileName);
      }
      else
      {
        status = ResponseDTO.Results.NG;
        message = SearchResultZero;
      }

      return Json(new ResponseDTO(status, message, data));
    }

    /// <summary>
    /// ヘッダー付CSVダウンロード：JSで作成
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>結果(json)</returns>
    /// <remarks>POST api/user/downloadHeaderCSVJS</remarks>
    [HttpPost("downloadHeaderCSVJS")]
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
        var models = service.GetAllUsers(searchCondition);
        var generator = new CSVGenerator<UserModel>();
        foreach (var model in models)
        {
          generator.Add(model);
        }
        csvData.AppendLine(generator.GetCSV(true, getDownloadColumnUserModel()));
      }
      catch (Exception ex)
      {
        logger.LogCritical("{0}", ex.Message);
        return BadRequest();
      }

      var status = ResponseDTO.Results.OK;
      var message = string.Empty;
      var data = new Dictionary<string, string>();

      if (csvData.Length > 0)
      {
        data.Add("csv", csvData.ToString());

        // サンプルのファイル名
        string fileName = string.Format("テスト_{0:yyyyMMddHHmmss}.csv", DateTime.Now);
        fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
        data.Add("filename", fileName);
      }
      else
      {
        status = ResponseDTO.Results.NG;
        message = SearchResultZero;
      }

      return Json(new ResponseDTO(status, message, data));
    }
    #endregion

    #endregion

    #region プライベートメソッド

    /// <summary>
    /// リクエストから条件入力インスタンスを取得
    /// </summary>
    /// <param name="src">リクエスト</param>
    /// <returns>条件入力インスタンス</returns>
    private UserSearchCondition getUserSearchCondition(Newtonsoft.Json.Linq.JObject src)
    {
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
      if (param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        var userParam = requestData.Children().ToDictionary(KeyValue => KeyValue.Path, KeyValue => requestData[KeyValue.Path].ToObject<object>());
        return UserModel.Create(userParam);
      }

      return UserModel.Create(new Dictionary<string,object>());
    }

    /// <summary>
    /// ダウンロード用カラム名を取得
    /// </summary>
    /// <returns>物理カラム名と論理カラム名のリスト</returns>
    private Dictionary<string, string> getDownloadColumnUserModel()
    {
      var result = new Dictionary<string, string>();

      // ダミーインスタンス作成
      var model = new UserModel(string.Empty, string.Empty);

      // カラム名設定
      result.Add(nameof(model.UserID), "ユーザーID");
      result.Add(nameof(model.UserName), "ユーザー名");
      result.Add(nameof(model.IsDelete), "削除");

      return result;
    }
    #endregion

  }
}