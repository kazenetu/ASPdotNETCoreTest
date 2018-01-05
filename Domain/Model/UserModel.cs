namespace Domain.Model
{
  /// <summary>
  /// ユーザーモデル
  /// </summary>
  public class UserModel
  {
    #region プライベートフィールド

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="userName">ユーザー名</param>
    /// <param name="password">パスワード</param>
    public UserModel(string userID, string userName, string password)
    {
      this.UserID = userID;
      this.UserName = userName;
      this.Password = password;
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
    #endregion

    #region メソッド
    #endregion
  }
}