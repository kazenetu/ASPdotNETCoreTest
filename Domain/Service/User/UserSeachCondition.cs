namespace Domain.Service.User
{
  /// <summary>
  /// 検索条件
  /// </summary>
  public class UserSeachCondition
  {

    /// <summary>
    /// 検索条件：ユーザーID
    /// </summary>
    /// <returns>ユーザーID(未選択時:string.Empty)</returns>
    public string SearchUserId{private set;get;} = string.Empty;

    /// <summary>
    /// 表示対象のページインデックス
    /// </summary>
    /// <returns>ページインデックス</returns>
    public int PageIndex{private set;get;} = 0;
    
    /// <summary>
    /// ソートキー
    /// </summary>
    /// <returns>ソートキー</returns>
    public string SortKey{private set;get;} = string.Empty;

    /// <summary>
    /// ソートタイプ
    /// </summary>
    /// <returns>ソートタイプ(昇順・降順)</returns>
    public string SortType{private set;get;} = string.Empty;
  }
}