using System.Security.Cryptography;

namespace Validacao_1.Shared.Services
{
    public class CriptografiaServico
    {
        public string HashPassword(string password)
        {
            const int iterations = 100_000;
            const int saltSize = 32;
            const int keySize = 32;

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(keySize);

            string saltBase64 = Convert.ToBase64String(salt);
            string keyBase64 = Convert.ToBase64String(key);

            return $"{iterations}.{saltBase64}.{keyBase64}";
        }

        public bool VerifyPassword(string password, string storehash)
        {
            var parts = storehash.Split('.');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out int iterations)) return false;

            var salt = Convert.FromBase64String(parts[1]);
            var storedKey = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var enteredKey = pbkdf2.GetBytes(storedKey.Length);

            return CryptographicOperations.FixedTimeEquals(enteredKey, storedKey);
        }
    }
}
