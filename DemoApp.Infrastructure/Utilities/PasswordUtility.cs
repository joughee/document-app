using DemoApp.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.Utilities
{
    public static class PasswordUtility
    {
        private const int saltSize = 16; // 128 bit 
        private const int keySize = 32; // 256 bit
        public static string EncryptPassword(string password, int iterations)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              saltSize,
              iterations,
              HashAlgorithmName.SHA512))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{iterations}.{salt}.{key}";
            }
        }

        public static bool CheckPassword(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(keySize);

                var verified = keyToCheck.SequenceEqual(key);

                return verified;
            }
        }
    }
}
