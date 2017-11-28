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

        private static void ProcessInput(World world, string userInput)
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
                try
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
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
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
        /// Initiates a search of events near a given coordinate, displaying a list of nearby events, the cheapest ticket for each event and their distance from the given coordinate.
        /// </summary>
        /// <param name="world">The world to search.</param>
        /// <param name="x">The X coordinate to centre the search on.</param>
        /// <param name="y">The Y coordinate to centre the search on.</param>
        private static void PerformEventSearch(World world, int x, int y)
        {
            var searchResults = world.SearchNearbyEvents(x, y);
            foreach (var searchResult in searchResults)
            {
                Console.WriteLine(searchResult);
            }
        }

        /// <summary>
        /// Initiates a query of a coordinate, displaying the event and tickets at that coordinate.
        /// </summary>
        /// <param name="world">The world to query.</param>
        /// <param name="x">The X coordinate to query.</param>
        /// <param name="y">The Y coordinate to query.</param>
        private static void PerformCellQuery(World world, int x, int y)
        {
            Cell cell = world.LocateCell(x, y);
            Console.WriteLine(cell);
        }

        /// <summary>
        /// Attempts to extract coordinate values from user input.
        /// </summary>
        /// <param name="userInput">The input string provided by the user.</param>
        /// <param name="x">The output variable to store the extracted X coordinate in.</param>
        /// <param name="y">The output variable to store the extracted Y coordinate in.</param>
        /// <returns>Returns true if coordinate extraction succeeded, otherwise false.</returns>
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
