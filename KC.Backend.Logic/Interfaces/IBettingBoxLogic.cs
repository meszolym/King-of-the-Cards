namespace KC.Backend.Logic.Interfaces;

public interface IBettingBoxLogic
{
    void Claim(Guid sessionId, int boxIdx, Guid playerId);
    void DisclaimBox(Guid sessionId, int boxIdx, Guid playerId);
    void UpdateBet(Guid sessionId, int boxIdx, Guid playerId, double amount, int handIdx = 0);
    void ClearHands(Guid sessionId, int boxIdx);
}