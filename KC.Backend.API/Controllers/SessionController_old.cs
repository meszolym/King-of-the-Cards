﻿// using KC.Backend.Logic.Interfaces;
// using KC.Backend.Models.GameItems;
// using KC.Backend.Models.GameManagement;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.OpenApi.Validations;
// using Timer = System.Timers.Timer;
//
// namespace KC.Backend.API.Controllers;
//
// [ApiController]
// [Route("[controller]")]
// public class SessionController_old(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IHubContext<SignalRHub> signalRHub) : Controller
// {
//     private const int numberOfBoxes = 6;
//     private const int numberOfDecks = 6;
//     private const int secondsToPlaceBets = 15;
//     private const int minutesToPurgeOldSessions = 10;
//     private const int secondsToPlay = 60;
//     private const int secondsToPause = 2;
//
//     [HttpGet("{Id:guid}")]
//     public Session GetSession(Guid Id) => sessionLogic.Get(Id);
//
//     // [HttpGet]
//     // public IEnumerable<Session> GetAllSessions()
//     // {
//     //     if (sessionLogic.PurgeOldSessions(TimeSpan.FromMinutes(minutesToPurgeOldSessions)))
//     //         signalRHub.Clients.Group("NoSession").SendAsync("PurgeOldSessions"); //why is this bookmarked?
//     //
//     //     return sessionLogic.GetAllSessions();
//     // }
//
//     // [HttpPost]
//     // public Session CreateSession()
//     // {
//     //     var timer = new TickingTimer(TimeSpan.FromSeconds(secondsToPlaceBets+1)); //+1 sec to account for the time it takes to process everything
//     //
//     //     var sess = sessionLogic.CreateSession(numberOfBoxes, numberOfDecks, timer);
//     //
//     //     timer.Elapsed += (sender, args) => BettingTimerElapsed(sess);
//     //     timer.Tick += (sender, args) => signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("TimerTick", timer.RemainingSeconds);
//     //
//     //     signalRHub.Clients.Group("NoSession").SendAsync("SessionCreated", sess);
//     //
//     //     return sess;
//     // }
//
//     // private void BettingTimerElapsed(Session sess)
//     // {
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("TimerElapsed", sess);
//     //     sess.DealStartingCards();
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("CardsDealt", sess);
//     //
//     //     if (!sess.DealerCheck())
//     //     {
//     //         var next = sess.TransferTurn();
//     //         signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("NextTurn", sess, next);
//     //         return;
//     //     }
//     //
//     //     sess.FinishAllHandsInPlay();
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("DealerBlackJack", sess);
//     //
//     //     sess.PayOutBets();
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("PayoutDone", sess);
//     //
//     //     sess.ResetSession();
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("SessionReset", sess);
//     // }
//
//     // [HttpPost("{sessionId:guid}/Subscribe/{playerId}")]
//     // public void JoinSession(Guid sessionId, string playerId)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var session = sessions.Single(s => s.Id == sessionId)
//     //     signalRHub.Groups.AddToGroupAsync(player.ConnectionId, session.Id.ToString());
//     //     signalRHub.Groups.RemoveFromGroupAsync(player.ConnectionId, "NoSession");
//     // }
//     //
//     // [HttpDelete("{sessionId:guid}/Subscribe/{playerId}")]
//     // public void LeaveSession(Guid sessionId, string playerId)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var session = sessions.Single(s => s.Id == sessionId)
//     //     signalRHub.Groups.RemoveFromGroupAsync(player.ConnectionId, session.Id.ToString());
//     //     signalRHub.Groups.AddToGroupAsync(player.ConnectionId, "NoSession");
//     // }
//     //
//     // [HttpPost("{sessionId:guid}/{boxIdx:int}/ClaimBox/{playerId}")]
//     // public void ClaimBox(Guid sessionId, int boxIdx, string playerId)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //     sess.ClaimBox(boxIdx, player);
//     //     signalRHub.Clients.GroupExcept(sess.Id.ToString(), player.ConnectionId).SendAsync("BoxClaimed", sess);
//     // }
//     //
//     // [HttpDelete("{sessionId:guid}/{boxIdx:int}/ClaimBox/{playerId}")]
//     // public void DisclaimBox(Guid sessionId, int boxIdx, string playerId)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //     sess.DisclaimBox(boxIdx, player);
//     //     signalRHub.Clients.GroupExcept(sess.Id.ToString(), player.ConnectionId).SendAsync("BoxDisclaimed",sess);
//     // }
//     //
//     // [HttpPut("{sessionId:guid}/{boxIdx:int}/UpdateBet/{playerId}/{amount:double}")]
//     // public void Bet(Guid sessionId, int boxIdx, string playerId, double amount)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //     sess.UpdateBet(boxIdx, player, amount);
//     //     signalRHub.Clients.GroupExcept(sess.Id.ToString(), player.ConnectionId).SendAsync("BoxBetUpdated", sess);
//     //     var timerOn = sess.UpdateTimer();
//     //     signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("TimerState", sess, timerOn);
//     // }
//     //
//     // [HttpPost("{sessionId:guid}/{boxIdx:int}/{handIdx:int}/MakeMove/{playerId}/{move}")]
//     // public TurnInfo MakeMove(Guid sessionId, int boxIdx, int handIdx, string playerId, Move move)
//     // {
//     //     var player = playerLogic.Get(playerId);
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //
//     //     //TODO: check if split or double -> make changes to player balance
//     //     sess.MakeMove(boxIdx, handIdx, player, move);
//     //
//     //     //If only stand left as a possible move, transfer turn
//     //     if (!sess.Boxes[boxIdx].Hands[handIdx].GetPossibleActions().Any(x => x != Move.Stand)) sess.TurnInfo = sess.TransferTurn();
//     //
//     //     signalRHub.Clients.GroupExcept(sess.Id.ToString(), player.ConnectionId).SendAsync("MoveMade", sess);
//     //
//     //     return sess.TurnInfo;
//     // }
//     //
//     // [HttpGet("{sessionId:guid}/{boxIdx:int}/{handIdx:int}/GetPossibleMoves")]
//     // public IEnumerable<Move> GetPossibleMoves(Guid sessionId, int boxIdx, int handIdx)
//     // {
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //     return sess.Boxes[boxIdx].Hands[handIdx].GetPossibleActions();
//     // }
//     //
//     // [HttpGet("{sessionId:guid}/{boxIdx:int}/{handIdx:int}/GetHandValue")]
//     // public HandValue GetHandValue(Guid sessionId, int boxIdx, int handIdx)
//     // {
//     //     var sess = sessions.Single(s => s.Id == sessionId)
//     //     return sess.Boxes[boxIdx].Hands[handIdx].GetValue();
//     // }
//
// }