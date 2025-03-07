using KC.Backend.Models;

namespace KC.Backend.Logic.GameItemsLogic;
public static class BettingBoxUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="box"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Claim(this BettingBox box, Guid playerId)
    {
        if (box.OwnerId != Guid.Empty )
        {
            throw new InvalidOperationException("Box already has an owner.");
        }
        box.OwnerId = playerId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="box"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Disclaim(this BettingBox box, Guid playerId)
    {
        if (box.OwnerId != playerId)
        {
            throw new InvalidOperationException("Player does not own this box.");
        }
        box.OwnerId = Guid.Empty;
    }

/// <summary>
/// 
/// </summary>
/// <param name="box"></param>
/// <param name="playerId"></param>
/// <param name="amount"></param>
/// <param name="handIdx"></param>
/// <returns>Difference between old and new balance</returns>
/// <exception cref="InvalidOperationException"></exception>
/// <exception cref="ArgumentException"></exception>
    public static void UpdateBet(this BettingBox box, Guid playerId, double amount, int handIdx = 0)
    {
        if (box.OwnerId != playerId)
        {
            throw new InvalidOperationException("Player does not own this box.");
        }

        if (amount < 0)
        {
            throw new ArgumentException("Bet cannot be less than 0.");
        }
        
        var oldBet = box.Hands[handIdx].Bet;
        box.Hands[handIdx].Bet = amount;
    }

    public static void ClearHands(this BettingBox box)
    {
        box.Hands.Clear();
        box.Hands.Add(new ());
    }

}
