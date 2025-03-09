using System.Net.NetworkInformation;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    void AddPlayer(Player player);
    void RemovePlayer(PhysicalAddress playerId);
    Player Get(PhysicalAddress playerId);
    void UpdateName(PhysicalAddress playerId, string name);
    void UpdateBalance(PhysicalAddress playerId, int balance);
}