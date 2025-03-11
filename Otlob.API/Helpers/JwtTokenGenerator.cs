namespace Otlob.API.Helpers
{
    public static class JwtTokenGenerator
    {
        public static string GenerateToken(JwtOptions jwtOptions, AppUser user)
        {
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryInMinutes),
                SigningCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(
                [
                         new Claim(ClaimTypes.Email, user.Email??string.Empty),
                         new Claim(ClaimTypes.Name, user.UserName??string.Empty)
                     ]),
            };
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
