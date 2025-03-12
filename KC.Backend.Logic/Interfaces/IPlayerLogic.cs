using System.Net.NetworkInformation;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    void AddPlayer(Player player);
    void RemovePlayer(MacAddress playerId);
    Player Get(MacAddress playerId);
    void UpdateName(MacAddress playerId, string name);
    void UpdateBalance(MacAddress playerId, int balance);
}