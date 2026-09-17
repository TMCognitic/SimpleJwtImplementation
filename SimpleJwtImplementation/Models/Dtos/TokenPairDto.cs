namespace SimpleJwtImplementation.Models.Dtos
{
    public class TokenPairDto(string token, string refreshToken)
    {
        public string Token { get; } = token;
        public string RefreshToken { get; } = refreshToken;
    }
}
