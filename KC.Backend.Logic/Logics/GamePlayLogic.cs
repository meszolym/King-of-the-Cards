using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Backend.Logic.Core.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.GameManagement;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics;

public class GamePlayLogic(IList<Session> sessions, IDictionary<MacAddress, Guid> macToPlayerGuid, IRuleBook ruleBook, HandUpdatedDelegate handUpdatedDelegate, BetUpdatedDelegate betUpdatedDelegate) : IGamePlayLogic
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
        
        shoe.ResetShuffleCardPlacement();
    }

    /// <summary>
    /// Gets if move is possible on a given hand on a given box.
    /// </summary>
    /// <returns></returns>
    public bool CanMakeMove(Guid sessionId, int boxIdx, int handIdx, Move move) =>
        GetPossibleActionsOnHand(sessionId, boxIdx, handIdx).Contains(move);
    
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
    // public Card TakeCardFromShoe(Guid sessionId)
    // {
    //     var session = sessions.Single(s => s.Id == sessionId);
    //     return ;
    // }

    public async Task AddCardToHand(Guid sessionId, int boxIdx, int handIdx)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        Hand hand;
        
        if (boxIdx == -1 && handIdx == -1) //dealer hand
        {
            hand = session.Table.Dealer.Hand;
        }
        else
        {
            hand = session.Table.BettingBoxes[boxIdx].Hands[handIdx];
        }
        
        hand.Cards.Add(session.Table.Shoe.Cards[session.Table.Shoe.NextCardIdx++]);
        await handUpdatedDelegate(sessionId);
        
    }

    /// <summary>
    /// Plays dealer's hand according to the rules.
    /// </summary>
    /// <exception cref="InvalidOperationException">"It's not the dealer's turn."</exception>
    /// <exception cref="InvalidOperationException">"Dealer's hand is already finished."</exception>
    public async Task DealerPlayHand(Guid sessionId, TimeSpan delayBetweenCards)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (session.CurrentTurnInfo.PlayersTurn) throw new InvalidOperationException("It's not the dealer's turn.");
        var dealerHand = session.Table.Dealer.Hand;
        if (session.Table.Dealer.Hand.Finished) throw new InvalidOperationException("Dealer's hand is already finished.");
        
        var handsInPlayNotBustNoBj = session.Table.BettingBoxes
            .Where(b => b.Hands.Any(h => h.Bet > 0 
                                         && !ruleBook.GetValue(h).IsBlackJack 
                                         && ruleBook.GetValue(h).NumberValue <= 21))
            .ToArray();
        
        session.Table.Dealer.ShowAllCards = true;
        await handUpdatedDelegate(sessionId);
        await Task.Delay(delayBetweenCards);

        if (handsInPlayNotBustNoBj.Length == 0)
        {
            //No need to play the hand
            dealerHand.Finished = true;
            return;
        }
        
        while (ruleBook.DealerShouldHit(dealerHand))
        {
            await AddCardToHand(session.Id, -1, -1); //add card to dealer hand
            await Task.Delay(delayBetweenCards);
        }
        
        dealerHand.Finished = true;
    }

    /// <summary>
    /// Deals cards to the players and the dealer at the start of a round.
    /// </summary>
    /// <exception cref="InvalidOperationException">Shoe needs shuffling.</exception>
    public async Task DealStartingCards(Guid sessionId, TimeSpan delayBetweenCards, bool checkShuffle = false)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        session.DestructionTimer.Reset();
        
        //if shoe needs shuffling, throw exception
        if (checkShuffle && session.Table.Shoe.ShuffleCardIdx <= session.Table.Shoe.NextCardIdx)
        {
            throw new InvalidOperationException("Shoe needs shuffling.");
        }

        for (var i = 0; i < 2; i++)
        {
            foreach (var boxIdx in session.Table.BettingBoxes.Where(b => b.Hands[0].Bet > 0).Select(b => b.IdxOnTable))
            {
                await AddCardToHand(session.Id, boxIdx, 0);
            }
            await AddCardToHand(session.Id, -1, -1); //add card to dealer hand
            
            await Task.Delay(delayBetweenCards);
        }
    }

    /// <summary>
    /// Checks for dealer blackjack.
    /// </summary>
    /// <returns>True if the dealer has blackjack</returns>
    public bool DealerCheck(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!ruleBook.DealerCheckBlackJack(session.Table.Dealer.Hand)) return false;
        session.Table.Dealer.ShowAllCards = true;
        session.Table.Dealer.Hand.Finished = true;
        return true;
    }
    
    public IEnumerable<Move> GetPossibleActionsOnHand(Hand hand) => ruleBook.GetPossibleActionsOnHand(hand);
    public IEnumerable<Move> GetPossibleActionsOnHand(Guid sessionId, int boxIdx, int handIdx = 0) => 
        GetPossibleActionsOnHand(sessions.Single(s => s.Id == sessionId).Table.BettingBoxes[boxIdx].Hands[handIdx]);
    
    /// <summary>
    /// Makes a move on a given hand of a given player on a given box. Does not handle player balance, hand bets or transferring turns.
    /// </summary>
    /// <exception cref="InvalidOperationException">"The hand is not in turn."</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player."</exception>
    /// <exception cref="InvalidOperationException">"Action not possible." if the rulebook states that this action is not possible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If move is not handled.</exception>
    public async Task MakeMove(Guid sessionId, int boxIdx, MacAddress playerId, Move move, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CurrentTurnInfo.PlayersTurn || session.CurrentTurnInfo.BoxIdx != boxIdx || session.CurrentTurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand is not in turn.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != macToPlayerGuid[playerId]) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];
        if (!GetPossibleActionsOnHand(hand).Contains(move)) throw new InvalidOperationException("Action not possible.");
        
        session.DestructionTimer.Reset();
        
        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                await AddCardToHand(sessionId, boxIdx, handIdx);
                break;
            case Move.Double:
                await AddCardToHand(sessionId, boxIdx, handIdx);
                hand.Finished = true;
                break;
            case Move.Split:
                box.Hands.Add(new Hand(){Cards = [hand.Cards[1]], FromSplit = true});
                hand.FromSplit = true;
                hand.Cards.RemoveAt(1);
                await AddCardToHand(sessionId, boxIdx, handIdx);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if a move cannot be made, mark hand as finished
        if (!GetPossibleActionsOnHand(hand).Any())
            hand.Finished = true;

    }
        
    private IEnumerable<BettingBox> BoxesInPlay(Guid sessionId) =>
        sessions.Single(s => s.Id == sessionId).Table.BettingBoxes.Where(box => box.Hands[0].Bet > 0);
    
    public async Task TransferTurn(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);

        Move[] movesOnThisHand = [];
        
        while (true)
        {
            try
            {
                if (session.CurrentTurnInfo.PlayersTurn)
                    movesOnThisHand = GetPossibleActionsOnHand(session.Table.BettingBoxes[session.CurrentTurnInfo.BoxIdx]
                        .Hands[session.CurrentTurnInfo.HandIdx]).ToArray();
            }
            catch (Exception e)
            {
                movesOnThisHand = [];
            }
            
            if (movesOnThisHand.Length != 0) return;
            
            #region dealers turn handling
            //if it's the dealer's turn, transfer to the first player's turn
            // TODO: check this (i know this is for starting the round, but it's not good when there's a new round)
            if (!session.CurrentTurnInfo.PlayersTurn)
            {
                session.CurrentTurnInfo = new TurnInfo(true, BoxesInPlay(sessionId).First().IdxOnTable, 0);
                continue;
            }
        
            #endregion
            
            #region hand left handling
            var box = session.Table.BettingBoxes[session.CurrentTurnInfo.BoxIdx];
            var hand = box.Hands[session.CurrentTurnInfo.HandIdx];

            //mark hand as finished
            hand.Finished = true;

            // //if bust, 0 out bet
            // if (ruleBook.GetValue(hand).NumberValue > 21) hand.Bet = 0;

            //if there are more hands left in the box, transfer to the next hand
            if (box.Hands.Count > session.CurrentTurnInfo.HandIdx+1)
            {
                session.CurrentTurnInfo = session.CurrentTurnInfo with { HandIdx = session.CurrentTurnInfo.HandIdx + 1 };
                await AddCardToHand(sessionId, session.CurrentTurnInfo.BoxIdx, session.CurrentTurnInfo.HandIdx);
                return;
            }
            #endregion

            #region box left handling
            //if there are more boxes left, transfer to the next box
            var nextBox = BoxesInPlay(sessionId).FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.IdxOnTable > box.IdxOnTable);
            if (nextBox is not null)
            {
                session.CurrentTurnInfo = session.CurrentTurnInfo with { BoxIdx = nextBox.IdxOnTable, HandIdx = 0 };
                continue;
            }
            #endregion

            #region no boxes left handling
            //if there are no more boxes left, transfer to the dealer's turn
            session.CurrentTurnInfo = new TurnInfo(); //false, 0, 0 by default
            return;
            #endregion

        }
        
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
        session.CurrentTurnInfo = new TurnInfo(); //false, 0, 0 by default, informs players
        
    }
    
    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    public async Task PayOutBetsToBettingBoxes(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        var dealerHand = session.Table.Dealer.Hand;

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
            await betUpdatedDelegate(sessionId, box.IdxOnTable);
        }
    }
    
    public async Task ClearHands(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        foreach (var box in session.Table.BettingBoxes)
        {
            box.Hands.Clear();
            box.Hands.Add(new ());
        }
        session.Table.Dealer.Hand = new ();
        session.Table.Dealer.ShowAllCards = false;
        await handUpdatedDelegate(sessionId);
    }
}