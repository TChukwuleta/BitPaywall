using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Infrastructure.Utility
{
    public static class ExtensionMethods
    {
        public static JwtSecurityToken ExtractToken(this string str)
        {
            var stream = str;
            if (str.Contains("Bearer"))
            {
                stream = str.Remove(0, 7);
            }
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var token = jsonToken as JwtSecurityToken;

            return token;
        }

        public static bool ValidateToken(this JwtSecurityToken accessToken, string userId)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var tokenId = accessToken.Claims.First(claim => claim.Type == "userid")?.Value;
                //  var roleAccessLevel = accessToken.Claims.First(claim => claim.Type == "roleaccesslevel")?.Value;
                if (userId != tokenId)
                {
                    throw new Exception("Invalid Token Credentials");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckAccessLevels(this JwtSecurityToken accessToken)
        {
            try
            {
                var roleAccessLevel = accessToken.Claims.First(claim => claim.Type == "roleaccesslevel")?.Value;
                if (roleAccessLevel != UserAccessLevel.Admin.ToString() || roleAccessLevel != UserAccessLevel.SuperAdmin.ToString())
                {
                    //To be uncommented later
                    if (roleAccessLevel != UserAccessLevel.SuperAdmin.ToString())
                    {
                        throw new Exception("Access Denied");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
