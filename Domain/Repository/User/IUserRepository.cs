using System;
using System.Collections.Generic;
using Domain.Model;
using Domain.Service.User;

namespace Domain.Repository.User
{
  public interface IUserRepository : IRepositoryBase
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
    List<UserModel> GetAllUsers(UserSearchCondition seachCondition);

    /// <summary>
    /// 検索結果のレコード件数を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <returns>レコード数</returns>
    int GetRecordCount(UserSearchCondition seachCondition);

    /// <summary>
    /// ユーザーのページ分を取得する
    /// </summary>
    /// <param name="seachCondition">検索条件</param>
    /// <param name="pageCount">1ページ当たりの係数</param>
    /// <returns>ユーザーのリスト</returns>
    List<UserModel> GetUsers(UserSearchCondition seachCondition, int pageCount);

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    UserModel Find(string userId);

    /// <summary>
    /// ユーザーの登録
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <param name="loginUserId">ログイン中のユーザーID</param>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <param name="entryDate">登録日時</param>
    /// <returns>成否</returns>
    bool Append(UserModel userData, string loginUserId, string passwordHash, DateTime entryDate);

    /// <summary>
    /// ユーザーの更新
    /// </summary>
    /// <param name="userData">ユーザーデータ</param>
    /// <param name="loginUserId">ログイン中のユーザーID</param>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <returns>成否</returns>
    bool Modify(UserModel userData, string loginUserId, string passwordHash);

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">現在のパスワード</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>成否</returns>
    bool ChangePassword(string userID, string password, string newPassword);
  }
}