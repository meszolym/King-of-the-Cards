using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Extensions;

public static class PlayerExtensions
{
    public static Player WithBalance(this Player player, double balance)
    {
        player.Balance = balance;
        return player;
    }
}