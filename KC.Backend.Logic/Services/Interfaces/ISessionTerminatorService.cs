using System;
using System.Collections.Generic;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Services.Interfaces;

public interface ISessionTerminatorService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The list of players that recieved a refund</returns>
    IEnumerable<MacAddress> RefundAndRemoveSession(Guid id);
}