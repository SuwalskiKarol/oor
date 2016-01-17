using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace add
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] kolekcjaLiczb = new[] { 1, 4, 6, 5, 3, 2, 8, 9, 0 };

            var querry1 = kolekcjaLiczb.Select(n => n).ToArray().OrderBy(b=>b);

            foreach (var item in querry1)
            {
                Console.Write(item + " ");
            }
            Console.ReadLine();
        }
    }
}
