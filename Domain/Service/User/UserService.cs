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
    public int GetPageCount(UserSeachCondition seachCondition)
    {
      return getTotalPageCount(repository.GetRecordCount(seachCondition));
    }

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザーのリスト</returns>
    public List<UserModel> GetUsers(UserSeachCondition seachCondition)
    {
      return repository.GetUsers(seachCondition,PageCount);
    }

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    public UserModel GetUser(UserSeachCondition seachCondition)
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
      return repository.changePassword(userID,password,newPassword);
    }
  }
}