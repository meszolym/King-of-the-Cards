using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Utilities;

public static class DtoConverter
{
    public static SessionDto ToDto(this Session session) => 
        new SessionDto
        {
            Id = session.Id,
            Table = session.Table.ToDto(),
            CurrentTurnInfo = session.CurrentTurnInfo,
            CanPlaceBets = session.CanPlaceBets,
        };

    public static TableDto ToDto(this Table table) => new TableDto()
    {
        DealerVisibleCards = table.Dealer.DealerVisibleCards,
        BettingBoxes = table.BettingBoxes.Select(b => b.ToDto())
    };
    
    public static BettingBoxDto ToDto(this BettingBox bettingBox) => new BettingBoxDto()
    {
        OwnerId = bettingBox.OwnerId,
        Hands = bettingBox.Hands.Select(h => h.ToDto())
    };
    
    public static HandDto ToDto(this Hand hand) => new HandDto()
    {
        Cards = hand.Cards
    };

}