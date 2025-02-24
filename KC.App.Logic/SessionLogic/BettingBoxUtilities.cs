using KC.App.Backend.Models.Classes;
using KC.App.Logic.SessionLogic;

namespace KC.App.Logic.SessionLogic;
public static class BettingBoxUtilities
{
    internal static BettingBox CreateEmptyBettingBox(int idx) => new(idx, [HandUtilities.CreateEmptyPlayerHand()], null);
    
    public static void Claim(this BettingBox box, Player player)
    {
        if (!box.CheckNoOwner())
        {
            throw new InvalidOperationException("Box already has an owner.");
        }
        box.Owner = player;
    }

    public static void Disclaim(this BettingBox box, Player player)
    {
        if (!box.CheckOwner(player))
        {
            throw new InvalidOperationException("Player does not own this box.");
        }
        box.Owner = null;
    }

    public static void UpdateBet(this BettingBox box, Player player, double amount)
    {
        if (!box.CheckOwner(player))
        {
            throw new InvalidOperationException("Player does not own this box.");
        }

        if (amount < 0)
        {
            throw new ArgumentException("Bet cannot be less than 0.");
        }

        box.Hands[0].Bet = amount;
    }

    public static bool CheckOwner(this BettingBox box, Player player) => box.Owner is not null && box.Owner.Equals(player);

    public static bool CheckNoOwner(this BettingBox box) => box.Owner is null;

    public static void ClearHands(this BettingBox box)
    {
        box.Hands.Clear();
        box.Hands.Add(HandUtilities.CreateEmptyPlayerHand());
    }

}
