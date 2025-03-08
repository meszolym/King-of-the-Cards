using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

//TODO: Make the logic more atomic, chaining them together will be handled by the API layer.
public class GamePlayLogic(IList<Session> sessions, IRuleBook ruleBook) : IGamePlayLogic
{
    /// <summary>
    /// Shuffles the shoe of the table in the session.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="random"></param>
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

    /// <summary>
    /// Gives a card from the shoe of the session.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public Card GiveCard(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        return session.Table.Shoe.Cards[session.Table.Shoe.NextCardIdx++];
    }

    /// <summary>
    /// Plays dealer's hand according to the rules.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <exception cref="InvalidOperationException">"It's not the dealer's turn."</exception>
    /// <exception cref="InvalidOperationException">"Dealer's hand is already finished."</exception>
    public void DealerPlayHand(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (session.CurrentTurnInfo.PlayersTurn) throw new InvalidOperationException("It's not the dealer's turn.");
        if (session.Table.Dealer.DealerHand.Finished) throw new InvalidOperationException("Dealer's hand is already finished.");
        
        session.Table.Dealer.DealerHand.Finished = true;
        var dealerHand = session.Table.Dealer.DealerHand;

        while (ruleBook.DealerShouldHit(dealerHand))
        {
            dealerHand.Cards.Add(GiveCard(sessionId));
        }
        
        dealerHand.Finished = true;
        
    }
    
    /// <summary>
    /// Deals cards to the players and the dealer at the start of a round.
    /// </summary>
    /// <param name="sessionId"></param>
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
                box.Hands[0].Cards.Add(GiveCard(sessionId));
            }
            session.Table.Dealer.DealerHand.Cards.Add(GiveCard(sessionId));
        }
    }

    /// <summary>
    /// Checks for dealer blackjack.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>True if the dealer has blackjack</returns>
    public bool DealerCheck(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!ruleBook.GetValue(session.Table.Dealer.DealerHand).IsBlackJack) return false;
        session.Table.Dealer.DealerShowsAllCards = true;
        session.Table.Dealer.DealerHand.Finished = true;
        return true;
    }
    
        /// <summary>
    /// Makes a move on a given hand of a given player on a given box. Does not handle player balance, hand bets or transferring turns.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <param name="move"></param>
    /// <param name="handIdx"></param>
    /// <exception cref="InvalidOperationException">"The hand is not in turn."</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player."</exception>
    /// <exception cref="InvalidOperationException">"Action not possible." if the rulebook states that this action is not possible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If move is not handled.</exception>
    public void MakeMove(Guid sessionId, int boxIdx, Guid playerId, Move move, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CurrentTurnInfo.PlayersTurn || session.CurrentTurnInfo.BoxIdx != boxIdx || session.CurrentTurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand is not in turn.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];
        if (!ruleBook.GetPossibleActionsOnHand(hand).Contains(move)) throw new InvalidOperationException("Action not possible.");
        
        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                hand.Cards.Add(GiveCard(sessionId));
                break;
            case Move.Double:
                hand.Cards.Add(GiveCard(sessionId));
                break;
            case Move.Split:
                box.Hands.Add(new Hand(){Cards = new List<Card>(){hand.Cards[1]}, FromSplit = true});
                hand.Cards.RemoveAt(1);
                hand.Cards.Add(GiveCard(sessionId));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if bust, mark hand as finished
        if (ruleBook.GetValue(hand).NumberValue > 21)
            hand.Finished = true;

        session.LastMoveMadeAt = DateTime.Now;
    }
}