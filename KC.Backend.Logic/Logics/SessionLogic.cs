using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.GameManagement;

namespace KC.Backend.Logic.Logics;

//TODO: Make the logic more atomic, chaining them together will be handled by the API layer.
public class SessionLogic(IList<Session> sessions, IRuleBook ruleBook) : ISessionLogic
{
    private static CardShoe CreateUnshuffledShoe(uint numberOfDecks) =>
        new CardShoe([.. Enumerable.Range(0, (int)numberOfDecks).SelectMany(i => GetDeck())]);

    private static IEnumerable<Card> GetDeck() => Enum.GetValues<Card.CardSuit>().Where(s => s != Card.CardSuit.None)
        .SelectMany(suit => Enum.GetValues<Card.CardFace>().Select(face => new Card {Face = face, Suit = suit}));
    
    /// <summary>
    /// Creates an empty session. Make sure to subscribe to events of the timers.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="shuffleCardPlacement"></param>
    /// <param name="shuffleCardRange"></param>
    /// <param name="bettingTimeSpan"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, uint shuffleCardRange, TimeSpan bettingTimeSpan, TimeSpan sessionDestructionTimeSpan, Random? random = null)
    {
        random ??= Random.Shared;
        var shoe = CreateUnshuffledShoe(numberOfDecks);
        
        shuffleCardPlacement = random.Next((int)(shuffleCardPlacement - shuffleCardRange),(int)(shuffleCardPlacement + shuffleCardRange));
        
        if (shuffleCardPlacement < 0)
            shuffleCardPlacement += shoe.Cards.Count;
        
        shoe.ShuffleCardIdx = shuffleCardPlacement;
        
        var table = new Table((int)numberOfBoxes, shoe);
        
        var sess = new Session(table, bettingTimeSpan, sessionDestructionTimeSpan);
        
        sess.DestructionTimer.Start();
        
        sessions.Add(sess);
        return sess;
    }

    public Session RemoveSession(Guid sessId)
    {
        var session = sessions.Single(s => s.Id == sessId);
        sessions.Remove(session);
        session.DestructionTimer.Stop();
        return session;
    }

    public Session Get(Guid sessionId) => sessions.Single(s => s.Id == sessionId);
    
    public IEnumerable<Session> GetAll() => sessions;
    
    public void UpdateBettingTimer(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (session.Table.BettingBoxes.Any(b => b.Hands[0].Bet > 0)
            && !session.BettingTimer.Enabled) session.BettingTimer.Start();
        else if (session.BettingTimer.Enabled) session.BettingTimer.Stop();
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