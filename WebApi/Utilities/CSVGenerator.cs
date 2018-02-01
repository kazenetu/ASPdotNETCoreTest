using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApi.Utilities
{
  /// <summary>
  /// CSV作成クラス
  /// </summary>
  public class CSVGenerator<T> : List<T>
  {
    /// <summary>
    /// CSV出力(ヘッダーなし・カラム設定なし)
    /// </summary>
    /// <returns>カンマ区切りの文字列</returns>
    public string GetCSV()
    {
      return GetCSV(false, new Dictionary<string, string>());
    }

    /// <summary>
    /// CSV出力(カラム設定なし)
    /// </summary>
    /// <param name="header">ヘッダー付きか否か</param>
    /// <returns>カンマ区切りの文字列</returns>
    public string GetCSV(bool header)
    {
      return GetCSV(header, new Dictionary<string, string>());
    }

    /// <summary>
    /// CSV出力(ヘッダーなし)
    /// </summary>
    /// <returns>カンマ区切りの文字列</returns>
    /// <param name="columnList">物理カラム名と論理カラム名のリスト</param>
    public string GetCSV(Dictionary<string, string> columnList)
    {
      return GetCSV(false, columnList);
    }

    /// <summary>
    /// CSV出力
    /// </summary>
    /// <param name="header">ヘッダー付きか否か</param>
    /// <param name="columnList">物理カラム名と論理カラム名のリスト</param>
    /// <returns>カンマ区切りの文字列</returns>
    public string GetCSV(bool header, Dictionary<string, string> columnList)
    {
      if (!this.Any())
      {
        return string.Empty;
      }

      var properties = GetType().GenericTypeArguments[0].GetProperties();

      var title = new List<string>();
      var data = new StringBuilder();

      // カラム名が未定義の場合は物理名を設定する
      if (!columnList.Any())
      {
        foreach (var property in properties)
        {
          columnList.Add(property.Name, property.Name);
        }
      }

      // ヘッダー作成
      if (header)
      {
        foreach (var column in columnList)
        {
          title.Add(column.Value);
        }
      }

      // データ作成
      foreach (var item in this)
      {

        var line = new List<string>();
        foreach (var column in columnList)
        {
          // 対象のプロパティを取得
          var targetPropertys = properties.Where((prop) => prop.Name == column.Key);
          if (!targetPropertys.Any())
          {
            continue;
          }

          line.Add(targetPropertys.First().GetValue(item) as string);
        }

        // データリストにカンマ区切りの文字列に変換して追加
        data.Append(string.Join(",", line));

        // データ行後の改行
        data.Append(Environment.NewLine);
      }

      var result = string.Join(",", title);
      if (!string.IsNullOrEmpty(result))
      {
        result += Environment.NewLine;
      }
      result += data.ToString();

      return result;
    }
  }
}