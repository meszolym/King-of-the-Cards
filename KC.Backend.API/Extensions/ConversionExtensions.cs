using KC.Backend.Logic;
using KC.Backend.Logic.Extensions;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Extensions;
public static class ConversionExtensions
{
    public static PlayerReadDto ToDto(this Player player) => new PlayerReadDto(player.Id, player.Name, player.Balance);
    public static HandReadDto ToDto(this Hand hand) => new HandReadDto(hand.Cards, hand.Bet);

    public static BettingBoxReadDto ToDto(this BettingBox bettingBox, GetPlayerNameDelegate getPlayerName) =>
        new BettingBoxReadDto(bettingBox.IdxOnTable, bettingBox.OwnerId, getPlayerName(bettingBox.OwnerId), bettingBox.Hands.Select(h => h.ToDto()));

    public static TableReadDto ToDto(this Table table, GetPlayerNameDelegate getPlayerName) =>
        new TableReadDto(table.Dealer.GetVisibleCards(), table.BettingBoxes.Select(b => b.ToDto(getPlayerName)).OrderBy(b => b.BoxIdx));
    
    public static SessionReadDto ToDto(this Session session, GetPlayerNameDelegate getPlayerName) => new SessionReadDto(session.Id, session.Table.ToDto(getPlayerName),
        session.CurrentTurnInfo, session.CanPlaceBets);
}