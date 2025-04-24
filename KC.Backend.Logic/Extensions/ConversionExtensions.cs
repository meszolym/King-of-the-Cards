using System;
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

    public static BettingBoxReadDto ToDto(this BettingBox bettingBox, Func<Guid, string> getPlayerName) =>
        new BettingBoxReadDto(bettingBox.IdxOnTable, bettingBox.OwnerId, getPlayerName(bettingBox.OwnerId), bettingBox.Hands.Select(h => h.ToDto()));

    public static TableReadDto ToDto(this Table table, Func<Guid, string> getPlayerName) =>
        new TableReadDto(table.Dealer.GetVisibleCards(), table.BettingBoxes.Select(b => b.ToDto(getPlayerName)).OrderBy(b => b.BoxIdx));
    
    public static SessionReadDto ToDto(this Session session, Func<Guid, string> getPlayerName) => new SessionReadDto(session.Id, session.Table.ToDto(getPlayerName),
        session.CurrentTurnInfo, session.CanPlaceBets);
}