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

            // Reset singleton 
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

        [Test]
        public void TestRoomAssignmentRespectsThreshold()
        {
            // Fill up a room beyond the threshold so it shouldn't be selected
            var room = centerA.Rooms.First();
            room.IsActive = true;
            room.AssignSeat();
            room.AssignSeat(); // Fully occupied (2/2), threshold 0.25 will fail

            service.ApplyStudent("S1");

            // Room should not have been selected, total occupied should be 3
            int totalOccupied = centerA.Rooms.Concat(centerB.Rooms).Sum(r => r.OccupiedSeats);
            Assert.AreEqual(3, totalOccupied);
        }

        [Test]
        public void TestInactiveRoomStaysInactiveIfNotNeeded()
        {
            // Already 2 active rooms should be enough per minRoomsPerCenter
            centerA.Rooms[0].IsActive = true;
            centerA.Rooms[1].IsActive = true;
            centerA.Rooms[2].IsActive = false;

            service.ApplyStudent("S1");

            Assert.IsFalse(centerA.Rooms[2].IsActive); // R3 should not have been activated
        }

        [Test]
        public void TestNewRoomActivatedWhenThresholdNotMet()
        {
            // Fill up the only active room to make threshold fail
            var r1 = centerA.Rooms[0];
            r1.IsActive = true;
            r1.AssignSeat(); r1.AssignSeat(); // Full

            // Only one active room and it's full — should activate another
            service.ApplyStudent("S1");

            Assert.IsTrue(centerA.Rooms.Any(r => r.IsActive && r != r1));
        }

        [Test]
        public void TestMultipleStudentsAssignedToDifferentRooms()
        {
            service.ApplyStudent("S1");
            service.ApplyStudent("S2");
            service.ApplyStudent("S3");

            var occupiedRooms = centerA.Rooms.Concat(centerB.Rooms).Where(r => r.OccupiedSeats > 0).ToList();

            Assert.AreEqual(3, occupiedRooms.Count);
        }

        [Test]
        public void TestApplyStudentIncrementsOccupiedSeats()
        {
            var before = centerA.Rooms.Concat(centerB.Rooms).Sum(r => r.OccupiedSeats);

            service.ApplyStudent("S1");

            var after = centerA.Rooms.Concat(centerB.Rooms).Sum(r => r.OccupiedSeats);

            Assert.AreEqual(before + 1, after);
        }

    }
}
