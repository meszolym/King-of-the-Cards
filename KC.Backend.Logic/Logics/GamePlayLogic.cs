using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Core.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.GameManagement;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics;

//TODO: Make the logic more atomic, chaining them together will be handled by the API layer.
public class GamePlayLogic(IList<Session> sessions, IDictionary<MacAddress, Guid> macToPlayerGuid, IRuleBook ruleBook) : IGamePlayLogic
{
    /// <summary>
    /// Shuffles the shoe of the table in the session.
    /// </summary>
    public void Shuffle(Guid sessionId, Random? random = null)
    {
        random ??= Random.Shared;
        var session = sessions.Single(s => s.Id == sessionId);
        session.DestructionTimer.Reset();

        var shoe = session.Table.Shoe;
        
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    /// <summary>
    /// Shuffles the shoe of the table in the session if the shuffle card was reached.
    /// </summary>
    public bool ShuffleIfNeeded(Guid sessionId, Random? random = null)
    {
        random ??= Random.Shared;
        var session = sessions.Single(s => s.Id == sessionId);
        if (session.Table.Shoe.ShuffleCardIdx > session.Table.Shoe.NextCardIdx) return false;
        
        Shuffle(sessionId, random);
        return true;
    }

    /// <summary>
    /// Gives a card from the shoe of the session.
    /// </summary>
    /// <returns></returns>
    public Card GiveCard(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        return session.Table.Shoe.Cards[session.Table.Shoe.NextCardIdx++];
    }

    /// <summary>
    /// Plays dealer's hand according to the rules.
    /// </summary>
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
    /// Deals cards to the players and the dealer at the start of a round (only 1 per hand, so THIS HAS TO BE CALLED TWICE).
    /// </summary>
    /// <exception cref="InvalidOperationException">Shoe needs shuffling.</exception>
    public void DealHalfOfStartingCards(Guid sessionId, bool checkShuffle = false)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        session.DestructionTimer.Reset();
        
        //if shoe needs shuffling, throw exception
        if (checkShuffle && session.Table.Shoe.ShuffleCardIdx <= session.Table.Shoe.NextCardIdx)
        {
            throw new InvalidOperationException("Shoe needs shuffling.");
        }
        
        foreach (var box in session.Table.BettingBoxes.Where(b => b.Hands[0].Bet > 0))
        {
            box.Hands[0].Cards.Add(GiveCard(sessionId));
        }
        session.Table.Dealer.DealerHand.Cards.Add(GiveCard(sessionId));
    }

