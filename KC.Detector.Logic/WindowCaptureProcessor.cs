using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Emgu.CV;
using LanguageExt;

namespace KC.Detector.Logic;

//IDK what to do with this
public class WindowCaptureProcessor(Either<string, IntPtr> appIdentifier, TimeSpan timeBetweenCaptures)
{
    private IDisposable subscription;

    private Subject<Rectangle> capturedCards = new Subject<Rectangle>();

    public void StartProcessing()
    {
        subscription = Observable.Interval(timeBetweenCaptures)
            .Select(_ => WindowCaptureService.CaptureWindowShot(appIdentifier))
            .Subscribe(b => ProcessBitmap(b));
    }

    public void StopProcessing() => subscription.Dispose();
    public void ProcessBitmap(Fin<Bitmap> bitmap) => bitmap.Map(b => b.ToMat()).Map(ImageUtils.GetTopCards); //.Map();

}