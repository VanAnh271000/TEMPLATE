using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToSha256Hash(this object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes);
        }
    }
}
