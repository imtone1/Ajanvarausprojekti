using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Ajanvarausprojekti.Functions
{
    public static class Hmac
    {
        private const int SaltSize = 0; //kun nolla, palauttaa aina saman kryptauksen
        public static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SaltSize];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }
        public static byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }
    }
}