using System.Timers;

namespace KC.Backend.Logic.Extensions;

public static class TimerExtensions
{
    public static void Reset(this Timer timer)
    {
        timer.Stop();
        timer.Start();
    }
}