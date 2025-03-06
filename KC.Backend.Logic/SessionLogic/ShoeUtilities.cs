using KC.App.Backend.Models.Classes;
using KC.App.Backend.Models.Enums;
using KC.App.Backend.Models.Structs;

namespace KC.Backend.Logic.SessionLogic;

public static class ShoeUtilities
{
    internal static Shoe CreateUnshuffledShoe(uint numberOfDecks) =>
       new Shoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

    private static IEnumerable<Card> GetDeck() => Enum.GetValues<CardSuit>().Where(s => s != CardSuit.None)
        .SelectMany(suit => Enum.GetValues<CardFace>().Select(face => new Card(suit, face)));

    public static void Shuffle(this Shoe shoe, Random random)
    {
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    public static Card TakeCard(this Shoe shoe) => shoe.Cards[shoe.NextCardIdx++];
}