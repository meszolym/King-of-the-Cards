using KC.App.Models.Classes;
using KC.App.Models.Enums;
using KC.App.Models.Structs;
namespace KC.App.Logic.SessionLogic.ShoeLogic;

internal class ShoeService
{
    internal static Shoe CreateUnshuffledShoe(uint numberOfDecks) =>
        new Shoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

    private static IEnumerable<Card> GetDeck() => Enum.GetValues<CardSuit>().Where(s => s != CardSuit.None)
        .SelectMany(suit => Enum.GetValues<CardFace>().Select(face => new Card(suit, face)));
}