using KC.Backend.Models;

namespace KC.Backend.Logic.GameItemsLogic;

public static class CardShoeUtilities
{
    internal static CardShoe CreateUnshuffledShoe(uint numberOfDecks) =>
       new CardShoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

    private static IEnumerable<Card> GetDeck() => Enum.GetValues<Card.CardSuit>().Where(s => s != Card.CardSuit.None)
        .SelectMany(suit => Enum.GetValues<Card.CardFace>().Select(face => new Card {Face = face, Suit = suit}));

    public static void Shuffle(this CardShoe shoe, Random random)
    {
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    public static Card TakeCard(this CardShoe shoe) => shoe.Cards[shoe.NextCardIdx++];
}