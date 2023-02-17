using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Todo.Database;
using Todo.Service.Services;
using Todo.WebAPi.Cors;
using Todo.WebAPi.Helpers;

namespace Todo.WebAPi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddLogging();
            builder.Services.AddCors();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new ApiDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
                });
           
            
            // Database
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Services
            builder.Services.AddScoped<ITodo, Service.Services.TodoService>();
            builder.Services.AddScoped<ITags, Service.Services.Tags>();
            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            });
            
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }
            
            
            app.UseRouting();

            var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();
            app.UseCors(policyBuilder =>
            {
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowCredentials();
                policyBuilder.AllowAnyHeader();
                policyBuilder.WithOrigins(corsSettings.AllowedOrigins);
                policyBuilder.SetPreflightMaxAge(TimeSpan.FromDays(1));
            });

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();
            
            app.Run();
        }
    }
}

