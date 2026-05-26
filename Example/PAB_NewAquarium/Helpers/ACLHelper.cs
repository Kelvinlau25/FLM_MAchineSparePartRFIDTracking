using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;

namespace PAB_NewAquarium.Helpers
{
    public class ACLHelper
    {
        public static ACL_UserObj JWTTokenToACLObj(string JWTToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(JWTToken);
            var token = jsonToken as JwtSecurityToken;


            ACL_UserObj ACL_UserObj = new ACL_UserObj();
            ACL_UserObj.ID_MM_EMPLOYEE = token.Claims.FirstOrDefault(claim => claim.Type == "ID_MM_EMPLOYEE") == null ? -1 : Convert.ToInt32(token.Claims.FirstOrDefault(claim => claim.Type == "ID_MM_EMPLOYEE").Value);
            ACL_UserObj.ID_ACL_USER = token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_USER") == null ? -1 : Convert.ToInt32(token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_USER").Value);
            ACL_UserObj.ID_ACL_ROLE = token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_ROLE") == null ? -1 : Convert.ToInt32(token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_ROLE").Value);
            ACL_UserObj.ID_ACL_RESOURCE = token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_RESOURCE") == null ? -1 : Convert.ToInt32(token.Claims.FirstOrDefault(claim => claim.Type == "ID_ACL_RESOURCE").Value);
            ACL_UserObj.USER_ID = token.Claims.FirstOrDefault(claim => claim.Type == "USER_ID") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "USER_ID").Value;
            ACL_UserObj.USR_EMAIL = token.Claims.FirstOrDefault(claim => claim.Type == "USR_EMAIL") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "USR_EMAIL").Value;
            ACL_UserObj.COMPANY = token.Claims.FirstOrDefault(claim => claim.Type == "COMPANY") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "COMPANY").Value;
            ACL_UserObj.EMP_NO = token.Claims.FirstOrDefault(claim => claim.Type == "EMP_NO") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "EMP_NO").Value;
            ACL_UserObj.EMP_NAME = token.Claims.FirstOrDefault(claim => claim.Type == "EMP_NAME") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "EMP_NAME").Value;
            ACL_UserObj.ROLE_NAME = token.Claims.FirstOrDefault(claim => claim.Type == "ROLE_NAME") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "ROLE_NAME").Value;
            ACL_UserObj.ROLE_DESC = token.Claims.FirstOrDefault(claim => claim.Type == "ROLE_DESC") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "ROLE_DESC").Value;
            ACL_UserObj.RESOURCE_NAME = token.Claims.FirstOrDefault(claim => claim.Type == "RESOURCE_NAME") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "RESOURCE_NAME").Value;
            ACL_UserObj.RESOURCE_DESC = token.Claims.FirstOrDefault(claim => claim.Type == "RESOURCE_DESC") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "RESOURCE_DESC").Value;
            ACL_UserObj.TITLE_MODULE = token.Claims.FirstOrDefault(claim => claim.Type == "TITLE_MODULE") == null ? "" : token.Claims.FirstOrDefault(claim => claim.Type == "TITLE_MODULE").Value;
            return ACL_UserObj;
        }

        public static bool ValidatePassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            var isValidated = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && password.Length >= 6 && hasSymbols.IsMatch(password);
            return isValidated;
        }
    }
}