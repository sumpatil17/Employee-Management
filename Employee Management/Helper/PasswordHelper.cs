using System.Security.Cryptography;
using System.Text;
using System;

namespace Employee_Management.Helper
{
    public class PasswordHelper
    {
        // Generate a hashed password with a salt
        public static string GenerateHashedPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = GenerateSalt();
            var saltedPassword = password + salt;
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hash) + ":" + salt; // Store hash and salt together
        }

        // Generate a random salt
        private static string GenerateSalt()
        {
            var buffer = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Split stored hash into hash and salt parts
            var parts = storedPasswordHash.Split(':');
            if (parts.Length != 2)
                throw new FormatException("Stored password hash is in an invalid format.");

            var storedHash = parts[0];
            var salt = parts[1];

            // Hash the entered password with the same salt
            using var sha256 = SHA256.Create();
            var saltedPassword = enteredPassword + salt;
            var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            // Compare the stored hash with the computed hash
            return Convert.ToBase64String(computedHash) == storedHash;
        }
    }
}
