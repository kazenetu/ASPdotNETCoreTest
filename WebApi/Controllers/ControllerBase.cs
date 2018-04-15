using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
  /// <summary>
  /// コントローラークラスのスーパークラス
  /// </summary>
  public class ControllerBase : Controller
  {
    #region クラス定数

    /// <summary>
    /// セッション用クッキー名
    /// </summary>
    public static readonly string SessionCookieName = "sid";


    #endregion

    #region インスタンス定数
    /// <summary>
    /// セッションキー：ユーザーID
    /// </summary>
    protected readonly string SessionKeyUserID = "userID";
    #endregion

    #region プロパティ
    /// <summary>
    /// セッション
    /// </summary>      
    protected ISession session
    {
      get
      {
        return HttpContext.Session;
      }
    }
    #endregion

    #region メソッド

    /// <summary>
    /// セッションキーの文字列を取得
    /// </summary>
    /// <param name="sessionKey">セッションキー</param>
    /// <returns>キーがある場合は対応する文字列、ない場合はnull</returns>
    protected string getSessionString(string sessionKey)
    {
      string result = session.GetString(sessionKey);

      return result;
    }

    /// <summary>
    /// ログイン中のユーザーIDを取得
    /// </summary>
    /// <param name="param">入力情報</param>
    /// <returns>ユーザーID</returns>
    /// <remarks>取得できない場合はnull</remarks>
    protected string getLoginUserId(Dictionary<string, object> param)
    {
      var paramNameUserId = "loginUserId";
      if (!param.ContainsKey(paramNameUserId))
      {
        return null;
      }
      return param[paramNameUserId].ToString();
    }

    /// <summary>
    /// ログインチェック
    /// </summary>
    /// <param name="param">入力されたユーザーID</param>
    /// <returns>ログイン結果</returns>
    protected bool isLogin(Dictionary<string, object> param)
    {
      var userID = getLoginUserId(param);
      var loginUser = getSessionString(SessionKeyUserID);

      if (loginUser == userID)
      {
        return true;
      }

#if DEBUG
      return true;
#else
      return false;
#endif
    }

    /// <summary>
    /// セッションのリフレッシュ
    /// </summary>
    protected void refreshSession()
    {
      // セッション破棄
      session.Clear();
    }
    #endregion
  }
}