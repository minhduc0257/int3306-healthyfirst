using Newtonsoft.Json;

namespace int3306
{
    public class OAuth2TokenObject
    {
        [JsonProperty("token")]
        public string Token { get; set; } 
    }
}