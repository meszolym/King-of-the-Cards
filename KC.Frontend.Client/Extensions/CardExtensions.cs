using System;
using System.Text;
using KC.Shared.Models.GameItems;

namespace KC.Frontend.Client.Extensions;

public static class CardExtensions
{
    public static Uri ImagePath(this Card card)
    {
        var sb = new StringBuilder();
        sb.Append("avares://KC.Frontend.Client/Assets/Cards/card");

        if (card.Face == Card.CardFace.None)
        {
            sb.Append("Back_blue5");
        }
        else
        {
            sb.Append(card.Suit.ToString());
            string face = card.Face switch
            {
                Card.CardFace.King => "K",
                Card.CardFace.Queen => "Q",
                Card.CardFace.Jack => "J",
                Card.CardFace.Ace => "A",
                _ => ((int)card.Face).ToString()
            };
            sb.Append(face);
        }
        sb.Append(".png");
        return new Uri(sb.ToString());
    }
}