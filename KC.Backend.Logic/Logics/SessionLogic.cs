using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Core.Interfaces;
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
        .SelectMany(suit => Enum.GetValues<Card.CardFace>().Where(face => face != Card.CardFace.None).Select(face => new Card {Face = face, Suit = suit}));
    
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>Whether the betting timer is enabled after the update.</returns>
    public bool UpdateBettingTimer(Guid sessionId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        
        if (session.Table.BettingBoxes.Any(b => b.Hands[0].Bet > 0))
        {
            if (!session.BettingTimer.Enabled) session.BettingTimer.Start();
        }
        else if (session.BettingTimer.Enabled)
        {
            session.BettingTimer.Stop();
        }
        
        return session.BettingTimer.Enabled;
    }
}