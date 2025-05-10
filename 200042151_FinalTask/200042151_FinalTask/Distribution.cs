using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _200042151_FinalTask
{

    // Entity Room
    public class Room
    {
        public string Id { get; }
        public string Location { get; }
        public int Capacity { get; }
        public int OccupiedSeats { get; private set; }

        public bool IsActive { get; set; }

        public Room(string id, string location, int capacity)
        {
            Id = id;
            Location = location;
            Capacity = capacity;
            OccupiedSeats = 0;
            IsActive = false;
        }

        public bool HasAvailableSeat(double threshold)
        {
            double available = Capacity - OccupiedSeats;
            return (available / Capacity) > threshold;
        }

        public void AssignSeat()
        {
            if (OccupiedSeats >= Capacity)
                throw new InvalidOperationException("Room is full.");
            OccupiedSeats++;
        }

    }

    // Entity Center
    public class Center
    {
        public string Name { get; }
        public List<Room> Rooms { get; }

        public Center(string name)
        {
            Name = name;
            Rooms = new List<Room>();
        }

        public void AddRoom(Room room) => Rooms.Add(room);
        public List<Room> GetActiveRooms() => Rooms.Where(r => r.IsActive).ToList();

        public List<Room> GetInactiveRooms() => Rooms.Where(r => !r.IsActive).ToList();
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Strategy Pattern for distribution
    /////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Interface for the distribution strategy
    public interface IDistributionStrategy
    {
        Room AllocateRoom(List<Center> centers);
    }

    //concrete class
    public class ThresholdBasedDistribution : IDistributionStrategy
    {
        // Minimum number of rooms that must be active in a center
        private readonly int minRoomsPerCenter;
        // Percentage of empty seats required in a room to be considered for allocation
        private readonly double threshold;


        public ThresholdBasedDistribution(int minRoomsPerCenter, double threshold)
        {
            this.minRoomsPerCenter = minRoomsPerCenter;
            this.threshold = threshold;
        }

        public Room AllocateRoom(List<Center> centers)
        {
            // Randomize center order 
            var shuffledCenters = centers.OrderBy(c => Guid.NewGuid()).ToList();

            foreach (var center in shuffledCenters)
            {
                // Get currently active rooms in the center
                var activeRooms = center.GetActiveRooms();

                // Ensure at least the minimum number of active rooms is maintained
                if (activeRooms.Count < minRoomsPerCenter)
                {
                    // Activate additional rooms if available
                    var roomsToActivate = center.GetInactiveRooms()
                        .Take(minRoomsPerCenter - activeRooms.Count);

                    foreach (var room in roomsToActivate)
                        room.IsActive = true;

                    // Refresh the active room list after activation
                    activeRooms = center.GetActiveRooms();
                }


                // Check if no active rooms pass the threshold --> activate 1 more room
                var availableRooms = activeRooms
                    .Where(r => r.HasAvailableSeat(threshold))
                    .ToList();

                if (!availableRooms.Any())
                {
                    var nextInactive = center.GetInactiveRooms().FirstOrDefault();
                    if (nextInactive != null)
                    {
                        nextInactive.IsActive = true;
                        activeRooms = center.GetActiveRooms();
                        availableRooms = activeRooms
                            .Where(r => r.HasAvailableSeat(threshold))
                            .ToList();
                    }
                }

                // Randomize and allocate
                foreach (var room in availableRooms.OrderBy(r => Guid.NewGuid()))
                    return room;
            }

            // If no room is found, throw an exception
            throw new InvalidOperationException("No suitable room found for allocation. All active rooms are full or unavailable.");
        }
    }

}
