namespace Domain.Service
{
  /// <summary>
  /// サービスクラスのスーパークラス
  /// </summary>
  public class ServiceBase
  {
    #region クラス定数

    /// <summary>
    /// 1ページあたりのレコード数
    /// </summary>
    protected const int PageCount = 20;

    /// <summary>
    /// 更新結果
    /// </summary>
    public enum UpdateResult
    {
      /// <summary>
      /// 成功
      /// </summary>
      OK,

      /// <summary>
      /// 失敗：バージョン違い(排他エラー)
      /// </summary>
      ErrorVaersion,

      /// <summary>
      /// 失敗
      /// </summary>
      Error
    };

    #endregion

    #region クラスメソッド

    /// <summary>
    /// レコード数から総ページ数を取得
    /// </summary>
    /// <param name="recordCount">レコード数</param>
    /// <returns>総ページ数(レコード数がゼロ件の場合は-1)</returns>
    protected int getTotalPageCount(int recordCount)
    {
      int pageCount = -1;
      if (recordCount > 0)
      {
        pageCount = recordCount / PageCount;
        if (recordCount <= PageCount)
        {
          pageCount = 0;
        }
        else
        {
          if (recordCount - pageCount * PageCount > 0)
          {
            pageCount++;
          }
        }
      }
      return pageCount;
    }

    #endregion
  }
}