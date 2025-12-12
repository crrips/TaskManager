using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager
{
    public partial class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            RegisterServices(builder);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            
            RegisterSwagger(builder);

            RegisterIdentity(builder);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API V1");
            });

            app.UseRouting();

            UseIdentity(app);

            await using (var scope = app.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
            }

            app.MapControllers();

            await app.RunAsync();
        }
    }
}