using System;
using System.Security.Cryptography;
using System.Text;

namespace PolyGames.Common
{
    public class FileNameGenerator
    {
        public string GenerateHash()
        {
            string input = Guid.NewGuid().ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                int hashLength = new Random().Next(6, 11);
                StringBuilder builder = new StringBuilder();

                int currentIndex = 0;
                while (builder.Length < hashLength)
                {
                    builder.Append(hashBytes[currentIndex].ToString("x2"));
                    currentIndex = (currentIndex + 1) % hashBytes.Length;
                }

                return builder.ToString().Substring(0, hashLength);
            }
        }

        public string GetPictureFileName(int imageIndex = 0)
        {
            return $"pic_{GenerateHash()}_{DateTime.Now.ToString("ddMMyy_hhmmss")}_{imageIndex}";
        }

        public string GetVideoFileName()
        {
            return $"vid_{GenerateHash()}_{DateTime.Now.ToString("ddMMyy_hhmmss")}";
        }

        public string GetExeFileName()
        {
            return $"exe_{GenerateHash()}_{DateTime.Now.ToString("ddMMyy_hhmmss")}";
        }
    }
}