using System;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public static class PasswordService
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "A senha não pode ser nula ou vazia.");
        }

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
        {
            return false;
        }

        string calculatedHash = HashPassword(password);

        // Compara os hashes de forma segura para evitar ataques de temporização
        return string.Equals(calculatedHash, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
