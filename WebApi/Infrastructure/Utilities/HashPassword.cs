using System.Security.Cryptography;
using System.Text;

namespace WebApi.Infrastructure.Utilities
{
    public class HashPassword256
    {
        public static string HashPassword(string password)
        {
            var bytes = Encoding.ASCII.GetBytes(password);
            var hash = SHA256.HashData(bytes);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
