using KC.Models.Classes;
using KC.Models.Enums;
using KC.Models.Structs;

namespace KC.Logic.ShoeLogic;

public class ShoeService
{
    public static Shoe CreateShoe(int numberOfDecks) =>
        new Shoe(Enumerable.Range(0, numberOfDecks).SelectMany(i => GetDeck()).ToArray());

    public static IEnumerable<Card> GetDeck() => Enum.GetValues<CardSuit>()
        .SelectMany(suit => Enum.GetValues<CardFace>().Select(face => new Card(suit, face)));
}