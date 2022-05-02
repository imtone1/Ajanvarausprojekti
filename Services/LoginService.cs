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


    }
}