using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebApi.Utilities
{
  /// <summary>
  /// ハッシュユーティリティ
  /// </summary>
  public class HashUtility
  {
    /// <summary>
    /// ハッシュ作成
    /// </summary>
    /// <param name="userId">ユーザーID(ソルト用)</param>
    /// <param name="password">平文パスワード</param>
    /// <param name="createDate">データ作成日時(ソルト用)</param>
    /// <returns></returns>
    public static string Create(string userId, string password, DateTime createDate)
    {
      byte[] salt = new byte[128 / 8];
      var saltSrc = createDate.ToString("yyyyMMddhhmmsss") + userId;
      salt = Encoding.UTF8.GetBytes(saltSrc);

      // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));

      return hashed;
    }

  }
}