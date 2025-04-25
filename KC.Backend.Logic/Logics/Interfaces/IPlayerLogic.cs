using System;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics.Interfaces;

public interface IPlayerLogic
{
    void AddPlayer(MacAddress mac, Player player);
    void RemovePlayer(MacAddress playerId);
    Player Get(MacAddress playerId);
    Player Get(Guid playerId);
    void UpdateName(MacAddress playerId, string name);
    void AddToBalance(MacAddress playerId, double amount);
    void AddToBalance(Guid playerId, double amount);
    void UpdateBalance(MacAddress playerId, double balance);
    void UpdateBalance(Guid playerId, double balance);
    void UpdatePlayerConnectionId(MacAddress playerId, string connectionId);
}