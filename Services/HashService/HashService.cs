using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class HashService
    {
        public static string Hash(string value)
        {
            //value = "b9sj5l2" + value + "jv9sj3b";
            //unchecked
            //{
            //    var result = 3 * 11 * 23 * 43 * 71 * 103;
            //    foreach (var c in value)
            //        result = result * c + 5 * 17 * 37 * 61;
            //    return result.ToString();
            //}
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(value);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
