using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Models.Classes;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;

public static class BettingBoxExtensions
{
    //unnecesary, can be substituted with indexing.
    //public static Option<Hand> FindHand(this BettingBox box, int Idx) => box.Hands.ElementAtOrDefault(Idx);

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

    public static void PlaceBet(this BettingBox box, Player player, double amount)
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
        box.Hands.Add(HandService.CreateEmptyPlayerHand());
    }

}
