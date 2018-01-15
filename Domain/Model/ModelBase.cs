using System;
using System.Collections.Generic;

namespace Domain.Model
{
  /// <summary>
  /// モデルベース
  /// </summary>
  public abstract class ModelBase
  {
    /// <summary>
    /// CSV出力
    /// </summary>
    /// <returns>カンマ区切りの文字列</returns>
    public string GetCSV()
    {
      return GetCSV(false);
    }

    /// <summary>
    /// CSV出力
    /// </summary>
    /// <param name="header">ヘッダー付きか否か</param>
    /// <returns>カンマ区切りの文字列</returns>
    public string GetCSV(bool header)
    {
      var properties = GetType().GetProperties();

      var title = new List<string>();
      var data = new List<string>();

      foreach(var property in properties)
      {
        if(header)
        {
          title.Add(property.Name);
        }
        data.Add(property.GetValue(this) as string);
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