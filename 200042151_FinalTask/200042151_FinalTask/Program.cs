
using System;
using System.Collections.Generic;

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



}

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
}


public class Program
{
    public static void Main()
    {

    }
}