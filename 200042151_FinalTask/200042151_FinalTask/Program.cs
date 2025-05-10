
using System;
using System.Collections.Generic;
using System.Linq;

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

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Singleton Pattern for Allocation
/////////////////////////////////////////////////////////////////////////////////////////////////////////

public class AllocationService
{
    // Singleton instance of AllocationService to ensure a single point of control
    private static AllocationService? _instance;

    // Public accessor to get the singleton instance
    public static AllocationService Instance => _instance ??= new AllocationService();

    private IDistributionStrategy _distributionStrategy;

    private List<Center> _centers;

    private AllocationService()
    {
        _centers = new List<Center>();
    }

    // Set the distribution strategy
    public void SetDistributionStrategy(IDistributionStrategy strategy)
    {
        _distributionStrategy = strategy;
    }

    // Add a new center to the pool of available centers
    public void AddCenter(Center center)
    {
        _centers.Add(center);
    }

    // Allocate a room to a student
    public void ApplyStudent(string studentId)
    {
        // Use the strategy to get an appropriate room
        Room room = _distributionStrategy.AllocateRoom(_centers);
        room.AssignSeat();

        Console.WriteLine($"Student {studentId} assigned to Room {room.Id} in {room.Location}");
    }
}


//Main Program
public class Program
{
    public static void Main()
    {
        var centerA = new Center("Center A");
        centerA.AddRoom(new Room("R1", "Building A", 10));
        centerA.AddRoom(new Room("R2", "Building A", 10));
        centerA.AddRoom(new Room("R3", "Building A", 10));
        centerA.AddRoom(new Room("R4", "Building A", 10));

        var centerB = new Center("Center B");
        centerB.AddRoom(new Room("R5", "Building B", 10));
        centerB.AddRoom(new Room("R6", "Building B", 10));
        centerB.AddRoom(new Room("R7", "Building B", 10));

        var service = AllocationService.Instance;
        service.AddCenter(centerA);
        service.AddCenter(centerB);

        service.SetDistributionStrategy(new ThresholdBasedDistribution(minRoomsPerCenter: 3, threshold: 0.15));

        for (int i = 0; i < 60; i++)
        {
            service.ApplyStudent($"S{i + 1}");
        }

    }
}