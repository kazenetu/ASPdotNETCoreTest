using System;
using System.Collections.Generic;
using Domain.Model;
using Domain.Repository.User;

namespace Domain.Service.User
{
  public class UserService : ServiceBase, IUserService
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
      return repository.GetAllUsers(searchCondition);
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
      return repository.GetUsers(searchCondition, PageCount);
    }

    /// <summary>
    /// ユーザーを取得する
    /// </summary>
    /// <param name="userID">ユーザー名</param>
    /// <returns>ユーザー情報(検索できない場合はnull)</returns>
    public UserModel Find(string userID)
    {
      return repository.Find(userID);
    }

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <param name="entryDate">登録日時(未設定時null)</param>
    /// <returns>成否</returns>
    public UpdateResult Save(UserModel userData, string loginUserId, string passwordHash, DateTime? entryDate = null)
    {
      var result = UpdateResult.Error;

      // トランザクション作成
      repository.BeginTransaction();

      // 処理実行
      var dbResult = false;
      var latestData = Find(userData.UserID);
      if(latestData == null)
      {
        if(entryDate.HasValue)
        {
          dbResult = repository.Append(userData, loginUserId, passwordHash, entryDate.Value);
        }
      }
      else
      {
        result = UpdateResult.ErrorVaersion;
        if(userData.EqualsVersion(latestData.ModifyVersion)){
          dbResult = repository.Modify(userData, loginUserId, passwordHash);
        }
      }

      // コミットまたはロールバック
      if (dbResult)
      {
        repository.Commit();
        result = UpdateResult.OK;
      }
      else
      {
        repository.Rollback();
      }

      return result;
    }

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="password">現在のパスワード</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>成否</returns>
    public UpdateResult ChangePassword(string userID, string password, string newPassword)
    {
      var result = UpdateResult.Error;

      // トランザクション作成
      repository.BeginTransaction();

      // 処理実行
      var dbResult = repository.ChangePassword(userID, password, newPassword);

      // コミットまたはロールバック
      if (dbResult)
      {
        repository.Commit();
        result = UpdateResult.OK;
      }
      else
      {
        repository.Rollback();
      }

      return result;
    }
  }
}