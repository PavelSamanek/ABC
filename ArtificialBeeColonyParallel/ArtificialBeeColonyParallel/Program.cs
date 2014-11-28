using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialBeeColonyParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("\nBegin Simulated Bee Colony algorithm demo\n");
                Console.WriteLine("Loading cities data for SBC Traveling Salesman Problem analysis");
                Data pointsData = new Data(20);
                Console.WriteLine(pointsData.ToString());
                Console.WriteLine("Number of cities = " + pointsData.points.Length);
                Console.WriteLine("Number of possible paths = " + pointsData.NumberOfPossiblePaths().ToString("#,###"));
                Console.WriteLine("Best possible solution (shortest path) length = " + pointsData.ShortestPathLength().ToString("F4"));

                int totalNumberBees = 100;
                int numberInactive = 30;
                int numberActive = 50;
                int numberScout = 20;

                int maxNumberVisits = 100;
                int maxNumberCycles = 99999;
                //int maxNumberVisits = 95;
                //int maxNumberCycles = 10570;
                //int maxNumberVisits = 300; // proportional to # of possible neighbors to given solution
                //int maxNumberCycles = 32450;

                NestSerial nestSerial = new NestSerial(totalNumberBees, numberInactive, numberActive, numberScout, maxNumberVisits, maxNumberCycles, pointsData);
                Console.WriteLine("\nInitial random hive");
                Console.WriteLine(nestSerial);


                bool doProgressBar = true;
                nestSerial.Solve(doProgressBar);

                Console.WriteLine("\nFinal hive");
                Console.WriteLine(nestSerial);

                Console.WriteLine("End Simulated Bee Colony demo");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
