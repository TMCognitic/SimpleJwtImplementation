using Microsoft.IdentityModel.Tokens;
using SimpleJwtImplementation.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimpleJwtImplementation.Infrastructrure
{
    public class TokenService : ITokenService
    {
        private const string _privateKey = "MaSuperCléPrivéeDeLaMortQuiTueOuPas!!!";

        public string GenerateToken(User user)
        {
            byte[] secretKey = Encoding.Default.GetBytes(_privateKey);
            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(secretKey);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            JwtSecurityToken Token = new JwtSecurityToken(
                issuer: "https://localhost:7079",
                audience: "https://localhost:7079",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
             );

            string token = new JwtSecurityTokenHandler().WriteToken(Token);

            return token;

        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public int GetUserIdFromToken(string token)
        {
            JwtSecurityToken securityToken = new JwtSecurityToken(token);
            Claim? sid = securityToken.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);

            if (sid is null)
                throw new InvalidOperationException("No Sid found");

            return int.Parse(sid.Value);
        }
    }
}
