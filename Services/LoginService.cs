using Ajanvarausprojekti.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ajanvarausprojekti.Services
{
    public class LoginService
    {
        //Irina: salasana hash
        public string md5_string(string password)
        {
            //salasanan hash
            var crpwd = "";
            var salt = Hmac.GenerateSalt();
            var hmac1 = Hmac.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(password), salt);
            crpwd = (Convert.ToBase64String(hmac1));
            string md5_password = string.Empty;
            md5_password = crpwd;

            return md5_password;
        }

        //Irina: Tämä liittyy varauksen salasanan generoimiseen
        public string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }
    }
}