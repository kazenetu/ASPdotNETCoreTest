using System.Collections.Generic;
using Domain.Model;
using Domain.Repository.User;

namespace Domain.Service.User
{
  public class UserService :  ServiceBase,IUserService
  {
    private readonly IUserRepository repository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">ユーザーリポジトリ</param>    
    public UserService(IUserRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="userID">ユーザー名</param>
    /// <param name="password">パスワード</param>
    /// <returns></returns>
    public UserModel Login(string userID, string password)
    {
      return repository.Login(userID, password);
    }

    /// <summary>
    /// ユーザーリストを取得する
    /// </summary>
    /// <param name="searchCondition">検索条件</param>
    /// <returns>ユーザーリスト</returns>
    public List<UserModel> GetAllUsers(UserSearchCondition searchCondition)
    {
      return new List<UserModel>();
    }

    /// <summary>
    /// 検索結果のページ総数を取得する
    /// </summary>
    /// <param name="searchCondition">検索条件</param>
    /// <returns>ページ総数</returns>
    public int GetPageCount(UserSearchCondition searchCondition)
    {
      return getTotalPageCount(repository.GetRecordCount(searchCondition));
    }

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="searchCondition">検索条件</param>
    /// <returns>ユーザーのリスト</returns>
    public List<UserModel> GetUsers(UserSearchCondition searchCondition)
    {
      return repository.GetUsers(searchCondition,PageCount);
    }

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="userID">ユーザー名</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    public UserModel Find(string userID)
    {
      return null;
    }

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    public bool Save(UserModel userData)
    {
      return false;
    }

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    public bool Remove(UserModel userData)
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
    public bool ChangePassword(string userID, string password, string newPassword)
    {
      return repository.changePassword(userID,password,newPassword);
    }
  }
}