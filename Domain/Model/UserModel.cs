using System;

namespace Domain.Model
{
  /// <summary>
  /// ユーザーモデル
  /// </summary>
  [Serializable]
  public class UserModel : ModelBase
  {
    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="userName">ユーザー名</param>
    /// <param name="password">パスワード</param>
    public UserModel(string userID, string userName, string password = "", bool isDelete = false,
                     string entryUserId = "", DateTime? entryDate = null,
                     string modifyUserId = "", DateTime? modifyDate = null, int modifyVersion = 1)
    {
      this.UserID = userID;
      this.UserName = userName;
      this.Password = password;
      this.IsDelete = isDelete;
      this.EntryUserId = entryUserId;
      this.EntryDate = entryDate;
      this.ModifyUserId = modifyUserId;
      this.ModifyDate = modifyDate;
      this.ModifyVersion = modifyVersion;
    }

    #endregion

    #region プロパティ
    /// <summary>
    /// ユーザーID
    /// </summary>
    /// <returns>ユーザーID</returns>
    public string UserID { private set; get; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    /// <returns>ユーザー名</returns>
    public string UserName { private set; get; }

    /// <summary>
    /// パスワード
    /// </summary>
    /// <returns>パスワード</returns>
    public string Password { private set; get; }

    /// <summary>
    /// 論理削除されているか否か
    /// </summary>
    /// <returns>論理削除されている場合はtrue</returns>
    public bool IsDelete { private set; get; }

    /// <summary>
    /// 登録ユーザーID
    /// </summary>
    /// <returns>ユーザーIDまたはnull</returns>
    public string EntryUserId { private set; get; }

    /// <summary>
    /// 登録日時
    /// </summary>
    /// <returns>登録日時またはnull</returns>
    public DateTime? EntryDate { private set; get; }

    /// <summary>
    /// 更新ユーザーID
    /// </summary>
    /// <returns>ユーザーIDまたはnull</returns>
    public string ModifyUserId { private set; get; }

    /// <summary>
    /// 更新日時
    /// </summary>
    /// <returns>更新日時またはnull</returns>
    public DateTime? ModifyDate { private set; get; }

    /// <summary>
    /// 更新バージョン
    /// </summary>
    /// <returns>更新バージョン</returns>
    /// <remarks>楽観的排他処理用</remarks>
    public int ModifyVersion { private set; get; }
    #endregion

    #region メソッド

    /// <summary>
    /// バージョンが一致するか否か
    /// </summary>
    /// <param name="vesion">比較対象バージョン</param>
    /// <returns>バージョンが一致する場合はtrue</returns>
    public bool EqualsVersion(int vesion){
      return ModifyVersion == vesion;
    }
    #endregion
  }
}