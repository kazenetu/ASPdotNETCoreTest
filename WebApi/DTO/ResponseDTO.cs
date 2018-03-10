namespace WebApi.DTO
{
  /// <summary>
  /// Response用DTO
  /// </summary>
  public class ResponseDTO
  {
    /// <summary>
    /// ステータス列挙型
    /// </summary>
    public enum Results
    {
      OK, NG
    }

    /// <summary>
    /// ステータス
    /// </summary>
    private Results result;

    /// <summary>
    /// ステータス(文字列)
    /// </summary>
    public string Result
    {
      get
      {
        return result.ToString();
      }
    }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public string ErrorMessage { private set; get; }

    /// <summary>
    /// データ
    /// </summary>
    public object ResponseData { private set; get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="result">結果</param>
    /// <param name="errorMessage">エラーメッセージ</param>
    public ResponseDTO(Results result, string errorMessage)
    {
      this.result = result;
      this.ErrorMessage = errorMessage;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="result">結果</param>
    /// <param name="errorMessage">エラーメッセージ</param>
    /// <param name="responseData">データ</param>
    public ResponseDTO(Results result, string errorMessage, object responseData)
    {
      this.result = result;
      this.ErrorMessage = errorMessage;
      this.ResponseData = responseData;
    }

  }
}
