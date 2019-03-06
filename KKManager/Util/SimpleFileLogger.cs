using System;
using System.IO;
using System.Reactive.Disposables;

namespace KKManager.Util
{
    public static class SimpleFileLogger
    {
        public static IDisposable SetupLogging(string location)
        {
            var writer = new FileStream(location + ".log", FileMode.OpenOrCreate);
            var tw = new StreamWriter(writer);

            tw.WriteLine("=======================");
            tw.WriteLine(DateTime.Now + " Application start");
            Console.SetOut(tw);
            Console.SetError(tw);

            return new CompositeDisposable(tw, writer);
        }
    }
}