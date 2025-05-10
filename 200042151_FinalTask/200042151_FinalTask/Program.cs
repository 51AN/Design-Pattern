
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

        //Migration Service
        var departments = new List<Department>
        {
            new Department("CSE", 1),
            new Department("EEE", 2),
            new Department("ME", 1)
        };

        var students = new List<Student>
        {
            new Student("S1", 1, new List<string>{ "CSE", "EEE", "ME" }),
            new Student("S2", 2, new List<string>{ "ME", "EEE" }),
            new Student("S3", 3, new List<string>{ "CSE", "EEE" }),
            new Student("S4", 4, new List<string>{ "CSE", "EEE" }),
            new Student("S5", 5, new List<string>{ "EEE" })
        };

        // Initialize and run the migration system with the default strategy
        var migrationSystem = new MigrationSystem(departments, students, new ConcreteMigrationStrategy());
        migrationSystem.RunMigration(3);  // Run for 3 migration calls

    }
}