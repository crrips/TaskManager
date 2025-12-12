using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskManager
{
    public partial class Program
    {
        static void RegisterIdentity(WebApplicationBuilder builder)
        {
			var jwtKey = builder.Configuration["Jwt:Key"];

			if (string.IsNullOrEmpty(jwtKey))
				throw new Exception("Jwt:Key is missing in appsettings.json!");

			var key = Encoding.UTF8.GetBytes(jwtKey);

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidateAudience = false,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateLifetime = true,
						ClockSkew = TimeSpan.Zero
					};

					// Add diagnostics to see exact reason of invalid_token during development
					options.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = context =>
						{
							Console.WriteLine("[JWT] Authentication failed: " + context.Exception);
							return Task.CompletedTask;
						}
					};
				});

			// Identity related service registrations would go here
		}

		static void UseIdentity(WebApplication app)
		{
			app.UseAuthentication();
			app.UseAuthorization();
        }	
    }
}
