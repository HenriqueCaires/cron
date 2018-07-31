using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Cron.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var daemon = new CronDaemon();
            daemon.Add("* * * * *", () =>
            {
                System.Console.WriteLine("Job 1");
            });

            daemon.Start();

            daemon.Add("* * * * *", () =>
            {
                System.Console.WriteLine("Job 1");
            });

            Task.Delay(10 * 60 * 1000).Wait();
            daemon.Stop();
        }
    }
}
