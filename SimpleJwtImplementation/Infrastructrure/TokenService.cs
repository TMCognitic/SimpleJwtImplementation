using Microsoft.IdentityModel.Tokens;
using SimpleJwtImplementation.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
             );

            string token = new JwtSecurityTokenHandler().WriteToken(Token);

            return token;

        }
    }
}
