using System;
using System.Security.Cryptography;
using System.Text;

namespace ExactTarget.TriggeredEmail.Core
{
    public class ExternalKeyGenerator
    {
        public static string GenerateExternalKey(string value)
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            return new Guid(bytes).ToString();
        }
    }
}