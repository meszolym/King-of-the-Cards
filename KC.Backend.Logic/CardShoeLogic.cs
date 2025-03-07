using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic;

public class CardShoeLogic : ICardShoeLogic
{
    public CardShoe CreateUnshuffledShoe(uint numberOfDecks) =>
        new CardShoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

    private IEnumerable<Card> GetDeck() => Enum.GetValues<Card.CardSuit>().Where(s => s != Card.CardSuit.None)
        .SelectMany(suit => Enum.GetValues<Card.CardFace>().Select(face => new Card {Face = face, Suit = suit}));

    public void Shuffle(ref CardShoe shoe, Random random)
    {
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    public Card TakeCard(CardShoe shoe) => shoe.Cards[shoe.NextCardIdx++];
}