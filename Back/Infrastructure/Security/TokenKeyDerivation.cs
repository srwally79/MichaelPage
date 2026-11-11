using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public static class TokenKeyDerivation
{
    public static byte[] Derive(string secret) => SHA256.HashData(Encoding.UTF8.GetBytes(secret));
}
