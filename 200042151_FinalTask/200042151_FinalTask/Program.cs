
using System;
using System.Collections.Generic;
using System.Linq;

//Entity Room
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

//Entity Center
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


//Main Program
public class Program
{
    public static void Main()
    {

    }
}