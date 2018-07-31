using System;
using System.Threading.Tasks;

namespace Cron.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var CronDaemon = new CronDaemon();

            CronDaemon.Start();

            CronDaemon.Add("*   *   *   *   *", () =>
            {
                System.Console.WriteLine("C1");
            });

            CronDaemon.Add("*   *   *   *   *", () =>
            {
                System.Console.WriteLine("C2");
            });

            CronDaemon.Stop();
            CronDaemon.Start();

            CronDaemon.Add("*   *   *   *   *", () =>
            {
                System.Console.WriteLine("C3");
            });

            CronDaemon.Add("*   *   *   *   *", () =>
            {
                System.Console.WriteLine("C4");
            });

            System.Console.WriteLine("type exit for you know, exit...");
            while (System.Console.ReadLine() != "exit")
            {
                Task.Delay(100).Wait();
            }
        }
    }
}
