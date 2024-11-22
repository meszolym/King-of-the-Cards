using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Emgu.CV;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.Detector.Logic;

public static class WindowCaptureService
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);


    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private static Fin<IntPtr> GetWindowHandle(string appName) =>
        ((Option<IntPtr>)Process.GetProcesses().FirstOrDefault(proc => proc.MainWindowTitle.Contains(appName))
            .MainWindowHandle).ToFin(Error.New("Application not found."));

    public static Fin<Bitmap> CaptureWindowShot(Either<string, IntPtr> appIdentifier, bool hasToBoFocused = true) =>
        appIdentifier
            .Match(Left: s => CaptureWindowShot(s, hasToBoFocused),
                Right: i => CaptureWindowShot(i, hasToBoFocused));

    public static Fin<Bitmap> CaptureWindowShot(string appName, bool hasToBoFocused = true) =>
        GetWindowHandle(appName).Bind(i => CaptureWindowShot(i, hasToBoFocused));

    public static Fin<Bitmap> CaptureWindowShot(IntPtr handle, bool hasToBoFocused = true)
    {
        if (hasToBoFocused)
        {
            var foregroundWindowHandle = GetForegroundWindow();
            if (foregroundWindowHandle != handle)
            {
               return Error.New("Window is not focused.");
            }
        }

        var rect = new Rect();
        GetWindowRect(handle, ref rect);
        var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        var result = new Bitmap(bounds.Width, bounds.Height);

        using (var graphics = Graphics.FromImage(result))
        {
            graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
        }

        return result;
    }

    
}