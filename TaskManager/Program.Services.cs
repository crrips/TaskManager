using TaskManager.Services;

namespace TaskManager
{
    public partial class Program
    {
        static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IHashService, HashService>();
        }
    }
}
