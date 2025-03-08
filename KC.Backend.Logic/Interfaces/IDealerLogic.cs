using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IDealerLogic
{
    void Shuffle(Guid sessionId, Random? random = null);
    Card GiveCard(Guid sessionId);
    void DealerPlayHand(Guid sessionId);
    void DealStartingCards(Guid sessionId);
    bool DealerCheck(Guid sessionId);
}