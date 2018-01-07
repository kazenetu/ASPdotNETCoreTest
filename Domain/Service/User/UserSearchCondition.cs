namespace Domain.Service.User
{
  /// <summary>
  /// 検索条件クラス
  /// </summary>
  public class UserSearchCondition
  {

    /// <summary>
    /// 検索条件：ユーザーID
    /// </summary>
    /// <returns>ユーザーID(未選択時:string.Empty)</returns>
    public string SearchUserId{set;get;} = string.Empty;

    /// <summary>
    /// 表示対象のページインデックス
    /// </summary>
    /// <returns>ページインデックス</returns>
    public int PageIndex{set;get;} = 0;
    
    /// <summary>
    /// ソートキー
    /// </summary>
    /// <returns>ソートキー</returns>
    public string SortKey{set;get;} = string.Empty;

    /// <summary>
    /// ソートタイプ
    /// </summary>
    /// <returns>ソートタイプ(昇順・降順)</returns>
    public string SortType{set;get;} = string.Empty;
  }
}