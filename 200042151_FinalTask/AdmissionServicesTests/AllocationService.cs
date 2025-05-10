using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using _200042151_FinalTask;

namespace AdmissionServicesTests
{
    internal class AllocationServiceTests
    {
        private Center centerA;
        private Center centerB;
        private AllocationService service;

        [SetUp]
        public void Setup()
        {
            centerA = new Center("Center A");
            centerA.AddRoom(new Room("R1", "Building A", 2));
            centerA.AddRoom(new Room("R2", "Building A", 2));
            centerA.AddRoom(new Room("R3", "Building A", 2));

            centerB = new Center("Center B");
            centerB.AddRoom(new Room("R4", "Building B", 2));
            centerB.AddRoom(new Room("R5", "Building B", 2));

            // Reset singleton (Reflection or helper if needed)
            typeof(AllocationService)
                .GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                .SetValue(null, null);

            service = AllocationService.Instance;
            service.AddCenter(centerA);
            service.AddCenter(centerB);
            service.SetDistributionStrategy(new ThresholdBasedDistribution(minRoomsPerCenter: 2, threshold: 0.25));
        }

        [Test]
        public void TestSingletonEnforcement()
        {
            var secondInstance = AllocationService.Instance;
            Assert.AreSame(service, secondInstance);
        }

        [Test]
        public void TestStudentRoomAssignment()
        {
            service.ApplyStudent("S1");
            var totalOccupied = centerA.Rooms.Concat(centerB.Rooms).Sum(r => r.OccupiedSeats);
            Assert.AreEqual(1, totalOccupied);
        }

        [Test]
        public void TestRoomActivationThreshold()
        {
            for (int i = 0; i < 5; i++)
                service.ApplyStudent($"S{i}");

            int totalActive = centerA.Rooms.Count(r => r.IsActive) + centerB.Rooms.Count(r => r.IsActive);
            Assert.GreaterOrEqual(totalActive, 2); // Based on strategy requirement
        }

        [Test]
        public void TestAllRoomsExhaustedThrowsException()
        {
            int totalCapacity = centerA.Rooms.Sum(r => r.Capacity) + centerB.Rooms.Sum(r => r.Capacity);

            for (int i = 0; i < totalCapacity; i++)
                service.ApplyStudent($"S{i}");

            Assert.Throws<InvalidOperationException>(() => service.ApplyStudent("OverflowStudent"));
        }
    }
}
