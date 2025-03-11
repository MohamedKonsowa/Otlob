namespace Otlob.API.Options
{
    // This class is used to store the JWT options from the appsettings.json file (Options Pattern)
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}