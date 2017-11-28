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
                return MaximumX - MinimumX + 1;
            }
        }

        public int WorldHeight
        {
            get
            {
                return MaximumY - MinimumY + 1;
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
            MinimumX = minX;
            MinimumY = minY;
            MaximumX = maxX;
            MaximumY = maxY;

            cells = new Cell[WorldWidth,WorldHeight];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell LocateCell(int x, int y)
        {
            return cells[OffsetXCoordinate(x), OffsetYCoordinate(y)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="numEvents"></param>
        /// <returns></returns>
        public IEnumerable<SearchResult> SearchNearbyEvents(int x, int y, int numEvents=5)
        {
            Cell c = this.LocateCell(x, y);
            return SearchNearbyEvents(c, numEvents);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="numEvents"></param>
        /// <returns></returns>
        private IEnumerable<SearchResult> SearchNearbyEvents(Cell cell, int numEvents=5)
        {
            IList<Cell> results = new List<Cell>();
            foreach (var currentCell in cells)
            {
                if (currentCell.Event != null && currentCell.Event.HasTicketsAvailable)
                {
                    results.Add(currentCell);
                }
            }
            //for (int x = 0; x < cells.GetLength(0); x++)
            //{
            //    for (int y = 0; y < cells.GetLength(1); y++)
            //    {
            //        var currentCell = cells[x, y];
                    
            //    }
            //}
            return results.OrderBy(c => c.CalculateDistanceToCell(cell)).Take(numEvents).Select(c =>
            {
                return new SearchResult(
                    c.Event, 
                    c.Event.FindCheapestTicket(), 
                    c.CalculateDistanceToCell(cell));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int OffsetXCoordinate(int x)
        {
            return x + Math.Abs(MinimumX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private int OffsetYCoordinate(int y)
        {
            return y + Math.Abs(MinimumY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rng"></param>
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
                    cells[OffsetXCoordinate(x), OffsetYCoordinate(y)] = new Cell(x, y, ev);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="maxTickets"></param>
        /// <param name="maxTicketPrice"></param>
        /// <returns></returns>
        public static IList<Ticket> GenerateTickets(Random rng, int maxTickets=2, double maxTicketPrice=99.99)
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
