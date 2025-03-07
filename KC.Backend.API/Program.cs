using KC.Backend.Data;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Logic.SessionLogic;
using KC.Backend.Logic.PlayerLogic;
using Microsoft.AspNetCore.Diagnostics;
using KC.App.Backend.Models.Classes;
using KC.Backend.Models;
using KC.Backend.Models.GameManagement;


namespace KC.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            List<Player> players = [];
            List<Session> sessions = [];

            builder.Services.AddSingleton(players);
            builder.Services.AddSingleton(sessions);

            builder.Services.AddTransient<IDataStore<Player, string>, DataStore<Player, string>>();
            builder.Services.AddTransient<IDataStore<Session, Guid>, DataStore<Session, Guid>>();
            builder.Services.AddTransient<IPlayerLogic, PlayerLogic>();
            builder.Services.AddTransient<ISessionLogic, SessionLogic>();

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            //app.UseAuthorization();

            app.MapControllers();
            app.MapHub<SignalRHub>("/signalR");

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var ex = context.Features
                    .Get<IExceptionHandlerPathFeature>()
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
