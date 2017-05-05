using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Town_Map_Generator
{
    class Program
    {
        static void Main()
        {
            var mapgen = new Landmass();
            mapgen.GenerateLandmass();
            Console.ReadKey;
        }
    }
}
