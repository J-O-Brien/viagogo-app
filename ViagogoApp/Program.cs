using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViagogoApp;

namespace ViagogoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var world = World.GenerateWorld();
            string userInput = null;
            
            while (true)
            {

                userInput = CaptureInput();
                ProcessInput(world, userInput);
                ClearScreen();
            }
        }

        private static void ClearScreen()
        {
            Console.WriteLine();
            Console.WriteLine("Press <Enter> to search again");
            Console.ReadLine();
            Console.Clear();
        }

        private static bool ProcessInput(World world, string userInput)
        {
            bool queryCellMode = false;
            int x, y;
            if (userInput.Equals("exit"))
            {
                Environment.Exit(0);
            }
            else if (userInput.StartsWith("?"))
            {
                queryCellMode = true;
                userInput = userInput.Replace("?", "");
            }

            if (!TryGetCoordinates(userInput, out x, out y))
            {
                Console.WriteLine("Coordinates must be entered as follows: x, y");
            }
            else
            {
                if (queryCellMode)
                {
                    PerformCellQuery(world, x, y);
                }
                else
                {
                    PerformEventSearch(world, x, y);
                }
            }

            return queryCellMode;
        }

        private static string CaptureInput()
        {
            string userInput;
            Console.WriteLine("Enter a pair of coordinates (x, y) or type 'exit' to finish.");
            Console.WriteLine("Coordinates can be prefixed with a question mark (? x, y) to query the events at a cell");
            Console.Write("Enter coordinates: ");
            userInput = Console.ReadLine().Trim().ToLower();
            return userInput;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void PerformEventSearch(World world, int x, int y)
        {
            var searchResults = world.SearchNearbyEvents(x, y);
            foreach (var searchResult in searchResults)
            {
                Console.WriteLine(searchResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void PerformCellQuery(World world, int x, int y)
        {
            Cell cell = world.LocateCell(x, y);
            Console.WriteLine(cell);
        }

        private static bool TryGetCoordinates(string userInput, out int x, out int y)
        {
            x = 0;
            y = 0;
            // IEnumerable.Take(2) could have been used here, 
            // but I preferred to invalidate input that was not strictly X,Y
            IList<string> coordinates = userInput.Split(',').Select(elem => elem.Trim()).ToList();
            if (coordinates.Count() != 2)
            {
                return false;
            }
            else if (!int.TryParse(coordinates[0], out x))
            {
                return false;
            }
            else if (!int.TryParse(coordinates[1], out y))
            {
                return false;
            }
            return true;
        }
    }
}
