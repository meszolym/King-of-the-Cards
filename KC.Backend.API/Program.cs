using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using KC.Backend.Logic;
using KC.Backend.Logic.Core;
using KC.Backend.Logic.Core.Interfaces;
using KC.Backend.Logic.Logics;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services;
using KC.Backend.Logic.Services.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;
using Scalar.AspNetCore;


namespace KC.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            List<Player> players = [];
            List<Session> sessions = [];
            Dictionary<MacAddress, Guid> macToPlayerId = [];
            
            //repositories
            builder.Services.AddSingleton<IList<Player>>(players);
            builder.Services.AddSingleton<IList<Session>>(sessions);
            builder.Services.AddSingleton<IDictionary<MacAddress, Guid>>(macToPlayerId);
            
            builder.Services.RegisterLogicLayerServices().RegisterApiLayerServices().RegisterDelegates();
            
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "/openapi/{documentName}.json";
                });
                app.MapScalarApiReference(options =>
                {
                    options.WithTheme(ScalarTheme.Kepler);
                    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            //app.UseAuthorization();

            app.MapControllers();
            app.MapHub<SignalRHub>("/signalR");

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var ex = context.Features
                    .Get<IExceptionHandlerPathFeature>()!
                    .Error;
                var resp = new
                {
                    Type = ex.GetType().Name,
                    Msg = ex.Message
                };
                await context.Response.WriteAsJsonAsync(resp);

            }));
            app.Run();
        }
    }
}
