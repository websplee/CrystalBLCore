using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace CrystalBLCore.BusinessServices
{
    public class UserService
    {
        public UserService()
        {
        }

        internal static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        internal static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }


        // Get the current user token
        internal static JwtSecurityToken UserToken()
        {
            IHttpContextAccessor httpAccessor = new HttpContextAccessor();
            var handler = new JwtSecurityTokenHandler();
            string authHeader = httpAccessor.HttpContext?.Request.Headers["Authorization"];

            if (authHeader != null && authHeader != "Bearer test token")
            {
                authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(authHeader);
                var tokenString = handler.ReadToken(authHeader) as JwtSecurityToken;

                return tokenString;
            }
            return null;
        }

        // Returen current user Id
        internal static long CurrentUserId()
        {
            if (UserToken() != null)
                return long.Parse(UserToken().Claims.First(claim => claim.Type == "nameid").Value);          
            else return 0;
        }
        
        // Returen current employee Id
        internal static long CurrentEmployeeId()
        {
            if (UserToken() != null)
                return long.Parse(UserToken().Claims.First(claim => claim.Type == "employeeId").Value);
            else return 0;
        }
        
        // Returen current user department Id
        internal static long CurrentUserDepartmentId()
        {
            if (UserToken() != null)
                return long.Parse(UserToken().Claims.First(claim => claim.Type == "departmentId").Value);
            else return 0;
        }

        // Returen current user roles 
        internal static string[] CurrentUserRole()
        {
            string[] claimList = new string[UserToken().Claims.Where(claim => claim.Type == "role").Count()];

            if (UserToken() != null)
            {
                int i = 0;
                foreach (var claim in UserToken().Claims.Where(claim => claim.Type == "role"))
                {
                    claimList[i] = claim.Value;
                    i++;
                }
                return claimList;
            }
            //return UserToken().Claims.First(claim => claim.Type == "role").Value;
            else return null;
        }
        
        // Returen user modules
        /*internal static string CurrentUserModules()
        {
            if (UserToken() != null)
                return UserToken().Claims.First(claim => claim.Type == "modules").Value;
            else return null;
        }*/
    }
}
