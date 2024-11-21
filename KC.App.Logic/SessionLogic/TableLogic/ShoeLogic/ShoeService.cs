using KC.App.Models.Classes;
using KC.App.Models.Enums;
using KC.App.Models.Structs;
using LanguageExt;
namespace KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;

public class ShoeService
{
    public static Shoe CreateUnshuffledShoe(uint numberOfDecks) =>
        new Shoe(new Seq<Card>(Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())));

    public static IEnumerable<Card> GetDeck() => Enum.GetValues<CardSuit>().Where(s => s != CardSuit.None)
        .SelectMany(suit => Enum.GetValues<CardFace>().Select(face => new Card(suit, face)));
}