using System.Linq;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;

namespace KC.Backend.Logic.Extensions;

public static class ConversionExtensions
{
    public static Player ToDomain(this PlayerRegisterDto registerDto) => new Player(registerDto.Name, 0, string.Empty);

    public static PlayerReadDto ToDto(this Player player) => new PlayerReadDto(player.Id, player.Name, player.Balance);
    public static HandReadDto ToDto(this Hand hand) => new HandReadDto(hand.Cards, hand.Bet);

    public static BettingBoxReadDto ToDto(this BettingBox bettingBox) =>
        new BettingBoxReadDto(bettingBox.IdxOnTable, bettingBox.OwnerId, bettingBox.Hands.Select(h => h.ToDto()));

    public static TableReadDto ToDto(this Table table) =>
        new TableReadDto(table.Dealer.DealerVisibleCards, table.BettingBoxes.Select(b => b.ToDto()).OrderBy(b => b.BoxIdx));
    
    public static SessionReadDto ToDto(this Session session) => new SessionReadDto(session.Id, session.Table.ToDto(),
        session.CurrentTurnInfo, session.CanPlaceBets);
}