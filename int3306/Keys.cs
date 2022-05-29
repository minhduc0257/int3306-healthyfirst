using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace int3306
{
    public static class Keys
    {
        public static readonly SymmetricSecurityKey SigningKey = new(
            Aes.Create().EncryptEcb(Encoding.UTF8.GetBytes("int3306"), PaddingMode.PKCS7)
        );
        
        public static readonly TokenValidationParameters TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "int3306",
            ValidAudience = "int3306",
            IssuerSigningKey = SigningKey
        };
    }
}