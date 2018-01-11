using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;
using Domain.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
  [Route("api/user")]
  public class UsersController : ControllerBase
  {
    private readonly IUserService service;

    private readonly ILogger logger;

    private static string ErrorLoginNG  = "ログイン失敗";
    private static string ErrorPasswordNG  = "パスワード失敗";
    private static string SearchResultZero = "検索結果ゼロ件";
    private static string ErrorNotFound = "データが見つかりません";

    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
      this.service = service;
      this.logger = logger;
    }

    // POST api/user/login
    [HttpPost("login")]
    [AutoValidateAntiforgeryToken]
    public IActionResult LoginPost([FromBody]Dictionary<string, object> param)
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
        var model = service.Login(userId, password);
        if (model != null)
        {
          // セッション破棄
          refreshSession();

          // セッションキー設定
          session.SetString(SessionKeyUserID,userId);

          serviceResult = true;
          data.Add("name", model.UserName);
        }
      }
      catch(Exception ex)
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
        result.Add("errorMessage",ErrorLoginNG);
      }
      result.Add("responseData", data);

      return Json(result);
    }

    // POST api/user/logout
    [HttpPost("logout")]
    [AutoValidateAntiforgeryToken]
    public IActionResult LogoutPost([FromBody]Dictionary<string, object> param)
    {

      // セッション破棄
      HttpContext.Response.Cookies.Delete(ControllerBase.SessionCookieName);
      session.Clear();

      var result = new Dictionary<string, object>();
      result.Add("result", "OK");

      return Json(result);
    }

    // POST api/user/passwordChange
    [HttpPost("passwordChange")]
    [AutoValidateAntiforgeryToken]
    public IActionResult PasswordChangePost([FromBody]Dictionary<string, object> param)
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
      var serviceResult = false;
      try
      {
        serviceResult = service.ChangePassword(userId, password, newPassword);
      }
      catch(Exception ex)
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
        result.Add("errorMessage",ErrorPasswordNG);
      }
      result.Add("responseData", data);

      return Json(result);
    }

    // POST api/user/totalpage
    [HttpPost("totalpage")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Totalpage([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if(!isLogin(param)){
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if(param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName=string.Empty;

        // パラメータの設定
        paramName = "searchUserId";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SearchUserId = requestData[paramName].ToString();
        }
        paramName = "pageIndex";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.PageIndex = (int)requestData[paramName];
        }
        paramName = "sortKey";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SortKey = requestData[paramName].ToString();
        }
        paramName = "sortType";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SortType = requestData[paramName].ToString();
        }
      }

      var serviceResult = 0;
      try
      {
        serviceResult = service.GetPageCount(searchCondition);
      }
      catch(Exception ex)
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
        result.Add("errorMessage",SearchResultZero);
      }
      result.Add("responseData", serviceResult);

      return Json(result);
    }

    // POST api/user/page
    [HttpPost("page")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Page([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if(!isLogin(param)){
        return Unauthorized();
      }

      var searchCondition = new UserSearchCondition();
      if(param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName=string.Empty;

        // パラメータの設定
        paramName = "searchUserId";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SearchUserId = requestData[paramName].ToString();
        }
        paramName = "pageIndex";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.PageIndex = (int)requestData[paramName];
        }
        paramName = "sortKey";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SortKey = requestData[paramName].ToString();
        }
        paramName = "sortType";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          searchCondition.SortType = requestData[paramName].ToString();
        }
      }

      var serviceResult = new List<UserModel>();
      try
      {
        serviceResult.AddRange(service.GetUsers(searchCondition));
      }
      catch(Exception ex)
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
        result.Add("errorMessage",SearchResultZero);
      }

      return Json(result);
    }

    // POST api/user/find
    [HttpPost("find")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Find([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if(!isLogin(param)){
        return Unauthorized();
      }

      var userId = string.Empty;

      if(param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName=string.Empty;

        // パラメータの設定
        paramName = "id";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          userId = requestData[paramName].ToString();
        }
      }

      // 入力チェック
      if(string.IsNullOrEmpty(userId))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(userId));
        return BadRequest();
      }

      UserModel serviceResult = null;
      try
      {
        serviceResult = service.Find(userId);
      }
      catch(Exception ex)
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
      }
      result.Add("responseData", serviceResult);

      return Json(result);
    }

    // POST api/user/insert
    [HttpPost("insert")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Insert([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if(!isLogin(param)){
        return Unauthorized();
      }

      // model取得
      var model = createModel(param);

      // 入力チェック
      if(string.IsNullOrEmpty(model.UserID))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(model.UserID));
        return BadRequest();
      }

      var serviceResult = false;
      try
      {
        serviceResult = service.Save(model, getLoginUserId(param));
      }
      catch(Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult)
      {
        result.Add("result", "OK");
        result.Add("responseData", string.Empty);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", ErrorNotFound);
      }

      return Json(result);
    }

    // POST api/user/update
    [HttpPost("update")]
    [AutoValidateAntiforgeryToken]
    public IActionResult Update([FromBody]Dictionary<string, object> param)
    {
      // ログインチェック
      if(!isLogin(param)){
        return Unauthorized();
      }

      // model取得
      var model = createModel(param);

      // 入力チェック
      if(string.IsNullOrEmpty(model.UserID))
      {
        logger.LogError("Pram[{0}]が未設定", nameof(model.UserID));
        return BadRequest();
      }

      var serviceResult = false;
      try
      {
        serviceResult = service.Save(model, getLoginUserId(param));
      }
      catch(Exception ex)
      {
        logger.LogCritical("[{0}", ex.Message);
        return BadRequest();
      }

      var result = new Dictionary<string, object>();
      if (serviceResult)
      {
        result.Add("result", "OK");
        result.Add("responseData", null);
      }
      else
      {
        result.Add("result", "NG");
        result.Add("errorMessage", ErrorNotFound);
      }

      return Json(result);
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

      if(param.ContainsKey("requestData"))
      {
        var requestData = param["requestData"] as Newtonsoft.Json.Linq.JObject;
        Newtonsoft.Json.Linq.JToken jsonToken = null;

        var paramName = string.Empty;

        // パラメータの設定
        paramName = "id";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          userId = requestData[paramName].ToString();
        }
        paramName = "name";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          userName = requestData[paramName].ToString();
        }
        paramName = "password";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          password = requestData[paramName].ToString();
        }
        paramName = "isDelete";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          bool.TryParse(requestData[paramName].ToString(), out isDelete);
        }
        paramName = "version";
        if (requestData.TryGetValue(paramName,out jsonToken))
        {
          int.TryParse(requestData[paramName].ToString(), out version);
        }
      }

      return new UserModel(userId, userName, password, isDelete,
                           string.Empty, null, string.Empty, null, version);

    }

  }
}