using KC.App.Data;
using KC.App.Logic.Interfaces;
using KC.App.Logic.SessionLogic;
using KC.App.Logic.PlayerLogic;
using KC.App.Models.Classes;

namespace KC.App.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            //app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
