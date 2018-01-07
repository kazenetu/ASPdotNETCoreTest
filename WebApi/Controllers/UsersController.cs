using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;
using Domain.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
  [Route("api/user")]
  public class UsersController : ControllerBase
  {
    private readonly IUserService service;

    private static string ErrorLoginNG  = "ログイン失敗";
    private static string ErrorPasswordNG  = "パスワード失敗";
    private static string SearchResultZero = "検索結果ゼロ件";

    public UsersController(IUserService service)
    {
      this.service = service;
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
        //logger.LogError("Pram[{0}]が未設定", paramNameUserId);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNamePassword))
      {
        //logger.LogError("Pram[{0}]が未設定", paramNamePassword);
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
      catch//(Exception ex){
      {

        //TODO :ログ出力
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
        //logger.LogError("Pram[{0}]が未設定", paramNameUserId);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNamePassword))
      {
        //logger.LogError("Pram[{0}]が未設定", paramNamePassword);
        return BadRequest();
      }
      if (!param.ContainsKey(paramNameNewPassword))
      {
        //logger.LogError("Pram[{0}]が未設定", paramNameNewPassword);
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
      catch//(Exception ex){
      {

        //TODO :ログ出力
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
        return BadRequest();
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
        var message = ex.Message;

        //TODO :ログ出力
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
        return BadRequest();
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
        var message = ex.Message;

        //TODO :ログ出力
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

  }
}