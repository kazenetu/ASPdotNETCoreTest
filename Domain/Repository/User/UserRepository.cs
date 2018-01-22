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
  public class UserRepository : RepositoryBase, IUserRepository 
  {
    public UserRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {

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
    public List<UserModel> GetAllUsers(UserSearchCondition seachCondition)
    {
      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine("  MT_USER.USER_ID");
      sql.AppendLine("  , MT_USER.USER_NAME");
      sql.AppendLine("  , MT_USER.PASSWORD");
      sql.AppendLine("  , MT_USER.DEL_FLAG");
      sql.AppendLine("  , ENTRY_USER_INFO.USER_NAME ENTRY_USER");
      sql.AppendLine("  , MT_USER.ENTRY_DATE");
      sql.AppendLine("  , MOD_USER_NFO.USER_NAME MOD_USER");
      sql.AppendLine("  , MT_USER.MOD_DATE");
      sql.AppendLine("  , MT_USER.MOD_VERSION ");
      sql.AppendLine("FROM");
      sql.AppendLine("  MT_USER ");
      sql.AppendLine("  LEFT JOIN MT_USER ENTRY_USER_INFO ");
      sql.AppendLine("    ON ENTRY_USER_INFO.USER_ID = MT_USER.ENTRY_USER");
      sql.AppendLine("  LEFT JOIN MT_USER MOD_USER_NFO ");
      sql.AppendLine("    ON MOD_USER_NFO.USER_ID = MT_USER.MOD_USER");

      // パラメータの設定
      db.ClearParam();
      if (!string.IsNullOrEmpty(seachCondition.SearchUserId))
      {
        sql.AppendLine("where ");
        sql.AppendLine("  MT_USER.USER_ID like @USER_ID");
        db.AddParam("@USER_ID", string.Format("%{0}%", seachCondition.SearchUserId));
      }
      sql.AppendLine("ORDER BY");
      sql.AppendLine("  MT_USER.USER_ID");

      // SQL発行
      var result = new List<UserModel>();
      var dbResult = db.Fill(sql.ToString());
      foreach (DataRow row in dbResult.Rows)
      {
        result.Add(createUserModel(row, false));
      }

      return result;
    }

    /// <summary>
    /// 検索結果のレコード件数を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>レコード数</returns>
    public int GetRecordCount(UserSearchCondition seachCondition)
    {
      var sql = new StringBuilder();
      sql.AppendLine("select");
      sql.AppendLine("  cast(count(USER_ID) as int) CNT");
      sql.AppendLine("from");
      sql.AppendLine("  MT_USER");

      // Param設定
      db.ClearParam();
      if (!string.IsNullOrEmpty(seachCondition.SearchUserId))
      {
        sql.AppendLine("where ");
        sql.AppendLine("  USER_ID like @USER_ID");
        db.AddParam("@USER_ID", string.Format("%{0}%", seachCondition.SearchUserId));
      }

      int recordCount = -1;
      var result = db.Fill(sql.ToString());
      if (result.Rows.Count > 0)
      {
        int.TryParse(result.Rows[0]["CNT"].ToString(), out recordCount);
      }

      // レコード件数を返す
      return recordCount;
    }

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <param name="pageCount">1ページ当たりの係数</param>
    /// <returns>ユーザーのリスト</returns>
    public List<UserModel> GetUsers(UserSearchCondition seachCondition, int pageCount)
    {
      var sql = new StringBuilder();
      sql.AppendLine("select * from MT_USER ");

      // 検索条件
      int pageIndex = seachCondition.PageIndex;
      string searchUserId = seachCondition.SearchUserId;

      // ソートキー
      var sortKeys = new Dictionary<string, string>();
      sortKeys.Add("ID", "USER_ID");
      sortKeys.Add("NAME", "USER_NAME");
      sortKeys.Add("REMOVE", "DEL_FLAG");

      string sortKey = "USER_ID ";
      if (sortKeys.ContainsKey(seachCondition.SortKey))
      {
        sortKey = sortKeys[seachCondition.SortKey] + " ";
      }
      if (!string.IsNullOrEmpty(seachCondition.SortType))
      {
        string tempType = seachCondition.SortType.ToUpper();

        if (new string[] { "ASC", "DESC" }.Any((sortType) => { return sortType == tempType; }))
        {
          sortKey += tempType;
        }
      }

      // パラメータの設定
      db.ClearParam();
      if (!string.IsNullOrEmpty(seachCondition.SearchUserId))
      {
        sql.AppendLine("where ");
        sql.AppendLine("  USER_ID like @USER_ID");
        db.AddParam("@USER_ID", string.Format("%{0}%", seachCondition.SearchUserId));
      }
      sql.AppendLine(string.Format(" ORDER BY {0}", sortKey));
      sql.AppendLine(string.Format("LIMIT {0} OFFSET {1}", pageCount, pageIndex * pageCount));

      // SQL発行
      var result = new List<UserModel>();
      var dbResult = db.Fill(sql.ToString());
      foreach (DataRow row in dbResult.Rows)
      {
        result.Add(createUserModel(row, false));
      }

      return result;
    }

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    public UserModel Find(string userId)
    {
      var sql = new StringBuilder();
      sql.AppendLine("select");
      sql.AppendLine("  * ");
      sql.AppendLine("from");
      sql.AppendLine("  MT_USER");
      sql.AppendLine("where ");
      sql.AppendLine("  USER_ID = @USER_ID");

      // Param設定
      db.ClearParam();
      db.AddParam("@USER_ID", userId);

      var result = db.Fill(sql.ToString());
      if (result.Rows.Count > 0)
      {
        return createUserModel(result.Rows[0]);
      }

      return null;
    }

    /// <summary>
    /// ユーザーの登録
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <param name="loginUserId">ログイン中のユーザーID</param>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <param name="entryDate">登録日時</param>
    /// <returns>成否</returns>
    public bool Append(UserModel userData, string loginUserId, string passwordHash, DateTime entryDate)
    {
      var sql = new StringBuilder();
      sql.AppendLine("INSERT"); 
      sql.AppendLine("INTO MT_USER( ");
      sql.AppendLine("  USER_ID");
      sql.AppendLine("  , USER_NAME");
      sql.AppendLine("  , PASSWORD");
      sql.AppendLine("  , DEL_FLAG");
      sql.AppendLine("  , ENTRY_USER");
      sql.AppendLine("  , ENTRY_DATE");
      sql.AppendLine("  , MOD_USER");
      sql.AppendLine("  , MOD_DATE");
      sql.AppendLine("  , MOD_VERSION");
      sql.AppendLine(") ");
      sql.AppendLine("VALUES ( ");
      sql.AppendLine("  @USER_ID");
      sql.AppendLine("  , @USER_NAME");
      sql.AppendLine("  , @PASSWORD");
      sql.AppendLine("  , @DEL_FLAG");
      sql.AppendLine("  , @ENTRY_USER");
      sql.AppendLine("  , @ENTRY_DATE");
      sql.AppendLine("  , @MOD_USER");
      sql.AppendLine("  , @MOD_DATE");
      sql.AppendLine("  , @MOD_VERSION");
      sql.AppendLine(") ");

      var proccesDateTime = entryDate;

      // Param設定
      db.ClearParam();
      db.AddParam("@USER_ID", userData.UserID);
      db.AddParam("@USER_NAME", userData.UserName);
      db.AddParam("@PASSWORD", passwordHash);
      db.AddParam("@DEL_FLAG", userData.IsDelete);

      db.AddParam("@ENTRY_USER", loginUserId);
      db.AddParam("@ENTRY_DATE", proccesDateTime);
      db.AddParam("@MOD_USER", loginUserId);
      db.AddParam("@MOD_DATE", proccesDateTime);
      db.AddParam("@MOD_VERSION", 1);

      try
      {
        var resultCount = db.ExecuteNonQuery(sql.ToString());
        return resultCount > 0;
      }
      catch
      {
        //Log出力
      }

      return false;
    }

    /// <summary>
    /// ユーザーの更新
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <param name="loginUserId">ログイン中のユーザーID</param>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <returns>成否</returns>
    public bool Modify(UserModel userData, string loginUserId, string passwordHash)
    {
      var sql = new StringBuilder();
      sql.AppendLine("UPDATE MT_USER ");
      sql.AppendLine("SET");
      sql.AppendLine("  USER_NAME = @USER_NAME");
      sql.AppendLine("  , PASSWORD = @PASSWORD");
      sql.AppendLine("  , DEL_FLAG = @DEL_FLAG");
      sql.AppendLine("  , MOD_USER = @MOD_USER");
      sql.AppendLine("  , MOD_DATE = @MOD_DATE");
      sql.AppendLine("  , MOD_VERSION = @NEW_MOD_VERSION ");
      sql.AppendLine("WHERE");
      sql.AppendLine("  USER_ID = @USER_ID");
      sql.AppendLine("  AND MOD_VERSION = @MOD_VERSION");

      var proccesDateTime = DateTime.Now;

      // Param設定
      db.ClearParam();
      db.AddParam("@USER_NAME", userData.UserName);
      db.AddParam("@PASSWORD", passwordHash);
      db.AddParam("@DEL_FLAG", userData.IsDelete);
      db.AddParam("@NEW_MOD_VERSION", userData.ModifyVersion + 1);

      db.AddParam("@MOD_USER", loginUserId);
      db.AddParam("@MOD_DATE", proccesDateTime);

      db.AddParam("@USER_ID", userData.UserID);
      db.AddParam("@MOD_VERSION", userData.ModifyVersion);

      try
      {
        var resultCount = db.ExecuteNonQuery(sql.ToString());
        return resultCount > 0;
      }
      catch
      {
        //Log出力
      }

      return false;
    }

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">現在のパスワード</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>成否</returns>
    public bool ChangePassword(string userID, string password, string newPassword)
    {
      // 変更対象のModelを取得する
      var model = Find(userID);

      // 取得できない場合はfalseを返す
      if (model == null)
      {
        return false;
      }

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
          return true;
        }
        else
        {
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
    /// <param name="setPassword">パスワードを設定するか否か</param>
    /// <returns>UserModelインスタンス</returns>
    private UserModel createUserModel(DataRow src, bool setPassword = true)
    {
      string userID = string.Empty;
      string userName = string.Empty;
      string password = string.Empty;
      bool isDelete = false;
      string entryUserId = string.Empty;
      DateTime? entryDate = null;
      string modifyUserId = string.Empty;
      DateTime? modifyDate = null;
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
        if(setPassword)
        {
          password = src[columnName].ToString();
        }
      }
      columnName = "DEL_FLAG";
      if (columns.Contains(columnName))
      {
        var tempValue = 0;
        if (int.TryParse(src[columnName].ToString(), out tempValue))
        {
          isDelete = Convert.ToBoolean(tempValue);
        }
      }
      columnName = "ENTRY_USER";
      if (columns.Contains(columnName))
      {
        entryUserId = src[columnName].ToString();
      }
      columnName = "ENTRY_DATE";
      if (columns.Contains(columnName) && src[columnName] != null)
      {
        DateTime entryDateValue;
        if(DateTime.TryParse(src[columnName].ToString(),out entryDateValue))
        {
          entryDate = entryDateValue;
        }
      }
      columnName = "MOD_USER";
      if (columns.Contains(columnName))
      {
        modifyUserId = src[columnName].ToString();
      }
      columnName = "MOD_DATE";
      if (columns.Contains(columnName))
      {
        DateTime modifyDateValue;
        if(DateTime.TryParse(src[columnName].ToString(),out modifyDateValue))
        {
          modifyDate = modifyDateValue;
        }
      }
      columnName = "MOD_VERSION";
      if (columns.Contains(columnName))
      {
        int.TryParse(src[columnName].ToString(), out modifyVersion);
      }

      return new UserModel(userID, userName, password, isDelete,
                           entryUserId, entryDate,
                           modifyUserId, modifyDate, modifyVersion);
    }
    #endregion

  }
}