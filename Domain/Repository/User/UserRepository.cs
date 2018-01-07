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
using System.Data;

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
      sql.AppendLine("  AND PASSWORD = @PASSWORD");

      // Param設定
      db.ClearParam();
      db.AddParam("@USER_ID", userID);
      db.AddParam("@PASSWORD", password);

      var result = db.Fill(sql.ToString());
      if (result.Rows.Count > 0)
      {
        return createUserModel(result.Rows[0]);
      }

      return null;
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
    /// 検索結果のレコード件数を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>レコード数</returns>
    public int GetRecordCount(UserSeachCondition seachCondition)
    {
      var sql = new StringBuilder();
      sql.AppendLine("select");
      sql.AppendLine("  cast(count(USER_ID) as int) CNT");
      sql.AppendLine("from");
      sql.AppendLine("  MT_USER");

      // Param設定
      db.ClearParam();
      if(!string.IsNullOrEmpty(seachCondition.SearchUserId)){
        sql.AppendLine("where ");
        sql.AppendLine("  USER_ID like %@USER_ID%");
        db.AddParam("@USER_ID", seachCondition.SearchUserId);
      }

      int recordCount = 0;
      var result = db.Fill(sql.ToString());
      if (result.Rows.Count > 0)
      {
        recordCount = (int)result.Rows[0]["CNT"];
      }

      // レコード件数を返す
      return recordCount;
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
    public UserModel Find(string userId)
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


    #region プライベートメソッド

    /// <summary>
    /// UserModelの作成
    /// </summary>
    /// <param name="src">データ元のDataRow</param>
    /// <returns>UserModelインスタンス</returns>
    private UserModel createUserModel(DataRow src)
    {
      string userID = string.Empty;
      string userName = string.Empty;
      string password = string.Empty;
      bool isDelete = false;
      string entryUserId = string.Empty;
      DateTimeOffset? entryDate = null;
      string modifyUserId = string.Empty;
      DateTimeOffset? modifyDate = null;
      int modifyVersion = 1;

      var columns = src.Table.Columns;
      var columnName = string.Empty;

      columnName = "USER_ID";
      if (columns.Contains(columnName))
      {
        userID = src[columnName].ToString();
      }
      columnName = "USER_NAME";
      if (columns.Contains(columnName))
      {
        userName = src[columnName].ToString();
      }
      columnName = "PASSWORD";
      if (columns.Contains(columnName))
      {
        password = src[columnName].ToString();
      }
      columnName = "DEL_FLAGE";
      if (columns.Contains(columnName))
      {
        isDelete = (bool)src[columnName];
      }
      columnName = "ENTRY_USER";
      if (columns.Contains(columnName))
      {
        entryUserId = src[columnName].ToString();
      }
      columnName = "ENTRY_DATE";
      if (columns.Contains(columnName))
      {
        entryDate = src[columnName] as DateTimeOffset?;
      }
      columnName = "MOD_USER";
      if (columns.Contains(columnName))
      {
        modifyUserId = src[columnName].ToString();
      }
      columnName = "MOD_DATE";
      if (columns.Contains(columnName))
      {
        modifyDate = src[columnName] as DateTimeOffset?;
      }
      columnName = "MOD_VERSION";
      if (columns.Contains(columnName))
      {
        modifyVersion = (int)src[columnName];
      }

      return new UserModel(userID, userName, password, isDelete,
                           entryUserId, entryDate,
                           modifyUserId, modifyDate, modifyVersion);
    }
    #endregion

  }
}