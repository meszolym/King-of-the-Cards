using KC.Backend.API.Services;
using Microsoft.AspNetCore.Diagnostics;
using KC.Backend.Logic;
using KC.Backend.Logic.Interfaces;
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
            
            builder.Services.AddSingleton<IList<Player>>(players);
            builder.Services.AddSingleton<IList<Session>>(sessions);
            
            builder.Services.AddTransient<IBettingBoxLogic, BettingBoxLogic>();
            builder.Services.AddTransient<IGamePlayLogic, GamePlayLogic>();
            builder.Services.AddTransient<IPlayerLogic, PlayerLogic>();
            builder.Services.AddTransient<IRuleBook, RuleBook>();
            builder.Services.AddTransient<ISessionLogic, SessionLogic>();
            
            builder.Services.AddSignalR();
            builder.Services.AddTransient<IClientCommunicator, SignalRHub>();

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

        //TODO: Remove
        private static class GenerateSeed
        {
            #region Generate seed for session

            private static CardShoe CreateUnshuffledShoe(uint numberOfDecks) =>
                new CardShoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

            private static IEnumerable<Card> GetDeck() => Enum.GetValues<Card.CardSuit>().Where(s => s != Card.CardSuit.None)
                .SelectMany(suit => Enum.GetValues<Card.CardFace>().Select(face => new Card {Face = face, Suit = suit}));
    
            /// <summary>
            /// Creates an empty session. Make sure to subscribe to events of the timer.
            /// </summary>
            /// <param name="numberOfBoxes"></param>
            /// <param name="numberOfDecks"></param>
            /// <param name="shuffleCardPlacement"></param>
            /// <param name="shuffleCardRange"></param>
            /// <param name="bettingTimerSeconds"></param>
            /// <param name="random"></param>
            /// <returns></returns>
            public static Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, int shuffleCardRange, int bettingTimerSeconds, Random? random = null)
            {
                random ??= Random.Shared;
                var shoe = CreateUnshuffledShoe(numberOfDecks);
                shuffleCardPlacement = random.Next(shuffleCardPlacement - shuffleCardRange, shuffleCardPlacement + shuffleCardRange);
                shoe.ShuffleCardIdx = shuffleCardPlacement;
        
                var table = new Table((int)numberOfBoxes, shoe);
                
                table.BettingBoxes[0].Hands[0].Cards.Add(Card.WithSuitAndFace(Card.CardSuit.Spades, Card.CardFace.Ace));
                
                var sess = new Session(table, TimeSpan.FromSeconds(bettingTimerSeconds), TimeSpan.FromMinutes(5));
            
                return sess;
            }

            #endregion
        }


    }
}
