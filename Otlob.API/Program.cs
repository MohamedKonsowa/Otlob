namespace Otlob.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddServices();
            builder.Services.AddDbContexts(builder);

            //Options Pattern
            JwtOptions? jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new();
            builder.Services.AddSingleton(jwtOptions);

            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    //ClockSkew = TimeSpan.Zero,
                    //LifetimeValidator = (notBefore, expires, token, parameters) =>
                    //{
                    //    if (expires != null)
                    //    {
                    //        return expires > DateTime.UtcNow;
                    //    }
                    //    return false;
                    //}
                };
            });

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var identityContext = serviceProvider.GetRequiredService<IdentityContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                await dbContext.Database.MigrateAsync();
                await identityContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
                Console.WriteLine(ex.Message);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
