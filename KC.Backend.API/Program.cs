using Microsoft.AspNetCore.Diagnostics;
using KC.Backend.Logic;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;


namespace KC.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            List<Player> players = [];
            List<Session> sessions = [];

            builder.Services.AddSingleton(players);
            builder.Services.AddSingleton(sessions);

            builder.Services.AddTransient<IList<Player>, List<Player>>();
            builder.Services.AddTransient<IList<Session>, List<Session>>();
            builder.Services.AddTransient<IBettingBoxLogic, BettingBoxLogic>();
            builder.Services.AddTransient<IGamePlayLogic, GamePlayLogic>();
            builder.Services.AddTransient<IPlayerLogic, PlayerLogic>();
            builder.Services.AddTransient<IRuleBook, RuleBook>();
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
