using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViagogoApp
{
    class World
    {
        private Cell[,] cells;

        public int MinimumX { get; private set; }
        public int MinimumY { get; private set; }
        public int MaximumX { get; private set; }
        public int MaximumY { get; private set; }

        //Add 1 to the world size to include both upper and lower boundary values
        public int WorldWidth
        {
            get
            {
                return this.MaximumX - this.MinimumX + 1;
            }
        }

        public int WorldHeight
        {
            get
            {
                return this.MaximumY - this.MinimumY + 1;
            }
        }

        
        public World(int minX, int minY, int maxX, int maxY)
        {
            if (minX > maxX)
            {
                throw new ArgumentOutOfRangeException(String.Format("{} must be less than or equal to {}", nameof(minX), nameof(maxX)));
            }
            if (minY > maxY)
            {
                throw new ArgumentOutOfRangeException(String.Format("{} must be less than or equal to {}", nameof(minX), nameof(maxX)));
            }
            this.MinimumX = minX;
            this.MinimumY = minY;
            this.MaximumX = maxX;
            this.MaximumY = maxY;

            this.cells = new Cell[this.WorldWidth, this.WorldHeight];
        }

        /// <summary>
        /// Finds the cell at a given XY coordinate, by translating the XY coordinate to the corresponding index in the backing collection.
        /// </summary>
        /// <param name="x">The X coordinate of the desired cell.</param>
        /// <param name="y">The Y coordinate of the desired cell.</param>
        /// <returns>The cell with the specified coordinates.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the X or Y values lie outside the world bounds.</exception>
        public Cell LocateCell(int x, int y)
        {
            if (x < this.MinimumX || x > this.MaximumX)
            {
                throw new ArgumentOutOfRangeException(String.Format("x must be between {0} and {1} inclusive", this.MinimumX, this.MaximumX));
            }
            else if (y < this.MinimumY || x > this.MaximumY)
            {
                throw new ArgumentOutOfRangeException(String.Format("y must be between {0} and {1} inclusive", this.MinimumY, this.MaximumY));
            }
            return this.cells[OffsetXCoordinate(x), OffsetYCoordinate(y)];
        }

        /// <summary>
        /// Finds the specified number of events, and the cheapest ticket for each event, centred on the specified XY coordinate, ordered by distance.
        /// </summary>
        /// <param name="x">The X coordinate to centre the search on</param>
        /// <param name="y">The Y coordinate to centre the search on</param>
        /// <param name="numEvents">The maximum number of events to return</param>
        /// <returns>A collection of search results, consisting of the event, the cheapest ticket for the event, and the distance from the search coordinate</returns>
        public IEnumerable<SearchResult> SearchNearbyEvents(int x, int y, int numEvents=5)
        {
            Cell c = this.LocateCell(x, y);
            return this.SearchNearbyEvents(c, numEvents);
        }

        /// <summary>
        /// Helper function to locate up to the specified number of nearest events and cheapest corresponding ticket, centred on a cell
        /// </summary>
        /// <param name="cell">The cell to centre the search on</param>
        /// <param name="numEvents">The maximum number of events to return</param>
        /// <returns>A collection of search results, consisting of the event, the cheapest ticket for the event, and the distance from the search coordinate</returns>
        /// <seealso cref="SearchNearbyEvents(int, int, int)"/>
        private IEnumerable<SearchResult> SearchNearbyEvents(Cell cell, int numEvents=5)
        {
            IList<Cell> results = new List<Cell>();
            foreach (var currentCell in this.cells)
            {
                if (currentCell.Event != null && currentCell.Event.HasTicketsAvailable)
                {
                    results.Add(currentCell);
                }
            }
            return results.OrderBy(c => c.CalculateDistanceToCell(cell)).Take(numEvents).Select(c =>
            {
                return new SearchResult(
                    c.Event,
                    c.Event.FindCheapestTicket(),
                    c.CalculateDistanceToCell(cell));
            });
        }

        /// <summary>
        /// Returns the array index of the first dimension of the cell array, for a given X coordinate
        /// </summary>
        /// <param name="x">The X coordinate to be offset</param>
        /// <returns>The X coordinate, offset by the length of the first dimension of the cell array</returns>
        private int OffsetXCoordinate(int x)
        {
            return x + Math.Abs(this.MinimumX);
        }

        /// <summary>
        /// Returns the array index of the second dimension of the cells array, for a given Y coordinate
        /// </summary>
        /// <param name="y">The Y coordinate to be offset</param>
        /// <returns>The Y coordinate, offset by the length of the second dimension of the cell array</returns>
        private int OffsetYCoordinate(int y)
        {
            return y + Math.Abs(this.MinimumY);
        }

        /// <summary>
        /// Utility function to randomly populate the cells with events and tickets.
        /// </summary>
        /// <param name="rng">A random number generator to determine event frequency</param>
        /// <param name="eventFrequency"></param>
        private void PopulateCells(Random rng, double eventFrequency = 1)
        {
            foreach (var x in Enumerable.Range(this.MinimumX, this.WorldWidth))
            {
                foreach (var y in Enumerable.Range(this.MinimumY, this.WorldHeight))
                {
                    var eventRoll = rng.NextDouble();
                    Event ev = null;
                    if (eventRoll <= eventFrequency)
                    {
                        ev = new Event(Ticket.GenerateTickets(rng));
                    }
                    cells[this.OffsetXCoordinate(x), this.OffsetYCoordinate(y)] = new Cell(x, y, ev);
                }
            }
        }

        /// <summary>
        /// Utility function to generate the world, and populate its cells with events.
        /// </summary>
        /// <param name="minX">The minimum X value for the world.</param>
        /// <param name="minY">The minimum Y value for the world.</param>
        /// <param name="maxX">The maximum X value for the world.</param>
        /// <param name="maxY">The maximum Y value for the world.</param>
        /// <returns>A world populated with cells, with each cell consisting of an event with a number of tickets.</returns>
        public static World GenerateWorld(int minX=-10, int minY=-10, int maxX=10, int maxY=10)
        {
            var world = new World(minX, minY, maxX, maxY);
            var rng = new Random();
            world.PopulateCells(rng);
            return world;
        }
    }

    class Cell
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Event Event { get; private set; }

        public Cell(int x, int y, Event ev)
        {
            this.X = x;
            this.Y = y;
            this.Event = ev;
        }

        /// <summary>
        /// Calculates the Manhattan distance between this cell and another cell in the world.
        /// </summary>
        /// <param name="other">The cell to measure the distance to.</param>
        /// <returns>The distance between this cell and the other cell.</returns>
        public int CalculateDistanceToCell(Cell other)
        {
            return Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
        }

        public override string ToString()
        {
            return String.Format("Cell {0}, {1}\r\n{2}", this.X, this.Y, this.Event != null ? this.Event.Dump() : "\tNo event");
        }
    }

    class Event
    {
        private static int nextId = 1;

        public int Id { get; private set; }
        private IList<Ticket> Tickets { get; set; }
        public bool HasTicketsAvailable { get { return this.Tickets.Count > 0; } }

        public Ticket FindCheapestTicket()
        {
            return this.Tickets.OrderBy(t => t.Price).FirstOrDefault();
        }

        public Event(IList<Ticket> tickets)
            : this(nextId++, tickets)
        {
        }

        private Event(int id, IList<Ticket> tickets)
        {
            this.Id = id;
            this.Tickets = tickets;
        }

        public override string ToString()
        {
            return String.Format("Event {0:000}", this.Id);
        }

        /// <summary>
        /// Utility function to provide all available data for an event.
        /// </summary>
        /// <returns>A summary of the event, and the tickets available for the event (if any).</returns>
        public string Dump()
        {
            var sb = new StringBuilder();
            sb.Append(String.Format("\t{0}\r\n", this));
            if (this.HasTicketsAvailable)
            {
                foreach (var ticket in this.Tickets)
                {
                    sb.Append(String.Format("\t\t{0}\r\n", ticket));
                }
            }
            else
            {
                sb.Append(String.Format("\t\tNo tickets available\r\n"));
            }
            return sb.ToString();
        }
    }

    class Ticket
    {
        private static int nextId = 1;

        public int Id { get;  private set; }
        //For convenience of RNG, price is expressed as a double instead of a decimal
        public double Price { get; private set; }

        public Ticket(double price)
            : this(nextId++, price)
        {
        }

        private Ticket(int id, double price)
        {
            this.Id = id;
            this.Price = price;
        }

        /// <summary>
        /// Utility function to populate an event with a random number of tickets.
        /// </summary>
        /// <param name="rng">A random number generator to determine the number and price of tickets for an event.</param>
        /// <param name="maxTickets">The maximum number of tickets per event</param>
        /// <param name="maxTicketPrice">The maximum price of each ticket.</param>
        /// <returns>A collection of randomly generated tickets.</returns>
        public static IList<Ticket> GenerateTickets(Random rng, int maxTickets=5, double maxTicketPrice=99.99)
        {
            var tickets = new List<Ticket>();
            var numTickets = rng.Next(maxTickets);
            for (int i = 0; i < numTickets; i++)
            {
                var price = Math.Max(1.0, rng.NextDouble() * maxTicketPrice);
                tickets.Add(new Ticket(price));
            }
            return tickets;
        }

        public override string ToString()
        {
            return String.Format("Ticket {0:D6} - ${1:00.00}", this.Id, this.Price);
        }
    }

    class SearchResult
    {
        public Event Event { get; private set; }
        public Ticket CheapestTicket { get; private set; }
        public int Distance { get; private set; }

        public SearchResult(Event ev, Ticket cheapestTicket, int distance)
        {
            this.Event = ev;
            this.CheapestTicket = cheapestTicket;
            this.Distance = distance;
        }

        public override string ToString()
        {
            return String.Format("{0} - ${1:00.00}, distance {2}", this.Event, this.CheapestTicket.Price, this.Distance);
        }
    }
}