    /// <summary>
    /// Checks for dealer blackjack.
    /// </summary>
    /// <returns>True if the dealer has blackjack</returns>
    public bool DealerCheck(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!ruleBook.GetValue(session.Table.Dealer.DealerHand).IsBlackJack) return false;
        session.Table.Dealer.DealerShowsAllCards = true;
        session.Table.Dealer.DealerHand.Finished = true;
        return true;
    }
    
    public IEnumerable<Move> GetPossibleActionsOnHand(Hand hand) => ruleBook.GetPossibleActionsOnHand(hand);
    
    /// <summary>
    /// Makes a move on a given hand of a given player on a given box. Does not handle player balance, hand bets or transferring turns.
    /// </summary>
    /// <exception cref="InvalidOperationException">"The hand is not in turn."</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player."</exception>
    /// <exception cref="InvalidOperationException">"Action not possible." if the rulebook states that this action is not possible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If move is not handled.</exception>
    public void MakeMove(Guid sessionId, int boxIdx, MacAddress playerId, Move move, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CurrentTurnInfo.PlayersTurn || session.CurrentTurnInfo.BoxIdx != boxIdx || session.CurrentTurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand is not in turn.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != macToPlayerGuid[playerId]) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];
        if (!GetPossibleActionsOnHand(hand).Contains(move)) throw new InvalidOperationException("Action not possible.");
        
        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
            case Move.Double:
                hand.Cards.Add(GiveCard(sessionId));
                break;
            case Move.Split:
                box.Hands.Add(new Hand(){Cards = [hand.Cards[1]], FromSplit = true});
                hand.Cards.RemoveAt(1);
                hand.Cards.Add(GiveCard(sessionId));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if bust, mark hand as finished
        if (ruleBook.GetValue(hand).NumberValue > 21)
            hand.Finished = true;

        session.DestructionTimer.Reset();
    }
        
    private IEnumerable<BettingBox> BoxesInPlay(Guid sessionId) =>
        sessions.Single(s => s.Id == sessionId).Table.BettingBoxes.Where(box => box.Hands[0].Bet > 0);
    
    public void TransferTurn(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        #region dealers turn handling
        //if it's the dealer's turn, transfer to the first player's turn
        if (!session.CurrentTurnInfo.PlayersTurn)
        {
            session.CurrentTurnInfo = new TurnInfo(true, BoxesInPlay(sessionId).First().IdxOnTable, 0);
            return;
        }
        
        #endregion

        #region hand left handling
        var box = session.Table.BettingBoxes[session.CurrentTurnInfo.BoxIdx];
        var hand = box.Hands[session.CurrentTurnInfo.HandIdx];

        //mark hand as finished
        hand.Finished = true;

        //if bust, 0 out bet
        if (ruleBook.GetValue(hand).NumberValue > 21) hand.Bet = 0;

        //if there are more hands left in the box, transfer to the next hand
        if (box.Hands.Count > session.CurrentTurnInfo.HandIdx+1)
        {
            session.CurrentTurnInfo = session.CurrentTurnInfo with { HandIdx = session.CurrentTurnInfo.HandIdx + 1 };
            hand = box.Hands[session.CurrentTurnInfo.HandIdx];

            return;
        }
        #endregion

        #region box left handling
        //if there are more boxes left, transfer to the next box
        var nextBox = BoxesInPlay(sessionId).FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.IdxOnTable > box.IdxOnTable);
        if (nextBox is not null)
        {
            session.CurrentTurnInfo = session.CurrentTurnInfo with { BoxIdx = nextBox.IdxOnTable, HandIdx = 0 };
            return;
        }
        #endregion

        #region no boxes left handling
        //if there are no more boxes left, transfer to the dealer's turn
        session.CurrentTurnInfo = new TurnInfo(); //false, 0, 0 by default
        return;

        #endregion
    }

    public void FinishAllHandsInPlay(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        foreach (var b in BoxesInPlay(sessionId))
        {
            foreach (var h in b.Hands)
            {
                h.Finished = true;
            }
        }
    }
    
    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    public void PayOutBets(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        var dealerHand = session.Table.Dealer.DealerHand;

        foreach (var box in BoxesInPlay(sessionId))
        {
            foreach (var hand in box.Hands)
            {
                if (ruleBook.GetValue(hand).NumberValue > 21) //player bust
                {
                    hand.Bet = 0; //lose bet
                    continue;
                }

                if (ruleBook.GetValue(dealerHand).NumberValue > 21) //dealer bust
                {
                    if (ruleBook.GetValue(hand).IsBlackJack) hand.Bet += hand.Bet * ruleBook.BlackjackPayoutMultiplier; //if player has blackjack, pay out 1.5x bet
                    else hand.Bet += hand.Bet * ruleBook.StandardPayoutMultiplier; //pay out bet
                    continue;
                }

                if (ruleBook.GetValue(dealerHand).IsBlackJack) //dealer has blackjack
                {
                    if (!ruleBook.GetValue(hand).IsBlackJack) hand.Bet = 0; //if player doesn't have blackjack, lose bet, else bet stays the same
                    if (!ruleBook.GetValue(hand).IsBlackJack) hand.Bet += hand.Bet * ruleBook.BjVsBjPayoutMultiplier;
                    continue;
                }

                if (ruleBook.GetValue(hand).IsBlackJack) //player has blackjack
                {
                    hand.Bet += hand.Bet * ruleBook.BlackjackPayoutMultiplier; //if player has blackjack, pay out 1.5x bet
                }

                if (ruleBook.GetValue(hand).NumberValue > ruleBook.GetValue(dealerHand).NumberValue) //player has stronger hand
                {
                    hand.Bet += hand.Bet * ruleBook.StandardPayoutMultiplier; //pay out bet
                    continue;
                }

                if (ruleBook.GetValue(hand).NumberValue < ruleBook.GetValue(dealerHand).NumberValue) //player has weaker hand
                {
                    hand.Bet = 0; //lose bet
                }

                //if same value, bet stays the same
            }
        }
    }
    
    public void ClearHands(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        foreach (var box in session.Table.BettingBoxes)
        {
            box.Hands.Clear();
            box.Hands.Add(new ());
        }
    }
}