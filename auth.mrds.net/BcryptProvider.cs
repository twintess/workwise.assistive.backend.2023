using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;

namespace auth.mrds.net
{
    public static class BcryptProvider
    {
        public static string HashPassword(string password)
        {
            return BC.HashPassword(password, 16, true);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BC.Verify(password, hash, true, HashType.SHA384);
        }
    }
}