using KC.Models.Classes;
using KC.Models.Enums;
using KC.Models.Structs;
using LanguageExt;
namespace KC.Logic.ShoeLogic;

public class ShoeService
{
    public static Shoe CreateShoe(int numberOfDecks) =>
        new Shoe(new Seq<Card>(Enumerable.Range(0, numberOfDecks).SelectMany(i => GetDeck())));

    public static IEnumerable<Card> GetDeck() => Enum.GetValues<CardSuit>()
        .SelectMany(suit => Enum.GetValues<CardFace>().Select(face => new Card(suit, face)));
}