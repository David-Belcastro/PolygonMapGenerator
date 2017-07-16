using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Town_Map_Generator;

namespace CubesFortune
{
    class Program
    {
        static void Main(string[] args)
        {
            int? mapsize = null;
            Console.WriteLine("How big you want it boss?");
            while (mapsize == null)
            {
                mapsize = GetUserInputForMapSize(Console.ReadLine());
            }

            var variant = 0.1;
            //Console.WriteLine(mapsize);
            for (var i = 0; i < 10; i++)
            {
                var generator = new ImageGenerator(new Random().Next());


                generator.createimage(mapsize ?? 3600, variant);
                variant += .1;
            }
            
            //Console.WriteLine("All Done");
            //Console.ReadLine();
        }

        private static int? GetUserInputForMapSize(string userinput)
        {
            int mapsize;
            if (int.TryParse(userinput, out mapsize) && mapsize >= 10)
            {
                return mapsize;
            }
            else
            {
                Console.WriteLine("Gotta be bigger than 10 there boss");
                return null;
            }
        }
    }
}
