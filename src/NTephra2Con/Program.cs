using CLAP;
using System;
using Common.Logging;

namespace NTephra2
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger<TephraMain>();

        private static void Main(string[] args)
        {
            try
            {
                var app = new TephraMain();
                Parser.Run(args, app);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in ${nameof(Main)}", ex);
            }
            Console.ReadKey();
        }
    }
}
