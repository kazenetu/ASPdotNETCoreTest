using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Model
{
  /// <summary>
  /// モデルベース
  /// </summary>
  public abstract class ModelBase
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
      var properties = GetType().GetProperties();

      var title = new List<string>();
      var data = new List<string>();

      // カラム名が未定義の場合は物理名を設定する
      if (!columnList.Any())
      {
        foreach(var property in properties)
        {
          columnList.Add(property.Name, property.Name);
        }
      }

      foreach(var column in columnList)
      {
        // 対象のプロパティを取得
        var targetPropertys = properties.Where((item)=>item.Name == column.Key);
        if(!targetPropertys.Any()){
          continue;
        }

        if(header)
        {
          // ヘッダー作成
          title.Add(column.Value);
        }
        // データ作衛
        data.Add(targetPropertys.First().GetValue(this) as string);
      }

      var result = string.Join(",",title);
      if(!string.IsNullOrEmpty(result)){
        result+=Environment.NewLine;
      }
      result+=string.Join(",",data);

      return result;
    }
  }
}