using System;
using Commons;
using Commons.ConfigModel;
using Commons.Interfaces;
using Microsoft.Extensions.Options;

namespace Domain.Repository
{
  /// <summary>
  /// Repositoryのスーパークラス
  /// </summary>
  public class RepositoryBase : IRepositoryBase, IDisposable
  {
    protected IDatabase db;
    
    public RepositoryBase(IOptions<DatabaseConfigModel> config){
      db = DatabaseFactory.Create(config.Value);
    }

    public void Dispose()
    {
      db.Dispose();
    }

    /// <summary>
    /// トランザクション設定
    /// </summary>
    public void BeginTransaction()
    {
      db.BeginTransaction();
    }

    /// <summary>
    /// コミット
    /// </summary>
    public void Commit()
    {
      db.Commit();
    }

    /// <summary>
    /// ロールバック
    /// </summary>
    public void Rollback()
    {
      db.Rollback();
    }

  } 
}