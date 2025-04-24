using System;
using System.Collections.Generic;

namespace KC.Backend.Logic.Services.Interfaces;

public interface ISessionTerminatorService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The list of player Guids that received a refund</returns>
    IEnumerable<Guid> RefundAndRemoveSession(Guid id);
}