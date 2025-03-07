using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface ICardShoeLogic
{
    CardShoe CreateUnshuffledShoe(uint numberOfDecks);
    void Shuffle(ref CardShoe shoe, Random random);
    Card TakeCard(CardShoe shoe);
}