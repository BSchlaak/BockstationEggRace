using System;
using System.Threading.Tasks;

namespace Bockstation.EggRace.UI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var connector = new Core.Mqtt.Connector("raspberrypi.local");
            connector.MessageReceived += MessageReceived;

            Task.Run(async () => await connector.ConnectAsync());
            //Task.Run(async () => await connector.ConnectManagedAsync());

            while (true) { }
        }

        private static void MessageReceived(object sender, Tuple<long, long, long, long> e)
        {
            System.Console.WriteLine($"Message received: {e}");

            if (e.Item1 > 0)
            {
                var startTime = ParseTimeSpan(e.Item1);

                if (e.Item2 == 0)
                {
                    System.Console.WriteLine($"Started at: {startTime}");
                }
                else
                {
                    var intermediateTime1 = ParseTimeSpan(e.Item2);

                    if (e.Item3 == 0)
                    {
                        System.Console.WriteLine($"First intermediate time at: {intermediateTime1} (after {intermediateTime1 - startTime})");
                    }
                    else
                    {
                        var intermediateTime2 = ParseTimeSpan(e.Item3);

                        if (e.Item4 == 0)
                        {
                            System.Console.WriteLine($"Second intermediate time at: {intermediateTime2} (after {intermediateTime2 - startTime})");
                        }
                        else
                        {
                            var finishTime = ParseTimeSpan(e.Item4);
                            System.Console.WriteLine($"Finished at: {finishTime} (after {finishTime - startTime})");
                        }
                    }
                }
            }
        }

        private static TimeSpan ParseTimeSpan(long timestamp)
        {
            TimeSpan result = TimeSpan.Zero;

            if (int.TryParse(timestamp.ToString().Substring(0, 2), out var hours) &&
                int.TryParse(timestamp.ToString().Substring(2, 2), out var minutes) &&
                int.TryParse(timestamp.ToString().Substring(4, 2), out var seconds) &&
                int.TryParse(timestamp.ToString().Substring(6, 3), out var millis))
            {
                result = new TimeSpan(0, hours, minutes, seconds, millis);
            }

            return result;
        }
    }
}
