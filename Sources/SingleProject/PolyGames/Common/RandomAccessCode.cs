using System;
using System.Linq;

namespace PolyGames.Common
{
    public class RandomAccessCode
    {
        public static string GenerateAccessCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var accessCode = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return accessCode;
        }
    }
}