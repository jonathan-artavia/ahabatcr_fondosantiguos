using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Activities.Statements;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fondos_Antiguos
{
    public class DataSecurity
    {
        public static string Hash(string text) => Encoding.UTF8.GetString(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text)));

        public static bool Verify(string baseValue, string provided)
        {
            byte[] hashedProv = System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(provided));
            return hashedProv.SequenceEqual(Encoding.UTF8.GetBytes(baseValue));
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            return FaUserManager.IsValid(user.UserName, Encoding.UTF8.GetBytes(password), user) ? IdentityResult.Success : IdentityResult.Failed("Inicio fallido");
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 12,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };


            //Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(GetInt32(0, chars.Count),
                    randomChars[0][GetInt32(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(GetInt32(0, chars.Count),
                    randomChars[1][GetInt32(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(GetInt32(0, chars.Count),
                    randomChars[2][GetInt32(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric) //se cae aqui
                chars.Insert(GetInt32(0, chars.Count),
                    randomChars[3][GetInt32(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[GetInt32(0, randomChars.Length)];
                chars.Insert(GetInt32(0, chars.Count),
                    rcs[GetInt32(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public static Int32 GetInt32(int start, int finish)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);

            if (closestNumber(finish, 10) > 0)
                return Math.Abs(start + ((int)BitConverter.ToUInt32(byteArray, 0) % (int)Math.Ceiling((decimal)finish)));
            return 0;
        }

        static int closestNumber(int n, int m)
        {
            // find the quotient 
            int q = n / m;

            // 1st possible closest number 
            int n1 = m * q;

            // 2nd possible closest number 
            int n2 = (n * m) > 0 ? (m * (q + 1)) : (m * (q - 1));

            // if true, then n1 is the required closest number 
            if (Math.Abs(n - n1) < Math.Abs(n - n2))
                return n1;

            // else n2 is the required closest number     
            return n2;
        }


    }

    public class PasswordOptions
    {
        public PasswordOptions()
        {
        }

        public int RequiredLength { get; set; }
        public int RequiredUniqueChars { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
    }
}