using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Utilities;

public static class DtoConverter
{
    public static PlayerDto ToDto(this Player player) => new PlayerDto(player.Name, player.Balance);

    public static SessionDto ToDto(this Session session) =>
        new SessionDto(session.Id, session.Table.ToDto(), session.CurrentTurnInfo, session.CanPlaceBets);
    public static TableDto ToDto(this Table table) =>
        new TableDto(table.Dealer.DealerVisibleCards, table.BettingBoxes.Select(b => b.ToDto()));

    public static BettingBoxDto ToDto(this BettingBox bettingBox) =>
        new BettingBoxDto(bettingBox.OwnerId, bettingBox.Hands.Select(h => h.ToDto()));

    public static HandDto ToDto(this Hand hand) => new HandDto(hand.Cards, hand.Bet);

    public static Player ToModel(this PlayerRegisterDto dto) => new Player(dto.MacAddress, dto.Name, 500, string.Empty);

}