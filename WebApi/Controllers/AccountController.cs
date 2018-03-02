using System;
using System.Collections.Generic;
using Domain.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApi.Utilities;

namespace WebApi.Controllers
{
  [Route("api/account")]
  public class AccountController : ControllerBase
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
    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="service">サービスインスタンス</param>
    /// <param name="logger">ログインスタンス</param>
    public AccountController(IUserService service, ILogger<UsersController> logger)
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
          passwordHash = HashUtility.Create(model.UserID, password, model.EntryDate.Value);
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

    #endregion
  }
}