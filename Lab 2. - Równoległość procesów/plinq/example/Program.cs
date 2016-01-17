using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace example
{
    class Program
    {
        private static Random rnd = new Random();
        static void Main(string[] args)
        {
            DateTime start = DateTime.UtcNow;

            IEnumerable<int> items = Enumerable.Range(0, 20);

            var results = items
                .Select(ProcessItem)
                .ToList();

            results.ForEach(Console.WriteLine);

            DateTime end = DateTime.UtcNow;
            TimeSpan duration = end - start;

            Console.WriteLine("Finished. Took {0}", duration);

            Console.ReadLine();
        }

        private static string ProcessItem(int item)
        {
            int pause = rnd.Next(900, 1100);
            Thread.Sleep(pause);

            return string.Format("Result of item {0}", item);
        }
    }
}
