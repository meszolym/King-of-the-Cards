using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

public class DealerLogic(IList<Session> sessions, IRuleBook ruleBook) : IDealerLogic
{
    public void Shuffle(Guid sessionId, Random? random = null)
    {
        random ??= Random.Shared;
        
        var shoe = sessions.Single(s => s.Id == sessionId).Table.Shoe;
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    public Card TakeCard(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        return session.Table.Shoe.Cards[session.Table.Shoe.NextCardIdx++];
    }

    public void DealerPlayHand(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (session.CurrentTurnInfo.PlayersTurn) throw new InvalidOperationException("It's not the dealer's turn.");
        if (session.Table.Dealer.DealerHand.Finished) throw new InvalidOperationException("Dealer's hand is already finished.");
        
        session.Table.Dealer.DealerHand.Finished = true;
        var dealerHand = session.Table.Dealer.DealerHand;

        while (ruleBook.DealerShouldHit(dealerHand))
        {
            dealerHand.Cards.Add(TakeCard(sessionId));
        }
        
        dealerHand.Finished = true;
        
    }
    
    public void DealStartingCards(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        //if shoe needs shuffling, shuffle
        if (session.Table.Shoe.ShuffleCardIdx <= session.Table.Shoe.NextCardIdx)
        {
            Shuffle(sessionId);
        }

        //deal cards
        for (int i = 0; i < 2; i++)
        {
            foreach (BettingBox box in session.Table.BettingBoxes.Where(b => b.Hands[0].Bet > 0))
            {
                box.Hands[0].Cards.Add(TakeCard(sessionId));
            }
            session.Table.Dealer.DealerHand.Cards.Add(TakeCard(sessionId));
        }
    }

    public bool DealerCheck(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!ruleBook.GetValue(session.Table.Dealer.DealerHand).IsBlackJack) return false;
        session.Table.Dealer.DealerShowsAllCards = true;
        session.Table.Dealer.DealerHand.Finished = true;
        return true;
    }
}