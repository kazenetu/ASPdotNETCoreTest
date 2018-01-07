using System.Collections.Generic;
using System.Text;
using System.Linq;
using Commons;
using Commons.ConfigModel;
using Commons.Interfaces;
using Domain.Model;
using Domain.Service.User;
using Microsoft.Extensions.Options;
using System;

namespace Domain.Repository.User
{
  /// <summary>
  /// ユーザーリポジトリ
  /// </summary>
  public class UserRepository : IUserRepository
  {
    private IDatabase db;

    public UserRepository(IOptions<DatabaseConfigModel> config)
    {
      db = DatabaseFactory.Create(config.Value);
    }

    public void Dispose()
    {
      db.Dispose();
    }

    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="userID">ユーザー名</param>
    /// <param name="password">パスワード</param>
    /// <returns></returns>
    public UserModel Login(string userID, string password)
    {
      var sql = new StringBuilder();
      sql.AppendLine("select");
      sql.AppendLine("  USER_NAME");
      sql.AppendLine("from");
      sql.AppendLine("  MT_USER");
      sql.AppendLine("where ");
      sql.AppendLine("  USER_ID = @USER_ID");

      // Param設定
      db.ClearParam();
      db.AddParam("@USER_ID", userID);

      UserModel model = null;
      var result = db.Fill(sql.ToString());
      if (result.Rows.Count > 0)
      {
        model = new UserModel(userID, result.Rows[0]["USER_NAME"].ToString(), string.Empty);
      }

      return model;
    }

    /// <summary>
    /// ユーザーリストを取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザーリスト</returns>
    public List<UserModel> GetAllUsers(UserSeachCondition seachCondition)
    {
      return new List<UserModel>();
    }

    /// <summary>
    /// 検索結果のページ総数を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ページ総数</returns>
    public int GetUserPageCount(UserSeachCondition seachCondition)
    {
      return 0;
    }

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザーのリスト</returns>
    public List<UserModel> GetUsers(UserSeachCondition seachCondition)
    {
      return new List<UserModel>();
    }

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    UserModel Find(string userId)
    {
      return null;
    }

    /// <summary>
    /// ユーザーの登録
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    public bool append(UserModel userData)
    {
      return false;
    }

    /// <summary>
    /// ユーザーの更新
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    public bool update(UserModel userData)
    {
      return false;
    }

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    public bool remove(UserModel userData)
    {
      return false;
    }

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">現在のパスワード</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>成否</returns>
    public bool changePassword(string userID, string password, string newPassword)
    {
      var sql = new StringBuilder();
      sql.Append("update MT_USER set PASSWORD = @NEW_PASSWORD");
      sql.Append("  where");
      sql.Append("  USER_ID = @USER_ID");
      sql.Append("  and PASSWORD = @PASSWORD");

      // Param設定
      db.ClearParam();
      db.AddParam("@NEW_PASSWORD", newPassword);
      db.AddParam("@USER_ID", userID);
      db.AddParam("@PASSWORD", password);

      try
      {
        var updateCount = db.ExecuteNonQuery(sql.ToString());
        if (updateCount > 0)
        {
          db.Commit();
          return true;
        }
        else
        {
          db.Rollback();
          return false;
        }
      }
      catch
      {
        //Log出力
      }

      return false;
    }
  }
}