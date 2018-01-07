using System.Collections.Generic;
using Domain.Model;

namespace Domain.Service.User
{
  public interface IUserService
  {
    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="userID">ユーザー名</param>
    /// <param name="password">パスワード</param>
    /// <returns></returns>
    UserModel Login(string userID, string password);

    /// <summary>
    /// ユーザーリストを取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザーリスト</returns>
    List<UserModel> GetAllUsers(UserSeachCondition seachCondition);

    /// <summary>
    /// 検索結果のページ総数を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ページ総数</returns>
    int GetPageCount(UserSeachCondition seachCondition);

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザーのリスト</returns>
    List<UserModel> GetUsers(UserSeachCondition seachCondition);

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    UserModel GetUser(UserSeachCondition seachCondition);

    /// <summary>
    /// ユーザーの登録
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    bool append(UserModel userData);

    /// <summary>
    /// ユーザーの更新
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    bool update(UserModel userData);

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <returns>成否</returns>
    bool remove(UserModel userData);

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">現在のパスワード</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>成否</returns>
    bool changePassword(string userID, string password, string newPassword);
  }
}