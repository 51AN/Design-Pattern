
using System;
using System.Collections.Generic;
using System.Linq;
using _200042151_FinalTask;



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