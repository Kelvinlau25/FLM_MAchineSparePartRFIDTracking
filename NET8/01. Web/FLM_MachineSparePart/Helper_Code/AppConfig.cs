using Microsoft.Extensions.Configuration;

namespace FILM_Sparepart_MVC.Helper_Code
{
    public static class AppConfig
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetConnectionString(string name)
        {
            return _configuration?.GetConnectionString(name) ?? string.Empty;
        }

        public static string GetAppSetting(string key)
        {
            return _configuration?[key] ?? string.Empty;
        }
    }
}
